using System;
using System.Globalization;

namespace Loxodon.Framework.Obfuscation
{

    [Serializable]
    public struct ObfuscatedInt : IComparable, IConvertible, IComparable<int>, IEquatable<int>
    {
        private static int sequence = (int)DateTime.Now.Ticks;

        private int seed;
        private int data;
        private int check;

        public ObfuscatedInt(int value)
            : this(value, sequence++)
        {
        }

        public ObfuscatedInt(int value, int seed)
        {
            this.seed = seed;
            this.data = 0;
            this.check = 0;
            Value = value;
        }

        internal int Value
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

        public static implicit operator ObfuscatedInt(byte data)
        {
            return new ObfuscatedInt(data);
        }

        public static explicit operator byte(ObfuscatedInt data)
        {
            return Convert.ToByte(data.Value);
        }

        public static implicit operator ObfuscatedInt(short data)
        {
            return new ObfuscatedInt(data);
        }

        public static explicit operator short(ObfuscatedInt data)
        {
            return Convert.ToInt16(data.Value);
        }

        /// <summary>
        /// 隐式转换int到ObfuscatedInt
        /// 如：ObfuscatedInt i=2;
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator ObfuscatedInt(int data)
        {
            return new ObfuscatedInt(data);
        }

        /// <summary>
        /// 隐式转换ObfuscatedInt 到 int
        /// 如：int i=new ObfuscatedInt(20);
        /// 当然显示转换也支持，如下
        /// 如：ObfuscatedInt i=new ObfuscatedInt(20); int n= (int)i;
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator int(ObfuscatedInt data)
        {
            return data.Value;
        }

        /// <summary>
        /// 转换long到ObfuscatedInt（显示）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedInt(long data)
        {
            return new ObfuscatedInt(Convert.ToInt32(data));
        }

        /// <summary>
        /// 转换ObfuscatedInt 到 long(隐式)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator long(ObfuscatedInt data)
        {
            return data.Value;
        }

        /// <summary>
        /// 显示转换浮点到ObfuscatedInt (高精度转为低精度 要求显示强转)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedInt(float data)
        {
            return new ObfuscatedInt(Convert.ToInt32(data));
        }

        /// <summary>
        /// 转换ObfuscatedInt 到浮点
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator float(ObfuscatedInt data)
        {
            return data.Value;
        }

        /// <summary>
        /// 转换double到ObfuscatedInt (高精度转为低精度 要求显示强转)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedInt(double data)
        {
            return new ObfuscatedInt(Convert.ToInt32(data));
        }

        /// <summary>
        /// 转换ObfuscatedInt到double
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator double(ObfuscatedInt data)
        {
            return data.Value;
        }

        /* 重载操作符 == != > < >= <= */
        public static bool operator ==(ObfuscatedInt lhs, ObfuscatedInt rhs)
        {
            return lhs.Value == rhs.Value;
        }

        public static bool operator !=(ObfuscatedInt lhs, ObfuscatedInt rhs)
        {
            return lhs.Value != rhs.Value;
        }

        public static bool operator >(ObfuscatedInt lhs, ObfuscatedInt rhs)
        {
            return lhs.Value > rhs.Value;
        }

        public static bool operator <(ObfuscatedInt lhs, ObfuscatedInt rhs)
        {
            return lhs.Value < rhs.Value;
        }

        public static bool operator >=(ObfuscatedInt lhs, ObfuscatedInt rhs)
        {
            return lhs.Value >= rhs.Value;
        }

        public static bool operator <=(ObfuscatedInt lhs, ObfuscatedInt rhs)
        {
            return lhs.Value <= rhs.Value;
        }

        /* 重载操作符 ++ -- + - * / */
        public static ObfuscatedInt operator ++(ObfuscatedInt data)
        {
            return new ObfuscatedInt(data.Value + 1);
        }

        public static ObfuscatedInt operator --(ObfuscatedInt data)
        {
            return new ObfuscatedInt(data.Value - 1);
        }

        public static ObfuscatedInt operator +(ObfuscatedInt lhs, ObfuscatedInt rhs)
        {
            return new ObfuscatedInt(lhs.Value + rhs.Value);
        }

        public static ObfuscatedInt operator -(ObfuscatedInt lhs, ObfuscatedInt rhs)
        {
            return new ObfuscatedInt(lhs.Value - rhs.Value);
        }

        public static ObfuscatedInt operator *(ObfuscatedInt lhs, ObfuscatedInt rhs)
        {
            return new ObfuscatedInt(lhs.Value * rhs.Value);
        }

        public static ObfuscatedInt operator /(ObfuscatedInt lhs, ObfuscatedInt rhs)
        {
            return new ObfuscatedInt(lhs.Value / rhs.Value);
        }

        public static ObfuscatedInt operator %(ObfuscatedInt lhs, ObfuscatedInt rhs)
        {
            return new ObfuscatedInt(lhs.Value % rhs.Value);
        }

        public int CompareTo(object value)
        {
            if (value == null) return 1;

            if (value is int)
            {
                int i = (int)value;
                if (Value < i) return -1;
                if (Value > i) return 1;
                return 0;
            }

            if (value is ObfuscatedInt)
            {
                ObfuscatedInt i = (ObfuscatedInt)value;
                if (Value < i.Value) return -1;
                if (Value > i.Value) return 1;
                return 0;
            }

            throw new ArgumentException();
        }

        public int CompareTo(int value)
        {
            if (this.Value < value) return -1;
            if (this.Value > value) return 1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObfuscatedInt)
                return this.Value == ((ObfuscatedInt)obj).Value;

            if (obj is int)
                return this.Value == (int)obj;

            return false;
        }

        public bool Equals(int obj)
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
            return TypeCode.Int32;
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
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, "Can't convert {0} to {1}.", "int", "DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            return Convert.ChangeType(this.Value, type);
        }
    }
}


