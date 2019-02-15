using System;
using System.Reflection;
using System.Collections.Generic;

namespace Loxodon.Framework.Binding.Reflection
{
    public class ProxyFactory : IProxyFactory, IProxyRegistry
    {
        public static readonly ProxyFactory Default = new ProxyFactory();

        private readonly Dictionary<Type, ProxyType> types = new Dictionary<Type, ProxyType>();

        protected ProxyFactory()
        {
        }

        public IProxyType Create(Type type)
        {
            return GetProxyType(type);
        }

        public void Register(IProxyFieldInfo info)
        {
            if (info == null)
                return;

            ProxyType proxyType = GetProxyType(info.DeclaringType);
            proxyType.Add(info);
        }

        public void Register(IProxyPropertyInfo info)
        {
            if (info == null)
                return;

            ProxyType proxyType = GetProxyType(info.DeclaringType);
            proxyType.Add(info);
        }

        public void Register(IProxyMethodInfo info)
        {
            if (info == null)
                return;

            ProxyType proxyType = GetProxyType(info.DeclaringType);
            proxyType.Add(info);
        }

        internal ProxyType GetProxyType(Type type)
        {
            ProxyType proxyType;
            if (types.TryGetValue(type, out proxyType))
                return proxyType;

            proxyType = new ProxyType(type, this);
            types[type] = proxyType;
            return proxyType;
        }

        internal IProxyFieldInfo Create(FieldInfo info)
        {
#if UNITY_IOS
            return new ProxyFieldInfo(info);
#else
            try
            {
                return (IProxyFieldInfo)Activator.CreateInstance(typeof(ProxyFieldInfo<,>).MakeGenericType(info.DeclaringType, info.FieldType), info);
            }
            catch (Exception)
            {
                return new ProxyFieldInfo(info);
            }
#endif
        }

        internal IProxyMethodInfo Create(MethodInfo info)
        {
#if UNITY_IOS
            return new ProxyMethodInfo(info);
#else
            try
            {
                Type returnType = info.ReturnType;
                ParameterInfo[] parameters = info.GetParameters();
                Type type = info.DeclaringType;
                if (typeof(void).Equals(returnType))
                {
                    if (info.IsStatic)
                    {
                        if (parameters == null || parameters.Length == 0)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyActionInfo<>).MakeGenericType(type), info);
                        }
                        else if (parameters.Length == 1)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyActionInfo<,>).MakeGenericType(type, parameters[0].ParameterType), info);
                        }
                        else if (parameters.Length == 2)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyActionInfo<,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType), info);
                        }
                        else if (parameters.Length == 3)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyActionInfo<,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType), info);
                        }
                    }
                    else
                    {
                        if (parameters == null || parameters.Length == 0)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyActionInfo<>).MakeGenericType(type), info);
                        }
                        else if (parameters.Length == 1)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyActionInfo<,>).MakeGenericType(type, parameters[0].ParameterType), info);
                        }
                        else if (parameters.Length == 2)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyActionInfo<,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType), info);
                        }
                        else if (parameters.Length == 3)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyActionInfo<,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType), info);
                        }
                    }

                    return new ProxyMethodInfo(info);
                }
                else
                {
                    if (info.IsStatic)
                    {
                        if (parameters == null || parameters.Length == 0)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyFuncInfo<,>).MakeGenericType(type, returnType), info);
                        }
                        else if (parameters.Length == 1)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyFuncInfo<,,>).MakeGenericType(type, parameters[0].ParameterType, returnType), info);
                        }
                        else if (parameters.Length == 2)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyFuncInfo<,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, returnType), info);
                        }
                        else if (parameters.Length == 3)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyFuncInfo<,,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, returnType), info);
                        }
                    }
                    else
                    {
                        if (parameters == null || parameters.Length == 0)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyFuncInfo<,>).MakeGenericType(type, returnType), info);
                        }
                        else if (parameters.Length == 1)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyFuncInfo<,,>).MakeGenericType(type, parameters[0].ParameterType, returnType), info);
                        }
                        else if (parameters.Length == 2)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyFuncInfo<,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, returnType), info);
                        }
                        else if (parameters.Length == 3)
                        {
                            return (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyFuncInfo<,,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, returnType), info);
                        }
                    }

                    return new ProxyMethodInfo(info);
                }
            }
            catch (Exception)
            {
                return new ProxyMethodInfo(info);
            }
#endif
        }

        internal IProxyPropertyInfo Create(PropertyInfo info)
        {
#if UNITY_IOS
            return new ProxyPropertyInfo(info);
#else            
            try
            {
                Type type = info.DeclaringType;
                if (info.IsStatic())
                {
                    ParameterInfo[] parameters = info.GetIndexParameters();
                    if (parameters != null && parameters.Length > 0)
                        throw new NotSupportedException();

                    return (IProxyPropertyInfo)Activator.CreateInstance(typeof(StaticProxyPropertyInfo<,>).MakeGenericType(type, info.PropertyType), info);
                }
                else
                {
                    ParameterInfo[] parameters = info.GetIndexParameters();
                    if (parameters != null && parameters.Length == 1)
                        return (IProxyPropertyInfo)Activator.CreateInstance(typeof(ProxyPropertyInfo<,,>).MakeGenericType(type, parameters[0].ParameterType, info.PropertyType), info);

                    return (IProxyPropertyInfo)Activator.CreateInstance(typeof(ProxyPropertyInfo<,>).MakeGenericType(type, info.PropertyType), info);
                }
            }
            catch (Exception)
            {
                return new ProxyPropertyInfo(info);
            }
#endif
        }
    }
}
