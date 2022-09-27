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

using Loxodon.Framework.Localizations;
#if NEWTONSOFT
using Newtonsoft.Json;
#endif

namespace Loxodon.Framework.Examples.Domains
{
    public class EquipmentInfo
    {
#if NEWTONSOFT
        [JsonProperty("id")]
#endif
        public int Id { get; set; }

#if NEWTONSOFT
        [JsonProperty("sign")]
#endif
        public string Sign { get; protected set; }

#if NEWTONSOFT
        [JsonProperty("name")]
#endif
        public LocalizedString Name { get; set; }

#if NEWTONSOFT
        [JsonProperty("category")]
#endif
        public Category Category { get; set; }

#if NEWTONSOFT
        [JsonProperty("quality")]
#endif
        public int Quality { get; set; }

#if NEWTONSOFT
        [JsonProperty("health")]
#endif
        public float Health { get; set; }

#if NEWTONSOFT
        [JsonProperty("attackDamage")]
#endif
        public float AttackDamage { get; set; }

#if NEWTONSOFT
        [JsonProperty("abilityPower")]
#endif
        public float AbilityPower { get; set; }

#if NEWTONSOFT
        [JsonProperty("armor")]
#endif
        public float Armor { get; set; }

#if NEWTONSOFT
        [JsonProperty("magicResist")]
#endif
        public float MagicResist { get; set; }
    }
}
