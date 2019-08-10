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

namespace Loxodon.Framework.Prefs
{
    /// <summary>
    /// Abstract class for accessing and modifying preference data
    /// </summary>
    public abstract class Preferences
    {
        /// <summary>
        /// The name of global preferences.
        /// </summary>
        protected static readonly string GLOBAL_NAME = "_GLOBAL_";
        protected const char ARRAY_SEPARATOR = '|';
        private static Dictionary<string, Preferences> cache = new Dictionary<string, Preferences>();
        private static IFactory _defaultFactory;
        private static IFactory _factory;

        private string name;

        static Preferences()
        {
            _defaultFactory = new PlayerPrefsPreferencesFactory();
        }

        /// <summary>
        /// Retrieve a factory of Preferences.
        /// </summary>
        /// <returns></returns>
        protected static IFactory GetFactory()
        {
            if (_factory != null)
                return _factory;
            return _defaultFactory;
        }



        /// <summary>
        /// Retrieve a global preferences.
        /// </summary>
        /// <returns></returns>
        public static Preferences GetGlobalPreferences()
        {
            return GetPreferences(GLOBAL_NAME);
        }

        /// <summary>
        /// Retrieve a user's preferences.
        /// </summary>
        /// <param name="name">The name of the preferences to retrieve.eg:username or username@zone</param>
        /// <returns></returns>
        public static Preferences GetPreferences(string name)
        {
            Preferences prefs;
            if (cache.TryGetValue(name, out prefs))
                return prefs;

            prefs = GetFactory().Create(name);
            cache[name] = prefs;
            return prefs;
        }

