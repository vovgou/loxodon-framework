using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Globalization;

using Loxodon.Framework.Localizations;

namespace Loxodon.Framework.Tutorials
{
	public class LocalizationExample : MonoBehaviour
	{
		public Dropdown dropdown;

		private Localization localization;

		void Awake ()
		{
			CultureInfo cultureInfo = Locale.GetCultureInfoByLanguage (SystemLanguage.English);
			Localization.Current = Localization.Create (new DefaultDataProvider ("LocalizationTutorials", new XmlDocumentParser ()), cultureInfo);
			this.localization = Localization.Current;

			this.dropdown.onValueChanged.AddListener (OnValueChanged);
		}

		void OnValueChanged (int value)
		{
			switch (value) {
			case 0:
				this.localization.CultureInfo = Locale.GetCultureInfoByLanguage (SystemLanguage.English);
				break;
			case 1:
				this.localization.CultureInfo = Locale.GetCultureInfoByLanguage (SystemLanguage.ChineseSimplified);
				break;
			default:
				this.localization.CultureInfo = Locale.GetCultureInfoByLanguage (SystemLanguage.English);
				break;
			}
		}

		void OnDestroy ()
		{
			this.dropdown.onValueChanged.RemoveListener (OnValueChanged);
		}
	}
}