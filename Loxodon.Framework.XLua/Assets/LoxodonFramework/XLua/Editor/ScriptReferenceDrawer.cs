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

using Loxodon.Framework.Views;
using UnityEditor;
using UnityEngine;

namespace Loxodon.Framework.Editors
{
    [CustomPropertyDrawer(typeof(ScriptReference))]
    public class ScriptReferenceDrawer : PropertyDrawer
    {
        private const float HORIZONTAL_GAP = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var textProperty = property.FindPropertyRelative("text");
            var filenameProperty = property.FindPropertyRelative("filename");
            var typeProperty = property.FindPropertyRelative("type");

            float y = position.y;
            float x = position.x;
            float height = GetPropertyHeight(property, label);
            float width = position.width - HORIZONTAL_GAP * 2;

            Rect nameRect = new Rect(x, y, Mathf.Min(200, width * 0.3f), height);
            Rect typeRect = new Rect(nameRect.xMax + HORIZONTAL_GAP, y, Mathf.Min(100, width * 0.2f), height);
            Rect valueRect = new Rect(typeRect.xMax + HORIZONTAL_GAP, y, position.xMax - typeRect.xMax - HORIZONTAL_GAP, height);

            EditorGUI.LabelField(nameRect, property.displayName);

            ScriptReferenceType typeValue = (ScriptReferenceType)typeProperty.enumValueIndex;
            EditorGUI.BeginChangeCheck();
            typeValue = (ScriptReferenceType)EditorGUI.EnumPopup(typeRect, typeValue);
            if (EditorGUI.EndChangeCheck())
            {
                typeProperty.enumValueIndex = (int)typeValue;
            }

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0.1f;
            switch (typeValue)
            {
                case ScriptReferenceType.TextAsset:
                    EditorGUI.PropertyField(valueRect, textProperty, GUIContent.none);
                    break;
                case ScriptReferenceType.Filename:
                    EditorGUI.PropertyField(valueRect, filenameProperty, GUIContent.none);
                    break;
            }
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.EndProperty();
        }
    }
}