        /// <summary>
        /// Register a factory of Preferences.
        /// </summary>
        /// <param name="factory"></param>
        public static void Register(IFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Save all these preferences.
        /// </summary>
        public static void SaveAll()
        {
            foreach (Preferences prefs in cache.Values)
            {
                prefs.Save();
            }
        }

        /// <summary>
        /// Delete all these preferences.
        /// </summary>
        public static void DeleteAll()
        {
            foreach (Preferences prefs in cache.Values)
            {
                prefs.Delete();
            }
            cache.Clear();
        }

        /// <summary>
        /// Preferences
        /// </summary>
        /// <param name="name"></param>
        public Preferences(string name)
        {
            this.name = name;
            if (string.IsNullOrEmpty(this.name))
                this.name = GLOBAL_NAME;
        }

        /// <summary>
        /// The name of the preferences
        /// </summary>
        public string Name
        {
            get { return this.name; }
            protected set { this.name = value; }
        }

        /// <summary>
        /// Load the preferences from the local file system.
        /// </summary>
        protected abstract void Load();

        /// <summary>
        /// Retrieve a string value from the preferences.
        /// The method returns null if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a string.</returns>
        public string GetString(string key)
        {
            return this.GetObject<string>(key, null);
        }

        /// <summary>
        /// Retrieve a string value from the preferences. 
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <param name="defaultValue">Value to return if this preference does not exist</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a string.</returns>
        public string GetString(string key, string defaultValue)
        {
            return this.GetObject<string>(key, defaultValue);
        }

        /// <summary>
        /// Set a string value in the preferences
        /// </summary>
        /// <param name="key">The name of the preference</param>
        /// <param name="value">The new value for the preference</param>
        public void SetString(string key, string value)
        {
            this.SetObject<string>(key, value);
        }

        /// <summary>
        /// Retrieve a float value from the preferences.
        /// The method returns 0f if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a float.</returns>
        public float GetFloat(string key)
        {
            return this.GetObject<float>(key, 0f);
        }

        /// <summary>
        /// Retrieve a float value from the preferences. 
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <param name="defaultValue">Value to return if this preference does not exist</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a float.</returns>
        public float GetFloat(string key, float defaultValue)
        {
            return this.GetObject<float>(key, defaultValue);
        }

        /// <summary>
        /// Set a float value in the preferences
        /// </summary>
        /// <param name="key">The name of the preference</param>
        /// <param name="value">The new value for the preference</param>
        public void SetFloat(string key, float value)
        {
            this.SetObject<float>(key, value);
        }

        /// <summary>
        /// Retrieve a double value from the preferences.
        /// The method returns 0d if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a double.</returns>
        public double GetDouble(string key)
        {
            return this.GetObject<double>(key, 0d);
        }

        /// <summary>
        /// Retrieve a double value from the preferences. 
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <param name="defaultValue">Value to return if this preference does not exist</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a double.</returns>
        public double GetDouble(string key, double defaultValue)
        {
            return this.GetObject<double>(key, defaultValue);
        }

        /// <summary>
        /// Set a double value in the preferences
        /// </summary>
        /// <param name="key">The name of the preference</param>
        /// <param name="value">The new value for the preference</param>
        public void SetDouble(string key, double value)
        {
            this.SetObject<double>(key, value);
        }

        /// <summary>
        /// Retrieve a bool value from the preferences.
        /// The method returns false if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a bool.</returns>
        public bool GetBool(string key)
        {
            return this.GetObject<bool>(key, false);
        }

        /// <summary>
        /// Retrieve a bool value from the preferences. 
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <param name="defaultValue">Value to return if this preference does not exist</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a bool.</returns>
        public bool GetBool(string key, bool defaultValue)
        {
            return this.GetObject<bool>(key, defaultValue);
        }

        /// <summary>
        /// Set a bool value in the preferences
        /// </summary>
        /// <param name="key">The name of the preference</param>
        /// <param name="value">The new value for the preference</param>
        public void SetBool(string key, bool value)
        {
            this.SetObject<bool>(key, value);
        }

        /// <summary>
        /// Retrieve a int value from the preferences.
        /// The method returns 0 if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a int.</returns>
        public int GetInt(string key)
        {
            return this.GetObject<int>(key, 0);
        }

        /// <summary>
        /// Retrieve a int value from the preferences. 
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <param name="defaultValue">Value to return if this preference does not exist</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a int.</returns>
        public int GetInt(string key, int defaultValue)
        {
            return this.GetObject<int>(key, defaultValue);
        }

        /// <summary>
        /// Set a int value in the preferences
        /// </summary>
        /// <param name="key">The name of the preference</param>
        /// <param name="value">The new value for the preference</param>
        public void SetInt(string key, int value)
        {
            this.SetObject<int>(key, value);
        }

        /// <summary>
        /// Retrieve a long value from the preferences.
        /// The method returns 0L if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a long.</returns>
        public long GetLong(string key)
        {
            return this.GetObject<long>(key, 0L);
        }

        /// <summary>
        /// Retrieve a long value from the preferences. 
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <param name="defaultValue">Value to return if this preference does not exist</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a long.</returns>
        public long GetLong(string key, long defaultValue)
        {
            return this.GetObject<long>(key, defaultValue);
        }

        /// <summary>
        /// Set a long value in the preferences
        /// </summary>
        /// <param name="key">The name of the preference</param>
        /// <param name="value">The new value for the preference</param>
        public void SetLong(string key, long value)
        {
            this.SetObject<long>(key, value);
        }

        /// <summary>
        /// Retrieve a value from the preferences.
        /// The method returns null if the key is not found.
        /// Supported Types:ValueType,string, Vector2 ,Vector3 ,Vector4,Color,Color32 and Serializable types
        /// </summary>       
        /// <param name="key">The name of the preference to retrieve</param>
        /// <param name="type">Supported: ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32,Version and Serializable types</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not the type.</returns>
        public object GetObject(string key, Type type)
        {
            return this.GetObject(key, type, null);
        }

        /// <summary>
        /// Retrieve a value from the preferences. 
        /// The method returns defaultValue if the key is not found.
        /// Supported Types:ValueType,string, Vector2 ,Vector3 ,Vector4,Color,Color32,Version and Serializable types
        /// </summary>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <param name="type">Supported: ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32,Version and Serializable types</param>
        /// <param name="defaultValue">Value to return if this preference does not exist</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.Throws
        /// Exception if there is a preference with this name that is not the type.</returns>
        public abstract object GetObject(string key, Type type, object defaultValue);

        /// <summary>
        /// Set a value in the preferences
        /// Supported Types:ValueType,string, Vector2 ,Vector3 ,Vector4,Color,Color32 and  Serializable types
        /// </summary>
        /// <param name="key">The name of the preference</param>
        /// <param name="value">The new value for the preference</param>
        public abstract void SetObject(string key, object value);

        /// <summary>
        /// Retrieve a T value from the preferences.
        /// The method returns default(T) if the key is not found.
        /// Supported Types:ValueType,string, Vector2 ,Vector3 ,Vector4,Color,Color32 and  Serializable types
        /// </summary>
        /// <typeparam name="T">Supported: ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32 and  Serializable types</typeparam>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a T.</returns>
        public T GetObject<T>(string key)
        {
            return this.GetObject<T>(key, default(T));
        }

        /// <summary>
        /// Retrieve a T value from the preferences. 
        /// The method returns defaultValue if the key is not found.
        /// Supported Types:ValueType,string, Vector2 ,Vector3 ,Vector4,Color,Color32,Version and Serializable types
        /// </summary>
        /// <typeparam name="T">Supported: ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32,Version and Serializable types</typeparam>
        /// <param name="key">The name of the preference to retrieve</param>
        /// <param name="defaultValue">Value to return if this preference does not exist</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.  Throws
        /// Exception if there is a preference with this name that is not a T.</returns>
        public abstract T GetObject<T>(string key, T defaultValue);

        /// <summary>
        /// Set a T value in the preferences
        /// Supported Types:ValueType,string, Vector2 ,Vector3 ,Vector4,Color,Color32 and  Serializable types
        /// </summary>
        /// <typeparam name="T">Supported: ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32 and Serializable types</typeparam>
        /// <param name="key">The name of the preference</param>
        /// <param name="value">The new value for the preference</param>
        public abstract void SetObject<T>(string key, T value);

        /// <summary>
        /// Retrieve an array from the preferences. 
        /// The method returns null if the key is not found.
        /// Supported Types:ValueType,string[], Vector2[],Vector3[],Vector4[],Color[],Color32[],Version[] and  Serializable types
        /// </summary>
        /// <param name="key">The name of the preference</param>
        /// <param name="type">Supported: ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32 and  Serializable types</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.Throws
        /// Exception if there is a preference with this name that is not an array.</returns>
        public object[] GetArray(string key, Type type)
        {
            return GetArray(key, type, null);
        }

        /// <summary>
        /// Retrieve an array from the preferences. 
        /// The method returns defaultValue if the key is not found.
        /// Supported Types:ValueType,string[], Vector2[],Vector3[],Vector4[],Color[],Color32[],Version[] and  Serializable types
        /// </summary>
        /// <typeparam name="T">Supported: ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32 and  Serializable types</typeparam>
        /// <param name="key">The name of the preference</param>
        /// <param name="defaultValue">Value to return if this preference does not exist</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.Throws
        /// Exception if there is a preference with this name that is not an array.</returns>
        public abstract object[] GetArray(string key, Type type, object[] defaultValue);

        /// <summary>
        /// Set an array in the preferences
        /// Supported Types:ValueType,string[], Vector2[],Vector3[],Vector4[],Color[],Color32[],Version[] and  Serializable types
        /// </summary>
        /// <typeparam name="T">Supported: ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32 and  Serializable types</typeparam>
        /// <param name="key">The name of the preference</param>
        /// <param name="values">The new value for the preference</param>
        public abstract void SetArray(string key, object[] values);

        /// <summary>
        /// Retrieve the array value of T from the preferences. 
        /// The method returns null if the key is not found.
        /// Supported Types:ValueType,string[], Vector2[],Vector3[],Vector4[],Color[],Color32[],Version[] and  Serializable types
        /// </summary>
        /// <typeparam name="T">Supported: ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32 and  Serializable types</typeparam>
        /// <param name="key">The name of the preference</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.Throws
        /// Exception if there is a preference with this name that is not an array of T.</returns>
        public T[] GetArray<T>(string key)
        {
            return GetArray<T>(key, null);
        }

        /// <summary>
        /// Retrieve the array value of T from the preferences. 
        /// The method returns defaultValue if the key is not found.
        /// Supported Types:ValueType,string[], Vector2[],Vector3[],Vector4[],Color[],Color32[],Version[] and  Serializable types
        /// </summary>
        /// <typeparam name="T">Supported: ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32 and  Serializable types</typeparam>
        /// <param name="key">The name of the preference</param>
        /// <param name="defaultValue">Value to return if this preference does not exist</param>
        /// <exception cref="Exception"></exception>
        /// <returns>Returns the preference value if it exists, or defaultValue.Throws
        /// Exception if there is a preference with this name that is not an array of T.</returns>
        public abstract T[] GetArray<T>(string key, T[] defaultValue);

        /// <summary>
        /// Set an array value of T in the preferences
        /// Supported Types:ValueType,string[], Vector2[],Vector3[],Vector4[],Color[],Color32[],Version[] and  Serializable types
        /// </summary>
        /// <typeparam name="T">Supported: ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32 and  Serializable types</typeparam>
        /// <param name="key">The name of the preference</param>
        /// <param name="values">The new value for the preference</param>
        public abstract void SetArray<T>(string key, T[] values);

        /// <summary>
        /// Checks whether the preferences contains a preference.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool ContainsKey(string key);

        /// <summary>
        /// Remove a value from the preferences.
        /// </summary>
        /// <param name="key">The name of the preference to remove</param>
        public abstract void Remove(string key);

        /// <summary>
        /// Remove all values.
        /// </summary>
        public abstract void RemoveAll();

        /// <summary>
        /// Save the preferences to the local disk
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// Delete the preferences file.
        /// </summary>
        public abstract void Delete();
    }
}
