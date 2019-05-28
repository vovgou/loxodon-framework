using System;

namespace Loxodon.Framework.Binding.Reflection
{
    public class WeakProxyInvoker : IProxyInvoker
    {
        private WeakReference target;
        private IProxyMethodInfo proxyMethodInfo;
        public WeakProxyInvoker(WeakReference target, IProxyMethodInfo proxyMethodInfo)
        {
            this.target = target;
            this.proxyMethodInfo = proxyMethodInfo;
        }

        public virtual IProxyMethodInfo ProxyMethodInfo { get { return this.proxyMethodInfo; } }

        public object Invoke(params object[] args)
        {
            if (this.proxyMethodInfo.IsStatic)
                return this.proxyMethodInfo.Invoke(null, args);

            if (this.target == null || !this.target.IsAlive)
                return null;

            var obj = this.target.Target;
            if (obj == null)
                return null;

            return this.proxyMethodInfo.Invoke(obj, args);
        }
    }
}
