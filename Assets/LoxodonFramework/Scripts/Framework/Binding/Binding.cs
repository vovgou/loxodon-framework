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
using UnityEngine;

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

        private EventHandler sourceValueChangedHandler;
        private EventHandler targetValueChangedHandler;

        private IConverter converter;

        private object _lock = new object();
        private bool isUpdatingSource;
        private bool isUpdatingTarget;

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

                return this.bindingMode;
            }
        }

        protected void UpdateDataOnBind()
        {
            try
            {
                if (this.UpdateTargetOnFirstBind(this.BindingMode) && this.sourceProxy != null)
                {
                    this.UpdateTargetFromSource();
                }

                if (this.UpdateSourceOnFirstBind(this.BindingMode) && this.targetProxy != null && this.targetProxy is IObtainable)
                {
                    this.UpdateSourceFromTarget();
                }
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("An exception occurs in UpdateTargetOnBind.exception: {0}", e);
            }
        }

        protected void CreateSourceProxy(object source, SourceDescription description)
        {
            this.DisposeSourceProxy();

            this.sourceProxy = this.sourceProxyFactory.CreateProxy(description.IsStatic ? null : source, description);

            if (this.IsSubscribeSourceValueChanged(this.BindingMode) && this.sourceProxy is INotifiable)
            {
                this.sourceValueChangedHandler = (sender, args) => this.UpdateTargetFromSource();
                (this.sourceProxy as INotifiable).ValueChanged += this.sourceValueChangedHandler;
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
                        (this.sourceProxy as INotifiable).ValueChanged -= this.sourceValueChangedHandler;
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

            if (this.IsSubscribeTargetValueChanged(this.BindingMode) && this.targetProxy is INotifiable)
            {
                this.targetValueChangedHandler = (sender, args) => this.UpdateSourceFromTarget();
                (this.targetProxy as INotifiable).ValueChanged += this.targetValueChangedHandler;
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
                        (this.targetProxy as INotifiable).ValueChanged -= this.targetValueChangedHandler;
                        this.targetValueChangedHandler = null;
                    }
                    this.targetProxy.Dispose();
                    this.targetProxy = null;
                }
            }
            catch (Exception) { }
        }

        protected virtual void UpdateTargetFromSource()
        {
            lock (_lock)
            {
                //Run on the main thread
                Executors.RunOnMainThread(() =>
                {
                    try
                    {
                        if (this.isUpdatingSource)
                            return;

                        this.isUpdatingTarget = true;

                        Type valueType = this.sourceProxy.Type;
                        IObtainable obtainable = this.sourceProxy as IObtainable;
                        if (obtainable == null)
                            return;

                        IModifiable modifier = this.targetProxy as IModifiable;
                        if (modifier == null)
                            return;

#if NETFX_CORE
                        TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(valueType);
#else
                        TypeCode typeCode = Type.GetTypeCode(valueType);
#endif
                        switch (typeCode)
                        {
                            case TypeCode.Boolean:
                                {
                                    var value = obtainable.GetValue<bool>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.Byte:
                                {
                                    var value = obtainable.GetValue<byte>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.Char:
                                {
                                    var value = obtainable.GetValue<char>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.DateTime:
                                {
                                    var value = obtainable.GetValue<DateTime>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.Decimal:
                                {
                                    var value = obtainable.GetValue<decimal>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.Double:
                                {
                                    var value = obtainable.GetValue<double>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.Int16:
                                {
                                    var value = obtainable.GetValue<Int16>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.Int32:
                                {
                                    var value = obtainable.GetValue<Int32>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.Int64:
                                {
                                    var value = obtainable.GetValue<Int64>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.SByte:
                                {
                                    var value = obtainable.GetValue<sbyte>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.Single:
                                {
                                    var value = obtainable.GetValue<float>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.String:
                                {
                                    var value = obtainable.GetValue<string>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.UInt16:
                                {
                                    var value = obtainable.GetValue<UInt16>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.UInt32:
                                {
                                    var value = obtainable.GetValue<UInt32>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.UInt64:
                                {
                                    var value = obtainable.GetValue<UInt64>();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                            case TypeCode.Object:
                                {
                                    if (valueType.Equals(typeof(Vector2)))
                                    {
                                        var value = obtainable.GetValue<Vector2>();
                                        this.SetTargetValue(modifier, value);
                                    }
                                    else if (valueType.Equals(typeof(Vector3)))
                                    {
                                        var value = obtainable.GetValue<Vector3>();
                                        this.SetTargetValue(modifier, value);
                                    }
                                    else if (valueType.Equals(typeof(Vector4)))
                                    {
                                        var value = obtainable.GetValue<Vector4>();
                                        this.SetTargetValue(modifier, value);
                                    }
                                    else if (valueType.Equals(typeof(Color)))
                                    {
                                        var value = obtainable.GetValue<Color>();
                                        this.SetTargetValue(modifier, value);
                                    }
                                    else if (valueType.Equals(typeof(Rect)))
                                    {
                                        var value = obtainable.GetValue<Rect>();
                                        this.SetTargetValue(modifier, value);
                                    }
                                    else if (valueType.Equals(typeof(Quaternion)))
                                    {
                                        var value = obtainable.GetValue<Quaternion>();
                                        this.SetTargetValue(modifier, value);
                                    }
                                    else if (valueType.Equals(typeof(Version)))
                                    {
                                        var value = obtainable.GetValue<Version>();
                                        this.SetTargetValue(modifier, value);
                                    }
                                    else
                                    {
                                        var value = obtainable.GetValue();
                                        this.SetTargetValue(modifier, value);
                                    }
                                    break;
                                }
                            default:
                                {
                                    var value = obtainable.GetValue();
                                    this.SetTargetValue(modifier, value);
                                    break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        if (log.IsErrorEnabled)
                            log.ErrorFormat("An exception occurs when the target property is updated.Please check this binding \"{0}\".exception: {1}", this.bindingDescription.ToString(), e);
                    }
                    finally
                    {
                        this.isUpdatingTarget = false;
                    }
                });
            }
        }

        protected virtual void UpdateSourceFromTarget()
        {
            try
            {
                if (this.isUpdatingTarget)
                    return;

                this.isUpdatingSource = true;

                Type valueType = this.targetProxy.Type;
                IObtainable obtainable = this.targetProxy as IObtainable;
                if (obtainable == null)
                    return;

                IModifiable modifier = this.sourceProxy as IModifiable;
                if (modifier == null)
                    return;

#if NETFX_CORE
                TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(valueType);
#else
                TypeCode typeCode = Type.GetTypeCode(valueType);
#endif
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        {
                            var value = obtainable.GetValue<bool>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.Byte:
                        {
                            var value = obtainable.GetValue<byte>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.Char:
                        {
                            var value = obtainable.GetValue<char>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.DateTime:
                        {
                            var value = obtainable.GetValue<DateTime>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.Decimal:
                        {
                            var value = obtainable.GetValue<decimal>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.Double:
                        {
                            var value = obtainable.GetValue<double>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.Int16:
                        {
                            var value = obtainable.GetValue<Int16>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.Int32:
                        {
                            var value = obtainable.GetValue<Int32>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.Int64:
                        {
                            var value = obtainable.GetValue<Int64>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.SByte:
                        {
                            var value = obtainable.GetValue<sbyte>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.Single:
                        {
                            var value = obtainable.GetValue<float>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.String:
                        {
                            var value = obtainable.GetValue<string>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.UInt16:
                        {
                            var value = obtainable.GetValue<UInt16>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.UInt32:
                        {
                            var value = obtainable.GetValue<UInt32>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.UInt64:
                        {
                            var value = obtainable.GetValue<UInt64>();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                    case TypeCode.Object:
                        {
                            if (valueType.Equals(typeof(Vector2)))
                            {
                                var value = obtainable.GetValue<Vector2>();
                                this.SetSourceValue(modifier, value);
                            }
                            else if (valueType.Equals(typeof(Vector3)))
                            {
                                var value = obtainable.GetValue<Vector3>();
                                this.SetSourceValue(modifier, value);
                            }
                            else if (valueType.Equals(typeof(Vector4)))
                            {
                                var value = obtainable.GetValue<Vector4>();
                                this.SetSourceValue(modifier, value);
                            }
                            else if (valueType.Equals(typeof(Color)))
                            {
                                var value = obtainable.GetValue<Color>();
                                this.SetSourceValue(modifier, value);
                            }
                            else if (valueType.Equals(typeof(Rect)))
                            {
                                var value = obtainable.GetValue<Rect>();
                                this.SetSourceValue(modifier, value);
                            }
                            else if (valueType.Equals(typeof(Quaternion)))
                            {
                                var value = obtainable.GetValue<Quaternion>();
                                this.SetSourceValue(modifier, value);
                            }
                            else if (valueType.Equals(typeof(Version)))
                            {
                                var value = obtainable.GetValue<Version>();
                                this.SetSourceValue(modifier, value);
                            }
                            else
                            {
                                var value = obtainable.GetValue();
                                this.SetSourceValue(modifier, value);
                            }
                            break;
                        }
                    default:
                        {
                            var value = obtainable.GetValue();
                            this.SetSourceValue(modifier, value);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("An exception occurs when the source property is updated.Please check this binding \"{0}\".exception: {1}", this.bindingDescription.ToString(), e);
            }
            finally
            {
                this.isUpdatingSource = false;
            }
        }

        protected void SetTargetValue<T>(IModifiable modifier, T value)
        {
            if (this.converter == null && typeof(T).Equals(this.targetProxy.Type))
            {
                modifier.SetValue(value);
                return;
            }

            object safeValue = value;
            if (this.converter != null)
                safeValue = this.converter.Convert(value);

            if (!typeof(UnityEventBase).IsAssignableFrom(this.targetProxy.Type))
                safeValue = this.targetProxy.Type.ToSafe(safeValue);

            modifier.SetValue(safeValue);
        }

        private void SetSourceValue<T>(IModifiable modifier, T value)
        {
            if (this.converter == null && typeof(T).Equals(this.sourceProxy.Type))
            {
                modifier.SetValue(value);
                return;
            }

            object safeValue = value;
            if (this.converter != null)
                safeValue = this.converter.ConvertBack(safeValue);

            safeValue = this.sourceProxy.Type.ToSafe(safeValue);

            modifier.SetValue(safeValue);
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
    }
}
