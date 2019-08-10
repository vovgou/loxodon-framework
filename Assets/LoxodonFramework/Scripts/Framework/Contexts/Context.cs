/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;

using Loxodon.Framework.Services;
using System;

namespace Loxodon.Framework.Contexts
{
    public class Context : IDisposable
    {
        private static ApplicationContext context = new ApplicationContext();
        private static Dictionary<string, Context> contexts = new Dictionary<string, Context>();
        public static ApplicationContext GetApplicationContext()
        {
            return Context.context;
        }

        public static void SetApplicationContext(ApplicationContext context)
        {
            Context.context = context;
        }

        public static Context GetContext(string key)
        {
            Context context = null;
            contexts.TryGetValue(key, out context);
            return context;
        }

        public static T GetContext<T>(string key) where T : Context
        {
            return (T)GetContext(key);
        }

        public static void AddContext(string key, Context context)
        {
            contexts.Add(key, context);
        }

        public static void RemoveContext(string key)
        {
            contexts.Remove(key);
        }

        private bool innerContainer = false;
        private Context contextBase;
        private IServiceContainer container;
        private Dictionary<string, object> attributes;

        public Context() : this(null, null)
        {
        }

        public Context(IServiceContainer container, Context contextBase)
        {
            this.attributes = new Dictionary<string, object>();
            this.contextBase = contextBase;
            this.container = container;
            if (this.container == null)
            {
                this.innerContainer = true;
                this.container = new ServiceContainer();
            }
        }

        public virtual bool Contains(string name, bool cascade = true)
        {
            if (this.attributes.ContainsKey(name))
                return true;

            if (cascade && this.contextBase != null)
                return this.contextBase.Contains(name, cascade);

            return false;
        }

        public virtual object Get(string name, bool cascade = true)
        {
            return this.Get<object>(name, cascade);
        }

        public virtual T Get<T>(string name, bool cascade = true)
        {
            object v;
            if (this.attributes.TryGetValue(name, out v))
                return (T)v;

            if (cascade && this.contextBase != null)
                return this.contextBase.Get<T>(name, cascade);

            return default(T);
        }

        public virtual void Set(string name, object value)
        {
            this.Set<object>(name, value);
        }

        public virtual void Set<T>(string name, T value)
        {
            this.attributes[name] = value;
        }

        public virtual object Remove(string name)
        {
            return this.Remove<object>(name);
        }

        public virtual T Remove<T>(string name)
        {
            if (!this.attributes.ContainsKey(name))
                return default(T);

            object v = this.attributes[name];
            this.attributes.Remove(name);
            return (T)v;
        }

        public virtual IEnumerator GetEnumerator()
        {
            return this.attributes.GetEnumerator();
        }

        public virtual IServiceContainer GetContainer()
        {
            return this.container;
        }

        public virtual object GetService(Type type)
        {
            object result = this.container.Resolve(type);
            if (result != null)
                return result;

            if (this.contextBase != null)
                return this.contextBase.GetService(type);

            return null;
        }

        public virtual object GetService(string name)
        {
            object result = this.container.Resolve(name);
            if (result != null)
                return result;

            if (this.contextBase != null)
                return this.contextBase.GetService(name);

            return null;
        }

        public virtual T GetService<T>()
        {
            T result = this.container.Resolve<T>();
            if (result != null)
                return result;

            if (this.contextBase != null)
                return this.contextBase.GetService<T>();

            return default(T);
        }

        public virtual T GetService<T>(string name)
        {
            T result = this.container.Resolve<T>(name);
            if (result != null)
                return result;

            if (this.contextBase != null)
                return this.contextBase.GetService<T>(name);

            return default(T);
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (this.innerContainer && this.container != null)
                    {
                        IDisposable dis = this.container as IDisposable;
                        if (dis != null)
                            dis.Dispose();
                    }
                }
                disposed = true;
            }
        }

        ~Context()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
