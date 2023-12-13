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
using System.Text;
using static Loxodon.Framework.TextFormatting.IFormatter;
namespace Loxodon.Framework.TextFormatting
{
    public static class StringBuilderExtensions
    {
        private const int FORMAT_SPAN_SIZE = 128;
        private static readonly object EMPTY = new object();

        [ThreadStatic]
        private static StringBuilder result = new StringBuilder(128);

        public static StringBuilder AppendFormat<T>(this StringBuilder builder, string format, T[] values)
        {
            return AppendFormat(builder, format, values, GetFormatter<T>());
        }

        public static StringBuilder AppendFormat<T>(this StringBuilder builder, string format, T value)
        {
            return AppendFormat(builder, format, value, GetFormatter<T>());
        }

        public static StringBuilder AppendFormat<T0, T1>(this StringBuilder builder, string format, T0 t0, T1 t1)
        {
            return AppendFormat(builder, format, 2, t0, t1, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY);
        }

        public static StringBuilder AppendFormat<T0, T1, T2>(this StringBuilder builder, string format, T0 t0, T1 t1, T2 t2)
        {
            return AppendFormat(builder, format, 3, t0, t1, t2, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY);
        }

        public static StringBuilder AppendFormat<T0, T1, T2, T3>(this StringBuilder builder, string format, T0 t0, T1 t1, T2 t2, T3 t3)
        {
            return AppendFormat(builder, format, 4, t0, t1, t2, t3, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY);
        }

        public static StringBuilder AppendFormat<T0, T1, T2, T3, T4>(this StringBuilder builder, string format, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return AppendFormat(builder, format, 5, t0, t1, t2, t3, t4, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY);
        }

        public static StringBuilder AppendFormat<T0, T1, T2, T3, T4, T5>(this StringBuilder builder, string format, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return AppendFormat(builder, format, 6, t0, t1, t2, t3, t4, t5, EMPTY, EMPTY, EMPTY, EMPTY);
        }

        public static StringBuilder AppendFormat<T0, T1, T2, T3, T4, T5, T6>(this StringBuilder builder, string format, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            return AppendFormat(builder, format, 7, t0, t1, t2, t3, t4, t5, t6, EMPTY, EMPTY, EMPTY);
        }

        public static StringBuilder AppendFormat<T0, T1, T2, T3, T4, T5, T6, T7>(this StringBuilder builder, string format, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            return AppendFormat(builder, format, 8, t0, t1, t2, t3, t4, t5, t6, t7, EMPTY, EMPTY);
        }

        public static StringBuilder AppendFormat<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this StringBuilder builder, string format, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            return AppendFormat(builder, format, 9, t0, t1, t2, t3, t4, t5, t6, t7, t8, EMPTY);
        }

        public static StringBuilder AppendFormat<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this StringBuilder builder, string format, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            return AppendFormat(builder, format, 10, t0, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }

        private static StringBuilder AppendFormat<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(StringBuilder builder, string format, int paramCount, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            int pos = 0;
            int len = format.Length;
            char ch = '\x0';
            Span<char> formatSpan = stackalloc char[FORMAT_SPAN_SIZE];
            int formatIndex = 0;
            while (true)
            {
                while (pos < len)
                {
                    ch = format[pos];

                    pos++;
                    if (ch == '}')
                    {
                        if (pos < len && format[pos] == '}') // Treat as escape character for }}
                            pos++;
                        else
                            FormatError();
                    }

                    if (ch == '{')
                    {
                        if (pos < len && format[pos] == '{') // Treat as escape character for {{
                            pos++;
                        else
                        {
                            pos--;
                            break;
                        }
                    }

                    builder.Append(ch);
                }

                if (pos == len)
                    break;

                pos++;
                if (pos == len || (ch = format[pos]) < '0' || ch > '9')
                    FormatError();
                int index = 0;
                do
                {
                    index = index * 10 + ch - '0';
                    pos++;
                    if (pos == len)
                        FormatError();
                    ch = format[pos];
                } while (ch >= '0' && ch <= '9' && index < 1000000);

                if (index >= paramCount)
                    throw new FormatException("The index of the format is out of range.");

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;

                bool leftJustify = false;
                int width = 0;
                if (ch == ',')
                {
                    pos++;
                    while (pos < len && format[pos] == ' ')
                        pos++;

                    if (pos == len)
                        FormatError();
                    ch = format[pos];
                    if (ch == '-')
                    {
                        leftJustify = true;
                        pos++;
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                    }
                    if (ch < '0' || ch > '9')
                        FormatError();
                    do
                    {
                        width = width * 10 + ch - '0';
                        pos++;
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                    } while (ch >= '0' && ch <= '9' && width < 1000000);
                }

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;

                formatIndex = 0;
                if (ch == ':')
                {
                    pos++;
                    while (true)
                    {
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                        if (!IsValidFormatChar(ch))
                            break;

                        formatSpan[formatIndex++] = ch;
                        pos++;

                        ///Delete the following code. There is a bug here. When there are three curly brackets next to each other, 
                        ///the format will not meet the expectations. For example, {{this is test,my age is {Age:D2}}}, 
                        ///the format after parsing is "D2} " instead of "D2"

                        //if (ch == '{')
                        //{
                        //    if (pos < len && format[pos] == '{')  // Treat as escape character for {{
                        //        pos++;
                        //    else
                        //        FormatError();
                        //}
                        //else if (ch == '}')
                        //{
                        //    if (pos < len && format[pos] == '}')  // Treat as escape character for }}
                        //        pos++;
                        //    else
                        //    {
                        //        pos--;
                        //        break;
                        //    }
                        //}                       
                    }
                }

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;

                if (ch != '}')
                    FormatError();
                pos++;

                ReadOnlySpan<char> fmt = formatSpan.Slice(0, formatIndex);
                switch (index)
                {
                    case 0:
                        Format(fmt, t0, result.Clear());
                        break;
                    case 1:
                        Format(fmt, t1, result.Clear());
                        break;
                    case 2:
                        Format(fmt, t2, result.Clear());
                        break;
                    case 3:
                        Format(fmt, t3, result.Clear());
                        break;
                    case 4:
                        Format(fmt, t4, result.Clear());
                        break;
                    case 5:
                        Format(fmt, t5, result.Clear());
                        break;
                    case 6:
                        Format(fmt, t6, result.Clear());
                        break;
                    case 7:
                        Format(fmt, t7, result.Clear());
                        break;
                    case 8:
                        Format(fmt, t8, result.Clear());
                        break;
                    case 9:
                        Format(fmt, t9, result.Clear());
                        break;
                    default:
                        throw new NotSupportedException();
                }

                int pad = width - result.Length;
                if (!leftJustify && pad > 0)
                    builder.Append(' ', pad);
                AppendStringBuilder(builder, result);
                result.Clear();
                if (leftJustify && pad > 0)
                    builder.Append(' ', pad);
            }
            return builder;
        }

