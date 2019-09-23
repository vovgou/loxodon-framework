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

using Loxodon.Framework.Localizations;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using ValueType = Loxodon.Framework.Localizations.ValueType;

namespace Loxodon.Framework.Editors
{
    [CustomEditor(typeof(LocalizationSourceBehaviour))]
    [CanEditMultipleObjects]

    public class LocalizationSourceBehaviourEditor : LocalizationSourceEditor
    {
        [SerializeField]
        private int currLanguageIndex = 0;

        private int foldoutIndex = -1;
        private int toolbarIndex = 0;
        private int selectedLanguageIndex = -1;

        private float elementHeight = 0;

        private ReorderableList entryList;
        private ReorderableList languageList;
        private List<string> languages;
        private MultilingualSource source;

        protected virtual void OnEnable()
        {
            var sourceProperty = serializedObject.FindProperty("Source");
            var entriesProperty = sourceProperty.FindPropertyRelative("entries");
            var languagesProperty = sourceProperty.FindPropertyRelative("languages");

            this.source = (target as LocalizationSourceBehaviour).Source;
            this.languages = source.Languages;
            if (this.languages.Count <= 0)
            {
                CultureInfo cultureInfo = Locale.GetCultureInfo();
                this.languages.Add(cultureInfo.Name);
            }

            entryList = new ReorderableList(entriesProperty.serializedObject, entriesProperty, true, true, true, true);
            entryList.elementHeight = 22;
            entryList.onAddCallback = OnAddEntry;
            entryList.onRemoveCallback = OnRemoveEntry;
            entryList.drawHeaderCallback = DrawEntryListHeader;
            entryList.drawElementCallback = DrawEntryListElement;
            entryList.elementHeightCallback = EntryListElementHeight;
            entryList.drawElementBackgroundCallback = DrawElementBackground;

            languageList = new ReorderableList(languagesProperty.serializedObject, languagesProperty, true, true, true, true);
            languageList.elementHeight = 22;
            languageList.onAddCallback = OnAddLanguage;
            languageList.onRemoveCallback = OnRemoveLanguage;
            languageList.drawHeaderCallback = DrawLanguageListHeader;
            languageList.drawElementCallback = DrawLanguageListElement;
            languageList.drawElementBackgroundCallback = DrawElementBackground;
            languageList.onSelectCallback = list =>
            {
                this.selectedLanguageIndex = list.index;
            };
            languageList.onReorderCallback = list =>
            {
                int oldIndex = this.selectedLanguageIndex;
                int newIndex = list.index;
                OnMove(oldIndex, newIndex);
                this.selectedLanguageIndex = newIndex;
            };
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            if (GUILayout.Button("Localization Source", TitleGUIStyle))
            {
                Application.OpenURL("https://github.com/cocowolf/loxodon-framework/blob/master/docs/LoxodonFramework.md");
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUIContent[] menuContents = new GUIContent[2] { new GUIContent("Source"), new GUIContent("Language") };
            toolbarIndex = GUILayout.SelectionGrid(toolbarIndex, menuContents, menuContents.Length, EditorStyles.toolbarButton, GUILayout.Width(100 * menuContents.Length));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            switch (toolbarIndex)
            {
                case 0:
                    this.entryList.DoLayoutList();
                    break;
                case 1:
                    this.languageList.DoLayoutList();
                    break;
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        protected virtual float EntryListElementHeight(int index)
        {
            if (this.foldoutIndex == index)
                return this.elementHeight;

            return entryList.elementHeight;
        }

        protected virtual void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, false, true);
        }

        #region EntryList

        protected virtual void DrawEntryListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var entries = entryList.serializedProperty;
            if (index < 0 || index >= entries.arraySize)
                return;

            var entry = entries.GetArrayElementAtIndex(index);

            float x = rect.x;
            float y = rect.y + 2;
            float width = rect.width - 40;
            float height = rect.height;

            Rect entryRect = new Rect(x, y, width, height);
            DrawEntryField(entryRect, entry, index);

            var buttonLeftRect = new Rect(entryRect.xMax + HORIZONTAL_GAP, y - 1, 18, 18);
            var buttonRightRect = new Rect(buttonLeftRect.xMax, y - 1, 18, 18);

            if (GUI.Button(buttonLeftRect, new GUIContent("+"), EditorStyles.miniButtonLeft))
            {
                DuplicateEntry(entries, index);
            }
            if (GUI.Button(buttonRightRect, new GUIContent("-"), EditorStyles.miniButtonRight))
            {
                AskRemoveEntry(entries, index);
            }
        }

