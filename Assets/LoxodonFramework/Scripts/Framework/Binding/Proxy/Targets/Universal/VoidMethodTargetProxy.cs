using System;
using System.Reflection;

using Loxodon.Log;
using Loxodon.Framework.Binding.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class VoidMethodTargetProxy : AbstractProxy, ITargetProxy, IObtainable
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(VoidMethodTargetProxy));

        private readonly WeakReference target;
        protected IProxyInvoker invoker;
        public VoidMethodTargetProxy(MethodInfo methodInfo) : this(null, methodInfo)
        {
        }

        public VoidMethodTargetProxy(object target, MethodInfo methodInfo)
        {
            if (!methodInfo.ReturnType.Equals(typeof(void)))
                throw new ArgumentException("methodInfo");

            if (target != null)
                this.target = new WeakReference(target, true);

            this.invoker = methodInfo.AsProxy(target);
        }

        public virtual BindingMode DefaultMode { get { return BindingMode.OneWayToSource; } }

        public virtual Type Type { get { return typeof(IProxyInvoker); } }

        public virtual object Target { get { return this.target != null && this.target.IsAlive ? this.target.Target : null; } }

        public virtual object GetValue()
        {
            return this.invoker;
        }
    }
}
