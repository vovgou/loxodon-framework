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

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Loxodon.Framework.Localizations
{
    /// <summary>
    /// XML document parser
    /// </summary>
    public class XmlDocumentParser : AbstractDocumentParser
    {
        public XmlDocumentParser() : this(null)
        {
        }

        public XmlDocumentParser(List<ITypeConverter> converters) : base(converters)
        {            
        }

        public override Dictionary<string, object> Parse(Stream input, CultureInfo cultureInfo)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            using (XmlTextReader reader = new XmlTextReader(input))
            {
                string elementName = null;
                string typeName = null;
                string name = null;
                string value = null;
                List<string> list = new List<string>();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            elementName = reader.Name;
                            if (string.IsNullOrEmpty(elementName) || elementName.Equals("resources"))
                                break;

                            if (elementName.Equals("item"))
                            {
                                //array item
                                var content = reader.ReadElementString();
                                list.Add(content);
                                break;
                            }

                            name = reader.GetAttribute("name");
                            if (string.IsNullOrEmpty(name))
                                throw new XmlException("The attribute of name is null.");

                            if (elementName.EndsWith("-array"))
                            {
                                //array item
                                typeName = elementName.Replace("-array", "");
                                list.Clear();
                                break;
                            }

                            typeName = elementName;
                            value = reader.ReadElementString();
                            data[name] = this.Parse(typeName, value);
                            break;
                        case XmlNodeType.EndElement:
                            elementName = reader.Name;
                            if (!string.IsNullOrEmpty(elementName) && elementName.EndsWith("-array"))
                            {
                                //array
                                data[name] = this.Parse(typeName, list);
                                list.Clear();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return data;
        }
    }
}
