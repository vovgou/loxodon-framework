using System;

namespace Loxodon.Framework.Observables
{
    [Serializable]
    public class ObservableProperty : IObservableProperty
    {
        private readonly object _lock = new object();
        private EventHandler valueChanged;
        private object _value;

        public event EventHandler ValueChanged
        {
            add { lock (_lock) { this.valueChanged += value; } }
            remove { lock (_lock) { this.valueChanged -= value; } }
        }

        public ObservableProperty() : this(null)
        {
        }
        public ObservableProperty(object value)
        {
            this._value = value;
        }

        public virtual Type Type { get { return this._value != null ? this._value.GetType() : typeof(object); } }

        public virtual object Value
        {
            get { return this._value; }
            set
            {
                if (object.Equals(this._value, value))
                    return;

                this._value = value;
                this.RaiseValueChanged();
            }
        }

        protected void RaiseValueChanged()
        {
            var handler = this.valueChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public override string ToString()
        {
            var v = this.Value;
            if (v == null)
                return "";

            return v.ToString();
        }
    }

    public class ObservableProperty<T> : ObservableProperty, IObservableProperty<T>
    {
        public ObservableProperty() : this(default(T))
        {
        }
        public ObservableProperty(T value) : base(value)
        {
        }

        public override Type Type { get { return typeof(T); } }

        public new virtual T Value
        {
            get { return (T)base.Value; }
            set { base.Value = value; }
        }

        public static implicit operator T(ObservableProperty<T> data)
        {
            return data.Value;
        }

        public static implicit operator ObservableProperty<T>(T data)
        {
            return new ObservableProperty<T>(data);
        }
    }

}