        protected virtual void DrawEntryField(Rect position, SerializedProperty entry, int index)
        {
            var keyProperty = entry.FindPropertyRelative("key");
            var typeProperty = entry.FindPropertyRelative("type");
            var valuesProperty = entry.FindPropertyRelative("values");

            ValueType type = (ValueType)typeProperty.enumValueIndex;

            float x = position.x;
            float y = position.y;
            float height = EditorGUIUtility.singleLineHeight;
            float width = position.width - HORIZONTAL_GAP * 2;

            Rect foldoutRect = new Rect(x + 15, y, 16, 16);
            var foldout = index == foldoutIndex;
            EditorGUI.BeginChangeCheck();
            foldout = EditorGUI.Foldout(foldoutRect, foldout, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                foldoutIndex = foldout ? index : -1;
                this.entryList.index = foldoutIndex;
            }

            if (foldoutIndex != index)
            {
                Rect keyRect = new Rect(foldoutRect.xMax - 15, y, Mathf.Min(200, width * 0.4f), height);
                Rect typeRect = new Rect(keyRect.xMax + HORIZONTAL_GAP, y, Mathf.Min(120, width * 0.2f), height);
                Rect valueRect = new Rect(typeRect.xMax + HORIZONTAL_GAP, y, position.xMax - typeRect.xMax - HORIZONTAL_GAP, height);

                EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
                EditorGUI.LabelField(typeRect, type.ToString());
                if (this.currLanguageIndex >= this.languages.Count)
                    this.currLanguageIndex = 0;

                var valueProperty = valuesProperty.GetArrayElementAtIndex(this.currLanguageIndex);
                DrawValueField(valueRect, valueProperty, type);
            }
            else
            {
                y += height + VERTICAL_GAP;
                width = position.width + 40;
                Rect keyLabelRect = new Rect(x, y, Mathf.Min(200, width * 0.4f), height);
                Rect keyRect = new Rect(keyLabelRect.xMax + HORIZONTAL_GAP, y, width - keyLabelRect.width - HORIZONTAL_GAP, height);
                EditorGUI.LabelField(keyLabelRect, "Key");
                EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);

                y += height + VERTICAL_GAP;

                Rect typeLabelRect = new Rect(x, y, Mathf.Min(200, width * 0.4f), height);
                Rect typeRect = new Rect(typeLabelRect.xMax + HORIZONTAL_GAP, y, width - typeLabelRect.width - HORIZONTAL_GAP, height);
                EditorGUI.LabelField(typeLabelRect, "Type");
                EditorGUI.LabelField(typeRect, type.ToString());

                y += height + VERTICAL_GAP;

                float valueHeight = type == ValueType.String ? height * 2 : height;

                for (int i = 0; i < languages.Count; i++)
                {
                    Rect languageRect = new Rect(x, y, Mathf.Min(200, width * 0.4f), height);
                    Rect valueRect = new Rect(languageRect.xMax + HORIZONTAL_GAP, y, position.xMax - languageRect.xMax - HORIZONTAL_GAP, valueHeight);

                    Language language = Language.GetLanguage(languages[i]);
                    var valueProperty = valuesProperty.GetArrayElementAtIndex(i);

                    string languageContent = string.Format("{0} [{1}]", language.Name, language.Code);
                    if (!string.IsNullOrEmpty(language.Country))
                        languageContent = string.Format("{0}({1}) [{2}]", language.Name, language.Country, language.Code);

                    EditorGUI.LabelField(languageRect, new GUIContent(languageContent, languageContent));
                    DrawValueField(valueRect, valueProperty, type, true);

                    y += valueHeight + VERTICAL_GAP;
                }

                this.elementHeight = y - position.y + 5;
            }
        }

