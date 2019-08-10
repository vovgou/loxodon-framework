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
using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Configurations
{
    class SubsetConfiguration : ConfigurationBase
    {
        private string prefix;
        private ConfigurationBase parent;
        public SubsetConfiguration(ConfigurationBase parent, string prefix)
        {
            this.parent = parent;
            this.prefix = prefix;
        }

        protected string GetParentKey(string key)
        {
            if ("".Equals(key) || key == null)
                throw new ArgumentNullException(key);

            return prefix + KEY_DELIMITER + key;
        }

        protected string GetChildKey(string key)
        {
            if (!key.StartsWith(prefix))
                throw new ArgumentException(string.Format("The parent key '{0}' is not in the subset.", key));

            return key.Substring(prefix.Length + KEY_DELIMITER.Length);
        }

        public override IConfiguration Subset(string prefix)
        {
            return parent.Subset(GetParentKey(prefix));
        }

        public override bool ContainsKey(string key)
        {
            return parent.ContainsKey(GetParentKey(key));
        }

        public override IEnumerator<string> GetKeys()
        {
            return new TransformEnumerator<string, string>(parent.GetKeys(prefix), (key) => GetChildKey(key));
        }

        public override object GetProperty(string key)
        {
            return parent.GetProperty(GetParentKey(key));
        }

        public override void AddProperty(string key, object value)
        {
            parent.AddProperty(GetParentKey(key), value);
        }

        public override void SetProperty(string key, object value)
        {
            parent.SetProperty(GetParentKey(key), value);
        }

        public override void RemoveProperty(string key)
        {
            parent.RemoveProperty(GetParentKey(key));
        }

        public override void Clear()
        {
            IEnumerator<string> it = GetKeys();
            for (; it.MoveNext();)
            {
                RemoveProperty(it.Current);
            }
        }
    }
}
