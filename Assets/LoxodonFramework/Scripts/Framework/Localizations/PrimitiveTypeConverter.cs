using System;

namespace Loxodon.Framework.Localizations
{
    public class PrimitiveTypeConverter : ITypeConverter
    {
        public bool Support(string typeName)
        {
            switch (typeName)
            {
                case "string":
                case "boolean":
                case "sbyte":
                case "byte":
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                case "char":
                case "float":
                case "double":
                case "decimal":
                case "datetime":
                    return true;
                default:
                    return false;
            }
        }

        public Type GetType(string typeName)
        {
            switch (typeName)
            {
                case "string":
                    return typeof(string);
                case "boolean":
                    return typeof(bool);
                case "sbyte":
                    return typeof(sbyte);
                case "byte":
                    return typeof(byte);
                case "short":
                    return typeof(short);
                case "ushort":
                    return typeof(ushort);
                case "int":
                    return typeof(int);
                case "uint":
                    return typeof(uint);
                case "long":
                    return typeof(long);
                case "ulong":
                    return typeof(ulong);
                case "char":
                    return typeof(char);
                case "float":
                    return typeof(float);
                case "double":
                    return typeof(double);
                case "decimal":
                    return typeof(decimal);
                case "datetime":
                    return typeof(DateTime);
                default:
                    throw new NotSupportedException();
            }
        }

        public object Convert(Type type, object value)
        {
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif
            string text = (string)value;
            switch (typeCode)
            {
                case TypeCode.String:
                    return text;
                case TypeCode.Boolean:
                    return bool.Parse(text);
                case TypeCode.SByte:
                    return sbyte.Parse(text);
                case TypeCode.Byte:
                    return byte.Parse(text);
                case TypeCode.Int16:
                    return short.Parse(text);
                case TypeCode.UInt16:
                    return ushort.Parse(text);
                case TypeCode.Int32:
                    return int.Parse(text);
                case TypeCode.UInt32:
                    return uint.Parse(text);
                case TypeCode.Int64:
                    return long.Parse(text);
                case TypeCode.UInt64:
                    return ulong.Parse(text);
                case TypeCode.Char:
                    return char.Parse(text);
                case TypeCode.Single:
                    return float.Parse(text);
                case TypeCode.Double:
                    return double.Parse(text);
                case TypeCode.Decimal:
                    return decimal.Parse(text);
                case TypeCode.DateTime:
                    return DateTime.Parse(text);
            }

            throw new NotSupportedException();
        }
    }
}
