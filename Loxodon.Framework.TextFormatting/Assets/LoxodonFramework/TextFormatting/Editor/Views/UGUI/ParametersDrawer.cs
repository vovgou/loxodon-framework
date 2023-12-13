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

using Loxodon.Framework.Views.UGUI;
using UnityEditor;
using UnityEngine;

namespace Loxodon.Framework.TextFormatting.Editors
{
    [CustomPropertyDrawer(typeof(Parameters), true)]
    public class ParametersDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty textProperty = property.FindPropertyRelative("m_Text");
            return base.GetPropertyHeight(textProperty, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty textProperty = property.FindPropertyRelative("m_Text");
            SerializedProperty capacityProperty = property.FindPropertyRelative("m_Capacity");
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, textProperty, label);
            if (EditorGUI.EndChangeCheck())
            {
                if (textProperty.objectReferenceValue != null)
                {
                    FormattableText formableText = (FormattableText)textProperty.objectReferenceValue;
                    if (capacityProperty != null)
                    {
                        int count = formableText.ParameterCount;
                        capacityProperty.intValue = count;
                    }
                }
            }
            EditorGUI.EndProperty();
        }
    }
}