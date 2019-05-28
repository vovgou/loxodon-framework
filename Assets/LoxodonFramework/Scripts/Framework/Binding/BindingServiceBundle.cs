using System.Reflection;
using Loxodon.Framework.Services;
using Loxodon.Framework.Binding.Reflection;
using Loxodon.Framework.Binding.Converters;
using Loxodon.Framework.Binding.Binders;
using Loxodon.Framework.Binding.Paths;
using Loxodon.Framework.Binding.Proxy.Sources;
using Loxodon.Framework.Binding.Proxy.Sources.Object;
using Loxodon.Framework.Binding.Proxy.Sources.Expressions;
using Loxodon.Framework.Binding.Proxy.Sources.Text;
using Loxodon.Framework.Binding.Proxy.Targets;

namespace Loxodon.Framework.Binding
{
    public class BindingServiceBundle : AbstractServiceBundle
    {
        public BindingServiceBundle(IServiceContainer container) : base(container)
        {
        }

        protected override void OnStart(IServiceContainer container)
        {
            PathParser pathParser = new PathParser();
            ExpressionPathFinder expressionPathFinder = new ExpressionPathFinder();
            ConverterRegistry converterRegistry = new ConverterRegistry();

            ObjectSourceProxyFactory objectSourceProxyFactory = new ObjectSourceProxyFactory();
            objectSourceProxyFactory.Register(new UniversalNodeProxyFactory(), 0);

            SourceProxyFactory sourceFactory = new SourceProxyFactory();
            sourceFactory.Register(new LiteralSourceProxyFactory(), 0);
            sourceFactory.Register(new ExpressionSourceProxyFactory(sourceFactory, expressionPathFinder), 1);
            sourceFactory.Register(objectSourceProxyFactory, 2);

            TargetProxyFactory targetFactory = new TargetProxyFactory();
            targetFactory.Register(new UniversalTargetProxyFactory(), 0);
            targetFactory.Register(new UnityTargetProxyFactory(), 10);

            BindingFactory bindingFactory = new BindingFactory(sourceFactory, targetFactory);
            StandardBinder binder = new StandardBinder(bindingFactory);

            container.Register<IBinder>(binder);
            container.Register<IBindingFactory>(bindingFactory);
            container.Register<IConverterRegistry>(converterRegistry);

            container.Register<IExpressionPathFinder>(expressionPathFinder);
            container.Register<IPathParser>(pathParser);

            container.Register<INodeProxyFactory>(objectSourceProxyFactory);
            container.Register<INodeProxyFactoryRegister>(objectSourceProxyFactory);

            container.Register<ISourceProxyFactory>(sourceFactory);
            container.Register<ISourceProxyFactoryRegistry>(sourceFactory);

            container.Register<ITargetProxyFactory>(targetFactory);
            container.Register<ITargetProxyFactoryRegister>(targetFactory);
        }

        protected override void OnStop(IServiceContainer container)
        {
            container.Unregister<IBinder>();
            container.Unregister<IBindingFactory>();
            container.Unregister<IConverterRegistry>();

            container.Unregister<IExpressionPathFinder>();
            container.Unregister<IPathParser>();

            container.Unregister<INodeProxyFactory>();
            container.Unregister<INodeProxyFactoryRegister>();

            container.Unregister<ISourceProxyFactory>();
            container.Unregister<ISourceProxyFactoryRegistry>();

            container.Unregister<ITargetProxyFactory>();
            container.Unregister<ITargetProxyFactoryRegister>();
        }
    }
}
