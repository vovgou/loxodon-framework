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

using System.Collections.Generic;

namespace Loxodon.Framework.Configurations
{
    public class MemoryConfiguration : ConfigurationBase
    {
        private readonly Dictionary<string, object> dict = new Dictionary<string, object>();

        public MemoryConfiguration()
        {
        }

        public MemoryConfiguration(Dictionary<string, object> dict)
        {
            if (dict != null && dict.Count > 0)
            {
                foreach (var kv in dict)
                {
                    dict.Add(kv.Key, kv.Value);
                }
            }
        }

        public override bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public override IEnumerator<string> GetKeys()
        {
            return dict.Keys.GetEnumerator();
        }

        public override object GetProperty(string key)
        {
            object value = null;
            dict.TryGetValue(key, out value);
            return value;
        }

        public override void AddProperty(string key, object value)
        {
            if (dict.ContainsKey(key))
                throw new AlreadyExistsException(key);

            dict.Add(key, value);
        }

        public override void SetProperty(string key, object value)
        {
            dict[key] = value;
        }

        public override void RemoveProperty(string key)
        {
            dict.Remove(key);
        }

        public override void Clear()
        {
            dict.Clear();
        }
    }
}