using System;

namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyObject:IDisposable
    {
        event EventHandler ValueChanged;

        Type Type { get; }

        bool IsRoot { get; }

        object Value { get; set; }

        T GetProperty<T>(string name);

        void SetProperty<T>(string name, T value);

        T GetField<T>(string name);

        void SetField<T>(string name, T value);

        object Invoke(string name, params object[] args);

        object Invoke(string name,Type[] parameterTypes, params object[] args);

        IProxyObject GetPropertyProxy(string name);

        IProxyInvoker GetMethodInvoker(string name);

        IProxyInvoker GetMethodInvoker(string name, Type[] parameterTypes);
    }

    public interface IProxyObject<T> : IProxyObject
    {
        new T Value { get; set; }

        IProxyObject<E> GetPropertyProxy<E>(string name);

        TResult Invoke<TResult>(string name, params object[] args);

        TResult Invoke<TResult>(string name, Type[] parameterTypes, params object[] args);
    }

    public interface IProxyCollection : IProxyObject
    {
        object this[object key] { get; set; }

        IProxyObject GetItemProxy(object key);
    }

    public interface IProxyCollection<T> : IProxyObject<T>, IProxyCollection
    {       
        new T this[object key] { get; set; }

        new IProxyObject<T> GetItemProxy(object key);
    }
}
