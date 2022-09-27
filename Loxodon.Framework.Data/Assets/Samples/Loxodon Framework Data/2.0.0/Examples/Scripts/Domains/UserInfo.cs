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
using System.Collections.Generic;
#if NEWTONSOFT
using Newtonsoft.Json;
#endif

namespace Loxodon.Framework.Examples.Domains
{
    public class UserInfo
    {
#if NEWTONSOFT
        [JsonProperty("id")]
#endif
        public int Id { get; set; }

#if NEWTONSOFT
        [JsonProperty("username")]
#endif
        public string Username { get; set; }

#if NEWTONSOFT
        [JsonProperty("name")]
#endif
        public LocalizedString Name { get; set; }

#if NEWTONSOFT
        [JsonProperty("emails")]
#endif
        public List<string> Emails { get; set; }

#if NEWTONSOFT
        [JsonProperty("information")]
#endif
        public LocalizedString Information { get; set; }

#if NEWTONSOFT
        [JsonProperty("address")]
#endif
        public Address Address { get; set; }

#if NEWTONSOFT
        [JsonProperty("status")]
#endif
        public Status Status { get; set; }
    }

    public class Address
    {
#if NEWTONSOFT
        [JsonProperty("province")]
#endif
        public string Province { get; set; }

#if NEWTONSOFT
        [JsonProperty("street")]
#endif
        public string Street { get; set; }

#if NEWTONSOFT
        [JsonProperty("postcode")]
#endif
        public string Postcode { get; set; }
    }

    public enum Status
    {
        OK,
        LOCKED,
        DELETED
    }
}
