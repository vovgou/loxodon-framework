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

namespace Loxodon.Framework.Views
{
    public class Bundle : IBundle
    {
        protected IDictionary<string, object> data = new Dictionary<string, object>();

        public Bundle()
        {
        }

        public Bundle(IBundle bundle)
        {
            PutAll(bundle);
        }

        public virtual int Count
        {
            get { return this.data.Count; }
        }

        public virtual IDictionary<string, object> Data
        {
            get { return this.data; }
        }

        public virtual ICollection<string> Keys
        {
            get { return this.data.Keys; }
        }

        public virtual ICollection<object> Values
        {
            get { return this.data.Values; }
        }

        public virtual void Clear()
        {
            this.data.Clear();
        }

        public virtual bool ContainsKey(string key)
        {
            return this.data.ContainsKey(key);
        }

        public virtual bool Remove(string key)
        {
            return this.data.Remove(key);
        }

        public virtual T Get<T>(string key) where T : new()
        {
            return Get(key, default(T));
        }

        public virtual T Get<T>(string key, T defaultValue) where T : new()
        {
            object value;
            if (this.data.TryGetValue(key, out value))
                return (T)value;

            return defaultValue;
        }

        public virtual void Put<T>(string key, T value)
        {
            if (!IsValidType(value))
                throw new ArgumentException("Value must be serializable!");

            this.data[key] = value;
        }

        public virtual void PutAll(IBundle bundle)
        {
            foreach (KeyValuePair<string, object> kv in bundle.Data)
            {
                if (!IsValidType(kv.Value))
                    throw new ArgumentException("Value must be serializable!");

                this.data[kv.Key] = kv.Value;
            }
        }

        protected virtual bool IsValidType(object value)
        {
            return true;
        }
    }
}
