using System;
using System.Reflection;
using System.Collections.Generic;

namespace Loxodon.Framework.Binding.Reflection
{
    public class ProxyType : IProxyType
    {
        private readonly Dictionary<FieldInfo, IProxyFieldInfo> fields = new Dictionary<FieldInfo, IProxyFieldInfo>();

        private readonly Dictionary<PropertyInfo, IProxyPropertyInfo> properties = new Dictionary<PropertyInfo, IProxyPropertyInfo>();

        private readonly Dictionary<MethodInfo, IProxyMethodInfo> methods = new Dictionary<MethodInfo, IProxyMethodInfo>();

        private readonly Type type;
        private readonly ProxyFactory factory;
        internal ProxyType(Type type, ProxyFactory factory)
        {
            this.type = type;
            this.factory = factory;
        }

        public Type Type { get { return this.type; } }

        public void Add(IProxyFieldInfo info)
        {
            if (info == null || info.FieldInfo == null)
                return;

            fields[info.FieldInfo] = info;
        }
        public void Add(IProxyPropertyInfo info)
        {
            if (info == null || info.PropertyInfo == null)
                return;

            properties[info.PropertyInfo] = info;
        }
        public void Add(IProxyMethodInfo info)
        {
            if (info == null || info.MethodInfo == null)
                return;

            methods[info.MethodInfo] = info;
        }

        public void Remove(IProxyFieldInfo info)
        {
            if (info == null || info.FieldInfo == null)
                return;

            fields.Remove(info.FieldInfo);
        }

        public void Remove(IProxyPropertyInfo info)
        {
            if (info == null || info.PropertyInfo == null)
                return;

            properties.Remove(info.PropertyInfo);
        }

        public void Remove(IProxyMethodInfo info)
        {
            if (info == null || info.MethodInfo == null)
                return;

            methods.Remove(info.MethodInfo);
        }

        public virtual IProxyFieldInfo GetField(string name)
        {
            FieldInfo field = this.type.GetField(name);
            return GetField(field);
        }

        public virtual IProxyFieldInfo GetField(FieldInfo field)
        {
            if (field == null || !field.DeclaringType.IsAssignableFrom(this.Type))
                return null;

            IProxyFieldInfo proxyField;
            if (this.fields.TryGetValue(field, out proxyField))
                return proxyField;

            proxyField = this.factory.Create(field);
            fields[field] = proxyField;
            return proxyField;
        }

        public virtual IProxyPropertyInfo GetProperty(string name)
        {
            PropertyInfo property = this.type.GetProperty(name);
            return GetProperty(property);
        }

        public virtual IProxyPropertyInfo GetProperty(PropertyInfo property)
        {
            if (property == null || !property.DeclaringType.IsAssignableFrom(this.Type))
                return null;

            IProxyPropertyInfo proxyProperty;
            if (this.properties.TryGetValue(property, out proxyProperty))
                return proxyProperty;

            proxyProperty = this.factory.Create(property);
            properties[property] = proxyProperty;
            return proxyProperty;
        }

        public virtual IProxyMethodInfo GetMethod(string name)
        {
            MethodInfo method = this.type.GetMethod(name);
            return GetMethod(method);
        }

        public virtual IProxyMethodInfo GetMethod(string name, Type[] parameterTypes)
        {
            MethodInfo method = this.type.GetMethod(name, parameterTypes);
            return GetMethod(method);
        }

        public virtual IProxyMethodInfo GetMethod(MethodInfo method)
        {
            if (method == null || !method.DeclaringType.IsAssignableFrom(this.Type))
                return null;

            IProxyMethodInfo proxyMethod;
            if (this.methods.TryGetValue(method, out proxyMethod))
                return proxyMethod;

            proxyMethod = this.factory.Create(method);
            methods[method] = proxyMethod;
            return proxyMethod;
        }
    }
}
