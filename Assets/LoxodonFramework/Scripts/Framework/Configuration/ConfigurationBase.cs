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

using Loxodon.Framework.Utilities;
using Loxodon.Log;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loxodon.Framework.Configurations
{
    public abstract class ConfigurationBase : IConfiguration
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConfigurationBase));

        private static readonly DefaultTypeConverter defaultTypeConverter = new DefaultTypeConverter();

        protected static readonly string KEY_DELIMITER = ".";
        protected static readonly Version DEFAULT_VERSION = new Version("1.0.0");
        protected static readonly DateTime DEFAULT_DATETIME = new DateTime();

        private List<ITypeConverter> converters = new List<ITypeConverter>();

        public ConfigurationBase() : this(null)
        {
        }

        public ConfigurationBase(ITypeConverter[] converters)
        {
            this.converters.Add(defaultTypeConverter);
            if (converters != null && converters.Length > 0)
            {
                foreach (var converter in converters)
                {
                    this.converters.Insert(0, converter);
                }
            }
        }

        protected virtual T GetProperty<T>(string key, T defaultValue)
        {
            object value = GetProperty(key);
            if (value == null)
                return defaultValue;

            return (T)ConvertTo(typeof(T), value);
        }

        protected virtual object GetProperty(string key, Type type, object defaultValue)
        {
            object value = GetProperty(key);
            if (value == null)
                return defaultValue;

            return ConvertTo(type, value);
        }

        protected virtual object ConvertTo(Type type, object value)
        {
            try
            {
                for (int i = 0; i < converters.Count; i++)
                {
                    var converter = converters[i];
                    if (!converter.Support(type))
                        continue;

                    return converter.Convert(type, value);
                }
            }
            catch (FormatException e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("This value \"{0}\" cannot be converted to type \"{1}\"", value, type.Name);

                throw e;
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("This value \"{0}\" cannot be converted to type \"{1}\"", value, type.Name);

                throw new FormatException(string.Format("This value \"{0}\" cannot be converted to the type \"{1}\"", value, type.Name), e);
            }

            throw new NotSupportedException(string.Format("This value \"{0}\" cannot be converted to the type \"{1}\"", value, type.Name));
        }

        public virtual void AddTypeConverter(ITypeConverter converter)
        {
            this.converters.Insert(0, converter);
        }

        public virtual IConfiguration Subset(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException("the prefix is null or empty", "prefix");

            return new SubsetConfiguration(this, prefix);
        }

        public virtual bool IsEmpty { get { return !this.GetKeys().MoveNext(); } }

        public virtual IEnumerator<string> GetKeys(string prefix)
        {
            return new FilterEnumerator<string>(GetKeys(), (it) =>
            {
                return it.StartsWith(prefix + KEY_DELIMITER);
            });
        }

        public bool GetBoolean(string key)
        {
            return GetBoolean(key, false);
        }

        public bool GetBoolean(string key, bool defaultValue)
        {
            return GetProperty(key, defaultValue);
        }

        public float GetFloat(string key)
        {
            return this.GetFloat(key, 0);
        }

        public float GetFloat(string key, float defaultValue)
        {
            return this.GetProperty(key, defaultValue);
        }

        public double GetDouble(string key)
        {
            return this.GetDouble(key, 0);
        }

        public double GetDouble(string key, double defaultValue)
        {
            return this.GetProperty(key, defaultValue);
        }

        public short GetShort(string key)
        {
            return this.GetShort(key, 0);
        }

        public short GetShort(string key, short defaultValue)
        {
            return this.GetProperty(key, defaultValue);
        }

        public int GetInt(string key)
        {
            return this.GetInt(key, 0);
        }

        public int GetInt(string key, int defaultValue)
        {
            return this.GetProperty(key, defaultValue);
        }

        public long GetLong(string key)
        {
            return this.GetLong(key, 0);
        }

        public long GetLong(string key, long defaultValue)
        {
            return this.GetProperty(key, defaultValue);
        }

        public string GetString(string key)
        {
            return this.GetString(key, null);
        }

        public string GetString(string key, string defaultValue)
        {
            return GetProperty<string>(key, defaultValue);
        }

        public string GetFormattedString(string key, params object[] args)
        {
            string format = GetString(key, null);
            if (format == null)
                return null;

            return string.Format(format, args);
        }

        public DateTime GetDateTime(string key)
        {
            return this.GetDateTime(key, DEFAULT_DATETIME);
        }

        public DateTime GetDateTime(string key, DateTime defaultValue)
        {
            return this.GetProperty(key, defaultValue);
        }

        public Version GetVersion(string key)
        {
            return this.GetVersion(key, DEFAULT_VERSION);
        }

        public virtual Version GetVersion(string key, Version defaultValue)
        {
            return this.GetProperty(key, defaultValue);
        }

        public T GetObject<T>(string key)
        {
            return this.GetObject(key, default(T));
        }

        public virtual T GetObject<T>(string key, T defaultValue)
        {
            return this.GetProperty(key, defaultValue);
        }

        public object[] GetArray(string key, Type type)
        {
            return GetArray(key, type, new object[0]);
        }

        public object[] GetArray(string key, Type type, object[] defaultValue)
        {
            object value = GetProperty(key);
            if (value == null)
                return defaultValue;

            if (value is string)
            {
                string str = (string)value;
                if (string.IsNullOrEmpty(str))
                    return defaultValue;

                List<object> list = new List<object>();
                string[] items = StringSpliter.Split(str, ',');
                foreach (string item in items)
                {
                    object ret = null;
                    try
                    {
                        ret = ConvertTo(type, item);
                    }
                    catch (NotSupportedException e)
                    {
                        throw e;
                    }
                    catch (Exception) { }
                    list.Add(ret);
                }
                return list.ToArray();
            }

            Array array = value as Array;
            if (array != null)
            {
                List<object> list = new List<object>();
                for (int i = 0; i < array.Length; i++)
                {
                    var item = array.GetValue(i);
                    object ret = null;
                    try
                    {
                        ret = ConvertTo(type, item);
                    }
                    catch (NotSupportedException e)
                    {
                        throw e;
                    }
                    catch (Exception) { }
                    list.Add(ret);
                }
                return list.ToArray();
            }

            throw new FormatException(string.Format("This value \"{0}\" cannot be converted to an \"{1}\" array.", value, type.Name));
        }

        public T[] GetArray<T>(string key)
        {
            return GetArray(key, new T[0]);
        }

        public virtual T[] GetArray<T>(string key, T[] defaultValue)
        {
            object value = GetProperty(key);
            if (value == null)
                return defaultValue;

            if (value is string)
            {
                string str = (string)value;
                if (string.IsNullOrEmpty(str))
                    return defaultValue;

                List<T> list = new List<T>();
                string[] items = StringSpliter.Split(str, ',');
                foreach (string item in items)
                {
                    T ret = default(T);
                    try
                    {
                        ret = (T)ConvertTo(typeof(T), item);
                    }
                    catch (NotSupportedException e)
                    {
                        throw e;
                    }
                    catch (Exception) { }
                    list.Add(ret);
                }
                return list.ToArray();
            }

            if (value is T[])
                return (T[])value;

            Array array = value as Array;
            if (array != null)
            {
                List<T> list = new List<T>();
                for (int i = 0; i < array.Length; i++)
                {
                    var item = array.GetValue(i);
                    T ret = default(T);
                    try
                    {
                        ret = (T)ConvertTo(typeof(T), item);
                    }
                    catch (NotSupportedException e)
                    {
                        throw e;
                    }
                    catch (Exception) { }
                    list.Add(ret);
                }
                return list.ToArray();
            }

            throw new FormatException(string.Format("This value \"{0}\" cannot be converted to an \"{1}\" array.", value, typeof(T).Name));
        }

        public override string ToString()
        {
            IEnumerator<string> it = this.GetKeys();
            StringBuilder buf = new StringBuilder();
            buf.Append(this.GetType().Name).Append("{ \r\n");
            while (it.MoveNext())
            {
                string key = it.Current;
                buf.AppendFormat("  {0} = {1}\r\n", key, GetProperty(key));
            }
            buf.Append("}");
            return buf.ToString();
        }

        public abstract IEnumerator<string> GetKeys();

        public abstract bool ContainsKey(string key);

        public abstract object GetProperty(string key);

        public abstract void AddProperty(string key, object value);

        public abstract void RemoveProperty(string key);

        public abstract void SetProperty(string key, object value);

        public abstract void Clear();
    }
}
