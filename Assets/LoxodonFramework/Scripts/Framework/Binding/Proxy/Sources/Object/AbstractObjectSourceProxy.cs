namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    using System;

    using Loxodon.Log;
    using Loxodon.Framework.Binding.Proxy;

    public abstract class AbstractObjectSourceProxy : AbstractProxy, IObjectSourceProxy
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(AbstractObjectSourceProxy));

        private readonly object source;

        //JIT Exception
        //public event EventHandler<EventArgs> ValueChanged;
        private readonly object eventLock = new object();
        private EventHandler<EventArgs> valueChanged;
        public event EventHandler<EventArgs> ValueChanged
        {
            add { lock (eventLock) { this.valueChanged += value; } }
            remove { lock (eventLock) { this.valueChanged -= value; } }
        }


        public AbstractObjectSourceProxy(object source)
        {
            this.source = source;
        }

        public virtual Type Type { get { return typeof(object); } }

        public object Source { get { return this.source; } }

        public abstract object GetValue();

        public abstract void SetValue(object value);

        protected void RaiseValueChanged()
        {
            var handler = this.valueChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
