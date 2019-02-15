using System;
using System.Reflection;

using Loxodon.Log;
using Loxodon.Framework.Binding.Paths;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class ChainedFieldObjectSourceProxy : AbstractFieldObjectSourceProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ChainedFieldObjectSourceProxy));

        private bool disposed = false;
        private readonly IObjectSourceProxyFactory factory;
        private readonly PathToken nextToken;

        private IObjectSourceProxy childProxy;

        public ChainedFieldObjectSourceProxy(FieldInfo fieldInfo, PathToken nextToken, IObjectSourceProxyFactory factory) : this(null, fieldInfo, nextToken, factory)
        {
        }

        public ChainedFieldObjectSourceProxy(object source, FieldInfo fieldInfo, PathToken nextToken, IObjectSourceProxyFactory factory) : base(source, fieldInfo)
        {
            this.nextToken = nextToken;
            this.factory = factory;
            this.UpdateChildProxy();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (this.childProxy != null)
                {
                    this.childProxy.Dispose();
                    this.childProxy = null;
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }

        public override Type Type
        {
            get
            {
                if (this.childProxy == null)
                    return typeof(object);

                return this.childProxy.Type;
            }
        }

        public override object GetValue()
        {
            if (this.childProxy == null)
            {
                return ReturnObject.UNSET;
            }

            return this.childProxy.GetValue();
        }

        public override void SetValue(object value)
        {
            if (this.childProxy == null)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("SetValue ignored in binding.the \"{0}\" field is missing", this.FieldInfo.Name);
                return;
            }

            this.childProxy.SetValue(value);
        }

        protected virtual void OnChildPropertyChanged(object sender, EventArgs e)
        {
            this.RaiseValueChanged();
        }

        protected virtual object GetFieldValue()
        {
            if (this.Source == null && !this.proxyField.IsStatic)
                return null;

            return this.proxyField.GetValue(this.Source);
        }

        protected virtual void UpdateChildProxy()
        {
            if (this.childProxy != null)
            {
                this.childProxy.ValueChanged -= this.OnChildPropertyChanged;
                this.childProxy.Dispose();
                this.childProxy = null;
            }

            var currentValue = this.GetFieldValue();
            if (currentValue == null)
                return;

            this.childProxy = this.factory.CreateProxy(currentValue, this.nextToken);
            if(this.childProxy!=null)
                this.childProxy.ValueChanged += this.OnChildPropertyChanged;
        }
    }
}
