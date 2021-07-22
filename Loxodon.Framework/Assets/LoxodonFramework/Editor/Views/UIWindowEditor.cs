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
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Loxodon.Framework.Editors
{
    [CustomEditor(typeof(Window), true)]

    public class UIWindowEditor : Editor
    {
        [SerializeField]
        private bool foldout = true;
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            SerializedProperty property = serializedObject.GetIterator();
            var windowTypeProperty = serializedObject.FindProperty("windowType");

            WindowType windowType = (WindowType)windowTypeProperty.enumValueIndex;
            foldout = EditorGUILayout.Foldout(foldout, new GUIContent("Window Settings", ""));

            List<GUIContent> windowSettings = new List<GUIContent>()
            {
                new GUIContent("windowType","Type of Window"),
                new GUIContent("windowPriority","When pop-up windows are queued to open, windows with higher priority will be opened first."),
                new GUIContent("stateBroadcast","If true, the window state broadcasting feature is enabled.")
            };

            bool expanded = true;
            while (property.NextVisible(expanded))
            {
                using (new EditorGUI.DisabledScope("m_Script" == property.propertyPath))
                {
                    if ("m_Script" == property.propertyPath)
                        continue;

                    GUIContent propertyContent = windowSettings.Find(c => c.text.Equals(property.propertyPath));
                    if (propertyContent != null)
                    {
                        if (foldout)
                        {
                            if ("windowPriority" == property.propertyPath && windowType != WindowType.QUEUED_POPUP)
                                continue;

                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(property, propertyContent);
                            EditorGUI.indentLevel--;
                        }
                        continue;
                    }

                    EditorGUILayout.PropertyField(property, true);
                }
                expanded = false;
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}