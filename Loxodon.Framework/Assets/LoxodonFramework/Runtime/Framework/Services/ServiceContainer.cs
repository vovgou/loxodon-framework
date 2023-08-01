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

using System;
using System.Collections.Concurrent;

namespace Loxodon.Framework.Services
{
    public class ServiceContainer : IServiceContainer, IDisposable
    {
        private readonly object _lock = new object();
        private ConcurrentDictionary<string, Entry> nameServiceMappings = new ConcurrentDictionary<string, Entry>();
        private ConcurrentDictionary<Type, Entry> typeServiceMappings = new ConcurrentDictionary<Type, Entry>();

        public virtual object Resolve(Type type)
        {
            Entry entry;
            if (typeServiceMappings.TryGetValue(type, out entry))
                return entry.Factory.Create();
            return null;
        }

        public virtual T Resolve<T>()
        {
            Entry entry;
            if (typeServiceMappings.TryGetValue(typeof(T), out entry))
                return (T)entry.Factory.Create();
            return default(T);
        }

        public virtual object Resolve(string name)
        {
            Entry entry;
            if (nameServiceMappings.TryGetValue(name, out entry))
                return entry.Factory.Create();
            return null;
        }

        public virtual T Resolve<T>(string name)
        {
            Entry entry;
            if (nameServiceMappings.TryGetValue(name, out entry))
                return (T)entry.Factory.Create();
            return default(T);
        }

        public virtual void Register<T>(Func<T> factory)
        {
            this.Register0(typeof(T), new GenericFactory<T>(factory));
        }

        public virtual void Register(Type type, object target)
        {
            this.Register0(type, new SingleInstanceFactory(target));
        }

        public virtual void Register(string name, object target)
        {
            this.Register0(name, new SingleInstanceFactory(target));
        }

        public virtual void Register<T>(T target)
        {
            this.Register0(typeof(T), new SingleInstanceFactory(target));
        }

        public virtual void Register<T>(string name, Func<T> factory)
        {
            this.Register0(name, new GenericFactory<T>(factory));
        }

        public virtual void Register<T>(string name, T target)
        {
            this.Register0(name, new SingleInstanceFactory(target));
        }

        public virtual void Unregister(Type type)
        {
            this.Unregister0(type);
        }

        public virtual void Unregister<T>()
        {
            this.Unregister0(typeof(T));
        }

        public virtual void Unregister(string name)
        {
            this.Unregister0(name);
        }

        //internal void Register0(string name, Type type, IFactory factory)
        //{
        //    lock (_lock)
        //    {
        //        var entry = new Entry(name, type, factory);
        //        if (!nameServiceMappings.TryAdd(name, entry))
        //            throw new DuplicateRegisterServiceException(string.Format("Duplicate key {0}", name));

        //        typeServiceMappings.TryAdd(type, entry);
        //    }
        //}

        /// <summary>
        /// For services registered with a type, if the type is not a generic type, 
        /// it can be retrieved by type or type name.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="factory"></param>
        internal void Register0(Type type, IFactory factory)
        {
            lock (_lock)
            {
                string name = type.IsGenericType ? null : type.Name;
                Entry entry = new Entry(name, type, factory);
                if (!typeServiceMappings.TryAdd(type, entry))
                    throw new DuplicateRegisterServiceException(string.Format("Duplicate key {0}", type));

                if (!string.IsNullOrEmpty(name))
                    nameServiceMappings.TryAdd(name, entry);
            }
        }

        /// <summary>
        /// Services registered with a name can only be retrieved with a name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="factory"></param>
        internal void Register0(string name, IFactory factory)
        {
            lock (_lock)
            {
                if (!nameServiceMappings.TryAdd(name, new Entry(name, null, factory)))
                    throw new DuplicateRegisterServiceException(string.Format("Duplicate key {0}", name));
            }
        }

        internal void Unregister0(string name)
        {
            lock (_lock)
            {
                Entry entry;
                if (!nameServiceMappings.TryRemove(name, out entry) || entry == null || entry.Type == null)
                    return;

                Entry entry2;
                if (!typeServiceMappings.TryGetValue(entry.Type, out entry2) || entry != entry2)
                    return;

                typeServiceMappings.TryRemove(entry.Type, out _);
            }
        }

        internal void Unregister0(Type type)
        {
            lock (_lock)
            {
                Entry entry;
                if (!typeServiceMappings.TryRemove(type, out entry) || entry == null || string.IsNullOrEmpty(entry.Name))
                    return;

                Entry entry2;
                if (!nameServiceMappings.TryGetValue(entry.Name, out entry2) || entry != entry2)
                    return;

                nameServiceMappings.TryRemove(entry.Name, out _);
            }
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    foreach (var kv in nameServiceMappings)
                        kv.Value.Dispose();

                    this.nameServiceMappings.Clear();
                    this.nameServiceMappings = null;

                    foreach (var kv in typeServiceMappings)
                        kv.Value.Dispose();

                    this.typeServiceMappings.Clear();
                    this.typeServiceMappings = null;
                }
                disposed = true;
            }
        }

        ~ServiceContainer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        internal class Entry : IDisposable
        {
            public Entry(string name, Type type, IFactory factory)
            {
                this.Name = name;
                this.Type = type;
                this.Factory = factory;
            }

            public string Name { get; }
            public Type Type { get; }
            public IFactory Factory { get; }
            public void Dispose()
            {
                Factory.Dispose();
            }
        }

        internal interface IFactory : IDisposable
        {
            object Create();
        }

        internal class GenericFactory<T> : IFactory
        {
            private Func<T> func;

            public GenericFactory(Func<T> func)
            {
                this.func = func;
            }

            public virtual object Create()
            {
                return func();
            }

            public void Dispose()
            {
            }
        }

        internal class SingleInstanceFactory : IFactory
        {
            private object target;

            public SingleInstanceFactory(object target)
            {
                this.target = target;
            }

            public virtual object Create()
            {
                return target;
            }

            #region IDisposable Support
            private bool disposed = false;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        var disposable = target as IDisposable;
                        if (disposable != null)
                            disposable.Dispose();
                        this.target = null;
                    }

                    disposed = true;
                }
            }

            ~SingleInstanceFactory()
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
}
