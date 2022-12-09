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

namespace Loxodon.Framework.Prefs
{
    public class DefaultSerializer : ISerializer
    {
        private readonly object _lock = new object();
        private readonly static ComparerImpl<ITypeEncoder> comparer = new ComparerImpl<ITypeEncoder>();
        private List<ITypeEncoder> encoders = new List<ITypeEncoder>();

        public DefaultSerializer()
        {
            AddTypeEncoder(new PrimitiveTypeEncoder());
            AddTypeEncoder(new VersionTypeEncoder());
            AddTypeEncoder(new EnumTypeEncoder());
            AddTypeEncoder(new JsonTypeEncoder());
        }

        public virtual void AddTypeEncoder(ITypeEncoder encoder)
        {
            lock (_lock)
            {
                if (encoders.Contains(encoder))
                    return;

                encoders.Add(encoder);
                encoders.Sort(comparer);
            }
        }

        public virtual void RemoveTypeEncoder(ITypeEncoder encoder)
        {
            lock (_lock)
            {
                if (!encoders.Contains(encoder))
                    return;

                encoders.Remove(encoder);
            }
        }

        public virtual object Deserialize(string input, Type type)
        {
            lock (_lock)
            {
                for (int i = 0; i < encoders.Count; i++)
                {
                    try
                    {
                        ITypeEncoder encoder = encoders[i];
                        if (!encoder.IsSupport(type))
                            continue;

                        return encoder.Decode(type, input);
                    }
                    catch (Exception) { }
                }

            }
            throw new NotSupportedException(string.Format("This value \"{0}\" cannot be converted to the type \"{1}\"", input, type.Name));
        }

        public virtual string Serialize(object value)
        {
            lock (_lock)
            {
                for (int i = 0; i < encoders.Count; i++)
                {
                    try
                    {
                        ITypeEncoder encoder = encoders[i];
                        if (!encoder.IsSupport(value.GetType()))
                            continue;

                        return encoder.Encode(value);
                    }
                    catch (Exception) { }
                }
            }
            throw new NotSupportedException(string.Format("Unsupported type, this value \"{0}\" cannot be serialized", value));
        }

        class ComparerImpl<T> : IComparer<T> where T : ITypeEncoder
        {
            public int Compare(T x, T y)
            {
                return y.Priority.CompareTo(x.Priority);
            }
        }
    }
}
