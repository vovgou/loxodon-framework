using System;

namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyFactory
    {
        IProxyType Create(Type type);
    }
}
