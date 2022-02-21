using Loxodon.Framework.Binding.Proxy.Sources.Object;
using Loxodon.Framework.Binding.Proxy.Targets;
using Loxodon.Framework.Services;

namespace Loxodon.Framework.Binding
{
    public class ILRuntimeBindingServiceBundle : BindingServiceBundle
    {
        public ILRuntimeBindingServiceBundle(IServiceContainer container) : base(container)
        {
        }

        protected override void OnStart(IServiceContainer container)
        {
            base.OnStart(container);

            /* Support ILruntime */
            INodeProxyFactoryRegister objectSourceProxyFactoryRegistry = container.Resolve<INodeProxyFactoryRegister>();
            objectSourceProxyFactoryRegistry.Register(new ILRuntimeNodeProxyFactory(), 20);

            ITargetProxyFactoryRegister targetProxyFactoryRegister = container.Resolve<ITargetProxyFactoryRegister>();
            targetProxyFactoryRegister.Register(new ILRuntimeTargetProxyFactory(), 30);
        }
    }
}
