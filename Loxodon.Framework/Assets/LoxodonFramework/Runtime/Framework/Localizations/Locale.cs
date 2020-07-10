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

using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

using Loxodon.Log;

namespace Loxodon.Framework.Localizations
{
    public class Locale
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Locale));
        private static readonly CultureInfo DEFAULT_CULTUREINFO = new CultureInfo("en");

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
#if UNITY_2018_1_OR_NEWER
            return CultureInfo.CurrentUICulture;
#else
            return GetCultureInfoByLanguage(Application.systemLanguage, DEFAULT_CULTUREINFO);
#endif
        }

        public static CultureInfo GetCultureInfoByLanguage(SystemLanguage language)
        {
            return GetCultureInfoByLanguage(language, DEFAULT_CULTUREINFO);
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
