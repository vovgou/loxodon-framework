using System;

namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyFieldInfo : IProxyMemberInfo
    {
        Type ValueType { get; }

        object GetValue(object target);

        void SetValue(object target, object value);
    }

    public interface IProxyFieldInfo<TValue> : IProxyFieldInfo
    {
        new TValue GetValue(object target);

        void SetValue(object target, TValue value);
    }

    public interface IProxyFieldInfo<T, TValue> : IProxyFieldInfo<TValue>
    {
        TValue GetValue(T target);

        void SetValue(T target, TValue value);
    }
}
