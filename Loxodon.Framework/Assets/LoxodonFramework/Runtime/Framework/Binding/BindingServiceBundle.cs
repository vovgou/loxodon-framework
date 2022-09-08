/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using Loxodon.Framework.Binding.Binders;
using Loxodon.Framework.Binding.Converters;
using Loxodon.Framework.Binding.Paths;
using Loxodon.Framework.Binding.Proxy.Sources;
using Loxodon.Framework.Binding.Proxy.Sources.Expressions;
using Loxodon.Framework.Binding.Proxy.Sources.Object;
using Loxodon.Framework.Binding.Proxy.Sources.Text;
using Loxodon.Framework.Binding.Proxy.Targets;
using Loxodon.Framework.Services;

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
#if UNITY_2019_1_OR_NEWER
            targetFactory.Register(new VisualElementProxyFactory(), 30);
#endif

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
