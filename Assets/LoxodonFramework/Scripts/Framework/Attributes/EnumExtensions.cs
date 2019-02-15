using System;
using System.Reflection;
#if NETFX_CORE
using System.Collections.Generic;
#endif

namespace Loxodon.Framework.Attributes
{
    public static class EnumExtensions
    {
        public static string GetRemark(this Enum e)
        {
            Type type = e.GetType();
            FieldInfo fieldInfo = type.GetField(e.ToString());
            if (fieldInfo == null)
                return string.Empty;

#if NETFX_CORE
            IEnumerable<Attribute> attrs = fieldInfo.GetCustomAttributes(typeof(RemarkAttribute), false);
#else
            object[] attrs = fieldInfo.GetCustomAttributes(typeof(RemarkAttribute), false);
#endif
            foreach (RemarkAttribute attr in attrs)
                return attr.Remark;

            return string.Empty;
        }
    }    
}
