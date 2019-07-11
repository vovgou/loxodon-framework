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
                return null;

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
        private StringSpliter()
        {
        }

        public void Reset(string text, char[] separators)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Invalid argument", "text");

            if (separators == null || separators.Length == 0)
                throw new ArgumentException("Invalid argument", "separators");

            this.text = text;
            this.separators = separators;
            this.total = this.text.Length;
            this.pos = -1;
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
        }

        public void Clear()
        {
            this.text = null;
            this.separators = null;
            this.pos = -1;
            this.total = 0;
        }

        public string[] Split()
        {
            List<string> list = new List<string>();
            while (this.MoveNext())
            {
                char ch = this.Current;
                if (separators.Contains(ch))
                {
                    list.Add("");
                    continue;
                }

                string content = this.ReadString(separators);
                list.Add(content);
            }

            if (separators.Contains(this.Current))
                list.Add("");

            return list.ToArray();
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
            char ch = this.Current;
            if (ch != start)
                throw new Exception(string.Format("Error parsing string , unexpected quote character {0} in text {1}", ch, this.text));

            while (this.MoveNext())
            {
                ch = this.Current;
                if (ch == end)
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
            return buf.ToString();
        }
    }
}
