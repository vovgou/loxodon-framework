using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Mono.Cecil.Pdb;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.ILRuntimes.Adapters;
using Loxodon.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Loxodon.Framework.ILRuntimes
{
    public static class ILRuntimeEnvironment
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ILRuntimeEnvironment));

        private static object syncLock = new object();
        private static AppDomain appdomain;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeCreate()
        {
            Initialize();
        }

        static ILRuntimeEnvironment()
        {
            Initialize();
        }

        private static void Initialize()
        {
            lock (syncLock)
            {
                if (appdomain != null)
                    return;

                appdomain = new AppDomain();

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
                appdomain.UnityMainThreadID = Thread.CurrentThread.ManagedThreadId;
#endif

                RegisterGameObjectCLRRedirection();

                //Register delegations
                appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.EventArgs>();
                appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler>((act) =>
                {
                    return new System.EventHandler((sender, e) =>
                    {
                        ((Action<System.Object, System.EventArgs>)act)(sender, e);
                    });
                });

                appdomain.DelegateManager.RegisterMethodDelegate<System.Object, PropertyChangedEventArgs>();
                appdomain.DelegateManager.RegisterDelegateConvertor<PropertyChangedEventHandler>((act) =>
                {
                    return new PropertyChangedEventHandler((sender, e) =>
                    {
                        ((Action<System.Object, PropertyChangedEventArgs>)act)(sender, e);
                    });
                });

                appdomain.DelegateManager.RegisterMethodDelegate<Loxodon.Framework.ILRuntimes.Adapters.INotifyPropertyChangedAdapter.Adapter>();


                //Register adapters
                appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdapter());
                appdomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
                appdomain.RegisterCrossBindingAdaptor(new UIViewAdapter());
                appdomain.RegisterCrossBindingAdaptor(new WindowAdapter());
                appdomain.RegisterCrossBindingAdaptor(new ViewAdapter());
                appdomain.RegisterCrossBindingAdaptor(new INotifyPropertyChangedAdapter());
                appdomain.RegisterCrossBindingAdaptor(new IViewModelAdapter());
            }
        }

        public static AppDomain AppDomain { get { return appdomain; } }

        public static Task LoadAssembly(Uri dllPath)
        {
            return LoadAssembly(dllPath, null);
        }

        public static async Task LoadAssembly(Uri dllPath, Uri pdbPath)
        {
            byte[] dllData = null;
            byte[] pdbData = null;
            using (UnityWebRequest www = new UnityWebRequest(dllPath))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                await www.SendWebRequest();
                if (!string.IsNullOrEmpty(www.error))
                    throw new Exception(string.Format("Failed to load the DLL at the address '{0}'.Error:{1}", dllPath, www.error));
                dllData = www.downloadHandler.data;
            }

            if (pdbPath != null)
            {
                using (UnityWebRequest www = new UnityWebRequest(pdbPath))
                {
                    www.downloadHandler = new DownloadHandlerBuffer();
                    await www.SendWebRequest();
                    if (!string.IsNullOrEmpty(www.error))
                        throw new Exception(string.Format("Failed to load the PDB at the address '{0}'.Error:{1}", dllPath, www.error));
                    pdbData = www.downloadHandler.data;
                }
            }

            if (pdbData != null)
            {
                MemoryStream dllStream = new MemoryStream(dllData);
                MemoryStream pdbStream = new MemoryStream(pdbData);
                AppDomain.LoadAssembly(dllStream, pdbStream, new PdbReaderProvider());

            }
            else
            {
                MemoryStream dllStream = new MemoryStream(dllData);
                AppDomain.LoadAssembly(dllStream);
            }
        }

        private static unsafe void RegisterGameObjectCLRRedirection()
        {
            var methods = typeof(GameObject).GetMethods();
            foreach (var method in methods)
            {
                if (method.Name == "AddComponent" && method.GetGenericArguments().Length == 1)
                {
                    AppDomain.RegisterCLRMethodRedirection(method, AddComponent);
                }
            }

            foreach (var method in methods)
            {
                if (method.Name == "GetComponent" && method.GetGenericArguments().Length == 1)
                {
                    AppDomain.RegisterCLRMethodRedirection(method, GetComponent);
                }
            }
        }

        private unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res;
                if (type is CLRType)
                {
                    res = instance.AddComponent(type.TypeForCLR);
                }
                else
                {
                    var ilInstance = new ILTypeInstance(type as ILType, false);
                    var clrInstance = instance.AddComponent(type.TypeForCLR);
                    IBehaviourAdapter adapter = clrInstance as IBehaviourAdapter;
                    if (adapter == null)
                        throw new NotSupportedException();

                    adapter.ILInstance = ilInstance;
                    adapter.AppDomain = __domain;
                    ilInstance.CLRInstance = clrInstance;
                    res = adapter.ILInstance;
                    (clrInstance as IBehaviourAdapter).Awake();
                }
                return ILIntepreter.PushObject(ptr, __mStack, res);
            }
            return __esp;
        }

        private unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res = null;
                if (type is CLRType)
                {
                    res = instance.GetComponent(type.TypeForCLR);
                }
                else
                {
                    var clrInstances = instance.GetComponents(type.TypeForCLR);
                    for (int i = 0; i < clrInstances.Length; i++)
                    {
                        var clrInstance = (CrossBindingAdaptorType)clrInstances[i];
                        if (clrInstance.ILInstance != null)
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                res = clrInstance.ILInstance;
                                break;
                            }
                        }
                    }
                }
                return ILIntepreter.PushObject(ptr, __mStack, res);
            }
            return __esp;
        }
    }
}