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

using Loxodon.Framework.Binding.Paths;
using Loxodon.Framework.Binding.Proxy.Sources;
using Loxodon.Framework.Binding.Proxy.Sources.Expressions;
using Loxodon.Framework.Binding.Proxy.Sources.Object;
using Loxodon.Framework.Binding.Proxy.Targets;
using Loxodon.Framework.Services;

namespace Loxodon.Framework.Binding
{
    public class LuaBindingServiceBundle : BindingServiceBundle
    {
        public LuaBindingServiceBundle(IServiceContainer container) : base(container)
        {
        }

        protected override void OnStart(IServiceContainer container)
        {
            base.OnStart(container);

            /* Support XLua */
            INodeProxyFactoryRegister objectSourceProxyFactoryRegistry = container.Resolve<INodeProxyFactoryRegister>();
            objectSourceProxyFactoryRegistry.Register(new LuaNodeProxyFactory(), 20);

            IPathParser pathParser = container.Resolve<IPathParser>();
            ISourceProxyFactory sourceFactoryService = container.Resolve<ISourceProxyFactory>();
            ISourceProxyFactoryRegistry sourceProxyFactoryRegistry = container.Resolve<ISourceProxyFactoryRegistry>();
            sourceProxyFactoryRegistry.Register(new LuaExpressionSourceProxyFactory(sourceFactoryService, pathParser), 20);

            ITargetProxyFactoryRegister targetProxyFactoryRegister = container.Resolve<ITargetProxyFactoryRegister>();
            targetProxyFactoryRegister.Register(new LuaTargetProxyFactory(), 30);
        }

        protected override void OnStop(IServiceContainer container)
        {
            base.OnStop(container);
        }
    }
}