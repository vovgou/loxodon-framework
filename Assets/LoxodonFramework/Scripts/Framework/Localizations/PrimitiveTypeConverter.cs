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
            return System.Convert.ChangeType(value, type);
        }
    }
}
