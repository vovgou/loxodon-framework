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
using System;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Loxodon.Framework.TextUGUI.Editors
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

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Format);
            if (EditorGUI.EndChangeCheck())
            {
                try
                {
                    int count = ParamsCount(m_Format.stringValue);
                    m_ParameterCount.intValue = count;
                }
                catch (Exception) { }
            }
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_ParameterCount);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(m_FontData);

            AppearanceControlsGUI();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }

        private static int ParamsCount(string format)
        {
            if (string.IsNullOrEmpty(format))
                return 0;

            int pos = 0;
            int len = format.Length;
            char ch = '\x0';
            int maxIndex = -1;
            while (true)
            {
                while (pos < len)
                {
                    ch = format[pos];
                    pos++;
                    if (ch == '}')
                    {
                        if (pos < len && format[pos] == '}') // Treat as escape character for }}
                            pos++;
                        else
                            FormatError();
                    }

                    if (ch == '{')
                    {
                        if (pos < len && format[pos] == '{') // Treat as escape character for {{
                            pos++;
                        else
                        {
                            pos--;
                            break;
                        }
                    }
                }

                if (pos == len)
                    break;

                pos++;
                if (pos == len || (ch = format[pos]) < '0' || ch > '9')
                    FormatError();
                int index = 0;
                do
                {
                    index = index * 10 + ch - '0';
                    pos++;
                    if (pos == len)
                        FormatError();
                    ch = format[pos];
                } while (ch >= '0' && ch <= '9' && index < 1000000);

                maxIndex = Math.Max(maxIndex, index);

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;

                int width = 0;
                if (ch == ',')
                {
                    pos++;
                    while (pos < len && format[pos] == ' ')
                        pos++;

                    if (pos == len)
                        FormatError();
                    ch = format[pos];
                    if (ch == '-')
                    {
                        pos++;
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                    }
                    if (ch < '0' || ch > '9')
                        FormatError();
                    do
                    {
                        width = width * 10 + ch - '0';
                        pos++;
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                    } while (ch >= '0' && ch <= '9' && width < 1000000);
                }

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;

                if (ch == ':')
                {
                    pos++;
                    while (true)
                    {
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                        if (!IsValidFormatChar(ch))
                            break;

                        pos++;
                    }
                }

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;

                if (ch != '}')
                    FormatError();
                pos++;
            }
            return maxIndex + 1;
        }

        private static bool IsValidFormatChar(char ch)
        {
            if (ch == 123 || ch == 125)//{ } 
                return false;

            if ((ch >= 32 && ch <= 122) || ch == 124)
                return true;
            return false;
        }

        private static void FormatError()
        {
            throw new FormatException("Invalid Format");
        }
    }
}