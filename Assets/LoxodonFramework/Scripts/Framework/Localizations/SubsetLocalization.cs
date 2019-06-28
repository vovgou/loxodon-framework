using System;
using UnityEngine;

namespace Loxodon.Framework.Localizations
{
    class SubsetLocalization : ILocalization
    {
        private readonly string prefix;
        private readonly Localization parent;
        public SubsetLocalization(Localization parent, string prefix) : base()
        {
            this.parent = parent;
            this.prefix = prefix;
        }

        protected string GetParentKey(string key)
        {
            if ("".Equals(key) || key == null)
                throw new ArgumentNullException(key);

            return string.Format("{0}.{1}", prefix, key);
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
            return parent.Subset(GetParentKey(prefix));
        }

        /// <summary>
        /// Whether the localization file contains this key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool ContainsKey(string key)
        {
            return parent.ContainsKey(GetParentKey(key));
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
        /// Gets a message based on a key using the supplied args, as defined in "string.Format", or the provided key if no message is found.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual string GetText(string key, params object[] args)
        {
            return this.GetText(key, key, args);
        }

        /// <summary>
        /// Gets a message based on a key, or, if the message is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual string GetText(string key, string defaultValue)
        {
            return this.Get<string>(key, defaultValue);
        }

        /// <summary>
        /// Gets a message based on a key using the supplied args, as defined in "string.Format", or, if the message is not found, a supplied  default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual string GetText(string key, string defaultValue, params object[] args)
        {
            string format = this.Get<string>(key, defaultValue);
            return string.Format(format, args);
        }

        /// <summary>
        /// Gets a boolean value based on a key, or, if the value is not found, the value 'false' is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool GetBoolean(string key)
        {
            return this.Get<bool>(key);
        }

        /// <summary>
        /// Gets a boolean value based on a key, or, if the value is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual bool GetBoolean(string key, bool defaultValue)
        {
            return this.Get<bool>(key, defaultValue);
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
            return this.Get<int>(key, defaultValue);
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
            return this.Get<long>(key, defaultValue);
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
            return this.Get<double>(key, defaultValue);
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
            return this.Get<float>(key, defaultValue);
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
            return this.Get<Color>(key, defaultValue);
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
            return this.Get<Vector3>(key, defaultValue);
        }

        /// <summary>
        /// Gets a datetime value based on a key, or, if the value is not found, the value 'DateTime(0)' is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual DateTime GetDateTime(string key)
        {
            return this.Get<DateTime>(key, new DateTime(0));
        }

        /// <summary>
        /// Gets a datetime value based on a key, or, if the value is not found, a supplied default value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual DateTime GetDateTime(string key, DateTime defaultValue)
        {
            return this.Get<DateTime>(key, defaultValue);
        }

        /// <summary>
        /// Gets a value based on a key, or, if the value is not found, the value 'default(T)' is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T Get<T>(string key)
        {
            return this.Get<T>(key, default(T));
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
            return parent.Get(GetParentKey(key), defaultValue);
        }
    }
}