        private static StringBuilder AppendFormat<T>(StringBuilder builder, string format, T value, IFormatter formatter)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            int pos = 0;
            int len = format.Length;
            char ch = '\x0';
            Span<char> formatSpan = stackalloc char[FORMAT_SPAN_SIZE];
            int formatIndex = 0;
            while (true)
            {
                while (pos < len)
                {
                    ch = format[pos];

                    pos++;
                    if (ch == '}')
                    {
                        if (pos < len && format[pos] == '}') // Treat as escape character for }}
                            pos++;
                        else
                            FormatError();
                    }

                    if (ch == '{')
                    {
                        if (pos < len && format[pos] == '{') // Treat as escape character for {{
                            pos++;
                        else
                        {
                            pos--;
                            break;
                        }
                    }

                    builder.Append(ch);
                }

                if (pos == len)
                    break;

                pos++;
                if (pos == len || (ch = format[pos]) < '0' || ch > '9')
                    FormatError();
                int index = 0;
                do
                {
                    index = index * 10 + ch - '0';
                    pos++;
                    if (pos == len)
                        FormatError();
                    ch = format[pos];
                } while (ch >= '0' && ch <= '9' && index < 1000000);
                if (index >= 1)
                    throw new FormatException("The index of the format is out of range.");

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;

                bool leftJustify = false;
                int width = 0;
                if (ch == ',')
                {
                    pos++;
                    while (pos < len && format[pos] == ' ')
                        pos++;

                    if (pos == len)
                        FormatError();
                    ch = format[pos];
                    if (ch == '-')
                    {
                        leftJustify = true;
                        pos++;
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                    }
                    if (ch < '0' || ch > '9')
                        FormatError();
                    do
                    {
                        width = width * 10 + ch - '0';
                        pos++;
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                    } while (ch >= '0' && ch <= '9' && width < 1000000);
                }

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;
                //object arg = args[index];
                formatIndex = 0;
                if (ch == ':')
                {
                    pos++;
                    while (true)
                    {
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                        if (!IsValidFormatChar(ch))
                            break;

                        formatSpan[formatIndex++] = ch;
                        pos++;

                        ///Delete the following code. There is a bug here. When there are three curly brackets next to each other, 
                        ///the format will not meet the expectations. For example, {{this is test,my age is {Age:D2}}}, 
                        ///the format after parsing is "D2} " instead of "D2"

                        //if (ch == '{')
                        //{
                        //    if (pos < len && format[pos] == '{')  // Treat as escape character for {{
                        //        pos++;
                        //    else
                        //        FormatError();
                        //}
                        //else if (ch == '}')
                        //{
                        //    if (pos < len && format[pos] == '}')  // Treat as escape character for }}
                        //        pos++;
                        //    else
                        //    {
                        //        pos--;
                        //        break;
                        //    }
                        //}                        
                    }
                }

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;

                if (ch != '}')
                    FormatError();
                pos++;

                ReadOnlySpan<char> fmt = formatSpan.Slice(0, formatIndex);
                Format(fmt, value, formatter, result.Clear());

                int pad = width - result.Length;
                if (!leftJustify && pad > 0)
                    builder.Append(' ', pad);
                AppendStringBuilder(builder, result);
                result.Clear();
                if (leftJustify && pad > 0)
                    builder.Append(' ', pad);
            }
            return builder;
        }

        private static StringBuilder AppendFormat<T>(StringBuilder builder, string format, T[] values, IFormatter formatter)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            int pos = 0;
            int len = format.Length;
            char ch = '\x0';
            Span<char> formatSpan = stackalloc char[FORMAT_SPAN_SIZE];
            int formatIndex = 0;
            while (true)
            {
                while (pos < len)
                {
                    ch = format[pos];

                    pos++;
                    if (ch == '}')
                    {
                        if (pos < len && format[pos] == '}') // Treat as escape character for }}
                            pos++;
                        else
                            FormatError();
                    }

                    if (ch == '{')
                    {
                        if (pos < len && format[pos] == '{') // Treat as escape character for {{
                            pos++;
                        else
                        {
                            pos--;
                            break;
                        }
                    }

                    builder.Append(ch);
                }

                if (pos == len)
                    break;

                pos++;
                if (pos == len || (ch = format[pos]) < '0' || ch > '9')
                    FormatError();
                int index = 0;
                do
                {
                    index = index * 10 + ch - '0';
                    pos++;
                    if (pos == len)
                        FormatError();
                    ch = format[pos];
                } while (ch >= '0' && ch <= '9' && index < 1000000);
                if (index >= values.Length)
                    throw new FormatException("The index of the format is out of range.");

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;

                bool leftJustify = false;
                int width = 0;
                if (ch == ',')
                {
                    pos++;
                    while (pos < len && format[pos] == ' ')
                        pos++;

                    if (pos == len)
                        FormatError();
                    ch = format[pos];
                    if (ch == '-')
                    {
                        leftJustify = true;
                        pos++;
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                    }
                    if (ch < '0' || ch > '9')
                        FormatError();
                    do
                    {
                        width = width * 10 + ch - '0';
                        pos++;
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                    } while (ch >= '0' && ch <= '9' && width < 1000000);
                }

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;
                T value = values[index];

                formatIndex = 0;
                if (ch == ':')
                {
                    pos++;
                    while (true)
                    {
                        if (pos == len)
                            FormatError();
                        ch = format[pos];
                        if (!IsValidFormatChar(ch))
                            break;

                        formatSpan[formatIndex++] = ch;
                        pos++;

                        ///Delete the following code. There is a bug here. When there are three curly brackets next to each other, 
                        ///the format will not meet the expectations. For example, {{this is test,my age is {Age:D2}}}, 
                        ///the format after parsing is "D2} " instead of "D2"

                        //if (ch == '{')
                        //{
                        //    if (pos < len && format[pos] == '{')  // Treat as escape character for {{
                        //        pos++;
                        //    else
                        //        FormatError();
                        //}
                        //else if (ch == '}')
                        //{
                        //    if (pos < len && format[pos] == '}')  // Treat as escape character for }}
                        //        pos++;
                        //    else
                        //    {
                        //        pos--;
                        //        break;
                        //    }
                        //}                       
                    }
                }

                while (pos < len && (ch = format[pos]) == ' ')
                    pos++;

                if (ch != '}')
                    FormatError();
                pos++;

                ReadOnlySpan<char> fmt = formatSpan.Slice(0, formatIndex);
                Format(fmt, value, formatter, result.Clear());

                int pad = width - result.Length;
                if (!leftJustify && pad > 0)
                    builder.Append(' ', pad);
                AppendStringBuilder(builder, result);
                result.Clear();
                if (leftJustify && pad > 0)
                    builder.Append(' ', pad);
            }
            return builder;
        }

        private static bool IsValidFormatChar(char ch)
        {
            if (ch == 123 || ch == 125)//{ } 
                return false;

            if ((ch >= 32 && ch <= 122) || ch == 124)
                return true;
            return false;
        }

        private static void Format<T>(ReadOnlySpan<char> format, T value, IFormatter formatter, StringBuilder builder)
        {
            if (formatter is IFormatter<T> genericFormatter)
                genericFormatter.Format(format, value, builder);
            else
                formatter.Format(format, value, builder);
        }

        private static void Format<T>(ReadOnlySpan<char> format, T value, StringBuilder builder)
        {
            IFormatter formatter = GetFormatter<T>();
            if (formatter is IFormatter<T> genericFormatter)
                genericFormatter.Format(format, value, builder);
            else
                formatter.Format(format, value, builder);
        }

        private static StringBuilder AppendStringBuilder(StringBuilder builder, StringBuilder value)
        {
            int len = value.Length;
            for (int i = 0; i < len; i++)
            {
                builder.Append(value[i]);
            }
            return builder;
        }

        private static void FormatError()
        {
            throw new FormatException("Invalid Format");
        }
    }
}