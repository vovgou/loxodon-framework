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
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using Loxodon.Log;

namespace Loxodon.Framework.Prefs
{
    /// <summary>
    /// 
    /// </summary>
    public class BinaryFilePreferencesFactory : AbstractFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public BinaryFilePreferencesFactory() : this(null, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        public BinaryFilePreferencesFactory(ISerializer serializer) : this(serializer, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="encryptor"></param>
        public BinaryFilePreferencesFactory(ISerializer serializer, IEncryptor encryptor) : base(serializer, encryptor)
        {
        }

        /// <summary>
        /// Create an instance of the BinaryFilePreferences.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Preferences Create(string name)
        {
            return new BinaryFilePreferences(name, this.Serializer, this.Encryptor);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BinaryFilePreferences : Preferences
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BinaryFilePreferences));

        private string root;
        /// <summary>
        /// cache.
        /// </summary>
        protected readonly Dictionary<string, string> dict = new Dictionary<string, string>();

        /// <summary>
        /// serializer
        /// </summary>
        protected readonly ISerializer serializer;
        /// <summary>
        /// encryptor
        /// </summary>
        protected readonly IEncryptor encryptor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serializer"></param>
        /// <param name="encryptor"></param>
        public BinaryFilePreferences(string name, ISerializer serializer, IEncryptor encryptor) : base(name)
        {
            this.root = Application.persistentDataPath;
            this.serializer = serializer;
            this.encryptor = encryptor;
            this.Load();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual StringBuilder GetDirectory()
        {
            StringBuilder buf = new StringBuilder(this.root);
            buf.Append("/").Append(this.Name).Append("/");
            return buf;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual StringBuilder GetFullFileName()
        {
            return this.GetDirectory().Append("prefs.dat");
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Load()
        {
            try
            {
                string filename = this.GetFullFileName().ToString();
                if (!File.Exists(filename))
                    return;

                byte[] data = File.ReadAllBytes(filename);
                if (data == null || data.Length <= 0)
                    return;

                if (this.encryptor != null)
                    data = encryptor.Decode(data);

                this.dict.Clear();
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            string key = reader.ReadString();
                            string value = reader.ReadString();
                            this.dict.Add(key, value);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Load failed,{0}", e);
            }
        }

        public override object GetObject(string key, Type type, object defaultValue)
        {
            if (!this.dict.ContainsKey(key))
                return defaultValue;

            string str = this.dict[key];
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            return serializer.Deserialize(str, type);
        }

        public override void SetObject(string key, object value)
        {
            if (value == null)
            {
                this.dict.Remove(key);
                return;
            }

            this.dict[key] = serializer.Serialize(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public override T GetObject<T>(string key, T defaultValue)
        {
            if (!this.dict.ContainsKey(key))
                return defaultValue;

            string str = this.dict[key];
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            return (T)serializer.Deserialize(str, typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void SetObject<T>(string key, T value)
        {
            if (value == null)
            {
                this.dict.Remove(key);
                return;
            }

            this.dict[key] = serializer.Serialize(value);
        }

        public override object[] GetArray(string key, Type type, object[] defaultValue)
        {
            if (!this.dict.ContainsKey(key))
                return defaultValue;

            string str = this.dict[key];
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            string[] items = str.Split(ARRAY_SEPARATOR);
            List<object> list = new List<object>();
            for (int i = 0; i < items.Length; i++)
            {
                string item = items[i];
                if (string.IsNullOrEmpty(item))
                    list.Add(null);
                else
                {
                    list.Add(serializer.Deserialize(items[i], type));
                }
            }
            return list.ToArray();
        }

        public override void SetArray(string key, object[] values)
        {
            if (values == null || values.Length == 0)
            {
                this.dict.Remove(key);
                return;
            }

            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                buf.Append(serializer.Serialize(value));
                if (i < values.Length - 1)
                    buf.Append(ARRAY_SEPARATOR);
            }

            this.dict[key] = buf.ToString();
        }

        public override T[] GetArray<T>(string key, T[] defaultValue)
        {
            if (!this.dict.ContainsKey(key))
                return defaultValue;

            string str = this.dict[key];
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            string[] items = str.Split(ARRAY_SEPARATOR);
            List<T> list = new List<T>();
            for (int i = 0; i < items.Length; i++)
            {
                string item = items[i];
                if (string.IsNullOrEmpty(item))
                    list.Add(default(T));
                else
                {
                    list.Add((T)serializer.Deserialize(items[i], typeof(T)));
                }
            }
            return list.ToArray();
        }

        public override void SetArray<T>(string key, T[] values)
        {
            if (values == null || values.Length == 0)
            {
                this.dict.Remove(key);
                return;
            }

            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                buf.Append(serializer.Serialize(value));
                if (i < values.Length - 1)
                    buf.Append(ARRAY_SEPARATOR);
            }

            this.dict[key] = buf.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool ContainsKey(string key)
        {
            return this.dict.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public override void Remove(string key)
        {
            if (this.dict.ContainsKey(key))
                this.dict.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void RemoveAll()
        {
            this.dict.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Save()
        {
            if (this.dict.Count <= 0)
            {
                this.Delete();
                return;
            }

            Directory.CreateDirectory(this.GetDirectory().ToString());
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(this.dict.Count);
                    foreach (KeyValuePair<string, string> kv in this.dict)
                    {
                        writer.Write(kv.Key);
                        writer.Write(kv.Value);
                    }
                    writer.Flush();
                }
                byte[] data = stream.ToArray();
                if (this.encryptor != null)
                    data = encryptor.Encode(data);

                var filename = this.GetFullFileName().ToString();
                File.WriteAllBytes(filename, data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Delete()
        {
            this.dict.Clear();
            string filename = this.GetFullFileName().ToString();
            if (File.Exists(filename))
                File.Delete(filename);
        }
    }
}
