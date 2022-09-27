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
using UnityEngine;

namespace Loxodon.Framework.Data.Converters
{
    public class ColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Color).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default(Color);

            if (reader.TokenType != JsonToken.String)
                throw ExceptionUtil.Create(reader, "Unexpected token or value when parsing color.");

            try
            {
                var value = (string)reader.Value;
                if (string.IsNullOrWhiteSpace(value))
                    return default(Color);

                Color color;
                if (ColorUtility.TryParseHtmlString(value.Trim(), out color))
                    return color;

                throw ExceptionUtil.Create(reader, string.Format("Error parsing color string: {0}", reader.Value));
            }
            catch (Exception ex)
            {
                throw ExceptionUtil.Create(reader, string.Format("Error parsing color string: {0}", reader.Value), ex);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else if (value is Color)
            {
                writer.WriteValue(ColorUtility.ToHtmlStringRGBA((Color)value));
            }
            else
            {
                throw new JsonSerializationException(string.Format("Unsupported types:{0}", value));
            }
        }
    }
}
