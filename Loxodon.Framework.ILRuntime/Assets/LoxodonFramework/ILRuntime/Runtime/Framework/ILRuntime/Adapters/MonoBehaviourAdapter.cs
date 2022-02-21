using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Loxodon.Framework.ILRuntimes.Adapters
{
    public class MonoBehaviourAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mAwake_0 = new CrossBindingMethodInfo("Awake");
        static CrossBindingMethodInfo mOnEnable_1 = new CrossBindingMethodInfo("OnEnable");
        static CrossBindingMethodInfo mStart_2 = new CrossBindingMethodInfo("Start");
        static CrossBindingMethodInfo mOnDisable_3 = new CrossBindingMethodInfo("OnDisable");
        static CrossBindingMethodInfo mOnDestroy_4 = new CrossBindingMethodInfo("OnDestroy");

        public override Type BaseCLRType
        {
            get
            {
                return typeof(MonoBehaviour);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : MonoBehaviour, CrossBindingAdaptorType, IBehaviourAdapter
        {
            ILTypeInstance instance;
            AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } set { instance = value; } }

            public AppDomain AppDomain { get { return appdomain; } set { appdomain = value; } }

            public void Awake()
            {
                if (this.instance == null)
                    return;

                if (!mAwake_0.CheckShouldInvokeBase(this.instance))
                    mAwake_0.Invoke(this.instance);
            }

            protected void OnEnable()
            {
                if (this.instance == null)
                    return;

                if (!mOnEnable_1.CheckShouldInvokeBase(this.instance))
                    mOnEnable_1.Invoke(this.instance);
            }

            protected void Start()
            {
                if (!mStart_2.CheckShouldInvokeBase(this.instance))
                    mStart_2.Invoke(this.instance);
            }

            protected void OnDisable()
            {
                if (!mOnDisable_3.CheckShouldInvokeBase(this.instance))
                    mOnDisable_3.Invoke(this.instance);
            }

            protected void OnDestroy()
            {
                if (!mOnDestroy_4.CheckShouldInvokeBase(this.instance))
                    mOnDestroy_4.Invoke(this.instance);
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

