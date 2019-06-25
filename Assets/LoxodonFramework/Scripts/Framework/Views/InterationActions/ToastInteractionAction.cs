using Loxodon.Framework.Interactivity;
using System;

namespace Loxodon.Framework.Views.InteractionActions
{
    public class ToastInteractionAction : InteractionActionBase<ToastNotification>
    {
        private string viewName;
        private IUIViewGroup viewGroup;

        public ToastInteractionAction(IUIViewGroup viewGroup) : this(viewGroup, null)
        {
        }

        public ToastInteractionAction(IUIViewGroup viewGroup, string viewName)
        {
            this.viewGroup = viewGroup;
            this.viewName = viewName;
        }

        public override void Action(ToastNotification notification, Action callback)
        {
            if (notification == null)
                return;

            Toast.Show(this.viewName, this.viewGroup, notification.Message, notification.Duration, null, callback);
        }
    }
}
