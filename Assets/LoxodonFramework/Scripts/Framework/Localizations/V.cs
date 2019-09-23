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

using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Localizations
{
    public class V<T> : IObservableProperty<T>
    {
        private readonly object _lock = new object();
        private EventHandler valueChanged;
        private string key;
        private IObservableProperty property;

        public event EventHandler ValueChanged
        {
            add { lock (_lock) { this.valueChanged += value; } }
            remove { lock (_lock) { this.valueChanged -= value; } }
        }

        public V(string key) : base()
        {
            this.key = key;
        }

        public virtual Type Type { get { return typeof(T); } }

        private void OnValueChanged(object sender, EventArgs e)
        {
            this.RaiseValueChanged();
        }

        protected void RaiseValueChanged()
        {
            var handler = this.valueChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected IObservableProperty Property
        {
            get
            {
                if (this.property != null)
                    return this.property;

                lock (this)
                {
                    if (this.property == null)
                    {
                        this.property = Localization.Current.GetValue(key);
                        this.property.ValueChanged += OnValueChanged;
                    }
                    return this.property;
                }
            }
        }

        public T Value
        {
            get { return (T)this.Property.Value; }
            set { this.Property.Value = value; }
        }

        object IObservableProperty.Value
        {
            get { return this.Value; }
            set { this.Value = (T)value; }
        }

        public static implicit operator T(V<T> data)
        {
            return data.Value;
        }

        public override string ToString()
        {
            var v = this.Value;
            if (v == null)
                return "";

            return v.ToString();
        }
    }
}
