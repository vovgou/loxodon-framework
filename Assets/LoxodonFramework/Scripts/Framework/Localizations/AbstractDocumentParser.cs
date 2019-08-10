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
using System.Globalization;
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
            this.converters.Add(new RectTypeConverter());
            this.converters.Add(new PrimitiveTypeConverter());            
        }

        public abstract Dictionary<string, object> Parse(Stream input, CultureInfo cultureInfo);

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

        protected virtual object Parse(string typeName, IList<string> values)
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
