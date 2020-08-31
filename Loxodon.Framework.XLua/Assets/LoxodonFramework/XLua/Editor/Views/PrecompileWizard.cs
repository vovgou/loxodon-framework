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
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.Text;

namespace Loxodon.Framework.XLua.Editors
{
    public class PrecompileWizard : ScriptableWizard
    {
        private const string CONTEXT_MENU_EDIT_ITEM = "Edit Extension";
        private const string PATH = "Assets/LoxodonFramework/Editor/AppData/XLua/PrecompileSetting.json";
        private const string LUA_EXTENSION_PATTERN = @"((\.lua)|(\.lua\.txt))$";

        [MenuItem("Tools/Loxodon/Precompile Wizard for Lua")]
        static void CreateWizard()
        {
            PrecompileWizard wizard = DisplayWizard<PrecompileWizard>("Precompile Wizard", "Precompile Or Copy", "Apply");
            wizard.isValid = true;
        }

        [SerializeField]
        [HideInInspector]
        private bool onlyCopy;

        [SerializeField]
        [HideInInspector]
        private string bin;

        [SerializeField]
        [HideInInspector]
        private string output;
        [SerializeField]
        [HideInInspector]
        private string extension = ".bytes";
        [SerializeField]
        [HideInInspector]
        private List<string> extensions = new List<string> { ".luac", ".bytes" };
        private string extensionsStr = null;
        private bool isEditExtensions = false;

        [SerializeField]
        [HideInInspector]
        private List<string> srcRoots = new List<string>();
        [SerializeField]
        [HideInInspector]
        private bool srcFoldout = true;

        [SerializeField]
        [HideInInspector]
        private bool debug = false;

        [SerializeField]
        [HideInInspector]
        private bool encryptionFoldout = true;
        [SerializeField]
        [HideInInspector]
        private bool encryptionEnable = false;
        [SerializeField]
        [HideInInspector]
        private string currTypeName = "RijndaelCryptographFactory";
        [SerializeField]
        [HideInInspector]
        private Dictionary encryptionSetting = new Dictionary();

        private int encryptorTypeIndex = 0;
        private List<Type> encryptorTypes;
        private string[] encryptorTypeNames;

        private EncryptorFactory factory;
        private SerializedObject serializedObject;

        protected virtual string Extension
        {
            get
            {
                if (!this.extensions.Contains(this.extension))
                    this.extension = this.extensions[0];
                return this.extension;
            }
        }

        private void OnEnable()
        {
            this.Load();

            if (this.srcRoots.Count == 0)
                this.srcRoots.Add("");

            this.encryptorTypes = this.GetEncryptorFactoryTypes();
            this.encryptorTypeNames = this.encryptorTypes.Select(t => t.Name).ToArray();
            this.encryptorTypeIndex = Array.IndexOf(this.encryptorTypeNames, this.currTypeName);
            if (encryptorTypeIndex > encryptorTypes.Count - 1 || encryptorTypeIndex < 0)
                encryptorTypeIndex = 0;

            this.CreateSerializedObject();
        }

        protected virtual void CreateSerializedObject()
        {
            if (this.factory != null)
            {
                ScriptableObject.DestroyImmediate(this.factory);
                this.factory = null;
            }

            if (this.serializedObject != null)
            {
                this.serializedObject.Dispose();
                this.serializedObject = null;
            }

            Type type = encryptorTypes[encryptorTypeIndex];
            this.currTypeName = type.Name;
            this.factory = (EncryptorFactory)ScriptableObject.CreateInstance(type);

            string json = this.encryptionSetting.Get(currTypeName);
            if (!string.IsNullOrEmpty(json))
                JsonUtility.FromJsonOverwrite(json, this.factory);
            this.serializedObject = new SerializedObject(this.factory);
        }

        protected override bool DrawWizardGUI()
        {
            //base.DrawWizardGUI();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Only Copy", "Only copy files, do not compile."));
            this.onlyCopy = EditorGUILayout.Toggle(this.onlyCopy);
            EditorGUILayout.EndHorizontal();

            if (!this.onlyCopy)
            {
                EditorGUILayout.Space();
                DrawBinPath();
            }

            EditorGUILayout.Space();
            DrawOutputPath();
            EditorGUILayout.Space();
            DrawSrcPaths();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            this.debug = EditorGUILayout.Toggle(new GUIContent("Debug", "If it is not debug mode, the debug information will be stripped."), debug);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            DrawEncryption();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            return true;
        }

