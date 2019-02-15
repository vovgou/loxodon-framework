using System;
using System.Collections.Generic;
using System.Reflection;

namespace Loxodon.Framework.Binding.Reflection
{
    public static class ProxyTypeExtentions
    {
        static readonly ProxyFactory factory = ProxyFactory.Default;
        public static IProxyType AsProxy(this Type type)
        {
            return factory.Create(type);
        }

        public static IProxyFieldInfo AsProxy(this FieldInfo info)
        {
            IProxyType proxyType = factory.Create(info.DeclaringType);
            return proxyType.GetField(info);
        }

        public static IProxyPropertyInfo AsProxy(this PropertyInfo info)
        {
            IProxyType proxyType = factory.Create(info.DeclaringType);
            return proxyType.GetProperty(info);
        }

        public static IProxyMethodInfo AsProxy(this MethodInfo info)
        {
            IProxyType proxyType = factory.Create(info.DeclaringType);
            return proxyType.GetMethod(info);
        }

        public static IProxyInvoker AsProxy(this MethodInfo info, object target)
        {
            IProxyType proxyType = factory.Create(info.DeclaringType);
            IProxyMethodInfo proxyMethodInfo = proxyType.GetMethod(info);
            return new ProxyInvoker(target, proxyMethodInfo);
        }

        public static List<Type> GetParameterTypes(this MethodInfo info)
        {
            List<Type> list = new List<Type>();
            foreach (ParameterInfo p in info.GetParameters())
            {
                list.Add(p.ParameterType);
            }
            return list;
        }       
    }
}
