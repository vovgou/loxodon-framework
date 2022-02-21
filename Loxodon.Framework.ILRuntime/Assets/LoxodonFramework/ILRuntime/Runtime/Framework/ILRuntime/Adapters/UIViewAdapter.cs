using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Loxodon.Framework.ILRuntimes.Adapters
{
    public class UIViewAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.String> mget_Name_0 = new CrossBindingFunctionInfo<System.String>("get_Name");
        static CrossBindingMethodInfo<System.String> mset_Name_1 = new CrossBindingMethodInfo<System.String>("set_Name");
        static CrossBindingFunctionInfo<UnityEngine.Transform> mget_Parent_2 = new CrossBindingFunctionInfo<UnityEngine.Transform>("get_Parent");
        static CrossBindingFunctionInfo<UnityEngine.GameObject> mget_Owner_3 = new CrossBindingFunctionInfo<UnityEngine.GameObject>("get_Owner");
        static CrossBindingFunctionInfo<UnityEngine.Transform> mget_Transform_4 = new CrossBindingFunctionInfo<UnityEngine.Transform>("get_Transform");
        static CrossBindingFunctionInfo<UnityEngine.RectTransform> mget_RectTransform_5 = new CrossBindingFunctionInfo<UnityEngine.RectTransform>("get_RectTransform");
        static CrossBindingFunctionInfo<System.Boolean> mget_Visibility_6 = new CrossBindingFunctionInfo<System.Boolean>("get_Visibility");
        static CrossBindingMethodInfo<System.Boolean> mset_Visibility_7 = new CrossBindingMethodInfo<System.Boolean>("set_Visibility");
        static CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation> mget_EnterAnimation_8 = new CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation>("get_EnterAnimation");
        static CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation> mset_EnterAnimation_9 = new CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation>("set_EnterAnimation");
        static CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation> mget_ExitAnimation_10 = new CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation>("get_ExitAnimation");
        static CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation> mset_ExitAnimation_11 = new CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation>("set_ExitAnimation");
        static CrossBindingMethodInfo mOnEnable_12 = new CrossBindingMethodInfo("OnEnable");
        static CrossBindingMethodInfo mOnDisable_13 = new CrossBindingMethodInfo("OnDisable");
        static CrossBindingFunctionInfo<System.Single> mget_Alpha_14 = new CrossBindingFunctionInfo<System.Single>("get_Alpha");
        static CrossBindingMethodInfo<System.Single> mset_Alpha_15 = new CrossBindingMethodInfo<System.Single>("set_Alpha");
        static CrossBindingFunctionInfo<System.Boolean> mget_Interactable_16 = new CrossBindingFunctionInfo<System.Boolean>("get_Interactable");
        static CrossBindingMethodInfo<System.Boolean> mset_Interactable_17 = new CrossBindingMethodInfo<System.Boolean>("set_Interactable");
        static CrossBindingFunctionInfo<UnityEngine.CanvasGroup> mget_CanvasGroup_18 = new CrossBindingFunctionInfo<UnityEngine.CanvasGroup>("get_CanvasGroup");
        static CrossBindingFunctionInfo<Loxodon.Framework.Views.IAttributes> mget_ExtraAttributes_19 = new CrossBindingFunctionInfo<Loxodon.Framework.Views.IAttributes>("get_ExtraAttributes");
        static CrossBindingMethodInfo mOnVisibilityChanged_20 = new CrossBindingMethodInfo("OnVisibilityChanged");
        static CrossBindingMethodInfo mAwake_21 = new CrossBindingMethodInfo("Awake");
        static CrossBindingMethodInfo mStart_22 = new CrossBindingMethodInfo("Start");
        static CrossBindingMethodInfo mOnDestroy_23 = new CrossBindingMethodInfo("OnDestroy");
        static CrossBindingFunctionInfo<System.Boolean> mIsActive_24 = new CrossBindingFunctionInfo<System.Boolean>("IsActive");
        static CrossBindingMethodInfo mOnValidate_25 = new CrossBindingMethodInfo("OnValidate");
        static CrossBindingMethodInfo mReset_26 = new CrossBindingMethodInfo("Reset");
        static CrossBindingMethodInfo mOnRectTransformDimensionsChange_27 = new CrossBindingMethodInfo("OnRectTransformDimensionsChange");
        static CrossBindingMethodInfo mOnBeforeTransformParentChanged_28 = new CrossBindingMethodInfo("OnBeforeTransformParentChanged");
        static CrossBindingMethodInfo mOnTransformParentChanged_29 = new CrossBindingMethodInfo("OnTransformParentChanged");
        static CrossBindingMethodInfo mOnDidApplyAnimationProperties_30 = new CrossBindingMethodInfo("OnDidApplyAnimationProperties");
        static CrossBindingMethodInfo mOnCanvasGroupChanged_31 = new CrossBindingMethodInfo("OnCanvasGroupChanged");
        static CrossBindingMethodInfo mOnCanvasHierarchyChanged_32 = new CrossBindingMethodInfo("OnCanvasHierarchyChanged");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Loxodon.Framework.Views.UIView);
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

        public class Adapter : Loxodon.Framework.Views.UIView, CrossBindingAdaptorType, IBehaviourAdapter
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

            protected override void OnEnable()
            {
                if (this.instance == null)
                    return;

                if (mOnEnable_12.CheckShouldInvokeBase(this.instance))
                    base.OnEnable();
                else
                    mOnEnable_12.Invoke(this.instance);
            }

            protected override void OnDisable()
            {
                if (this.instance == null)
                    return;

                if (mOnDisable_13.CheckShouldInvokeBase(this.instance))
                    base.OnDisable();
                else
                    mOnDisable_13.Invoke(this.instance);
            }

            protected override void OnVisibilityChanged()
            {
                if (this.instance == null)
                    return;

                if (mOnVisibilityChanged_20.CheckShouldInvokeBase(this.instance))
                    base.OnVisibilityChanged();
                else
                    mOnVisibilityChanged_20.Invoke(this.instance);
            }

            protected override void Awake()
            {
                if (this.instance == null)
                    return;

                if (mAwake_21.CheckShouldInvokeBase(this.instance))
                    base.Awake();
                else
                    mAwake_21.Invoke(this.instance);
            }

            protected override void Start()
            {
                if (mStart_22.CheckShouldInvokeBase(this.instance))
                    base.Start();
                else
                    mStart_22.Invoke(this.instance);
            }

            protected override void OnDestroy()
            {
                if (mOnDestroy_23.CheckShouldInvokeBase(this.instance))
                    base.OnDestroy();
                else
                    mOnDestroy_23.Invoke(this.instance);
            }

            //public override System.Boolean IsActive()
            //{
            //    if (mIsActive_24.CheckShouldInvokeBase(this.instance))
            //        return base.IsActive();
            //    else
            //        return mIsActive_24.Invoke(this.instance);
            //}

            //protected override void OnValidate()
            //{
            //    if (mOnValidate_25.CheckShouldInvokeBase(this.instance))
            //        base.OnValidate();
            //    else
            //        mOnValidate_25.Invoke(this.instance);
            //}

            //protected override void Reset()
            //{
            //    if (mReset_26.CheckShouldInvokeBase(this.instance))
            //        base.Reset();
            //    else
            //        mReset_26.Invoke(this.instance);
            //}

            //protected override void OnRectTransformDimensionsChange()
            //{
            //    if (mOnRectTransformDimensionsChange_27.CheckShouldInvokeBase(this.instance))
            //        base.OnRectTransformDimensionsChange();
            //    else
            //        mOnRectTransformDimensionsChange_27.Invoke(this.instance);
            //}

            //protected override void OnBeforeTransformParentChanged()
            //{
            //    if (mOnBeforeTransformParentChanged_28.CheckShouldInvokeBase(this.instance))
            //        base.OnBeforeTransformParentChanged();
            //    else
            //        mOnBeforeTransformParentChanged_28.Invoke(this.instance);
            //}

            //protected override void OnTransformParentChanged()
            //{
            //    if (mOnTransformParentChanged_29.CheckShouldInvokeBase(this.instance))
            //        base.OnTransformParentChanged();
            //    else
            //        mOnTransformParentChanged_29.Invoke(this.instance);
            //}

            //protected override void OnDidApplyAnimationProperties()
            //{
            //    if (mOnDidApplyAnimationProperties_30.CheckShouldInvokeBase(this.instance))
            //        base.OnDidApplyAnimationProperties();
            //    else
            //        mOnDidApplyAnimationProperties_30.Invoke(this.instance);
            //}

            //protected override void OnCanvasGroupChanged()
            //{
            //    if (mOnCanvasGroupChanged_31.CheckShouldInvokeBase(this.instance))
            //        base.OnCanvasGroupChanged();
            //    else
            //        mOnCanvasGroupChanged_31.Invoke(this.instance);
            //}

            //protected override void OnCanvasHierarchyChanged()
            //{
            //    if (mOnCanvasHierarchyChanged_32.CheckShouldInvokeBase(this.instance))
            //        base.OnCanvasHierarchyChanged();
            //    else
            //        mOnCanvasHierarchyChanged_32.Invoke(this.instance);
            //}

            public override System.String Name
            {
                get
                {
                    if (mget_Name_0.CheckShouldInvokeBase(this.instance))
                        return base.Name;
                    else
                        return mget_Name_0.Invoke(this.instance);

                }
                set
                {
                    if (mset_Name_1.CheckShouldInvokeBase(this.instance))
                        base.Name = value;
                    else
                        mset_Name_1.Invoke(this.instance, value);

                }
            }

            public override UnityEngine.Transform Parent
            {
                get
                {
                    if (mget_Parent_2.CheckShouldInvokeBase(this.instance))
                        return base.Parent;
                    else
                        return mget_Parent_2.Invoke(this.instance);

                }
            }

            public override UnityEngine.GameObject Owner
            {
                get
                {
                    if (mget_Owner_3.CheckShouldInvokeBase(this.instance))
                        return base.Owner;
                    else
                        return mget_Owner_3.Invoke(this.instance);

                }
            }

            public override UnityEngine.Transform Transform
            {
                get
                {
                    if (mget_Transform_4.CheckShouldInvokeBase(this.instance))
                        return base.Transform;
                    else
                        return mget_Transform_4.Invoke(this.instance);

                }
            }

            public override UnityEngine.RectTransform RectTransform
            {
                get
                {
                    if (mget_RectTransform_5.CheckShouldInvokeBase(this.instance))
                        return base.RectTransform;
                    else
                        return mget_RectTransform_5.Invoke(this.instance);

                }
            }

            public override System.Boolean Visibility
            {
                get
                {
                    if (mget_Visibility_6.CheckShouldInvokeBase(this.instance))
                        return base.Visibility;
                    else
                        return mget_Visibility_6.Invoke(this.instance);

                }
                set
                {
                    if (mset_Visibility_7.CheckShouldInvokeBase(this.instance))
                        base.Visibility = value;
                    else
                        mset_Visibility_7.Invoke(this.instance, value);

                }
            }

            public override Loxodon.Framework.Views.Animations.IAnimation EnterAnimation
            {
                get
                {
                    if (mget_EnterAnimation_8.CheckShouldInvokeBase(this.instance))
                        return base.EnterAnimation;
                    else
                        return mget_EnterAnimation_8.Invoke(this.instance);

                }
                set
                {
                    if (mset_EnterAnimation_9.CheckShouldInvokeBase(this.instance))
                        base.EnterAnimation = value;
                    else
                        mset_EnterAnimation_9.Invoke(this.instance, value);

                }
            }

            public override Loxodon.Framework.Views.Animations.IAnimation ExitAnimation
            {
                get
                {
                    if (mget_ExitAnimation_10.CheckShouldInvokeBase(this.instance))
                        return base.ExitAnimation;
                    else
                        return mget_ExitAnimation_10.Invoke(this.instance);

                }
                set
                {
                    if (mset_ExitAnimation_11.CheckShouldInvokeBase(this.instance))
                        base.ExitAnimation = value;
                    else
                        mset_ExitAnimation_11.Invoke(this.instance, value);

                }
            }

            public override System.Single Alpha
            {
                get
                {
                    if (mget_Alpha_14.CheckShouldInvokeBase(this.instance))
                        return base.Alpha;
                    else
                        return mget_Alpha_14.Invoke(this.instance);

                }
                set
                {
                    if (mset_Alpha_15.CheckShouldInvokeBase(this.instance))
                        base.Alpha = value;
                    else
                        mset_Alpha_15.Invoke(this.instance, value);

                }
            }

            public override System.Boolean Interactable
            {
                get
                {
                    if (mget_Interactable_16.CheckShouldInvokeBase(this.instance))
                        return base.Interactable;
                    else
                        return mget_Interactable_16.Invoke(this.instance);

                }
                set
                {
                    if (mset_Interactable_17.CheckShouldInvokeBase(this.instance))
                        base.Interactable = value;
                    else
                        mset_Interactable_17.Invoke(this.instance, value);

                }
            }

            public override UnityEngine.CanvasGroup CanvasGroup
            {
                get
                {
                    if (mget_CanvasGroup_18.CheckShouldInvokeBase(this.instance))
                        return base.CanvasGroup;
                    else
                        return mget_CanvasGroup_18.Invoke(this.instance);

                }
            }

            public override Loxodon.Framework.Views.IAttributes ExtraAttributes
            {
                get
                {
                    if (mget_ExtraAttributes_19.CheckShouldInvokeBase(this.instance))
                        return base.ExtraAttributes;
                    else
                        return mget_ExtraAttributes_19.Invoke(this.instance);

                }
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

            void IBehaviourAdapter.Awake()
            {
                this.Awake();
                this.OnEnable();
            }
        }
    }
}

