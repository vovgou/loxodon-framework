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
using System.Collections.Concurrent;
using System.Text;
using UnityEngine;

namespace Loxodon.Framework.TextFormatting
{
    public interface IFormatter
    {
        public readonly static IFormatter<bool> BOOLEAN_FORMATTER = new BooleanFormatter();
        public readonly static IFormatter<char> CHAR_FORMATTER = new CharFormatter();
        public readonly static IFormatter<byte> BYTE_FORMATTER = new ByteFormatter();
        public readonly static IFormatter<sbyte> SBYTE_FORMATTER = new SByteFormatter();
        public readonly static IFormatter<short> SHORT_FORMATTER = new Int16Formatter();
        public readonly static IFormatter<ushort> USHORT_FORMATTER = new UInt16Formatter();
        public readonly static IFormatter<int> INT_FORMATTER = new Int32Formatter();
        public readonly static IFormatter<uint> UINT_FORMATTER = new UInt32Formatter();
        public readonly static IFormatter<long> LONG_FORMATTER = new Int64Formatter();
        public readonly static IFormatter<ulong> ULONG_FORMATTER = new UInt64Formatter();
        public readonly static IFormatter<float> FLOAT_FORMATTER = new FloatFormatter();
        public readonly static IFormatter<double> DOUBLE_FORMATTER = new DoubleFormatter();
        public readonly static IFormatter<decimal> DECIMAL_FORMATTER = new DecimalFormatter();
        public readonly static IFormatter<DateTime> DATETIME_FORMATTER = new DateTimeFormatter();
        public readonly static IFormatter<TimeSpan> TIMESPAN_FORMATTER = new TimeSpanFormatter();
        public readonly static IFormatter<Vector2> VECTOR2_FORMATTER = new Vector2Formatter();
        public readonly static IFormatter<Vector3> VECTOR3_FORMATTER = new Vector3Formatter();
        public readonly static IFormatter<Vector4> VECTOR4_FORMATTER = new Vector4Formatter();
        public readonly static IFormatter<Rect> RECT_FORMATTER = new RectFormatter();
        public readonly static IFormatter<object> DEFAULT_FORMATTER = new DefaultFormatter();
        public readonly static ConcurrentDictionary<Type, IFormatter> FORMATTERS = new ConcurrentDictionary<Type, IFormatter>();
        static IFormatter()
        {
            FORMATTERS.TryAdd(typeof(bool), BOOLEAN_FORMATTER);
            FORMATTERS.TryAdd(typeof(char), CHAR_FORMATTER);
            FORMATTERS.TryAdd(typeof(byte), BYTE_FORMATTER);
            FORMATTERS.TryAdd(typeof(sbyte), SBYTE_FORMATTER);
            FORMATTERS.TryAdd(typeof(short), SHORT_FORMATTER);
            FORMATTERS.TryAdd(typeof(ushort), USHORT_FORMATTER);
            FORMATTERS.TryAdd(typeof(int), INT_FORMATTER);
            FORMATTERS.TryAdd(typeof(uint), UINT_FORMATTER);
            FORMATTERS.TryAdd(typeof(long), LONG_FORMATTER);
            FORMATTERS.TryAdd(typeof(ulong), ULONG_FORMATTER);
            FORMATTERS.TryAdd(typeof(float), FLOAT_FORMATTER);
            FORMATTERS.TryAdd(typeof(double), DOUBLE_FORMATTER);
            FORMATTERS.TryAdd(typeof(decimal), DECIMAL_FORMATTER);
            FORMATTERS.TryAdd(typeof(DateTime), DATETIME_FORMATTER);
            FORMATTERS.TryAdd(typeof(TimeSpan), TIMESPAN_FORMATTER);
            FORMATTERS.TryAdd(typeof(Vector2), VECTOR2_FORMATTER);
            FORMATTERS.TryAdd(typeof(Vector3), VECTOR3_FORMATTER);
            FORMATTERS.TryAdd(typeof(Vector4), VECTOR4_FORMATTER);
            FORMATTERS.TryAdd(typeof(Rect), RECT_FORMATTER);
            FORMATTERS.TryAdd(typeof(object), DEFAULT_FORMATTER);
        }

