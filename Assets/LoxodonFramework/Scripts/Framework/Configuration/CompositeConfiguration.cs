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
    public class CompositeConfiguration : ConfigurationBase
    {
        private readonly List<IConfiguration> configurations = new List<IConfiguration>();

        private readonly IConfiguration memoryConfiguration;

        public CompositeConfiguration() : this(null)
        {
        }

        public CompositeConfiguration(List<IConfiguration> configurations)
        {
            this.memoryConfiguration = new MemoryConfiguration();
            this.configurations.Add(memoryConfiguration);

            if (configurations != null && configurations.Count > 0)
            {
                for (int i = 0; i < configurations.Count; i++)
                {
                    var config = configurations[i];
                    if (config == null)
                        continue;

                    AddConfiguration(config);
                }
            }
        }

        /// <summary>
        /// Get the first configuration with a given key.
        /// </summary>
        /// <param name="key">the key to be checked</param>
        /// <exception cref="ArgumentException">if the source configuration cannot be determined</exception>
        /// <returns>the source configuration of this key</returns>
        public IConfiguration GetFirstConfiguration(string key)
        {
            if (key == null)
                throw new ArgumentException("Key must not be null!");

            for (int i = 0; i < configurations.Count; i++)
            {
                IConfiguration config = configurations[i];
                if (config != null && config.ContainsKey(key))
                    return config;
            }
            return null;
        }

        /// <summary>
        /// Return the configuration at the specified index.
        /// </summary>
        /// <param name="index">The index of the configuration to retrieve</param>
        /// <returns>the configuration at this index</returns>
        public IConfiguration GetConfiguration(int index)
        {
            if (index < 0 || index >= configurations.Count)
                return null;

            return configurations[index];
        }

        /// <summary>
        /// Returns the memory configuration. In this configuration changes are stored.
        /// </summary>
        /// <returns>the in memory configuration</returns>
        public IConfiguration getMemoryConfiguration()
        {
            return memoryConfiguration;
        }

        /// <summary>
        /// Add a new configuration, the new configuration has a higher priority.
        /// </summary>
        /// <param name="configuration"></param>
        public void AddConfiguration(IConfiguration configuration)
        {
            if (!configurations.Contains(configuration))
            {
                configurations.Insert(1, configuration);
            }
        }

        public void RemoveConfiguration(IConfiguration configuration)
        {
            if (!configuration.Equals(memoryConfiguration))
            {
                configurations.Remove(configuration);
            }
        }

        public int getNumberOfConfigurations()
        {
            return configurations.Count;
        }

        public override bool IsEmpty
        {
            get
            {
                for (int i = 0; i < configurations.Count; i++)
                {
                    IConfiguration config = configurations[i];
                    if (config != null && !config.IsEmpty)
                        return false;
                }
                return true;
            }
        }

        public override bool ContainsKey(string key)
        {
            for (int i = 0; i < configurations.Count; i++)
            {
                IConfiguration config = configurations[i];
                if (config != null && config.ContainsKey(key))
                    return true;
            }
            return false;
        }

        public override IEnumerator<string> GetKeys()
        {
            List<string> keys = new List<string>();
            for (int i = 0; i < configurations.Count; i++)
            {
                IConfiguration config = configurations[i];
                if (config == null)
                    continue;

                IEnumerator<string> j = config.GetKeys();
                while (j.MoveNext())
                {
                    string key = j.Current;
                    if (!keys.Contains(key))
                        keys.Add(key);
                }
            }
            return keys.GetEnumerator();
        }

        public override object GetProperty(string key)
        {
            for (int i = 0; i < configurations.Count; i++)
            {
                IConfiguration config = configurations[i];
                if (config != null && config.ContainsKey(key))
                    return config.GetProperty(key);
            }
            return null;
        }

        public override void AddProperty(string key, object value)
        {
            memoryConfiguration.AddProperty(key, value);
        }

        public override void SetProperty(string key, object value)
        {
            memoryConfiguration.SetProperty(key, value);
        }

        public override void RemoveProperty(string key)
        {
            memoryConfiguration.RemoveProperty(key);
        }

        public override void Clear()
        {
            memoryConfiguration.Clear();
            for (int i = configurations.Count - 1; i > 0; i--)
            {
                configurations.RemoveAt(i);
            }
        }
    }
}