        protected virtual void DrawEntryListHeader(Rect rect)
        {
            float x = rect.x;
            float y = rect.y;
            float width = rect.width;
            float height = rect.height;

            Rect labelRect = new Rect(x, y, Mathf.Min(200, width * 0.4f), height);
            Rect languageRect = new Rect(rect.xMax - 150, y, 150, height);

            List<string> list = GetSelectedLanguages();
            EditorGUI.LabelField(labelRect, "Localizations");
            currLanguageIndex = EditorGUI.Popup(languageRect, currLanguageIndex, list.ToArray(), EditorStyles.popup);
        }

        protected virtual void OnAddEntry(ReorderableList list)
        {
            var entries = list.serializedProperty;
            int index = entries.arraySize > 0 ? entries.arraySize : 0;
            this.DrawContextMenu(entries, index);
        }

        protected virtual void OnRemoveEntry(ReorderableList list)
        {
            var entries = list.serializedProperty;
            AskRemoveEntry(entries, list.index);
        }

        protected virtual void DrawContextMenu(SerializedProperty entries, int index)
        {
            GenericMenu menu = new GenericMenu();
            foreach (ValueType valueType in System.Enum.GetValues(typeof(ValueType)))
            {
                var type = valueType;
                menu.AddItem(new GUIContent(valueType.ToString()), false, context =>
                {
                    AddEntry(entries, index, type);
                }, null);
            }
            menu.ShowAsContext();
        }

