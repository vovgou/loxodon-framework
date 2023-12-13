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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;

namespace Loxodon.Framework.Fody
{
    public class FodyWeaver : IWeaver
    {
        protected const string CONFIG_DIR = "Assets/LoxodonFramework/Editor/AppData/Fody/";
        protected const string CONFIG_FULLNAME = "Assets/LoxodonFramework/Editor/AppData/Fody/FodyWeavers.xml";
        protected const string DEFAULT_CONFIG_TEMPLATE_DIR = "Assets/LoxodonFramework/Fody/Plugins/Editor/";
        protected const string PACKAGES_CONFIG_TEMPLATE_DIR = "Packages/com.vovgou.loxodon-framework-fody/Plugins/Editor/";
        protected const string DEFAULT_CONFIG_TEMPLATE_FILENAME = "FodyWeavers.template.xml";
        protected const string ASSEMBLIES_EDITOR_LIB_PATH = "Library/ScriptAssemblies/";
        protected const string ASSEMBLIES_BUILD_TEMP_PATH = "Temp/StagingArea/Data/Managed/";
        protected const string DEFAULT_ASSEMBLIE_FILENAME = "Assembly-CSharp";
        protected const string ASSEMBLIE_FILENAME_SUFFIX = ".dll";
        public static FodyWeaver Default = new FodyWeaver();

        public virtual void Weave(string assembliesRootPath)
        {
            if (Regex.IsMatch(Application.dataPath, @"[\u4e00-\u9fbb]+"))
                throw new Exception("Chinese/Unicode characters are not allowed in the project path.");

            if (!File.Exists(CONFIG_FULLNAME))
            {
                if (!Directory.Exists(CONFIG_DIR))
                    Directory.CreateDirectory(CONFIG_DIR);

                if (File.Exists(PACKAGES_CONFIG_TEMPLATE_DIR + DEFAULT_CONFIG_TEMPLATE_FILENAME))
                    File.Copy(PACKAGES_CONFIG_TEMPLATE_DIR + DEFAULT_CONFIG_TEMPLATE_FILENAME, CONFIG_FULLNAME);
                else if (File.Exists(DEFAULT_CONFIG_TEMPLATE_DIR + DEFAULT_CONFIG_TEMPLATE_FILENAME))
                    File.Copy(DEFAULT_CONFIG_TEMPLATE_DIR + DEFAULT_CONFIG_TEMPLATE_FILENAME, CONFIG_FULLNAME);
            }

            if (string.IsNullOrEmpty(assembliesRootPath))
                assembliesRootPath = ASSEMBLIES_EDITOR_LIB_PATH;

            assembliesRootPath = assembliesRootPath.Replace(@"\", "/");
            if (!assembliesRootPath.EndsWith("/"))
                assembliesRootPath += "/";

            string weaverAssemblyRoot = PACKAGES_CONFIG_TEMPLATE_DIR;
            if (Directory.Exists(PACKAGES_CONFIG_TEMPLATE_DIR))
                weaverAssemblyRoot = PACKAGES_CONFIG_TEMPLATE_DIR;
            else if (Directory.Exists(DEFAULT_CONFIG_TEMPLATE_DIR))
                weaverAssemblyRoot = DEFAULT_CONFIG_TEMPLATE_DIR;

            LoadAssembly(weaverAssemblyRoot + "Fody/FodyHelpers.dll");
            LoadAssembly(weaverAssemblyRoot + "Fody/Mono.Cecil.dll");
            LoadAssembly(weaverAssemblyRoot + "Fody/Mono.Cecil.Pdb.dll");
            LoadAssembly(weaverAssemblyRoot + "Fody/Mono.Cecil.Rocks.dll");
            Assembly assembly = LoadAssembly(weaverAssemblyRoot + "Fody/Fody.Unity.dll");

            List<string> assemblyNames = new List<string>();
            XElement config = XElement.Load(CONFIG_FULLNAME);
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

            foreach (string name in assemblyNames)
            {
                string assemblyFilename = assembliesRootPath + name;
                if (!assemblyFilename.EndsWith(ASSEMBLIE_FILENAME_SUFFIX, StringComparison.OrdinalIgnoreCase))
                    assemblyFilename += ASSEMBLIE_FILENAME_SUFFIX;

                if (!File.Exists(assemblyFilename))
                    continue;

                var weavingTaskType = FindType(assembly, "WeavingTask");
                Func<string, Type> weaverFinder = (weaverName) => WeaverFinder(weaverName);
                Action<int, string> logger = Log;
                var target = Activator.CreateInstance(weavingTaskType, new object[] { assemblyFilename, weaverFinder, config, logger });
                if (Execute(target))
                    Debug.LogFormat("Weaving code succeeded for {0}.dll", name);
                else
                    Debug.LogWarning($"The assembly has already been processed by Fody. Weaving aborted. Path: {assemblyFilename}");
            }
        }

        protected static bool Execute(object target)
        {
            MethodInfo methodInfo = target.GetType().GetMethod("Execute");
            return (bool)methodInfo.Invoke(target, null);
        }

        protected static void Log(int level, string message)
        {
            switch (level)
            {
                case 0:
                case 1:
                    Debug.Log(message);
                    break;
                case 2:
                    Debug.LogWarning(message);
                    break;
                case 3:
                    Debug.LogError(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }

        protected static Type WeaverFinder(string weaverName)
        {
            Assembly assembly = FindWeaverAssembly(weaverName);
            return FindType(assembly, "ModuleWeaver");
        }

        protected static Assembly LoadAssembly(string path)
        {
            return Assembly.Load(File.ReadAllBytes(path));
        }

        protected static Assembly FindWeaverAssembly(string weaverName)
        {
            string weaverDllName = $"{weaverName}.Fody.dll";
            DirectoryInfo weaverRoot = new DirectoryInfo(DEFAULT_CONFIG_TEMPLATE_DIR);
            if (weaverRoot.Exists)
            {
                foreach (var file in weaverRoot.GetFiles(weaverDllName, SearchOption.AllDirectories))
                {
                    return LoadAssembly(file.FullName);
                }
            }

            weaverRoot = new DirectoryInfo("Packages");
            if (weaverRoot.Exists)
            {
                foreach (var file in weaverRoot.GetFiles(weaverDllName, SearchOption.AllDirectories))
                {
                    return LoadAssembly(file.FullName);
                }
            }

            weaverRoot = new DirectoryInfo("Library/PackageCache");
            if (weaverRoot.Exists)
            {
                foreach (var file in weaverRoot.GetFiles(weaverDllName, SearchOption.AllDirectories))
                {
                    return LoadAssembly(file.FullName);
                }
            }

            Assembly[] listAssembly = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in listAssembly)
            {
                if (Regex.IsMatch(assembly.FullName, $"^{weaverName}.Fody"))
                    return assembly;
            }
            return null;
        }

        protected static Type FindType(Assembly assembly, string typeName)
        {
            try
            {
                return assembly
                    .GetTypes()
                    .FirstOrDefault(x => x.Name == typeName);
            }
            catch (ReflectionTypeLoadException exception)
            {
                throw new Exception(string.Format(
                    @"Could not load '{0}' from '{1}' due to ReflectionTypeLoadException.{2}", typeName, assembly.FullName, exception));
            }
        }
    }
}
