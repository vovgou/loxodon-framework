using System;
using System.Reflection;

using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class LeafFieldObjectSourceProxy : AbstractFieldObjectSourceProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LeafFieldObjectSourceProxy));

        public LeafFieldObjectSourceProxy(FieldInfo fieldInfo) : this(null, fieldInfo)
        {
        }

        public LeafFieldObjectSourceProxy(object source, FieldInfo fieldInfo) : base(source, fieldInfo)
        {
        }

        public override object GetValue()
        {
            try
            {
                return this.proxyField.GetValue(this.Source);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("GetValue failed with exception,{0}", e);

                return ReturnObject.UNSET;
            }
        }

        public override void SetValue(object value)
        {
            try
            {
                if (typeof(Delegate).IsAssignableFrom(this.FieldInfo.FieldType))
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Set value ignored in object field \"{0}\".Assignment of delegate type is not supported.", this.FieldInfo.Name);
                    return;
                }

                var fieldType = this.FieldInfo.FieldType;
                var safeValue = fieldType.ToSafe(value);

                if (this.IsEquals(safeValue))
                    return;

                this.proxyField.SetValue(this.Source, safeValue);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("SetValue failed with exception,{0}", e);
            }
        }
    }
}
