using System;
using System.Collections.Generic;

using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class TargetProxyCreationService : ITargetProxyFactoryRegister, ITargetProxyFactory
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TargetProxyCreationService));

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

        public void Register(AbstractTargetProxyFactory factory, int priority = 100)
        {
            if (factory == null)
                return;

            this.factories.Add(new PriorityFactoryPair(factory, priority));
            this.factories.Sort((x, y) => y.priority.CompareTo(x.priority));
        }

        struct PriorityFactoryPair
        {
            public PriorityFactoryPair(AbstractTargetProxyFactory factory, int priority)
            {
                this.factory = factory;
                this.priority = priority;
            }

            public int priority;
            public AbstractTargetProxyFactory factory;
        }
    }
}
