using System;
using System.Collections.Generic;
using System.Reflection;

using Loxodon.Log;
using Loxodon.Framework.Binding.Reflection;
using Loxodon.Framework.Interactivity;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class InteractionRequestFieldObjectSourceProxy : AbstractFieldObjectSourceProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InteractionRequestFieldObjectSourceProxy));

        private bool disposed = false;
        private IInteractionRequest request;

        protected IProxyInvoker invoker;/* Method Binding */
        protected Delegate handler;/* Delegate Binding */
        protected IScriptInvoker scriptInvoker;/* Script Function Binding  */

        public InteractionRequestFieldObjectSourceProxy(object source, FieldInfo fieldInfo) : base(source, fieldInfo)
        {
            this.BindEvent();
        }

        protected IInteractionRequest Request { get { return this.request; } }

        public override Type Type { get { return typeof(InteractionRequest); } }

        public override object GetValue()
        {
            throw new NotSupportedException();
        }

        public override void SetValue(object value)
        {
            try
            {
                if (this.invoker != null)
                    this.invoker = null;

                if (this.handler != null)
                    this.handler = null;

                if (this.scriptInvoker != null)
                    this.scriptInvoker = null;

                if (value == null)
                    return;

                //Bind Method
                IProxyInvoker invoker = value as IProxyInvoker;
                if (invoker != null)
                {
                    if (this.IsValid(invoker))
                    {
                        this.invoker = invoker;
                        return;
                    }

                    throw new ArgumentException("Bind method failed.the parameter types do not match.");
                }

                //Bind Delegate
                Delegate handler = value as Delegate;
                if (handler != null)
                {
                    if (this.IsValid(handler))
                    {
                        this.handler = handler;
                        return;
                    }

                    throw new ArgumentException("Bind method failed.the parameter types do not match.");
                }

                //Bind Script Function
                IScriptInvoker scriptInvoker = value as IScriptInvoker;
                if (scriptInvoker != null)
                {
                    this.scriptInvoker = scriptInvoker;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("SetValue failed with exception,{0}", e);
            }
        }

        protected virtual bool IsValid(Delegate handler)
        {
            if (handler is EventHandler<InteractionEventArgs>)
                return true;
#if NETFX_CORE
            MethodInfo info = handler.GetMethodInfo();
#else
            MethodInfo info = handler.Method;
#endif
            if (!info.ReturnType.Equals(typeof(void)))
                return false;

            List<Type> parameterTypes = info.GetParameterTypes();
            if (parameterTypes.Count != 2)
                return false;

            return parameterTypes[0].IsAssignableFrom(typeof(object)) && parameterTypes[1].IsAssignableFrom(typeof(InteractionEventArgs));
        }

        protected virtual bool IsValid(IProxyInvoker invoker)
        {
            MethodInfo info = invoker.ProxyMethodInfo.MethodInfo;
            if (!info.ReturnType.Equals(typeof(void)))
                return false;

            List<Type> parameterTypes = info.GetParameterTypes();
            if (parameterTypes.Count != 2)
                return false;

            return parameterTypes[0].IsAssignableFrom(typeof(object)) && parameterTypes[1].IsAssignableFrom(typeof(InteractionEventArgs));
        }

        protected virtual void BindEvent()
        {
            if (this.request != null)
            {
                this.request.Raised -= OnRaised;
                this.request = null;
            }

            this.request = (IInteractionRequest)proxyField.GetValue(this.Source);
            if (this.request == null)
                return;

            this.request.Raised += OnRaised;
        }

        protected virtual void UnbindEvent()
        {
            if (this.request != null)
                this.request.Raised -= OnRaised;
        }

        protected virtual void OnRaised(object sender, InteractionEventArgs args)
        {
            try
            {
                if (this.invoker != null)
                {
                    this.invoker.Invoke(sender, args);
                    return;
                }

                if (this.handler != null)
                {
                    if (this.handler is EventHandler<InteractionEventArgs>)
                        (this.handler as EventHandler<InteractionEventArgs>)(sender, args);
                    else {
                        this.handler.DynamicInvoke(sender, args);
                    }
                    return;
                }

                if (this.scriptInvoker != null)
                {
                    this.scriptInvoker.Invoke(sender, args);
                    return;
                }
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                this.UnbindEvent();
                this.handler = null;
                this.scriptInvoker = null;
                this.invoker = null;
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
