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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Loxodon.Log;

namespace Loxodon.Framework.Utilities
{
    [Serializable]
    public class WeakValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary where TValue : class
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WeakValueDictionary<TKey, TValue>));
        private const int MIN_CLEANUP_INTERVAL = 500;
        private int cleanupFlag = 0;
        protected Dictionary<TKey, WeakReference<TValue>> dictionary;

        public WeakValueDictionary()
        {
            this.dictionary = new Dictionary<TKey, WeakReference<TValue>>();
        }
        public WeakValueDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = new Dictionary<TKey, WeakReference<TValue>>();
            foreach (var kv in dictionary)
                this.dictionary.Add(kv.Key, new WeakReference<TValue>(kv.Value));
        }
        public WeakValueDictionary(IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, WeakReference<TValue>>(comparer);
        }
        public WeakValueDictionary(int capacity)
        {
            this.dictionary = new Dictionary<TKey, WeakReference<TValue>>(capacity);
        }
        public WeakValueDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, WeakReference<TValue>>(comparer);
            foreach (var kv in dictionary)
                this.dictionary.Add(kv.Key, new WeakReference<TValue>(kv.Value));
        }
        public WeakValueDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, WeakReference<TValue>>(capacity, comparer);
        }

        public TValue this[TKey key]
        {
            get
            {
                CleanupCheck();

                if (!dictionary.ContainsKey(key))
                    return default(TValue);
                return dictionary[key].Target;
            }
            set { Insert(key, value, false); }
        }

        public ICollection<TKey> Keys { get { return new KeyCollection(this.dictionary); } }

        public ICollection<TValue> Values { get { return new ValueCollection(this.dictionary); } }

        public void Add(TKey key, TValue value)
        {
            CleanupCheck();

            Insert(key, value, true);
        }

        public bool Remove(TKey key)
        {
            CleanupCheck();

            if (key == null)
                throw new ArgumentNullException("key");

            return this.dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            CleanupCheck();

            WeakReference<TValue> item;
            if (this.dictionary.TryGetValue(key, out item))
                value = item.Target;
            else
                value = null;
            return value != null;
        }

        public bool ContainsKey(TKey key)
        {
            CleanupCheck();

            WeakReference<TValue> item;
            if (this.dictionary.TryGetValue(key, out item) && item.IsAlive)
                return true;
            return false;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Insert(item.Key, item.Value, true);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            WeakReference<TValue> value;
            if (!this.dictionary.TryGetValue(item.Key, out value))
                return false;

            if (value.IsAlive && EqualityComparer<TValue>.Default.Equals(value.Target, item.Value))
                return true;
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            //if (array == null)
            //    throw new ArgumentNullException("array");

            //if (arrayIndex < 0 || arrayIndex > array.Length)
            //    throw new ArgumentOutOfRangeException("arrayIndex");

            //if (array.Length - arrayIndex < Count)
            //    throw new ArgumentException("array too small.");

            //var e = this.dictionary.GetEnumerator();
            //while (e.MoveNext())
            //{
            //    var kv = e.Current;
            //    if (kv.GetHashCode() >= 0)
            //        array[arrayIndex++] = new KeyValuePair<TKey, TValue>(kv.Key, kv.Value.Target);
            //}
            throw new NotSupportedException();
        }

        public int Count
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary)this.dictionary).IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var e = this.dictionary.GetEnumerator();
            while (e.MoveNext())
            {
                var kv = e.Current;
                if (kv.Value.IsAlive)
                    yield return new KeyValuePair<TKey, TValue>(kv.Key, kv.Value.Target);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IDictionary<TKey, TValue>).GetEnumerator();
        }

        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (items.Count > 0)
            {
                if (this.dictionary.Count > 0 && items.Keys.Any((k) => this.ContainsKey(k)))
                    throw new ArgumentException("An item with the same key has already been added.");

                foreach (var item in items)
                    this.dictionary.Add(item.Key, new WeakReference<TValue>(item.Value));
            }
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (add)
            {
                WeakReference<TValue> item;
                if (dictionary.TryGetValue(key, out item) && item.IsAlive)
                    throw new ArgumentException("An item with the same key has already been added.");
            }

            dictionary[key] = new WeakReference<TValue>(value);
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (!(key is TKey))
                    return null;
                if (!dictionary.ContainsKey((TKey)key))
                    return null;
                return dictionary[(TKey)key].Target;
            }
            set { Insert((TKey)key, (TValue)value, false); }
        }

        ICollection IDictionary.Keys { get { return new KeyCollection(this.dictionary); } }

        ICollection IDictionary.Values { get { return new ValueCollection(this.dictionary); } }

        bool IDictionary.Contains(object key)
        {
            if (!(key is TKey))
                return false;
            return (this as IDictionary<TKey, TValue>).ContainsKey((TKey)key);
        }

        void IDictionary.Add(object key, object value)
        {
            this.Add((TKey)key, (TValue)value);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotSupportedException();
        }

        void IDictionary.Remove(object key)
        {
            this.Remove((TKey)key);
        }

        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary)this.dictionary).IsFixedSize; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            //if (array == null)
            //    throw new ArgumentNullException("array");

            //if (index < 0 || index > array.Length)
            //    throw new ArgumentOutOfRangeException("arrayIndex");

            //if (array.Length - index < Count)
            //    throw new ArgumentException("array too small.");


            //KeyValuePair<TKey, TValue>[] pairs = array as KeyValuePair<TKey, TValue>[];
            //if (pairs != null)
            //{
            //    CopyTo(pairs, index);
            //    return;
            //}
            throw new NotSupportedException();
        }

        object ICollection.SyncRoot
        {
            get { return ((IDictionary)this.dictionary).SyncRoot; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((IDictionary)this.dictionary).IsSynchronized; }
        }

        protected virtual void CleanupCheck()
        {
            this.cleanupFlag++;
            if (this.cleanupFlag < MIN_CLEANUP_INTERVAL + this.dictionary.Count)
                return;

            this.cleanupFlag = 0;
            Cleanup();
        }

        /// <summary>
        /// Removes the left-over weak references for entries in the dictionary whose value has already been reclaimed by the garbage collector. 
        /// </summary>
        protected virtual void Cleanup()
        {
            try
            {
                lock ((this.dictionary as IDictionary).SyncRoot)
                {
                    List<TKey> keys = new List<TKey>();
                    var e = this.dictionary.GetEnumerator();
                    while (e.MoveNext())
                    {
                        var kv = e.Current;
                        if (!kv.Value.IsAlive)
                            keys.Add(kv.Key);
                    }

                    for (int i = 0; i < keys.Count; i++)
                        this.dictionary.Remove(keys[i]);
                }
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Removes the left-over weak references for entries in the dictionary whose value has already been reclaimed by the garbage collector.Error:{0}", e);
            }
        }

        [Serializable]
        protected class KeyCollection : ICollection<TKey>, ICollection
        {
            private Dictionary<TKey, WeakReference<TValue>> dictionary;

            public KeyCollection(Dictionary<TKey, WeakReference<TValue>> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                this.dictionary = dictionary;
            }

            public IEnumerator GetEnumerator()
            {
                return (this as IEnumerable<TKey>).GetEnumerator();
            }

            public void CopyTo(TKey[] array, int index)
            {
                throw new NotSupportedException();
            }

            public int Count
            {
                get
                {
                    throw new NotSupportedException();
                }
            }

            bool ICollection<TKey>.IsReadOnly
            {
                get { return true; }
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            Object ICollection.SyncRoot
            {
                get { return ((ICollection)dictionary).SyncRoot; }
            }

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException();
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TKey>.Contains(TKey item)
            {
                throw new NotSupportedException();
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                var e = this.dictionary.GetEnumerator();
                while (e.MoveNext())
                {
                    var kv = e.Current;
                    if (kv.Value.IsAlive)
                        yield return kv.Key;
                }
            }

            public void CopyTo(Array array, int index)
            {
                throw new NotSupportedException();
            }
        }


        [Serializable]
        protected class ValueCollection : ICollection<TValue>, ICollection
        {
            private Dictionary<TKey, WeakReference<TValue>> dictionary;

            public ValueCollection(Dictionary<TKey, WeakReference<TValue>> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                this.dictionary = dictionary;
            }

            public IEnumerator GetEnumerator()
            {
                return (this as IEnumerable<TValue>).GetEnumerator();
            }

            public void CopyTo(TValue[] array, int index)
            {
                //if (array == null)
                //    throw new ArgumentNullException("array");

                //if (index < 0 || index > array.Length)
                //    throw new ArgumentOutOfRangeException("arrayIndex");

                //if (array.Length - index < Count)
                //    throw new ArgumentException("array too small.");

                //var e = this.dictionary.GetEnumerator();
                //while (e.MoveNext())
                //{
                //    var kv = e.Current;
                //    if (kv.GetHashCode() >= 0 && kv.Value.IsAlive)
                //        array[index++] = kv.Value.Target;
                //}
                throw new NotSupportedException();
            }

            public int Count
            {
                get
                {
                    throw new NotSupportedException();
                }
            }

            bool ICollection<TValue>.IsReadOnly
            {
                get { return true; }
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            Object ICollection.SyncRoot
            {
                get { return ((ICollection)dictionary).SyncRoot; }
            }

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException();
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                throw new NotSupportedException();
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                var e = this.dictionary.GetEnumerator();
                while (e.MoveNext())
                {
                    var kv = e.Current;
                    if (kv.Value.IsAlive)
                        yield return kv.Value.Target;
                }
            }

            public void CopyTo(Array array, int index)
            {
                //if (array == null)
                //    throw new ArgumentNullException("array");

                //if (index < 0 || index > array.Length)
                //    throw new ArgumentOutOfRangeException("arrayIndex");

                //if (array.Length - index < Count)
                //    throw new ArgumentException("array too small.");


                //TValue[] pairs = array as TValue[];
                //if (pairs != null)
                //{
                //    CopyTo(pairs, index);
                //    return;
                //}
                throw new NotSupportedException();
            }
        }

        protected class WeakReference<T> : WeakReference
        {
            public WeakReference(T target) : base(target) { }
            public WeakReference(T target, bool trackResurrection) : base(target, trackResurrection) { }

            public new T Target
            {
                get { return (T)base.Target; }
                set { base.Target = value; }
            }
        }
    }
}
