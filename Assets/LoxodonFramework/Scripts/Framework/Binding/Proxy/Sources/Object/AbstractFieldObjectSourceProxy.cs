using System;
using System.Reflection;

using Loxodon.Log;
using Loxodon.Framework.Binding.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public abstract class AbstractFieldObjectSourceProxy : AbstractObjectSourceProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AbstractFieldObjectSourceProxy));

        private readonly FieldInfo fieldInfo;
        protected IProxyFieldInfo proxyField;

        public AbstractFieldObjectSourceProxy(object source, FieldInfo fieldInfo) : base(source)
        {
            this.fieldInfo = fieldInfo;
            this.proxyField = fieldInfo.AsProxy();

            if (this.proxyField.IsStatic)
                return;

            if (this.Source == null)
            {
                if (log.IsWarnEnabled)
                    log.Warn("Unable to bind to source as it's null");
                return;
            }
        }

        protected FieldInfo FieldInfo { get { return this.fieldInfo; } }

        public override Type Type { get { return this.fieldInfo != null ? this.fieldInfo.FieldType : null; } }

        protected bool IsEquals(object obj)
        {
            var value = this.GetValue();
            if (value != null)
                return value.Equals(obj);

            if (obj != null)
                return obj.Equals(value);

            return true;
        }
    }
}
