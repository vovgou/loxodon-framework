using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Binding.Reflection
{
    public abstract class ProxyObject : IProxyObject, IDisposable
    {
        private readonly object _lock = new object();
        private EventHandler valueChanged;
        protected Type type;
        protected object target;

        protected ProxyObject parentProxy;
        protected IProxyPropertyInfo proxyPropertyInfo;

        protected IProxyType proxyType;
        protected bool hasProperty;

        protected object _key;

        public ProxyObject(object target)
        {
            this.target = target;
            this.type = target.GetType();
#if NETFX_CORE
            this.hasProperty = !this.type.GetTypeInfo().IsValueType;
#else
            this.hasProperty = !this.type.IsValueType;
#endif
        }

        public ProxyObject(ProxyObject parentProxy, IProxyPropertyInfo proxyPropertyInfo) : this(parentProxy, proxyPropertyInfo, null)
        {
        }

        public ProxyObject(ProxyObject parentProxy, IProxyPropertyInfo proxyPropertyInfo, object key)
        {
            this.parentProxy = parentProxy;
            this.proxyPropertyInfo = proxyPropertyInfo;
            this._key = key;

            if (this.parentProxy == null || this.proxyPropertyInfo == null)
                throw new ArgumentNullException();

            this.parentProxy.ValueChanged += OnParent_ValueChanged;
            var parent = this.parentProxy.Value;
            if (parent != null && parent is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)parent).PropertyChanged += OnParent_PropertyChanged;
            }

            if (this._key != null && parent != null && parent is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)parent).CollectionChanged += OnParent_CollectionChanged;
            }

            this.type = proxyPropertyInfo.PropertyType;
#if NETFX_CORE
            this.hasProperty = !this.type.GetTypeInfo().IsValueType;
#else
            this.hasProperty = !this.type.IsValueType;
