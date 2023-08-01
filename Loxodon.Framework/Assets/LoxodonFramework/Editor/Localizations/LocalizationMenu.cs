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

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Loxodon.Framework.Localizations;

namespace Loxodon.Framework.Editors
{
    public static class LocalizationMenu
    {
        private const string MENU_NAME = "Assets/Loxodon/Localization Make";
        private const string CLASS_NAME_KEY = "_LOXODON_LOCALIZATION_CLASS_NAME_KEY";
        private const string OUTPUT_DIR_KEY = "_LOXODON_LOCALIZATION_OUTPUT_DIR_KEY";
        private const string DEFAULT_OUTPUT_DIR = "Assets/Gen/";
        private const string DEFAULT_CLASS_NAME = "R";
        private const string EXTENSION = ".xml";

        [MenuItem(MENU_NAME, false, 0)]
        static void Generate()
        {
            var selections = Selection.objects;
            if (selections == null || selections.Length <= 0)
                return;

            var dir = EditorPrefs.GetString(OUTPUT_DIR_KEY, DEFAULT_OUTPUT_DIR);
            var className = EditorPrefs.GetString(CLASS_NAME_KEY, DEFAULT_CLASS_NAME);
            string location = EditorUtility.SaveFilePanel("Generate Code", dir, className, "cs");
            if (string.IsNullOrEmpty(location))
                return;

            dir = GetRelativeDirectory(location);
            className = GetClassName(location);

            EditorPrefs.SetString(OUTPUT_DIR_KEY, dir);
            EditorPrefs.SetString(CLASS_NAME_KEY, className);

            CodeGenerator generator = new CodeGenerator();
            XmlDocumentParser parser = new XmlDocumentParser();
            Dictionary<string, object> data = new Dictionary<string, object>();
            ISet<string> files = GetSelections(EXTENSION);
            if (files == null || files.Count <= 0)
                return;

            foreach (var file in files)
            {
                try
                {
                    TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(file);
                    var dict = parser.Parse(new MemoryStream(textAsset.bytes), Locale.GetCultureInfo());
                    foreach (KeyValuePair<string, object> kv in dict)
                    {
                        data[kv.Key] = kv.Value;
                    }
                }
                catch (Exception)
                {
                }
            }
            if (data.Count <= 0)
                return;

            var code = generator.Generate(className, data);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(location, code);

            AssetDatabase.Refresh();
        }

        static string GetRelativeDirectory(string location)
        {
            int start = location.LastIndexOf("Assets");
            if (start < 0)
                return "Assets";

            int end = location.LastIndexOf("/") + 1;
            return location.Substring(start, (end - start));
        }

        static string GetClassName(string location)
        {
            int start = location.LastIndexOf("/") + 1;
            int end = location.LastIndexOf(".cs");
            return location.Substring(start, (end - start));
        }

        static ISet<string> GetSelections(string extension)
        {
            HashSet<string> set = new HashSet<string>();
            var selections = Selection.objects;
            if (selections == null || selections.Length <= 0)
                return set;
            
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

        [MenuItem(MENU_NAME, true)]
        static bool IsValidated()
        {
            var selections = Selection.objects;
            if (selections == null || selections.Length <= 0)
                return false;

            foreach (var s in selections)
            {
                string path = AssetDatabase.GetAssetPath(s);
                if (s is DefaultAsset && Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path, "*" + EXTENSION, SearchOption.AllDirectories);
                    if (files != null && files.Length > 0)
                        return true;
                }
                else if (s is TextAsset)
                {
                    if (path.ToLower().EndsWith(EXTENSION))
                        return true;
                }
            }
            return false;
        }
    }
}

