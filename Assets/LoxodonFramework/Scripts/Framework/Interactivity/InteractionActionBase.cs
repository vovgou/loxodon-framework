using System;

namespace Loxodon.Framework.Interactivity
{
    public abstract class InteractionActionBase<TNotification> : IInteractionAction
    {
        public void OnRequest(object sender, InteractionEventArgs args)
        {
            Action callback = args.Callback;
            TNotification notification = (TNotification)args.Context;
            this.Action(notification, callback);
        }

        public abstract void Action(TNotification notification, Action callback);
    }
}
