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
            ITargetProxy proxy = null;
            if (TryCreateProxy(target, description, out proxy))
                return proxy;

            if (log.IsWarnEnabled)
                log.WarnFormat("Unable to bind: not found {0} on {1}", description.TargetName, target.GetType().Name);

            throw new BindingException("Unable to bind: \"{0}\"", description.ToString());
        }

        protected virtual bool TryCreateProxy(object target, BindingDescription description, out ITargetProxy proxy)
        {
            proxy = null;
            foreach (PriorityFactoryPair pair in this.factories)
            {
                try
                {
                    var factory = pair.factory;
                    if (factory == null)
                        continue;

                    proxy = factory.CreateProxy(target, description);
                    if (proxy != null)
                        return true;
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Unable to bind:{0};exception:{1}", description.ToString(), e);
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
