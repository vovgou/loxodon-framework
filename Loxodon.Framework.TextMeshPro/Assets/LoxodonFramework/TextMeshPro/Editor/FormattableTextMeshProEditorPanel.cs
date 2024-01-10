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

using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace Loxodon.Framework.Views.TextMeshPro.Editor
{
    [CustomEditor(typeof(FormattableTextMeshPro), true), CanEditMultipleObjects]
    public class FormattableTextMeshProEditorPanel : TMP_EditorPanel
    {
        static readonly GUIContent k_FormatLabel = new GUIContent("Format", "text formatting");
        static readonly GUIContent k_ParameterCountLabel = new GUIContent("Parameter Count", "Parameter Count");

        SerializedProperty m_FormatProp;
        SerializedProperty m_ParameterCountProp;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_FormatProp = serializedObject.FindProperty("m_Format");
            m_ParameterCountProp = serializedObject.FindProperty("m_ParameterCount");
        }

        public override void OnInspectorGUI()
        {
            // Make sure Multi selection only includes TMP Text objects.
            if (IsMixSelectionTypes()) return;

            serializedObject.Update();

            DrawFormatParameters();

            DrawMainSettings();

            DrawExtraSettings();

            EditorGUILayout.Space();

            if (serializedObject.ApplyModifiedProperties() || m_HavePropertiesChanged)
            {
                m_TextComponent.havePropertiesChanged = true;
                m_HavePropertiesChanged = false;
                EditorUtility.SetDirty(target);
            }
        }

        protected void DrawFormatParameters()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_FormatProp, k_FormatLabel);
            if (EditorGUI.EndChangeCheck())
            {
                m_HavePropertiesChanged = true;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_ParameterCountProp, k_ParameterCountLabel);
            if (EditorGUI.EndChangeCheck())
            {
                m_HavePropertiesChanged = true;
            }
        }
    }
}
