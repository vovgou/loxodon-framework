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

using System;
using System.Collections.Generic;
using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Sources
{
    public class SourceProxyFactory : ISourceProxyFactory, ISourceProxyFactoryRegistry
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SourceProxyFactory));

        private List<PriorityFactoryPair> factories = new List<PriorityFactoryPair>();

        public ISourceProxy CreateProxy(object source, SourceDescription description)
        {
            try
            {
                if (!description.IsStatic && source == null)
                    return new EmptSourceProxy(description);

                ISourceProxy proxy = null;
                if (TryCreateProxy(source, description, out proxy))
                    return proxy;

                throw new NotSupportedException("Not found available proxy factory.");
            }
            catch (Exception e)
            {
                throw new ProxyException(e, "An exception occurred while creating a proxy for the \"{0}\".", description.ToString());
            }
        }

        protected virtual bool TryCreateProxy(object source, SourceDescription description, out ISourceProxy proxy)
        {
            proxy = null;
            foreach (PriorityFactoryPair pair in this.factories)
            {
                var factory = pair.factory;
                if (factory == null)
                    continue;

                try
                {
                    proxy = factory.CreateProxy(source, description);
                    if (proxy != null)
                        return true;
                }
                catch (MissingMemberException e)
                {
                    throw e;
                }
                catch (NullReferenceException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("An exception occurred when using the \"{0}\" factory to create a proxy for the \"{1}\";exception:{2}", factory.GetType().Name, description.ToString(), e);
                }
            }

            proxy = null;
            return false;
        }

        public void Register(ISourceProxyFactory factory, int priority = 100)
        {
            if (factory == null)
                return;

            this.factories.Add(new PriorityFactoryPair(factory, priority));
            this.factories.Sort((x, y) => y.priority.CompareTo(x.priority));
        }

        public void Unregister(ISourceProxyFactory factory)
        {
            if (factory == null)
                return;

            this.factories.RemoveAll(pair => pair.factory == factory);
        }

        struct PriorityFactoryPair
        {
            public PriorityFactoryPair(ISourceProxyFactory factory, int priority)
            {
                this.factory = factory;
                this.priority = priority;
            }
            public int priority;
            public ISourceProxyFactory factory;
        }
    }
}
