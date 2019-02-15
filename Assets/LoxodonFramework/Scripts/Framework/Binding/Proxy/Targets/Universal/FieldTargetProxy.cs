using System;
using System.Reflection;

using Loxodon.Log;
using Loxodon.Framework.Binding.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class FieldTargetProxy : AbstractTargetProxy, IObtainable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FieldTargetProxy));

        protected readonly FieldInfo fieldInfo;
        protected readonly IProxyFieldInfo proxyField;

        public override Type Type { get { return this.fieldInfo.FieldType; } }

        public override BindingMode DefaultMode { get { return BindingMode.OneWay; } }

        public FieldTargetProxy(object target, FieldInfo fieldInfo) : base(target)
        {
            this.fieldInfo = fieldInfo;
            this.proxyField = fieldInfo.AsProxy();
        }

        public virtual object GetValue()
        {
            var target = this.Target;
            if (target == null)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Get value ignored in target's property \"{0}\",the weak reference to the target is null.", this.fieldInfo.Name);

                return ReturnObject.UNSET;
            }

            return proxyField.GetValue(target);
        }

        protected override void SetValueImpl(object target, object value)
        {
            this.proxyField.SetValue(target, value);
        }

        protected override void DoSubscribeForValueChange(object target)
        {
            throw new NotSupportedException("This is a field proxy,don't support the \"ValueChange\" event.");
        }
    }
}
