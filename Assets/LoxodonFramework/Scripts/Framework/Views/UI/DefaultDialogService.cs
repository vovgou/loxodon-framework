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

namespace Loxodon.Framework.Views
{
    public class DefaultDialogService : IDialogService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultDialogService));

        public virtual IAsyncResult<int> ShowDialog(string title, string message)
        {
            return this.ShowDialog(title, message, null, null, null, true);
        }

        public virtual IAsyncResult<int> ShowDialog(string title, string message, string buttonText)
        {
            return this.ShowDialog(title, message, buttonText, null, null, false);
        }

        public virtual IAsyncResult<int> ShowDialog(string title, string message, string confirmButtonText, string cancelButtonText)
        {
            return this.ShowDialog(title, message, confirmButtonText, cancelButtonText, null, false);
        }

        public virtual IAsyncResult<int> ShowDialog(string title, string message, string confirmButtonText, string cancelButtonText, string neutralButtonText)
        {
            return this.ShowDialog(title, message, confirmButtonText, cancelButtonText, neutralButtonText, false);
        }

        public virtual IAsyncResult<int> ShowDialog(string title, string message, string confirmButtonText, string cancelButtonText, string neutralButtonText, bool canceledOnTouchOutside)
        {
            AsyncResult<int> result = new AsyncResult<int>();
            try
            {
                AlertDialog.ShowMessage(message, title, confirmButtonText, neutralButtonText, cancelButtonText, canceledOnTouchOutside, (which) => { result.SetResult(which); });
            }
            catch (Exception e)
            {
                result.SetException(e);
            }
            return result;
        }

        public virtual IAsyncResult<TViewModel> ShowDialog<TViewModel>(string viewName, TViewModel viewModel) where TViewModel : IViewModel
        {
            AsyncResult<TViewModel> result = new AsyncResult<TViewModel>();
            Window window = null;
            try
            {
                ApplicationContext context = Context.GetApplicationContext();
                IUIViewLocator locator = context.GetService<IUIViewLocator>();
                if (locator == null)
                {
                    if (log.IsWarnEnabled)
                        log.Warn("Not found the \"IUIViewLocator\".");

                    throw new NotFoundException("Not found the \"IUIViewLocator\".");
                }

                if (string.IsNullOrEmpty(viewName))
                    throw new ArgumentNullException("The view name is null.");

                window = locator.LoadView<Window>(viewName);
                if (window == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Not found the dialog window named \"{0}\".", viewName);

                    throw new NotFoundException(string.Format("Not found the dialog window named \"{0}\".", viewName));
                }

                if (window is AlertDialogWindow && viewModel is AlertDialogViewModel)
                    (window as AlertDialogWindow).ViewModel = viewModel as AlertDialogViewModel;
                else
                    window.SetDataContext(viewModel);

                EventHandler handler = null;
                handler = (sender, eventArgs) =>
                {
                    window.OnDismissed -= handler;
                    result.SetResult(viewModel);
                };
                window.Create();
                window.OnDismissed += handler;
                window.Show(true);
            }
            catch (Exception e)
            {
                result.SetException(e);
                if (window != null)
                    window.Dismiss();
            }
            return result;
        }

        public IAsyncResult<IViewModel> ShowDialog(string viewName, IViewModel viewModel)
        {
            return this.ShowDialog<IViewModel>(viewName, viewModel);
        }
    }
}
