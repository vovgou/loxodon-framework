using System;
using System.Reflection;

namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyPropertyInfo
    {
        PropertyInfo PropertyInfo { get; }

        bool IsStatic { get; }

        Type DeclaringType { get; }

        Type PropertyType { get; }

        string Name { get; }

        bool CanRead { get; }

        bool CanWrite { get; }

        object GetValue(object target, object index = null);

        void SetValue(object target, object newValue, object index = null);

    }

    public interface IProxyPropertyInfo<T, TValue> : IProxyPropertyInfo
    {
        TValue GetValue(T target);

        void SetValue(T target, TValue newValue);
    }

    public interface IProxyPropertyInfo<T, TIndex, TValue> : IProxyPropertyInfo
    {
        TValue GetValue(T target, TIndex index);

        void SetValue(T target, TValue newValue, TIndex index);
    }
}
