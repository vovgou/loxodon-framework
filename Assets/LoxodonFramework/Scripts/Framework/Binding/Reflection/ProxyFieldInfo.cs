using System;
using System.Reflection;
using Loxodon.Log;

#if !UNITY_IOS && !ENABLE_IL2CPP && !NET_STANDARD_2_0
using System.Reflection.Emit;
#endif

#if !UNITY_IOS && !ENABLE_IL2CPP
using System.Linq.Expressions;
#endif

namespace Loxodon.Framework.Binding.Reflection
{
    public class ProxyFieldInfo : IProxyFieldInfo
    {
        private readonly FieldInfo fieldInfo;
        private bool isValueType = false;

        public ProxyFieldInfo(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
            if (this.fieldInfo == null)
                throw new FieldAccessException();

#if NETFX_CORE
            this.isValueType = this.fieldInfo.DeclaringType.GetTypeInfo().IsValueType;
#else
            this.isValueType = this.fieldInfo.DeclaringType.IsValueType;
#endif
        }

        public FieldInfo FieldInfo { get { return this.fieldInfo; } }

        public Type DeclaringType { get { return this.fieldInfo.DeclaringType; } }

        public Type FieldType { get { return this.fieldInfo.FieldType; } }

        public bool IsStatic { get { return this.fieldInfo.IsStatic; } }

        public string Name { get { return this.fieldInfo.Name; } }

        public object GetValue(object target)
        {
            return this.fieldInfo.GetValue(target);
        }

        public void SetValue(object target, object value)
        {
            if (fieldInfo.IsInitOnly)
                throw new MemberAccessException("The value is read-only.");

            if (this.isValueType)
                throw new NotSupportedException("Assignments of Value type are not supported.");

            this.fieldInfo.SetValue(target, value);
        }
    }

    public class ProxyFieldInfo<T, TValue> : IProxyFieldInfo<T, TValue>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProxyFieldInfo<T, TValue>));

        private readonly FieldInfo fieldInfo;
        private Func<T, TValue> getter;
        private Action<T, TValue> setter;
        private bool isValueType = false;

        public ProxyFieldInfo(string fieldName) : this(typeof(T).GetField(fieldName), null, null)
        {
        }

        public ProxyFieldInfo(FieldInfo fieldInfo) : this(fieldInfo, null, null)
        {
        }

        public ProxyFieldInfo(string fieldName, Func<T, TValue> getter, Action<T, TValue> setter) : this(typeof(T).GetField(fieldName), getter, setter)
        {
        }

        public ProxyFieldInfo(FieldInfo fieldInfo, Func<T, TValue> getter, Action<T, TValue> setter)
        {
            this.fieldInfo = fieldInfo;
            if (this.fieldInfo == null)
                throw new FieldAccessException();

            if (!fieldInfo.DeclaringType.IsAssignableFrom(typeof(T)) || !fieldInfo.FieldType.Equals(typeof(TValue)))
                throw new ArgumentException();

            this.getter = getter;
            this.setter = setter;

            if (this.getter == null)
                this.getter = MakeGetter(this.fieldInfo);

            if (this.setter == null)
                this.setter = MakeSetter(this.fieldInfo);

#if NETFX_CORE
            this.isValueType = this.fieldInfo.DeclaringType.GetTypeInfo().IsValueType;
#else
            this.isValueType = this.fieldInfo.DeclaringType.IsValueType;
#endif
        }

        public FieldInfo FieldInfo { get { return this.fieldInfo; } }

        public Type DeclaringType { get { return this.fieldInfo.DeclaringType; } }

        public Type FieldType { get { return this.fieldInfo.FieldType; } }

        public bool IsStatic { get { return this.fieldInfo.IsStatic; } }

        public string Name { get { return this.fieldInfo.Name; } }

        private Action<T, TValue> MakeSetter(FieldInfo fieldInfo)
        {
            if (isValueType)
                return null;

            if (fieldInfo.IsInitOnly)
                return null;

#if !ENABLE_IL2CPP
#if NETFX_CORE || NET_STANDARD_2_0
            try
            {
                var targetExp = Expression.Parameter(typeof(T), "target");
                var paramExp = Expression.Parameter(typeof(TValue), "value");
                var fieldExp = Expression.Field(fieldInfo.IsStatic ? null : targetExp, fieldInfo);
                var assignExp = Expression.Assign(fieldExp, paramExp);
                var lambda = Expression.Lambda<Action<T, TValue>>(assignExp, targetExp, paramExp);
                return lambda.Compile();
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
#elif !UNITY_IOS
            try
            {
                DynamicMethod m = new DynamicMethod("Setter", typeof(void), new Type[] { typeof(T), typeof(TValue) }, typeof(T));
                ILGenerator cg = m.GetILGenerator();
                if (fieldInfo.IsStatic)
                {
                    cg.Emit(OpCodes.Ldarg_1);
                    cg.Emit(OpCodes.Stsfld, fieldInfo);
                    cg.Emit(OpCodes.Ret);
                }
                else
                {
                    cg.Emit(OpCodes.Ldarg_0);
                    cg.Emit(OpCodes.Ldarg_1);
                    cg.Emit(OpCodes.Stfld, fieldInfo);
                    cg.Emit(OpCodes.Ret);
                }
                return (Action<T, TValue>)m.CreateDelegate(typeof(Action<T, TValue>));
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
#endif
#endif
            return null;
        }

        private Func<T, TValue> MakeGetter(FieldInfo fieldInfo)
        {
#if !UNITY_IOS && !ENABLE_IL2CPP
            try
            {
                var targetExp = Expression.Parameter(typeof(T), "target");
                var fieldExp = Expression.Field(fieldInfo.IsStatic ? null : targetExp, fieldInfo);
                var lambda = Expression.Lambda<Func<T, TValue>>(fieldExp, targetExp);
                return lambda.Compile();
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
#endif
            return null;
        }

        public object GetValue(object target)
        {
            return GetValue((T)target);
        }

        public TValue GetValue(T target)
        {
            if (this.getter != null)
                return this.getter(target);

            return (TValue)this.fieldInfo.GetValue(target);
        }

        public void SetValue(object target, object value)
        {
            SetValue((T)target, (TValue)value);
        }

        public void SetValue(T target, TValue value)
        {
            if (fieldInfo.IsInitOnly)
                throw new MemberAccessException("The value is read-only.");

            if (this.isValueType)
                throw new NotSupportedException("Assignments of Value type are not supported.");

            if (this.setter != null)
            {
                this.setter(target, value);
                return;
            }

            this.fieldInfo.SetValue(target, value);
        }
    }
}
