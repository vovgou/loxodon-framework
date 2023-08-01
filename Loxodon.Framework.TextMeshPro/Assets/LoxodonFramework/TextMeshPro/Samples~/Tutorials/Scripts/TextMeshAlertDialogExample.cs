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

using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Services;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Loxodon.Framework.Views.InteractionActions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials
{
    public class TextMeshAlertDialogViewModel : ViewModelBase
    {
        public readonly InteractionRequest<DialogNotification> AlertDialogRequest = new InteractionRequest<DialogNotification>();
        public readonly AsyncInteractionRequest<DialogNotification> AsyncAlertDialogRequest = new AsyncInteractionRequest<DialogNotification>();
        public readonly InteractionRequest<ToastNotification> ToastRequest = new InteractionRequest<ToastNotification>();

        public TextMeshAlertDialogViewModel()
        {
            this.OpenAlertDialog = new SimpleCommand(() =>
            {
                this.OpenAlertDialog.Enabled = false;

                DialogNotification notification = new DialogNotification("TextMeshPro AlertDialog Example", "This is a dialog test.", "Yes", "No", true);

                Action<DialogNotification> callback = n =>
                {
                    this.OpenAlertDialog.Enabled = true;

                    if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
                    {
                        Debug.LogFormat("Click: Yes");
                    }
                    else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
                    {
                        Debug.LogFormat("Click: No");
                    }
                };

                this.AlertDialogRequest.Raise(notification, callback);
            });

            this.AsyncOpenAlertDialog = new SimpleCommand(async () =>
            {
                this.AsyncOpenAlertDialog.Enabled = false;
                DialogNotification notification = new DialogNotification("TextMeshPro AlertDialog Example", "This is a dialog test.", "Yes", "No", true);
                await this.AsyncAlertDialogRequest.Raise(notification);
                this.AsyncOpenAlertDialog.Enabled = true;
                if (notification.DialogResult == AlertDialog.BUTTON_POSITIVE)
                {
                    Debug.LogFormat("Click: Yes");
                }
                else if (notification.DialogResult == AlertDialog.BUTTON_NEGATIVE)
                {
                    Debug.LogFormat("Click: No");
                }
            });

            this.ShowToast = new SimpleCommand(() =>
            {
                ToastNotification notification = new ToastNotification("This is a toast test.", 2f);
                this.ToastRequest.Raise(notification);
            });
        }

        public SimpleCommand OpenAlertDialog { get; }
        public SimpleCommand AsyncOpenAlertDialog { get; }
        public SimpleCommand ShowToast { get; }
    }

    public class TextMeshAlertDialogExample : WindowView
    {
        public Button openAlert;
        public Button asyncOpenAlert;
        public Button showToast;

        private ToastInteractionAction toastInteractionAction;
        private AsyncDialogInteractionAction dialogInteractionAction;

        protected override void Awake()
        {
            ApplicationContext context = Context.GetApplicationContext();
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();

            /* Initialize the ui view locator and register UIViewLocator */
            IServiceContainer container = context.GetContainer();
            container.Register<IUIViewLocator>(new DefaultUIViewLocator());

            AlertDialog.ViewName = "UI/TextMeshAlertDialog";
            Toast.ViewName = "UI/TextMeshToast";
        }

        protected override void Start()
        {
            this.toastInteractionAction = new ToastInteractionAction(this);
            this.dialogInteractionAction = new AsyncDialogInteractionAction("UI/TextMeshAlertDialog");

            TextMeshAlertDialogViewModel viewModel = new TextMeshAlertDialogViewModel();
            this.SetDataContext(viewModel);

            /* databinding */
            BindingSet<TextMeshAlertDialogExample, TextMeshAlertDialogViewModel> bindingSet = this.CreateBindingSet<TextMeshAlertDialogExample, TextMeshAlertDialogViewModel>();

            /* Bind the method "OnOpenAlert" to an interactive request */
            bindingSet.Bind().For(v => v.OnOpenAlert).To(vm => vm.AlertDialogRequest);

            /* Bind the DialogInteractionAction to an interactive request */
            bindingSet.Bind().For(v => v.dialogInteractionAction).To(vm => vm.AsyncAlertDialogRequest);

            bindingSet.Bind().For(v => v.toastInteractionAction).To(vm => vm.ToastRequest);

            /* Binding command */
            bindingSet.Bind(this.openAlert).For(v => v.onClick).To(vm => vm.OpenAlertDialog);
            bindingSet.Bind(this.asyncOpenAlert).For(v => v.onClick).To(vm => vm.AsyncOpenAlertDialog);
            bindingSet.Bind(this.showToast).For(v => v.onClick).To(vm => vm.ShowToast);

            bindingSet.Build();
        }

        private void OnOpenAlert(object sender, InteractionEventArgs args)
        {
            DialogNotification notification = args.Context as DialogNotification;
            var callback = args.Callback;

            if (notification == null)
                return;

            AlertDialog.ShowMessage(notification.Message, notification.Title, notification.ConfirmButtonText, null, notification.CancelButtonText, notification.CanceledOnTouchOutside, (result) =>
            {
                notification.DialogResult = result;
                callback?.Invoke();
            });
        }
    }
}