        protected virtual void DrawEncryption()
        {
            this.encryptionFoldout = EditorGUILayout.Foldout(this.encryptionFoldout, "Encryption", true);
            EditorGUILayout.Space();
            if (encryptionFoldout)
            {
                EditorGUI.indentLevel++;
                this.encryptionEnable = EditorGUILayout.Toggle("Enable", this.encryptionEnable);
                EditorGUILayout.Space();
                bool guiEnabled = GUI.enabled;
                if (!this.encryptionEnable)
                    GUI.enabled = false;

                EditorGUI.BeginChangeCheck();
                encryptorTypeIndex = EditorGUILayout.Popup(encryptorTypeIndex, this.encryptorTypeNames);
                if (EditorGUI.EndChangeCheck())
                    this.CreateSerializedObject();

                EditorGUILayout.Space();

                string[] propertyToExclude = new string[] { "m_Script" };
                SerializedProperty property = this.serializedObject.GetIterator();

                bool expanded = true;
                while (property.NextVisible(expanded))
                {
                    expanded = false;
                    if (propertyToExclude.Contains(property.name))
                        continue;

                    EditorGUILayout.PropertyField(property, true);
                    EditorGUILayout.Space();
                }
                serializedObject.ApplyModifiedProperties();
                GUI.enabled = guiEnabled;
                EditorGUI.indentLevel--;
            }
        }

        protected virtual void DrawOutputPath()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Output", "The output folder"));
            this.output = EditorGUILayout.TextField(output);
            if (this.isEditExtensions)
            {
                if (this.extensionsStr == null)
                {
                    StringBuilder buf = new StringBuilder();
                    for (int i = 0; i < extensions.Count; i++)
                    {
                        var extension = extensions[i];
                        buf.Append(extension);
                        if (i < extensions.Count - 1)
                            buf.Append(",");
                    }
                    this.extensionsStr = buf.ToString();
                }

                extensionsStr = EditorGUILayout.TextField(extensionsStr, GUILayout.Width(120));
                if (GUILayout.Button("OK", EditorStyles.miniButton, GUILayout.Width(80)))
                {
                    if (!string.IsNullOrEmpty(extensionsStr))
                    {
                        this.isEditExtensions = false;
                        this.extensions.Clear();
                        foreach (string ext in this.extensionsStr.Split(','))
                        {
                            string extension = ext.Trim();
                            if (string.IsNullOrEmpty(extension))
                                continue;

                            if (!extension.StartsWith("."))
                                extension = "." + extension;

                            this.extensions.Add(extension);
                        }
                    }
                    GUI.FocusControl(null);
                }
            }
            else
            {
                if (EditorGUILayout.DropdownButton(new GUIContent(Extension), FocusType.Passive, EditorStyles.popup, GUILayout.Width(80)))
                {
                    DrawExtensionContextMenu();
                }
                DrawBrowseFolderButton(ref this.output, "Browse", string.Empty);
            }
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DrawExtensionContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            foreach (var ext in extensions)
            {
                bool on = ext.Equals(this.extension);
                menu.AddItem(new GUIContent(ext), on, context =>
                {
                    this.extension = ext;
                }, null);
            }

            menu.AddItem(new GUIContent(CONTEXT_MENU_EDIT_ITEM), false, context =>
             {
                 this.isEditExtensions = true;
             }, null);
            menu.ShowAsContext();
        }

