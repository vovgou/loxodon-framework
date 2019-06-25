using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Services;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials
{
    public class DialogServiceExampleViewModel : ViewModelBase
    {
        private SimpleCommand openAlertDialog;
        private SimpleCommand openAlertDialog2;

        private IDialogService dialogService;

        public DialogServiceExampleViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;

            this.openAlertDialog = new SimpleCommand(() =>
            {
                this.openAlertDialog.Enabled = false;
                IAsyncResult<int> result = this.dialogService.ShowDialog("Dialog Service Example", "This is a dialog test.", "Yes", "No", null, true);
                result.Callbackable().OnCallback(r =>
                {
                    if (r.Result == AlertDialog.BUTTON_POSITIVE)
                    {
                        Debug.LogFormat("Click: Yes");
                    }
                    else if (r.Result == AlertDialog.BUTTON_NEGATIVE)
                    {
                        Debug.LogFormat("Click: No");
                    }
                    this.openAlertDialog.Enabled = true;
                });
            });

            this.openAlertDialog2 = new SimpleCommand(() =>
            {
                this.openAlertDialog2.Enabled = false;

                AlertDialogViewModel viewModel = new AlertDialogViewModel();
                viewModel.Title = "Dialog Service Example";
                viewModel.Message = "This is a dialog test.";
                viewModel.ConfirmButtonText = "OK";

                IAsyncResult<AlertDialogViewModel> result = this.dialogService.ShowDialog("UI/AlertDialog", viewModel);
                result.Callbackable().OnCallback(r =>
                {
                    AlertDialogViewModel vm = r.Result;
                    if (vm.Result == AlertDialog.BUTTON_POSITIVE)
                    {
                        Debug.LogFormat("Click: OK");
                    }
                    this.openAlertDialog2.Enabled = true;
                });
            });
        }

        public ICommand OpenAlertDialog { get { return this.openAlertDialog; } }
        public ICommand OpenAlertDialog2 { get { return this.openAlertDialog2; } }
    }

    public class DialogServiceExample : WindowView
    {
        public Button openAlert;
        public Button openAlert2;
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

            /* Initialize the dialog service */
            IDialogService dialogService = new DefaultDialogService();
            container.Register<IDialogService>(dialogService);
        }

        protected override void Start()
        {
            ApplicationContext context = Context.GetApplicationContext();
            IDialogService dialogService = context.GetService<IDialogService>();
            DialogServiceExampleViewModel viewModel = new DialogServiceExampleViewModel(dialogService);
            this.SetDataContext(viewModel);

            /* databinding */
            BindingSet<DialogServiceExample, DialogServiceExampleViewModel> bindingSet = this.CreateBindingSet<DialogServiceExample, DialogServiceExampleViewModel>();

            /* Binding command */
            bindingSet.Bind(this.openAlert).For(v => v.onClick).To(vm => vm.OpenAlertDialog);
            bindingSet.Bind(this.openAlert2).For(v => v.onClick).To(vm => vm.OpenAlertDialog2);

            bindingSet.Build();
        }
    }
}
