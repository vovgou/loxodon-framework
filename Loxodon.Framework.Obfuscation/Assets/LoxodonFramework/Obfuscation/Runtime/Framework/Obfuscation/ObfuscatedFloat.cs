using System;
using System.Globalization;

namespace Loxodon.Framework.Obfuscation
{
    [Serializable]
    public struct ObfuscatedFloat : IComparable, IConvertible, IComparable<float>, IEquatable<float>
    {
        private static int sequence = (int)DateTime.Now.Ticks;

        private int seed;
        private int data; //数据 转换为int存储 进行运算时不会丢失精度
        private int check;

        static unsafe int ConvertValue(float value)
        {
            float* ptr = &value;
            return *((int*)ptr);
        }

        static unsafe float ConvertValue(int value)
        {
            int* ptr = &value;
            return *((float*)ptr);
        }

        //static int ConvertValue(float value)
        //{
        //    return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        //}

        //static float ConvertValue(int value)
        //{
        //    return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        //}

        public ObfuscatedFloat(float value)
            : this(value, sequence++)
        {
        }

        public ObfuscatedFloat(float value, int seed)
        {
            this.seed = seed;
            this.data = 0;
            this.check = 0;
            Value = value;
        }

        internal float Value
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

        public static implicit operator ObfuscatedFloat(byte data)
        {
            return new ObfuscatedFloat(data);
        }

        public static explicit operator byte(ObfuscatedFloat data)
        {
            return Convert.ToByte(data.Value);
        }

        public static implicit operator ObfuscatedFloat(short data)
        {
            return new ObfuscatedFloat(data);
        }

        public static explicit operator short(ObfuscatedFloat data)
        {
            return Convert.ToInt16(data.Value);
        }

        /// <summary>
        /// 隐式转换int到ObfuscatedFloat
        /// 如：ObfuscatedFloat i=2;
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator ObfuscatedFloat(int data)
        {
            return new ObfuscatedFloat(data);
        }

        /// <summary>
        /// 显示转换ObfuscatedFloat 到 int
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator int(ObfuscatedFloat data)
        {
            return Convert.ToInt32(data.Value);
        }

        /// <summary>
        /// 隐式转换long到ObfuscatedFloat
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator ObfuscatedFloat(long data)
        {
            return new ObfuscatedFloat(data);
        }

        /// <summary>
        /// 转换ObfuscatedFloat 到 long(显示)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator long(ObfuscatedFloat data)
        {
            return Convert.ToInt64(data.Value);
        }

        /// <summary>
        /// 显示转换浮点到ObfuscatedFloat (高精度转为低精度 要求显示强转)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator ObfuscatedFloat(float data)
        {
            return new ObfuscatedFloat(data);
        }

        /// <summary>
        /// 转换ObfuscatedFloat 到浮点
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator float(ObfuscatedFloat data)
        {
            return data.Value;
        }

        /// <summary>
        /// 转换double到ObfuscatedFloat (高精度转为低精度 要求显示强转)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator ObfuscatedFloat(double data)
        {
            return new ObfuscatedFloat(Convert.ToSingle(data));
        }

        /// <summary>
        /// 转换ObfuscatedFloat到double
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator double(ObfuscatedFloat data)
        {
            return data.Value;
        }

        /* 重载操作符 == != > < >= <= */
        public static bool operator ==(ObfuscatedFloat lhs, ObfuscatedFloat rhs)
        {
            return lhs.Value == rhs.Value;
        }

        public static bool operator !=(ObfuscatedFloat lhs, ObfuscatedFloat rhs)
        {
            return lhs.Value != rhs.Value;
        }

        public static bool operator >(ObfuscatedFloat lhs, ObfuscatedFloat rhs)
        {
            return lhs.Value > rhs.Value;
        }

        public static bool operator <(ObfuscatedFloat lhs, ObfuscatedFloat rhs)
        {
            return lhs.Value < rhs.Value;
        }

        public static bool operator >=(ObfuscatedFloat lhs, ObfuscatedFloat rhs)
        {
            return lhs.Value >= rhs.Value;
        }

        public static bool operator <=(ObfuscatedFloat lhs, ObfuscatedFloat rhs)
        {
            return lhs.Value <= rhs.Value;
        }

        /* 重载操作符 ++ -- + - * / */
        public static ObfuscatedFloat operator ++(ObfuscatedFloat data)
        {
            return new ObfuscatedFloat(data.Value + 1f);
        }

        public static ObfuscatedFloat operator --(ObfuscatedFloat data)
        {
            return new ObfuscatedFloat(data.Value - 1f);
        }

        public static ObfuscatedFloat operator +(ObfuscatedFloat lhs, ObfuscatedFloat rhs)
        {
            return new ObfuscatedFloat(lhs.Value + rhs.Value);
        }

        public static ObfuscatedFloat operator -(ObfuscatedFloat lhs, ObfuscatedFloat rhs)
        {
            return new ObfuscatedFloat(lhs.Value - rhs.Value);
        }

        public static ObfuscatedFloat operator *(ObfuscatedFloat lhs, ObfuscatedFloat rhs)
        {
            return new ObfuscatedFloat(lhs.Value * rhs.Value);
        }

        public static ObfuscatedFloat operator /(ObfuscatedFloat lhs, ObfuscatedFloat rhs)
        {
            return new ObfuscatedFloat(lhs.Value / rhs.Value);
        }

        public static ObfuscatedFloat operator %(ObfuscatedFloat lhs, ObfuscatedFloat rhs)
        {
            return new ObfuscatedFloat(lhs.Value % rhs.Value);
        }

        public int CompareTo(object value)
        {
            if (value == null) return 1;

            if (value is float)
            {
                float i = (float)value;
                if (Value < i) return -1;
                if (Value > i) return 1;
                return 0;
            }

            if (value is ObfuscatedFloat)
            {
                ObfuscatedFloat i = (ObfuscatedFloat)value;
                if (Value < i.Value) return -1;
                if (Value > i.Value) return 1;
                return 0;
            }

            throw new ArgumentException();
        }


        public int CompareTo(float value)
        {
            if (this.Value < value) return -1;
            if (this.Value > value) return 1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObfuscatedFloat)
                return this.Value == ((ObfuscatedFloat)obj).Value;

            if (obj is float)
                return this.Value == (float)obj;

            return false;
        }

        public bool Equals(float obj)
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
            return TypeCode.Single;
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
            return this.Value;
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
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, "Can't convert {0} to {1}.", "float", "DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            return Convert.ChangeType(this.Value, type);
        }
    }
}


