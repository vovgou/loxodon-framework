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
using Loxodon.Framework.ViewModels;
using Loxodon.Log;
using System;
using System.Threading.Tasks;

namespace Loxodon.Framework.Views.InteractionActions
{
    public class AsyncDialogInteractionAction : AsyncLoadableInteractionActionBase<object>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AsyncDialogInteractionAction));

        private Window window;

        public AsyncDialogInteractionAction(string viewName) : base(viewName, null, null)
        {
        }

        public AsyncDialogInteractionAction(string viewName, IUIViewLocator locator) : base(viewName, locator)
        {
        }
        public Window Window { get { return this.window; } }

        public override Task Action(object context)
        {
            if (context is WindowNotification notification)
            {
                bool ignoreAnimation = notification.IgnoreAnimation;
                switch (notification.ActionType)
                {
                    case Interactivity.ActionType.CREATE:
                        return Create(notification.ViewModel);
                    case Interactivity.ActionType.SHOW:
                        return Show(notification.ViewModel, ignoreAnimation);
                    case Interactivity.ActionType.HIDE:
                        return Hide(ignoreAnimation);
                    case Interactivity.ActionType.DISMISS:
                        return Dismiss(ignoreAnimation);
                }
                return Task.CompletedTask;
            }
            else
            {
                return Show(context);
            }
        }

        protected async Task Create(object viewModel)
        {
            try
            {
                window = await LoadWindowAsync<Window>();
                if (window == null)
                    throw new NotFoundException(string.Format("Not found the dialog window named \"{0}\".", ViewName));

                if (window is AlertDialogWindowBase && viewModel is AlertDialogViewModel)
                {
                    (window as AlertDialogWindowBase).ViewModel = viewModel as AlertDialogViewModel;
                }
                else if (window is AlertDialogWindowBase && viewModel is DialogNotification notification)
                {
                    AlertDialogViewModel dialogViewModel = new AlertDialogViewModel();
                    dialogViewModel.Message = notification.Message;
                    dialogViewModel.Title = notification.Title;
                    dialogViewModel.ConfirmButtonText = notification.ConfirmButtonText;
                    dialogViewModel.NeutralButtonText = notification.NeutralButtonText;
                    dialogViewModel.CancelButtonText = notification.CancelButtonText;
                    dialogViewModel.CanceledOnTouchOutside = notification.CanceledOnTouchOutside;
                    dialogViewModel.Click = (result) => notification.DialogResult = result;
                    (window as AlertDialogWindowBase).ViewModel = dialogViewModel;
                }
                else
                {
                    if (viewModel != null)
                        window.SetDataContext(viewModel);
                }

                window.Create();
            }
            catch (Exception e)
            {
                window = null;
                throw e;
            }
        }

        protected async Task Show(object viewModel, bool ignoreAnimation = false)
        {
            try
            {
                if (window == null)
                    await Create(viewModel);

                await window.Show(ignoreAnimation);
                await window.WaitDismissed();
                window = null;
            }
            catch (Exception e)
            {
                if (window != null)
                    await window.Dismiss(ignoreAnimation);
                window = null;
                throw e;
            }
        }

        protected async Task Hide(bool ignoreAnimation = false)
        {
            if (window != null)
                await window.Hide(ignoreAnimation);
        }

        protected async Task Dismiss(bool ignoreAnimation = false)
        {
            if (window != null)
                await window.Dismiss(ignoreAnimation);
        }
    }
}
