using System;
using System.Reflection;

using Loxodon.Log;

namespace Loxodon.Framework.Binding.Reflection
{
    public abstract class AbstractProxyPropertyInfo : IProxyPropertyInfo
    {
        protected readonly PropertyInfo propertyInfo;
        protected bool canRead = false;
        protected bool canWrite = false;
        protected bool isStatic = false;
        protected bool isValueType = false;

        public AbstractProxyPropertyInfo(PropertyInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("Parameters' info 'cannot be null!");

            this.propertyInfo = info;
            this.isStatic = info.IsStatic();
#if NETFX_CORE
            this.isValueType = info.DeclaringType.GetTypeInfo().IsValueType;
#else
            this.isValueType = info.DeclaringType.IsValueType;
#endif
        }

        public PropertyInfo PropertyInfo { get { return this.propertyInfo; } }

        public bool IsStatic { get { return this.isStatic; } }

        public Type DeclaringType { get { return this.propertyInfo.DeclaringType; } }

        public Type PropertyType { get { return this.propertyInfo.PropertyType; } }

        public string Name { get { return this.propertyInfo.Name; } }

        public bool CanRead
        {
            get { return this.canRead; }
        }

        public bool CanWrite
        {
            get { return this.canWrite; }
        }

        public abstract object GetValue(object target, object index = null);

        public abstract void SetValue(object target, object newValue, object index = null);
    }

    public class ProxyPropertyInfo : AbstractProxyPropertyInfo
    {
        public ProxyPropertyInfo(PropertyInfo info) : base(info)
        {
            this.canRead = this.propertyInfo.CanRead;
            this.canWrite = this.propertyInfo.CanWrite;
        }

        public override object GetValue(object target, object index = null)
        {
            if (!this.canRead)
                throw new MemberAccessException();

            var method = this.propertyInfo.GetGetMethod();
            return method.Invoke(target, index == null ? null : new object[] { index });
        }

        public override void SetValue(object target, object newValue, object index = null)
        {
            if (this.isValueType)
                throw new NotSupportedException("Assignments of Value type are not supported.");

            if (!this.canWrite)
                throw new MemberAccessException();

            var method = this.propertyInfo.GetSetMethod();
            method.Invoke(target, index == null ? new object[] { newValue } : new object[] { index, newValue });
        }
    }

