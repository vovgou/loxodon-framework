using System;
using Loxodon.Log;
using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class ObservablePropertyObjectSourceProxy : AbstractObjectSourceProxy
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(ObservablePropertyObjectSourceProxy));
        private bool disposed = false;
        protected IObservableProperty observableProperty;

        public ObservablePropertyObjectSourceProxy(IObservableProperty observableProperty) : this(null,observableProperty)
        {
        }

        public ObservablePropertyObjectSourceProxy(object source, IObservableProperty observableProperty) : base(source)
        {
            this.observableProperty = observableProperty;
            this.observableProperty.ValueChanged += OnValueChanged;
        }

        public override Type Type { get { return observableProperty.Type; } }

        protected virtual void OnValueChanged(object sender, EventArgs e)
        {
            this.RaiseValueChanged();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                this.observableProperty.ValueChanged -= OnValueChanged;
                disposed = true;
                base.Dispose(disposing);
            }
        }

        public override object GetValue()
        {
            return this.observableProperty.Value;
        }

        public override void SetValue(object value)
        {
            this.observableProperty.Value = value;
        }
    }
}
