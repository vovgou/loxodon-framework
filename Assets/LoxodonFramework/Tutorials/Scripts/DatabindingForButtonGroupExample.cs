using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials
{
    public class ButtonGroupViewModel : ViewModelBase
    {
        private string text;
        private readonly SimpleCommand<int> click;
        public ButtonGroupViewModel()
        {
            this.click = new SimpleCommand<int>(OnClick);
        }

        public string Text
        {
            get { return this.text; }
            set { this.Set<string>(ref text, value, "Text"); }
        }

        public ICommand Click
        {
            get { return this.click; }
        }

        public void OnClick(int buttonNo)
        {
            Executors.RunOnCoroutineNoReturn(DoClick(buttonNo));
        }

        private IEnumerator DoClick(int buttonNo)
        {
            this.click.Enabled = false;
            this.Text = string.Format("Click Button:{0}.Restore button status after one second", buttonNo);
            Debug.LogFormat("Click Button:{0}", buttonNo);

            //Restore button status after one second
            yield return new WaitForSeconds(1f);
            this.click.Enabled = true;
        }

    }

    public class DatabindingForButtonGroupExample : UIView
    {
        public Button button1;
        public Button button2;
        public Button button3;
        public Button button4;
        public Button button5;
        public Text text;

        protected override void Awake()
        {
            ApplicationContext context = Context.GetApplicationContext();
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();


            CultureInfo cultureInfo = Locale.GetCultureInfo();
            Localization.Current = Localization.Create(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()), cultureInfo);
        }

        protected override void Start()
        {
            ButtonGroupViewModel viewModel = new ButtonGroupViewModel();

            IBindingContext bindingContext = this.BindingContext();
            bindingContext.DataContext = viewModel;

            /* databinding */
            BindingSet<DatabindingForButtonGroupExample, ButtonGroupViewModel> bindingSet = this.CreateBindingSet<DatabindingForButtonGroupExample, ButtonGroupViewModel>();
            bindingSet.Bind(this.button1).For(v => v.onClick).To(vm => vm.Click).CommandParameter(1);
            bindingSet.Bind(this.button2).For(v => v.onClick).To(vm => vm.Click).CommandParameter(2);
            bindingSet.Bind(this.button3).For(v => v.onClick).To(vm => vm.Click).CommandParameter(3);
            bindingSet.Bind(this.button4).For(v => v.onClick).To(vm => vm.Click).CommandParameter(4);
            bindingSet.Bind(this.button5).For(v => v.onClick).To(vm => vm.Click).CommandParameter(5);

            bindingSet.Bind(this.text).For(v => v.text).To(vm => vm.Text).OneWay();

            bindingSet.Build();
        }
    }
}
