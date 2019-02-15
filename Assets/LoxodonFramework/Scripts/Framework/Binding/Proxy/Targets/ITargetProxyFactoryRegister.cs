namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public interface ITargetProxyFactoryRegister
    {
        void Register(AbstractTargetProxyFactory factory, int priority = 100);
    }
}
