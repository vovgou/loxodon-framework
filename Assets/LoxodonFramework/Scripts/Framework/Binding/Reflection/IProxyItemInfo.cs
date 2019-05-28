using System;

namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyItemInfo : IProxyMemberInfo
    {
        Type ValueType { get; }

        object GetValue(object target, object key);

        void SetValue(object target, object key, object value);
    }

    public interface IProxyItemInfo<TKey, TValue> : IProxyItemInfo
    {
        TValue GetValue(object target, TKey key);

        void SetValue(object target, TKey key, TValue value);
    }

    public interface IProxyItemInfo<T, TKey, TValue> : IProxyItemInfo<TKey, TValue>
    {
        TValue GetValue(T target, TKey key);

        void SetValue(T target, TKey key, TValue value);
    }
}
