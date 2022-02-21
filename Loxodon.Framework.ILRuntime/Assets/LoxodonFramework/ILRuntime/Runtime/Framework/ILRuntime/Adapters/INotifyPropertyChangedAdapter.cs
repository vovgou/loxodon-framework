using System;
using System.ComponentModel;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Loxodon.Framework.ILRuntimes.Adapters
{   
    public class INotifyPropertyChangedAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo<System.ComponentModel.PropertyChangedEventHandler> madd_PropertyChanged_0 = new CrossBindingMethodInfo<System.ComponentModel.PropertyChangedEventHandler>("add_PropertyChanged");
        static CrossBindingMethodInfo<System.ComponentModel.PropertyChangedEventHandler> mremove_PropertyChanged_1 = new CrossBindingMethodInfo<System.ComponentModel.PropertyChangedEventHandler>("remove_PropertyChanged");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.ComponentModel.INotifyPropertyChanged);
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

        public class Adapter : System.ComponentModel.INotifyPropertyChanged, CrossBindingAdaptorType
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

            public event PropertyChangedEventHandler PropertyChanged
            {
                add { madd_PropertyChanged_0.Invoke(this.instance, value); }
                remove { mremove_PropertyChanged_1.Invoke(this.instance, value); }
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

