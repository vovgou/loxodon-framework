using System;

using Loxodon.Log;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Binding.Paths;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class ChainedObservablePropertyObjectSourceProxy : ObservablePropertyObjectSourceProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ChainedObservablePropertyObjectSourceProxy));

        private bool disposed = false;
        private readonly IObjectSourceProxyFactory factory;
        private readonly PathToken nextToken;
        private IObjectSourceProxy childProxy;

        public ChainedObservablePropertyObjectSourceProxy(IObservableProperty observableProperty, PathToken nextToken, IObjectSourceProxyFactory factory) : this(null, observableProperty, nextToken, factory)
        {
        }

        public ChainedObservablePropertyObjectSourceProxy(object source, IObservableProperty observableProperty, PathToken nextToken, IObjectSourceProxyFactory factory) : base(source, observableProperty)
        {
            this.nextToken = nextToken;
            this.factory = factory;
            this.UpdateChildProxy();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (this.childProxy != null)
                    {
                        this.childProxy.Dispose();
                        this.childProxy = null;
                    }
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
                    log.Warn("SetValue ignored in binding,the property is missing");
                return;
            }

            this.childProxy.SetValue(value);
        }

        protected override void OnValueChanged(object sender, EventArgs e)
        {
            this.UpdateChildProxy();
            base.OnValueChanged(sender, e);
        }

        protected virtual void OnChildPropertyChanged(object sender, EventArgs e)
        {
            this.RaiseValueChanged();
        }

        protected virtual void UpdateChildProxy()
        {
            if (this.childProxy != null)
            {
                this.childProxy.ValueChanged -= this.OnChildPropertyChanged;
                this.childProxy.Dispose();
                this.childProxy = null;
            }

            var currentValue = this.observableProperty.Value;
            if (currentValue == null)
                return;

            this.childProxy = this.factory.CreateProxy(currentValue, this.nextToken);
            if (this.childProxy != null)
                this.childProxy.ValueChanged += this.OnChildPropertyChanged;
        }
    }
}
