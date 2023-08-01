/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Loxodon.Framework.Views.InteractionActions
{
    public class AsyncViewInteractionAction : AsyncLoadableInteractionActionBase<VisibilityNotification>
    {
        private UIView view;
        private bool autoDestroy;
        public AsyncViewInteractionAction(string viewName, bool autoDestroy = true) : this(viewName, null, autoDestroy)
        {
        }

        public AsyncViewInteractionAction(string viewName, IUIViewLocator locator, bool autoDestroy = true) : base(viewName, locator)
        {
            this.autoDestroy = autoDestroy;
        }

        public AsyncViewInteractionAction(UIView view, bool autoDestroy = false) : base(null, null,null)
        {
            this.view = view;
            this.autoDestroy = autoDestroy;
        }

        public UIView View { get { return this.view; } }

        public override Task Action(VisibilityNotification notification)
        {
            if (notification.Visible)
                return Show(notification.ViewModel, notification.WaitDisabled);
            else
                return Hide();
        }

        protected async Task Show(object viewModel, bool waitDisabled)
        {
            try
            {
                if (view == null)
                    view = await this.LoadViewAsync<UIView>();

                if (view == null)
                    throw new NotFoundException(string.Format("Not found the view named \"{0}\".", ViewName));

                if (autoDestroy)
                {
                    view.WaitDisabled().Callbackable().OnCallback(r =>
                    {
                        this.view = null;
                    });
                }

                if (viewModel != null)
                    view.SetDataContext(viewModel);

                view.Visibility = true;

                if (waitDisabled)
                    await view.WaitDisabled();
            }
            catch (Exception e)
            {
                if (autoDestroy)
                    Destroy();
                throw e;
            }
        }

        protected Task Hide()
        {
            if (view != null)
            {
                this.view.Visibility = false;
                if (autoDestroy)
                    Destroy();
            }
            return Task.CompletedTask;
        }

        private void Destroy()
        {
            if (view == null)
                return;

            GameObject go = view.Owner;
            if (go != null)
                GameObject.Destroy(go);
            this.view = null;
        }
    }
}
