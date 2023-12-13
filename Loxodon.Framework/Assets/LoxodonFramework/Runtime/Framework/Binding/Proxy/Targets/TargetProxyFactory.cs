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

using Loxodon.Log;
using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class TargetProxyFactory : ITargetProxyFactory, ITargetProxyFactoryRegister
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TargetProxyFactory));

        private List<PriorityFactoryPair> factories = new List<PriorityFactoryPair>();

        public ITargetProxy CreateProxy(object target, BindingDescription description)
        {
            try
            {
                ITargetProxy proxy = null;
                if (TryCreateProxy(target, description, out proxy))
                    return proxy;

                throw new NotSupportedException("Not found available proxy factory.");
            }
            catch (Exception e)
            {
                throw new ProxyException(e, "Unable to bind the \"{0}\".An exception occurred while creating a proxy for the \"{1}\" property of class \"{2}\".", description.ToString(), description.TargetName, target.GetType().Name);
            }
        }

        protected virtual bool TryCreateProxy(object target, BindingDescription description, out ITargetProxy proxy)
        {
            proxy = null;
            foreach (PriorityFactoryPair pair in this.factories)
            {
                var factory = pair.factory;
                if (factory == null)
                    continue;

                try
                {
                    proxy = factory.CreateProxy(target, description);
                    if (proxy != null)
                        return true;

                }
                catch (MissingMemberException e)
                {
                    if (!TargetNameUtil.IsCollection(description.TargetName))
                        throw e;
                }
                catch (NullReferenceException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("An exception occurred when using the \"{0}\" factory to create a proxy for the \"{1}\" property of class \"{2}\";exception:{3}", factory.GetType().Name, description.TargetName, target.GetType().Name, e);
                }
            }

            return false;
        }

        public void Register(ITargetProxyFactory factory, int priority = 100)
        {
            if (factory == null)
                return;

            this.factories.Add(new PriorityFactoryPair(factory, priority));
            this.factories.Sort((x, y) => y.priority.CompareTo(x.priority));
        }

        public void Unregister(ITargetProxyFactory factory)
        {
            if (factory == null)
                return;

            this.factories.RemoveAll(pair => pair.factory == factory);
        }

        struct PriorityFactoryPair
        {
            public PriorityFactoryPair(ITargetProxyFactory factory, int priority)
            {
                this.factory = factory;
                this.priority = priority;
            }

            public int priority;
            public ITargetProxyFactory factory;
        }
    }
}
