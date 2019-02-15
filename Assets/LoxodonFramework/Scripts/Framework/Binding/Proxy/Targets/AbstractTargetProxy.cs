using System;

using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Targets
{

    public abstract class AbstractTargetProxy : AbstractProxy, ITargetProxy,IModifiable, INotifiable<ValueChangedEventArgs>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AbstractTargetProxy));

        private bool disposed = false;
        private readonly WeakReference target;
        private bool subscribed = false;
        protected bool isUpdatingSource;
        protected bool isUpdatingTarget;

        private readonly object eventLock = new object();
        private EventHandler<ValueChangedEventArgs> valueChanged;
        private string name;
        public AbstractTargetProxy(object target)
        {
            if (target != null)
            {
                this.target = new WeakReference(target, true);
                this.name = target.ToString();
            }
        }

        public virtual BindingMode DefaultMode { get { return BindingMode.OneWay; } }

        public virtual Type Type { get { return typeof(object); } }

        public virtual object Target { get { return this.target != null && this.target.IsAlive ? this.target.Target : null; } }

        public event EventHandler<ValueChangedEventArgs> ValueChanged
        {
            add
            {
                lock (eventLock)
                {
                    this.valueChanged += value;

                    if (this.valueChanged != null && !this.subscribed)
                        this.Subscribe();
                }
            }

            remove
            {
                lock (eventLock)
                {
                    this.valueChanged -= value;

                    if (this.valueChanged == null && this.subscribed)
                        this.Unsubscribe();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                this.Unsubscribe();
                base.Dispose(disposing);
            }
        }

        protected void Subscribe()
        {
            try
            {
                if (subscribed)
                    return;

                var target = this.Target;
                if (target == null)
                    return;

                this.subscribed = true;
                this.DoSubscribeForValueChange(target);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0} Subscribe Exception:{1}", this.name, e);
            }
        }

        protected virtual void DoSubscribeForValueChange(object target)
        {
        }

        protected void Unsubscribe()
        {
            try
            {
                if (!subscribed)
                    return;

                var target = this.Target;
                if (target == null)
                    return;

                this.subscribed = false;
                this.DoUnsubscribeForValueChange(target);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0} Unsubscribe Exception:{1}", this.name, e);
            }
        }
        protected virtual void DoUnsubscribeForValueChange(object target)
        {
        }

        public void SetValue(object value)
        {
            var target = this.Target;
            if (target == null)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Weak target is null in the \"{0}\" type,skipping set.", this.GetType().Name);

                return;
            }

            if (this.isUpdatingSource)
                return;

            try
            {
                this.isUpdatingTarget = true;
                this.SetValueImpl(target, value);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Set value error  in {0},skipping set,exception:{1}", this.GetType().Name, e);
            }
            finally
            {
                this.isUpdatingTarget = false;
            }
        }

        protected abstract void SetValueImpl(object target, object value);

        protected void RaiseValueChanged(object newValue)
        {
            if (this.isUpdatingTarget || this.isUpdatingSource)
                return;

            try
            {
                this.isUpdatingSource = true;
                var handler = this.valueChanged;
                if (handler != null)
                    handler(this, new ValueChangedEventArgs(newValue));
            }
            finally
            {
                this.isUpdatingSource = false;
            }
        }
    }

    public abstract class AbstractTargetProxy<T> : AbstractTargetProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AbstractTargetProxy<T>));

        public AbstractTargetProxy(object target) : base(target)
        {
        }

        public new void SetValue(object value)
        {
            var target = this.Target;
            if (target == null)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Weak target is null in {0},skipping set", this.GetType().Name);

                return;
            }

            if (this.isUpdatingSource)
                return;

            try
            {
                this.isUpdatingTarget = true;
                this.SetValueImpl((T)target, value);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("set value error  in {0},skipping set,exception:{1}", this.GetType().Name, e);
            }
            finally
            {
                this.isUpdatingTarget = false;
            }
        }

        protected abstract void SetValueImpl(T target, object value);
    }
}
