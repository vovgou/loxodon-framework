/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using Loxodon.Framework.Utilities;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Loxodon.Framework.Data.Editors
{
    public class NPOISheetReader : ISheetReader
    {
        protected static readonly char[] ITEM_SEPARATOR = new char[] { ';', ',', '；', '，' };
        protected static readonly char[] KEY_VALUE_SEPARATOR = new char[] { '=' };
        protected static readonly Regex INDEX_REGEX = new Regex("\\((index|unique)\\)$");
        protected ISheet sheet;

        protected int columnNameLineNo;
        protected int typeNameLineNo;
        protected int commentLineNo;
        protected int dataStartLineNo;
        protected int totalCount;

        protected List<ColumnInfo> columnInfos;

        public NPOISheetReader(ISheet sheet, int columnNameLineNo = 0, int typeNameLineNo = 1, int commentLineNo = 2, int dataStartLineNo = 3)
        {
            this.sheet = sheet;
            this.columnNameLineNo = columnNameLineNo;
            this.typeNameLineNo = typeNameLineNo;
            this.commentLineNo = commentLineNo;
            this.dataStartLineNo = dataStartLineNo;
            this.totalCount = sheet.LastRowNum;

            if (this.totalCount < this.dataStartLineNo)
                throw new Exception(string.Format("The sheet \"{0}\" is missing the header or the content is empty", sheet.SheetName));

            this.columnInfos = this.ReadHeader();
        }

        public string Name { get { return this.sheet.SheetName; } }

        public int StartLine { get { return this.dataStartLineNo; } }

        public int TotalCount { get { return this.totalCount; } }

        public List<ColumnInfo> Headers { get { return this.columnInfos; } }

        public virtual DataRecord ReadLine(int rowNum)
        {
            if (rowNum < this.dataStartLineNo || rowNum > totalCount)
                throw new Exception(string.Format("Line number must be greater than or equal to {0} and less than or equal to {1}", dataStartLineNo, totalCount));

            if (columnInfos.Count <= 0)
                return null;

            IRow row = sheet.GetRow(rowNum);

            DataRecord record = new DataRecord();
            bool isNullLine = true;
            foreach (var info in columnInfos)
            {
                bool isNull = false;
                var value = ReadValue(row, info, out isNull);
                AddValueToDict(record, info.ColumnName, value);
                if (!isNull)
                    isNullLine = false;
            }

            if (isNullLine)
                return null;

            return record;
        }

        protected virtual string Trim(string input)
        {
            return Regex.Replace(input, @"(^\s+)|(\s+$)", "");
        }

        protected virtual object ReadValue(IRow row, ColumnInfo info, out bool isNull)
        {
            ICell cell = row.GetCell(info.ColumnNo);
            string str = cell != null ? cell.ToString() : null;
            isNull = string.IsNullOrWhiteSpace(str);
            try
            {
                switch (info.TypeName)
                {
                    case "string":
                        return str;
                    case "int":
                        if (isNull)
                            return 0;
                        return Convert.ToInt32(str);
                    case "float":
                        if (isNull)
                            return 0f;
                        return Convert.ToSingle(str);
                    case "bool":
                        return ToBoolean(str);
                    case "string[]":
                        {
                            if (isNull)
                                return new string[0];

                            string[] values = StringSpliter.Split(str, ITEM_SEPARATOR, StringSplitOptions.RemoveEmptyEntries); ;
                            string[] result = new string[values.Length];
                            for (int i = 0; i < values.Length; i++)
                                result[i] = Trim(values[i]);
                            return result;
                        }
                    case "int[]":
                        {
                            if (isNull)
                                return new int[0];

                            string[] values = str.Split(ITEM_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                            int[] result = new int[values.Length];
                            for (int i = 0; i < values.Length; i++)
                                result[i] = Convert.ToInt32(Trim(values[i]));
                            return result;
                        }
                    case "float[]":
                        {
                            if (isNull)
                                return new float[0];

                            string[] values = str.Split(ITEM_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                            float[] result = new float[values.Length];
                            for (int i = 0; i < values.Length; i++)
                                result[i] = Convert.ToSingle(Trim(values[i]));
                            return result;
                        }
                    case "bool[]":
                        {
                            if (isNull)
                                return new bool[0];

                            string[] values = str.Split(ITEM_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                            bool[] result = new bool[values.Length];
                            for (int i = 0; i < values.Length; i++)
                                result[i] = ToBoolean(Trim(values[i]));
                            return result;
                        }
                    case "string{}":
                        {
                            Dictionary<string, string> result = new Dictionary<string, string>();
                            if (isNull)
                                return result;

                            string[] values = StringSpliter.Split(str, ITEM_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < values.Length; i++)
                            {
                                string[] keyValueStr = StringSpliter.Split(values[i], KEY_VALUE_SEPARATOR);
                                if (keyValueStr.Length != 2 || string.IsNullOrEmpty(keyValueStr[0]))
                                    throw new FormatException(string.Format("{0}", str));

                                string key = Trim(keyValueStr[0]);
                                string value = Trim(keyValueStr[1]);
                                if (result.ContainsKey(key))
                                    throw new FormatException(string.Format("Duplicate key:{0}", key));

                                result.Add(key, value);
                            }
                            return result;
                        }
                    case "int{}":
                        {
                            Dictionary<string, int> result = new Dictionary<string, int>();
                            if (isNull)
                                return result;

                            string[] values = str.Split(ITEM_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < values.Length; i++)
                            {
                                string[] keyValueStr = values[i].Split(KEY_VALUE_SEPARATOR);
                                if (keyValueStr.Length != 2 || string.IsNullOrEmpty(keyValueStr[0]))
                                    throw new FormatException(string.Format("{0}", str));

                                string key = Trim(keyValueStr[0]);
                                int value = Convert.ToInt32(Trim(keyValueStr[1]));
                                if (result.ContainsKey(key))
                                    throw new FormatException(string.Format("Duplicate key:{0}", key));

                                result.Add(key, value);
                            }
                            return result;
                        }
                    case "float{}":
                        {
                            Dictionary<string, float> result = new Dictionary<string, float>();
                            if (isNull)
                                return result;

                            string[] values = str.Split(ITEM_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < values.Length; i++)
                            {
                                string[] keyValueStr = values[i].Split(KEY_VALUE_SEPARATOR);
                                if (keyValueStr.Length != 2 || string.IsNullOrEmpty(keyValueStr[0]))
                                    throw new FormatException(string.Format("{0}", str));

                                string key = Trim(keyValueStr[0]);
                                float value = Convert.ToSingle(Trim(keyValueStr[1]));
                                if (result.ContainsKey(key))
                                    throw new FormatException(string.Format("Duplicate key:{0}", key));

                                result.Add(key, value);
                            }
                            return result;
                        }
                    case "bool{}":
                        {
                            Dictionary<string, bool> result = new Dictionary<string, bool>();
                            if (isNull)
                                return result;

                            string[] values = str.Split(ITEM_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < values.Length; i++)
                            {
                                string[] keyValueStr = values[i].Split(KEY_VALUE_SEPARATOR);
                                if (keyValueStr.Length != 2 || string.IsNullOrEmpty(keyValueStr[0]))
                                    throw new FormatException(string.Format("{0}", str));

                                string key = Trim(keyValueStr[0]);
                                bool value = ToBoolean(Trim(keyValueStr[1]));
                                if (result.ContainsKey(key))
                                    throw new FormatException(string.Format("Duplicate key:{0}", key));

                                result.Add(key, value);
                            }
                            return result;
                        }
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                throw new FormatException(string.Format("In the address \"row:{0} column:{1}\" of sheet \"{2}\", the input string \"{3}\" is incorrect.", cell.RowIndex + 1, cell.ColumnIndex + 1, sheet.SheetName, str), e);
            }
        }

        protected virtual List<ColumnInfo> ReadHeader()
        {
            List<ColumnInfo> infos = new List<ColumnInfo>();
            if (sheet.LastRowNum <= 0)
                return infos;

            var firstRow = sheet.GetRow(0);
            for (int i = firstRow.FirstCellNum; i < firstRow.LastCellNum; i++)
            {
                ICell colunmNameCell = sheet.GetRow(this.columnNameLineNo).GetCell(i);
                ICell typeNameCell = sheet.GetRow(this.typeNameLineNo).GetCell(i);
                ICell commentCell = this.commentLineNo >= 0 ? sheet.GetRow(this.commentLineNo).GetCell(i) : null;

                string colunmName = colunmNameCell != null ? colunmNameCell.StringCellValue : string.Empty;
                string typeNameAndIndex = typeNameCell != null ? typeNameCell.StringCellValue : string.Empty;
                string comment = commentCell != null ? commentCell.StringCellValue : string.Empty;

                if (string.IsNullOrWhiteSpace(colunmName) || string.IsNullOrWhiteSpace(typeNameAndIndex))
                    continue;

                typeNameAndIndex = typeNameAndIndex.ToLower().Trim();
                string typeName = ParseTypeName(typeNameAndIndex);
                if (!Support(typeName))
                    continue;

                IndexType indexType = ParseIndexType(typeNameAndIndex);
                infos.Add(new ColumnInfo(i, colunmName.Trim(), typeName, comment, indexType));
            }
            return infos;
        }

        protected virtual string ParseTypeName(string str)
        {
            return INDEX_REGEX.Replace(str, "");
        }

        protected virtual IndexType ParseIndexType(string str)
        {
            Match match = INDEX_REGEX.Match(str);
            if (match.Success)
            {
                if ("unique".Equals(match.Groups[1].Value))
                    return IndexType.Unique;
                return IndexType.Index;
            }
            return IndexType.None;
        }

        protected virtual bool Support(string typeName)
        {
            switch (typeName)
            {
                case "string":
                case "int":
                case "float":
                case "bool":
                case "string[]":
                case "int[]":
                case "float[]":
                case "bool[]":
                case "string{}":
                case "int{}":
                case "float{}":
                case "bool{}":
                    return true;
                default:
                    return false;
            }
        }

        protected virtual bool ToBoolean(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            str = str.Trim().ToLower();
            int value = 0;
            if (Int32.TryParse(str, out value))
                return value > 0;

            return Convert.ToBoolean(str);
        }

        protected virtual void AddValueToDict(Dictionary<string, object> dict, string path, object value)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new FormatException(string.Format("The column name is null or empty"));

            int index = path.IndexOf('.');
            if (index == 0)
                throw new FormatException(string.Format("Illegal column name:{0}", path));

            if (index < 0)
            {
                dict.Add(path, value);
                return;
            }

            string key = path.Substring(0, index);
            string childPath = path.Substring(index + 1);
            Dictionary<string, object> childDict;
            if (!dict.ContainsKey(key))
            {
                childDict = new Dictionary<string, object>();
                dict.Add(key, childDict);
            }
            else
            {
                childDict = (Dictionary<string, object>)dict[key];
            }
            AddValueToDict(childDict, childPath, value);
        }
    }
}
