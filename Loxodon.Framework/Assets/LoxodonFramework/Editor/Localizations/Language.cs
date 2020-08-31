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
using System.Collections.Generic;
namespace Loxodon.Framework.Editors
{
    public class Language
    {
        private readonly static List<Language> languages = new List<Language>()
        {
            new Language("Afrikaans/Afrikaans [af]","Afrikaans","","af"),
            new Language("Afrikaans/South Africa [af-ZA]","Afrikaans","South Africa","af-ZA"),
            new Language("Albanian/Albania [sq-AL]","Albanian","Albania","sq-AL"),
            new Language("Albanian/Albanian [sq]","Albanian","","sq"),
            new Language("Arabic/Arabic [ar]","Arabic","","ar"),
            new Language("Arabic/Algeria [ar-DZ]","Arabic","Algeria","ar-DZ"),
            new Language("Arabic/Bahrain [ar-BH]","Arabic","Bahrain","ar-BH"),
            new Language("Arabic/Egypt [ar-EG]","Arabic","Egypt","ar-EG"),
            new Language("Arabic/Iraq [ar-IQ]","Arabic","Iraq","ar-IQ"),
            new Language("Arabic/Jordan [ar-JO]","Arabic","Jordan","ar-JO"),
            new Language("Arabic/Kuwait [ar-KW]","Arabic","Kuwait","ar-KW"),
            new Language("Arabic/Lebanon [ar-LB]","Arabic","Lebanon","ar-LB"),
            new Language("Arabic/Libya [ar-LY]","Arabic","Libya","ar-LY"),
            new Language("Arabic/Morocco [ar-MA]","Arabic","Morocco","ar-MA"),
            new Language("Arabic/Oman [ar-OM]","Arabic","Oman","ar-OM"),
            new Language("Arabic/Qatar [ar-QA]","Arabic","Qatar","ar-QA"),
            new Language("Arabic/Saudi Arabia [ar-SA]","Arabic","Saudi Arabia","ar-SA"),
            new Language("Arabic/Syria [ar-SY]","Arabic","Syria","ar-SY"),
            new Language("Arabic/Tunisia [ar-TN]","Arabic","Tunisia","ar-TN"),
            new Language("Arabic/United Arab Emirates [ar-AE]","Arabic","United Arab Emirates","ar-AE"),
            new Language("Arabic/Yemen [ar-YE]","Arabic","Yemen","ar-YE"),
            new Language("Armenian/Armenia [hy-AM]","Armenian","Armenia","hy-AM"),
            new Language("Armenian/Armenian [hy]","Armenian","","hy"),
            new Language("Basque/Basque [eu]","Basque","","eu"),
            new Language("Basque/Spain [eu-ES]","Basque","Spain","eu-ES"),
            new Language("Belarusian/Belarus [be-BY]","Belarusian","Belarus","be-BY"),
            new Language("Belarusian/Belarusian [be]","Belarusian","","be"),
            new Language("Bulgarian/Bulgaria [bg-BG]","Bulgarian","Bulgaria","bg-BG"),
            new Language("Bulgarian/Bulgarian [bg]","Bulgarian","","bg"),
            new Language("Catalan/Catalan [ca]","Catalan","","ca"),
            new Language("Catalan/Spain [ca-ES]","Catalan","Spain","ca-ES"),
            new Language("Chinese/Chinese [zh]","Chinese","","zh"),
            new Language("Chinese/China [zh-CN]","Chinese","China","zh-CN"),
            new Language("Chinese/Simplified [zh-CHS]","Chinese","Simplified","zh-CHS"),
            new Language("Chinese/Traditional [zh-CHT]","Chinese","Traditional","zh-CHT"),
            new Language("Chinese/Hong Kong S.A.R., China [zh-HK]","Chinese","Hong Kong S.A.R., China","zh-HK"),
            new Language("Chinese/Macao S.A.R. China [zh-MO]","Chinese","Macao S.A.R. China","zh-MO"),
            new Language("Chinese/Singapore [zh-SG]","Chinese","Singapore","zh-SG"),
            new Language("Chinese/Taiwan [zh-TW]","Chinese","Taiwan","zh-TW"),
            new Language("Croatian/Croatia [hr-HR]","Croatian","Croatia","hr-HR"),
            new Language("Croatian/Croatian [hr]","Croatian","","hr"),
            new Language("Czech/Czech [cs]","Czech","","cs"),
            new Language("Czech/Czech Republic [cs-CZ]","Czech","Czech Republic","cs-CZ"),
            new Language("Danish/Danish [da]","Danish","","da"),
            new Language("Danish/Denmark [da-DK]","Danish","Denmark","da-DK"),
            new Language("Dutch/Belgium [nl-BE]","Dutch","Belgium","nl-BE"),
            new Language("Dutch/Dutch [nl]","Dutch","","nl"),
            new Language("Dutch/Netherlands [nl-NL]","Dutch","Netherlands","nl-NL"),
            new Language("English/Australia [en-AU]","English","Australia","en-AU"),
            new Language("English/Canada [en-CA]","English","Canada","en-CA"),
            new Language("English/English [en]","English","","en"),
            new Language("English/Ireland [en-IE]","English","Ireland","en-IE"),
            new Language("English/New Zealand [en-NZ]","English","New Zealand","en-NZ"),
            new Language("English/Philippines [en-PH]","English","Philippines","en-PH"),
            new Language("English/South Africa [en-ZA]","English","South Africa","en-ZA"),
            new Language("English/Trinidad and Tobago [en-TT]","English","Trinidad and Tobago","en-TT"),
            new Language("English/United Kingdom [en-GB]","English","United Kingdom","en-GB"),
            new Language("English/United States [en-US]","English","United States","en-US"),
            new Language("English/Zimbabwe [en-ZW]","English","Zimbabwe","en-ZW"),
            new Language("Estonian/Estonia [et-EE]","Estonian","Estonia","et-EE"),
            new Language("Estonian/Estonian [et]","Estonian","","et"),
            new Language("Faroese/Faroe Islands [fo-FO]","Faroese","Faroe Islands","fo-FO"),
            new Language("Faroese/Faroese [fo]","Faroese","","fo"),
            new Language("Finnish/Finland [fi-FI]","Finnish","Finland","fi-FI"),
            new Language("Finnish/Finnish [fi]","Finnish","","fi"),
            new Language("French/Belgium [fr-BE]","French","Belgium","fr-BE"),
            new Language("French/Canada [fr-CA]","French","Canada","fr-CA"),
            new Language("French/France [fr-FR]","French","France","fr-FR"),
            new Language("French/French [fr]","French","","fr"),
            new Language("French/Luxembourg [fr-LU]","French","Luxembourg","fr-LU"),
            new Language("French/Switzerland [fr-CH]","French","Switzerland","fr-CH"),
            new Language("Gallegan/Gallegan [gl]","Gallegan","","gl"),
            new Language("Gallegan/Spain [gl-ES]","Gallegan","Spain","gl-ES"),
            new Language("Georgian/Georgia [ka-GE]","Georgian","Georgia","ka-GE"),
            new Language("Georgian/Georgian [ka]","Georgian","","ka"),
            new Language("German/Austria [de-AT]","German","Austria","de-AT"),
            new Language("German/German [de]","German","","de"),
            new Language("German/Germany [de-DE]","German","Germany","de-DE"),
            new Language("German/Luxembourg [de-LU]","German","Luxembourg","de-LU"),
            new Language("German/Switzerland [de-CH]","German","Switzerland","de-CH"),
            new Language("Greek/Greece [el-GR]","Greek","Greece","el-GR"),
            new Language("Greek/Greek [el]","Greek","","el"),
            new Language("Gujarati/Gujarati [gu]","Gujarati","","gu"),
            new Language("Gujarati/India [gu-IN]","Gujarati","India","gu-IN"),
            new Language("Hebrew/Hebrew [he]","Hebrew","","he"),
            new Language("Hebrew/Israel [he-IL]","Hebrew","Israel","he-IL"),
            new Language("Hindi/Hindi [hi]","Hindi","","hi"),
            new Language("Hindi/India [hi-IN]","Hindi","India","hi-IN"),
            new Language("Hungarian/Hungarian [hu]","Hungarian","","hu"),
            new Language("Hungarian/Hungary [hu-HU]","Hungarian","Hungary","hu-HU"),
            new Language("Icelandic/Iceland [is-IS]","Icelandic","Iceland","is-IS"),
            new Language("Icelandic/Icelandic [is]","Icelandic","","is"),
            new Language("Indonesian/Indonesia [id-ID]","Indonesian","Indonesia","id-ID"),
            new Language("Indonesian/Indonesian [id]","Indonesian","","id"),
            new Language("Italian/Italian [it]","Italian","","it"),
            new Language("Italian/Italy [it-IT]","Italian","Italy","it-IT"),
            new Language("Italian/Switzerland [it-CH]","Italian","Switzerland","it-CH"),
            new Language("Japanese/Japan [ja-JP]","Japanese","Japan","ja-JP"),
            new Language("Japanese/Japanese [ja]","Japanese","","ja"),
            new Language("Kannada/India [kn-IN]","Kannada","India","kn-IN"),
            new Language("Kannada/Kannada [kn]","Kannada","","kn"),
            new Language("Konkani/India [kok-IN]","Konkani","India","kok-IN"),
            new Language("Konkani/Konkani [kok]","Konkani","","kok"),
            new Language("Korean/Korean [ko]","Korean","","ko"),
            new Language("Korean/South Korea [ko-KR]","Korean","South Korea","ko-KR"),
            new Language("Latvian/Latvia [lv-LV]","Latvian","Latvia","lv-LV"),
            new Language("Latvian/Latvian [lv]","Latvian","","lv"),
            new Language("Lithuanian/Lithuania [lt-LT]","Lithuanian","Lithuania","lt-LT"),
            new Language("Lithuanian/Lithuanian [lt]","Lithuanian","","lt"),
            new Language("Macedonian/Macedonia [mk-MK]","Macedonian","Macedonia","mk-MK"),
            new Language("Macedonian/Macedonian [mk]","Macedonian","","mk"),
            new Language("Marathi/India [mr-IN]","Marathi","India","mr-IN"),
            new Language("Marathi/Marathi [mr]","Marathi","","mr"),
            new Language("Norwegian/Bokmål Norway [nb-NO]","Norwegian","Bokmål Norway","nb-NO"),
            new Language("Norwegian/Norwegian [no]","Norwegian","","no"),
            new Language("Norwegian/Nynorsk Norway [nn-NO]","Norwegian","Nynorsk Norway","nn-NO"),
            new Language("Persian/Iran [fa-IR]","Persian","Iran","fa-IR"),
            new Language("Persian/Persian [fa]","Persian","","fa"),
            new Language("Polish/Poland [pl-PL]","Polish","Poland","pl-PL"),
            new Language("Polish/Polish [pl]","Polish","","pl"),
            new Language("Portuguese/Brazil [pt-BR]","Portuguese","Brazil","pt-BR"),
            new Language("Portuguese/Portugal [pt-PT]","Portuguese","Portugal","pt-PT"),
            new Language("Portuguese/Portuguese [pt]","Portuguese","","pt"),
            new Language("Romanian/Romania [ro-RO]","Romanian","Romania","ro-RO"),
            new Language("Romanian/Romanian [ro]","Romanian","","ro"),
            new Language("Russian/Russia [ru-RU]","Russian","Russia","ru-RU"),
            new Language("Russian/Russian [ru]","Russian","","ru"),
            new Language("Slovak/Slovak [sk]","Slovak","","sk"),
            new Language("Slovak/Slovakia [sk-SK]","Slovak","Slovakia","sk-SK"),
            new Language("Slovenian/Slovenia [sl-SI]","Slovenian","Slovenia","sl-SI"),
            new Language("Slovenian/Slovenian [sl]","Slovenian","","sl"),
            new Language("Spanish/Argentina [es-AR]","Spanish","Argentina","es-AR"),
            new Language("Spanish/Bolivia [es-BO]","Spanish","Bolivia","es-BO"),
            new Language("Spanish/Chile [es-CL]","Spanish","Chile","es-CL"),
            new Language("Spanish/Colombia [es-CO]","Spanish","Colombia","es-CO"),
            new Language("Spanish/Costa Rica [es-CR]","Spanish","Costa Rica","es-CR"),
            new Language("Spanish/Dominican Republic [es-DO]","Spanish","Dominican Republic","es-DO"),
            new Language("Spanish/Ecuador [es-EC]","Spanish","Ecuador","es-EC"),
            new Language("Spanish/El Salvador [es-SV]","Spanish","El Salvador","es-SV"),
            new Language("Spanish/Guatemala [es-GT]","Spanish","Guatemala","es-GT"),
            new Language("Spanish/Honduras [es-HN]","Spanish","Honduras","es-HN"),
            new Language("Spanish/Mexico [es-MX]","Spanish","Mexico","es-MX"),
            new Language("Spanish/Nicaragua [es-NI]","Spanish","Nicaragua","es-NI"),
            new Language("Spanish/Panama [es-PA]","Spanish","Panama","es-PA"),
            new Language("Spanish/Paraguay [es-PY]","Spanish","Paraguay","es-PY"),
            new Language("Spanish/Peru [es-PE]","Spanish","Peru","es-PE"),
            new Language("Spanish/Puerto Rico [es-PR]","Spanish","Puerto Rico","es-PR"),
            new Language("Spanish/Spain [es-ES]","Spanish","Spain","es-ES"),
            new Language("Spanish/Spanish [es]","Spanish","","es"),
            new Language("Spanish/Uruguay [es-UY]","Spanish","Uruguay","es-UY"),
            new Language("Spanish/Venezuela [es-VE]","Spanish","Venezuela","es-VE"),
            new Language("Swahili/Kenya [sw-KE]","Swahili","Kenya","sw-KE"),
            new Language("Swahili/Swahili [sw]","Swahili","","sw"),
            new Language("Swedish/Finland [sv-FI]","Swedish","Finland","sv-FI"),
            new Language("Swedish/Sweden [sv-SE]","Swedish","Sweden","sv-SE"),
            new Language("Swedish/Swedish [sv]","Swedish","","sv"),
            new Language("Tamil/India [ta-IN]","Tamil","India","ta-IN"),
            new Language("Tamil/Tamil [ta]","Tamil","","ta"),
            new Language("Telugu/India [te-IN]","Telugu","India","te-IN"),
            new Language("Telugu/Telugu [te]","Telugu","","te"),
            new Language("Thai/Thai [th]","Thai","","th"),
            new Language("Thai/Thailand [th-TH]","Thai","Thailand","th-TH"),
            new Language("Turkish/Turkey [tr-TR]","Turkish","Turkey","tr-TR"),
            new Language("Turkish/Turkish [tr]","Turkish","","tr"),
            new Language("Ukrainian/Ukraine [uk-UA]","Ukrainian","Ukraine","uk-UA"),
            new Language("Ukrainian/Ukrainian [uk]","Ukrainian","","uk"),
            new Language("Vietnamese/Vietnam [vi-VN]","Vietnamese","Vietnam","vi-VN"),
            new Language("Vietnamese/Vietnamese [vi]","Vietnamese","","vi")
        };

        public static List<Language> GetAll(params string[] filters)
        {
            List<Language> list = new List<Language>();
            foreach (var language in languages)
            {
                if (Array.IndexOf(filters, language.Code) >= 0)
                    continue;
                list.Add(language);
            }
            return list;
        }

        public static Language GetLanguage(string code)
        {
            foreach (var language in languages)
            {
                if (language.Code == code)
                    return language;
            }
            return null;
        }

        public Language(string key, string name, string country, string code)
        {
            this.Key = key;
            this.Name = name;
            this.Country = country;
            this.Code = code;
        }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Country { get; set; }
    }
}
