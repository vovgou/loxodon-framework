using Loxodon.Framework.Binding.Reflection;
using System;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class EventTargetProxy : EventTargetProxyBase
    {
        private bool disposed = false;
        protected readonly IProxyEventInfo eventInfo;
        protected Delegate handler;

        public EventTargetProxy(object target, IProxyEventInfo eventInfo) : base(target)
        {
            this.eventInfo = eventInfo;
        }

        public override Type Type { get { return this.eventInfo.HandlerType; } }

        public override BindingMode DefaultMode { get { return BindingMode.OneWay; } }

        public override void SetValue(object value)
        {
            if (value != null && !value.GetType().Equals(this.Type))
                throw new ArgumentException("Binding delegate to event failed, mismatched delegate type", "value");

            var target = this.Target;
            if (target == null)
                return;

            Unbind(target);

            if (value == null)
                return;

            if (value.GetType().Equals(this.Type))
            {
                this.handler = (Delegate)value;
                Bind(target);
                return;
            }
        }

        public override void SetValue<TValue>(TValue value)
        {
            this.SetValue((object)value);
        }

        protected virtual void Bind(object target)
        {
            if (this.handler == null)
                return;

            if (this.eventInfo != null)
                this.eventInfo.Add(target, handler);
        }

        protected virtual void Unbind(object target)
        {
            if (this.handler == null)
                return;

            if (this.eventInfo != null)
                this.eventInfo.Remove(target, handler);

            this.handler = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                var target = this.Target;
                if (this.eventInfo.IsStatic || target != null)
                    this.Unbind(target);

                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
