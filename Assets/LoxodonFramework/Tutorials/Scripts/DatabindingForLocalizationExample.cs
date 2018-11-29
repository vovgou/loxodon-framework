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
            bindingSet.Bind(this.dropdown).For(v => v.onValueChanged).To(vm => vm.OnValueChanged(0));
            bindingSet.Build();

            BindingSet<DatabindingForLocalizationExample> staticBindingSet = this.CreateBindingSet<DatabindingForLocalizationExample>();
            staticBindingSet.Bind(this.text).For(v => v.text).To(() => Res.localization_tutorials_content).OneWay();
            staticBindingSet.Build();
        }
    }
}