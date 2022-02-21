using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Loxodon.Framework.ILRuntimes.Adapters
{
    public class UIBehaviourAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mAwake_0 = new CrossBindingMethodInfo("Awake");
        static CrossBindingMethodInfo mOnEnable_1 = new CrossBindingMethodInfo("OnEnable");
        static CrossBindingMethodInfo mStart_2 = new CrossBindingMethodInfo("Start");
        static CrossBindingMethodInfo mOnDisable_3 = new CrossBindingMethodInfo("OnDisable");
        static CrossBindingMethodInfo mOnDestroy_4 = new CrossBindingMethodInfo("OnDestroy");
        static CrossBindingFunctionInfo<System.Boolean> mIsActive_5 = new CrossBindingFunctionInfo<System.Boolean>("IsActive");
        static CrossBindingMethodInfo mOnValidate_6 = new CrossBindingMethodInfo("OnValidate");
        static CrossBindingMethodInfo mReset_7 = new CrossBindingMethodInfo("Reset");
        static CrossBindingMethodInfo mOnRectTransformDimensionsChange_8 = new CrossBindingMethodInfo("OnRectTransformDimensionsChange");
        static CrossBindingMethodInfo mOnBeforeTransformParentChanged_9 = new CrossBindingMethodInfo("OnBeforeTransformParentChanged");
        static CrossBindingMethodInfo mOnTransformParentChanged_10 = new CrossBindingMethodInfo("OnTransformParentChanged");
        static CrossBindingMethodInfo mOnDidApplyAnimationProperties_11 = new CrossBindingMethodInfo("OnDidApplyAnimationProperties");
        static CrossBindingMethodInfo mOnCanvasGroupChanged_12 = new CrossBindingMethodInfo("OnCanvasGroupChanged");
        static CrossBindingMethodInfo mOnCanvasHierarchyChanged_13 = new CrossBindingMethodInfo("OnCanvasHierarchyChanged");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(UnityEngine.EventSystems.UIBehaviour);
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

        public class Adapter : UnityEngine.EventSystems.UIBehaviour, CrossBindingAdaptorType, IBehaviourAdapter
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

            public ILTypeInstance ILInstance { get { return this.instance; } set { this.instance = value; } }

            public AppDomain AppDomain { get { return this.appdomain; } set { this.appdomain = value; } }

            protected override void Awake()
            {
                if (mAwake_0.CheckShouldInvokeBase(this.instance))
                    base.Awake();
                else
                    mAwake_0.Invoke(this.instance);
            }

            protected override void OnEnable()
            {
                if (mOnEnable_1.CheckShouldInvokeBase(this.instance))
                    base.OnEnable();
                else
                    mOnEnable_1.Invoke(this.instance);
            }

            protected override void Start()
            {
                if (mStart_2.CheckShouldInvokeBase(this.instance))
                    base.Start();
                else
                    mStart_2.Invoke(this.instance);
            }

            protected override void OnDisable()
            {
                if (mOnDisable_3.CheckShouldInvokeBase(this.instance))
                    base.OnDisable();
                else
                    mOnDisable_3.Invoke(this.instance);
            }

            protected override void OnDestroy()
            {
                if (mOnDestroy_4.CheckShouldInvokeBase(this.instance))
                    base.OnDestroy();
                else
                    mOnDestroy_4.Invoke(this.instance);
            }

            //public override System.Boolean IsActive()
            //{
            //    if (mIsActive_5.CheckShouldInvokeBase(this.instance))
            //        return base.IsActive();
            //    else
            //        return mIsActive_5.Invoke(this.instance);
            //}

            //protected override void OnValidate()
            //{
            //    if (mOnValidate_6.CheckShouldInvokeBase(this.instance))
            //        base.OnValidate();
            //    else
            //        mOnValidate_6.Invoke(this.instance);
            //}

            //protected override void Reset()
            //{
            //    if (mReset_7.CheckShouldInvokeBase(this.instance))
            //        base.Reset();
            //    else
            //        mReset_7.Invoke(this.instance);
            //}

            //protected override void OnRectTransformDimensionsChange()
            //{
            //    if (mOnRectTransformDimensionsChange_8.CheckShouldInvokeBase(this.instance))
            //        base.OnRectTransformDimensionsChange();
            //    else
            //        mOnRectTransformDimensionsChange_8.Invoke(this.instance);
            //}

            //protected override void OnBeforeTransformParentChanged()
            //{
            //    if (mOnBeforeTransformParentChanged_9.CheckShouldInvokeBase(this.instance))
            //        base.OnBeforeTransformParentChanged();
            //    else
            //        mOnBeforeTransformParentChanged_9.Invoke(this.instance);
            //}

            //protected override void OnTransformParentChanged()
            //{
            //    if (mOnTransformParentChanged_10.CheckShouldInvokeBase(this.instance))
            //        base.OnTransformParentChanged();
            //    else
            //        mOnTransformParentChanged_10.Invoke(this.instance);
            //}

            //protected override void OnDidApplyAnimationProperties()
            //{
            //    if (mOnDidApplyAnimationProperties_11.CheckShouldInvokeBase(this.instance))
            //        base.OnDidApplyAnimationProperties();
            //    else
            //        mOnDidApplyAnimationProperties_11.Invoke(this.instance);
            //}

            //protected override void OnCanvasGroupChanged()
            //{
            //    if (mOnCanvasGroupChanged_12.CheckShouldInvokeBase(this.instance))
            //        base.OnCanvasGroupChanged();
            //    else
            //        mOnCanvasGroupChanged_12.Invoke(this.instance);
            //}

            //protected override void OnCanvasHierarchyChanged()
            //{
            //    if (mOnCanvasHierarchyChanged_13.CheckShouldInvokeBase(this.instance))
            //        base.OnCanvasHierarchyChanged();
            //    else
            //        mOnCanvasHierarchyChanged_13.Invoke(this.instance);
            //}

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

            void IBehaviourAdapter.Awake()
            {
                this.Awake();
                this.OnEnable();
            }
        }
    }
}

