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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Loxodon.Framework.Utilities
{
    public class StringSpliter : IEnumerator<char>
    {
        [ThreadStatic]
        private static StringSpliter spliter;
        private static StringSpliter Spliter
        {
            get
            {
                if (spliter == null)
                    spliter = new StringSpliter();
                return spliter;
            }
        }

        public static string[] Split(string input, params char[] characters)
        {
            if (string.IsNullOrEmpty(input))
                return new string[0];

            var spliter = Spliter;
            try
            {
                spliter.Reset(input, characters);
                return spliter.Split();
            }
            finally
            {
                spliter.Clear();
            }
        }

        private string text;
        private char[] separators;
        private int total = 0;
        private int pos = -1;
        private readonly List<string> items = new List<string>();
        private StringSpliter()
        {
        }

        public void Reset(string text, char[] separators)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Invalid argument", "text");

            if (separators == null || separators.Length == 0)
                this.separators = new char[] { ',' };
            else
                this.separators = separators;

            this.text = text;
            this.total = this.text.Length;
            this.pos = -1;
            this.items.Clear();
        }

        public char Current
        {
            get { return this.text[pos]; }
        }

        object IEnumerator.Current
        {
            get { return this.text[pos]; }
        }

        public void Dispose()
        {
            this.text = null;
            this.pos = -1;
        }

        public bool MoveNext()
        {
            if (this.pos < this.total - 1)
            {
                this.pos++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this.pos = -1;
            this.items.Clear();
        }

        public void Clear()
        {
            this.text = null;
            this.separators = null;
            this.pos = -1;
            this.total = 0;
            this.items.Clear();
        }

        public string[] Split()
        {
            while (this.MoveNext())
            {
                char ch = this.Current;
                if (separators.Contains(ch))
                {
                    items.Add("");
                    continue;
                }

                string content = this.ReadString(separators);
                items.Add(content);
            }

            if (separators.Contains(this.Current))
                items.Add("");

            return items.ToArray();
        }

        private bool IsEOF()
        {
            return this.pos >= this.total;
        }

        private void ReadStructString(StringBuilder buf, char start, char end)
        {
            char ch = this.Current;
            if (ch != start)
                throw new Exception(string.Format("Error parsing string , unexpected quote character {0} in text {1}", ch, this.text));

            buf.Append(ch);

            while (this.MoveNext())
            {
                ch = this.Current;
                if (ch == '(')
                {
                    ReadStructString(buf, '(', ')');
                    continue;
                }
                else if (ch == '[')
                {
                    ReadStructString(buf, '[', ']');
                    continue;
                }
                else if (ch == '{')
                {
                    ReadStructString(buf, '{', '}');
                    continue;
                }
                else if (ch == '<')
                {
                    ReadStructString(buf, '<', '>');
                    continue;
                }

                buf.Append(ch);
                if (ch == end)
                    return;
            }

            throw new Exception(string.Format("Not found the end character '{0}' in the text {1}.", end, this.text));
        }

        private void ReadQuotedString(StringBuilder buf, char start, char end)
        {
            char prev = '\0';
            char ch = this.Current;
            if (ch != start)
                throw new Exception(string.Format("Error parsing string , unexpected quote character {0} in text {1}", ch, this.text));

            while (this.MoveNext())
            {
                prev = ch;
                ch = this.Current;
                if (prev != '\\' && ch == end)
                    return;

                buf.Append(ch);
            }

            throw new Exception(string.Format("Not found the end character '{0}' in the text {1}.", end, this.text));
        }

        private string ReadString(char[] separators)
        {
            StringBuilder buf = new StringBuilder();
            char ch = this.Current;
            do
            {
                ch = this.Current;
                if (ch == '(')
                {
                    this.ReadStructString(buf, '(', ')');
                }
                else if (ch == '[')
                {
                    this.ReadStructString(buf, '[', ']');
                }
                else if (ch == '{')
                {
                    this.ReadStructString(buf, '{', '}');
                }
                else if (ch == '<')
                {
                    this.ReadStructString(buf, '<', '>');
                }
                else if (ch == '\'')
                {
                    this.ReadQuotedString(buf, '\'', '\'');
                }
                else if (ch == '\"')
                {
                    this.ReadQuotedString(buf, '\"', '\"');
                }
                else
                {
                    if (separators.Contains(ch))
                        break;

                    buf.Append(ch);
                }
            } while (this.MoveNext());

            buf.Replace("&quot;", "\"");
            buf.Replace("\\\"", "\"");
            return buf.ToString();
        }
    }
}
