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
using Loxodon.Framework.Binding.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class ItemTargetProxy<TKey> : ValueTargetProxyBase
    {
        protected readonly IProxyItemInfo itemInfo;
        protected readonly TKey key;
        public ItemTargetProxy(object target, TKey key, IProxyItemInfo itemInfo) : base(target)
        {
            this.key = key;
            this.itemInfo = itemInfo;
        }

        public override Type Type { get { return this.itemInfo.ValueType; } }

        public override TypeCode TypeCode { get { return itemInfo.ValueTypeCode; } }

        public override BindingMode DefaultMode { get { return BindingMode.OneWay; } }

        public override object GetValue()
        {
            var target = this.Target;
            if (target == null)
                return null;

            return itemInfo.GetValue(target, key);
        }

        public override TValue GetValue<TValue>()
        {
            var target = this.Target;
            if (target == null)
                return default(TValue);

            if (!typeof(TValue).IsAssignableFrom(this.itemInfo.ValueType))
                throw new InvalidCastException();

            var proxy = itemInfo as IProxyItemInfo<TKey, TValue>;
            if (proxy != null)
                return proxy.GetValue(target, key);

            return (TValue)this.itemInfo.GetValue(target, key);
        }

        public override void SetValue(object value)
        {
            var target = this.Target;
            if (target == null)
                return;

            this.itemInfo.SetValue(target, key, value);
        }

        public override void SetValue<TValue>(TValue value)
        {
            var target = this.Target;
            if (target == null)
                return;

            var proxy = itemInfo as IProxyItemInfo<TKey, TValue>;
            if (proxy != null)
            {
                proxy.SetValue(target, key, value);
                return;
            }

            this.itemInfo.SetValue(target, key, value);
        }
    }
}
