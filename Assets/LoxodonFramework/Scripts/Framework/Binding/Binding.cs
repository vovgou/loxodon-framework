using System;
#if NETFX_CORE
using System.Reflection;
#endif

using Loxodon.Log;
using Loxodon.Framework.Binding.Proxy;
using Loxodon.Framework.Binding.Converters;
using Loxodon.Framework.Binding.Proxy.Sources;
using Loxodon.Framework.Binding.Proxy.Targets;
using UnityEngine.Events;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Binding
{
    public class Binding : AbstractBinding
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Binding));

        private readonly ISourceProxyFactory sourceProxyFactory;
        private readonly ITargetProxyFactory targetProxyFactory;

        private bool disposed = false;
        private BindingMode bindingMode = BindingMode.Default;
        private BindingDescription bindingDescription;
        private ISourceProxy sourceProxy;
        private ITargetProxy targetProxy;

        private EventHandler<EventArgs> sourceValueChangedHandler;
        private EventHandler<ValueChangedEventArgs> targetValueChangedHandler;

        private IConverter converter;

        public Binding(object source, object target, BindingDescription bindingDescription, ISourceProxyFactory sourceProxyFactory, ITargetProxyFactory targetProxyFactory) : base(source, target)
        {
            this.bindingDescription = bindingDescription;

            this.converter = bindingDescription.Converter;
            this.sourceProxyFactory = sourceProxyFactory;
            this.targetProxyFactory = targetProxyFactory;

            this.CreateTargetProxy(this.Target, this.bindingDescription);
            this.CreateSourceProxy(this.DataContext, this.bindingDescription.Source);
            this.UpdateDataOnBind();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                this.DisposeSourceProxy();
                this.DisposeTargetProxy();
                disposed = true;
                base.Dispose(disposing);
            }
        }

        protected override void OnDataContextChanged()
        {
            if (this.bindingDescription.Source.IsStatic)
                return;

            this.CreateSourceProxy(this.DataContext, this.bindingDescription.Source);
            this.UpdateDataOnBind();
        }

        protected BindingMode BindingMode
        {
            get
            {
                if (this.bindingMode != BindingMode.Default)
                    return this.bindingMode;

                this.bindingMode = this.bindingDescription.Mode;
                if (bindingMode == BindingMode.Default)
                    bindingMode = this.targetProxy.DefaultMode;

                if (bindingMode == BindingMode.Default && log.IsWarnEnabled)
                    log.WarnFormat("Not set the BindingMode!");

                //if (mode == BindingMode.TwoWay) {
                //    if (!(this._sourceProxy is IModifiable))
                //        mode = BindingMode.OneWay;

                //    if (!(this._sourceProxy is IModifiable) && !(this._sourceProxy is INotifiable<System.EventArgs>))
                //        mode = BindingMode.OneTime;

                //    if ((this._sourceProxy is IModifiable) && !(this._sourceProxy is INotifiable<System.EventArgs>))
                //        mode = BindingMode.OneWayToSource;
                //}

                return this.bindingMode;
            }
        }

        protected void CreateSourceProxy(object source, SourceDescription description)
        {
            this.DisposeSourceProxy();

            this.sourceProxy = this.sourceProxyFactory.CreateProxy(description.IsStatic ? null : source, description);

            if (this.IsSubscribeSourceValueChanged(this.BindingMode) && this.sourceProxy is INotifiable<EventArgs>)
            {
                this.sourceValueChangedHandler = (sender, args) =>
                {
                    var value = this.sourceProxy.GetValue();
                    this.UpdateTargetFromSource(value);
                };

                (this.sourceProxy as INotifiable<EventArgs>).ValueChanged += this.sourceValueChangedHandler;
            }
        }

        protected void DisposeSourceProxy()
        {
            try
            {
                if (this.sourceProxy != null)
                {
                    if (this.sourceValueChangedHandler != null)
                    {
                        (this.sourceProxy as INotifiable<EventArgs>).ValueChanged -= this.sourceValueChangedHandler;
                        this.sourceValueChangedHandler = null;
                    }

                    this.sourceProxy.Dispose();
                    this.sourceProxy = null;
                }
            }
            catch (Exception) { }
        }

        protected void CreateTargetProxy(object target, BindingDescription description)
        {
            this.DisposeTargetProxy();

            this.targetProxy = this.targetProxyFactory.CreateProxy(target, description);

            if (this.IsSubscribeTargetValueChanged(this.BindingMode) && this.targetProxy is INotifiable<ValueChangedEventArgs>)
            {
                this.targetValueChangedHandler = (sender, args) => this.UpdateSourceFromTarget(args.Value);
                (this.targetProxy as INotifiable<ValueChangedEventArgs>).ValueChanged += this.targetValueChangedHandler;
            }
        }

        protected void DisposeTargetProxy()
        {
            try
            {
                if (this.targetProxy != null)
                {
                    if (this.targetValueChangedHandler != null)
                    {
                        (this.targetProxy as INotifiable<ValueChangedEventArgs>).ValueChanged -= this.targetValueChangedHandler;
                        this.targetValueChangedHandler = null;
                    }
                    this.targetProxy.Dispose();
                    this.targetProxy = null;
                }
            }
            catch (Exception) { }
        }

        protected void UpdateDataOnBind()
        {
            try
            {
                if (this.UpdateTargetOnFirstBind(this.BindingMode) && this.sourceProxy != null)
                {
                    var value = this.sourceProxy.GetValue();
                    this.UpdateTargetFromSource(value);
                }

                if (this.UpdateSourceOnFirstBind(this.BindingMode) && this.targetProxy != null && this.targetProxy is IObtainable)
                {
                    var value = (this.targetProxy as IObtainable).GetValue();
                    this.UpdateSourceFromTarget(value);
                }
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("An exception occurs in UpdateTargetOnBind.exception: {0}", e);
            }
        }

        protected void UpdateTargetFromSource(object value)
        {
            try
            {
                IModifiable modifier = this.targetProxy as IModifiable;
                if (modifier == null)
                    return;

                if (value == ReturnObject.NOTHING)
                    return;

                if (value == ReturnObject.UNSET)
                {
                    value = this.targetProxy.Type.CreateDefault();
                }
                else if (this.converter != null)
                {
                    value = this.converter.Convert(value);
                }

                if (!typeof(UnityEventBase).IsAssignableFrom(this.targetProxy.Type))
                    value = this.targetProxy.Type.ToSafe(value);

                Executors.RunOnMainThread(() => { modifier.SetValue(value); });

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("An exception occurs when the target property is updated.Please check this binding \"{0}\".exception: {1}", this.bindingDescription.ToString(), e);
            }
        }

        private void UpdateSourceFromTarget(object value)
        {
            try
            {
                IModifiable modifier = this.sourceProxy as IModifiable;
                if (modifier == null)
                    return;

                if (value == ReturnObject.NOTHING)
                    return;

                if (value == ReturnObject.UNSET)
                    return;

                if (this.converter != null)
                    value = this.converter.ConvertBack(value);

                value = this.sourceProxy.Type.ToSafe(value);

                modifier.SetValue(value);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("An exception occurs when the source property is updated.Please check this binding \"{0}\".exception: {1}", this.bindingDescription.ToString(), e);
            }
        }


        protected bool IsSubscribeSourceValueChanged(BindingMode bindingMode)
        {
            switch (bindingMode)
            {
                case BindingMode.Default:
                    return true;

                case BindingMode.OneWay:
                case BindingMode.TwoWay:
                    return true;

                case BindingMode.OneTime:
                case BindingMode.OneWayToSource:
                    return false;

                default:
                    throw new BindingException("Unexpected BindingMode");
            }
        }

        protected bool IsSubscribeTargetValueChanged(BindingMode bindingMode)
        {
            switch (bindingMode)
            {
                case BindingMode.Default:
                    return true;

                case BindingMode.OneWay:
                case BindingMode.OneTime:
                    return false;

                case BindingMode.TwoWay:
                case BindingMode.OneWayToSource:
                    return true;

                default:
                    throw new BindingException("Unexpected BindingMode");
            }
        }

        protected bool UpdateTargetOnFirstBind(BindingMode bindingMode)
        {
            switch (bindingMode)
            {
                case BindingMode.Default:
                    return true;

                case BindingMode.OneWay:
                case BindingMode.OneTime:
                case BindingMode.TwoWay:
                    return true;

                case BindingMode.OneWayToSource:
                    return false;

                default:
                    throw new BindingException("Unexpected BindingMode");
            }
        }

        protected bool UpdateSourceOnFirstBind(BindingMode bindingMode)
        {
            switch (bindingMode)
            {
                case BindingMode.OneWayToSource:
                    return true;

                case BindingMode.Default:
                    return false;

                case BindingMode.OneWay:
                case BindingMode.OneTime:
                case BindingMode.TwoWay:
                    return false;

                default:
                    throw new BindingException("Unexpected BindingMode");
            }
        }
    }
}
