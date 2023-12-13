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
using UnityEditor.UI;

namespace Loxodon.Framework.TextFormatting.Editors
{
    [CustomEditor(typeof(FormattableText), true)]
    [CanEditMultipleObjects]
    public class FormattableTextEditor : GraphicEditor
    {
        SerializedProperty m_Format;
        SerializedProperty m_FontData;
        SerializedProperty m_ParameterCount;
        protected override void OnEnable()
        {
            base.OnEnable();
            m_Format = serializedObject.FindProperty("m_Format");
            m_FontData = serializedObject.FindProperty("m_FontData");
            m_ParameterCount = serializedObject.FindProperty("m_ParameterCount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Format);
            EditorGUILayout.PropertyField(m_ParameterCount);
            EditorGUILayout.PropertyField(m_FontData);

            AppearanceControlsGUI();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}