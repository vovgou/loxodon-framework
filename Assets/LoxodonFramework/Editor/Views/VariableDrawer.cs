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
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Loxodon.Framework.Editors
{
    [CustomPropertyDrawer(typeof(Variable))]
    public class VariableDrawer : PropertyDrawer
    {
        private const float HORIZONTAL_GAP = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var name = property.FindPropertyRelative("name");
            var objectValue = property.FindPropertyRelative("objectValue");
            var dataValue = property.FindPropertyRelative("dataValue");
            var variableType = property.FindPropertyRelative("variableType");

            float y = position.y;
            float x = position.x;
            float height = GetPropertyHeight(property, label);
            float width = position.width - HORIZONTAL_GAP * 2;

            Rect nameRect = new Rect(x, y, Mathf.Min(200, width * 0.4f), height);
            Rect typeRect = new Rect(nameRect.xMax + HORIZONTAL_GAP, y, Mathf.Min(120, width * 0.2f), height);
            Rect valueRect = new Rect(typeRect.xMax + HORIZONTAL_GAP, y, position.xMax - typeRect.xMax - HORIZONTAL_GAP, height);

            EditorGUI.PropertyField(nameRect, name, GUIContent.none);

            VariableType variableTypeValue = (VariableType)variableType.enumValueIndex;

            if (variableTypeValue == VariableType.Component)
            {
                int index = 0;
                List<System.Type> types = new List<System.Type>();
                var component = (Component)objectValue.objectReferenceValue;
                if (component != null)
                {
                    GameObject go = component.gameObject;
                    foreach (var c in go.GetComponents<Component>())
                    {
                        if (!types.Contains(c.GetType()))
                            types.Add(c.GetType());
                    }

                    for (int i = 0; i < types.Count; i++)
                    {
                        if (component.GetType().Equals(types[i]))
                        {
                            index = i;
                            break;
                        }
                    }
                }

                if (types.Count <= 0)
                    types.Add(typeof(Transform));

                List<GUIContent> contents = new List<GUIContent>();
                foreach (var t in types)
                {
                    contents.Add(new GUIContent(t.Name, t.FullName));
                }

                EditorGUI.BeginChangeCheck();
                var newIndex = EditorGUI.Popup(typeRect, GUIContent.none, index, contents.ToArray(), EditorStyles.popup);
                if (EditorGUI.EndChangeCheck())
                {
                    if (component != null)
                        objectValue.objectReferenceValue = component.gameObject.GetComponent(types[newIndex]);
                    else
                        objectValue.objectReferenceValue = null;
                }
            }
            else
            {
                EditorGUI.LabelField(typeRect, variableTypeValue.ToString());
            }

            switch (variableTypeValue)
            {
                case VariableType.Component:
                    EditorGUI.BeginChangeCheck();
                    objectValue.objectReferenceValue = EditorGUI.ObjectField(valueRect, GUIContent.none, objectValue.objectReferenceValue, typeof(UnityEngine.Component), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (string.IsNullOrEmpty(name.stringValue) && objectValue.objectReferenceValue != null)
                            name.stringValue = NormalizeName(objectValue.objectReferenceValue.name);
                    }
                    break;
                case VariableType.GameObject:
                    EditorGUI.BeginChangeCheck();
                    objectValue.objectReferenceValue = EditorGUI.ObjectField(valueRect, GUIContent.none, objectValue.objectReferenceValue, typeof(UnityEngine.GameObject), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (string.IsNullOrEmpty(name.stringValue) && objectValue.objectReferenceValue != null)
                            name.stringValue = NormalizeName(objectValue.objectReferenceValue.name);
                    }
                    break;
                case VariableType.Object:
                    EditorGUI.BeginChangeCheck();
                    objectValue.objectReferenceValue = EditorGUI.ObjectField(valueRect, GUIContent.none, objectValue.objectReferenceValue, typeof(UnityEngine.Object), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (string.IsNullOrEmpty(name.stringValue) && objectValue.objectReferenceValue != null)
                            name.stringValue = NormalizeName(objectValue.objectReferenceValue.name);
                    }
                    break;
                case VariableType.Color:
                    Color color = DataConverter.ToColor(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    color = EditorGUI.ColorField(valueRect, GUIContent.none, color);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(color);
                    }
                    break;
                case VariableType.Vector2:
                    Vector2 vector2 = DataConverter.ToVector2(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    vector2 = EditorGUI.Vector2Field(valueRect, GUIContent.none, vector2);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(vector2);
                    }
                    break;
                case VariableType.Vector3:
                    Vector3 vector3 = DataConverter.ToVector3(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    vector3 = EditorGUI.Vector3Field(valueRect, GUIContent.none, vector3);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(vector3);
                    }
                    break;
                case VariableType.Vector4:
                    Vector4 vector4 = DataConverter.ToVector4(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
#if UNITY_5_6_OR_NEWER
                    vector4 = EditorGUI.Vector4Field(valueRect, GUIContent.none, vector4);
#else
                    var tmpRect = valueRect;
                    tmpRect.y -= height;
                    vector4 = EditorGUI.Vector4Field(tmpRect, "", vector4);
#endif
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(vector4);
                    }
                    break;
                case VariableType.Boolean:
                    bool b = DataConverter.ToBoolean(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    b = EditorGUI.Toggle(valueRect, GUIContent.none, b);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(b);
                    }
                    break;
                case VariableType.Float:
                    float f = DataConverter.ToSingle(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    f = EditorGUI.FloatField(valueRect, GUIContent.none, f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(f);
                    }
                    break;
                case VariableType.Integer:
                    int i = DataConverter.ToInt32(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    i = EditorGUI.IntField(valueRect, GUIContent.none, i);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(i);
                    }
                    break;
                case VariableType.String:
                    string s = DataConverter.ToString(dataValue.stringValue);
                    EditorGUI.BeginChangeCheck();
                    s = EditorGUI.TextField(valueRect, GUIContent.none, s);
                    if (EditorGUI.EndChangeCheck())
                    {
                        dataValue.stringValue = DataConverter.GetString(s);
                    }
                    break;
                default:
                    break;
            }
            EditorGUI.EndProperty();
        }


        protected virtual string NormalizeName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "";

            name = name.Replace(" ", "");
            return char.ToLower(name[0]) + name.Substring(1);
        }
    }
}