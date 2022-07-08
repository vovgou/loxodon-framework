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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Loxodon.Log;
using System.Collections.Concurrent;

namespace Loxodon.Framework.Execution
{
    /// <summary>
    /// Interceptable enumerator
    /// Pooled the InterceptableEnumerator and the promise related features built in to optimize GC  
    /// </summary>
    public class InterceptableEnumerator : IEnumerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InterceptableEnumerator));
        private const int CAPACITY = 100;
        private static readonly ConcurrentQueue<InterceptableEnumerator> pools = new ConcurrentQueue<InterceptableEnumerator>();

        public static InterceptableEnumerator Create(IEnumerator routine)
        {
            InterceptableEnumerator enumerator;
            if (pools.TryDequeue(out enumerator))
            {
                enumerator.stack.Push(routine);
                return enumerator;
            }
            return new InterceptableEnumerator(routine);
        }

        private static void Free(InterceptableEnumerator enumerator)
        {
            if (pools.Count > CAPACITY)
                return;

            enumerator.Clear();
            pools.Enqueue(enumerator);
        }

        private object current;
        private Stack<IEnumerator> stack = new Stack<IEnumerator>();
        private List<Func<bool>> hasNext = new List<Func<bool>>();
        private Action<Exception> onException;
        private Action onFinally;

        public InterceptableEnumerator(IEnumerator routine)
        {
            this.stack.Push(routine);
        }

        public object Current { get { return this.current; } }

        public bool MoveNext()
        {
            try
            {
                if (!this.HasNext())
                {
                    this.OnFinally();
                    return false;
                }

                if (stack.Count <= 0)
                {
                    this.OnFinally();
                    return false;
                }

                IEnumerator ie = stack.Peek();
                bool hasNext = ie.MoveNext();
                if (!hasNext)
                {
                    this.stack.Pop();
                    return MoveNext();
                }

                this.current = ie.Current;
                if (this.current is IEnumerator)
                {
                    stack.Push(this.current as IEnumerator);
                    return MoveNext();
                }

                if (this.current is Coroutine && log.IsWarnEnabled)
                    log.Warn("The Enumerator's results contains the 'UnityEngine.Coroutine' type,If occurs an exception,it can't be catched.It is recommended to use 'yield return routine',rather than 'yield return StartCoroutine(routine)'.");

                return true;
            }
            catch (Exception e)
            {
                this.OnException(e);
                this.OnFinally();
                return false;
            }
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        private void OnException(Exception e)
        {
            try
            {
                if (this.onException == null)
                    return;

                onException(e);
            }
            catch (Exception) { }
        }

        private void OnFinally()
        {
            try
            {
                if (this.onFinally == null)
                    return;

                onFinally();
            }
            catch (Exception) { }
            finally
            {
                Free(this);
            }
        }

        private void Clear()
        {
            this.current = null;
            this.onException = null;
            this.onFinally = null;
            this.hasNext.Clear();
            this.stack.Clear();
        }

        private bool HasNext()
        {
            if (hasNext.Count > 0)
            {
                foreach (Func<bool> action in this.hasNext)
                {
                    if (!action())
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Register a condition code block.
        /// </summary>
        /// <param name="hasNext"></param>
        public virtual void RegisterConditionBlock(Func<bool> hasNext)
        {
            if (hasNext != null)
                this.hasNext.Add(hasNext);
        }

        /// <summary>
        /// Register a code block, when an exception occurs it will be executed.
        /// </summary>
        /// <param name="onException"></param>
        public virtual void RegisterCatchBlock(Action<Exception> onException)
        {
            if (onException != null)
                this.onException += onException;
        }

        /// <summary>
        /// Register a code block, when the end of the operation is executed.
        /// </summary>
        /// <param name="onFinally"></param>
        public virtual void RegisterFinallyBlock(Action onFinally)
        {
            if (onFinally != null)
                this.onFinally += onFinally;
        }
    }
}
