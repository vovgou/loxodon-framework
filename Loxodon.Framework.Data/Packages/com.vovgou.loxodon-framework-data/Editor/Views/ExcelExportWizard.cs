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
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Loxodon.Framework.Data.Editors
{
    public class ExcelExportWizard : ScriptableWizard
    {
        protected const float HORIZONTAL_GAP = 5;
        protected const float VERTICAL_GAP = 5;

        private const string PATH = "Assets/LoxodonFramework/Editor/AppData/Data/ExcelExportSetting.json";

        [MenuItem("Tools/Loxodon/Excel Export Wizard")]
        static void CreateWizard()
        {
            ExcelExportWizard wizard = DisplayWizard<ExcelExportWizard>("Excel Export Wizard", "", "Export");
            wizard.minSize = new Vector2(800, 500);
            wizard.isValid = true;
        }

        private static GUIStyle titleStyle;
        public static GUIStyle TitleGUIStyle
        {
            get
            {
                if (titleStyle == null)
                {
                    titleStyle = new GUIStyle("HeaderLabel");
                    titleStyle.fontSize = 18;
                    titleStyle.normal.textColor = Color.Lerp(Color.white, Color.gray, 0.5f);
                    titleStyle.fontStyle = FontStyle.BoldAndItalic;
                    titleStyle.alignment = TextAnchor.UpperCenter;
                }
                return titleStyle;
            }
        }

        [SerializeField]
        [HideInInspector]
        private string latestInputPath;
        [SerializeField]
        [HideInInspector]
        private string latestOutputPath;
        [SerializeField]
        [HideInInspector]
        private string processorTypeName;
        [SerializeField]
        [HideInInspector]
        private string nameGeneratorTypeName;
        [SerializeField]
        [HideInInspector]
        private Dictionary processorParameters = new Dictionary();

        private IExportProcessor processor;
        private List<FileEntry> entries;
        private SerializedObject serializedObject;

        private float expandedElementHeight = 0;
        private int toolbarIndex = 0;
        private Vector2 scrollPosition;
        private ReorderableList fileList;

        private int processorTypeIndex = 0;
        private List<Type> processorTypes;
        private string[] processorTypeNames;

        private void OnEnable()
        {
            this.Load();

            if (entries == null)
                entries = new List<FileEntry>();

            this.expandedElementHeight = 64;

            fileList = new ReorderableList(entries, typeof(FileEntry), false, true, true, true);
            fileList.elementHeight = 22;
            fileList.onAddCallback = OnAdd;
            fileList.onRemoveCallback = OnRemove;
            fileList.drawHeaderCallback = DrawHeader;
            fileList.drawElementCallback = DrawElement;
            fileList.drawElementBackgroundCallback = DrawElementBackground;
            fileList.elementHeightCallback = ElementHeight;

            this.LoadTypes();
            this.CreateProcessor();
        }

        private void OnDisable()
        {
            this.Save();
        }

        protected override bool DrawWizardGUI()
        {
            //base.DrawWizardGUI();

            if (GUILayout.Button("Excel Export", TitleGUIStyle))
            {
                Application.OpenURL("https://github.com/vovgou/loxodon-framework/wiki");
            }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUIContent[] menuContents = new GUIContent[] { new GUIContent("Excel Files"), new GUIContent("Configuration") };
            toolbarIndex = GUILayout.SelectionGrid(toolbarIndex, menuContents, menuContents.Length, EditorStyles.toolbarButton, GUILayout.Width(100 * menuContents.Length));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical();

            switch (toolbarIndex)
            {
                case 0:
                    {
                        this.fileList.DoLayoutList();
                        break;
                    }
                case 1:
                    {
                        EditorGUI.indentLevel += 1;

                        EditorGUI.BeginChangeCheck();
                        processorTypeIndex = EditorGUILayout.Popup("Processor", processorTypeIndex, this.processorTypeNames);
                        if (EditorGUI.EndChangeCheck())
                            this.CreateProcessor();

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
                        EditorGUI.indentLevel -= 1;
                        break;
                    }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            return true;
        }

        protected virtual void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0 || index >= fileList.count)
                return;

            FileEntry entry = (FileEntry)fileList.list[index];

            float x = rect.x;
            float y = rect.y + 2;
            float width = rect.width - HORIZONTAL_GAP * 2 - 40;
            float height = EditorGUIUtility.singleLineHeight;

            Rect foldoutRect = new Rect(x, y + 1, 16, 16);
            Rect entryRect = new Rect(foldoutRect.xMax + HORIZONTAL_GAP, y + 1, width, height);

            var foldout = index == fileList.index;
            EditorGUI.BeginChangeCheck();
            EditorGUI.Foldout(foldoutRect, foldout, GUIContent.none);

            if (foldout)
            {
                EditorGUI.LabelField(entryRect, entry.Filename);

                float labelWidth = 100f;
                Rect sheetLabelRect = new Rect(x + 20, y + height + VERTICAL_GAP + 1, 50f, height);
                Rect sheetRect = new Rect(sheetLabelRect.xMax + HORIZONTAL_GAP, y + height + VERTICAL_GAP + 1, labelWidth, height);

                EditorGUI.LabelField(sheetLabelRect, "Sheets:");
                foreach (var sheet in entry.Sheets)
                {
                    sheet.IsValid = EditorGUI.ToggleLeft(sheetRect, sheet.Name, sheet.IsValid);
                    sheetRect.x += labelWidth + HORIZONTAL_GAP;
                }
            }
            else
            {
                EditorGUI.LabelField(entryRect, entry.Filename);
            }
        }

        protected virtual float ElementHeight(int index)
        {
            if (this.fileList.index == index)
                return this.expandedElementHeight;

            return fileList.elementHeight;
        }

        protected virtual void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Files");
        }

        protected virtual void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, false, true);
        }

        protected virtual void OnAdd(ReorderableList list)
        {
            string path = EditorUtility.OpenFolderPanel("Browse Folder", this.latestInputPath, string.Empty);
            if (string.IsNullOrEmpty(path))
                return;

            this.latestInputPath = path;
            this.processor.ImportFiles(this.latestInputPath, (message, progress) =>
            {
                EditorUtility.DisplayProgressBar("Import", message, progress);
            }, entries =>
             {
                 this.entries.AddRange(entries);
                 if (fileList.count > 0 && fileList.index < 0)
                     fileList.index = 0;
                 EditorUtility.ClearProgressBar();
             });
        }

        protected virtual void OnRemove(ReorderableList list)
        {
            if (EditorUtility.DisplayDialog("Confirm delete", "Are you sure you want to delete the item?", "Yes", "Cancel"))
            {
                Remove(list.index);
            }
        }

        private void Remove(int index)
        {
            if (index < 0 || index >= fileList.count)
                return;

            fileList.list.RemoveAt(index);
        }

        private void OnWizardOtherButton()
        {
            this.Save();

            if (this.entries == null || this.entries.Count <= 0)
                return;

            string path = EditorUtility.OpenFolderPanel("Save to", this.latestOutputPath, string.Empty);
            if (string.IsNullOrEmpty(path))
                return;

            this.latestOutputPath = path;
            this.processor.ExportFiles(this.latestOutputPath, this.entries, (message, progress) =>
              {
                  EditorUtility.DisplayProgressBar("Export", message, progress);
              }, () =>
              {
                  EditorUtility.ClearProgressBar();
                  EditorUtility.OpenWithDefaultApp(this.latestOutputPath);
              });

        }

        protected void CreateProcessor()
        {
            try
            {
                Type type = processorTypes[processorTypeIndex];
                this.processorTypeName = type.Name;
                this.processor = (IExportProcessor)ScriptableObject.CreateInstance(type);
            }
            catch (Exception)
            {
                Type type = typeof(ExportProcessor);
                this.processorTypeName = type.Name;
                this.processorTypeIndex = Array.IndexOf(this.processorTypeNames, this.processorTypeName);
                this.processor = (IExportProcessor)ScriptableObject.CreateInstance(type);
            }

            string json = this.processorParameters.Get(this.processorTypeName);
            if (!string.IsNullOrEmpty(json))
                JsonUtility.FromJsonOverwrite(json, this.processor);
            this.serializedObject = new SerializedObject((UnityEngine.Object)processor);
        }

        protected void LoadTypes()
        {
            List<Type> processorTypes = new List<Type>();
            List<Type> generatorTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assembly.FullName;
                if (Regex.IsMatch(name, "^((Unity\\.)|(UnityEngine)|(UnityEditor)|(mscorlib)|(System\\.)|(Mono\\.)|(nunit\\.)|(Microsoft\\.)|(SyntaxTree\\.))"))
                    continue;

                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IExportProcessor).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        if (!processorTypes.Contains(type))
                            processorTypes.Add(type);
                        continue;
                    }
                }
            }

            if (string.IsNullOrEmpty(this.processorTypeName))
                this.processorTypeName = typeof(JsonExportProcessor).Name;
            this.processorTypes = processorTypes;
            this.processorTypeNames = this.processorTypes.Select(t => t.Name).ToArray();
            this.processorTypeIndex = Array.IndexOf(this.processorTypeNames, this.processorTypeName);
            if (processorTypeIndex > processorTypes.Count - 1 || processorTypeIndex < 0)
                processorTypeIndex = 0;
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

                var json = JsonUtility.ToJson(this.processor);
                this.processorParameters.Set(processor.GetType().Name, json);

                File.WriteAllText(PATH, JsonUtility.ToJson(this));

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Write {0} failure. Error:{1}", PATH, e);
            }
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