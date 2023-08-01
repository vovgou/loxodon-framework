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
            set { this.Set(ref this.username, value); }
        }

        public string Email
        {
            get { return this.email; }
            set { this.Set(ref this.email, value); }
        }

        public bool Remember
        {
            get { return this.remember; }
            set { this.Set(ref this.remember, value); }
        }

        public Vector3 Vector
        {
            get { return this.vector; }
            set { this.Set(ref this.vector, value); }
        }

        public Color Color
        {
            get { return this.color; }
            set { this.Set(ref this.color, value); }
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
            var localization = Localization.Current;
            localization.CultureInfo = cultureInfo;
            localization.AddDataProvider(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()));
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
            bindingSet.Bind(this.variables.Get<Button>("submit")).For(v => v.onClick).To(vm => vm.OnSubmit);
            bindingSet.Build();
        }

    }
}
