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
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Loxodon.Framework.ObjectPool
{
    public class MixedObjectPool<T> : IMixedObjectPool<T> where T : class
    {
        private const int DEFAULT_MAX_SIZE_PER_TYPE = 8;
        private readonly ConcurrentDictionary<string, List<T>> entries;
        private readonly ConcurrentDictionary<string, int> typeSize;
        private readonly IMixedObjectFactory<T> factory;
        private readonly object _lock = new object();
        private int defaultMaxSizePerType;
        public MixedObjectPool(IMixedObjectFactory<T> factory) : this(factory, DEFAULT_MAX_SIZE_PER_TYPE)
        {
        }

        public MixedObjectPool(IMixedObjectFactory<T> factory, int defaultMaxSizePerType)
        {
            this.factory = factory;
            this.defaultMaxSizePerType = defaultMaxSizePerType;

            if (defaultMaxSizePerType <= 0)
                throw new ArgumentException("the maxSize must be greater than 0");

            this.entries = new ConcurrentDictionary<string, List<T>>();
            this.typeSize = new ConcurrentDictionary<string, int>();
        }

        public int GetMaxSize(string typeName)
        {
            int size;
            if (typeSize.TryGetValue(typeName, out size))
                return size;
            return defaultMaxSizePerType;
        }

        public void SetMaxSize(string typeName, int value)
        {
            this.typeSize.AddOrUpdate(typeName, value, (key, oldValue) => value);
        }

        public T Allocate(string typeName)
        {
            if (this.disposed)
                throw new ObjectDisposedException(GetType().Name);

            lock (_lock)
            {
                List<T> list;
                if (entries.TryGetValue(typeName, out list) && list.Count > 0)
                {
                    T obj = list[0];
                    list.RemoveAt(0);
                    return obj;
                }
            }

            return factory.Create(this, typeName);
        }

        public void Free(string typeName, T obj)
        {
            if (obj == null)
                return;

            if (this.disposed || !factory.Validate(typeName, obj))
            {
                factory.Destroy(typeName, obj);
                return;
            }

            lock (_lock)
            {
                int maxSize = GetMaxSize(typeName);
                List<T> list = entries.GetOrAdd(typeName, n => new List<T>());
                if (list.Count >= maxSize)
                {
                    factory.Destroy(typeName, obj);
                    return;
                }

                factory.Reset(typeName, obj);
                list.Add(obj);
            }
        }

        protected virtual void Clear()
        {
            lock (_lock)
            {
                foreach (var kv in entries)
                {
                    string typeName = kv.Key;
                    List<T> list = kv.Value;
                    if (list == null || list.Count <= 0)
                        continue;

                    list.ForEach(e => factory.Destroy(typeName, e));
                    list.Clear();
                }
                entries.Clear();
                typeSize.Clear();
            }
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                this.Clear();
                disposed = true;
            }
        }

        ~MixedObjectPool()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
