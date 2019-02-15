using System;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Loxodon.Framework.Localizations
{
    public class VectorTypeConverter : ITypeConverter
    {
        public bool Support(string typeName)
        {
            switch (typeName)
            {
                case "vector2":
                case "vector3":
                case "vector4":
                    return true;
                default:
                    return false;
            }
        }

        public Type GetType(string typeName)
        {
            switch (typeName)
            {
                case "vector2":
                    return typeof(Vector2);
                case "vector3":
                    return typeof(Vector3);
                case "vector4":
                    return typeof(Vector4);
                default:
                    throw new NotSupportedException();
            }
        }

        public object Convert(Type type, object value)
        {
            if (type == null)
                throw new NotSupportedException();

            var val = Regex.Replace(((string)value).Trim(), @"(^\()|(\)$)", "");
            if (type.Equals(typeof(Vector2)))
            {
                try
                {
                    string[] s = val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 2)
                        return new Vector2(float.Parse(s[0]), float.Parse(s[1]));

                }
                catch (Exception)
                {
                }
                throw new FormatException(string.Format("The '{0}' is illegal Vector2.", value));
            }

            if (type.Equals(typeof(Vector3)))
            {
                try
                {
                    string[] s = val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 3)
                        return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));

                }
                catch (Exception)
                {
                }
                throw new FormatException(string.Format("The '{0}' is illegal Vector3.", value));
            }

            if (type.Equals(typeof(Vector4)))
            {
                try
                {
                    string[] s = val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 4)
                        return new Vector4(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3]));

                }
                catch (Exception)
                {
                }

                throw new FormatException(string.Format("The '{0}' is illegal Vector4.", value));
            }

            throw new NotSupportedException();
        }
    }
}
