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
            return factory.Get(type);
        }

        public static IProxyEventInfo AsProxy(this EventInfo info)
        {
            IProxyType proxyType = factory.Get(info.DeclaringType);
            return proxyType.GetEvent(info.Name);
        }

        public static IProxyFieldInfo AsProxy(this FieldInfo info)
        {
            IProxyType proxyType = factory.Get(info.DeclaringType);
            if (info.IsPublic)
                return proxyType.GetField(info.Name);
            return proxyType.GetField(info.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static IProxyPropertyInfo AsProxy(this PropertyInfo info)
        {
            IProxyType proxyType = factory.Get(info.DeclaringType);
            if (info.GetGetMethod().IsPublic)
                return proxyType.GetProperty(info.Name);
            return proxyType.GetProperty(info.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static IProxyMethodInfo AsProxy(this MethodInfo info)
        {
            IProxyType proxyType = factory.Get(info.DeclaringType);
            Type[] types = info.GetParameterTypes().ToArray();
            if (info.IsPublic)
                return proxyType.GetMethod(info.Name, types);

            return proxyType.GetMethod(info.Name, types, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
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
