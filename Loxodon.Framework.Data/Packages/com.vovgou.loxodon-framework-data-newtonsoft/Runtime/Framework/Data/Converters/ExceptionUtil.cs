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
using System.Text;

namespace Loxodon.Framework.Data.Converters
{
    public static class ExceptionUtil
    {
        public static JsonSerializationException Create(JsonReader reader, string message)
        {
            return Create(reader, message, null);
        }

        public static JsonSerializationException Create(JsonReader reader, string message, Exception ex)
        {
            return Create(reader as IJsonLineInfo, reader.Path, message, ex);
        }

        public static JsonSerializationException Create(IJsonLineInfo lineInfo, string path, string message, Exception ex)
        {
            return new JsonSerializationException(FormatMessage(lineInfo, path, message), ex);
        }

        internal static string FormatMessage(IJsonLineInfo lineInfo, string path, string message)
        {
            StringBuilder buf = new StringBuilder();
            if (!message.EndsWith(Environment.NewLine, StringComparison.Ordinal))
            {
                buf.Append(message.Trim());
                if (!message.EndsWith("."))
                    buf.Append(".");
                buf.Append(" ");
            }

            buf.AppendFormat("Path '{0}'", path);
            if (lineInfo != null && lineInfo.HasLineInfo())
                buf.AppendFormat(", line {0}, position {1}", lineInfo.LineNumber, lineInfo.LinePosition);

            buf.Append(".");
            return buf.ToString();
        }
    }
}
