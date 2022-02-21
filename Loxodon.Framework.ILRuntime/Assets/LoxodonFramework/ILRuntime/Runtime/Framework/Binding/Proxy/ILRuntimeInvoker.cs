using ILRuntime.CLR.Method;
using Loxodon.Log;
using System;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;


namespace Loxodon.Framework.Binding.Proxy
{
    public class ILRuntimeInvoker : IScriptInvoker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ILRuntimeInvoker));

        private readonly AppDomain appdomain;
        private readonly WeakReference target;
        private readonly IMethod method;
        public ILRuntimeInvoker(AppDomain appdomain, object target, IMethod method)
        {
            if (target == null)
                throw new ArgumentNullException("target", "Unable to bind to target as it's null");

            this.appdomain = appdomain;
            this.target = new WeakReference(target, false);
            this.method = method;
        }

        public object Target { get { return this.target != null && this.target.IsAlive ? this.target.Target : null; } }

        public object Invoke(params object[] args)
        {
            try
            {
                var target = this.Target;
                if (target == null)
                    return null;

                return appdomain.Invoke(this.method, target, args);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }
    }
}
