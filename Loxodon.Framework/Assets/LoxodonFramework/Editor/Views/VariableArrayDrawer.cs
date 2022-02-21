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

using Loxodon.Framework.Views.Variables;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Loxodon.Framework.Editors
{
    [CustomPropertyDrawer(typeof(VariableArray))]
    public class VariableArrayDrawer : PropertyDrawer
    {
        private const float HORIZONTAL_GAP = 5;
        private const float VERTICAL_GAP = 5;

        private ReorderableList list;

        private ReorderableList GetList(SerializedProperty property)
        {
            if (list == null)
            {
                list = new ReorderableList(property.serializedObject, property, true, true, true, true);
                list.elementHeight = 21;
                list.drawElementCallback = DrawElement;
                list.drawHeaderCallback = DrawHeader;
                list.onAddDropdownCallback = OnAddElement;
                list.onRemoveCallback = OnRemoveElement;
                list.drawElementBackgroundCallback = DrawElementBackground;
            }
            else
            {
                list.serializedProperty = property;
            }
            return list;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
                return 40;

            float height = base.GetPropertyHeight(property, label) + 60;
            var variables = property.FindPropertyRelative("variables");
            for (int i = 0; i < variables.arraySize; i++)
                height += EditorGUI.GetPropertyHeight(variables.GetArrayElementAtIndex(i)) + VERTICAL_GAP;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
            {
                EditorGUI.HelpBox(position,"Components that are only on of the selected objects cannot be multi-edited", MessageType.Warning);
                return;
            }

            var list = GetList(property.FindPropertyRelative("variables"));
            list.DoList(position);
        }

        private void OnAddElement(Rect rect, ReorderableList list)
        {
            var variables = list.serializedProperty;
            int index = variables.arraySize > 0 ? variables.arraySize : 0;
            this.DrawContextMenu(variables, index);
        }

        private void OnRemoveElement(ReorderableList list)
        {
            var variables = list.serializedProperty;
            AskRemoveVariable(variables, list.index);
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, false, true);
        }

        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Variables");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var variables = list.serializedProperty;
            if (index < 0 || index >= variables.arraySize)
                return;

            var variable = variables.GetArrayElementAtIndex(index);

            float x = rect.x;
            float y = rect.y + 2;
            float width = rect.width - 40;
            float height = rect.height;

            Rect variableRect = new Rect(x, y, width, height);
            EditorGUI.PropertyField(variableRect, variable, GUIContent.none);

            var buttonLeftRect = new Rect(variableRect.xMax + HORIZONTAL_GAP, y - 1, 18, 18);
            var buttonRightRect = new Rect(buttonLeftRect.xMax, y - 1, 18, 18);

            if (GUI.Button(buttonLeftRect, new GUIContent("+"), EditorStyles.miniButtonLeft))
            {
                DuplicateVariable(variables, index);
            }
            if (GUI.Button(buttonRightRect, new GUIContent("-"), EditorStyles.miniButtonRight))
            {
                AskRemoveVariable(variables, index);
            }
        }

        protected virtual void DrawContextMenu(SerializedProperty variables, int index)
        {
            GenericMenu menu = new GenericMenu();
            foreach (VariableType variableType in System.Enum.GetValues(typeof(VariableType)))
            {
                var type = variableType;
                menu.AddItem(new GUIContent(variableType.ToString()), false, context =>
                {
                    AddVariable(variables, index, type);
                }, null);
            }
            menu.ShowAsContext();
        }

        protected virtual void AddVariable(SerializedProperty variables, int index, VariableType type)
        {
            if (index < 0 || index > variables.arraySize)
                return;

            variables.serializedObject.Update();
            variables.InsertArrayElementAtIndex(index);
            SerializedProperty variableProperty = variables.GetArrayElementAtIndex(index);
            variableProperty.FindPropertyRelative("variableType").enumValueIndex = (int)type;

            variableProperty.FindPropertyRelative("name").stringValue = "";
            variableProperty.FindPropertyRelative("objectValue").objectReferenceValue = null;
            variableProperty.FindPropertyRelative("dataValue").stringValue = "";

            variables.serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        protected virtual void DuplicateVariable(SerializedProperty variables, int index)
        {
            if (index < 0 || index >= variables.arraySize)
                return;

            variables.serializedObject.Update();
            variables.InsertArrayElementAtIndex(index);
            SerializedProperty variableProperty = variables.GetArrayElementAtIndex(index + 1);

            variableProperty.FindPropertyRelative("name").stringValue = "";
            variableProperty.FindPropertyRelative("objectValue").objectReferenceValue = null;
            variableProperty.FindPropertyRelative("dataValue").stringValue = "";

            variables.serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        protected virtual void AskRemoveVariable(SerializedProperty variables, int index)
        {
            if (variables == null || index < 0 || index >= variables.arraySize)
                return;

            var variable = variables.GetArrayElementAtIndex(index);
            var name = variable.FindPropertyRelative("name").stringValue;
            if (string.IsNullOrEmpty(name))
            {
                RemoveVariable(variables, index);
                return;
            }

            if (EditorUtility.DisplayDialog("Confirm delete", string.Format("Are you sure you want to delete the item named \"{0}\"?", name), "Yes", "Cancel"))
            {
                RemoveVariable(variables, index);
            }
        }

        protected virtual void RemoveVariable(SerializedProperty variables, int index)
        {
            if (index < 0 || index >= variables.arraySize)
                return;

            variables.serializedObject.Update();
            variables.DeleteArrayElementAtIndex(index);
            variables.serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }
    }
}