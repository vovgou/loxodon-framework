using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Loxodon.Framework.Localizations
{
    public class RectTypeConverter : ITypeConverter
    {
        public bool Support(string typeName)
        {
            switch (typeName)
            {
                case "rect":
                    return true;
                default:
                    return false;
            }
        }

        public Type GetType(string typeName)
        {
            switch (typeName)
            {
                case "rect":
                    return typeof(Rect);
                default:
                    throw new NotSupportedException();
            }
        }

        public object Convert(Type type, object value)
        {
            if (type == null)
                throw new NotSupportedException();

            try
            {
                var val = Regex.Replace(((string)value).Trim(), @"(^\()|(\)$)", "");
                string[] s = val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (s.Length == 4)
                    return new Rect(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3]));

            }
            catch (Exception)
            {
            }
            throw new FormatException(string.Format("The '{0}' is illegal Rect.", value));
        }
    }
}