#endif
        }

        protected IProxyType ProxyType
        {
            get
            {
                if (this.proxyType == null)
                {
                    this.proxyType = ProxyFactory.Default.Create(this.type);
                }
                return this.proxyType;
            }
        }

        public event EventHandler ValueChanged
        {
            add { lock (_lock) { this.valueChanged += value; } }
            remove { lock (_lock) { this.valueChanged -= value; } }
        }

        public Type Type { get { return this.type; } }

        public bool IsRoot { get { return this.parentProxy == null; } }

        protected virtual object GetValueFromParent()
        {
            var parent = this.parentProxy.Value;
            if (parent == null)
                return this.Type.CreateDefault();

            object value = this.proxyPropertyInfo.GetValue(parent, this._key);
            return this.type.ToSafe(value);
        }

        protected virtual void SetValueToParent(object value)
        {
            var parent = this.parentProxy.Value;
            if (parent == null)
                return;

            if (value == null)
                value = this.Type.CreateDefault();

            this.proxyPropertyInfo.SetValue(parent, value, this._key);
        }

        public virtual object Value
        {
            get
            {
                if (this.target != null)
                    return this.target;

                if (!IsRoot)
                    this.target = this.GetValueFromParent();
                else
                    this.target = this.Type.CreateDefault();

                return this.target;
            }
            set
            {
                if (!IsRoot)
                {
                    this.SetValueToParent(value);
                    return;
                }

                if (this.target == value)
                    return;

                this.target = value != null ? value : this.Type.CreateDefault();
                this.RaiseValueChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnParent_ValueChanged(object sender, EventArgs e)
        {
            this.target = this.GetValueFromParent();
            RaiseValueChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnParent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.proxyPropertyInfo.Name.Equals(e.PropertyName))
            {
                this.target = this.GetValueFromParent();
                RaiseValueChanged();
            }
        }

        protected virtual void OnParent_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset || ((e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Remove) && e.OldItems != null && e.OldItems.Count > 0))
            {
                var target = this.GetValueFromParent();
                if (this.target != target)
                {
                    this.target = target;
                    RaiseValueChanged();
                }
            }
        }

        protected void RaiseValueChanged()
        {
            var handler = this.valueChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public abstract IProxyObject GetPropertyProxy(string name);

        public virtual IProxyInvoker GetMethodInvoker(string name)
        {
            IProxyMethodInfo proxyMethodInfo = this.ProxyType.GetMethod(name);
            if (proxyMethodInfo == null)
                throw new InvalidOperationException("Method not found");

            return new ProxyInvoker(this.Value,proxyMethodInfo);
        }

        public virtual IProxyInvoker GetMethodInvoker(string name, Type[] parameterTypes)
        {
            IProxyMethodInfo proxyMethodInfo = this.ProxyType.GetMethod(name, parameterTypes);
            if (proxyMethodInfo == null)
                throw new InvalidOperationException("Method not found");

            return new ProxyInvoker(this.Value, proxyMethodInfo);
        }

       

        public T GetProperty<T>(string name)
        {
            IProxyPropertyInfo proxyPropertyInfo = this.ProxyType.GetProperty(name);
            if (proxyPropertyInfo == null)
                throw new InvalidOperationException("Property not found");

            object value = proxyPropertyInfo.GetValue(this.Value);
            return (T)typeof(T).ToSafe(value);
        }

        public void SetProperty<T>(string name, T value)
        {
            IProxyPropertyInfo proxyPropertyInfo = this.ProxyType.GetProperty(name);
            if (proxyPropertyInfo == null)
                throw new InvalidOperationException("Property not found");

            proxyPropertyInfo.SetValue(this.Value, value);
        }

        public T GetField<T>(string name)
        {
            IProxyFieldInfo proxyFieldInfo = this.ProxyType.GetField(name);
            if (proxyFieldInfo == null)
                throw new InvalidOperationException("Field not found");

            object value = proxyFieldInfo.GetValue(this.Value);
            return (T)typeof(T).ToSafe(value);
        }

        public void SetField<T>(string name, T value)
        {
            IProxyFieldInfo proxyFieldInfo = this.ProxyType.GetField(name);
            if (proxyFieldInfo == null)
                throw new InvalidOperationException("Field not found");

            proxyFieldInfo.SetValue(this.Value, value);
        }

        public object Invoke(string name, params object[] args)
        {
            IProxyMethodInfo proxyMethodInfo = this.ProxyType.GetMethod(name);
            if (proxyMethodInfo == null)
                return null;
            return proxyMethodInfo.Invoke(this.Value, args);
        }

        public object Invoke(string name, Type[] parameterTypes, params object[] args)
        {
            IProxyMethodInfo proxyMethodInfo = this.ProxyType.GetMethod(name, parameterTypes);
            if (proxyMethodInfo == null)
                return null;
            return proxyMethodInfo.Invoke(this.Value, args);
        }

#region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (this.parentProxy != null)
                {
                    this.parentProxy.ValueChanged -= OnParent_ValueChanged;
                    var parent = this.parentProxy.Value;
                    if (parent != null && parent is INotifyPropertyChanged)
                    {
                        ((INotifyPropertyChanged)parent).PropertyChanged -= OnParent_PropertyChanged;
                    }

                    if (this._key != null && parent != null && parent is INotifyCollectionChanged)
                    {
                        ((INotifyCollectionChanged)parent).CollectionChanged -= OnParent_CollectionChanged;
                    }
                }
                disposed = true;
            }
        }

        ~ProxyObject()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