        public static IFormatter GetFormatter<T>()
        {
            return GetFormatter(typeof(T));
        }

        public static IFormatter GetFormatter(Type type)
        {
            IFormatter formatter = null;
            if (FORMATTERS.TryGetValue(type, out formatter))
                return formatter;
            return DEFAULT_FORMATTER;
        }

        public static bool Register<T>(IFormatter<T> formatter)
        {
            return FORMATTERS.TryAdd(typeof(T), formatter);
        }

        public static IFormatter<T> Unregister<T>()
        {
            IFormatter formatter;
            FORMATTERS.TryRemove(typeof(T), out formatter);
            return formatter as IFormatter<T>;
        }

        void Format(ReadOnlySpan<char> format, object value, StringBuilder builder);

        // In UnityEditor, when the parameter type is ReadOnlySpan<char>, the first call is correct. 
        // When the second call is made, the void Format(string format, object value, StringBuilder builder) function is actually called, 
        // which will result in UnityEditor crashes.
        //void Format(string format, object value, StringBuilder builder);
    }

    public interface IFormatter<T> : IFormatter
    {
        void Format(ReadOnlySpan<char> format, T value, StringBuilder builder);

        //void Format(string format, T value, StringBuilder builder);
    }

    public abstract class FormatterBase : IFormatter
    {
        public void Format(ReadOnlySpan<char> format, object value, StringBuilder builder)
        {
            if (format.Length > 0 && value is IFormattable formattable)
                builder.Append(formattable.ToString(format.ToString(), null));
            else if (value != null)
                builder.Append(value.ToString());
            else if (format.Length > 0)
                builder.Append(format);
        }

        //public void Format(string format, object value, StringBuilder builder)
        //{
        //    if (!string.IsNullOrEmpty(format) && value is IFormattable formattable)
        //        builder.Append(formattable.ToString(format, null));
        //    else if (value != null)
        //        builder.Append(value.ToString());
        //    else if (!string.IsNullOrEmpty(format))
        //        builder.Append(format);
        //}
    }

    internal class DefaultFormatter : FormatterBase, IFormatter<object>
    {
    }

    internal class BooleanFormatter : FormatterBase, IFormatter<bool>
    {
        public void Format(ReadOnlySpan<char> format, bool value, StringBuilder builder)
        {
            builder.Append(value ? "true" : "false");
        }

        //public void Format(string format, bool value, StringBuilder builder)
        //{
        //    builder.Append(value ? "true" : "false");
        //}
    }

    internal class CharFormatter : FormatterBase, IFormatter<char>
    {
        public void Format(ReadOnlySpan<char> format, char value, StringBuilder builder)
        {
            builder.Append(value);
        }

        //public void Format(string format, char value, StringBuilder builder)
        //{
        //    builder.Append(value);
        //}
    }

    internal class ByteFormatter : FormatterBase, IFormatter<byte>
    {
        public void Format(ReadOnlySpan<char> format, byte value, StringBuilder builder)
        {
            NumberFormatter.NumberToString(format, value, null, builder);
        }

        //public void Format(string format, byte value, StringBuilder builder)
        //{
        //    NumberFormatter.NumberToString(format, value, null, builder);
        //}
    }

    internal class SByteFormatter : FormatterBase, IFormatter<sbyte>
    {
        public void Format(ReadOnlySpan<char> format, sbyte value, StringBuilder builder)
        {
            NumberFormatter.NumberToString(format, value, null, builder);
        }

        //public void Format(string format, sbyte value, StringBuilder builder)
        //{
        //    NumberFormatter.NumberToString(format, value, null, builder);
        //}
    }

