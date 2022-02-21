using System;
using System.Globalization;

namespace Loxodon.Framework.Obfuscation
{
    [Serializable]
    public struct ObfuscatedShort : IComparable, IConvertible, IComparable<short>, IEquatable<short>
    {
        private static int sequence = (int)DateTime.Now.Ticks;

        private int seed;
        private int data;
        private int check;

        public ObfuscatedShort(short value)
            : this(value, sequence++)
        {
        }

        public ObfuscatedShort(short value, int seed)
        {
            this.seed = seed;
            this.data = 0;
            this.check = 0;
            Value = value;
        }

        internal short Value
        {
            get
            {
                var v = data ^ seed;
                if (((seed >> 8) ^ v) != check)
                    throw new Exception();
                return (short)v;
            }
            set
            {
                data = value ^ seed;
                check = (seed >> 8) ^ value;
            }
        }

        public static implicit operator ObfuscatedShort(byte data)
        {
            return new ObfuscatedShort(data);
        }

        public static explicit operator byte(ObfuscatedShort data)
        {
            return Convert.ToByte(data.Value);
        }

        public static implicit operator ObfuscatedShort(short data)
        {
            return new ObfuscatedShort(data);
        }

        public static explicit operator short(ObfuscatedShort data)
        {
            return data.Value;
        }

        public static explicit operator ObfuscatedShort(int data)
        {
            return new ObfuscatedShort(Convert.ToInt16(data));
        }

        public static implicit operator int(ObfuscatedShort data)
        {
            return data.Value;
        }

        /// <summary>
        /// 转换long到ObfuscatedShort（显示）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedShort(long data)
        {
            return new ObfuscatedShort(Convert.ToInt16(data));
        }

        /// <summary>
        /// 转换ObfuscatedShort 到 long(隐式)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator long(ObfuscatedShort data)
        {
            return data.Value;
        }

        /// <summary>
        /// 显示转换浮点到ObfuscatedShort (高精度转为低精度 要求显示强转)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedShort(float data)
        {
            return new ObfuscatedShort(Convert.ToInt16(data));
        }

        /// <summary>
        /// 转换ObfuscatedShort 到浮点
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator float(ObfuscatedShort data)
        {
            return data.Value;
        }

        /// <summary>
        /// 转换double到ObfuscatedShort (高精度转为低精度 要求显示强转)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedShort(double data)
        {
            return new ObfuscatedShort(Convert.ToInt16(data));
        }

        /// <summary>
        /// 转换ObfuscatedShort到double
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator double(ObfuscatedShort data)
        {
            return data.Value;
        }

        /* 重载操作符 == != > < >= <= */
        public static bool operator ==(ObfuscatedShort lhs, ObfuscatedShort rhs)
        {
            return lhs.Value == rhs.Value;
        }

        public static bool operator !=(ObfuscatedShort lhs, ObfuscatedShort rhs)
        {
            return lhs.Value != rhs.Value;
        }

        public static bool operator >(ObfuscatedShort lhs, ObfuscatedShort rhs)
        {
            return lhs.Value > rhs.Value;
        }

        public static bool operator <(ObfuscatedShort lhs, ObfuscatedShort rhs)
        {
            return lhs.Value < rhs.Value;
        }

        public static bool operator >=(ObfuscatedShort lhs, ObfuscatedShort rhs)
        {
            return lhs.Value >= rhs.Value;
        }

        public static bool operator <=(ObfuscatedShort lhs, ObfuscatedShort rhs)
        {
            return lhs.Value <= rhs.Value;
        }

        /* 重载操作符 ++ -- + - * / */
        public static ObfuscatedShort operator ++(ObfuscatedShort data)
        {
            data.Value++;
            return data;
        }

        public static ObfuscatedShort operator --(ObfuscatedShort data)
        {
            data.Value--;
            return data;
        }

        public static ObfuscatedShort operator +(ObfuscatedShort lhs, ObfuscatedShort rhs)
        {
            return new ObfuscatedShort(Convert.ToInt16(lhs.Value + rhs.Value));
        }

        public static ObfuscatedShort operator -(ObfuscatedShort lhs, ObfuscatedShort rhs)
        {
            return new ObfuscatedShort(Convert.ToInt16(lhs.Value - rhs.Value));
        }

        public static ObfuscatedShort operator *(ObfuscatedShort lhs, ObfuscatedShort rhs)
        {
            return new ObfuscatedShort(Convert.ToInt16(lhs.Value * rhs.Value));
        }

        public static ObfuscatedShort operator /(ObfuscatedShort lhs, ObfuscatedShort rhs)
        {
            return new ObfuscatedShort(Convert.ToInt16(lhs.Value / rhs.Value));
        }

        public static ObfuscatedShort operator %(ObfuscatedShort lhs, ObfuscatedShort rhs)
        {
            return new ObfuscatedShort(Convert.ToInt16(lhs.Value % rhs.Value));
        }

        public int CompareTo(object value)
        {
            if (value == null) return 1;

            if (value is short)
            {
                short i = (short)value;
                if (Value < i) return -1;
                if (Value > i) return 1;
                return 0;
            }

            if (value is ObfuscatedShort)
            {
                ObfuscatedShort i = (ObfuscatedShort)value;
                if (Value < i.Value) return -1;
                if (Value > i.Value) return 1;
                return 0;
            }

            throw new ArgumentException();
        }

        public int CompareTo(short value)
        {
            if (this.Value < value) return -1;
            if (this.Value > value) return 1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObfuscatedShort)
                return this.Value == ((ObfuscatedShort)obj).Value;

            if (obj is short)
                return this.Value == (short)obj;

            return false;
        }

        public bool Equals(short obj)
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
            return TypeCode.Int16;
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
            return this.Value;
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.Value);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this.Value);
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
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, "Can't convert {0} to {1}.", "short", "DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            return Convert.ChangeType(this.Value, type);
        }
    }
}