#endregion
    }

    public class ProxyObject<T> : ProxyObject, IProxyObject<T>
    {
        private bool disposed = false;
        protected Dictionary<string, IProxyObject> properties = new Dictionary<string, IProxyObject>();

        public ProxyObject(T target) : base(target)
        {
            this.type = typeof(T);
        }

        public ProxyObject(ProxyObject parentProxy, IProxyPropertyInfo proxyPropertyInfo) : base(parentProxy, proxyPropertyInfo)
        {
            if (!typeof(T).Equals(proxyPropertyInfo.PropertyType))
                throw new ArgumentException();
        }

        public ProxyObject(ProxyObject proxyParent, IProxyPropertyInfo proxyPropertyInfo, object key) : base(proxyParent, proxyPropertyInfo, key)
        {
            if (!typeof(T).Equals(proxyPropertyInfo.PropertyType))
                throw new ArgumentException();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    foreach (IProxyObject proxy in properties.Values)
                    {
                        proxy.Dispose();
                    }
                    properties.Clear();
                }
                base.Dispose(disposing);
                disposed = true;
            }
        }

        public new T Value
        {
            get { return (T)base.Value; }
            set { base.Value = value; }
        }

        public override IProxyObject GetPropertyProxy(string name)
        {
            if (!this.hasProperty)
                throw new InvalidOperationException("Property not found");

            IProxyObject property;
            if (properties.TryGetValue(name, out property))
                return property;

            IProxyPropertyInfo proxyPropertyInfo = this.ProxyType.GetProperty(name);

            if (proxyPropertyInfo == null)
                throw new InvalidOperationException("Property not found");

            property = this.CreateProxyObject(this, proxyPropertyInfo, null);
            properties[name] = property;
            return property;
        }

        protected virtual IProxyObject CreateProxyObject(IProxyObject parent, IProxyPropertyInfo propertyInfo, object key)
        {
            PropertyInfo item = propertyInfo.PropertyType.GetProperty("Item");
            if (item != null)
            {
                return (IProxyObject)Activator.CreateInstance(typeof(ProxyCollection<>).MakeGenericType(propertyInfo.PropertyType), parent, propertyInfo);
            }
            else
            {
                return (IProxyObject)Activator.CreateInstance(typeof(ProxyObject<>).MakeGenericType(propertyInfo.PropertyType), parent, propertyInfo, key);
            }
        }

        public virtual IProxyObject<E> GetPropertyProxy<E>(string name)
        {
            return (IProxyObject<E>)GetPropertyProxy(name);
        }

        public virtual TResult Invoke<TResult>(string name, params object[] args)
        {
            return (TResult)Invoke(name, args);
        }

        public virtual TResult Invoke<TResult>(string name, Type[] parameterTypes, params object[] args)
        {
            return (TResult)Invoke(name, parameterTypes, args);
        }
    }

    public class ProxyCollection<T> : ProxyObject<T>, IProxyCollection<T>
    {
        private bool disposed = false;
        protected Dictionary<object, IProxyObject> _items = new Dictionary<object, IProxyObject>();
        protected IProxyPropertyInfo _proxyItemPropertyInfo;

        public ProxyCollection(T target) : base(target)
        {
            this.type = typeof(T);
        }

        public ProxyCollection(ProxyObject parentProxy, IProxyPropertyInfo proxyPropertyInfo) : base(parentProxy, proxyPropertyInfo)
        {
            if (!typeof(T).Equals(proxyPropertyInfo.PropertyType))
                throw new ArgumentException();

            this._proxyItemPropertyInfo = this.ProxyType.GetProperty("Item");
            if (this._proxyItemPropertyInfo == null)
                throw new InvalidOperationException("Item not found");
        }

        public T this[object key]
        {
            get { return (T)((IProxyCollection)this)[key]; }
            set { ((IProxyCollection)this)[key] = value; }
        }

        public IProxyObject<T> GetItemProxy(object key)
        {
            return (IProxyObject<T>)((IProxyCollection)this).GetItemProxy(key);
        }

        object IProxyCollection.this[object key]
        {
            get { return this._proxyItemPropertyInfo.GetValue(this.Value, key); }
            set { this._proxyItemPropertyInfo.SetValue(this.Value, value, key); }
        }

        IProxyObject IProxyCollection.GetItemProxy(object key)
        {
            if (!hasProperty)
                throw new InvalidOperationException("Property not found");

            IProxyObject item;
            if (_items.TryGetValue(key, out item))
                return item;

            if (this._proxyItemPropertyInfo == null)
                throw new InvalidOperationException("Property not found");

            item = this.CreateProxyObject(this, this._proxyItemPropertyInfo, key);
            _items[key] = item;
            return item;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    foreach (IProxyObject proxy in _items.Values)
                    {
                        proxy.Dispose();
                    }
                    _items.Clear();
                }
                base.Dispose(disposing);
                disposed = true;
            }
        }
    }
}