    internal class Int16Formatter : FormatterBase, IFormatter<short>
    {
        public void Format(ReadOnlySpan<char> format, short value, StringBuilder builder)
        {
            NumberFormatter.NumberToString(format, value, null, builder);
        }

        //public void Format(string format, short value, StringBuilder builder)
        //{
        //    NumberFormatter.NumberToString(format, value, null, builder);
        //}
    }

    internal class UInt16Formatter : FormatterBase, IFormatter<ushort>
    {
        public void Format(ReadOnlySpan<char> format, ushort value, StringBuilder builder)
        {
            NumberFormatter.NumberToString(format, value, null, builder);
        }

        //public void Format(string format, ushort value, StringBuilder builder)
        //{
        //    NumberFormatter.NumberToString(format, value, null, builder);
        //}
    }

    internal class Int32Formatter : FormatterBase, IFormatter<int>
    {
        public void Format(ReadOnlySpan<char> format, int value, StringBuilder builder)
        {
            NumberFormatter.NumberToString(format, value, null, builder);
        }

        //public void Format(string format, int value, StringBuilder builder)
        //{
        //    NumberFormatter.NumberToString(format, value, null, builder);
        //}
    }

    internal class UInt32Formatter : FormatterBase, IFormatter<uint>
    {
        public void Format(ReadOnlySpan<char> format, uint value, StringBuilder builder)
        {
            NumberFormatter.NumberToString(format, value, null, builder);
        }

        //public void Format(string format, uint value, StringBuilder builder)
        //{
        //    NumberFormatter.NumberToString(format, value, null, builder);
        //}
    }

    internal class Int64Formatter : FormatterBase, IFormatter<long>
    {
        public void Format(ReadOnlySpan<char> format, long value, StringBuilder builder)
        {
            NumberFormatter.NumberToString(format, value, null, builder);
        }

        //public void Format(string format, long value, StringBuilder builder)
        //{
        //    NumberFormatter.NumberToString(format, value, null, builder);
        //}
    }

    internal class UInt64Formatter : FormatterBase, IFormatter<ulong>
    {
        public void Format(ReadOnlySpan<char> format, ulong value, StringBuilder builder)
        {
            NumberFormatter.NumberToString(format, value, null, builder);
        }

        //public void Format(string format, ulong value, StringBuilder builder)
        //{
        //    NumberFormatter.NumberToString(format, value, null, builder);
        //}
    }

    internal class FloatFormatter : FormatterBase, IFormatter<float>
    {
        public void Format(ReadOnlySpan<char> format, float value, StringBuilder builder)
        {
            NumberFormatter.NumberToString(format, value, null, builder);
        }

        //public void Format(string format, float value, StringBuilder builder)
        //{
        //    NumberFormatter.NumberToString(format, value, null, builder);
        //}
    }

    internal class DoubleFormatter : FormatterBase, IFormatter<double>
    {
        public void Format(ReadOnlySpan<char> format, double value, StringBuilder builder)
        {
            NumberFormatter.NumberToString(format, value, null, builder);
        }

        //public void Format(string format, double value, StringBuilder builder)
        //{
        //    NumberFormatter.NumberToString(format, value, null, builder);
        //}
    }

    internal class DecimalFormatter : FormatterBase, IFormatter<decimal>
    {
        public void Format(ReadOnlySpan<char> format, decimal value, StringBuilder builder)
        {
            NumberFormatter.NumberToString(format, value, null, builder);
        }

        //public void Format(string format, decimal value, StringBuilder builder)
        //{
        //    NumberFormatter.NumberToString(format, value, null, builder);
        //}
    }

    internal class DateTimeFormatter : FormatterBase, IFormatter<DateTime>
    {
        public void Format(ReadOnlySpan<char> format, DateTime value, StringBuilder builder)
        {
            DateTimeFormat.Format(value, format, builder);
        }

