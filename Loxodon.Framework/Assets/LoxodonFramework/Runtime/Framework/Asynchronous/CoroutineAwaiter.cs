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
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace Loxodon.Framework.Asynchronous
{
    public class CoroutineAwaiter : IAwaiter, ICriticalNotifyCompletion
    {
        protected object _lock = new object();
        protected bool done = false;
        protected Exception exception;
        protected Action continuation;

        public bool IsCompleted { get { return this.done; } }

        public void GetResult()
        {
            lock (_lock)
            {
                if (!done)
                    throw new Exception("The task is not finished yet");
            }

            if (exception != null)
                ExceptionDispatchInfo.Capture(exception).Throw();
        }

        public void SetResult(Exception exception)
        {
            lock (_lock)
            {
                if (done)
                    return;

                this.exception = exception;
                this.done = true;
                try
                {
                    if (this.continuation != null)
                        this.continuation();
                }
                catch (Exception) { }
                finally
                {
                    this.continuation = null;
                }
            }
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            lock (_lock)
            {
                if (this.done)
                {
                    continuation();
                }
                else
                {
                    this.continuation += continuation;
                }
            }
        }
    }

    public class CoroutineAwaiter<T> : CoroutineAwaiter, IAwaiter<T>, ICriticalNotifyCompletion
    {
        protected T result;

        public CoroutineAwaiter()
        {
        }

        public new T GetResult()
        {
            lock (_lock)
            {
                if (!done)
                    throw new Exception("The task is not finished yet");
            }

            if (exception != null)
                ExceptionDispatchInfo.Capture(exception).Throw();

            return result;
        }

        public void SetResult(T result, Exception exception)
        {
            lock (_lock)
            {
                if (done)
                    return;

                this.result = result;
                this.exception = exception;
                this.done = true;
                try
                {
                    if (this.continuation != null)
                        this.continuation();
                }
                catch (Exception) { }
                finally
                {
                    this.continuation = null;
                }
            }
        }
    }

    public struct AsyncOperationAwaiter : IAwaiter, ICriticalNotifyCompletion
    {
        private AsyncOperation asyncOperation;
        private Action<AsyncOperation> continuationAction;

        public AsyncOperationAwaiter(AsyncOperation asyncOperation)
        {
            this.asyncOperation = asyncOperation;
            this.continuationAction = null;
        }

        public bool IsCompleted => asyncOperation.isDone;

        public void GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");

            if (continuationAction != null)
                asyncOperation.completed -= continuationAction;
            continuationAction = null;
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            if (asyncOperation.isDone)
            {
                continuation();
            }
            else
            {
                continuationAction = (ao) => { continuation(); };
                asyncOperation.completed += continuationAction;
            }
        }
    }

    public struct AsyncOperationAwaiter<T, TResult> : IAwaiter<TResult>, ICriticalNotifyCompletion where T : AsyncOperation
    {
        private T asyncOperation;
        private Func<T, TResult> getter;
        private Action<AsyncOperation> continuationAction;

        public AsyncOperationAwaiter(T asyncOperation, Func<T, TResult> getter)
        {
            this.asyncOperation = asyncOperation ?? throw new ArgumentNullException("asyncOperation");
            this.getter = getter ?? throw new ArgumentNullException("getter");
            this.continuationAction = null;
        }

        public bool IsCompleted => asyncOperation.isDone;

        public TResult GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");

            if (continuationAction != null)
            {
                asyncOperation.completed -= continuationAction;
                continuationAction = null;
            }
            return getter(asyncOperation);
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            if (asyncOperation.isDone)
            {
                continuation();
            }
            else
            {
                continuationAction = (ao) => { continuation(); };
                asyncOperation.completed += continuationAction;
            }
        }
    }

    public struct AsyncResultAwaiter<T> : IAwaiter, ICriticalNotifyCompletion where T : IAsyncResult
    {
        private T asyncResult;

        public AsyncResultAwaiter(T asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");
            this.asyncResult = asyncResult;
        }

        public bool IsCompleted => asyncResult.IsDone;

        public void GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");

            if (asyncResult.Exception != null)
                ExceptionDispatchInfo.Capture(asyncResult.Exception).Throw();
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");
            asyncResult.Callbackable().OnCallback((ar) => { continuation(); });
        }
    }

    public struct AsyncResultAwaiter<T, TResult> : IAwaiter<TResult>, ICriticalNotifyCompletion where T : IAsyncResult<TResult>
    {
        private T asyncResult;

        public AsyncResultAwaiter(T asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");
            this.asyncResult = asyncResult;
        }

        public bool IsCompleted => asyncResult.IsDone;

        public TResult GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");

            if (asyncResult.Exception != null)
                ExceptionDispatchInfo.Capture(asyncResult.Exception).Throw();

            return this.asyncResult.Result;
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");
            asyncResult.Callbackable().OnCallback((ar) => { continuation(); });
        }
    }
}