using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Loxodon.Framework.ILRuntimes.Adapters
{   
    public class IAsyncStateMachineAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mMoveNext_0 = new CrossBindingMethodInfo("MoveNext");
        static CrossBindingMethodInfo<System.Runtime.CompilerServices.IAsyncStateMachine> mSetStateMachine_1 = new CrossBindingMethodInfo<System.Runtime.CompilerServices.IAsyncStateMachine>("SetStateMachine");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Runtime.CompilerServices.IAsyncStateMachine);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : System.Runtime.CompilerServices.IAsyncStateMachine, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public void MoveNext()
            {
                mMoveNext_0.Invoke(this.instance);
            }

            public void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine)
            {
                mSetStateMachine_1.Invoke(this.instance, stateMachine);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}

