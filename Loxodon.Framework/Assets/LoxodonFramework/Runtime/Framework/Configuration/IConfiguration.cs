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

namespace Loxodon.Framework.Configurations
{
    public interface IConfiguration
    {
        /// <summary>
        /// Return a decorator configuration containing every key from the current
        /// configuration that starts with the specified prefix.The prefix is
        /// removed from the keys in the subset.Since the subset is a decorator 
        /// and not a modified copy of the initial configuration, any change made 
        /// to the subset is available to the configuration, and reciprocally.
        /// </summary>
        /// <example> 
        /// For example, if the configuration contains the following properties:
        /// <code>
        /// prefix.number = 1
        /// prefix.string = Apache
        /// prefixed.foo = bar
        /// prefix = Jakarta
        /// </code>
        /// the configuration returned by <c>Subset("prefix")</c> will contain the properties:
        /// <code>
        /// number = 1
	    /// string = Apache
        /// </code>
        /// </example>
        /// <param name="prefix">The prefix used to select the properties.</param>
        /// <returns>a subset configuration</returns>
        IConfiguration Subset(string prefix);

        /// <summary>
        /// Whether the configuration is empty
        /// </summary>
        /// <returns></returns>
        bool IsEmpty { get; }

        /// <summary>
        /// Whether the configuration contains this key
        /// </summary>
        /// <param name="key">the key whose presence in this configuration is to be tested</param>
        /// <returns><c>true</c> if the configuration contains a value for this key, <c>false</c> otherwise</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets a collection of all child keys for a given prefix.
        /// </summary>
        /// <param name="prefix">The prefix used to select the properties.</param>
        /// <returns></returns>
        [Obsolete("This method will be removed in version 3.0")]
        IEnumerator<string> GetKeys(string prefix);

        /// <summary>
        /// Get a collection of all keys
        /// </summary>
        /// <returns></returns>
        IEnumerator<string> GetKeys();

        /// <summary>
        /// Retrieve a bool value from the configuration.
        /// The method returns false if the key is not found.
        /// </summary>
        /// <param name="key">The key of the property to retrieve</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue.Throws
        /// Exception if there is a property with this key that is not a bool.</returns>
        bool GetBoolean(string key);

        /// <summary>
        /// Retrieve a bool value from the configuration. 
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The key of the property to retrieve</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue.Throws
        /// Exception if there is a property with this key that is not a bool.</returns>
        bool GetBoolean(string key, bool defaultValue);

        /// <summary>
        /// Retrieve a float value from the configuration.
        /// The method returns 0f if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a float.</returns>
        float GetFloat(string key);

        /// <summary>
        /// Retrieve a float value from the configuration.
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a float.</returns>
        float GetFloat(string key, float defaultValue);

        /// <summary>
        /// Retrieve a double value from the configuration.
        /// The method returns 0d if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a double.</returns>
        double GetDouble(string key);

        /// <summary>
        /// Retrieve a double value from the configuration.
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a double.</returns>
        double GetDouble(string key, double defaultValue);

        /// <summary>
        /// Retrieve a short value from the configuration.
        /// The method returns 0 if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a short.</returns>
        short GetShort(string key);

        /// <summary>
        /// Retrieve a short value from the configuration.
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a short.</returns>
        short GetShort(string key, short defaultValue);

        /// <summary>
        /// Retrieve a int value from the configuration.
        /// The method returns 0 if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a int.</returns>
        int GetInt(string key);

        /// <summary>
        /// Retrieve a int value from the configuration.
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a int.</returns>
        int GetInt(string key, int defaultValue);

        /// <summary>
        /// Retrieve a long value from the configuration.
        /// The method returns 0 if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a long.</returns>
        long GetLong(string key);

        /// <summary>
        /// Retrieve a long value from the configuration.
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a long.</returns>
        long GetLong(string key, long defaultValue);

        /// <summary>
        /// Retrieve a string value from the configuration.
        /// The method returns null if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a string.</returns>
        string GetString(string key);

        /// <summary>
        /// Retrieve a string value from the configuration.
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a string.</returns>
        string GetString(string key, string defaultValue);

        /// <summary>
        /// Retrieve a string value from the configuration.
        /// The method returns null if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="args">Parameters used to format a string</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a string.</returns>
        string GetFormattedString(string key, params object[] args);

        /// <summary>
        /// Retrieve a <c>DateTime</c> value from the configuration.
        /// The method returns DateTime(0) if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a <c>DateTime</c>.</returns>
        DateTime GetDateTime(string key);

        /// <summary>
        /// Retrieve a <c>DateTime</c> value from the configuration.
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a <c>DateTime</c>.</returns>
        DateTime GetDateTime(string key, DateTime defaultValue);

        /// <summary>
        /// Retrieve a <c>Version</c> value from the configuration.
        /// The method returns DateTime(0) if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a <c>Version</c>.</returns>
        Version GetVersion(string key);

        /// <summary>
        /// Retrieve a <c>Version</c> value from the configuration.
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a <c>Version</c>.</returns>
        Version GetVersion(string key, Version defaultValue);

        /// <summary>
        /// Retrieve a <c>T</c> value from the configuration.
        /// The method returns default(T) if the key is not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The name of the property to retrieve</param>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a <c>T</c>.</returns>
        T GetObject<T>(string key);

        /// <summary>
        /// Retrieve a <c>T</c> value from the configuration.
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a <c>T</c>.</returns>
        T GetObject<T>(string key, T defaultValue);

        /// <summary>
        /// Retrieve a list value from the configuration.
        /// The method returns null if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="type">The type of array element</param>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a list.</returns>
        object[] GetArray(string key, Type type);

        /// <summary>
        /// Retrieve a list value from the configuration.
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="type">The type of array element</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a list.</returns>
        object[] GetArray(string key, Type type, object[] defaultValue);

        /// <summary>
        /// Retrieve a list value from the configuration.
        /// The method returns null if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a list.</returns>
        T[] GetArray<T>(string key);

        /// <summary>
        /// Retrieve a list value from the configuration.
        /// The method returns defaultValue if the key is not found.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <param name="defaultValue">Value returned if this property does not exist</param>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns>Returns the value if it exists, or defaultValue. Throws
        /// Exception if there is a property with this key that is not a list.</returns>
        T[] GetArray<T>(string key, T[] defaultValue);

        /// <summary>
        /// Retrieve a value from the configuration.
        /// </summary>
        /// <param name="key">The name of the property to retrieve</param>
        /// <returns></returns>
        object GetProperty(string key);

        /// <summary>
        /// Add a property
        /// </summary>
        /// <param name="key">The name of the property to add</param>
        /// <param name="value">The value of the property to add</param>
        void AddProperty(string key, object value);

        /// <summary>
        /// Update the property of the given key
        /// </summary>
        /// <param name="key">The name of the property to update</param>
        /// <param name="value">The value of the property to update</param>
        void SetProperty(string key, object value);

        /// <summary>
        /// Delete the property of the given key
        /// </summary>
        /// <param name="key">The name of the property to delete</param>
        void RemoveProperty(string key);

        /// <summary>
        /// Clear configuration
        /// </summary>
        void Clear();
    }
}
