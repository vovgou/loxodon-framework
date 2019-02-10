using System;
using System.Globalization;

using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;

using Loxodon.Framework.Localizations;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Views.Variables;
using UnityEngine.UI;
using Loxodon.Framework.ViewModels;
using UnityEngine;

namespace Loxodon.Framework.Tutorials
{
    public class VariableViewModel : ViewModelBase
    {
        private bool remember;
        private string username;
        private string email;
        private Color color;
        private Vector3 vector;

        public string Username
        {
            get { return this.username; }
            set { this.Set<string>(ref this.username, value, "Username"); }
        }

        public string Email
        {
            get { return this.email; }
            set { this.Set<string>(ref this.email, value, "Email"); }
        }

        public bool Remember
        {
            get { return this.remember; }
            set { this.Set<bool>(ref this.remember, value, "Remember"); }
        }

        public Vector3 Vector
        {
            get { return this.vector; }
            set { this.Set<Vector3>(ref this.vector, value, "Vector"); }
        }

        public Color Color
        {
            get { return this.color; }
            set { this.Set<Color>(ref this.color, value, "Color"); }
        }

        public void OnSubmit()
        {
            Debug.LogFormat("username:{0} email:{1} remember:{2} vector:{3} color:{4}", this.username, this.email, this.remember, this.vector, this.color);
        }
    }

    public class VariableExample : UIView
    {
        public VariableArray variables;

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
            VariableViewModel viewModel = new VariableViewModel()
            {
                Username = "test",
                Email = "yangpc.china@gmail.com",
                Remember = true
            };

            viewModel.Color = this.variables.Get<Color>("color");
            viewModel.Vector = this.variables.Get<Vector3>("vector");

            IBindingContext bindingContext = this.BindingContext();
            bindingContext.DataContext = viewModel;

            /* databinding */
            BindingSet<VariableExample, VariableViewModel> bindingSet = this.CreateBindingSet<VariableExample, VariableViewModel>();
            bindingSet.Bind(this.variables.Get<InputField>("username")).For(v => v.text, v => v.onEndEdit).To(vm => vm.Username).TwoWay();
            bindingSet.Bind(this.variables.Get<InputField>("email")).For(v => v.text, v => v.onEndEdit).To(vm => vm.Email).TwoWay();
            bindingSet.Bind(this.variables.Get<Toggle>("remember")).For(v => v.isOn, v => v.onValueChanged).To(vm => vm.Remember).TwoWay();
            bindingSet.Bind(this.variables.Get<Button>("submit")).For(v => v.onClick).To(vm => vm.OnSubmit());
            bindingSet.Build();
        }

    }
}
