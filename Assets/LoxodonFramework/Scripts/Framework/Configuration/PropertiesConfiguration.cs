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
using System.IO;
using System.Text.RegularExpressions;

namespace Loxodon.Framework.Configurations
{
    public class PropertiesConfiguration : ConfigurationBase
    {
        private readonly Dictionary<string, object> dict = new Dictionary<string, object>();
        public PropertiesConfiguration(string text)
        {
            this.Load(text);
        }

        protected void Load(string text)
        {
            StringReader reader = new StringReader(text);
            string line = null;
            while (null != (line = reader.ReadLine()))
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                if (Regex.IsMatch(line, @"^((#)|(//))"))
                    continue;

                int index = line.IndexOf("=");
                if (index <= 0 || (index + 1) >= line.Length)
                    throw new FormatException(string.Format("This line is not formatted correctly.line:{0}", line));

                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1).Trim();
                if (string.IsNullOrEmpty(key))
                    throw new FormatException(string.Format("The key is null or empty.line:{0}", line));

                if (dict.ContainsKey(key))
                    throw new AlreadyExistsException(string.Format("This key already exists.line:{0}", line));

                dict.Add(key, value);
            }
        }

        public override bool IsEmpty { get { return dict.Count == 0; } }

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
