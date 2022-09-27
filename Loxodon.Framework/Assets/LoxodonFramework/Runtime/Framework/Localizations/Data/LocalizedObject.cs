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

using Loxodon.Framework.Observables;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Loxodon.Framework.Localizations
{
    public class LocalizedObject<T> : Dictionary<string, T>, IObservableProperty<T>, IDisposable
    {
        private readonly object _lock = new object();
        private EventHandler valueChanged;
        private Localization localization;

        public LocalizedObject() : this(null, Localization.Current)
        {
        }

        public LocalizedObject(IDictionary<string, T> source) : this(source, Localization.Current)
        {
        }

        public LocalizedObject(IDictionary<string, T> source, Localization localization) : base()
        {
            if (source != null)
            {
                foreach (var kv in source)
                {
                    this.Add(kv.Key, kv.Value);
                }
            }
            this.localization = localization;
            if (localization != null)
                localization.CultureInfoChanged += OnCultureInfoChanged;
        }

        public event EventHandler ValueChanged
        {
            add { lock (_lock) { this.valueChanged += value; } }
            remove { lock (_lock) { this.valueChanged -= value; } }
        }

        public virtual Type Type { get { return typeof(T); } }

        protected void RaiseValueChanged()
        {
            this.valueChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual T Value
        {
            get { return this.GetValue(localization != null ? localization.CultureInfo : CultureInfo.CurrentUICulture); }
            set { throw new NotSupportedException(); }
        }

        object IObservableProperty.Value
        {
            get { return this.GetValue(localization != null ? localization.CultureInfo : CultureInfo.CurrentUICulture); }
            set { throw new NotSupportedException(); }
        }

        private void OnCultureInfoChanged(object sender, EventArgs e)
        {
            try
            {
                this.RaiseValueChanged();
            }
            catch (Exception) { }
        }

        protected virtual T GetValue(CultureInfo cultureInfo)
        {
            T value = default(T);
            if (this.TryGetValue(cultureInfo.Name, out value))
                return value;

            if (this.TryGetValue(cultureInfo.TwoLetterISOLanguageName, out value))
                return value;

            var ie = this.Values.GetEnumerator();
            if (ie.MoveNext())
                return ie.Current;

            return value;
        }

        public static implicit operator T(LocalizedObject<T> localized)
        {
            return localized.Value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is LocalizedObject<T>))
                return false;

            if (this == obj)
                return true;

            LocalizedObject<T> localized = (LocalizedObject<T>)obj;
            if (this.Equals(localized))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            var value = this.Value;
            if (value == null)
                return 0;

            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            var value = this.Value;
            if (value == null)
                return string.Empty;

            return value.ToString();
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (localization != null)
                    localization.CultureInfoChanged -= OnCultureInfoChanged;

                disposed = true;
            }
        }

        ~LocalizedObject()
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
