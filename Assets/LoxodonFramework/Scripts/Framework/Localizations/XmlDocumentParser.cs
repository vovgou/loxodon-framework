using System.Collections.Generic;
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

        public override Dictionary<string, object> Parse(Stream input)
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
