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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Video;

using Loxodon.Framework.Localizations;
using Loxodon.Framework.Binding;
using Loxodon.Log;

namespace Loxodon.Framework.Editors
{
    [CustomPropertyDrawer(typeof(LocalizedBindingDescriptionSet))]
    public class LocalizedBindingDescriptionSetDrawer : PropertyDrawer
    {
        private static readonly ILog log = LogManager.GetLogger("LocalizedBindingDescriptionSetDrawer");

        //private const float HORIZONTAL_GAP = 5;
        private const float VERTICAL_GAP = 5;

        private ReorderableList list;
        private List<TypeMeta> typeMetas;
        private object target;

        private ReorderableList GetList(SerializedProperty property)
        {
            if (list == null)
            {
                list = new ReorderableList(property.serializedObject, property, true, true, true, true);
                list.elementHeight = 22;
                list.drawElementCallback = DrawElement;
                list.drawHeaderCallback = DrawHeader;
                list.onAddCallback = OnAddElement;
                list.onRemoveCallback = OnRemoveElement;
                list.drawElementBackgroundCallback = DrawElementBackground;

                this.target = property.serializedObject.targetObject;
                typeMetas = this.CreateTypeMetas((Component)target);
            }
            else
            {
                list.serializedProperty = property;
            }
            return list;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label) + 40;
            var entries = property.FindPropertyRelative("descriptions");

            height += (EditorGUIUtility.singleLineHeight + VERTICAL_GAP) * entries.arraySize;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var list = GetList(property.FindPropertyRelative("descriptions"));
            if (list != null)
                list.DoList(position);
        }



        private void OnAddElement(ReorderableList list)
        {
            if (this.typeMetas == null || this.typeMetas.Count <= 0)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("No components are supported on the game object, please add UI components and try again");

                return;
            }

