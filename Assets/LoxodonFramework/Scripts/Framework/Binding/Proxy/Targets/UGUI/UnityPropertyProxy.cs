using System;
using System.Reflection;
using UnityEngine.Events;

using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class UnityPropertyProxy<TValue> : PropertyTargetProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UnityPropertyProxy<TValue>));

        private UnityEvent<TValue> unityEvent;
        public UnityPropertyProxy(object target, PropertyInfo propertyInfo, UnityEvent<TValue> unityEvent) : base(target, propertyInfo)
        {
            this.unityEvent = unityEvent;
        }

        public override BindingMode DefaultMode { get { return BindingMode.TwoWay; } }

        protected override void DoSubscribeForValueChange(object target)
        {
            if (this.unityEvent == null || target == null)
                return;

            try
            {
                unityEvent.AddListener(OnValueChanged);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0} ,registration failed！Exception:{1}", typeof(UnityEvent<TValue>).Name, e);
            }
        }

        protected override void DoUnsubscribeForValueChange(object target)
        {
            if (unityEvent != null)
                unityEvent.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(TValue value)
        {
            try
            {
                this.RaiseValueChanged(value);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Property[{0}] ValueChanged Exception,{1}", this.propertyInfo.Name, e);
            }
        }
    }
}
