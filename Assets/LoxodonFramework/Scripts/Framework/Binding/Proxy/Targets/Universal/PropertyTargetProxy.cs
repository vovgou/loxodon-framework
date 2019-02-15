using System;
using System.Reflection;
using System.ComponentModel;

using Loxodon.Log;
using Loxodon.Framework.Binding.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class PropertyTargetProxy : AbstractTargetProxy, IObtainable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PropertyTargetProxy));

        protected readonly PropertyInfo propertyInfo;
        protected readonly IProxyPropertyInfo proxyProperty;

        public override Type Type { get { return this.propertyInfo.PropertyType; } }

        public PropertyTargetProxy(object target, PropertyInfo propertyInfo) : base(target)
        {
            this.propertyInfo = propertyInfo;
            this.proxyProperty = propertyInfo.AsProxy();
        }

        public virtual object GetValue()
        {
            var target = this.Target;
            if (target == null)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Get value ignored in target's property \"{0}\",the weak reference to the target is null.", this.propertyInfo.Name);

                return ReturnObject.UNSET;
            }

            return proxyProperty.GetValue(target);
        }

        protected override void SetValueImpl(object target, object value)
        {
            this.proxyProperty.SetValue(target, value);
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

                var value = this.proxyProperty.GetValue(target);
                this.RaiseValueChanged(value);
            }
        }
    }

}
