using Loxodon.Framework.Observables;
using System;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class ObservableTargetProxy : ValueTargetProxyBase
    {
        protected readonly IObservableProperty observableProperty;

        public ObservableTargetProxy(object target, IObservableProperty observableProperty) : base(target)
        {
            this.observableProperty = observableProperty;
        }

        public override Type Type { get { return this.observableProperty.Type; } }

        public override BindingMode DefaultMode { get { return BindingMode.TwoWay; } }

        public override object GetValue()
        {
            return observableProperty.Value;
        }

        public override TValue GetValue<TValue>()
        {
            if (this.observableProperty is IObservableProperty<TValue>)
                return ((IObservableProperty<TValue>)this.observableProperty).Value;

            return (TValue)this.observableProperty.Value;
        }

        public override void SetValue(object value)
        {
            this.observableProperty.Value = value;
        }

        public override void SetValue<TValue>(TValue value)
        {
            if (this.observableProperty is IObservableProperty<TValue>)
            {
                ((IObservableProperty<TValue>)this.observableProperty).Value = value;
                return;
            }

            this.observableProperty.Value = value;
        }

        protected override void DoSubscribeForValueChange(object target)
        {
            this.observableProperty.ValueChanged += OnValueChanged;
        }

        protected override void DoUnsubscribeForValueChange(object target)
        {
            this.observableProperty.ValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            this.RaiseValueChanged();
        }
    }
}
