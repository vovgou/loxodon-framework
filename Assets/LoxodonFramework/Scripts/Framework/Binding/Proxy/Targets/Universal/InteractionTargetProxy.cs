using Loxodon.Framework.Interactivity;
using System;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class InteractionTargetProxy : TargetProxyBase, IObtainable
    {
        protected readonly EventHandler<InteractionEventArgs> handler;

        public InteractionTargetProxy(object target, IInteractionAction interactionAction) : base(target)
        {
            this.handler = interactionAction.OnRequest;
        }

        public override Type Type { get { return typeof(EventHandler<InteractionEventArgs>); } }

        public override BindingMode DefaultMode { get { return BindingMode.OneWayToSource; } }

        public object GetValue()
        {
            return handler;
        }

        public TValue GetValue<TValue>()
        {
            return (TValue)GetValue();
        }
    }
}
