using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Loxodon.Framework.Binding.Reflection
{
    public class ProxyItemInfo : IProxyItemInfo
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(ProxyItemInfo));

        private readonly bool isValueType;
        protected PropertyInfo propertyInfo;
        protected MethodInfo getMethod;
        protected MethodInfo setMethod;

        public ProxyItemInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.Name.Equals("Item"))
                throw new ArgumentException("The property types do not match!");

            if (!typeof(ICollection).IsAssignableFrom(propertyInfo.DeclaringType))
                throw new ArgumentException("The property types do not match!");

            this.propertyInfo = propertyInfo;
#if NETFX_CORE
            this.isValueType = this.propertyInfo.DeclaringType.GetTypeInfo().IsValueType;
#else
            this.isValueType = this.propertyInfo.DeclaringType.IsValueType;
#endif

            if (this.propertyInfo.CanRead)
                this.getMethod = propertyInfo.GetGetMethod();

            if (this.propertyInfo.CanWrite)
                this.setMethod = propertyInfo.GetSetMethod();
        }

        public bool IsValueType { get { return isValueType; } }

        public Type ValueType { get { return this.propertyInfo.PropertyType; } }

        public Type DeclaringType { get { return this.propertyInfo.DeclaringType; } }

        public string Name { get { return this.propertyInfo.Name; } }

        public bool IsStatic { get { return this.propertyInfo.IsStatic(); } }

        public object GetValue(object target, object key)
        {
            if (target is IList)
            {
                return ((IList)target)[(int)key];
            }

            if (target is IDictionary)
            {
                return ((IDictionary)target)[key];
            }

            if (this.getMethod == null)
                throw new MemberAccessException();

            return this.getMethod.Invoke(target, new object[] { key });
        }

        public void SetValue(object target, object key, object value)
        {
            if (target is IList)
            {
                ((IList)target)[(int)key] = value;
                return;
            }

            if (target is IDictionary)
            {
                ((IDictionary)target)[key] = value;
                return;
            }

            if (this.setMethod == null)
                throw new MemberAccessException();

            this.setMethod.Invoke(target, new object[] { key, value });
        }
    }


    public class ListProxyItemInfo<T, TValue> : ProxyItemInfo, IProxyItemInfo<T, int, TValue> where T : IList<TValue>
    {
        public ListProxyItemInfo(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            if (!typeof(TValue).Equals(this.propertyInfo.PropertyType) || !typeof(IList<TValue>).IsAssignableFrom(propertyInfo.DeclaringType))
                throw new ArgumentException("The property types do not match!");
        }

        public TValue GetValue(T target, int key)
        {
            return target[key];
        }

        public TValue GetValue(object target, int key)
        {
            return this.GetValue((T)target, key);
        }

        public void SetValue(T target, int key, TValue value)
        {
            target[key] = value;
        }

        public void SetValue(object target, int key, TValue value)
        {
            this.SetValue((T)target, key, value);
        }
    }

    public class DictionaryProxyItemInfo<T, TKey, TValue> : ProxyItemInfo, IProxyItemInfo<T, TKey, TValue> where T : IDictionary<TKey, TValue>
    {
        public DictionaryProxyItemInfo(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            if (!typeof(TValue).Equals(this.propertyInfo.PropertyType) || !typeof(IDictionary<TKey, TValue>).IsAssignableFrom(propertyInfo.DeclaringType))
                throw new ArgumentException("The property types do not match!");
        }

        public TValue GetValue(T target, TKey key)
        {
            return target[key];
        }

        public TValue GetValue(object target, TKey key)
        {
            return this.GetValue((T)target, key);
        }

        public void SetValue(T target, TKey key, TValue value)
        {
            target[key] = value;
        }

        public void SetValue(object target, TKey key, TValue value)
        {
            this.SetValue((T)target, key, value);
        }
    }

    public class ArrayProxyItemInfo : IProxyItemInfo
    {
        protected readonly Type type;
        public ArrayProxyItemInfo(Type type)
        {
            this.type = type;
            if (this.type == null || !this.type.IsArray)
                throw new ArgumentException();
        }

        public Type ValueType { get { return type.HasElementType ? type.GetElementType() : typeof(object); } }

        public Type DeclaringType { get { return this.type; } }

        public string Name { get { return "Item"; } }

        public bool IsStatic { get { return false; } }

        public virtual object GetValue(object target, object key)
        {
            return ((Array)target).GetValue((int)key);
        }

        public virtual void SetValue(object target, object key, object value)
        {
            ((Array)target).SetValue(value, (int)key);
        }
    }

    public class ArrayProxyItemInfo<T, TValue> : ArrayProxyItemInfo, IProxyItemInfo<T, int, TValue> where T : IList<TValue>
    {
        public ArrayProxyItemInfo() : base(typeof(T))
        {
        }

        public TValue GetValue(T target, int key)
        {
            return target[key];
        }

        public TValue GetValue(object target, int key)
        {
            return GetValue((T)target, key);
        }

        public void SetValue(T target, int key, TValue value)
        {
            target[key] = value;
        }

        public void SetValue(object target, int key, TValue value)
        {
            SetValue((T)target, key, value);
        }
    }
}
