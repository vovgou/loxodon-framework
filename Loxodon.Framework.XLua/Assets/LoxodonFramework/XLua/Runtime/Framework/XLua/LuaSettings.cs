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

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace Loxodon.Framework.XLua
{
    [Serializable]
    public class LuaSettings : ScriptableObject
    {
        public const string LUA_SETTINGS_PATH = "Assets/LoxodonFramework/Editor/AppData/XLua/LuaSettings.asset";

        [SerializeField]
        private List<DefaultAsset> srcRoots = new List<DefaultAsset>();

        public List<string> SrcRoots
        {
            get
            {
                if (this.srcRoots == null)
                    return new List<string>();
                return this.srcRoots.Where(asset => asset != null).Select(asset => AssetDatabase.GetAssetPath(asset)).ToList();
            }
        }

        public static LuaSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<LuaSettings>(LUA_SETTINGS_PATH);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LuaSettings>();
                FileInfo file = new FileInfo(LUA_SETTINGS_PATH);
                if (!file.Directory.Exists)
                    file.Directory.Create();

                AssetDatabase.CreateAsset(settings, LUA_SETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}
#endif