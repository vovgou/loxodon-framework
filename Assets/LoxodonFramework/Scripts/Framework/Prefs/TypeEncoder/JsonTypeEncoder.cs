using System;
using System.Collections;
using UnityEngine;

#if NETFX_CORE
using System.Reflection;
#endif

namespace Loxodon.Framework.Prefs
{
    public class JsonTypeEncoder : ITypeEncoder
    {
        private int priority = -1000;

        public int Priority
        {
            get { return this.priority; }
            set { this.priority = value; }
        }

        public bool IsSupport(Type type)
        {
            if (typeof(IList).IsAssignableFrom(type) || typeof(IDictionary).IsAssignableFrom(type))
                return false;

#if NETFX_CORE
            if (type.GetTypeInfo().IsPrimitive)
#else
            if (type.IsPrimitive)
#endif
                return false;

            return true;
        }

        public string Encode(object value)
        {
            try
            {
                return JsonUtility.ToJson(value);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("", e);
            }
        }

        public object Decode(Type type, string value)
        {
            try
            {
                return JsonUtility.FromJson(value, type);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("", e);
            }
        }
    }
}
