namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public interface ITargetProxyFactoryRegister
    {
        void Register(ITargetProxyFactory factory, int priority = 100);

        void Unregister(ITargetProxyFactory factory);
    }
}
