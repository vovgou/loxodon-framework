using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class EmptyTargetProxy : AbstractTargetProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmptyTargetProxy));

        public EmptyTargetProxy(object target) : base(target)
        {
        }

        public override BindingMode DefaultMode { get { return BindingMode.OneTime; } }

        protected override void SetValueImpl(object target, object value)
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty target proxy,If you see this, then didn't find the right target proxy factory.");
        }

        protected override void DoSubscribeForValueChange(object target)
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty target proxy,If you see this, then didn't find the right target proxy factory.");
        }
    }
}
