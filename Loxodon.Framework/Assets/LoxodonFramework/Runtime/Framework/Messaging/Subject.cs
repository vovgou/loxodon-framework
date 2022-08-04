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

using Loxodon.Log;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Loxodon.Framework.Messaging
{
    public abstract class SubjectBase
    {
        public abstract void Publish(object message);
    }

    public class Subject<T> : SubjectBase
    {
        private readonly ConcurrentDictionary<string, WeakReference<Subscription>> subscriptions = new ConcurrentDictionary<string, WeakReference<Subscription>>();
        public bool IsEmpty() { return subscriptions.Count <= 0; }

        public override void Publish(object message)
        {
            this.Publish((T)message);
        }

        public void Publish(T message)
        {
            if (subscriptions.Count <= 0)
                return;

            foreach (var kv in subscriptions)
            {
                Subscription subscription;
                kv.Value.TryGetTarget(out subscription);
                if (subscription != null)
                    subscription.Publish(message);
                else
                    subscriptions.TryRemove(kv.Key, out _);
            }
        }

        public ISubscription<T> Subscribe(Action<T> action)
        {
            return new Subscription(this, action);
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

        class Subscription : ISubscription<T>
        {
            private static readonly ILog log = LogManager.GetLogger(typeof(Subscription));

            private Subject<T> subject;
            private Action<T> action;
            private SynchronizationContext context;
            public string Key { get; private set; }

            public Subscription(Subject<T> subject, Action<T> action)
            {
                this.subject = subject;
                this.action = action;
                this.Key = Guid.NewGuid().ToString();
                this.subject.Add(this);
            }

            public void Publish(T message)
            {
                try
                {
                    if (this.context != null)
                        context.Post(state => action((T)state), message);
                    else
                        action(message);
                }
                catch (Exception e)
                {
#if DEBUG
                    throw;
#else
                    if (log.IsWarnEnabled)
                        log.Warn(e);
#endif
                }
            }

            public ISubscription<T> ObserveOn(SynchronizationContext context)
            {
                this.context = context ?? throw new ArgumentNullException("context");
                return this;
            }

            #region IDisposable Support
            private bool disposed = false;

            protected virtual void Dispose(bool disposing)
            {
                if (this.disposed)
                    return;

                try
                {
                    if (this.disposed)
                        return;

                    if (subject != null)
                        subject.Remove(this);

                    context = null;
                    action = null;
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
