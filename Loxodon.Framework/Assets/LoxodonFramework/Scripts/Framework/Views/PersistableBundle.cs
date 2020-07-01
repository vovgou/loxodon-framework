//using System;
//using UnityEngine;
//#if NETFX_CORE
//using System.Reflection;
//#endif

//namespace Loxodon.Framework.Views
//{
//    [Serializable]
//    public class PersistableBundle : Bundle, ISerializationCallbackReceiver
//    {
//        public PersistableBundle() : base()
//        {
//        }

//        public PersistableBundle(IBundle bundle) : base(bundle)
//        {
//        }

//        protected override bool IsValidType(object value)
//        {
//#if NETFX_CORE
//            return (value == null) || (value.GetType().GetTypeInfo().IsValueType) || 
//                (value.GetType().GetTypeInfo().GetCustomAttribute(typeof(SerializableAttribute), false) != null);
//#else
//            return (value == null) || (value.GetType().IsValueType) ||
//                (value.GetType().GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0);
//#endif
//        }

//        public void OnAfterDeserialize()
//        {
//        }

//        public void OnBeforeSerialize()
//        {
//        }
//    }
//}
