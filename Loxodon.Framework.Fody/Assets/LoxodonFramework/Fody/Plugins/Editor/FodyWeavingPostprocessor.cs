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

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Loxodon.Framework.Fody.Editos
{
    public static class FodyWeavingPostprocessor
    {
        private const string CONFIG_PATH_DIR = "Assets/LoxodonFramework/Editor/AppData/Fody/";
        private const string CONFIG_PATH = CONFIG_PATH_DIR + "FodyWeavers.xml";
        private const string DEFAULT_CONFIG_DIR = "Assets/LoxodonFramework/Fody/Plugins/Editor/";
        private const string DEFAULT_CONFIG_PACKAGES_DIR = "Packages/com.vovgou.loxodon-framework-fody/Plugins/Editor/";
        private const string DEFAULT_CONFIG_TEMPLATE_NAME = "FodyWeavers.template.xml";
        private const string ASSEMBLIES_ROOT_PATH = @"Library\ScriptAssemblies\";
        private const string DEFAULT_ASSEMBLIE_FILENAME = "Assembly-CSharp";
        private const string ASSEMBLIE_FILENAME_SUFFIX = ".dll";
        [DidReloadScripts]
        public static void OnScriptsReloaded()
        {
            DoWeave();
        }

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            DoWeave();
        }

        private static void DoWeave()
        {
            if (!File.Exists(CONFIG_PATH))
            {
                AssetDatabase.StartAssetEditing();
                if (!Directory.Exists(CONFIG_PATH_DIR))
                    Directory.CreateDirectory(CONFIG_PATH_DIR);

                if (File.Exists(DEFAULT_CONFIG_PACKAGES_DIR + DEFAULT_CONFIG_TEMPLATE_NAME))
                    File.Copy(DEFAULT_CONFIG_PACKAGES_DIR + DEFAULT_CONFIG_TEMPLATE_NAME, CONFIG_PATH);
                else if (File.Exists(DEFAULT_CONFIG_DIR + DEFAULT_CONFIG_TEMPLATE_NAME))
                    File.Copy(DEFAULT_CONFIG_DIR + DEFAULT_CONFIG_TEMPLATE_NAME, CONFIG_PATH);

                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }

            List<string> assemblyNames = new List<string>();
            XElement config = XElement.Load(CONFIG_PATH);
            XElement assemblyNamesNode = config.Element(XName.Get("AssemblyNames"));
            if (assemblyNamesNode != null)
            {
                foreach (var node in assemblyNamesNode.Elements())
                {
                    string assemblyName = node.Value;
                    if (string.IsNullOrEmpty(assemblyName))
                        continue;

                    assemblyNames.Add(assemblyName);
                }
                assemblyNamesNode.Remove();
            }

            if (assemblyNames.Count <= 0)
                assemblyNames.Add(DEFAULT_ASSEMBLIE_FILENAME);

            string weaverAssemblyRoot = DEFAULT_CONFIG_PACKAGES_DIR;
            if (Directory.Exists(DEFAULT_CONFIG_PACKAGES_DIR))
                weaverAssemblyRoot = DEFAULT_CONFIG_PACKAGES_DIR;
            else if (Directory.Exists(DEFAULT_CONFIG_DIR))
                weaverAssemblyRoot = DEFAULT_CONFIG_DIR;

            foreach (string name in assemblyNames)
            {
                string assemblyFilePath = ASSEMBLIES_ROOT_PATH + name;
                if (!assemblyFilePath.EndsWith(ASSEMBLIE_FILENAME_SUFFIX, System.StringComparison.OrdinalIgnoreCase))
                    assemblyFilePath += ASSEMBLIE_FILENAME_SUFFIX;

                if (!File.Exists(assemblyFilePath))
                    continue;

                WeavingTask weaver = new WeavingTask(assemblyFilePath, weaverAssemblyRoot, config);
                weaver.Execute();
                Debug.LogFormat("Weaving code succeeded for {0}.dll", name);
            }
        }
    }
}