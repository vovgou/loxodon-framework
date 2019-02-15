using System;
using System.Reflection;

namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyType
    {
        Type Type { get; }

        IProxyFieldInfo GetField(string name);

        IProxyFieldInfo GetField(FieldInfo field);

        IProxyPropertyInfo GetProperty(string name);

        IProxyPropertyInfo GetProperty(PropertyInfo property);

        IProxyMethodInfo GetMethod(string name);

        IProxyMethodInfo GetMethod(string name,Type[] parameterTypes);

        IProxyMethodInfo GetMethod(MethodInfo method);

    }
}
