using System;

namespace Loxodon.Framework.Prefs
{
    public class PrimitiveTypeEncoder : ITypeEncoder
    {
        private int priority = 1000;

        public int Priority
        {
            get { return this.priority; }
            set { this.priority = value; }
        }

        public bool IsSupport(Type type)
        {
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif

            switch (typeCode)
            {
                case TypeCode.String:
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    return true;
            }

            return false;
        }

        public string Encode(object value)
        {
            TypeCode typeCode = Convert.GetTypeCode(value);
            switch (typeCode)
            {
                case TypeCode.String:
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    return Convert.ToString(value);
            }

            throw new NotSupportedException();
        }

        public object Decode(Type type, string input)
        {
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif

            switch (typeCode)
            {
                case TypeCode.String:
                    return input;
                case TypeCode.Boolean:
                    return Convert.ToBoolean(input);
                case TypeCode.SByte:
                    return Convert.ToSByte(input);
                case TypeCode.Byte:
                    return Convert.ToByte(input);
                case TypeCode.Int16:
                    return Convert.ToInt16(input);
                case TypeCode.UInt16:
                    return Convert.ToUInt16(input);
                case TypeCode.Int32:
                    return Convert.ToInt32(input);
                case TypeCode.UInt32:
                    return Convert.ToUInt32(input);
                case TypeCode.Int64:
                    return Convert.ToInt64(input);
                case TypeCode.UInt64:
                    return Convert.ToUInt64(input);
                case TypeCode.Char:
                    return Convert.ToChar(input);
                case TypeCode.Single:
                    return Convert.ToSingle(input);
                case TypeCode.Double:
                    return Convert.ToDouble(input);
                case TypeCode.Decimal:
                    return Convert.ToDecimal(input);
                case TypeCode.DateTime:
                    return Convert.ToDateTime(input);
            }

            throw new NotSupportedException();
        }
    }
}
