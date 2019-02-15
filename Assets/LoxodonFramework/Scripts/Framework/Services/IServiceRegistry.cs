using System;

namespace Loxodon.Framework.Services
{
    public interface IServiceRegistry
    {
        void Register(Type type, object target);

        void Register<T>(T target);

        void Register(string name, object target);

        void Register<T>(string name, T target);

        void Register<T>(Func<T> factory);

        void Register<T>(string name, Func<T> factory);

        void Unregister<T>();

        void Unregister(Type type);

        void Unregister(string name);
    }
}
