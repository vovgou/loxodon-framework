using System;

namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyMemberInfo
    {
        Type DeclaringType { get; }

        string Name { get; }

        bool IsStatic { get; }
    }
}
