using System;
using System.Globalization;

namespace Loxodon.Framework.Obfuscation
{
    [Serializable]
    public struct ObfuscatedByte : IComparable, IConvertible, IComparable<byte>, IEquatable<byte>
    {
        private static int sequence = (int)DateTime.Now.Ticks;
        private int seed;
        private int data;
        private int check;

        public ObfuscatedByte(byte value)
            : this(value, sequence++)
        {
        }

        public ObfuscatedByte(byte value, int seed)
        {
            this.seed = seed;
            this.data = 0;
            this.check = 0;
            Value = value;
        }

        internal byte Value
        {
            get
            {
                var v = data ^ seed;
                if (((seed >> 8) ^ v) != check)
                    throw new Exception();
                return (byte)v;
            }
            set
            {
                data = value ^ seed;
                check = (seed >> 8) ^ value;
            }
        }

        public static implicit operator ObfuscatedByte(byte data)
        {
            return new ObfuscatedByte(data);
        }

        public static implicit operator byte(ObfuscatedByte data)
        {
            return data.Value;
        }

        public static explicit operator ObfuscatedByte(short data)
        {
            return new ObfuscatedByte(Convert.ToByte(data));
        }

        public static implicit operator short(ObfuscatedByte data)
        {
            return data.Value;
        }

        public static explicit operator ObfuscatedByte(int data)
        {
            return new ObfuscatedByte(Convert.ToByte(data));
        }

        public static implicit operator int(ObfuscatedByte data)
        {
            return data.Value;
        }

        /// <summary>
        /// 转换long到ObfuscatedByte（显示）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedByte(long data)
        {
            return new ObfuscatedByte(Convert.ToByte(data));
        }

        /// <summary>
        /// 转换ObfuscatedByte 到 long(隐式)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator long(ObfuscatedByte data)
        {
            return data.Value;
        }

        /// <summary>
        /// 显示转换浮点到ObfuscatedByte (高精度转为低精度 要求显示强转)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedByte(float data)
        {
            return new ObfuscatedByte(Convert.ToByte(data));
        }

        /// <summary>
        /// 转换ObfuscatedByte 到浮点
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator float(ObfuscatedByte data)
        {
            return data.Value;
        }

        /// <summary>
        /// 转换double到ObfuscatedByte (高精度转为低精度 要求显示强转)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedByte(double data)
        {
            return new ObfuscatedByte(Convert.ToByte(data));
        }

        /// <summary>
        /// 转换ObfuscatedByte到double
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator double(ObfuscatedByte data)
        {
            return data.Value;
        }

        /* 重载操作符 == != > < >= <= */
        public static bool operator ==(ObfuscatedByte lhs, ObfuscatedByte rhs)
        {
            return lhs.Value == rhs.Value;
        }

        public static bool operator !=(ObfuscatedByte lhs, ObfuscatedByte rhs)
        {
            return lhs.Value != rhs.Value;
        }

        public static bool operator >(ObfuscatedByte lhs, ObfuscatedByte rhs)
        {
            return lhs.Value > rhs.Value;
        }

        public static bool operator <(ObfuscatedByte lhs, ObfuscatedByte rhs)
        {
            return lhs.Value < rhs.Value;
        }

        public static bool operator >=(ObfuscatedByte lhs, ObfuscatedByte rhs)
        {
            return lhs.Value >= rhs.Value;
        }

        public static bool operator <=(ObfuscatedByte lhs, ObfuscatedByte rhs)
        {
            return lhs.Value <= rhs.Value;
        }

        /* 重载操作符 ++ -- + - * / */
        public static ObfuscatedByte operator ++(ObfuscatedByte data)
        {
            data.Value++;
            return data;
        }

        public static ObfuscatedByte operator --(ObfuscatedByte data)
        {
            data.Value--;
            return data;
        }

        public static ObfuscatedByte operator +(ObfuscatedByte lhs, ObfuscatedByte rhs)
        {
            return new ObfuscatedByte(Convert.ToByte(lhs.Value + rhs.Value));
        }

        public static ObfuscatedByte operator -(ObfuscatedByte lhs, ObfuscatedByte rhs)
        {
            return new ObfuscatedByte(Convert.ToByte(lhs.Value - rhs.Value));
        }

        public static ObfuscatedByte operator *(ObfuscatedByte lhs, ObfuscatedByte rhs)
        {
            return new ObfuscatedByte(Convert.ToByte(lhs.Value * rhs.Value));
        }

        public static ObfuscatedByte operator /(ObfuscatedByte lhs, ObfuscatedByte rhs)
        {
            return new ObfuscatedByte(Convert.ToByte(lhs.Value / rhs.Value));
        }

        public static ObfuscatedByte operator %(ObfuscatedByte lhs, ObfuscatedByte rhs)
        {
            return new ObfuscatedByte(Convert.ToByte(lhs.Value % rhs.Value));
        }

        public int CompareTo(object value)
        {
            if (value == null) return 1;

            if (value is byte)
            {
                byte i = (byte)value;
                if (Value < i) return -1;
                if (Value > i) return 1;
                return 0;
            }

            if (value is ObfuscatedByte)
            {
                ObfuscatedByte i = (ObfuscatedByte)value;
                if (Value < i.Value) return -1;
                if (Value > i.Value) return 1;
                return 0;
            }

            throw new ArgumentException();
        }

        public int CompareTo(byte value)
        {
            if (this.Value < value) return -1;
            if (this.Value > value) return 1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObfuscatedByte)
                return this.Value == ((ObfuscatedByte)obj).Value;

            if (obj is byte)
                return this.Value == (byte)obj;

            return false;
        }

        public bool Equals(byte obj)
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
            return TypeCode.Byte;
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
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, "Can't convert {0} to {1}.", "byte", "DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            return Convert.ChangeType(this.Value, type);
        }
    }
}
