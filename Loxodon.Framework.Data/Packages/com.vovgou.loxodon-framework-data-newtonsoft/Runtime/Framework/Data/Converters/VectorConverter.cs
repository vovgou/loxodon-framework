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

using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Loxodon.Framework.Data.Converters
{
    public class VectorConverter : JsonConverter
    {
        private static readonly char[] COMMA_SEPARATOR = new char[] { ',' };
        private static readonly string PATTERN = @"(^\()|(\)$)";
        public override bool CanConvert(Type objectType)
        {
            return typeof(Vector2).Equals(objectType) || typeof(Vector3).Equals(objectType) || typeof(Vector4).Equals(objectType) || typeof(Vector2Int).Equals(objectType) || typeof(Vector3Int).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string value;
            if (reader.TokenType == JsonToken.Null)
                value = null;
            else if (reader.TokenType == JsonToken.String)
                value = Regex.Replace(((string)reader.Value).Trim(), PATTERN, "");
            else
                throw ExceptionUtil.Create(reader, string.Format("Unexpected token or value when parsing {0}.", objectType.Name));

            try
            {
                if (typeof(Vector2).Equals(objectType))
                {
                    if (string.IsNullOrWhiteSpace(value))
                        return default(Vector2);

                    string[] s = value.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 2)
                        return new Vector2(float.Parse(s[0]), float.Parse(s[1]));
                }
                else if (typeof(Vector3).Equals(objectType))
                {
                    if (string.IsNullOrWhiteSpace(value))
                        return default(Vector3);

                    string[] s = value.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 3)
                        return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
                }
                else if (typeof(Vector4).Equals(objectType))
                {
                    if (string.IsNullOrWhiteSpace(value))
                        return default(Vector4);

                    string[] s = value.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 4)
                        return new Vector4(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3]));
                }
                else if (typeof(Vector2Int).Equals(objectType))
                {
                    if (string.IsNullOrWhiteSpace(value))
                        return default(Vector2Int);

                    string[] s = value.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 2)
                        return new Vector2Int(int.Parse(s[0]), int.Parse(s[1]));
                }
                else if (typeof(Vector3Int).Equals(objectType))
                {
                    if (string.IsNullOrWhiteSpace(value))
                        return default(Vector3Int);

                    string[] s = value.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 3)
                        return new Vector3Int(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]));
                }

                throw ExceptionUtil.Create(reader, string.Format("Error parsing {0} string: {1}", objectType.Name, reader.Value));
            }
            catch (Exception ex)
            {
                throw ExceptionUtil.Create(reader, string.Format("Error parsing {0} string: {1}", objectType.Name, reader.Value), ex);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else if (value is Vector2 v2)
            {
                writer.WriteValue(v2.ToString());
            }
            else if (value is Vector3 v3)
            {
                writer.WriteValue(v3.ToString());
            }
            else if (value is Vector4 v4)
            {
                writer.WriteValue(v4.ToString());
            }
            else if (value is Vector2Int v2int)
            {
                writer.WriteValue(v2int.ToString());
            }
            else if (value is Vector3Int v3int)
            {
                writer.WriteValue(v3int.ToString());
            }
            else
            {
                throw new JsonSerializationException(string.Format("Unsupported types:{0}", value));
            }
        }
    }
}
