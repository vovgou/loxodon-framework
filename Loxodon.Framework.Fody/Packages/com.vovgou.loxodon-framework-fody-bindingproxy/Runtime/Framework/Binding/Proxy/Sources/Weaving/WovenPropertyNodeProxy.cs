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

namespace Loxodon.Framework.Binding.Proxy.Sources.Weaving
{
    public abstract class WovenPropertyNodeProxy<T, TValue> : NotifiableSourceProxyBase, IObtainable, IObtainable<TValue>, IModifiable, IModifiable<TValue>, INotifiable, IPropertyNodeProxy where T : class
    {
        private readonly Type valueType;
        protected new T source;
        public WovenPropertyNodeProxy(T source) : base(source)
        {
            this.source = source;
            this.valueType = typeof(TValue);
        }

        public override Type Type { get { return valueType; } }

        public abstract TValue GetValue();

        public void OnPropertyChanged()
        {
            this.RaiseValueChanged();
        }

        public abstract void SetValue(TValue value);

        object IObtainable.GetValue()
        {
            return this.GetValue();
        }

        TValue1 IObtainable.GetValue<TValue1>()
        {
            if (this is IObtainable<TValue1> proxy)
                return proxy.GetValue();

            return (TValue1)Convert.ChangeType(GetValue(), typeof(TValue1));
        }

        void IModifiable.SetValue(object value)
        {
            this.SetValue((TValue)value);
        }

        void IModifiable.SetValue<TValue1>(TValue1 value)
        {
            if (this is IModifiable<TValue1> proxy)
                proxy.SetValue(value);
            else
                SetValue((TValue)Convert.ChangeType(value, typeof(TValue)));
        }
    }
}
