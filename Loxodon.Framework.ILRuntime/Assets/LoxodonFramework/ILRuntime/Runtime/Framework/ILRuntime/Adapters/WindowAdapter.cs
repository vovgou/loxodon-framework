using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Loxodon.Framework.ILRuntimes.Adapters
{   
    public class WindowAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mOnEnable_0 = new CrossBindingMethodInfo("OnEnable");
        static CrossBindingMethodInfo mOnDisable_1 = new CrossBindingMethodInfo("OnDisable");
        static CrossBindingFunctionInfo<System.Boolean, Loxodon.Framework.Asynchronous.IAsyncResult> mActivate_2 = new CrossBindingFunctionInfo<System.Boolean, Loxodon.Framework.Asynchronous.IAsyncResult>("Activate");
        static CrossBindingFunctionInfo<System.Boolean, Loxodon.Framework.Asynchronous.IAsyncResult> mPassivate_3 = new CrossBindingFunctionInfo<System.Boolean, Loxodon.Framework.Asynchronous.IAsyncResult>("Passivate");
        static CrossBindingMethodInfo mOnActivatedChanged_4 = new CrossBindingMethodInfo("OnActivatedChanged");
        static CrossBindingMethodInfo<Loxodon.Framework.Views.IBundle> mOnCreate_5 = new CrossBindingMethodInfo<Loxodon.Framework.Views.IBundle>("OnCreate");
        static CrossBindingFunctionInfo<System.Boolean, Loxodon.Framework.Asynchronous.IAsyncResult> mDoShow_6 = new CrossBindingFunctionInfo<System.Boolean, Loxodon.Framework.Asynchronous.IAsyncResult>("DoShow");
        static CrossBindingMethodInfo mOnShow_7 = new CrossBindingMethodInfo("OnShow");
        static CrossBindingFunctionInfo<System.Boolean, Loxodon.Framework.Asynchronous.IAsyncResult> mDoHide_8 = new CrossBindingFunctionInfo<System.Boolean, Loxodon.Framework.Asynchronous.IAsyncResult>("DoHide");
        static CrossBindingMethodInfo mOnHide_9 = new CrossBindingMethodInfo("OnHide");
        static CrossBindingMethodInfo mDoDismiss_10 = new CrossBindingMethodInfo("DoDismiss");
        static CrossBindingMethodInfo mOnDismiss_11 = new CrossBindingMethodInfo("OnDismiss");
        static CrossBindingMethodInfo mOnDestroy_12 = new CrossBindingMethodInfo("OnDestroy");
        static CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation> mget_ActivationAnimation_13 = new CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation>("get_ActivationAnimation");
        static CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation> mset_ActivationAnimation_14 = new CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation>("set_ActivationAnimation");
        static CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation> mget_PassivationAnimation_15 = new CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation>("get_PassivationAnimation");
        static CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation> mset_PassivationAnimation_16 = new CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation>("set_PassivationAnimation");
        static CrossBindingFunctionInfo<System.Collections.Generic.List<Loxodon.Framework.Views.IUIView>> mget_Views_17 = new CrossBindingFunctionInfo<System.Collections.Generic.List<Loxodon.Framework.Views.IUIView>>("get_Views");
        static CrossBindingFunctionInfo<System.String, Loxodon.Framework.Views.IUIView> mGetView_18 = new CrossBindingFunctionInfo<System.String, Loxodon.Framework.Views.IUIView>("GetView");
        static CrossBindingMethodInfo<Loxodon.Framework.Views.IUIView, System.Boolean> mAddView_19 = new CrossBindingMethodInfo<Loxodon.Framework.Views.IUIView, System.Boolean>("AddView");
        static CrossBindingMethodInfo<Loxodon.Framework.Views.IUIView, Loxodon.Framework.Views.UILayout> mAddView_20 = new CrossBindingMethodInfo<Loxodon.Framework.Views.IUIView, Loxodon.Framework.Views.UILayout>("AddView");
        static CrossBindingMethodInfo<Loxodon.Framework.Views.IUIView, System.Boolean> mRemoveView_21 = new CrossBindingMethodInfo<Loxodon.Framework.Views.IUIView, System.Boolean>("RemoveView");
        static CrossBindingFunctionInfo<System.String> mget_Name_22 = new CrossBindingFunctionInfo<System.String>("get_Name");
        static CrossBindingMethodInfo<System.String> mset_Name_23 = new CrossBindingMethodInfo<System.String>("set_Name");
        static CrossBindingFunctionInfo<UnityEngine.Transform> mget_Parent_24 = new CrossBindingFunctionInfo<UnityEngine.Transform>("get_Parent");
        static CrossBindingFunctionInfo<UnityEngine.GameObject> mget_Owner_25 = new CrossBindingFunctionInfo<UnityEngine.GameObject>("get_Owner");
        static CrossBindingFunctionInfo<UnityEngine.Transform> mget_Transform_26 = new CrossBindingFunctionInfo<UnityEngine.Transform>("get_Transform");
        static CrossBindingFunctionInfo<UnityEngine.RectTransform> mget_RectTransform_27 = new CrossBindingFunctionInfo<UnityEngine.RectTransform>("get_RectTransform");
        static CrossBindingFunctionInfo<System.Boolean> mget_Visibility_28 = new CrossBindingFunctionInfo<System.Boolean>("get_Visibility");
        static CrossBindingMethodInfo<System.Boolean> mset_Visibility_29 = new CrossBindingMethodInfo<System.Boolean>("set_Visibility");
        static CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation> mget_EnterAnimation_30 = new CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation>("get_EnterAnimation");
        static CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation> mset_EnterAnimation_31 = new CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation>("set_EnterAnimation");
        static CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation> mget_ExitAnimation_32 = new CrossBindingFunctionInfo<Loxodon.Framework.Views.Animations.IAnimation>("get_ExitAnimation");
        static CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation> mset_ExitAnimation_33 = new CrossBindingMethodInfo<Loxodon.Framework.Views.Animations.IAnimation>("set_ExitAnimation");
        static CrossBindingFunctionInfo<System.Single> mget_Alpha_34 = new CrossBindingFunctionInfo<System.Single>("get_Alpha");
        static CrossBindingMethodInfo<System.Single> mset_Alpha_35 = new CrossBindingMethodInfo<System.Single>("set_Alpha");
        static CrossBindingFunctionInfo<System.Boolean> mget_Interactable_36 = new CrossBindingFunctionInfo<System.Boolean>("get_Interactable");
        static CrossBindingMethodInfo<System.Boolean> mset_Interactable_37 = new CrossBindingMethodInfo<System.Boolean>("set_Interactable");
        static CrossBindingFunctionInfo<UnityEngine.CanvasGroup> mget_CanvasGroup_38 = new CrossBindingFunctionInfo<UnityEngine.CanvasGroup>("get_CanvasGroup");
        static CrossBindingFunctionInfo<Loxodon.Framework.Views.IAttributes> mget_ExtraAttributes_39 = new CrossBindingFunctionInfo<Loxodon.Framework.Views.IAttributes>("get_ExtraAttributes");
        static CrossBindingMethodInfo mOnVisibilityChanged_40 = new CrossBindingMethodInfo("OnVisibilityChanged");
        static CrossBindingMethodInfo mAwake_41 = new CrossBindingMethodInfo("Awake");
        static CrossBindingMethodInfo mStart_42 = new CrossBindingMethodInfo("Start");
        static CrossBindingFunctionInfo<System.Boolean> mIsActive_43 = new CrossBindingFunctionInfo<System.Boolean>("IsActive");
        static CrossBindingMethodInfo mOnValidate_44 = new CrossBindingMethodInfo("OnValidate");
        static CrossBindingMethodInfo mReset_45 = new CrossBindingMethodInfo("Reset");
        static CrossBindingMethodInfo mOnRectTransformDimensionsChange_46 = new CrossBindingMethodInfo("OnRectTransformDimensionsChange");
        static CrossBindingMethodInfo mOnBeforeTransformParentChanged_47 = new CrossBindingMethodInfo("OnBeforeTransformParentChanged");
        static CrossBindingMethodInfo mOnTransformParentChanged_48 = new CrossBindingMethodInfo("OnTransformParentChanged");
        static CrossBindingMethodInfo mOnDidApplyAnimationProperties_49 = new CrossBindingMethodInfo("OnDidApplyAnimationProperties");
        static CrossBindingMethodInfo mOnCanvasGroupChanged_50 = new CrossBindingMethodInfo("OnCanvasGroupChanged");
        static CrossBindingMethodInfo mOnCanvasHierarchyChanged_51 = new CrossBindingMethodInfo("OnCanvasHierarchyChanged");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Loxodon.Framework.Views.Window);
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

        public class Adapter : Loxodon.Framework.Views.Window, CrossBindingAdaptorType, IBehaviourAdapter
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

            public ILTypeInstance ILInstance { get { return instance; } set { instance = value; } }

            public AppDomain AppDomain { get { return appdomain; } set { appdomain = value; } }

            protected override void OnEnable()
            {
                if (this.instance == null)
                    return;

                if (mOnEnable_0.CheckShouldInvokeBase(this.instance))
                    base.OnEnable();
                else
                    mOnEnable_0.Invoke(this.instance);
            }

            protected override void OnDisable()
            {
                if (mOnDisable_1.CheckShouldInvokeBase(this.instance))
                    base.OnDisable();
                else
                    mOnDisable_1.Invoke(this.instance);
            }

            public override Loxodon.Framework.Asynchronous.IAsyncResult Activate(System.Boolean ignoreAnimation)
            {
                if (mActivate_2.CheckShouldInvokeBase(this.instance))
                    return base.Activate(ignoreAnimation);
                else
                    return mActivate_2.Invoke(this.instance, ignoreAnimation);
            }

            public override Loxodon.Framework.Asynchronous.IAsyncResult Passivate(System.Boolean ignoreAnimation)
            {
                if (mPassivate_3.CheckShouldInvokeBase(this.instance))
                    return base.Passivate(ignoreAnimation);
                else
                    return mPassivate_3.Invoke(this.instance, ignoreAnimation);
            }

            protected override void OnActivatedChanged()
            {
                if (mOnActivatedChanged_4.CheckShouldInvokeBase(this.instance))
                    base.OnActivatedChanged();
                else
                    mOnActivatedChanged_4.Invoke(this.instance);
            }

            protected override void OnCreate(Loxodon.Framework.Views.IBundle bundle)
            {
                mOnCreate_5.Invoke(this.instance, bundle);
            }

            public override Loxodon.Framework.Asynchronous.IAsyncResult DoShow(System.Boolean ignoreAnimation)
            {
                if (mDoShow_6.CheckShouldInvokeBase(this.instance))
                    return base.DoShow(ignoreAnimation);
                else
                    return mDoShow_6.Invoke(this.instance, ignoreAnimation);
            }

            protected override void OnShow()
            {
                if (mOnShow_7.CheckShouldInvokeBase(this.instance))
                    base.OnShow();
                else
                    mOnShow_7.Invoke(this.instance);
            }

            public override Loxodon.Framework.Asynchronous.IAsyncResult DoHide(System.Boolean ignoreAnimation)
            {
                if (mDoHide_8.CheckShouldInvokeBase(this.instance))
                    return base.DoHide(ignoreAnimation);
                else
                    return mDoHide_8.Invoke(this.instance, ignoreAnimation);
            }

            protected override void OnHide()
            {
                if (mOnHide_9.CheckShouldInvokeBase(this.instance))
                    base.OnHide();
                else
                    mOnHide_9.Invoke(this.instance);
            }

            public override void DoDismiss()
            {
                if (mDoDismiss_10.CheckShouldInvokeBase(this.instance))
                    base.DoDismiss();
                else
                    mDoDismiss_10.Invoke(this.instance);
            }

            protected override void OnDismiss()
            {
                if (mOnDismiss_11.CheckShouldInvokeBase(this.instance))
                    base.OnDismiss();
                else
                    mOnDismiss_11.Invoke(this.instance);
            }

            protected override void OnDestroy()
            {
                if (mOnDestroy_12.CheckShouldInvokeBase(this.instance))
                    base.OnDestroy();
                else
                    mOnDestroy_12.Invoke(this.instance);
            }

            public override Loxodon.Framework.Views.IUIView GetView(System.String name)
            {
                if (mGetView_18.CheckShouldInvokeBase(this.instance))
                    return base.GetView(name);
                else
                    return mGetView_18.Invoke(this.instance, name);
            }

            public override void AddView(Loxodon.Framework.Views.IUIView view, System.Boolean worldPositionStays)
            {
                if (mAddView_19.CheckShouldInvokeBase(this.instance))
                    base.AddView(view, worldPositionStays);
                else
                    mAddView_19.Invoke(this.instance, view, worldPositionStays);
            }

            public override void AddView(Loxodon.Framework.Views.IUIView view, Loxodon.Framework.Views.UILayout layout)
            {
                if (mAddView_20.CheckShouldInvokeBase(this.instance))
                    base.AddView(view, layout);
                else
                    mAddView_20.Invoke(this.instance, view, layout);
            }

            public override void RemoveView(Loxodon.Framework.Views.IUIView view, System.Boolean worldPositionStays)
            {
                if (mRemoveView_21.CheckShouldInvokeBase(this.instance))
                    base.RemoveView(view, worldPositionStays);
                else
                    mRemoveView_21.Invoke(this.instance, view, worldPositionStays);
            }

            protected override void OnVisibilityChanged()
            {
                if (mOnVisibilityChanged_40.CheckShouldInvokeBase(this.instance))
                    base.OnVisibilityChanged();
                else
                    mOnVisibilityChanged_40.Invoke(this.instance);
            }

            protected override void Awake()
            {
                if (this.instance == null)
                    return;

                if (mAwake_41.CheckShouldInvokeBase(this.instance))
                    base.Awake();
                else
                    mAwake_41.Invoke(this.instance);
            }

            protected override void Start()
            {
                if (mStart_42.CheckShouldInvokeBase(this.instance))
                    base.Start();
                else
                    mStart_42.Invoke(this.instance);
            }

            public override System.Boolean IsActive()
            {
                if (mIsActive_43.CheckShouldInvokeBase(this.instance))
                    return base.IsActive();
                else
                    return mIsActive_43.Invoke(this.instance);
            }

            //protected override void OnValidate()
            //{
            //    if (mOnValidate_44.CheckShouldInvokeBase(this.instance))
            //        base.OnValidate();
            //    else
            //        mOnValidate_44.Invoke(this.instance);
            //}

            //protected override void Reset()
            //{
            //    if (mReset_45.CheckShouldInvokeBase(this.instance))
            //        base.Reset();
            //    else
            //        mReset_45.Invoke(this.instance);
            //}

            //protected override void OnRectTransformDimensionsChange()
            //{
            //    if (mOnRectTransformDimensionsChange_46.CheckShouldInvokeBase(this.instance))
            //        base.OnRectTransformDimensionsChange();
            //    else
            //        mOnRectTransformDimensionsChange_46.Invoke(this.instance);
            //}

            //protected override void OnBeforeTransformParentChanged()
            //{
            //    if (mOnBeforeTransformParentChanged_47.CheckShouldInvokeBase(this.instance))
            //        base.OnBeforeTransformParentChanged();
            //    else
            //        mOnBeforeTransformParentChanged_47.Invoke(this.instance);
            //}

            //protected override void OnTransformParentChanged()
            //{
            //    if (mOnTransformParentChanged_48.CheckShouldInvokeBase(this.instance))
            //        base.OnTransformParentChanged();
            //    else
            //        mOnTransformParentChanged_48.Invoke(this.instance);
            //}

            //protected override void OnDidApplyAnimationProperties()
            //{
            //    if (mOnDidApplyAnimationProperties_49.CheckShouldInvokeBase(this.instance))
            //        base.OnDidApplyAnimationProperties();
            //    else
            //        mOnDidApplyAnimationProperties_49.Invoke(this.instance);
            //}

            //protected override void OnCanvasGroupChanged()
            //{
            //    if (mOnCanvasGroupChanged_50.CheckShouldInvokeBase(this.instance))
            //        base.OnCanvasGroupChanged();
            //    else
            //        mOnCanvasGroupChanged_50.Invoke(this.instance);
            //}

            //protected override void OnCanvasHierarchyChanged()
            //{
            //    if (mOnCanvasHierarchyChanged_51.CheckShouldInvokeBase(this.instance))
            //        base.OnCanvasHierarchyChanged();
            //    else
            //        mOnCanvasHierarchyChanged_51.Invoke(this.instance);
            //}

            public override Loxodon.Framework.Views.Animations.IAnimation ActivationAnimation
            {
            get
            {
                if (mget_ActivationAnimation_13.CheckShouldInvokeBase(this.instance))
                    return base.ActivationAnimation;
                else
                    return mget_ActivationAnimation_13.Invoke(this.instance);

            }
            set
            {
                if (mset_ActivationAnimation_14.CheckShouldInvokeBase(this.instance))
                    base.ActivationAnimation = value;
                else
                    mset_ActivationAnimation_14.Invoke(this.instance, value);

            }
            }

            public override Loxodon.Framework.Views.Animations.IAnimation PassivationAnimation
            {
            get
            {
                if (mget_PassivationAnimation_15.CheckShouldInvokeBase(this.instance))
                    return base.PassivationAnimation;
                else
                    return mget_PassivationAnimation_15.Invoke(this.instance);

            }
            set
            {
                if (mset_PassivationAnimation_16.CheckShouldInvokeBase(this.instance))
                    base.PassivationAnimation = value;
                else
                    mset_PassivationAnimation_16.Invoke(this.instance, value);

            }
            }

            public override System.Collections.Generic.List<Loxodon.Framework.Views.IUIView> Views
            {
            get
            {
                if (mget_Views_17.CheckShouldInvokeBase(this.instance))
                    return base.Views;
                else
                    return mget_Views_17.Invoke(this.instance);

            }
            }

            public override System.String Name
            {
            get
            {
                if (mget_Name_22.CheckShouldInvokeBase(this.instance))
                    return base.Name;
                else
                    return mget_Name_22.Invoke(this.instance);

            }
            set
            {
                if (mset_Name_23.CheckShouldInvokeBase(this.instance))
                    base.Name = value;
                else
                    mset_Name_23.Invoke(this.instance, value);

            }
            }

            public override UnityEngine.Transform Parent
            {
            get
            {
                if (mget_Parent_24.CheckShouldInvokeBase(this.instance))
                    return base.Parent;
                else
                    return mget_Parent_24.Invoke(this.instance);

            }
            }

            public override UnityEngine.GameObject Owner
            {
            get
            {
                if (mget_Owner_25.CheckShouldInvokeBase(this.instance))
                    return base.Owner;
                else
                    return mget_Owner_25.Invoke(this.instance);

            }
            }

            public override UnityEngine.Transform Transform
            {
            get
            {
                if (mget_Transform_26.CheckShouldInvokeBase(this.instance))
                    return base.Transform;
                else
                    return mget_Transform_26.Invoke(this.instance);

            }
            }

            public override UnityEngine.RectTransform RectTransform
            {
            get
            {
                if (mget_RectTransform_27.CheckShouldInvokeBase(this.instance))
                    return base.RectTransform;
                else
                    return mget_RectTransform_27.Invoke(this.instance);

            }
            }

            public override System.Boolean Visibility
            {
            get
            {
                if (mget_Visibility_28.CheckShouldInvokeBase(this.instance))
                    return base.Visibility;
                else
                    return mget_Visibility_28.Invoke(this.instance);

            }
            set
            {
                if (mset_Visibility_29.CheckShouldInvokeBase(this.instance))
                    base.Visibility = value;
                else
                    mset_Visibility_29.Invoke(this.instance, value);

            }
            }

            public override Loxodon.Framework.Views.Animations.IAnimation EnterAnimation
            {
            get
            {
                if (mget_EnterAnimation_30.CheckShouldInvokeBase(this.instance))
                    return base.EnterAnimation;
                else
                    return mget_EnterAnimation_30.Invoke(this.instance);

            }
            set
            {
                if (mset_EnterAnimation_31.CheckShouldInvokeBase(this.instance))
                    base.EnterAnimation = value;
                else
                    mset_EnterAnimation_31.Invoke(this.instance, value);

            }
            }

            public override Loxodon.Framework.Views.Animations.IAnimation ExitAnimation
            {
            get
            {
                if (mget_ExitAnimation_32.CheckShouldInvokeBase(this.instance))
                    return base.ExitAnimation;
                else
                    return mget_ExitAnimation_32.Invoke(this.instance);

            }
            set
            {
                if (mset_ExitAnimation_33.CheckShouldInvokeBase(this.instance))
                    base.ExitAnimation = value;
                else
                    mset_ExitAnimation_33.Invoke(this.instance, value);

            }
            }

            public override System.Single Alpha
            {
            get
            {
                if (mget_Alpha_34.CheckShouldInvokeBase(this.instance))
                    return base.Alpha;
                else
                    return mget_Alpha_34.Invoke(this.instance);

            }
            set
            {
                if (mset_Alpha_35.CheckShouldInvokeBase(this.instance))
                    base.Alpha = value;
                else
                    mset_Alpha_35.Invoke(this.instance, value);

            }
            }

            public override System.Boolean Interactable
            {
            get
            {
                if (mget_Interactable_36.CheckShouldInvokeBase(this.instance))
                    return base.Interactable;
                else
                    return mget_Interactable_36.Invoke(this.instance);

            }
            set
            {
                if (mset_Interactable_37.CheckShouldInvokeBase(this.instance))
                    base.Interactable = value;
                else
                    mset_Interactable_37.Invoke(this.instance, value);

            }
            }

            public override UnityEngine.CanvasGroup CanvasGroup
            {
            get
            {
                if (mget_CanvasGroup_38.CheckShouldInvokeBase(this.instance))
                    return base.CanvasGroup;
                else
                    return mget_CanvasGroup_38.Invoke(this.instance);

            }
            }

            public override Loxodon.Framework.Views.IAttributes ExtraAttributes
            {
            get
            {
                if (mget_ExtraAttributes_39.CheckShouldInvokeBase(this.instance))
                    return base.ExtraAttributes;
                else
                    return mget_ExtraAttributes_39.Invoke(this.instance);

            }
            }
            void IBehaviourAdapter.Awake()
            {
                this.Awake();
                this.OnEnable();
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

