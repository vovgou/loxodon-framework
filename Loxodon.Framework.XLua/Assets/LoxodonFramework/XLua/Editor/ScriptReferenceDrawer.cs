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
using Loxodon.Framework.XLua;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Loxodon.Framework.Editors
{
    [CustomPropertyDrawer(typeof(ScriptReference))]
    public class ScriptReferenceDrawer : PropertyDrawer
    {
        private const float HORIZONTAL_GAP = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var objectProperty = property.FindPropertyRelative("cachedAsset");
            var typeProperty = property.FindPropertyRelative("type");
            var textProperty = property.FindPropertyRelative("text");
            var filenameProperty = property.FindPropertyRelative("filename");

            float y = position.y;
            float x = position.x;
            float height = GetPropertyHeight(property, label);
            float width = position.width - HORIZONTAL_GAP * 2;

            Rect nameRect = new Rect(x, y, 60, height);
            Rect typeRect = new Rect(nameRect.xMax + HORIZONTAL_GAP, y, 80, height);
            Rect valueRect = new Rect(typeRect.xMax + HORIZONTAL_GAP, y, position.xMax - typeRect.xMax - HORIZONTAL_GAP, height);

            EditorGUI.LabelField(nameRect, property.displayName);

            Object asset = objectProperty.objectReferenceValue;
            ScriptReferenceType typeValue = (ScriptReferenceType)typeProperty.enumValueIndex;
            EditorGUI.BeginChangeCheck();
            ScriptReferenceType newTypeValue = (ScriptReferenceType)EditorGUI.EnumPopup(typeRect, typeValue);
            if (EditorGUI.EndChangeCheck() && typeValue != newTypeValue)
            {
                if (ValidateSetting(asset, newTypeValue))
                {
                    typeProperty.enumValueIndex = (int)newTypeValue;
                    UpdateProperty(filenameProperty, textProperty, newTypeValue, asset);
                }
            }

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0.1f;

            EditorGUI.BeginChangeCheck();
            Object newAsset = null;
            switch (newTypeValue)
            {
                case ScriptReferenceType.Filename:
                    {
                        if (asset != null)
                        {
                            var name = asset.name;
                            asset.name = filenameProperty.stringValue;
                            newAsset = EditorGUI.ObjectField(valueRect, GUIContent.none, asset, typeof(Object), false);
                            asset.name = name;
                        }
                        else
                        {
                            newAsset = EditorGUI.ObjectField(valueRect, GUIContent.none, asset, typeof(Object), false);
                        }
                        break;
                    }
                case ScriptReferenceType.TextAsset:
                    {
                        if (asset is TextAsset)
                            newAsset = EditorGUI.ObjectField(valueRect, GUIContent.none, asset, typeof(TextAsset), false);
                        else
                            newAsset = EditorGUI.ObjectField(valueRect, GUIContent.none, null, typeof(TextAsset), false);
                        break;
                    }
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (ValidateAsset(newAsset) && ValidateSetting(newAsset, newTypeValue))
                {
                    objectProperty.objectReferenceValue = newAsset;
                    UpdateProperty(filenameProperty, textProperty, newTypeValue, newAsset);
                }
            }

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.EndProperty();
        }

        protected virtual bool ValidateAsset(Object asset)
        {
            if (asset == null)
                return true;

            if (!(asset is TextAsset || asset is DefaultAsset))
            {
                Debug.LogWarningFormat("Invalid asset for ScriptReference");
                return false;
            }

            string path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path))
                return false;

            if (asset is DefaultAsset && Directory.Exists(path))
            {
                Debug.LogWarningFormat("Invalid asset for ScriptReference path = '{0}'.", path);
                return false;
            }

            if (path.EndsWith(".cs"))
            {
                Debug.LogWarningFormat("Invalid asset for ScriptReference path = '{0}'.", path);
                return false;
            }
            return true;
        }

        protected virtual bool ValidateSetting(Object asset, ScriptReferenceType type)
        {
            if (asset == null || type == ScriptReferenceType.TextAsset)
                return true;

            string path = AssetDatabase.GetAssetPath(asset);
            LuaSettings luaSettings = LuaSettings.GetOrCreateSettings();
            foreach (string root in luaSettings.SrcRoots)
            {
                if (path.StartsWith(root))
                    return true;
            }

            if (path.IndexOf("Resources") >= 0)
                return true;

            if (EditorUtility.DisplayDialog("Notice", string.Format("The file \"{0}\" is not in the source code folder of lua. Do you want to add a source code folder?", asset.name), "Yes", "Cancel"))
            {
                SettingsService.OpenProjectSettings("Project/LuaSettingsProvider");
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual void UpdateProperty(SerializedProperty filenameProperty, SerializedProperty textProperty, ScriptReferenceType type, Object asset)
        {
            switch (type)
            {
                case ScriptReferenceType.TextAsset:
                    if (asset != null && asset is TextAsset)
                        textProperty.objectReferenceValue = (TextAsset)asset;
                    else
                        textProperty.objectReferenceValue = null;
                    filenameProperty.stringValue = null;
                    break;
                case ScriptReferenceType.Filename:
                    if (asset != null)
                        filenameProperty.stringValue = GetFilename(asset);
                    else
                        filenameProperty.stringValue = null;
                    textProperty.objectReferenceValue = null;
                    break;
            }
        }

        protected virtual string GetFilename(Object asset)
        {
            if (asset == null)
                return null;

            string path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path))
                return null;

            int start = path.LastIndexOf("/");
            int dotIndex = path.IndexOf(".", start);
            if (dotIndex > -1)
                path = path.Substring(0, dotIndex);

            LuaSettings luaSettings = LuaSettings.GetOrCreateSettings();
            foreach (string root in luaSettings.SrcRoots)
            {
                if (path.StartsWith(root))
                {
                    path = path.Replace(root + "/", "").Replace("/", ".");
                    return path;
                }
            }

            int index = path.IndexOf("Resources");
            if (index >= 0)
                path = path.Substring(index + 10);

            path = path.Replace("/", ".");
            return path;
        }
    }
}