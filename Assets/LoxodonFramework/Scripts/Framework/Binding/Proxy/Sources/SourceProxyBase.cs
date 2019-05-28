using Loxodon.Log;
using System;

namespace Loxodon.Framework.Binding.Proxy.Sources
{
    public abstract class SourceProxyBase : BindingProxyBase, ISourceProxy
    {
        protected readonly object source;

        public SourceProxyBase(object source)
        {
            this.source = source;
        }

        public abstract Type Type { get; }

        public virtual object Source { get { return this.source; } }
    }

    public abstract class NotifiableSourceProxyBase : SourceProxyBase, INotifiable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NotifiableSourceProxyBase));

        protected readonly object _lock = new object();
        protected EventHandler valueChanged;

        public NotifiableSourceProxyBase(object source) : base(source)
        {
        }

        public virtual event EventHandler ValueChanged
        {
            add
            {
                lock (_lock) { this.valueChanged += value; }
            }

            remove
            {
                lock (_lock) { this.valueChanged -= value; }
            }
        }

        protected virtual void RaiseValueChanged()
        {
            try
            {
                if (this.valueChanged != null)
                    this.valueChanged(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn(e);
            }
        }
    }
}
