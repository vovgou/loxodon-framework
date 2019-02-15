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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public override T GetObject<T>(string key, T defaultValue)
        {
            try
            {
                if (!this.dict.ContainsKey(key))
                    return defaultValue;

                string str = this.dict[key];
                if (string.IsNullOrEmpty(str))
                    return defaultValue;

                return (T)serializer.Deserialize(str, typeof(T));
            }
            catch (Exception e)
            {
                throw new NotSupportedException("unsupported type", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void SetObject<T>(string key, T value)
        {
            try
            {
                if (value == null)
                {
                    this.dict.Remove(key);
                    return;
                }

                this.dict[key] = serializer.Serialize(value);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("unsupported type", e);
            }
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
