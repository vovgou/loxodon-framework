using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Localizations
{
    public class Localization : ILocalization
    {
        private const string DEFAULT_ROOT = "Localization";
        private static readonly object _lock = new object();
        private static Localization instance;

        private CultureInfo cultureInfo;
        private List<IDataProvider> providers;
        private Dictionary<string, IObservableProperty> data = new Dictionary<string, IObservableProperty>();

        public static Localization Current
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                            instance = Create(new DefaultDataProvider(DEFAULT_ROOT, new XmlDocumentParser()));
                    }
                }
                return instance;
            }
            set { lock (_lock) { instance = value; } }
        }

        public static Localization Create(IDataProvider provider)
        {
            return Create(provider, null);
        }

        public static Localization Create(IDataProvider provider, CultureInfo cultureInfo)
        {
            return new Localization(provider, cultureInfo);
        }

        protected Localization(IDataProvider provider, CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
            if (this.cultureInfo == null)
                this.cultureInfo = Locale.GetCultureInfo();

            this.providers = new List<IDataProvider>();
            if (provider != null)
                this.providers.Add(provider);

            this.Load();
        }

        public virtual CultureInfo CultureInfo
        {
            get { return this.cultureInfo; }
            set
            {
                if (this.cultureInfo != null && this.cultureInfo.Equals(value))
                    return;

                this.cultureInfo = value;
                this.OnCultureInfoChanged();
            }
        }

        protected virtual void OnCultureInfoChanged()
        {
            this.Load();
        }

        public virtual void AddDataProvider(IDataProvider provider)
        {
            if (provider == null)
                return;

            if (this.providers.Contains(provider))
                return;

            this.providers.Add(provider);

            provider.Load(this.CultureInfo, OnLoadCompleted);
        }

        public virtual void Refresh()
        {
            this.Load();
        }

        protected virtual void Load()
        {
            if (this.providers == null || this.providers.Count <= 0)
                return;

            foreach (IDataProvider provider in this.providers)
            {
                try
                {
                    provider.Load(this.CultureInfo, OnLoadCompleted);
                }
                catch (Exception) { }
            }
        }

        protected virtual void OnLoadCompleted(Dictionary<string, object> dict)
        {
            if (dict == null || dict.Count <= 0)
                return;

            foreach (KeyValuePair<string, object> kv in dict)
            {
                IObservableProperty property;
                if (this.data.TryGetValue(kv.Key, out property))
                {
                    property.Value = kv.Value;
                }
                else
                {
                    property = new ObservableProperty(kv.Value);
                    this.data[kv.Key] = property;
                }
            }
        }

        /// <summary>
        /// Return a decorator localization containing every key from the current
        /// localization that starts with the specified prefix.The prefix is
        /// removed from the keys in the subset.
        /// </summary>
        /// <param name="prefix">The prefix used to select the localization.</param>
        /// <returns>a subset localization</returns>
        public virtual ILocalization Subset(string prefix)
        {
            return new SubsetLocalization(this, prefix);
        }

        /// <summary>
        /// Whether the localization file contains this key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool ContainsKey(string key)
        {
            return this.data.ContainsKey(key);
        }

        /// <summary>
        /// Gets a message based on a message key or if no message is found the provided key is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string GetText(string key)
        {
            return this.GetText(key, key);
        }

        /// <summary>
        /// Gets a message based on a key, or, if the message is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual string GetText(string key, string defaultValue)
        {
            return this.Get(key, defaultValue);
        }

        /// <summary>
        /// Gets a message based on a key using the supplied args, as defined in "string.Format", or the provided key if no message is found.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual string GetFormattedText(string key, params object[] args)
        {
            string format = this.Get<string>(key, null);
            if (format == null)
                return key;

            return string.Format(format, args);
        }

        /// <summary>
        /// Gets a boolean value based on a key, or, if the value is not found, the value 'false' is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool GetBoolean(string key)
        {
            return this.Get(key, false);
        }

        /// <summary>
        /// Gets a boolean value based on a key, or, if the value is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual bool GetBoolean(string key, bool defaultValue)
        {
            return this.Get(key, defaultValue);
        }

        /// <summary>
        /// Gets a int value based on a key, or, if the value is not found, the value '0' is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual int GetInt(string key)
        {
            return this.Get<int>(key);
        }

        /// <summary>
        /// Gets a int value based on a key, or, if the value is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual int GetInt(string key, int defaultValue)
        {
            return this.Get(key, defaultValue);
        }

        /// <summary>
        /// Gets a long value based on a key, or, if the value is not found, the value '0' is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual long GetLong(string key)
        {
            return this.Get<long>(key);
        }

        /// <summary>
        /// Gets a long value based on a key, or, if the value is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual long GetLong(string key, long defaultValue)
        {
            return this.Get(key, defaultValue);
        }

        /// <summary>
        /// Gets a double value based on a key, or, if the value is not found, the value '0' is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual double GetDouble(string key)
        {
            return this.Get<double>(key);
        }

        /// <summary>
        /// Gets a double value based on a key, or, if the value is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual double GetDouble(string key, double defaultValue)
        {
            return this.Get(key, defaultValue);
        }

        /// <summary>
        /// Gets a float value based on a key, or, if the value is not found, the value '0' is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual float GetFloat(string key)
        {
            return this.Get<float>(key);
        }

        /// <summary>
        /// Gets a float value based on a key, or, if the value is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual float GetFloat(string key, float defaultValue)
        {
            return this.Get(key, defaultValue);
        }

        /// <summary>
        /// Gets a color value based on a key, or, if the value is not found, the value '#000000' is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual Color GetColor(string key)
        {
            return this.Get<Color>(key);
        }

        /// <summary>
        /// Gets a color value based on a key, or, if the value is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual Color GetColor(string key, Color defaultValue)
        {
            return this.Get(key, defaultValue);
        }

        /// <summary>
        /// Gets a vector3 value based on a key, or, if the value is not found, the value 'Vector3.zero' is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual Vector3 GetVector3(string key)
        {
            return this.Get<Vector3>(key);
        }

        /// <summary>
        /// Gets a vector3 value based on a key, or, if the value is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual Vector3 GetVector3(string key, Vector3 defaultValue)
        {
            return this.Get(key, defaultValue);
        }

        /// <summary>
        /// Gets a datetime value based on a key, or, if the value is not found, the value 'DateTime(0)' is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual DateTime GetDateTime(string key)
        {
            return this.Get(key, new DateTime(0));
        }

        /// <summary>
        /// Gets a datetime value based on a key, or, if the value is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual DateTime GetDateTime(string key, DateTime defaultValue)
        {
            return this.Get(key, defaultValue);
        }

        /// <summary>
        /// Gets a value based on a key, or, if the value is not found, the value 'default(T)' is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T Get<T>(string key)
        {
            return this.Get(key, default(T));
        }

        /// <summary>
        /// Gets a value based on a key, or, if the value is not found, a supplied default value is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual T Get<T>(string key, T defaultValue)
        {
            IObservableProperty value;
            if (this.data.TryGetValue(key, out value))
            {
                if (value is T)
                    return (T)value;

                if (value.Value is T)
                    return (T)(value.Value);

                return (T)Convert.ChangeType(value.Value, typeof(T));
            }
            return defaultValue;
        }
    }
}
