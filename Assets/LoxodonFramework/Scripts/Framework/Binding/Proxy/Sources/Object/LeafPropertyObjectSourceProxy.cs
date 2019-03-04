using System;
using System.Reflection;
using System.Collections.Specialized;

using Loxodon.Log;
using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public abstract class AbstractLeafPropertyObjectSourceProxy : AbstractPropertyObjectSourceProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AbstractLeafPropertyObjectSourceProxy));

        public AbstractLeafPropertyObjectSourceProxy(object source, PropertyInfo propertyInfo) : base(source, propertyInfo)
        {
        }

        public override object GetValue()
        {
            if (!this.PropertyInfo.CanRead)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Get value ignored in binding,the \"{0}\" property is writeonly", this.PropertyInfo.Name);

                return ReturnObject.UNSET;
            }

            try
            {
                return this.GetValueImpl();
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("Get value failed with exception,{0}", e);

                return ReturnObject.UNSET;
            }
        }

        public abstract object GetValueImpl();

        public override void SetValue(object value)
        {
            try
            {
                if (!this.PropertyInfo.CanWrite)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Set value ignored in binding,the \"{0}\" property is readonly", this.PropertyInfo.Name);
                    return;
                }

                if (typeof(Delegate).IsAssignableFrom(this.PropertyInfo.PropertyType))
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Set value ignored in object property \"{0}\".Assignment of delegate type is not supported.", this.PropertyInfo.Name);
                    return;
                }

                var propertyType = this.PropertyInfo.PropertyType;
                var safeValue = propertyType.ToSafe(value);

                if (this.IsEquals(safeValue))
                    return;

                this.SetValueImpl(safeValue);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("Set value failed with exception,{0}", e);
            }
        }

        public abstract void SetValueImpl(object value);
    }

    public class LeafPropertyObjectSourceProxy : AbstractLeafPropertyObjectSourceProxy
    {
        public LeafPropertyObjectSourceProxy(object source, PropertyInfo propertyInfo) : base(source, propertyInfo)
        {
        }

        public override object GetValueImpl()
        {
            if (this.Source == null && !this.proxyProperty.IsStatic)
                return null;

            return this.proxyProperty.GetValue(this.Source);
        }

        public override void SetValueImpl(object value)
        {
            if (this.Source == null && !this.proxyProperty.IsStatic)
                return;

            this.proxyProperty.SetValue(this.Source, value);
        }
    }

    public class LeafItemObjectSourceProxy : AbstractLeafPropertyObjectSourceProxy
    {
        private bool disposed = false;
        private readonly object _key;
        public LeafItemObjectSourceProxy(object source, PropertyInfo propertyInfo, object key) : base(source, propertyInfo)
        {
            this._key = key;

            if (this.Source is INotifyCollectionChanged)
            {
                var sourceCollection = this.Source as INotifyCollectionChanged;
                sourceCollection.CollectionChanged += OnCollectionChanged;
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

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.RaiseValueChanged();
            }
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

        public override object GetValueImpl()
        {
            if (this.Source == null)
                return null;

            return this.proxyProperty.GetValue(this.Source, this._key);
        }

        public override void SetValueImpl(object value)
        {
            if (this.Source == null)
                return;

            this.proxyProperty.SetValue(this.Source, value, this._key);
        }
    }
}
