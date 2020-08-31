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

using Loxodon.Framework.XLua;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace Loxodon.Framework.Editors
{
    class LuaSettingsProvider : SettingsProvider
    {
        private const float HORIZONTAL_GAP = 5;
        private const float VERTICAL_GAP = 5;

        private SerializedObject m_LuaSettings;
        private ReorderableList m_List;

        private Color transparentColor = new Color(1f, 1f, 1f, 0f);

        public LuaSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope)
        {
        }

        //public static bool IsSettingsAvailable()
        //{
        //    return File.Exists(LuaSettings.LUA_SETTINGS_PATH);
        //}

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_LuaSettings = LuaSettings.GetSerializedSettings();
            var rootsProperty = m_LuaSettings.FindProperty("srcRoots");

            m_List = new ReorderableList(rootsProperty.serializedObject, rootsProperty, true, true, true, true);
            m_List.elementHeight = 21;
            m_List.drawElementCallback = DrawElement;
            m_List.drawHeaderCallback = DrawHeader;
            m_List.onAddDropdownCallback = OnAddElement;
            m_List.onRemoveCallback = OnRemoveElement;
            m_List.drawElementBackgroundCallback = DrawElementBackground;
        }

        public override void OnGUI(string searchContext)
        {
            m_LuaSettings.Update();
            m_List.DoLayoutList();
            m_LuaSettings.ApplyModifiedProperties();
        }

        private void OnAddElement(Rect rect, ReorderableList list)
        {
            var roots = list.serializedProperty;
            int index = roots.arraySize > 0 ? roots.arraySize : 0;
            AddSrcRoot(roots, index);
        }

        private void OnRemoveElement(ReorderableList list)
        {
            var roots = list.serializedProperty;
            AskRemoveSrcRoot(roots, list.index);
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, false, true);
        }

        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "The root folders of Lua's source code");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var roots = m_List.serializedProperty;
            if (index < 0 || index >= roots.arraySize)
                return;

            var root = roots.GetArrayElementAtIndex(index);
            string path = AssetDatabase.GetAssetPath(root.objectReferenceValue);

            float x = rect.x;
            float y = rect.y;
            float width = rect.width;
            float height = EditorGUIUtility.singleLineHeight;

            Rect rootRect = new Rect(x, y, width, height);

            Object obj = root.objectReferenceValue;
            if (obj != null)
            {
                var name = obj.name;
                obj.name = path;
                EditorGUI.PropertyField(rootRect, root, GUIContent.none);
                obj.name = name;
            }
            else
            {
                EditorGUI.PropertyField(rootRect, root, GUIContent.none);
            }
        }

        protected virtual void AddSrcRoot(SerializedProperty roots, int index)
        {
            if (index < 0 || index > roots.arraySize)
                return;

            roots.serializedObject.Update();
            roots.InsertArrayElementAtIndex(index);
            SerializedProperty rootProperty = roots.GetArrayElementAtIndex(index);
            rootProperty.objectReferenceValue = null;

            roots.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void AskRemoveSrcRoot(SerializedProperty roots, int index)
        {
            if (roots == null || index < 0 || index >= roots.arraySize)
                return;

            var root = roots.GetArrayElementAtIndex(index);
            Object asset = root.objectReferenceValue;
            var path = asset != null ? AssetDatabase.GetAssetPath(asset) : "";
            if (string.IsNullOrEmpty(path))
            {
                RemoveSrcRoot(roots, index);
                return;
            }

            if (EditorUtility.DisplayDialog("Confirm delete", string.Format("Are you sure you want to delete the path \"{0}\"?", path), "Yes", "Cancel"))
            {
                RemoveSrcRoot(roots, index);
            }
        }

        protected virtual void RemoveSrcRoot(SerializedProperty roots, int index)
        {
            if (index < 0 || index >= roots.arraySize)
                return;

            roots.serializedObject.Update();
            var root = roots.GetArrayElementAtIndex(index);
            root.objectReferenceValue = null;
            roots.DeleteArrayElementAtIndex(index);
            roots.serializedObject.ApplyModifiedProperties();
        }

        [SettingsProvider]
        public static SettingsProvider CreateLuaSettingsProvider()
        {
            var provider = new LuaSettingsProvider("Project/LuaSettingsProvider", SettingsScope.Project);
            provider.label = "Lua Settings";
            return provider;
        }
    }
}