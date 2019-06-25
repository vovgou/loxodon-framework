using Loxodon.Framework.Interactivity;
using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Views.InteractionActions
{
    public class LoadingInteractionAction : InteractionActionBase<VisibilityNotification>
    {
        private List<Loading> list = new List<Loading>();
        public override void Action(VisibilityNotification notification, Action callback)
        {
            if (notification.Visible)
            {
                Loading loading = Loading.Show(true);
                if (loading != null)
                    list.Insert(0, loading);
            }
            else
            {
                if (list.Count <= 0)
                    return;

                Loading loading = list[0];
                list.RemoveAt(0);
                loading.Dispose();
            }
        }
    }
}
