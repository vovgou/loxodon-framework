namespace Loxodon.Framework.Services
{
    public abstract class AbstractServiceBundle : IServiceBundle
    {
        private IServiceContainer container;
        public AbstractServiceBundle(IServiceContainer container)
        {
            this.container = container;
        }

        public void Start()
        {
            this.OnStart(container);
        }

        protected abstract void OnStart(IServiceContainer container);

        public void Stop()
        {
            this.OnStop(container);
        }

        protected abstract void OnStop(IServiceContainer container);

    }
}
