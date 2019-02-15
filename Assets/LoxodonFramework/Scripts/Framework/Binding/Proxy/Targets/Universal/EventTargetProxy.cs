using System;
using System.Reflection;

using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class EventTargetProxy : AbstractTargetProxy
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(EventTargetProxy));

        private bool disposed = false;
        protected readonly EventInfo eventInfo;
        protected Delegate handler;

        public override Type Type { get { return this.eventInfo.EventHandlerType; } }

        public override BindingMode DefaultMode { get { return BindingMode.OneWay; } }

        public EventTargetProxy(object target, EventInfo eventInfo) : base(target)
        {
            this.eventInfo = eventInfo;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                var target = this.Target;
                if (this.eventInfo.IsStatic() || target != null)
                    this.Unbind(target);

                disposed = true;
                base.Dispose(disposing);
            }
        }

        protected override void SetValueImpl(object target, object value)
        {
            Unbind(target);

            if (value == null)
                return;

            if (value.GetType().Equals(this.Type))
            {
                this.handler = (Delegate)value;
                Bind(target);
                return;
            }

            throw new NotSupportedException();
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
