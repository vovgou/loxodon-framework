using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Binding.Reflection
{
    public class ProxyFactory
    {
        public static readonly ProxyFactory Default = new ProxyFactory();

        private readonly object _lock = new object();
        private readonly Dictionary<Type, ProxyType> types = new Dictionary<Type, ProxyType>();

        public IProxyType Get(Type type)
        {
            return GetType(type);
        }

        protected virtual ProxyType GetType(Type type)
        {
            ProxyType ret;
            if (this.types.TryGetValue(type, out ret) && ret != null)
                return ret;

            return Create(type);
        }

        public void Register(IProxyMemberInfo proxyMemberInfo)
        {
            if (proxyMemberInfo == null)
                return;

            ProxyType proxyType = this.GetType(proxyMemberInfo.DeclaringType);
            proxyType.Register(proxyMemberInfo);
        }

        public void Unregister(IProxyMemberInfo proxyMemberInfo)
        {
            if (proxyMemberInfo == null)
                return;

            ProxyType proxyType = this.GetType(proxyMemberInfo.DeclaringType);
            proxyType.Unregister(proxyMemberInfo);
        }

        protected ProxyType Create(Type type)
        {
            lock (_lock)
            {
                ProxyType proxyType = new ProxyType(type, this);
                this.types.Add(type, proxyType);
                return proxyType;
            }
        }
    }
}
