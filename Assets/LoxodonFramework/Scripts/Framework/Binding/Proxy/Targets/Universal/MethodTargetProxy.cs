using Loxodon.Framework.Binding.Reflection;
using System;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class MethodTargetProxy : TargetProxyBase, IObtainable
    {
        protected readonly IProxyMethodInfo methodInfo;
        protected IProxyInvoker invoker;
        public MethodTargetProxy(object target, IProxyMethodInfo methodInfo) : base(target)
        {
            this.methodInfo = methodInfo;
            if (!methodInfo.ReturnType.Equals(typeof(void)))
                throw new ArgumentException("methodInfo");

            this.invoker = new WeakProxyInvoker(new WeakReference(target, true), methodInfo);
        }

        public override BindingMode DefaultMode { get { return BindingMode.OneWayToSource; } }

        public override Type Type { get { return typeof(IProxyInvoker); } }

        public object GetValue()
        {
            return this.invoker;
        }

        public TValue GetValue<TValue>()
        {
            return (TValue)this.invoker;
        }
    }
}
