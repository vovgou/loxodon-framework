using System;
using System.Reflection;

namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyType
    {
        Type Type { get; }

        IProxyMemberInfo GetMember(string name);

        IProxyMemberInfo GetMember(string name, BindingFlags flags);

        IProxyFieldInfo GetField(string name);

        IProxyFieldInfo GetField(string name, BindingFlags flags);

        IProxyPropertyInfo GetProperty(string name);

        IProxyPropertyInfo GetProperty(string name, BindingFlags flags);

        IProxyItemInfo GetItem();

        IProxyMethodInfo GetMethod(string name);

        IProxyMethodInfo GetMethod(string name, BindingFlags flags);

        IProxyMethodInfo GetMethod(string name, Type[] parameterTypes);

        IProxyMethodInfo GetMethod(string name, Type[] parameterTypes, BindingFlags flags);

        IProxyEventInfo GetEvent(string name);
    }
}
