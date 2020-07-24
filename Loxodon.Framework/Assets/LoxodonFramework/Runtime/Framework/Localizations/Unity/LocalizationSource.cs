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
using UnityEngine;
using Object = UnityEngine.Object;

namespace Loxodon.Framework.Localizations
{
    [Serializable]
    public abstract class LocalizationSource
    {
    }

    [Serializable]
    public class MonolingualSource : LocalizationSource
    {
        [SerializeField]
        private List<MonolingualEntry> entries = new List<MonolingualEntry>();

        public List<MonolingualEntry> Entries { get { return this.entries; } }

        public Dictionary<string, object> GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (this.entries == null || this.entries.Count <= 0)
                return data;

            foreach (var entry in entries)
            {
                var key = entry.Key;
                var value = entry.GetValue();

                if (string.IsNullOrEmpty(key))
                    continue;

                data[key] = value;
            }
            return data;
        }
    }

    [Serializable]
    public class MultilingualSource : LocalizationSource
    {
        [SerializeField]
        private List<string> languages = new List<string>();

        [SerializeField]
        private List<MultilingualEntry> entries = new List<MultilingualEntry>();

        public List<string> Languages { get { return this.languages; } }

        public List<MultilingualEntry> Entries { get { return this.entries; } }

        public Dictionary<string, object> GetData(string language)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (this.entries == null || this.entries.Count <= 0)
                return data;

            int index = this.languages.IndexOf(language);
            if (index < 0)
                return data;

            foreach (var entry in entries)
            {
                var key = entry.Key;
                var value = entry.GetValue(index);
                if (string.IsNullOrEmpty(key))
                    continue;
                data[key] = value;
            }
            return data;
        }

        public bool AddLanguage(string language)
        {
            if (languages.Contains(language))
                return false;

            languages.Add(language);

            int index = this.languages.Count - 1;
            foreach (var entry in this.entries)
            {
                entry.SetValue(index, null);
            }
            return true;
        }

        public bool RemoveLanguage(string language)
        {
            int index = this.languages.IndexOf(language);
            if (index < 0)
                return false;

            languages.RemoveAt(index);
            foreach (var entry in this.entries)
            {
                entry.RemoveValue(index);
            }
            return true;
        }
    }

    public abstract class EntryBase
    {
        [SerializeField]
        protected string key;

        [SerializeField]
        protected ValueType type;

        public string Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public ValueType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
    }

    [Serializable]
    public class Value
    {
        [SerializeField]
        public string dataValue;

        [SerializeField]
        public Object objectValue;

        public Value()
        {
        }
    }

    [Serializable]
    public class MonolingualEntry : EntryBase
    {
        [SerializeField]
        private Value value;

        public object GetValue()
        {
            if (this.value == null)
                return null;

            string val = this.value.dataValue;
            Object obj = this.value.objectValue;
            switch (type)
            {
                case ValueType.Sprite:
                case ValueType.Texture2D:
                case ValueType.Texture3D:
                case ValueType.AudioClip:
                case ValueType.VideoClip:
                case ValueType.Material:
                case ValueType.Font:
                case ValueType.GameObject:
                    return obj;
                case ValueType.String:
                    return DataConverter.ToString(val);
                case ValueType.Boolean:
                    return DataConverter.ToBoolean(val);
                case ValueType.Float:
                    return DataConverter.ToSingle(val);
                case ValueType.Int:
                    return DataConverter.ToInt32(val);
                case ValueType.Color:
                    return DataConverter.ToColor(val);
                case ValueType.Vector2:
                    return DataConverter.ToVector2(val);
                case ValueType.Vector3:
                    return DataConverter.ToVector3(val);
                case ValueType.Vector4:
                    return DataConverter.ToVector4(val);
                default:
                    return null;
            }
        }

        public void SetValue(object value)
        {
            if (this.value == null)
                this.value = new Value();

            switch (type)
            {
                case ValueType.Sprite:
                case ValueType.Texture2D:
                case ValueType.Texture3D:
                case ValueType.AudioClip:
                case ValueType.VideoClip:
                case ValueType.Material:
                case ValueType.Font:
                case ValueType.GameObject:
                    this.value.objectValue = (Object)value;
                    break;
                case ValueType.String:
                    this.value.dataValue = DataConverter.GetString((string)value);
                    break;
                case ValueType.Boolean:
                    this.value.dataValue = DataConverter.GetString((bool)value);
                    break;
                case ValueType.Float:
                    this.value.dataValue = DataConverter.GetString((float)value);
                    break;
                case ValueType.Int:
                    this.value.dataValue = DataConverter.GetString((int)value);
                    break;
                case ValueType.Color:
                    this.value.dataValue = DataConverter.GetString((Color)value);
                    break;
                case ValueType.Vector2:
                    this.value.dataValue = DataConverter.GetString((Vector2)value);
                    break;
                case ValueType.Vector3:
                    this.value.dataValue = DataConverter.GetString((Vector3)value);
                    break;
                case ValueType.Vector4:
                    this.value.dataValue = DataConverter.GetString((Vector4)value);
                    break;
                default:
                    break;
            }
        }
    }

    [Serializable]
    public class MultilingualEntry : EntryBase
    {
        [SerializeField]
        private List<Value> values;

        public object GetValue(int index)
        {
            if (values == null || values.Count <= 0)
                return null;

            if (index < 0 || index >= values.Count)
                return null;

            Value value = this.values[index];
            if (value == null)
                return null;

            string data = value.dataValue;
            switch (type)
            {
                case ValueType.Sprite:
                case ValueType.Texture2D:
                case ValueType.Texture3D:
                case ValueType.AudioClip:
                case ValueType.VideoClip:
                case ValueType.Material:
                case ValueType.Font:
                case ValueType.GameObject:
                    return value.objectValue;
                case ValueType.String:
                    return DataConverter.ToString(data);
                case ValueType.Boolean:
                    return DataConverter.ToBoolean(data);
                case ValueType.Float:
                    return DataConverter.ToSingle(data);
                case ValueType.Int:
                    return DataConverter.ToInt32(data);
                case ValueType.Color:
                    return DataConverter.ToColor(data);
                case ValueType.Vector2:
                    return DataConverter.ToVector2(data);
                case ValueType.Vector3:
                    return DataConverter.ToVector3(data);
                case ValueType.Vector4:
                    return DataConverter.ToVector4(data);
                default:
                    return null;
            }
        }

        public void SetValue(int index, object obj)
        {
            if (values == null)
                this.values = new List<Value>();

            if (index < 0 || index > values.Count)
                return;

            if (index == values.Count)
                this.values.Add(new Value());

            Value value = this.values[index];
            if (value == null)
            {
                value = new Value();
                values[index] = value;
            }

            switch (type)
            {
                case ValueType.Sprite:
                case ValueType.Texture2D:
                case ValueType.Texture3D:
                case ValueType.AudioClip:
                case ValueType.VideoClip:
                case ValueType.Material:
                case ValueType.Font:
                case ValueType.GameObject:
                    value.objectValue = (Object)obj;
                    break;
                case ValueType.String:
                    value.dataValue = DataConverter.GetString((string)obj);
                    break;
                case ValueType.Boolean:
                    value.dataValue = DataConverter.GetString((bool)obj);
                    break;
                case ValueType.Float:
                    value.dataValue = DataConverter.GetString((float)obj);
                    break;
                case ValueType.Int:
                    value.dataValue = DataConverter.GetString((int)obj);
                    break;
                case ValueType.Color:
                    value.dataValue = DataConverter.GetString((Color)obj);
                    break;
                case ValueType.Vector2:
                    value.dataValue = DataConverter.GetString((Vector2)obj);
                    break;
                case ValueType.Vector3:
                    value.dataValue = DataConverter.GetString((Vector3)obj);
                    break;
                case ValueType.Vector4:
                    value.dataValue = DataConverter.GetString((Vector4)obj);
                    break;
                default:
                    break;
            }
        }

        public void RemoveValue(int index)
        {
            if (values == null || values.Count <= 0)
                return;

            if (index < 0 || index >= values.Count)
                return;

            this.values.RemoveAt(index);
        }
    }

    [Serializable]
    public enum ValueType
    {
        String = 0,
        Boolean = 1,
        Int = 2,
        Float = 3,
        Color = 4,
        Vector2 = 5,
        Vector3 = 6,
        Vector4 = 7,
        Sprite = 8,
        Texture2D = 9,
        Texture3D = 10,
        AudioClip = 11,
        VideoClip = 12,
        Material = 13,
        Font = 14,
        GameObject = 15
    }
}
