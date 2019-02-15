using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

using Loxodon.Log;

namespace Loxodon.Framework.Localizations
{
    public class Locale
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Locale));

        private static readonly Dictionary<SystemLanguage, CultureInfo> languages = new Dictionary<SystemLanguage, CultureInfo>()
        {
            { SystemLanguage.Afrikaans, new CultureInfo("af") },
            { SystemLanguage.Arabic , new CultureInfo("ar") },
            { SystemLanguage.Basque , new CultureInfo("eu") },
            { SystemLanguage.Belarusian , new CultureInfo("be") },
            { SystemLanguage.Bulgarian , new CultureInfo("bg") },
            { SystemLanguage.Catalan , new CultureInfo("ca") },
            { SystemLanguage.Chinese , new CultureInfo("zh-CN") },
            { SystemLanguage.ChineseSimplified , new CultureInfo("zh-CN") },
            { SystemLanguage.ChineseTraditional , new CultureInfo("zh-TW") },
            { SystemLanguage.Czech , new CultureInfo("cs") },
            { SystemLanguage.Danish , new CultureInfo("da") },
            { SystemLanguage.Dutch , new CultureInfo("nl") },
            { SystemLanguage.English , new CultureInfo("en") },
            { SystemLanguage.Estonian , new CultureInfo("et") },
            { SystemLanguage.Faroese , new CultureInfo("fo") },
            { SystemLanguage.Finnish , new CultureInfo("fi") },
            { SystemLanguage.French , new CultureInfo("fr") },
            { SystemLanguage.German , new CultureInfo("de") },
            { SystemLanguage.Greek , new CultureInfo("el") },
            { SystemLanguage.Hebrew , new CultureInfo("he") },
            { SystemLanguage.Hungarian , new CultureInfo("hu") },
            { SystemLanguage.Icelandic , new CultureInfo("is") },
            { SystemLanguage.Indonesian , new CultureInfo("id") },
            { SystemLanguage.Italian , new CultureInfo("it") },
            { SystemLanguage.Japanese , new CultureInfo("ja") },
            { SystemLanguage.Korean , new CultureInfo("ko") },
            { SystemLanguage.Latvian , new CultureInfo("lv") },
            { SystemLanguage.Lithuanian , new CultureInfo("lt") },
            { SystemLanguage.Norwegian , new CultureInfo("no") },
            { SystemLanguage.Polish , new CultureInfo("pl") },
            { SystemLanguage.Portuguese , new CultureInfo("pt") },
            { SystemLanguage.Romanian , new CultureInfo("ro") },
            { SystemLanguage.Russian , new CultureInfo("ru") },
            { SystemLanguage.SerboCroatian , new CultureInfo("hr") },
            { SystemLanguage.Slovak , new CultureInfo("sk") },
            { SystemLanguage.Slovenian , new CultureInfo("sl") },
            { SystemLanguage.Spanish , new CultureInfo("es") },
            { SystemLanguage.Swedish , new CultureInfo("sv") },
            { SystemLanguage.Thai , new CultureInfo("th") },
            { SystemLanguage.Turkish , new CultureInfo("tr") },
            { SystemLanguage.Ukrainian , new CultureInfo("uk") },
            { SystemLanguage.Vietnamese , new CultureInfo("vi") }
        };

        public static CultureInfo GetCultureInfo()
        {
            return GetCultureInfoByLanguage(Application.systemLanguage, new CultureInfo("en"));
        }

        public static CultureInfo GetCultureInfoByLanguage(SystemLanguage language)
        {
            return GetCultureInfoByLanguage(language, new CultureInfo("en"));
        }

        public static CultureInfo GetCultureInfoByLanguage(SystemLanguage language, CultureInfo defaultValue)
        {
            if (language == SystemLanguage.Unknown)
            {
                if (log.IsWarnEnabled)
                    log.Warn("The system language of this application is Unknown");

                return defaultValue;
            }

            CultureInfo cultureInfo;
            if (languages.TryGetValue(language, out cultureInfo))
                return cultureInfo;

            if (log.IsWarnEnabled)
                log.Warn("The system language of this application cannot be found!");

            return defaultValue;
        }
    }
}
