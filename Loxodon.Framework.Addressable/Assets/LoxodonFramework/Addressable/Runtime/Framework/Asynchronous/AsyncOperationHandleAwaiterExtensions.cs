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
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Loxodon.Framework.Asynchronous
{
    public static class AsyncOperationHandleAwaiterExtensions
    {
        public static IAwaiter<object> GetAwaiter(this AsyncOperationHandle target)
        {
            return new AsyncOperationHandleAwaiter(target);
        }

        public static IAwaiter<TResult> GetAwaiter<TResult>(this AsyncOperationHandle<TResult> target)
        {
            return new AsyncOperationHandleAwaiter<TResult>(target);
        }


        public struct AsyncOperationHandleAwaiter : IAwaiter<object>, ICriticalNotifyCompletion
        {
            private AsyncOperationHandle asyncOperation;
            private Action<AsyncOperationHandle> continuationAction;

            public AsyncOperationHandleAwaiter(AsyncOperationHandle asyncOperation)
            {
                this.asyncOperation = asyncOperation;
                this.continuationAction = null;
            }

            public bool IsCompleted => asyncOperation.IsDone;

            public object GetResult()
            {
                if (!IsCompleted)
                    throw new Exception("The task is not finished yet");

                if (continuationAction != null)
                    asyncOperation.Completed -= continuationAction;
                continuationAction = null;

                if (asyncOperation.OperationException != null)
                    ExceptionDispatchInfo.Capture(asyncOperation.OperationException).Throw();

                return asyncOperation.Result;
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                if (continuation == null)
                    throw new ArgumentNullException("continuation");

                if (asyncOperation.IsDone)
                {
                    continuation();
                }
                else
                {
                    continuationAction = (ao) => { continuation(); };
                    asyncOperation.Completed += continuationAction;
                }
            }
        }

        public struct AsyncOperationHandleAwaiter<TResult> : IAwaiter<TResult>, ICriticalNotifyCompletion
        {
            private AsyncOperationHandle<TResult> asyncOperation;
            private Action<AsyncOperationHandle<TResult>> continuationAction;

            public AsyncOperationHandleAwaiter(AsyncOperationHandle<TResult> asyncOperation)
            {
                this.asyncOperation = asyncOperation;
                this.continuationAction = null;
            }

            public bool IsCompleted => asyncOperation.IsDone;

            public TResult GetResult()
            {
                if (!IsCompleted)
                    throw new Exception("The task is not finished yet");

                if (continuationAction != null)
                {
                    asyncOperation.Completed -= continuationAction;
                    continuationAction = null;
                }

                if (asyncOperation.OperationException != null)
                    ExceptionDispatchInfo.Capture(asyncOperation.OperationException).Throw();

                return asyncOperation.Result;
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                if (continuation == null)
                    throw new ArgumentNullException("continuation");

                if (asyncOperation.IsDone)
                {
                    continuation();
                }
                else
                {
                    continuationAction = (ao) => { continuation(); };
                    asyncOperation.Completed += continuationAction;
                }
            }
        }
    }
}
