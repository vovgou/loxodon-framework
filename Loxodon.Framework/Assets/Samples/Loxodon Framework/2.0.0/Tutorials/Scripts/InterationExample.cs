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

using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;

using Loxodon.Framework.Localizations;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Services;
using Loxodon.Framework.Views.InteractionActions;

namespace Loxodon.Framework.Tutorials
{
    public class InterationViewModel : ViewModelBase
    {
        public readonly InteractionRequest<DialogNotification> AlertDialogRequest = new InteractionRequest<DialogNotification>();
        public readonly AsyncInteractionRequest<DialogNotification> AsyncAlertDialogRequest = new AsyncInteractionRequest<DialogNotification>();
        public readonly InteractionRequest<ToastNotification> ToastRequest = new InteractionRequest<ToastNotification>();
        public readonly InteractionRequest<VisibilityNotification> LoadingRequest = new InteractionRequest<VisibilityNotification>();

        public InterationViewModel()
        {
            this.OpenAlertDialog = new SimpleCommand(() =>
            {
                this.OpenAlertDialog.Enabled = false;

                DialogNotification notification = new DialogNotification("Interation Example", "This is a dialog test.", "Yes", "No", true);

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
                DialogNotification notification = new DialogNotification("Interation Example", "This is a dialog test.", "Yes", "No", true);
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

            this.ShowLoading = new SimpleCommand(() =>
            {
                VisibilityNotification notification = new VisibilityNotification(true);
                this.LoadingRequest.Raise(notification);
            });

            this.HideLoading = new SimpleCommand(() =>
            {
                VisibilityNotification notification = new VisibilityNotification(false);
                this.LoadingRequest.Raise(notification);
            });

        }

        public SimpleCommand OpenAlertDialog { get; }
        public SimpleCommand AsyncOpenAlertDialog { get; }
        public SimpleCommand ShowToast { get; }
        public SimpleCommand ShowLoading { get; }
        public SimpleCommand HideLoading { get; }
    }

    public class InterationExample : WindowView
    {
        public Button openAlert;
        public Button asyncOpenAlert;
        public Button showToast;
        public Button showLoading;
        public Button hideLoading;

        private List<Loading> list = new List<Loading>();

        private LoadingInteractionAction loadingInteractionAction;
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

            CultureInfo cultureInfo = Locale.GetCultureInfo();
            var localization = Localization.Current;
            localization.CultureInfo = cultureInfo;
            localization.AddDataProvider(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()));
            container.Register(localization);
        }

        protected override void Start()
        {
            this.loadingInteractionAction = new LoadingInteractionAction();
            this.toastInteractionAction = new ToastInteractionAction(this);
            this.dialogInteractionAction = new AsyncDialogInteractionAction("UI/AlertDialog");

            InterationViewModel viewModel = new InterationViewModel();
            this.SetDataContext(viewModel);

            /* databinding */
            BindingSet<InterationExample, InterationViewModel> bindingSet = this.CreateBindingSet<InterationExample, InterationViewModel>();

            /* Bind the method "OnOpenAlert" to an interactive request */
            bindingSet.Bind().For(v => v.OnOpenAlert).To(vm => vm.AlertDialogRequest);

            /* Bind the DialogInteractionAction to an interactive request */
            bindingSet.Bind().For(v => v.dialogInteractionAction).To(vm => vm.AsyncAlertDialogRequest);

            /* Bind the ToastInteractionAction to an interactive request */
            bindingSet.Bind().For(v => v.toastInteractionAction).To(vm => vm.ToastRequest);
            /* or bind the method "OnShowToast" to an interactive request */
            //bindingSet.Bind().For(v => v.OnShowToast).To(vm => vm.ToastRequest);

            /* Bind the LoadingInteractionAction to an interactive request */
            bindingSet.Bind().For(v => v.loadingInteractionAction).To(vm => vm.LoadingRequest);
            /* or bind the method "OnShowOrHideLoading" to an interactive request */
            //bindingSet.Bind().For(v => v.OnShowOrHideLoading).To(vm => vm.LoadingRequest);

            /* Binding command */
            bindingSet.Bind(this.openAlert).For(v => v.onClick).To(vm => vm.OpenAlertDialog);
            bindingSet.Bind(this.asyncOpenAlert).For(v => v.onClick).To(vm => vm.AsyncOpenAlertDialog);
            bindingSet.Bind(this.showToast).For(v => v.onClick).To(vm => vm.ShowToast);
            bindingSet.Bind(this.showLoading).For(v => v.onClick).To(vm => vm.ShowLoading);
            bindingSet.Bind(this.hideLoading).For(v => v.onClick).To(vm => vm.HideLoading);

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

        private void OnShowToast(object sender, InteractionEventArgs args)
        {
            ToastNotification notification = args.Context as ToastNotification;
            if (notification == null)
                return;

            Toast.Show(this, notification.Message, notification.Duration);
        }

        private void OnShowOrHideLoading(object sender, InteractionEventArgs args)
        {
            VisibilityNotification notification = args.Context as VisibilityNotification;
            if (notification == null)
                return;

            if (notification.Visible)
            {
                this.list.Add(Loading.Show());
            }
            else
            {
                if (this.list.Count <= 0)
                    return;

                Loading loading = this.list[0];
                loading.Dispose();
                this.list.RemoveAt(0);
            }
        }
    }
}
