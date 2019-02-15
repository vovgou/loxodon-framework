using System;
using System.Reflection;

using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class EventObjectSourceProxy : AbstractObjectSourceProxy
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(EventObjectSourceProxy));

        private bool disposed = false;
        private bool isStatic = false;
        protected readonly EventInfo eventInfo;
        protected Delegate handler;

        public EventObjectSourceProxy(EventInfo eventInfo) : this(null, eventInfo)
        {
        }

        public EventObjectSourceProxy(object source, EventInfo eventInfo) : base(source)
        {
            this.eventInfo = eventInfo;
            this.isStatic = this.eventInfo.IsStatic();
        }

        public override Type Type { get { return eventInfo.EventHandlerType; } }

        public override object GetValue()
        {
            throw new NotSupportedException();
        }

        public override void SetValue(object value)
        {
            Unbind(this.Source);

            if (value == null)
                return;

            if (value.GetType().Equals(this.Type))
            {
                this.handler = (Delegate)value;
                Bind(this.Source);
                return;
            }

            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                var source = this.Source;
                if (this.isStatic || source != null)
                    this.Unbind(source);

                disposed = true;
                base.Dispose(disposing);
            }
        }

        protected virtual void Bind(object target)
        {
            if (this.handler == null)
                return;

            if (this.eventInfo != null)
                this.eventInfo.AddEventHandler(target, handler);
        }

        protected virtual void Unbind(object target)
        {
            if (this.handler == null)
                return;

            if (this.eventInfo != null)
                this.eventInfo.RemoveEventHandler(target, handler);

            this.handler = null;
        }
    }
}
