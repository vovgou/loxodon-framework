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
using System.Collections.Generic;

namespace Loxodon.Framework.Messaging
{
    public abstract class SubjectBase
    {
        public abstract void Publish(object message);
    }

    public class Subject<T> : SubjectBase
    {
        private readonly object _lock = new object();
        private readonly List<Action<T>> actions = new List<Action<T>>();

        public bool IsEmpty()
        {
            lock (_lock)
            {
                return this.actions.Count <= 0;
            }
        }

        public override void Publish(object message)
        {
            this.Publish((T)message);
        }

        public void Publish(T message)
        {
            Action<T>[] array = null;
            lock (_lock)
            {
                if (actions.Count <= 0)
                    return;

                array = this.actions.ToArray();
            }

            if (array == null || array.Length <= 0)
                return;

            for (int i = 0; i < array.Length; i++)
            {
                try
                {
                    array[i](message);
                }
                catch (Exception) { }
            }
        }

        public IDisposable Subscribe(Action<T> action)
        {
            this.Add(action);
            return new Subscription(this, action);
        }

        internal void Add(Action<T> action)
        {
            lock (_lock)
            {
                this.actions.Add(action);
            }
        }

        internal void Remove(Action<T> action)
        {
            lock (_lock)
            {
                this.actions.Remove(action);
            }
        }

        class Subscription : IDisposable
        {
            private readonly object _lock = new object();
            private Subject<T> parent;
            private Action<T> action;

            public Subscription(Subject<T> parent, Action<T> action)
            {
                this.parent = parent;
                this.action = action;
            }

            #region IDisposable Support
            private bool disposed = false;

            protected virtual void Dispose(bool disposing)
            {
                if (this.disposed)
                    return;

                lock (_lock)
                {
                    try
                    {
                        if (this.disposed)
                            return;

                        if (parent != null)
                        {
                            parent.Remove(action);
                            action = null;
                            parent = null;
                        }
                    }
                    catch (Exception) { }
                    disposed = true;
                }
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