        protected virtual void AddEntry(SerializedProperty entries, int index, ValueType type)
        {
            if (index < 0 || index > entries.arraySize)
                return;

            entries.serializedObject.Update();
            entries.InsertArrayElementAtIndex(index);
            SerializedProperty entryProperty = entries.GetArrayElementAtIndex(index);

            var typeProperty = entryProperty.FindPropertyRelative("type");
            var keyProperty = entryProperty.FindPropertyRelative("key");
            var valuesProperty = entryProperty.FindPropertyRelative("values");

            keyProperty.stringValue = string.Empty;
            typeProperty.enumValueIndex = (int)type;

            for (int i = 0; i < valuesProperty.arraySize; i++)
            {
                var valueProperty = valuesProperty.GetArrayElementAtIndex(i);
                valueProperty.FindPropertyRelative("dataValue").stringValue = null;
                valueProperty.FindPropertyRelative("objectValue").objectReferenceValue = null;
            }

            entries.serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        protected virtual void DuplicateEntry(SerializedProperty entries, int index)
        {
            if (index < 0 || index >= entries.arraySize)
                return;

            entries.serializedObject.Update();
            entries.InsertArrayElementAtIndex(index);
            SerializedProperty entryProperty = entries.GetArrayElementAtIndex(index + 1);

            var keyProperty = entryProperty.FindPropertyRelative("key");
            var valuesProperty = entryProperty.FindPropertyRelative("values");

            keyProperty.stringValue = string.Empty;

            for (int i = 0; i < valuesProperty.arraySize; i++)
            {
                var valueProperty = valuesProperty.GetArrayElementAtIndex(i);
                valueProperty.FindPropertyRelative("dataValue").stringValue = null;
                valueProperty.FindPropertyRelative("objectValue").objectReferenceValue = null;
            }

            entries.serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        protected virtual void AskRemoveEntry(SerializedProperty entries, int index)
        {
            if (entries == null || index < 0 || index >= entries.arraySize)
                return;

            var variable = entries.GetArrayElementAtIndex(index);
            var key = variable.FindPropertyRelative("key").stringValue;
            if (string.IsNullOrEmpty(key))
            {
                RemoveEntry(entries, index);
                return;
            }

            if (EditorUtility.DisplayDialog("Confirm delete", string.Format("Are you sure you want to delete the item named \"{0}\"?", key), "Yes", "Cancel"))
            {
                RemoveEntry(entries, index);
            }
        }

        protected virtual void RemoveEntry(SerializedProperty entries, int index)
        {
            if (index < 0 || index >= entries.arraySize)
                return;

            entries.serializedObject.Update();
            entries.DeleteArrayElementAtIndex(index);
            entries.serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }
        #endregion

        #region LanguagePanel
        protected virtual void DrawLanguageListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var languagesProperty = this.languageList.serializedProperty;
            if (index < 0 || index >= languagesProperty.arraySize)
                return;

            var languageProperty = languagesProperty.GetArrayElementAtIndex(index);

            float x = rect.x;
            float y = rect.y;
            float width = rect.width - 20;
            float height = rect.height;

            Rect languageRect = new Rect(x, y, width, height);
            DrawLanguageField(languageRect, languageProperty, index);
            var deleteButtonRect = new Rect(languageRect.xMax + HORIZONTAL_GAP, y, 18, 18);

            if (languagesProperty.arraySize <= 1)
                GUI.enabled = false;

            if (GUI.Button(deleteButtonRect, new GUIContent("-"), EditorStyles.miniButton))
            {
                RemoveLanguage(languagesProperty, index);
            }
            GUI.enabled = true;
        }

        protected virtual void DrawLanguageListHeader(Rect rect)
        {
            GUI.Label(rect, "Languages");
        }

        protected void DrawLanguageField(Rect position, SerializedProperty languageProperty, int index)
        {
            var code = languageProperty.stringValue;
            var language = Language.GetLanguage(code);

            float x = position.x;
            float y = position.y + 2;
            float height = EditorGUIUtility.singleLineHeight;
            float width = position.width - HORIZONTAL_GAP * 2;

            Rect nameRect = new Rect(x, y, Mathf.Min(200, width * 0.4f), height);
            Rect defaultRect = new Rect(nameRect.xMax + HORIZONTAL_GAP, y, Mathf.Min(120, width * 0.2f), height);
            Rect codeRect = new Rect(defaultRect.xMax + HORIZONTAL_GAP, y - 1, position.xMax - defaultRect.xMax - HORIZONTAL_GAP, height);

            var name = language.Name;
            if (!string.IsNullOrEmpty(language.Country))
                name = string.Format("{0} ({1})", language.Name, language.Country);

            EditorGUI.LabelField(nameRect, name);

            EditorGUI.LabelField(defaultRect, index == 0 ? string.Format("default({0})", code) : "");

            if (GUI.Button(codeRect, new GUIContent(code), EditorStyles.popup))
            {
                DrawLanguageContextMenu(index);
            }
        }

        protected virtual void OnAddLanguage(ReorderableList list)
        {
            var languages = list.serializedProperty;
            int index = languages.arraySize > 0 ? languages.arraySize : 0;
            this.DrawLanguageContextMenu(index);
        }

        protected virtual void OnRemoveLanguage(ReorderableList list)
        {
            var languagesProperty = list.serializedProperty;
            RemoveLanguage(languagesProperty, list.index);
        }

        protected virtual List<Language> GetLanguages(params string[] filters)
        {
            List<Language> list = Language.GetAll(filters);
            list.Sort((x, y) =>
            {
                int n = x.Name.CompareTo(y.Name);
                if (n != 0)
                    return n;

                return x.Code.CompareTo(y.Code);
            });
            return list;
        }

        protected virtual List<string> GetSelectedLanguages()
        {
            List<string> list = new List<string>();
            foreach (var code in this.languages)
            {
                var language = Language.GetLanguage(code);
                list.Add(string.Format("{0} [{1}]", language.Name, language.Code));
            }
            return list;
        }

        protected virtual void DrawLanguageContextMenu(int index)
        {
            GenericMenu menu = new GenericMenu();
            foreach (var language in GetLanguages(languages.ToArray()))
            {
                var code = language.Code;
                menu.AddItem(new GUIContent(language.Key), false, context =>
                {
                    AddLanguage(languages, index, code);
                }, null);
            }

            menu.ShowAsContext();
        }

        protected virtual void AddLanguage(List<string> list, int index, string code)
        {
            if (index < 0 || index > list.Count)
                return;

            var languagesProperty = languageList.serializedProperty;
            try
            {
                if (index == list.Count)
                {
                    if (list.Contains(code))
                        return;
                    else
                    {
                        languagesProperty.InsertArrayElementAtIndex(index);
                        languagesProperty.GetArrayElementAtIndex(index).stringValue = code;
                        var entriesProperty = this.entryList.serializedProperty;
                        for (int i = 0; i < entriesProperty.arraySize; i++)
                        {
                            var entryProperty = entriesProperty.GetArrayElementAtIndex(i);
                            var valuesProperty = entryProperty.FindPropertyRelative("values");
                            valuesProperty.InsertArrayElementAtIndex(index);
                            var valueProperty = valuesProperty.GetArrayElementAtIndex(index);
                            valueProperty.FindPropertyRelative("dataValue").stringValue = null;
                            valueProperty.FindPropertyRelative("objectValue").objectReferenceValue = null;

                            if (valuesProperty.arraySize > languagesProperty.arraySize)
                            {
                                for (int j = valuesProperty.arraySize - 1; j >= languagesProperty.arraySize; j--)
                                {
                                    valuesProperty.DeleteArrayElementAtIndex(j);
                                }
                            }
                        }
                    }
                }
                else
                {
                    languagesProperty.GetArrayElementAtIndex(index).stringValue = code;
                }
            }
            finally
            {
                this.serializedObject.ApplyModifiedProperties();
                GUI.FocusControl(null);
            }

        }

        protected virtual void RemoveLanguage(SerializedProperty languagesProperty, int index)
        {
            if (languagesProperty == null || index < 0 || index >= languagesProperty.arraySize)
                return;

            if (languagesProperty.arraySize <= 1)
            {
                EditorUtility.DisplayDialog("Title", "The last item cannot be deleted!", "OK");
                return;
            }

            if (this.currLanguageIndex == index)
                this.currLanguageIndex = 0;

            var languageProperty = languagesProperty.GetArrayElementAtIndex(index);
            var code = languageProperty.stringValue;

            if (EditorUtility.DisplayDialog("Confirm delete", string.Format("Are you sure you want to delete the item named \"{0}\" and all its data?", code), "Yes", "Cancel"))
            {
                this.serializedObject.Update();
                languagesProperty.DeleteArrayElementAtIndex(index);

                var entriesProperty = this.entryList.serializedProperty;
                for (int i = 0; i < entriesProperty.arraySize; i++)
                {
                    var entryProperty = entriesProperty.GetArrayElementAtIndex(i);
                    var valuesProperty = entryProperty.FindPropertyRelative("values");
                    if (valuesProperty.arraySize > index)
                        valuesProperty.DeleteArrayElementAtIndex(index);
                }
                this.serializedObject.ApplyModifiedProperties();
                this.serializedObject.Update();

                GUI.FocusControl(null);
            }
        }

        protected virtual void OnMove(int oldIndex, int newIndex)
        {
            if (this.currLanguageIndex == oldIndex)
                this.currLanguageIndex = newIndex;

            this.serializedObject.Update();
            var entriesProperty = this.entryList.serializedProperty;
            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                var entryProperty = entriesProperty.GetArrayElementAtIndex(i);
                var valuesProperty = entryProperty.FindPropertyRelative("values");
                valuesProperty.MoveArrayElement(oldIndex, newIndex);
            }
            this.serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }
}
