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
                if (this.property == null)
                {
                    lock (this)
                    {
                        if (this.property == null)
                        {
                            this.property = Localization.Current.Get<IObservableProperty>(key);
                            this.property.ValueChanged += OnValueChanged;
                        }
                    }
                }
                return this.property;
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
