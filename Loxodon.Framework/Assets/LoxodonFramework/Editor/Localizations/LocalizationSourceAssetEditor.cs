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
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using ValueType = Loxodon.Framework.Localizations.ValueType;

namespace Loxodon.Framework.Editors
{
    [CustomEditor(typeof(LocalizationSourceAsset))]
    [CanEditMultipleObjects]
    public class LocalizationSourceAssetEditor : LocalizationSourceEditor
    {
        private int foldoutIndex = -1;
        private float elementHeight = 0;

        private ReorderableList entryList;

        protected virtual void OnEnable()
        {
            var source = serializedObject.FindProperty("Source");
            var entries = source.FindPropertyRelative("entries");

            entryList = new ReorderableList(entries.serializedObject, entries, true, true, true, true);
            entryList.elementHeight = 22;
            entryList.onAddCallback = OnAdd;
            entryList.onRemoveCallback = OnRemove;
            entryList.drawHeaderCallback = DrawHeader;
            entryList.drawElementCallback = DrawElement;
            entryList.drawElementBackgroundCallback = DrawElementBackground;
            entryList.elementHeightCallback = EntryListElementHeight;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Localization Source", TitleGUIStyle))
            {
                Application.OpenURL("https://github.com/cocowolf/loxodon-framework/blob/master/docs/LoxodonFramework.md");
            }
            GUILayout.Space(10);
            this.serializedObject.Update();
            this.entryList.DoLayoutList();
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

        protected virtual void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
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
            var valueProperty = entry.FindPropertyRelative("value");

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
                this.foldoutIndex = foldout ? index : -1;
                this.entryList.index = foldoutIndex;
            }

            if (foldoutIndex != index)
            {
                Rect keyRect = new Rect(foldoutRect.xMax - 15, y, Mathf.Min(200, width * 0.4f), height);
                Rect typeRect = new Rect(keyRect.xMax + HORIZONTAL_GAP, y, Mathf.Min(120, width * 0.2f), height);
                Rect valueRect = new Rect(typeRect.xMax + HORIZONTAL_GAP, y, position.xMax - typeRect.xMax - HORIZONTAL_GAP, height);

                EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);

                EditorGUI.LabelField(typeRect, type.ToString());
                DrawValueField(valueRect, valueProperty, type);
            }
            else
            {
                y += height + VERTICAL_GAP;
                width = position.width + 40;

                Rect keyLabelRect = new Rect(x, y, Mathf.Min(60, width * 0.2f), height);
                Rect keyRect = new Rect(keyLabelRect.xMax + HORIZONTAL_GAP, y, width - keyLabelRect.width - HORIZONTAL_GAP, height);
                EditorGUI.LabelField(keyLabelRect, "Key");
                EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);

                y += height + VERTICAL_GAP;

                Rect typeLabelRect = new Rect(x, y, Mathf.Min(60, width * 0.2f), height);
                Rect typeRect = new Rect(typeLabelRect.xMax + HORIZONTAL_GAP, y, width - typeLabelRect.width - HORIZONTAL_GAP, height);
                EditorGUI.LabelField(typeLabelRect, "Type");
                EditorGUI.LabelField(typeRect, type.ToString());

                y += height + VERTICAL_GAP;

                float valueHeight = type == ValueType.String ? height * 4 : height;
                Rect valueLabelRect = new Rect(x, y, Mathf.Min(60, width * 0.2f), height);
                Rect valueRect = new Rect(valueLabelRect.xMax + HORIZONTAL_GAP, y, width - valueLabelRect.width - HORIZONTAL_GAP, valueHeight);
                EditorGUI.LabelField(valueLabelRect, "Value");
                DrawValueField(valueRect, valueProperty, type, true);

                y += valueHeight + VERTICAL_GAP;

                this.elementHeight = y - position.y + 5;
            }
        }

        protected virtual void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Localizations");
        }

        protected virtual void OnAdd(ReorderableList list)
        {
            var entries = list.serializedProperty;
            int index = entries.arraySize > 0 ? entries.arraySize : 0;
            this.DrawContextMenu(entries, index);
        }

        protected virtual void OnRemove(ReorderableList list)
        {
            var entries = list.serializedProperty;
            AskRemoveEntry(entries, list.index);
        }

        protected virtual void DrawContextMenu(SerializedProperty entries, int index)
        {
            GenericMenu menu = new GenericMenu();
            foreach (ValueType valueType in Enum.GetValues(typeof(ValueType)))
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
            var valueProperty = entryProperty.FindPropertyRelative("value");

            typeProperty.enumValueIndex = (int)type;
            keyProperty.stringValue = string.Empty;
            valueProperty.FindPropertyRelative("dataValue").stringValue = null;
            valueProperty.FindPropertyRelative("objectValue").objectReferenceValue = null;

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
            var valueProperty = entryProperty.FindPropertyRelative("value");

            keyProperty.stringValue = string.Empty;
            valueProperty.FindPropertyRelative("dataValue").stringValue = null;
            valueProperty.FindPropertyRelative("objectValue").objectReferenceValue = null;

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
    }
}
