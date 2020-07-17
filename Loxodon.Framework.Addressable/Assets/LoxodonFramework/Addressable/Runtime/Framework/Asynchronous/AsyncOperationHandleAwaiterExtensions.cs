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
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Loxodon.Framework.Asynchronous
{
    public static class AsyncOperationHandleAwaiterExtensions
    {
#if !UNITY_WEBGL
        public static TaskAwaiter<object> GetAwaiter(this AsyncOperationHandle target)
        {
            return target.Task.GetAwaiter();
        }

        public static TaskAwaiter<TResult> GetAwaiter<TResult>(this AsyncOperationHandle<TResult> target)
        {
            return target.Task.GetAwaiter();
        }
#else
        public static CoroutineAwaiter<object> GetAwaiter(this AsyncOperationHandle target)
        {
            CoroutineAwaiter<object> awaiter = new CoroutineAwaiter<object>();
            Action<AsyncOperationHandle> handler = null;
            handler = (handle) =>
            {
                try
                {
                    if (handle.OperationException != null)
                        awaiter.SetResult(null, handle.OperationException);
                    else
                        awaiter.SetResult(handle.Result, null);
                }
                catch (Exception) { }
                try
                {
                    if (handler != null)
                        handle.Completed -= handler;
                }
                catch (Exception) { }
            };
            target.Completed += handler;
            return awaiter;
        }

        public static CoroutineAwaiter<TResult> GetAwaiter<TResult>(this AsyncOperationHandle<TResult> target)
        {
            CoroutineAwaiter<TResult> awaiter = new CoroutineAwaiter<TResult>();
            Action<AsyncOperationHandle<TResult>> handler = null;
            handler = (handle) =>
            {
                try
                {
                    if (handle.OperationException != null)
                        awaiter.SetResult(default(TResult), handle.OperationException);
                    else
                        awaiter.SetResult(handle.Result, null);
                }
                catch (Exception) { }
                try
                {
                    if (handler != null)
                        handle.Completed -= handler;
                }
                catch (Exception) { }
            };
            target.Completed += handler;
            return awaiter;
        }
#endif
    }
}
