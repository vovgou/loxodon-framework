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

using Loxodon.Log;
using System;
using XLua;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public abstract class LuaTableNodeProxy<TKey> : NotifiableSourceProxyBase, IObtainable, IModifiable
    {
        protected static readonly string PROPERTY_CHANGED_EVENT_SUBSCRIBE_NAME = "subscribe";
        protected static readonly string PROPERTY_CHANGED_EVENT_UNSUBSCRIBE_NAME = "unsubscribe";

        //private static readonly ILog log = LogManager.GetLogger(typeof(LuaTableNodeProxy<TKey>));

        private bool disposed = false;
        protected readonly TKey key;
        protected readonly LuaTable table;

        public LuaTableNodeProxy(LuaTable source, TKey key) : base(source)
        {
            if (source == null)
                throw new ArgumentException("source");

            this.key = key;
            this.table = source;

            this.Subscribe();
        }

        public override Type Type { get { return typeof(object); } }

        public TKey Key { get { return this.key; } }

        protected abstract void Subscribe();

        protected abstract void Unsubscribe();

        protected virtual void OnPropertyChanged()
        {
            this.RaiseValueChanged();
        }

        public virtual object GetValue()
        {
            return this.table.Get<TKey, object>(this.Key);
        }

        public TValue GetValue<TValue>()
        {
            return this.table.Get<TKey, TValue>(this.Key);
        }

        public void SetValue(object value)
        {
            //if (this.IsEquals(value))
            //    return;

            this.table.Set<TKey, object>(this.Key, value);
        }

        public void SetValue<TValue>(TValue value)
        {
            //if (this.IsEquals(value))
            //    return;

            this.table.Set<TKey, TValue>(this.Key, value);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                this.Unsubscribe();
                disposed = true;
                base.Dispose(disposing);
            }
        }

        //protected bool IsEquals(object obj)
        //{
        //    var value = this.GetValue();
        //    if (value != null)
        //        return value.Equals(obj);

        //    if (obj != null)
        //        return obj.Equals(value);

        //    return true;
        //}
    }

    public class LuaIntTableNodeProxy : LuaTableNodeProxy<int>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LuaIntTableNodeProxy));

        private Action onPropertyChanged;

        private ILuaObservableObject observableObject;

        public LuaIntTableNodeProxy(LuaTable source, int key) : base(source, key)
        {
        }

        protected override void Subscribe()
        {
            if (this.table.ContainsKey(PROPERTY_CHANGED_EVENT_SUBSCRIBE_NAME) && this.table.ContainsKey(PROPERTY_CHANGED_EVENT_UNSUBSCRIBE_NAME))
            {
                observableObject = this.table.Cast<ILuaObservableObject>();
                this.onPropertyChanged = this.OnPropertyChanged;
                observableObject.subscribe(this.Key, this.onPropertyChanged);
            }
            else
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The LuaTable \"{0}\" can't be listened.Not found the method named \"{1}\" or \"{2}\". Key:{3}", this.Source, PROPERTY_CHANGED_EVENT_SUBSCRIBE_NAME, PROPERTY_CHANGED_EVENT_UNSUBSCRIBE_NAME, Key);
            }
        }

        protected override void Unsubscribe()
        {
            if (this.observableObject != null)
                observableObject.unsubscribe(this.Key, this.onPropertyChanged);
        }
    }

    public class LuaStringTableNodeProxy : LuaTableNodeProxy<string>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LuaStringTableNodeProxy));

        private Action onPropertyChanged;

        private ILuaObservableObject observableObject;

        public LuaStringTableNodeProxy(LuaTable source, string key) : base(source, key)
        {
        }

        protected override void Subscribe()
        {
            if (this.table.ContainsKey(PROPERTY_CHANGED_EVENT_SUBSCRIBE_NAME) && this.table.ContainsKey(PROPERTY_CHANGED_EVENT_UNSUBSCRIBE_NAME))
            {
                observableObject = this.table.Cast<ILuaObservableObject>();
                this.onPropertyChanged = this.OnPropertyChanged;
                observableObject.subscribe(this.Key, this.onPropertyChanged);
            }
            else
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The LuaTable \"{0}\" can't be listened.Not found the method named \"{1}\" or \"{2}\". Key:{3}", this.Source, PROPERTY_CHANGED_EVENT_SUBSCRIBE_NAME, PROPERTY_CHANGED_EVENT_UNSUBSCRIBE_NAME, Key);
            }
        }

        protected override void Unsubscribe()
        {
            if (this.observableObject != null)
                observableObject.unsubscribe(this.Key, this.onPropertyChanged);
        }
    }
}
