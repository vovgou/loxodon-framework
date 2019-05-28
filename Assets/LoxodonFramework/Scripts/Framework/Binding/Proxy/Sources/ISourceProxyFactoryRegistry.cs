namespace Loxodon.Framework.Binding.Proxy.Sources
{
    public interface ISourceProxyFactoryRegistry
    {
        void Register(ISourceProxyFactory factory, int priority = 100);

        void Unregister(ISourceProxyFactory factory);
    }
}
