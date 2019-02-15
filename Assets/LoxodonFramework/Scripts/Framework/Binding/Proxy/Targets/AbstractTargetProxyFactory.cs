using System;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public abstract class AbstractTargetProxyFactory : ITargetProxyFactory
    {
        public ITargetProxy CreateProxy(object target, BindingDescription description)
        {
            if (target == null)
                return null;

            ITargetProxy proxy = null;
            if (TryCreateProxy(target, description, out proxy))
                return proxy;
            return proxy;
        }

        protected abstract bool TryCreateProxy(object target, BindingDescription description, out ITargetProxy proxy);
    }
}
