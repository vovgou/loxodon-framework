using Loxodon.Framework.Binding.Reflection;
using System;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class EventNodeProxy : SourceProxyBase, ISourceProxy, IModifiable
    {
        protected readonly IProxyEventInfo eventInfo;
        private bool disposed = false;
        private bool isStatic = false;
        protected Delegate handler;

        public EventNodeProxy(IProxyEventInfo eventInfo) : this(null, eventInfo)
        {
        }

        public EventNodeProxy(object source, IProxyEventInfo eventInfo) : base(source)
        {
            this.eventInfo = eventInfo;
            this.isStatic = this.eventInfo.IsStatic;
        }

        public override Type Type { get { return this.eventInfo.HandlerType; } }

        public virtual void SetValue<TValue>(TValue value)
        {
            SetValue((object)value);
        }

        public virtual void SetValue(object value)
        {
            if (value != null && !value.GetType().Equals(this.Type))
                throw new ArgumentException("Binding delegate to event failed, mismatched delegate type", "value");

            Unbind(this.Source, this.handler);
            this.handler = (Delegate)value;
            Bind(this.Source, this.handler);
        }

        protected virtual void Bind(object target, Delegate handler)
        {
            if (handler == null)
                return;

            if (this.eventInfo != null)
                this.eventInfo.Add(target, handler);
        }

        protected virtual void Unbind(object target, Delegate handler)
        {
            if (handler == null)
                return;

            if (this.eventInfo != null)
                this.eventInfo.Remove(target, handler);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                var source = this.Source;
                if (this.isStatic || source != null)
                    this.Unbind(source, handler);

                this.handler = null;
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
