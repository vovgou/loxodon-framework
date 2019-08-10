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

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Globalization;

using Loxodon.Framework.Localizations;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.ViewModels;

namespace Loxodon.Framework.Tutorials
{

    public class DatabindingForLocalizationViewModel : ViewModelBase
    {
        private Localization localization;

        public DatabindingForLocalizationViewModel(Localization localization)
        {
            this.localization = localization;
        }

        public void OnValueChanged(int value)
        {
            switch (value)
            {
                case 0:
                    this.localization.CultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.English);
                    break;
                case 1:
                    this.localization.CultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.ChineseSimplified);
                    break;
                default:
                    this.localization.CultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.English);
                    break;
            }
        }
    }

    public class DatabindingForLocalizationExample : MonoBehaviour
    {
        public Dropdown dropdown;

        public Text text;

        private Localization localization;

        void Awake()
        {
            ApplicationContext context = Context.GetApplicationContext();
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();

            CultureInfo cultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.English);
            Localization.Current = Localization.Create(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()), cultureInfo);
            this.localization = Localization.Current;
        }

        void Start()
        {
            BindingSet<DatabindingForLocalizationExample, DatabindingForLocalizationViewModel> bindingSet = this.CreateBindingSet<DatabindingForLocalizationExample, DatabindingForLocalizationViewModel>(new DatabindingForLocalizationViewModel(this.localization));
            bindingSet.Bind(this.dropdown).For(v => v.onValueChanged).To<int>(vm => vm.OnValueChanged);
            bindingSet.Build();

            BindingSet<DatabindingForLocalizationExample> staticBindingSet = this.CreateBindingSet<DatabindingForLocalizationExample>();
            staticBindingSet.Bind(this.text).For(v => v.text).To(() => Res.localization_tutorials_content).OneWay();
            staticBindingSet.Build();
        }
    }
}