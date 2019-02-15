using System;
using System.Reflection;
using System.ComponentModel;

using Loxodon.Log;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Binding.Paths;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public abstract class AbstractChainedPropertyObjectSourceProxy : AbstractPropertyObjectSourceProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AbstractChainedPropertyObjectSourceProxy));

        private bool disposed = false;
        private readonly IObjectSourceProxyFactory factory;
        private readonly PathToken nextToken;

        private IObjectSourceProxy childProxy;

        public AbstractChainedPropertyObjectSourceProxy(object source, PropertyInfo propertyInfo, PathToken nextToken, IObjectSourceProxyFactory factory) : base(source, propertyInfo)
        {
            this.nextToken = nextToken;
            this.factory = factory;
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
                    log.WarnFormat("SetValue ignored in binding.the \"{0}\" property is missing", this.PropertyInfo.Name);
                return;
            }

            this.childProxy.SetValue(value);
        }

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName.Equals(this.PropertyInfo.Name))
            {
                this.UpdateChildProxy();
                base.OnPropertyChanged(sender, e);
            }
        }

        protected virtual void OnChildPropertyChanged(object sender, EventArgs e)
        {
            this.RaiseValueChanged();
        }

        protected abstract object GetPropertyValue();

        protected virtual void UpdateChildProxy()
        {
            if (this.childProxy != null)
            {
                this.childProxy.ValueChanged -= this.OnChildPropertyChanged;
                this.childProxy.Dispose();
                this.childProxy = null;
            }

            var currentValue = this.GetPropertyValue();
            if (currentValue == null)
                return;

            this.childProxy = this.factory.CreateProxy(currentValue, this.nextToken);
            if (this.childProxy != null)
                this.childProxy.ValueChanged += this.OnChildPropertyChanged;
        }
    }

    public class ChainedPropertyObjectSourceProxy : AbstractChainedPropertyObjectSourceProxy
    {
        public ChainedPropertyObjectSourceProxy(PropertyInfo propertyInfo, PathToken nextToken, IObjectSourceProxyFactory factory) : this(null, propertyInfo, nextToken, factory)
        {
        }

        public ChainedPropertyObjectSourceProxy(object source, PropertyInfo propertyInfo, PathToken nextToken, IObjectSourceProxyFactory factory) : base(source, propertyInfo, nextToken, factory)
        {
            this.UpdateChildProxy();
        }

        protected override object GetPropertyValue()
        {
            if (this.Source == null && !this.proxyProperty.IsStatic)
                return null;

            return this.proxyProperty.GetValue(this.Source);
        }
    }

    public class ChainedItemObjectSourceProxy : AbstractChainedPropertyObjectSourceProxy
    {
        private bool disposed = false;
        private readonly object _key;

        public ChainedItemObjectSourceProxy(object source, PropertyInfo propertyInfo, PathToken nextToken, IObjectSourceProxyFactory factory, object key) : base(source, propertyInfo, nextToken, factory)
        {
            this._key = key;

            if (this.Source is INotifyCollectionChanged)
            {
                var sourceCollection = this.Source as INotifyCollectionChanged;
                sourceCollection.CollectionChanged += OnCollectionChanged;
            }

            this.UpdateChildProxy();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (this.Source is INotifyCollectionChanged)
                {
                    var sourceCollection = this.Source as INotifyCollectionChanged;
                    sourceCollection.CollectionChanged -= OnCollectionChanged;
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.RaiseValueChanged();
                return;
            }

            if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && this._key is int)
            {
                if ((int)this._key == e.OldStartingIndex)
                    this.RaiseValueChanged();
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Add)
            {
                this.RaiseValueChanged();
            }
        }

        protected override object GetPropertyValue()
        {
            if (this.Source == null)
                return null;

            return this.proxyProperty.GetValue(this.Source, this._key);
        }
    }
}
