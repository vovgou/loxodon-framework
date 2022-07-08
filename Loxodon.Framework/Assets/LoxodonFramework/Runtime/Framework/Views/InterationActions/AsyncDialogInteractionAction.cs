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
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using Loxodon.Log;
using System;
using System.Threading.Tasks;

namespace Loxodon.Framework.Views.InteractionActions
{
    public class AsyncDialogInteractionAction : AsyncInteractionActionBase<object>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AsyncDialogInteractionAction));

        private string viewName;

        public AsyncDialogInteractionAction(string viewName)
        {
            this.viewName = viewName;
        }
        public override async Task Action(object viewModel)
        {
            Window window = null;
            try
            {
                ApplicationContext context = Context.GetApplicationContext();
                IUIViewLocator locator = context.GetService<IUIViewLocator>();
                if (locator == null)
                    throw new NotFoundException("Not found the \"IUIViewLocator\".");

                if (string.IsNullOrEmpty(viewName))
                    throw new ArgumentNullException("The view name is null.");

                window = await locator.LoadWindowAsync<Window>(viewName);
                if (window == null)
                    throw new NotFoundException(string.Format("Not found the dialog window named \"{0}\".", viewName));

                if (window is AlertDialogWindow && viewModel is AlertDialogViewModel)
                {
                    (window as AlertDialogWindow).ViewModel = viewModel as AlertDialogViewModel;
                }
                else if (window is AlertDialogWindow && viewModel is DialogNotification notification)
                {
                    AlertDialogViewModel dialogViewModel = new AlertDialogViewModel();
                    dialogViewModel.Message = notification.Message;
                    dialogViewModel.Title = notification.Title;
                    dialogViewModel.ConfirmButtonText = notification.ConfirmButtonText;
                    dialogViewModel.NeutralButtonText = notification.NeutralButtonText;
                    dialogViewModel.CancelButtonText = notification.CancelButtonText;
                    dialogViewModel.CanceledOnTouchOutside = notification.CanceledOnTouchOutside;
                    dialogViewModel.Click = (result) => notification.DialogResult = result;
                    (window as AlertDialogWindow).ViewModel = dialogViewModel;
                }
                else
                {
                    window.SetDataContext(viewModel);
                }

                window.Create();
                await window.Show(true);
                await window.WaitDismissed();
            }
            catch (Exception e)
            {
                if (window != null)
                    await window.Dismiss();

                if (log.IsWarnEnabled)
                    log.Error("", e);
            }
        }
    }
}
