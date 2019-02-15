using System;
using System.Reflection;

using UnityEngine.Events;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public static class UnityEventExtentions
    {
        public static void AddListener(this UnityEventBase unity, Delegate listener)
        {
            MethodInfo add = unity.GetType().GetMethod("AddListener");
            add.Invoke(unity, new object[] { listener });
        }

        public static void RemoveListener(this UnityEventBase unity, Delegate listener)
        {
            MethodInfo add = unity.GetType().GetMethod("RemoveListener");
            add.Invoke(unity, new object[] { listener });
        }

        public static Type GetListenerType(this UnityEventBase unity)
        {
            MethodInfo info = unity.GetType().GetMethod("AddListener");
            if (info == null)
                return null;

            ParameterInfo[] parameters = info.GetParameters();
            if (parameters == null || parameters.Length <= 0)
                return null;

            return parameters[0].ParameterType;
        }

        public static Type GetInvokeParameterType(this UnityEventBase unity)
        {
            MethodInfo info = unity.GetType().GetMethod("Invoke");
            if (info == null)
                return null;

            ParameterInfo[] parameters = info.GetParameters();
            if (parameters == null || parameters.Length <= 0)
                return null;

            return parameters[0].ParameterType;
        }
    }
}