        //public void Format(string format, DateTime value, StringBuilder builder)
        //{
        //    DateTimeFormat.Format(value, format, builder);
        //}
    }

    internal class TimeSpanFormatter : FormatterBase, IFormatter<TimeSpan>
    {
        public void Format(ReadOnlySpan<char> format, TimeSpan value, StringBuilder builder)
        {
            TimeSpanFormat.Format(value, format, builder);
        }

        //public void Format(string format, TimeSpan value, StringBuilder builder)
        //{
        //    TimeSpanFormat.Format(value, format, builder);
        //}
    }

    internal class Vector2Formatter : FormatterBase, IFormatter<Vector2>
    {
        public void Format(ReadOnlySpan<char> format, Vector2 value, StringBuilder builder)
        {
            if (format.Length <= 0)
                format = "F2";

            builder.Append("(");
            NumberFormatter.NumberToString(format, value.x, null, builder);
            builder.Append(",");
            NumberFormatter.NumberToString(format, value.y, null, builder);
            builder.Append(")");
        }

        //public void Format(string format, Vector2 value, StringBuilder builder)
        //{
        //    ReadOnlySpan<char> f = string.IsNullOrEmpty(format) ? "F2" : format;
        //    this.Format(f, value, builder);
        //}
    }

    internal class Vector3Formatter : FormatterBase, IFormatter<Vector3>
    {
        public void Format(ReadOnlySpan<char> format, Vector3 value, StringBuilder builder)
        {
            if (format.Length <= 0)
                format = "F2";

            builder.Append("(");
            NumberFormatter.NumberToString(format, value.x, null, builder);
            builder.Append(",");
            NumberFormatter.NumberToString(format, value.y, null, builder);
            builder.Append(",");
            NumberFormatter.NumberToString(format, value.z, null, builder);
            builder.Append(")");
        }

        //public void Format(string format, Vector3 value, StringBuilder builder)
        //{
        //    ReadOnlySpan<char> f = string.IsNullOrEmpty(format) ? "F2" : format;
        //    this.Format(f, value, builder);
        //}
    }

    internal class Vector4Formatter : FormatterBase, IFormatter<Vector4>
    {
        public void Format(ReadOnlySpan<char> format, Vector4 value, StringBuilder builder)
        {
            if (format.Length <= 0)
                format = "F2";

            builder.Append("(");
            NumberFormatter.NumberToString(format, value.x, null, builder);
            builder.Append(",");
            NumberFormatter.NumberToString(format, value.y, null, builder);
            builder.Append(",");
            NumberFormatter.NumberToString(format, value.z, null, builder);
            builder.Append(",");
            NumberFormatter.NumberToString(format, value.w, null, builder);
            builder.Append(")");
        }

        //public void Format(string format, Vector4 value, StringBuilder builder)
        //{
        //    ReadOnlySpan<char> f = string.IsNullOrEmpty(format) ? "F2" : format;
        //    this.Format(f, value, builder);
        //}
    }

    internal class RectFormatter : FormatterBase, IFormatter<Rect>
    {
        public void Format(ReadOnlySpan<char> format, Rect value, StringBuilder builder)
        {
            if (format.Length <= 0)
                format = "F2";

            builder.Append("(x:");
            NumberFormatter.NumberToString(format, value.x, null, builder);
            builder.Append(",y:");
            NumberFormatter.NumberToString(format, value.y, null, builder);
            builder.Append(",width:");
            NumberFormatter.NumberToString(format, value.width, null, builder);
            builder.Append(",height:");
            NumberFormatter.NumberToString(format, value.height, null, builder);
            builder.Append(")");
        }

        //public void Format(string format, Rect value, StringBuilder builder)
        //{
        //    ReadOnlySpan<char> f = string.IsNullOrEmpty(format) ? "F2" : format;
        //    this.Format(f, value, builder);
        //}
    }
}