    public class ProxyPropertyInfo<T, TValue> : AbstractProxyPropertyInfo, IProxyPropertyInfo<T, TValue>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProxyPropertyInfo<T, TValue>));

        private Func<T, TValue> _getter;
        private Action<T, TValue> _setter;

        public ProxyPropertyInfo(PropertyInfo info) : this(info, null, null)
        {
        }

        public ProxyPropertyInfo(string propertyName, Func<T, TValue> getter, Action<T, TValue> setter) : this(typeof(T).GetProperty(propertyName), getter, setter)
        {
        }

        public ProxyPropertyInfo(PropertyInfo info, Func<T, TValue> getter, Action<T, TValue> setter) : base(info)
        {
            if (this.propertyInfo.IsStatic())
                throw new ArgumentException("The property is static!");

            if (!typeof(TValue).Equals(this.propertyInfo.PropertyType) || !this.propertyInfo.DeclaringType.IsAssignableFrom(typeof(T)))
                throw new ArgumentException("The property types do not match!");

            this._getter = getter;
            this._setter = setter;

            if (this._getter == null)
                this._getter = this.MakeGetter(this.propertyInfo);

            if (this._setter == null)
                this._setter = this.MakeSetter(this.propertyInfo);

            this.canRead = this._getter != null || this.propertyInfo.CanRead;
            this.canWrite = this._setter != null || this.propertyInfo.CanWrite;
        }

        private Action<T, TValue> MakeSetter(PropertyInfo propertyInfo)
        {
            try
            {
                if (isValueType)
                    return null;

                var setMethod = propertyInfo.GetSetMethod();
                if (setMethod == null)
                    return null;
#if NETFX_CORE
                return (Action<T, TValue>)setMethod.CreateDelegate(typeof(Action<T, TValue>));
#elif !UNITY_IOS
                return (Action<T, TValue>)Delegate.CreateDelegate(typeof(Action<T, TValue>), setMethod);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }

            return null;
        }

        private Func<T, TValue> MakeGetter(PropertyInfo propertyInfo)
        {
            try
            {
                if (isValueType)
                    return null;

                var getMethod = propertyInfo.GetGetMethod();
                if (getMethod == null)
                    return null;
#if NETFX_CORE 
                return (Func<T, TValue>)getMethod.CreateDelegate(typeof(Func<T, TValue>));
#elif !UNITY_IOS
                return (Func<T, TValue>)Delegate.CreateDelegate(typeof(Func<T, TValue>), getMethod);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }

            return null;
        }

        public virtual TValue GetValue(T target)
        {
            if (!this.canRead)
                throw new MemberAccessException();

            if (this._getter != null)
                return this._getter(target);

            var method = this.propertyInfo.GetGetMethod();
            return (TValue)method.Invoke(target, null);
        }

        public override object GetValue(object target, object index = null)
        {
            return this.GetValue((T)target);
        }

        public virtual void SetValue(T target, TValue newValue)
        {
            if (this.isValueType)
                throw new NotSupportedException("Assignments of Value type are not supported.");

            if (!this.canWrite)
                throw new MemberAccessException();

            if (this._setter != null)
            {
                this._setter(target, newValue);
                return;
            }

            var method = this.propertyInfo.GetSetMethod();
            method.Invoke(target, new object[] { newValue });
        }

        public override void SetValue(object target, object newValue, object index = null)
        {
            this.SetValue((T)target, (TValue)newValue);
        }
    }

    public class ProxyPropertyInfo<T, TIndex, TValue> : AbstractProxyPropertyInfo, IProxyPropertyInfo<T, TIndex, TValue>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProxyPropertyInfo<T, TIndex, TValue>));

        private Func<T, TIndex, TValue> _getter;
        private Action<T, TIndex, TValue> _setter;

        public ProxyPropertyInfo(string propertyName) : this(typeof(T).GetProperty(propertyName), null, null)
        {
        }

        public ProxyPropertyInfo(PropertyInfo info) : this(info, null, null)
        {
        }

        public ProxyPropertyInfo(string propertyName, Func<T, TIndex, TValue> getter, Action<T, TIndex, TValue> setter) : this(typeof(T).GetProperty(propertyName), getter, setter)
        {
        }

        public ProxyPropertyInfo(PropertyInfo info, Func<T, TIndex, TValue> getter, Action<T, TIndex, TValue> setter) : base(info)
        {
            if (info.IsStatic())
                throw new ArgumentException("The property is static!");

            if (!typeof(TValue).Equals(this.propertyInfo.PropertyType) || !this.propertyInfo.DeclaringType.IsAssignableFrom(typeof(T)) || this.propertyInfo.GetIndexParameters().Length != 1 || !typeof(TIndex).Equals(this.propertyInfo.GetIndexParameters()[0].ParameterType))
                throw new ArgumentException("The property types do not match!");

            this._getter = getter;
            this._setter = setter;

            if (this._getter == null)
                this._getter = this.MakeGetter(this.propertyInfo);

            if (this._setter == null)
                this._setter = this.MakeSetter(this.propertyInfo);

            this.canRead = this._getter != null || this.propertyInfo.CanRead;
            this.canWrite = this._setter != null || this.propertyInfo.CanWrite;
        }

        private Action<T, TIndex, TValue> MakeSetter(PropertyInfo propertyInfo)
        {
            try
            {
                if (isValueType)
                    return null;

                var setMethod = propertyInfo.GetSetMethod();
                if (setMethod == null)
                    return null;
#if NETFX_CORE
                return (Action<T, TIndex, TValue>)setMethod.CreateDelegate(typeof(Action<T, TIndex, TValue>));
#elif !UNITY_IOS
                return (Action<T, TIndex, TValue>)Delegate.CreateDelegate(typeof(Action<T, TIndex, TValue>), setMethod);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        private Func<T, TIndex, TValue> MakeGetter(PropertyInfo propertyInfo)
        {
            try
            {
                if (isValueType)
                    return null;

                var getMethod = propertyInfo.GetGetMethod();
                if (getMethod == null)
                    return null;
#if NETFX_CORE
                return (Func<T, TIndex, TValue>)getMethod.CreateDelegate(typeof(Func<T, TIndex, TValue>));
#elif !UNITY_IOS
                return (Func<T, TIndex, TValue>)Delegate.CreateDelegate(typeof(Func<T, TIndex, TValue>), getMethod);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }

            return null;
        }

        public virtual TValue GetValue(T target, TIndex index)
        {
            if (!this.canRead)
                throw new MemberAccessException();

            if (this._getter != null)
                return this._getter(target, index);

            var method = this.propertyInfo.GetGetMethod();
            return (TValue)method.Invoke(target, new object[] { index });
        }

        public override object GetValue(object target, object index)
        {
            return this.GetValue((T)target, (TIndex)index);
        }

        public virtual void SetValue(T target, TValue newValue, TIndex index)
        {
            if (this.isValueType)
                throw new NotSupportedException("Assignments of Value type are not supported.");

            if (!this.canWrite)
                throw new MemberAccessException();

            if (this._setter != null)
            {
                this._setter(target, index, newValue);
                return;
            }

            var method = this.propertyInfo.GetSetMethod();
            method.Invoke(target, new object[] { index, newValue });
        }

        public override void SetValue(object target, object newValue, object index)
        {
            this.SetValue((T)target, (TValue)newValue, (TIndex)index);
        }
    }
}
