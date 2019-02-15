using System;
using System.Reflection;

using Loxodon.Framework.Binding.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class VoidMethodObjectSourceProxy : AbstractObjectSourceProxy
    {
        protected IProxyInvoker invoker;
        public VoidMethodObjectSourceProxy(MethodInfo methodInfo) : this(null, methodInfo)
        {
        }

        public VoidMethodObjectSourceProxy(object source, MethodInfo methodInfo) : base(source)
        {
            if (!methodInfo.ReturnType.Equals(typeof(void)))
                throw new ArgumentException("methodInfo");

            this.invoker = methodInfo.AsProxy(source);
        }

        public override Type Type { get { return typeof(IProxyInvoker); } }

        public override object GetValue()
        {
            return this.invoker;
        }

        public override void SetValue(object value)
        {
            throw new NotSupportedException();
        }
    }
}
