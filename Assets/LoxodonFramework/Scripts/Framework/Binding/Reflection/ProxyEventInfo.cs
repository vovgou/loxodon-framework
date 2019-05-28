using System;
using System.Reflection;

namespace Loxodon.Framework.Binding.Reflection
{
    public class ProxyEventInfo : IProxyEventInfo
    {
        protected EventInfo eventInfo;
        public ProxyEventInfo(EventInfo eventInfo)
        {
            this.eventInfo = eventInfo;
        }

        public Type DeclaringType { get { return this.eventInfo.DeclaringType; } }

        public string Name { get { return this.eventInfo.Name; } }

        public bool IsStatic { get { return this.eventInfo.IsStatic(); } }

        public Type HandlerType { get { return this.eventInfo.EventHandlerType; } }

        public void Add(object target, Delegate handler)
        {
            this.eventInfo.AddEventHandler(target, handler);
        }

        public void Remove(object target, Delegate handler)
        {
            this.eventInfo.RemoveEventHandler(target, handler);
        }
    }
}
