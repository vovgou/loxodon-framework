using Loxodon.Framework.Binding.Reflection;
using System;

using INotifyPropertyChanged = System.ComponentModel.INotifyPropertyChanged;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class PropertyTargetProxy : ValueTargetProxyBase
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(PropertyTargetProxy));

        protected readonly IProxyPropertyInfo propertyInfo;

        public PropertyTargetProxy(object target, IProxyPropertyInfo propertyInfo) : base(target)
        {
            this.propertyInfo = propertyInfo;
        }

        public override Type Type { get { return this.propertyInfo.ValueType; } }

        public override BindingMode DefaultMode { get { return BindingMode.TwoWay; } }

        public override object GetValue()
        {
            var target = this.Target;
            if (target == null)
                return null;

            return propertyInfo.GetValue(target);
        }

        public override TValue GetValue<TValue>()
        {
            var target = this.Target;
            if (target == null)
                return default(TValue);

            if (propertyInfo is IProxyPropertyInfo<TValue>)
                return ((IProxyPropertyInfo<TValue>)propertyInfo).GetValue(target);

            return (TValue)propertyInfo.GetValue(target);
        }

        public override void SetValue(object value)
        {
            var target = this.Target;
            if (target == null)
                return;

            this.propertyInfo.SetValue(target, value);
        }

        public override void SetValue<TValue>(TValue value)
        {
            var target = this.Target;
            if (target == null)
                return;

            if (propertyInfo is IProxyPropertyInfo<TValue>)
            {
                ((IProxyPropertyInfo<TValue>)propertyInfo).SetValue(target, value);
                return;
            }

            this.propertyInfo.SetValue(target, value);
        }

        protected override void DoSubscribeForValueChange(object target)
        {
            if (target is INotifyPropertyChanged)
            {
                var targetNotify = target as INotifyPropertyChanged;
                targetNotify.PropertyChanged += OnPropertyChanged;
            }
        }

        protected override void DoUnsubscribeForValueChange(object target)
        {
            if (target is INotifyPropertyChanged)
            {
                var targetNotify = target as INotifyPropertyChanged;
                targetNotify.PropertyChanged -= OnPropertyChanged;
            }
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var name = e.PropertyName;
            if (string.IsNullOrEmpty(name) || name.Equals(this.propertyInfo.Name))
            {
                var target = this.Target;
                if (target == null)
                    return;

                this.RaiseValueChanged();
            }
        }
    }
}
