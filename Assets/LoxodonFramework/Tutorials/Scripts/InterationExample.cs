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

namespace Loxodon.Framework.Tutorials
{
    public class InterationViewModel : ViewModelBase
    {
        private InteractionRequest<DialogNotification> alertDialogRequest;
        private InteractionRequest<Notification> toastRequest;
        private InteractionRequest<VisibilityNotification> loadingRequest;

        private SimpleCommand openAlertDialog;
        private SimpleCommand showToast;
        private SimpleCommand showLoading;
        private SimpleCommand hideLoading;

        public InterationViewModel()
        {
            this.alertDialogRequest = new InteractionRequest<DialogNotification>(this);
            this.toastRequest = new InteractionRequest<Notification>(this);
            this.loadingRequest = new InteractionRequest<VisibilityNotification>();

            this.openAlertDialog = new SimpleCommand(() =>
            {
                this.openAlertDialog.Enabled = false;

                DialogNotification notification = new DialogNotification("Interation Example", "This is a dialog test.", "Yes", "No", true);

                Action<DialogNotification> callback = n =>
                {
                    this.openAlertDialog.Enabled = true;

                    if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
                    {
                        Debug.LogFormat("Click: Yes");
                    }
                    else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
                    {
                        Debug.LogFormat("Click: No");
                    }
                };

                this.alertDialogRequest.Raise(notification, callback);
            });

            this.showToast = new SimpleCommand(() =>
            {
                Notification notification = new Notification("This is a toast test.");
                this.toastRequest.Raise(notification);
            });

            this.showLoading = new SimpleCommand(() =>
            {
                VisibilityNotification notification = new VisibilityNotification(true);
                this.loadingRequest.Raise(notification);
            });

            this.hideLoading = new SimpleCommand(() =>
            {
                VisibilityNotification notification = new VisibilityNotification(false);
                this.loadingRequest.Raise(notification);
            });

        }

        public IInteractionRequest AlertDialogRequest { get { return this.alertDialogRequest; } }
        public IInteractionRequest ToastRequest { get { return this.toastRequest; } }
        public IInteractionRequest LoadingRequest { get { return this.loadingRequest; } }

        public ICommand OpenAlertDialog { get { return this.openAlertDialog; } }
        public ICommand ShowToast { get { return this.showToast; } }
        public ICommand ShowLoading { get { return this.showLoading; } }
        public ICommand HideLoading { get { return this.hideLoading; } }
    }

    public class InterationExample : WindowView
    {
        public Button openAlert;
        public Button showToast;
        public Button showLoading;
        public Button hideLoading;

        private List<Loading> list = new List<Loading>();

        protected override void Awake()
        {
            ApplicationContext context = Context.GetApplicationContext();
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();

            /* Initialize the ui view locator and register UIViewLocator */
            IServiceContainer container = context.GetContainer();
            container.Register<IUIViewLocator>(new DefaultUIViewLocator());

            CultureInfo cultureInfo = Locale.GetCultureInfo();
            Localization.Current = Localization.Create(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()), cultureInfo);
        }

        protected override void Start()
        {
            InterationViewModel viewModel = new InterationViewModel();
            this.SetDataContext(viewModel);

            /* databinding */
            BindingSet<InterationExample, InterationViewModel> bindingSet = this.CreateBindingSet<InterationExample, InterationViewModel>();

            /* Binding interaction request */
            bindingSet.Bind().For(v => this.OnOpenAlert(null, null)).To(vm => vm.AlertDialogRequest);
            bindingSet.Bind().For(v => this.OnShowToast(null, null)).To(vm => vm.ToastRequest);
            bindingSet.Bind().For(v => this.OnShowOrHideLoading(null, null)).To(vm => vm.LoadingRequest);

            /* Binding command */
            bindingSet.Bind(this.openAlert).For(v => v.onClick).To(vm => vm.OpenAlertDialog);
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

                  if (callback != null)
                      callback();
              });
        }

        private void OnShowToast(object sender, InteractionEventArgs args)
        {
            Notification notification = args.Context as Notification;
            if (notification == null)
                return;

            Toast.Show(this, notification.Message, 2f);
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
