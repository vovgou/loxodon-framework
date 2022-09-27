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
using UnityEngine;
using System.Text.RegularExpressions;

namespace Loxodon.Framework.Localizations
{
    public class VectorTypeConverter : ITypeConverter
    {
        private static readonly char[] COMMA_SEPARATOR = new char[] { ',' };
        private static readonly string PATTERN = @"(^\()|(\)$)";
        public bool Support(string typeName)
        {
            switch (typeName)
            {
                case "vector2":
                case "vector3":
                case "vector4":
                    return true;
                default:
                    return false;
            }
        }

        public Type GetType(string typeName)
        {
            switch (typeName)
            {
                case "vector2":
                    return typeof(Vector2);
                case "vector3":
                    return typeof(Vector3);
                case "vector4":
                    return typeof(Vector4);
                default:
                    throw new NotSupportedException();
            }
        }

        public object Convert(Type type, object value)
        {
            if (type == null)
                throw new NotSupportedException();

            var val = Regex.Replace(((string)value).Trim(), PATTERN, "");
            if (type.Equals(typeof(Vector2)))
            {
                try
                {
                    string[] s = val.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 2)
                        return new Vector2(float.Parse(s[0]), float.Parse(s[1]));
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format("The '{0}' is illegal Vector2.", value), e);
                }
                throw new FormatException(string.Format("The '{0}' is illegal Vector2.", value));
            }

            if (type.Equals(typeof(Vector3)))
            {
                try
                {
                    string[] s = val.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 3)
                        return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format("The '{0}' is illegal Vector3.", value), e);
                }
                throw new FormatException(string.Format("The '{0}' is illegal Vector3.", value));
            }

            if (type.Equals(typeof(Vector4)))
            {
                try
                {
                    string[] s = val.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 4)
                        return new Vector4(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3]));
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format("The '{0}' is illegal Vector4.", value), e);
                }
                throw new FormatException(string.Format("The '{0}' is illegal Vector4.", value));
            }

            throw new NotSupportedException();
        }
    }
}
