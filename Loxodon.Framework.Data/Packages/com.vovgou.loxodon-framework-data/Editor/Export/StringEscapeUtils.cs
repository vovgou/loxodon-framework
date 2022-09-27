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

using System.Collections.Concurrent;
using System.Text;

namespace Loxodon.Framework.Data.Editors
{
    public static class StringEscapeUtils
    {
        static ConcurrentDictionary<char, string> ESCAPE_CHARS = new ConcurrentDictionary<char, string>();
        static StringEscapeUtils()
        {
            ESCAPE_CHARS.TryAdd('\b', "\\b");
            ESCAPE_CHARS.TryAdd('\n', "\\n");
            ESCAPE_CHARS.TryAdd('\t', "\\t");
            ESCAPE_CHARS.TryAdd('\f', "\\f");
            ESCAPE_CHARS.TryAdd('\r', "\\r");
            ESCAPE_CHARS.TryAdd('\"', "\\\"");
            ESCAPE_CHARS.TryAdd('\\', "\\\\");
            ESCAPE_CHARS.TryAdd('/', "\\/");
        }

        public static string Escape(string input)
        {
            StringBuilder buf = new StringBuilder();
            string value = null;
            foreach (var c in input)
            {
                if (ESCAPE_CHARS.TryGetValue(c, out value))
                    buf.Append(value);
                else
                    buf.Append(c);
            }
            return buf.ToString();
        }
    }
}
