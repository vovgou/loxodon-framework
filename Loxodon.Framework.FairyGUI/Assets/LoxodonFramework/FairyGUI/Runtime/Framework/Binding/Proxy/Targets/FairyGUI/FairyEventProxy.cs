/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using FairyGUI;
using Loxodon.Framework.Binding.Reflection;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Execution;
using Loxodon.Log;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class FairyEventProxy : EventTargetProxyBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FairyEventProxy));

        private bool disposed = false;
        protected ICommand command;/* Command Binding */
        protected IInvoker invoker;/* Method Binding or Lua Function Binding */
        protected Delegate handler;/* Delegate Binding */

        protected EventListener listener;

        public FairyEventProxy(object target, EventListener listener) : base(target)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");

            this.listener = listener;
            this.BindEvent();
        }

        public override BindingMode DefaultMode { get { return BindingMode.OneWay; } }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                UnbindCommand(this.command);
                this.UnbindEvent();
                disposed = true;
                base.Dispose(disposing);
            }
        }

        public override Type Type { get { return typeof(EventListener); } }

        protected virtual void BindEvent()
        {
            this.listener.Add(OnEvent);
        }

        protected virtual void UnbindEvent()
        {
            this.listener.Remove(OnEvent);
        }

        protected virtual bool IsValid(Delegate handler)
        {
            if (handler is Action || handler is EventCallback0)
                return true;
#if NETFX_CORE
            MethodInfo info = handler.GetMethodInfo();
#else
            MethodInfo info = handler.Method;
#endif
            if (!info.ReturnType.Equals(typeof(void)))
                return false;

            List<Type> parameterTypes = info.GetParameterTypes();
            if (parameterTypes.Count == 0)
                return true;

            return false;
        }

        protected virtual bool IsValid(IProxyInvoker invoker)
        {
            IProxyMethodInfo info = invoker.ProxyMethodInfo;
            if (!info.ReturnType.Equals(typeof(void)))
                return false;

            var parameters = info.Parameters;
            if (parameters != null && parameters.Length != 0)
                return false;
            return true;
        }

        protected virtual void OnEvent()
        {
            try
            {
                if (this.command != null)
                {
                    this.command.Execute(null);
                    return;
                }

                if (this.invoker != null)
                {
                    this.invoker.Invoke();
                    return;
                }

                if (this.handler != null)
                {
                    if (this.handler is Action)
                    {
                        (this.handler as Action)();
                    }
                    else if (this.handler is EventCallback0)
                    {
                        (this.handler as EventCallback0)();
                    }
                    else
                    {
                        this.handler.DynamicInvoke();
                    }
                    return;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("{0}", e);
            }
        }

        public override void SetValue(object value)
        {
            var target = this.Target;
            if (target == null)
                return;

            if (this.command != null)
            {
                UnbindCommand(this.command);
                this.command = null;
            }

            if (this.invoker != null)
                this.invoker = null;

            if (this.handler != null)
                this.handler = null;

            if (value == null)
                return;

            //Bind Command
            ICommand command = value as ICommand;
            if (command != null)
            {
                this.command = command;
                BindCommand(this.command);
                UpdateTargetEnable();
                return;
            }

            //Bind Method
            IProxyInvoker proxyInvoker = value as IProxyInvoker;
            if (proxyInvoker != null)
            {
                if (this.IsValid(proxyInvoker))
                {
                    this.invoker = proxyInvoker;
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
            IInvoker invoker = value as IInvoker;
            if (invoker != null)
            {
                this.invoker = invoker;
            }
        }

        public override void SetValue<TValue>(TValue value)
        {
            this.SetValue((object)value);
        }

        protected virtual void OnCanExecuteChanged(object sender, EventArgs e)
        {
            Executors.RunOnMainThread(UpdateTargetEnable);
        }

        protected virtual void UpdateTargetEnable()
        {
            var target = this.Target;
            if (target == null || !(target is GObject))
                return;

            bool value = this.command == null ? false : this.command.CanExecute(null);
            ((GObject)target).enabled = value;
        }

        protected virtual void BindCommand(ICommand command)
        {
            if (command == null)
                return;

            command.CanExecuteChanged += OnCanExecuteChanged;
        }

        protected virtual void UnbindCommand(ICommand command)
        {
            if (command == null)
                return;

            command.CanExecuteChanged -= OnCanExecuteChanged;
        }
    }
}