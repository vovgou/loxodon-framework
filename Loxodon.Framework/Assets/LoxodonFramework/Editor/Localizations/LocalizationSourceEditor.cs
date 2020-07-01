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
using UnityEngine;
using UnityEngine.Video;

using Object = UnityEngine.Object;
using ValueType = Loxodon.Framework.Localizations.ValueType;

namespace Loxodon.Framework.Editors
{
    public abstract class LocalizationSourceEditor : Editor
    {
        protected const float HORIZONTAL_GAP = 5;
        protected const float VERTICAL_GAP = 5;

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
        protected virtual void DrawValueField(Rect rect, SerializedProperty valueProperty, ValueType type, bool multiline = false)
        {
            SerializedProperty objectValue = valueProperty.FindPropertyRelative("objectValue");
            SerializedProperty dataValue = valueProperty.FindPropertyRelative("dataValue");

            switch (type)
            {
                case ValueType.String:
                    string strValue = DataConverter.ToString(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    if (multiline)
                        strValue = EditorGUI.TextArea(rect, strValue, EditorStyles.textArea);
                    else
                        strValue = EditorGUI.TextField(rect, GUIContent.none, strValue, EditorStyles.textField);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(strValue);
                    }
                    break;
                case ValueType.Boolean:
                    bool boolValue = DataConverter.ToBoolean(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    boolValue = EditorGUI.Toggle(rect, GUIContent.none, boolValue, EditorStyles.toggle);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(boolValue);
                    }
                    break;
                case ValueType.Int:
                    int intValue = DataConverter.ToInt32(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    intValue = EditorGUI.IntField(rect, GUIContent.none, intValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(intValue);
                    }
                    break;
                case ValueType.Float:
                    float floatValue = DataConverter.ToSingle(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    floatValue = EditorGUI.FloatField(rect, GUIContent.none, floatValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(floatValue);
                    }
                    break;
                case ValueType.Color:
                    Color color = DataConverter.ToColor(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    color = EditorGUI.ColorField(rect, GUIContent.none, color);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(color);
                    }
                    break;
                case ValueType.Vector2:
                    Vector2 vector2 = DataConverter.ToVector2(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    vector2 = EditorGUI.Vector2Field(rect, GUIContent.none, vector2);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(vector2);
                    }
                    break;
                case ValueType.Vector3:
                    Vector3 vector3 = DataConverter.ToVector3(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    vector3 = EditorGUI.Vector3Field(rect, GUIContent.none, vector3);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(vector3);
                    }
                    break;
                case ValueType.Vector4:
                    Vector4 vector4 = DataConverter.ToVector4(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    vector4 = EditorGUI.Vector4Field(rect, GUIContent.none, vector4);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(vector4);
                    }
                    break;
                case ValueType.Sprite:
                case ValueType.Texture2D:
                case ValueType.Texture3D:
                case ValueType.AudioClip:
                case ValueType.VideoClip:
                case ValueType.Material:
                case ValueType.Font:
                case ValueType.GameObject:
                    var objType = GetObjectType(type);
                    objectValue.objectReferenceValue = EditorGUI.ObjectField(rect, GUIContent.none, objectValue.objectReferenceValue, objType, false);
                    break;
                default:
                    break;
            }
        }

        protected virtual Type GetObjectType(ValueType type)
        {
            switch (type)
            {
                case ValueType.Sprite:
                    return typeof(Sprite);
                case ValueType.Texture2D:
                    return typeof(Texture2D);
                case ValueType.Texture3D:
                    return typeof(Texture3D);
                case ValueType.AudioClip:
                    return typeof(AudioClip);
                case ValueType.VideoClip:
                    return typeof(VideoClip);
                case ValueType.Material:
                    return typeof(Material);
                case ValueType.Font:
                    return typeof(Font);
                case ValueType.GameObject:
                    return typeof(GameObject);
                default:
                    return typeof(Object);
            }
        }
    }
}