        protected virtual void DrawBinPath()
        {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bin", "Precompiled command"));
            this.bin = EditorGUILayout.TextField(this.bin);
            DrawBrowseFileButton(ref this.bin, "Browse", string.Empty);
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DrawSrcPaths()
        {
            this.srcFoldout = EditorGUILayout.Foldout(srcFoldout, "Src", true);
            if (srcFoldout)
            {
                for (int i = 0; i < srcRoots.Count; i++)
                {
                    var index = i;
                    var root = srcRoots[i];

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(" ");
                    EditorGUI.BeginChangeCheck();
                    root = EditorGUILayout.TextField(root);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.srcRoots[index] = root;
                    }
                    this.DrawBrowseFolderButton(ref this.srcRoots, index, "Browse", string.Empty);

                    if (GUILayout.Button(new GUIContent("+"), EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                    {
                        this.srcRoots.Insert(index + 1, "");
                        GUI.FocusControl(null);
                    }

                    if (this.srcRoots.Count <= 1)
                        GUI.enabled = false;
                    if (GUILayout.Button(new GUIContent("-"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
                    {
                        this.srcRoots.RemoveAt(index);
                        GUI.FocusControl(null);
                    }
                    if (!GUI.enabled)
                        GUI.enabled = true;

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        protected virtual void DrawBrowseFileButton(ref string url, string label, string extension)
        {
            if (GUILayout.Button(label, EditorStyles.miniButton, GUILayout.Width(80)))
            {
                var projectPath = Path.GetFullPath(".");
                string dir = projectPath;
                try
                {
                    FileInfo fileInfo = new FileInfo(url);
                    if (fileInfo.Directory.Exists)
                        dir = fileInfo.DirectoryName;
                }
                catch (Exception) { }
                var path = EditorUtility.OpenFilePanel("Browse File", dir, extension);
                if (!string.IsNullOrEmpty(path))
                {
                    projectPath = projectPath.Replace("\\", "/");
                    if (path.StartsWith(projectPath) && path.Length > projectPath.Length)
                        path = path.Remove(0, projectPath.Length + 1);
                    url = path;
                    GUI.FocusControl(null);
                }
            }
        }

        protected virtual void DrawBrowseFolderButton(ref string url, string label, string defaultName)
        {
            if (GUILayout.Button(label, EditorStyles.miniButton, GUILayout.Width(80)))
            {
                var projectPath = Path.GetFullPath(".");
                string dir = projectPath;
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(url);
                    if (directoryInfo.Exists)
                        dir = directoryInfo.FullName;
                }
                catch (Exception) { }

                var path = EditorUtility.OpenFolderPanel("Browse Folder", dir, defaultName);
                if (!string.IsNullOrEmpty(path))
                {
                    projectPath = projectPath.Replace("\\", "/");
                    if (path.StartsWith(projectPath) && path.Length > projectPath.Length)
                        path = path.Remove(0, projectPath.Length + 1);
                    url = path;
                    GUI.FocusControl(null);
                }
            }
        }

        protected virtual void DrawBrowseFolderButton(ref List<string> urls, int index, string label, string defaultName)
        {
            if (GUILayout.Button(label, EditorStyles.miniButton, GUILayout.Width(80)))
            {
                var path = EditorUtility.OpenFolderPanel("Browse Folder", urls[index], defaultName);
                if (!string.IsNullOrEmpty(path))
                {
                    var projectPath = Path.GetFullPath(".");
                    projectPath = projectPath.Replace("\\", "/");
                    if (path.StartsWith(projectPath) && path.Length > projectPath.Length)
                        path = path.Remove(0, projectPath.Length + 1);
                    urls[index] = path;
                    GUI.FocusControl(null);
                }
            }
        }

        void OnWizardUpdate()
        {
            if (!onlyCopy && (this.bin == null || string.IsNullOrEmpty(this.bin)))
            {
                errorString = "The \"Bin\" field cannot be empty, please select the precompilation command.";
                this.isValid = false;
            }
            else if (this.output == null || string.IsNullOrEmpty(this.output))
            {
                errorString = "Please select the root directory of the output file.";
                this.isValid = false;
            }
            else if (this.srcRoots == null || this.srcRoots.Count == 0)
            {
                errorString = "Please select the root directory of the lua source code.";
                this.isValid = false;
            }
            else if (this.srcRoots.Count > 0)
            {
                foreach (string root in this.srcRoots)
                {
                    if (string.IsNullOrEmpty(root) || string.IsNullOrEmpty(root.Trim()))
                    {
                        errorString = "Please select the root directory of the lua source code.";
                        this.isValid = false;
                        return;
                    }
                }

                this.errorString = "";
                this.isValid = true;
            }
            else
            {
                this.errorString = "";
                this.isValid = true;
            }
        }

        private void OnWizardCreate()
        {
            this.isValid = false;
            try
            {
                this.Save();
                EditorApplication.delayCall += () => this.Precompile();
            }
            finally
            {
                this.isValid = true;
            }
        }

        private void OnWizardOtherButton()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                this.Save();
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
        }

        private bool isLuaFile(string path)
        {
            if (Regex.IsMatch(path, LUA_EXTENSION_PATTERN, RegexOptions.IgnoreCase))
                return true;
            return false;
        }

        private void Precompile()
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                DirectoryInfo output = new DirectoryInfo(this.output);
                LuaCompiler compiler = this.encryptionEnable ? new LuaCompiler(this.bin, this.factory.Create()) : new LuaCompiler(this.bin);

                int total = 0;
                int i = 0;
                Dictionary<DirectoryInfo, List<FileInfo>> files = new Dictionary<DirectoryInfo, List<FileInfo>>(new DirectoryInfoEqualityComparer());
                foreach (string rootPath in this.srcRoots)
                {
                    if (string.IsNullOrEmpty(rootPath) || string.IsNullOrEmpty(rootPath.Trim()))
                        continue;

                    DirectoryInfo root = new DirectoryInfo(rootPath.Trim());
                    List<FileInfo> list;
                    if (!files.TryGetValue(root, out list))
                    {
                        list = new List<FileInfo>();
                        files.Add(root, list);
                    }

                    foreach (var fileInfo in root.GetFiles("*", SearchOption.AllDirectories))
                    {
                        if (!isLuaFile(fileInfo.FullName))
                            continue;

                        list.Add(fileInfo);
                        total++;
                    }
                }

                foreach (var kv in files)
                {
                    DirectoryInfo root = kv.Key;
                    List<FileInfo> list = kv.Value;
                    foreach (var fileInfo in list)
                    {
                        if (onlyCopy)
                        {
                            EditorUtility.DisplayProgressBar("Copy", "File copying, please wait!", (i++ / (float)total));
                            string fullName = Regex.Replace(fileInfo.FullName, LUA_EXTENSION_PATTERN, Extension);
                            fullName = fullName.Replace(root.FullName, output.FullName);
                            compiler.Copy(fileInfo, new FileInfo(fullName));
                        }
                        else
                        {
                            EditorUtility.DisplayProgressBar("Lua Precompile", "Precompiling, please wait!", (i++ / (float)total));
                            string fullName = Regex.Replace(fileInfo.FullName, LUA_EXTENSION_PATTERN, Extension);
                            fullName = fullName.Replace(root.FullName, output.FullName);
                            compiler.Compile(fileInfo, new FileInfo(fullName), this.debug);
                        }
                    }
                }
                EditorUtility.OpenWithDefaultApp(output.FullName);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Error:{0}", e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(PATH))
                    return;

                string json = File.ReadAllText(PATH);
                JsonUtility.FromJsonOverwrite(json, this);

            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Loads {0} failure. Error:{1}", PATH, e);
            }
        }

        private void Save()
        {
            try
            {
                if (!File.Exists(PATH))
                {
                    FileInfo info = new FileInfo(PATH);
                    if (!info.Directory.Exists)
                        info.Directory.Create();
                }

                var json = JsonUtility.ToJson(this.factory);
                this.encryptionSetting.Set(factory.GetType().Name, json);

                File.WriteAllText(PATH, JsonUtility.ToJson(this));

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Write {0} failure. Error:{1}", PATH, e);
            }
        }

        public List<Type> GetEncryptorFactoryTypes()
        {
            List<Type> factories = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assembly.FullName;
                if (!Regex.IsMatch(name, "^(Assembly-CSharp)"))
                    continue;

                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(EncryptorFactory).IsAssignableFrom(type) || type.IsAbstract)
                        continue;

                    factories.Add(type);
                }
            }
            return factories;
        }

        class DirectoryInfoEqualityComparer : IEqualityComparer<DirectoryInfo>
        {
            public bool Equals(DirectoryInfo x, DirectoryInfo y)
            {
                if (x == null && y == null)
                    return true;
                if (x != null && y != null)
                    return x.FullName.Equals(y.FullName);
                return false;
            }

            public int GetHashCode(DirectoryInfo obj)
            {
                return obj.FullName.GetHashCode();
            }
        }

        [Serializable]
        class Dictionary
        {
            [SerializeField]
            [HideInInspector]
            private List<string> keys = new List<string>();
            [SerializeField]
            [HideInInspector]
            private List<string> values = new List<string>();

            public void Set(string key, string value)
            {
                if (!keys.Contains(key))
                {
                    keys.Add(key);
                    values.Add("");
                }

                int index = keys.IndexOf(key);
                values[index] = value;
            }

            public string Get(string key)
            {
                if (!keys.Contains(key))
                    return null;

                int index = keys.IndexOf(key);
                return values[index];
            }
        }
    }
}
