using System;
using System.Reflection;

using Loxodon.Log;

namespace Loxodon.Framework.Binding.Reflection
{
    public class StaticProxyPropertyInfo<T, TValue> : AbstractProxyPropertyInfo, IProxyPropertyInfo<T, TValue>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StaticProxyPropertyInfo<T, TValue>));

        private Func<TValue> _getter;
        private Action<TValue> _setter;

        public StaticProxyPropertyInfo(string propertyName) : this(typeof(T).GetProperty(propertyName), null, null)
        {
        }

        public StaticProxyPropertyInfo(PropertyInfo info) : this(info, null, null)
        {
        }

        public StaticProxyPropertyInfo(string propertyName, Func<TValue> getter, Action<TValue> setter) : this(typeof(T).GetProperty(propertyName), getter, setter)
        {
        }

        public StaticProxyPropertyInfo(PropertyInfo info, Func<TValue> getter, Action<TValue> setter) : base(info)
        {
            if (!info.IsStatic())
                throw new ArgumentException("The property isn't static!");

            if (!typeof(TValue).Equals(this.propertyInfo.PropertyType) || !typeof(T).Equals(this.propertyInfo.DeclaringType))
                throw new ArgumentException("The property types do not match!");

            this.isStatic = true;

            this._getter = getter;
            this._setter = setter;

            if (this._getter == null)
                this._getter = this.MakeGetter(this.propertyInfo);

            if (this._setter == null)
                this._setter = this.MakeSetter(this.propertyInfo);

            this.canRead = this._getter != null || this.propertyInfo.CanRead;
            this.canWrite = this._setter != null || this.propertyInfo.CanWrite;
        }

        private Action<TValue> MakeSetter(PropertyInfo propertyInfo)
        {
            try
            {
                if (isValueType)
                    return null;

                var setMethod = propertyInfo.GetSetMethod();
                if (setMethod == null)
                    return null;
#if NETFX_CORE
                return (Action<TValue>)setMethod.CreateDelegate(typeof(Action<TValue>));
#elif !UNITY_IOS
                return (Action<TValue>)Delegate.CreateDelegate(typeof(Action<TValue>), setMethod);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        private Func<TValue> MakeGetter(PropertyInfo propertyInfo)
        {
            try
            {
                if (isValueType)
                    return null;

                var getMethod = propertyInfo.GetGetMethod();
                if (getMethod == null)
                    return null;

#if NETFX_CORE
                return (Func<TValue>)getMethod.CreateDelegate(typeof(Func<TValue>));
#elif !UNITY_IOS
                return (Func<TValue>)Delegate.CreateDelegate(typeof(Func<TValue>), getMethod);
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
                return this._getter();

            var method = this.propertyInfo.GetGetMethod();
            return (TValue)method.Invoke(null, null);
        }

        public override object GetValue(object target, object index = null)
        {
            return this.GetValue(default(T));
        }

        public virtual void SetValue(T target, TValue newValue)
        {
            if (this.isValueType)
                throw new NotSupportedException("Assignments of Value type are not supported.");

            if (!this.canWrite)
                throw new MemberAccessException();

            if (this._setter != null)
            {
                this._setter(newValue);
                return;
            }

            var method = this.propertyInfo.GetSetMethod();
            method.Invoke(target, new object[] { newValue });
        }

        public override void SetValue(object target, object newValue, object index = null)
        {
            this.SetValue(default(T), newValue != null ? (TValue)newValue : default(TValue));
        }
    }
}
