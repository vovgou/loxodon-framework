using System;
using System.Collections.Generic;
using System.IO;

namespace Loxodon.Framework.Localizations
{
    public abstract class AbstractDocumentParser : IDocumentParser
    {
        private List<ITypeConverter> converters = new List<ITypeConverter>();

        public AbstractDocumentParser() : this(null)
        {
        }

        public AbstractDocumentParser(List<ITypeConverter> converters)
        {
            if (converters != null)
                this.converters.AddRange(converters);
            this.converters.Add(new ColorTypeConverter());
            this.converters.Add(new VectorTypeConverter());
            this.converters.Add(new PrimitiveTypeConverter());
        }

        public abstract Dictionary<string, object> Parse(Stream input);

        protected virtual object Parse(string typeName, string value)
        {
            foreach (ITypeConverter converter in this.converters)
            {
                if (!converter.Support(typeName))
                    continue;

                Type type = converter.GetType(typeName);
                return converter.Convert(type, value);
            }

            throw new NotSupportedException(string.Format("The '{0}' is not supported.", typeName));
        }

        protected virtual object Parse(string typeName, List<string> values)
        {
            foreach (ITypeConverter converter in this.converters)
            {
                if (!converter.Support(typeName))
                    continue;

                Type type = converter.GetType(typeName);
                Array array = Array.CreateInstance(type, values.Count);
                for (int i = 0; i < values.Count; i++)
                {
                    object value = converter.Convert(type, values[i]);
                    array.SetValue(value, i);
                }
                return array;
            }

            throw new NotSupportedException(string.Format("The '{0}' is not supported.", typeName));
        }
    }
}
