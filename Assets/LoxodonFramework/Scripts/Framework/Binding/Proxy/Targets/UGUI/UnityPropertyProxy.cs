using Loxodon.Framework.Binding.Reflection;
using UnityEngine.Events;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class UnityPropertyProxy<TValue> : PropertyTargetProxy
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(UnityPropertyProxy<TValue>));

        private UnityEvent<TValue> unityEvent;
        public UnityPropertyProxy(object target, IProxyPropertyInfo propertyInfo, UnityEvent<TValue> unityEvent) : base(target, propertyInfo)
        {
            this.unityEvent = unityEvent;
        }

        public override BindingMode DefaultMode { get { return BindingMode.TwoWay; } }

        protected override void DoSubscribeForValueChange(object target)
        {
            if (this.unityEvent == null || target == null)
                return;

            unityEvent.AddListener(OnValueChanged);
        }

        protected override void DoUnsubscribeForValueChange(object target)
        {
            if (unityEvent != null)
                unityEvent.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(TValue value)
        {
            this.RaiseValueChanged();
        }
    }
}
