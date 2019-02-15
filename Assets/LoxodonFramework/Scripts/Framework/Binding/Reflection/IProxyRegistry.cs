namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyRegistry
    {
        void Register(IProxyFieldInfo info);

        void Register(IProxyPropertyInfo info);

        void Register(IProxyMethodInfo info);
    }
}
