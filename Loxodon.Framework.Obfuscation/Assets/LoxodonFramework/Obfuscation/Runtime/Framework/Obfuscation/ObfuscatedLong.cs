using System;
using System.Globalization;

namespace Loxodon.Framework.Obfuscation
{
    [Serializable]
    public struct ObfuscatedLong : IComparable, IConvertible, IComparable<long>, IEquatable<long>
    {
        private static long sequence = DateTime.Now.Ticks;

        private long seed;
        private long data;
        private long check;

        public ObfuscatedLong(long value)
            : this(value, sequence++)
        {
        }

        public ObfuscatedLong(long value, long seed)
        {
            this.seed = seed;
            this.data = 0;
            this.check = 0;
            Value = value;
        }

        internal long Value
        {
            get
            {
                var v = data ^ seed;
                if (((seed >> 8) ^ v) != check)
                    throw new Exception();
                return v;
            }
            set
            {
                data = value ^ seed;
                check = (seed >> 8) ^ value;
            }
        }

        /// <summary>
        /// 隐式转换long到ObfuscatedLong
        /// 如：ObfuscatedLong i=2;
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator ObfuscatedLong(long data)
        {
            return new ObfuscatedLong(data);
        }

        public static implicit operator ObfuscatedLong(byte data)
        {
            return new ObfuscatedLong(data);
        }

        public static explicit operator byte(ObfuscatedLong data)
        {
            return Convert.ToByte(data.Value);
        }

        public static implicit operator ObfuscatedLong(short data)
        {
            return new ObfuscatedLong(data);
        }

        public static explicit operator short(ObfuscatedLong data)
        {
            return Convert.ToInt16(data.Value);
        }

        /// <summary>
        /// 隐式转换ObfuscatedLong 到 long
        /// 如：long i=new ObfuscatedLong(20);
        /// 当然显示转换也支持，如下
        /// 如：ObfuscatedLong i=new ObfuscatedLong(20); long n= (int)i;
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator long(ObfuscatedLong data)
        {
            return data.Value;
        }

        /// <summary>
        /// 隐式转换int到ObfuscatedLong
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator ObfuscatedLong(int data)
        {
            return new ObfuscatedLong(data);
        }

        /// <summary>
        /// 显示转换ObfuscatedLong 到 int
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator int(ObfuscatedLong data)
        {
            return Convert.ToInt32(data.Value);
        }

        /// <summary>
        /// 显示转换浮点到ObfuscatedLong (高精度转为低精度 要求显示强转)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedLong(float data)
        {
            return new ObfuscatedLong(Convert.ToInt64(data));
        }

        /// <summary>
        /// 转换ObfuscatedLong 到浮点(隐式转换)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator float(ObfuscatedLong data)
        {
            return data.Value;
        }

        /// <summary>
        /// 转换double到ObfuscatedLong (高精度转为低精度 要求显示强转)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedLong(double data)
        {
            return new ObfuscatedLong(Convert.ToInt64(data));
        }

        /// <summary>
        /// 转换ObfuscatedLong到double(隐式)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator double(ObfuscatedLong data)
        {
            return data.Value;
        }

        /* 重载操作符 == != > < >= <= */
        public static bool operator ==(ObfuscatedLong lhs, ObfuscatedLong rhs)
        {
            return lhs.Value == rhs.Value;
        }

        public static bool operator !=(ObfuscatedLong lhs, ObfuscatedLong rhs)
        {
            return lhs.Value != rhs.Value;
        }

        public static bool operator >(ObfuscatedLong lhs, ObfuscatedLong rhs)
        {
            return lhs.Value > rhs.Value;
        }

        public static bool operator <(ObfuscatedLong lhs, ObfuscatedLong rhs)
        {
            return lhs.Value < rhs.Value;
        }

        public static bool operator >=(ObfuscatedLong lhs, ObfuscatedLong rhs)
        {
            return lhs.Value >= rhs.Value;
        }

        public static bool operator <=(ObfuscatedLong lhs, ObfuscatedLong rhs)
        {
            return lhs.Value <= rhs.Value;
        }

        /* 重载操作符 ++ -- + - * / */
        public static ObfuscatedLong operator ++(ObfuscatedLong data)
        {
            return new ObfuscatedLong(data.Value + 1);
        }

        public static ObfuscatedLong operator --(ObfuscatedLong data)
        {
            return new ObfuscatedLong(data.Value - 1);
        }

        public static ObfuscatedLong operator +(ObfuscatedLong lhs, ObfuscatedLong rhs)
        {
            return new ObfuscatedLong(lhs.Value + rhs.Value);
        }

        public static ObfuscatedLong operator -(ObfuscatedLong lhs, ObfuscatedLong rhs)
        {
            return new ObfuscatedLong(lhs.Value - rhs.Value);
        }

        public static ObfuscatedLong operator *(ObfuscatedLong lhs, ObfuscatedLong rhs)
        {
            return new ObfuscatedLong(lhs.Value * rhs.Value);
        }

        public static ObfuscatedLong operator /(ObfuscatedLong lhs, ObfuscatedLong rhs)
        {
            return new ObfuscatedLong(lhs.Value / rhs.Value);
        }

        public static ObfuscatedLong operator %(ObfuscatedLong lhs, ObfuscatedLong rhs)
        {
            return new ObfuscatedLong(lhs.Value % rhs.Value);
        }

        public int CompareTo(object value)
        {
            if (value == null) return 1;

            if (value is long)
            {
                long i = (long)value;
                if (Value < i) return -1;
                if (Value > i) return 1;
                return 0;
            }

            if (value is ObfuscatedLong)
            {
                ObfuscatedLong i = (ObfuscatedLong)value;
                if (Value < i.Value) return -1;
                if (Value > i.Value) return 1;
                return 0;
            }

            throw new ArgumentException();
        }

        public int CompareTo(long value)
        {
            if (Value < value) return -1;
            if (Value > value) return 1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObfuscatedLong)
                return this.Value == ((ObfuscatedLong)obj).Value;

            if (obj is long)
                return this.Value == (long)obj;

            return false;
        }

        public bool Equals(long obj)
        {
            return (this.Value == obj);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return Convert.ToString(Value);
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Int64;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(this.Value);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this.Value);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this.Value);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this.Value);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this.Value);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.Value);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(this.Value);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.Value);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return this.Value;
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.Value);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.Value);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return Convert.ToString(this.Value);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this.Value);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(this.Value);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, "Can't convert {0} to {1}.", "long", "DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            return Convert.ChangeType(this.Value, type);
        }
    }
}


