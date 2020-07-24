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

using CsvHelper;
using Loxodon.Framework.Utilities;
using Loxodon.Log;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Loxodon.Framework.Localizations
{
    public class CsvDocumentParser : AbstractDocumentParser
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CsvDocumentParser));

        public CsvDocumentParser() : this(null)
        {
        }

        public CsvDocumentParser(List<ITypeConverter> converters) : base(converters)
        {
        }

        public override Dictionary<string, object> Parse(Stream input, CultureInfo cultureInfo)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            using (CsvReader reader = new CsvReader(new StreamReader(input, Encoding.UTF8)))
            {
                if (!reader.Read() || !reader.ReadHeader())
                    return data;

                string[] headers = reader.Context.HeaderRecord;

                string defaultName = "default";
                string cultureISOName = cultureInfo.TwoLetterISOLanguageName;//eg:zh  en
                string cultureName = cultureInfo.Name;//eg:zh-CN  en-US              

                if (Array.IndexOf(headers, "default") < 0)
                {
                    /* If the default column is not configured, the first data column is used as the default column */
                    foreach (string header in headers)
                    {
                        if (Regex.IsMatch(header, @"^((key)|(type)|(description))$", RegexOptions.IgnoreCase))
                            continue;

                        defaultName = header;
                    }
                }

                string key = null;
                string typeName = null;
                string value = null;
                List<string> list = new List<string>();

                while (reader.Read())
                {
                    key = reader.GetField("key");
                    typeName = reader.GetField("type");

                    if (string.IsNullOrEmpty(key))
                        continue;

                    if (reader.TryGetField<string>(cultureName, out value) && !string.IsNullOrEmpty(value))
                    {
                        data[key] = this.ParseValue(typeName, value);
                        continue;
                    }

                    if (reader.TryGetField<string>(cultureISOName, out value) && !string.IsNullOrEmpty(value))
                    {
                        data[key] = this.ParseValue(typeName, value);
                        continue;
                    }

                    if (reader.TryGetField<string>(defaultName, out value) && !string.IsNullOrEmpty(value))
                    {
                        data[key] = this.ParseValue(typeName, value);
                        continue;
                    }

                    if (log.IsWarnEnabled)
                        log.WarnFormat("Not found the value when the language is {0} and the key is {1}.", cultureName, key);

                    data[key] = "";
                }
            }
            return data;
        }

        protected virtual object ParseValue(string typeName, string value)
        {
            if (typeName.EndsWith("-array", StringComparison.OrdinalIgnoreCase))
            {
                string[] array = StringSpliter.Split(value, ',');
                return this.Parse(typeName.Replace("-array", ""), array);
            }
            else
            {
                return this.Parse(typeName, value);
            }
        }
    }
}