            var entries = list.serializedProperty;
            int index = entries.arraySize > 0 ? entries.arraySize : 0;
            this.DrawContextMenu(entries, index);
        }

        private void OnRemoveElement(ReorderableList list)
        {
            var entries = list.serializedProperty;
            AskRemoveEntry(entries, list.index);
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, false, true);
        }

        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Localization Data Binder");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var entries = list.serializedProperty;
            if (index < 0 || index >= entries.arraySize)
                return;

            var entry = entries.GetArrayElementAtIndex(index);

            var typeNameProperty = entry.FindPropertyRelative("TypeName");
            var propertyNameProperty = entry.FindPropertyRelative("PropertyName");
            var keyProperty = entry.FindPropertyRelative("Key");
            var modeProperty = entry.FindPropertyRelative("Mode");

            BindingMode mode = (BindingMode)modeProperty.enumValueIndex;

            float x = rect.x;
            float y = rect.y + 2;
            float width = rect.width - 185;
            float height = EditorGUIUtility.singleLineHeight;

            Rect bindRect = new Rect(x, y, 35, height);
            Rect typeRect = new Rect(bindRect.xMax, y, Mathf.Min(100, width * 0.3f), height);
            Rect forRect = new Rect(typeRect.xMax, y, 35, height);
            Rect nameRect = new Rect(forRect.xMax, y, Mathf.Min(100, width * 0.3f), height);
            Rect toRect = new Rect(nameRect.xMax, y, 35, height);
            Rect keyRect = new Rect(toRect.xMax, y, width - typeRect.width - nameRect.width, height);
            Rect dotRect = new Rect(keyRect.xMax, y, 15, height);
            Rect modeRect = new Rect(dotRect.xMax, y, 65, height);

            string typeFullname = typeNameProperty.stringValue;
            string propertyName = propertyNameProperty.stringValue;

            string[] typeNames = typeMetas.Select(meta => meta.Type.Name).ToArray();
            TypeMeta typeMeta = typeMetas.Find(meta => meta.Type.FullName.Equals(typeFullname));

            string[] members = typeMeta != null ? typeMeta.Members.ToArray() : new string[] { propertyName };
            int nameSelectedIndex = typeMeta != null ? typeMeta.Members.FindIndex(m => m == propertyName) : 0;
            if (nameSelectedIndex < 0)
                nameSelectedIndex = 0;

            EditorGUI.LabelField(bindRect, "Bind(");
            if (typeMeta == null)
            {
                Color old = GUI.color;
                GUI.color = Color.red;
                EditorGUI.LabelField(typeRect, new GUIContent(GetTypeName(typeFullname), string.Format("The \"{0}\" component has been deleted", typeFullname)));
                GUI.color = old;
            }
            else
            {
                EditorGUI.LabelField(typeRect, new GUIContent(GetTypeName(typeFullname), typeFullname));
            }
            EditorGUI.LabelField(forRect, ").For(");

            EditorGUI.BeginChangeCheck();
            nameSelectedIndex = EditorGUI.Popup(nameRect, nameSelectedIndex, members);
            if (EditorGUI.EndChangeCheck())
            {
                propertyNameProperty.stringValue = members[nameSelectedIndex];
            }
            EditorGUI.LabelField(toRect, ").To(");

            EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
            if (string.IsNullOrEmpty(keyProperty.stringValue))
            {
                GUI.enabled = false;
                EditorGUI.LabelField(keyRect, new GUIContent("key", "Please fill in the key value of the localized object"));
                GUI.enabled = true;
            }

            EditorGUI.LabelField(dotRect, ").");

            int selectedModeIndex = mode == BindingMode.OneTime ? 1 : 0;
            EditorGUI.BeginChangeCheck();
            selectedModeIndex = EditorGUI.Popup(modeRect, selectedModeIndex, new GUIContent[] { new GUIContent("OneWay"), new GUIContent("OneTime") });
            if (EditorGUI.EndChangeCheck())
            {
                mode = selectedModeIndex == 1 ? BindingMode.OneTime : BindingMode.OneWay;
                modeProperty.enumValueIndex = (int)mode;
            }
        }

        private List<TypeMeta> CreateTypeMetas(Component target)
        {
            Type targetType = target.GetType();
            List<TypeMeta> typeMetas = new List<TypeMeta>();

            object[] attributes = targetType.GetCustomAttributes(typeof(AllowedMembersAttribute), true);
            if (attributes == null || attributes.Length <= 0)
                return new List<TypeMeta>();

            foreach (AllowedMembersAttribute attribute in attributes)
            {
                var type = attribute.Type;
                var names = attribute.Names;

                if (target.GetComponent(type) == null)
                    continue;

                List<string> members = new List<string>();
                foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (Array.IndexOf(names, propertyInfo.Name) < 0)
                        continue;

                    if (!IsValid(propertyInfo))
                        continue;

                    members.Add(propertyInfo.Name);
                }

                if (members.Count > 0)
                    typeMetas.Add(new TypeMeta(type, members));
            }
            return typeMetas;
        }

        private bool IsValid(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite)
                return false;

            var type = propertyInfo.PropertyType;
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.DateTime:
                case TypeCode.String:
                case TypeCode.Decimal:
                    return true;
                default:
                    {
                        if (type.Equals(typeof(Version)))
                            return true;
                        if (type.Equals(typeof(Color)))
                            return true;
                        if (type.Equals(typeof(Vector2)))
                            return true;
                        if (type.Equals(typeof(Vector3)))
                            return true;
                        if (type.Equals(typeof(Vector4)))
                            return true;
                        if (type.Equals(typeof(Rect)))
                            return true;
                        if (type.Equals(typeof(Material)))
                            return true;
                        if (type.Equals(typeof(Font)))
                            return true;
                        if (type.Equals(typeof(AudioClip)))
                            return true;
                        if (type.Equals(typeof(VideoClip)))
                            return true;
                        if (type.Equals(typeof(Sprite)))
                            return true;
                        if (type.Equals(typeof(Texture)))
                            return true;
                        return false;
                    }
            }
        }

        private string GetTypeName(string fullname)
        {
            int index = fullname.LastIndexOf('.');
            if (index < 0)
                return fullname;

            return fullname.Substring(index + 1);
        }

        protected virtual void DrawContextMenu(SerializedProperty entries, int index)
        {
            GenericMenu menu = new GenericMenu();
            foreach (TypeMeta typeMeta in typeMetas)
            {
                var type = typeMeta.Type;
                menu.AddItem(new GUIContent(type.FullName), false, context =>
                {
                    AddEntry(entries, index, typeMeta);
                }, null);
            }
            menu.ShowAsContext();
        }

        protected virtual void AddEntry(SerializedProperty entries, int index, TypeMeta typeMeta)
        {
            if (index < 0 || index > entries.arraySize)
                return;

            entries.serializedObject.Update();
            entries.InsertArrayElementAtIndex(index);
            SerializedProperty entryProperty = entries.GetArrayElementAtIndex(index);

            entryProperty.FindPropertyRelative("TypeName").stringValue = typeMeta.Type.FullName;
            entryProperty.FindPropertyRelative("PropertyName").stringValue = typeMeta.Members[0];
            entryProperty.FindPropertyRelative("Key").stringValue = "";
            entryProperty.FindPropertyRelative("Mode").enumValueIndex = (int)BindingMode.OneWay;

            entries.serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        protected virtual void AskRemoveEntry(SerializedProperty entries, int index)
        {
            if (entries == null || index < 0 || index >= entries.arraySize)
                return;

            var entry = entries.GetArrayElementAtIndex(index);
            var name = entry.FindPropertyRelative("PropertyName").stringValue;
            var key = entry.FindPropertyRelative("Key").stringValue;
            if (string.IsNullOrEmpty(key))
            {
                RemoveEntry(entries, index);
                return;
            }

            if (EditorUtility.DisplayDialog("Confirm delete", string.Format("Are you sure you want to delete the item named \"{0}\"?", name), "Yes", "Cancel"))
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

        protected class TypeMeta
        {
            private Type type;
            private readonly List<string> members = new List<string>();
            public TypeMeta(Type type, string[] members)
            {
                this.type = type;
                this.members.AddRange(members);
            }

            public TypeMeta(Type type, List<string> members)
            {
                this.type = type;
                this.members.AddRange(members);
            }

            public Type Type { get { return this.type; } }
            public List<string> Members { get { return this.members; } }
        }
    }
}
