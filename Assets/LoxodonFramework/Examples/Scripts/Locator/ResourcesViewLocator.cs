using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Views;

namespace Loxodon.Framework.Examples
{
    public class ResourcesViewLocator : UIViewLocatorBase
    {
        private GlobalWindowManager globalWindowManager;
        private Dictionary<string, WeakReference> templates = new Dictionary<string, WeakReference>();

        protected string Normalize(string name)
        {
            int index = name.IndexOf('.');
            if (index < 0)
                return name;

            return name.Substring(0, index);
        }

        protected virtual IWindowManager GetDefaultWindowManager()
        {
            if (globalWindowManager != null)
                return globalWindowManager;

            globalWindowManager = GameObject.FindObjectOfType<GlobalWindowManager>();
            if (globalWindowManager == null)
                throw new NotFoundException("GlobalWindowManager");

            return globalWindowManager;
        }

        public override T LoadView<T>(string name)
        {
            name = Normalize(name);
            WeakReference weakRef;
            GameObject viewTemplateGo = null;
            try
            {
                if (this.templates.TryGetValue(name, out weakRef) && weakRef.IsAlive)
                {
                    viewTemplateGo = (GameObject)weakRef.Target;

                    //Check if the object is valid because it may have been destroyed.
                    //Unmanaged objects,the weak caches do not accurately track the validity of objects.
                    if (viewTemplateGo != null)
                    {
                        string goName = viewTemplateGo.name;
                    }
                }
            }
            catch (Exception)
            {
                viewTemplateGo = null;
            }

            if (viewTemplateGo == null)
            {
                viewTemplateGo = Resources.Load<GameObject>(name);
                if (viewTemplateGo != null)
                {
                    viewTemplateGo.SetActive(false);
                    this.templates[name] = new WeakReference(viewTemplateGo);
                }
            }

            if (viewTemplateGo == null || viewTemplateGo.GetComponent<T>() == null)
                return default(T);

            GameObject go = GameObject.Instantiate(viewTemplateGo);
            go.name = viewTemplateGo.name;
            T view = go.GetComponent<T>();
            if (view == null && go != null)
                GameObject.Destroy(go);
            return view;
        }

        public override IProgressTask<float, T> LoadViewAsync<T>(string name)
        {
            ProgressTask<float, T> task = new ProgressTask<float, T>(p => DoLoad<T>(p, name));
            return task.Start(30);
        }

        protected virtual IEnumerator DoLoad<T>(IProgressPromise<float, T> promise, string name)
        {
            name = Normalize(name);
            WeakReference weakRef;
            GameObject viewTemplateGo = null;
            try
            {
                if (this.templates.TryGetValue(name, out weakRef) && weakRef.IsAlive)
                {
                    viewTemplateGo = (GameObject)weakRef.Target;

                    //Check if the object is valid because it may have been destroyed.
                    //Unmanaged objects,the weak caches do not accurately track the validity of objects.
                    if (viewTemplateGo != null)
                    {
                        string goName = viewTemplateGo.name;
                    }
                }
            }
            catch (Exception)
            {
                viewTemplateGo = null;
            }

            if (viewTemplateGo == null)
            {
                ResourceRequest request = Resources.LoadAsync<GameObject>(name);
                while (!request.isDone)
                {
                    promise.UpdateProgress(request.progress);
                    yield return null;
                }

                viewTemplateGo = (GameObject)request.asset;
                if (viewTemplateGo != null)
                {
                    viewTemplateGo.SetActive(false);
                    this.templates[name] = new WeakReference(viewTemplateGo);
                }
            }

            if (viewTemplateGo == null || viewTemplateGo.GetComponent<T>() == null)
            {
                promise.UpdateProgress(1f);
                promise.SetException(new NotFoundException(name));
                yield break;
            }

            GameObject go = GameObject.Instantiate(viewTemplateGo);
            go.name = viewTemplateGo.name;
            T view = go.GetComponent<T>();
            if (view == null)
            {
                GameObject.Destroy(go);
                promise.SetException(new NotFoundException(name));
            }
            else
            {
                promise.UpdateProgress(1f);
                promise.SetResult(view);
            }
        }

        public override T LoadWindow<T>(string name)
        {
            return LoadWindow<T>(null, name);
        }

        public override T LoadWindow<T>(IWindowManager windowManager, string name)
        {
            if (windowManager == null)
                windowManager = this.GetDefaultWindowManager();

            T target = this.LoadView<T>(name);
            if (target != null)
                target.WindowManager = windowManager;

            return target;
        }

        public override IProgressTask<float, T> LoadWindowAsync<T>(string name)
        {
            return this.LoadWindowAsync<T>(null, name);
        }

        public override IProgressTask<float, T> LoadWindowAsync<T>(IWindowManager windowManager, string name)
        {
            if (windowManager == null)
                windowManager = this.GetDefaultWindowManager();

            ProgressTask<float, T> task = new ProgressTask<float, T>(p => DoLoad<T>(p, name));
            return task.Start(30).OnPostExecute(win =>
            {
                win.WindowManager = windowManager;
            });
        }
    }
}
