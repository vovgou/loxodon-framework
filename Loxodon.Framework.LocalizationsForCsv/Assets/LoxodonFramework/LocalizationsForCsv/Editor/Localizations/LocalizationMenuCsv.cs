using CsvHelper;
using Loxodon.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Loxodon.Framework.Editors
{
    public static class LocalizationMenuCsv
    {
        private const string XML_OUTPUT_DIR_KEY = "_LOXODON_LOCALIZATION_XML_OUTPUT_DIR_KEY";
        private const string CSV_OUTPUT_DIR_KEY = "_LOXODON_LOCALIZATION_CSV_OUTPUT_DIR_KEY";
        private const string DEFAULT_OUTPUT_DIR = "Assets";

        private const string XML2CSV_MENU_NAME = "Assets/Loxodon/Xml To Csv";
        private const string CSV2XML_MENU_NAME = "Assets/Loxodon/Csv To Xml";

        private const string XML_EXTENSION = ".xml";
        private const string CSV_EXTENSION = ".csv";

        private readonly static char[] XML_SPECIAL_CHARS = new char[] { '<', '>', '&' };
        private readonly static char[] CSV_SPECIAL_CHARS = new char[] { ',' };

        [MenuItem(XML2CSV_MENU_NAME, false, 0)]
        static void Xml2Csv()
        {
            var selections = Selection.objects;
            if (selections == null || selections.Length <= 0)
                return;

            var dir = EditorPrefs.GetString(CSV_OUTPUT_DIR_KEY, DEFAULT_OUTPUT_DIR);
            if (!Directory.Exists(dir))
                dir = DEFAULT_OUTPUT_DIR;

            string location = EditorUtility.OpenFolderPanel("Please select the storage directory", dir, "");
            if (string.IsNullOrEmpty(location))
                return;

            dir = GetRelativeDirectory(location);
            EditorPrefs.SetString(CSV_OUTPUT_DIR_KEY, dir);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            ISet<string> files = GetSelections(XML_EXTENSION);
            if (files == null || files.Count <= 0)
                return;

            Dictionary<string, DataTable> dict = new Dictionary<string, DataTable>();
            foreach (var file in files)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(file);
                    string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    string localeName = fileInfo.Directory.Name;

                    DataTable dataTable = null;
                    if (!dict.TryGetValue(fileName, out dataTable))
                    {
                        dataTable = new DataTable();
                        dict.Add(fileName, dataTable);
                    }

                    TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(file);
                    ReadXml(new MemoryStream(textAsset.bytes), dataTable, localeName);
                }
                catch (Exception)
                {
                }
            }

            foreach (var kv in dict)
            {
                string fileName = kv.Key;
                DataTable dataTable = kv.Value;
                using (MemoryStream output = new MemoryStream())
                {
                    WriteCsv(output, dataTable);
                    File.WriteAllBytes(string.Format("{0}/{1}{2}", dir, fileName, CSV_EXTENSION), output.ToArray());
                }
            }
            AssetDatabase.Refresh();
        }


        [MenuItem(CSV2XML_MENU_NAME, false, 0)]
        static void Csv2Xml()
        {
            var selections = Selection.objects;
            if (selections == null || selections.Length <= 0)
                return;

            var dir = EditorPrefs.GetString(XML_OUTPUT_DIR_KEY, DEFAULT_OUTPUT_DIR);
            if (!Directory.Exists(dir))
                dir = DEFAULT_OUTPUT_DIR;

            string location = EditorUtility.OpenFolderPanel("Please select the storage directory", dir, "");
            if (string.IsNullOrEmpty(location))
                return;

            dir = GetRelativeDirectory(location);
            EditorPrefs.SetString(XML_OUTPUT_DIR_KEY, dir);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            ISet<string> files = GetSelections(CSV_EXTENSION);
            if (files == null || files.Count <= 0)
                return;

            Dictionary<string, DataTable> dict = new Dictionary<string, DataTable>();
            foreach (var file in files)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(file);
                    string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    DataTable dataTable = null;
                    if (!dict.TryGetValue(fileName, out dataTable))
                    {
                        dataTable = new DataTable();
                        dict.Add(fileName, dataTable);
                    }

                    TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(file);
                    ReadCsv(new MemoryStream(textAsset.bytes), dataTable);
                }
                catch (Exception)
                {
                }
            }

            foreach (var kv in dict)
            {
                string fileName = kv.Key;
                DataTable dataTable = kv.Value;

                foreach (DataColumn column in dataTable.Columns)
                {
                    if (Regex.IsMatch(column.ColumnName, @"^((key)|(type)|(description))$", RegexOptions.IgnoreCase))
                        continue;

                    using (MemoryStream output = new MemoryStream())
                    {
                        WriteXml(output, dataTable, column.ColumnName);
                        FileInfo fileInfo = new FileInfo(string.Format("{0}/{1}/{2}{3}", dir, column.ColumnName, fileName, XML_EXTENSION));
                        if (!fileInfo.Directory.Exists)
                            fileInfo.Directory.Create();

                        File.WriteAllBytes(fileInfo.FullName, output.ToArray());
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        [MenuItem(XML2CSV_MENU_NAME, true)]
        static bool IsValidatedXml()
        {
            return IsValidated(XML_EXTENSION);
        }

        [MenuItem(CSV2XML_MENU_NAME, true)]
        static bool IsValidatedCsv()
        {
            return IsValidated(CSV_EXTENSION);
        }

        static string GetRelativeDirectory(string dir)
        {
            int start = dir.LastIndexOf("Assets");
            if (start < 0)
                return "Assets";

            return dir.Substring(start);
        }

        static void ReadXml(Stream input, DataTable dataTable, string columnName)
        {
            var columns = dataTable.Columns;
            if (columns.Count <= 0)
            {
                columns.Add("key", typeof(string));
                columns.Add("type", typeof(string));
                dataTable.PrimaryKey = new DataColumn[] { columns["key"] };
            }

            if (!columns.Contains(columnName))
                columns.Add(columnName, typeof(string));

            var rows = dataTable.Rows;
            using (XmlTextReader reader = new XmlTextReader(input))
            {
                string elementName = null;
                string key = null;
                string value = null;
                List<string> list = new List<string>();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            elementName = reader.Name;
                            if (string.IsNullOrEmpty(elementName) || elementName.Equals("resources"))
                                break;

                            if (elementName.Equals("item"))
                            {
                                //array item
                                var content = reader.ReadElementString();
                                list.Add(content);
                                break;
                            }

                            key = reader.GetAttribute("name");
                            if (string.IsNullOrEmpty(key))
                                throw new XmlException("The attribute of name is null.");

                            if (elementName.EndsWith("-array"))
                            {
                                //array item
                                list.Clear();
                                break;
                            }

                            value = reader.ReadElementString();
                            if (rows.Contains(key))
                            {
                                var dataRow = rows.Find(key);
                                dataRow[columnName] = value;
                            }
                            else
                            {
                                var dataRow = dataTable.NewRow();
                                dataRow["key"] = key;
                                dataRow["type"] = elementName;
                                dataRow[columnName] = value;
                                rows.Add(dataRow);
                            }
                            break;
                        case XmlNodeType.EndElement:
                            elementName = reader.Name;
                            if (!string.IsNullOrEmpty(elementName) && elementName.EndsWith("-array"))
                            {
                                //array
                                bool hasSpecialChar = list.Exists(v => !string.IsNullOrEmpty(v) && ((!Regex.IsMatch(v.Trim(), @"^((\(\S*\))|(\[\S*\])|(\{\S*\})|(<\S*>))$") && v.IndexOfAny(CSV_SPECIAL_CHARS) != -1) || (v.IndexOf('"') != -1)));
                                StringBuilder buf = new StringBuilder();
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (hasSpecialChar)
                                        buf.AppendFormat("\"{0}\"", list[i].Replace("\"", "&quot;"));
                                    else
                                        buf.Append(list[i]);

                                    if (i < list.Count - 1)
                                        buf.Append(",");
                                }
                                value = buf.ToString();
                                list.Clear();

                                if (rows.Contains(key))
                                {
                                    var dataRow = rows.Find(key);
                                    dataRow[columnName] = value;
                                }
                                else
                                {
                                    var dataRow = dataTable.NewRow();
                                    dataRow["key"] = key;
                                    dataRow["type"] = elementName;
                                    dataRow[columnName] = value;
                                    rows.Add(dataRow);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        static void WriteXml(Stream output, DataTable dataTable, string columnName)
        {
            using (XmlTextWriter writer = new XmlTextWriter(output, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("resources");
                foreach (DataRow row in dataTable.Rows)
                {
                    string key = (string)row["key"];
                    string type = (string)row["type"];
                    string value = row[columnName] is DBNull ? null : (string)row[columnName];

                    if (type.EndsWith("-array"))
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            writer.WriteStartElement(type);
                            writer.WriteAttributeString("name", key);

                            string[] values = StringSpliter.Split(value, CSV_SPECIAL_CHARS);
                            foreach (string v in values)
                            {
                                writer.WriteStartElement("item");
                                if (!string.IsNullOrEmpty(v) & v.IndexOfAny(XML_SPECIAL_CHARS) != -1)
                                {
                                    writer.WriteCData(v);
                                }
                                else
                                {
                                    writer.WriteValue(v);
                                }
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            writer.WriteStartElement(type);
                            writer.WriteAttributeString("name", key);
                            if (value.IndexOfAny(XML_SPECIAL_CHARS) != -1)
                            {
                                writer.WriteCData(value);
                            }
                            else
                            {
                                writer.WriteValue(value);
                            }
                            writer.WriteEndElement();
                        }
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        static void ReadCsv(Stream input, DataTable dataTable)
        {
            using (CsvReader reader = new CsvReader(new StreamReader(input, Encoding.UTF8)))
            {
                using (var dataReader = new CsvDataReader(reader))
                {
                    dataTable.Load(dataReader);
                }
            }
        }

        static void WriteCsv(Stream output, DataTable dataTable)
        {
            using (CsvWriter writer = new CsvWriter(new StreamWriter(output, Encoding.UTF8)))
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    writer.WriteField(column.ColumnName);
                }
                writer.NextRecord();

                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (object item in row.ItemArray)
                    {
                        writer.WriteField(item);
                    }
                    writer.NextRecord();
                }
            }
        }

        static bool IsValidated(string extension)
        {
            var selections = Selection.objects;
            if (selections == null || selections.Length <= 0)
                return false;

            foreach (var s in selections)
            {
                string path = AssetDatabase.GetAssetPath(s);
                if (s is DefaultAsset && Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path, "*" + extension, SearchOption.AllDirectories);
                    if (files != null && files.Length > 0)
                        return true;
                }
                else if (s is TextAsset)
                {
                    if (path.ToLower().EndsWith(extension))
                        return true;
                }
            }
            return false;
        }

        static ISet<string> GetSelections(string extension)
        {
            HashSet<string> set = new HashSet<string>();
            var selections = Selection.objects;
            if (selections == null || selections.Length <= 0)
                return null;

            foreach (var s in selections)
            {
                string path = AssetDatabase.GetAssetPath(s);
                if (s is DefaultAsset && Directory.Exists(path))
                {
                    foreach (string file in Directory.GetFiles(path, "*" + extension, SearchOption.AllDirectories))
                    {
                        set.Add(file);
                    }
                }
                else if (s is TextAsset)
                {
                    if (path.ToLower().EndsWith(extension))
                        set.Add(path);
                }
            }
            return set;
        }
    }
}
