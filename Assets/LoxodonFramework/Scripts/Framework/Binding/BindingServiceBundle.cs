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

            ObjectSourceProxyCreationService objectSourceProxyCreationService = new ObjectSourceProxyCreationService();
            objectSourceProxyCreationService.Register(new UniversalObjectSourceProxyFactory(), 0);
            objectSourceProxyCreationService.Register(new InteractionRequestSourceProxyFactory(), 1);

            SourceProxyCreationService sourceFactoryService = new SourceProxyCreationService();
            sourceFactoryService.Register(new LiteralSourceProxyFactory(), 0);
            sourceFactoryService.Register(new ExpressionSourceProxyFactory(sourceFactoryService, expressionPathFinder), 1);
            sourceFactoryService.Register(objectSourceProxyCreationService, 2);

            TargetProxyCreationService targetFactoryService = new TargetProxyCreationService();
            targetFactoryService.Register(new UniversalTargetProxyFactory(), 0);
            targetFactoryService.Register(new UnityTargetProxyFactory(), 10);
            targetFactoryService.Register(new ObservablePropertyTargetProxyFactory(), 20);

            BindingFactory bindingFactory = new BindingFactory(sourceFactoryService, targetFactoryService);
            StandardBinder binder = new StandardBinder(bindingFactory);
            ProxyFactory proxyFactory = ProxyFactory.Default;

            PropertyInfo info = typeof(UnityEngine.GameObject).GetProperty("activeSelf");
            proxyFactory.Register(new ProxyPropertyInfo<UnityEngine.GameObject, bool>(info, g => g.activeSelf, (g, v) => g.SetActive(v)));

            container.Register<IBinder>(binder);
            container.Register<IBindingFactory>(bindingFactory);
            container.Register<IConverterRegistry>(converterRegistry);

            container.Register<IExpressionPathFinder>(expressionPathFinder);
            container.Register<IPathParser>(pathParser);

            container.Register<IObjectSourceProxyFactory>(objectSourceProxyCreationService);
            container.Register<IObjectSourceProxyFactoryRegistry>(objectSourceProxyCreationService);

            container.Register<ISourceProxyFactory>(sourceFactoryService);
            container.Register<ISourceProxyFactoryRegistry>(sourceFactoryService);

            container.Register<ITargetProxyFactory>(targetFactoryService);
            container.Register<ITargetProxyFactoryRegister>(targetFactoryService);

            container.Register<IProxyFactory>(proxyFactory);
            container.Register<IProxyRegistry>(proxyFactory);
        }

        protected override void OnStop(IServiceContainer container)
        {
            container.Unregister<IBinder>();
            container.Unregister<IBindingFactory>();
            container.Unregister<IConverterRegistry>();

            container.Unregister<IExpressionPathFinder>();
            container.Unregister<IPathParser>();

            container.Unregister<IObjectSourceProxyFactory>();
            container.Unregister<IObjectSourceProxyFactoryRegistry>();

            container.Unregister<ISourceProxyFactory>();
            container.Unregister<ISourceProxyFactoryRegistry>();

            container.Unregister<ITargetProxyFactory>();
            container.Unregister<ITargetProxyFactoryRegister>();

            container.Unregister<IProxyFactory>();
            container.Unregister<IProxyRegistry>();
        }
    }
}
