using System;
using UnityEngine;

namespace Loxodon.Framework.Localizations
{
    public class ColorTypeConverter : ITypeConverter
    {
        public bool Support(string typeName)
        {
            switch (typeName)
            {
                case "color":
                    return true;
                default:
                    return false;
            }
        }

        public Type GetType(string typeName)
        {
            switch (typeName)
            {
                case "color":
                    return typeof(Color);
                default:
                    throw new NotSupportedException();
            }
        }

        public object Convert(Type type, object value)
        {
            if (type == null)
                throw new NotSupportedException();

            Color color;
            if (ColorUtility.TryParseHtmlString((string)value, out color))
                return color;

            throw new FormatException(string.Format("The '{0}' is illegal Color.", value));
        }
    }
}
