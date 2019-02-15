using System.Collections.Generic;

using Loxodon.Log;
using Loxodon.Framework.Binding.Paths;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class ObjectSourceProxyCreationService : TypedSourceProxyFactory<ObjectSourceDescription>, IObjectSourceProxyFactory, IObjectSourceProxyFactoryRegistry
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ObjectSourceProxyCreationService));

        private List<PriorityFactoryPair> factories = new List<PriorityFactoryPair>();

        public void Register(ISpecificTypeObjectSourceProxyFactory factory, int priority = 100)
        {
            if (factory == null)
                return;

            this.factories.Add(new PriorityFactoryPair(factory, priority));
            this.factories.Sort((x, y) => y.priority.CompareTo(x.priority));
        }

        public void Unregister(ISpecificTypeObjectSourceProxyFactory factory)
        {
            if (factory == null)
                return;

            this.factories.RemoveAll(p => p.factory == factory);
        }

        protected override bool TryCreateProxy(object source, ObjectSourceDescription description, out ISourceProxy proxy)
        {
            proxy = null;
            var path = description.Path;
            if (path.Count <= 0)
            {
                if (log.IsWarnEnabled)
                    log.Warn("Unable to bind: an empty path node list!.");
                return false;
            }

            if (path.IsStatic && path.Count < 2)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Unable to bind: the \"{0}\" path be unsupported.", path.ToString());
                return false;
            }

            PathToken token = description.Path.AsPathToken();
            proxy = CreateProxy(source, token);
            if (proxy != null)
                return true;
            return false;
        }

        public IObjectSourceProxy CreateProxy(object source, PathToken token)
        {
            IObjectSourceProxy proxy = null;
            foreach (PriorityFactoryPair pair in this.factories)
            {
                var factory = pair.factory;
                if (factory == null)
                    continue;

                if (factory.TryCreateProxy(source, token, this, out proxy))
                    return proxy;
            }
            return proxy;
        }
    }

    struct PriorityFactoryPair
    {
        public PriorityFactoryPair(ISpecificTypeObjectSourceProxyFactory factory, int priority)
        {
            this.factory = factory;
            this.priority = priority;
        }
        public int priority;
        public ISpecificTypeObjectSourceProxyFactory factory;
    }
}
