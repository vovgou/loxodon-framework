using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Loxodon.Framework.ILRuntimes.Adapters
{
    public interface IBehaviourAdapter: CrossBindingAdaptorType
    {
        new ILTypeInstance ILInstance { get; set; }

        AppDomain AppDomain { get; set; }

        void Awake();

    }
}
