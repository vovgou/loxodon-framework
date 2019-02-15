using System;
using System.Reflection;

namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyFieldInfo
    {
        FieldInfo FieldInfo { get; }

        Type DeclaringType { get; }

        Type FieldType { get; }

        bool IsStatic { get; }

        string Name { get; }

        object GetValue(object target);

        void SetValue(object target, object value);
    }

    public interface IProxyFieldInfo<T, TValue> : IProxyFieldInfo
    {
        TValue GetValue(T target);

        void SetValue(T target, TValue value);
    }
}
