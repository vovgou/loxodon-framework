using System;
using System.Globalization;

namespace Loxodon.Framework.Obfuscation
{
    [Serializable]
    public struct ObfuscatedDouble : IComparable, IConvertible, IComparable<double>, IEquatable<double>
    {
        private static long sequence = DateTime.Now.Ticks;

        private long seed;
        private long data; //数据  转换为long存储 进行运算时不会丢失精度
        private long check;

        static unsafe long ConvertValue(double value)
        {
            double* ptr = &value;
            return *((long*)ptr);
        }

        static unsafe double ConvertValue(long value)
        {
            long* ptr = &value;
            return *((double*)ptr);
        }

        //static long ConvertValue(double value)
        //{
        //    return BitConverter.ToInt64(BitConverter.GetBytes(value), 0);
        //}

        //static double ConvertValue(long value)
        //{
        //    return BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
        //}

        public ObfuscatedDouble(double value)
            : this(value, sequence++)
        {
        }

        public ObfuscatedDouble(double value, long seed)
        {
            this.seed = seed;
            this.data = 0;
            this.check = 0;
            Value = value;
        }

        internal double Value
        {
            get
            {
                var v = data ^ seed;
                if (((seed >> 8) ^ v) != check)
                    throw new Exception();
                return ConvertValue(v);
            }
            set
            {
                var v = ConvertValue(value);
                data = v ^ seed;
                check = (seed >> 8) ^ v;
            }
        }

        public static implicit operator ObfuscatedDouble(byte data)
        {
            return new ObfuscatedDouble(data);
        }

        public static explicit operator byte(ObfuscatedDouble data)
        {
            return Convert.ToByte(data.Value);
        }

        public static implicit operator ObfuscatedDouble(short data)
        {
            return new ObfuscatedDouble(data);
        }

        public static explicit operator short(ObfuscatedDouble data)
        {
            return Convert.ToInt16(data.Value);
        }

        /// <summary>
        /// 隐式转换long到ObfuscatedDouble
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator ObfuscatedDouble(long data)
        {
            return new ObfuscatedDouble(data);
        }

        /// <summary>
        /// 隐式转换ObfuscatedDouble 到 long
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator long(ObfuscatedDouble data)
        {
            return Convert.ToInt64(data.Value);
        }

        /// <summary>
        /// 转换int到ObfuscatedDouble
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator ObfuscatedDouble(int data)
        {
            return new ObfuscatedDouble(data);
        }

        /// <summary>
        /// 显示转换ObfuscatedDouble 到 int
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator int(ObfuscatedDouble data)
        {
            return Convert.ToInt32(data.Value);
        }

        /// <summary>
        /// 转换浮点到ObfuscatedDouble
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator ObfuscatedDouble(float data)
        {
            return new ObfuscatedDouble(data);
        }

        /// <summary>
        /// 转换ObfuscatedDouble 到浮点
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator float(ObfuscatedDouble data)
        {
            return Convert.ToSingle(data.Value);
        }

        /// <summary>
        /// 转换double到ObfuscatedDouble
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator ObfuscatedDouble(double data)
        {
            return new ObfuscatedDouble(data);
        }

        /// <summary>
        /// 转换ObfuscatedDouble到double(隐式)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator double(ObfuscatedDouble data)
        {
            return data.Value;
        }

        /* 重载操作符 == != > < >= <= */
        public static bool operator ==(ObfuscatedDouble lhs, ObfuscatedDouble rhs)
        {
            return lhs.Value == rhs.Value;
        }

        public static bool operator !=(ObfuscatedDouble lhs, ObfuscatedDouble rhs)
        {
            return lhs.Value != rhs.Value;
        }

        public static bool operator >(ObfuscatedDouble lhs, ObfuscatedDouble rhs)
        {
            return lhs.Value > rhs.Value;
        }

        public static bool operator <(ObfuscatedDouble lhs, ObfuscatedDouble rhs)
        {
            return lhs.Value < rhs.Value;
        }

        public static bool operator >=(ObfuscatedDouble lhs, ObfuscatedDouble rhs)
        {
            return lhs.Value >= rhs.Value;
        }

        public static bool operator <=(ObfuscatedDouble lhs, ObfuscatedDouble rhs)
        {
            return lhs.Value <= rhs.Value;
        }

        /* 重载操作符 ++ -- + - * / */
        public static ObfuscatedDouble operator ++(ObfuscatedDouble data)
        {
            return new ObfuscatedDouble(data.Value + 1);
        }

        public static ObfuscatedDouble operator --(ObfuscatedDouble data)
        {
            return new ObfuscatedDouble(data.Value - 1);
        }

        public static ObfuscatedDouble operator +(ObfuscatedDouble lhs, ObfuscatedDouble rhs)
        {
            return new ObfuscatedDouble(lhs.Value + rhs.Value);
        }

        public static ObfuscatedDouble operator -(ObfuscatedDouble lhs, ObfuscatedDouble rhs)
        {
            return new ObfuscatedDouble(lhs.Value - rhs.Value);
        }

        public static ObfuscatedDouble operator *(ObfuscatedDouble lhs, ObfuscatedDouble rhs)
        {
            return new ObfuscatedDouble(lhs.Value * rhs.Value);
        }

        public static ObfuscatedDouble operator /(ObfuscatedDouble lhs, ObfuscatedDouble rhs)
        {
            return new ObfuscatedDouble(lhs.Value / rhs.Value);
        }

        public static ObfuscatedDouble operator %(ObfuscatedDouble lhs, ObfuscatedDouble rhs)
        {
            return new ObfuscatedDouble(lhs.Value % rhs.Value);
        }

        public int CompareTo(object value)
        {
            if (value == null) return 1;

            if (value is double)
            {
                double i = (double)value;
                if (Value < i) return -1;
                if (Value > i) return 1;
                return 0;
            }

            if (value is ObfuscatedDouble)
            {
                ObfuscatedDouble i = (ObfuscatedDouble)value;
                if (Value < i.Value) return -1;
                if (Value > i.Value) return 1;
                return 0;
            }

            throw new ArgumentException();
        }

        public int CompareTo(double value)
        {
            if (this.Value < value) return -1;
            if (this.Value > value) return 1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObfuscatedDouble)
                return this.Value == ((ObfuscatedDouble)obj).Value;

            if (obj is double)
                return this.Value == (double)obj;

            return false;
        }

        public bool Equals(double obj)
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
            return TypeCode.Double;
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
            return this.Value;
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(this.Value);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, "Can't convert {0} to {1}.", "double", "DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            return Convert.ChangeType(this.Value, type);
        }
    }
}


