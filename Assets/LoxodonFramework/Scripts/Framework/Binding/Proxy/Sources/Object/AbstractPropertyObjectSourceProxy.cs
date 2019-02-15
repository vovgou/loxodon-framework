using System;
using System.Reflection;
using System.ComponentModel;

using Loxodon.Log;
using Loxodon.Framework.Binding.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public abstract class AbstractPropertyObjectSourceProxy : AbstractObjectSourceProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AbstractPropertyObjectSourceProxy));

        private bool disposed = false;
        protected readonly PropertyInfo propertyInfo;
        protected string propertyName;

        protected IProxyPropertyInfo proxyProperty;

        public AbstractPropertyObjectSourceProxy(object source, PropertyInfo propertyInfo) : base(source)
        {
            this.propertyInfo = propertyInfo;
            this.propertyName = this.propertyInfo.Name;
            this.proxyProperty = propertyInfo.AsProxy();
          
            if (this.proxyProperty.IsStatic)
                return;
          
            if (this.Source == null)
            {
                if (log.IsWarnEnabled)
                    log.Warn("Unable to bind to source as it's null");
                return;
            }

            if (this.Source is INotifyPropertyChanged)
            {
                var sourceNotify = this.Source as INotifyPropertyChanged;
                sourceNotify.PropertyChanged += OnPropertyChanged;
            }
            
            if (this.Source is ValueType || !(this.Source is INotifyPropertyChanged))
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The \"{0}\" type can't be listened", this.Source.GetType().Name);
            }
        }

        protected PropertyInfo PropertyInfo { get { return this.propertyInfo; } }

        public override Type Type { get { return this.propertyInfo!=null ? this.propertyInfo.PropertyType:null; } }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var name = e.PropertyName;
            if (string.IsNullOrEmpty(name) || name.Equals(this.propertyName))
                this.RaiseValueChanged();
        }


        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (this.Source is INotifyPropertyChanged)
                {
                    var sourceNotify = this.Source as INotifyPropertyChanged;
                    sourceNotify.PropertyChanged -= OnPropertyChanged;
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }
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
