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
using System.Threading;

namespace Loxodon.Framework.Net.Connection
{
    public class Subject<T> : IDisposable
    {
        private readonly ConcurrentDictionary<string, WeakReference<Subscription>> subscriptions = new ConcurrentDictionary<string, WeakReference<Subscription>>();

        public Subject()
        {
        }

        public virtual void Publish(T message)
        {
            if (subscriptions.Count <= 0)
                return;

            foreach (var kv in subscriptions)
            {
                var key = kv.Key;
                var reference = kv.Value;
                Subscription subscription;
                if (reference.TryGetTarget(out subscription) && subscription != null)
                    subscription.Publish(message);
                else
                    subscriptions.TryRemove(key, out _);
            }
        }

        public virtual ISubscription<T> Subscribe()
        {
            return new Subscription(this);
        }

        public virtual ISubscription<T> Subscribe(Predicate<T> filter)
        {
            return new Subscription(this, filter);
        }

        void Add(Subscription subscription)
        {
            var reference = new WeakReference<Subscription>(subscription, false);
            this.subscriptions.TryAdd(subscription.Key, reference);
        }

        void Remove(Subscription subscription)
        {
            this.subscriptions.TryRemove(subscription.Key, out _);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                subscriptions.Clear();
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        class Subscription : ISubscription<T>
        {
            private Subject<T> subject;
            private Predicate<T> filter;
            private Action<T> action;
            private SynchronizationContext context;

            public Subscription(Subject<T> subject) : this(subject, null)
            {
            }

            public Subscription(Subject<T> subject, Predicate<T> filter)
            {
                this.Key = Guid.NewGuid().ToString();
                this.subject = subject ?? throw new ArgumentNullException("subject");
                this.filter = filter;
            }

            public string Key { get; private set; }

            public void Publish(T message)
            {
                try
                {
                    if (filter != null && !filter(message))
                        return;

                    if (this.context != null)
                    {
                        context.Post(state => action((T)state), message);
                    }
                    else
                    {
                        action(message);
                    }
                }
                catch (Exception)
                {
#if DEBUG
                    throw;
#endif
                }
            }

            public ISubscription<T> Filter(Predicate<T> filter)
            {
                if (this.action != null)
                    throw new InvalidOperationException("Please register the filter before the Subscribe() function is called");

                this.filter = filter ?? throw new ArgumentNullException("filter");
                return this;
            }

            public ISubscription<T> ObserveOn(SynchronizationContext context)
            {
                if (this.action != null)
                    throw new InvalidOperationException("Please set the SynchronizationContext before the Subscribe() function is called");

                this.context = context ?? throw new ArgumentNullException("context");
                return this;
            }

            public ISubscription<T> Subscribe(Action<T> action)
            {
                if (this.action != null)
                    throw new InvalidOperationException("The action already exists, please do not subscribe again");

                this.action = action ?? throw new ArgumentNullException("action");
                this.subject.Add(this);
                return this;
            }

            #region IDisposable Support
            private bool disposed = false;

            protected virtual void Dispose(bool disposing)
            {
                try
                {
                    if (this.disposed)
                        return;

                    if (subject != null)
                        subject.Remove(this);

                    Key = null;
                    context = null;
                    action = null;
                    filter = null;
                    subject = null;
                }
                catch (Exception) { }
                disposed = true;
            }

            ~Subscription()
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
