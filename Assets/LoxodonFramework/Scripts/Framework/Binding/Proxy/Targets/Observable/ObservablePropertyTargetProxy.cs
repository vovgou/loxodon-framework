using System;

using Loxodon.Log;
using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class ObservablePropertyTargetProxy : AbstractTargetProxy, IObtainable
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(ObservablePropertyTargetProxy));

        protected readonly IObservableProperty observableProperty;

        public override Type Type { get { return this.observableProperty.Type; } }

        public override BindingMode DefaultMode { get { return BindingMode.TwoWay; } }

        public ObservablePropertyTargetProxy(object target, IObservableProperty observableProperty) : base(target)
        {
            this.observableProperty = observableProperty;
        }

        public virtual object GetValue()
        {
            return observableProperty.Value;
        }

        protected override void SetValueImpl(object target, object value)
        {
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
            this.RaiseValueChanged(this.observableProperty.Value);
        }
    }
}
