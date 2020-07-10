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

#if NETFX_CORE || NET_STANDARD_2_0 || NET_4_6
using Loxodon.Framework.Execution;
using System;
using System.Collections;
using UnityEngine;

namespace Loxodon.Framework.Asynchronous
{
    public static class CoroutineAwaiterExtensions
    {
        private static void RunOnCoroutine(IEnumerator routine, Action<Exception> onFinished)
        {
            InterceptableEnumerator enumerator = routine is InterceptableEnumerator ? (InterceptableEnumerator)routine : new InterceptableEnumerator(routine);
            Exception exception = null;
            enumerator.RegisterCatchBlock(e =>
            {
                exception = e;
            });
            enumerator.RegisterFinallyBlock(() =>
            {
                onFinished(exception);
            });
            Executors.RunOnCoroutineNoReturn(enumerator);
        }

        private static IEnumerator DoYieldInstruction(YieldInstruction instruction)
        {
            yield return instruction;
        }

        private static IEnumerator DoYieldInstruction(CustomYieldInstruction instruction)
        {
            yield return instruction;
        }

        public static CoroutineAwaiter GetAwaiter(this IEnumerator coroutine)
        {
            CoroutineAwaiter awaiter = new CoroutineAwaiter();
            RunOnCoroutine(coroutine, e => awaiter.SetResult(e));
            return awaiter;
        }

        public static CoroutineAwaiter GetAwaiter(this YieldInstruction instruction)
        {
            CoroutineAwaiter awaiter = new CoroutineAwaiter();
            RunOnCoroutine(DoYieldInstruction(instruction), e => awaiter.SetResult(e));
            return awaiter;
        }

        public static CoroutineAwaiter GetAwaiter(this WaitForMainThread instruction)
        {
            CoroutineAwaiter awaiter = new CoroutineAwaiter();
            Executors.RunOnMainThread(() =>
            {
                awaiter.SetResult(null);
            });
            return awaiter;
        }

        public static CoroutineAwaiter GetAwaiter(this WaitForBackgroundThread instruction)
        {
            CoroutineAwaiter awaiter = new CoroutineAwaiter();
            Executors.RunAsyncNoReturn(() =>
            {
                awaiter.SetResult(null);
            });
            return awaiter;
        }

        public static CoroutineAwaiter<CustomYieldInstruction> GetAwaiter(this CustomYieldInstruction target)
        {
            CoroutineAwaiter<CustomYieldInstruction> awaiter = new CoroutineAwaiter<CustomYieldInstruction>();
            RunOnCoroutine(DoYieldInstruction(target), e => awaiter.SetResult(target, e));
            return awaiter;
        }

        public static CoroutineAwaiter<T> GetAwaiter<T>(this T target) where T : AsyncOperation
        {
            CoroutineAwaiter<T> awaiter = new CoroutineAwaiter<T>();
            Action<AsyncOperation> handler = null;
            handler = (operation) =>
            {
                try
                {
                    awaiter.SetResult(target, null);
                }
                catch (Exception) { }
                try
                {
                    if (handler != null)
                        target.completed -= handler;
                }
                catch (Exception) { }
            };
            target.completed += handler;
            return awaiter;
        }

        public static CoroutineAwaiter<UnityEngine.Object> GetAwaiter(this ResourceRequest target)
        {
            CoroutineAwaiter<UnityEngine.Object> awaiter = new CoroutineAwaiter<UnityEngine.Object>();
            Action<AsyncOperation> handler = null;
            handler = (operation) =>
            {
                try
                {
                    awaiter.SetResult(target.asset, null);
                }
                catch (Exception) { }
                try
                {
                    if (handler != null)
                        target.completed -= handler;
                }
                catch (Exception) { }
            };
            target.completed += handler;
            return awaiter;
        }

        public static CoroutineAwaiter<AssetBundle> GetAwaiter(this AssetBundleCreateRequest target)
        {
            CoroutineAwaiter<AssetBundle> awaiter = new CoroutineAwaiter<AssetBundle>();
            Action<AsyncOperation> handler = null;
            handler = (operation) =>
            {
                try
                {
                    awaiter.SetResult(target.assetBundle, null);
                }
                catch (Exception) { }
                try
                {
                    if (handler != null)
                        target.completed -= handler;
                }
                catch (Exception) { }
            };
            target.completed += handler;
            return awaiter;
        }

        public static CoroutineAwaiter<object> GetAwaiter(this IAsyncResult asyncResult)
        {
            CoroutineAwaiter<object> awaiter = new CoroutineAwaiter<object>();
            asyncResult.Callbackable().OnCallback(ar =>
            {
                awaiter.SetResult(ar.Result, ar.Exception);
            });
            return awaiter;
        }

        public static CoroutineAwaiter<TResult> GetAwaiter<TResult>(this IAsyncResult<TResult> asyncResult)
        {
            CoroutineAwaiter<TResult> awaiter = new CoroutineAwaiter<TResult>();
            asyncResult.Callbackable().OnCallback(ar =>
            {
                awaiter.SetResult(ar.Result, ar.Exception);
            });
            return awaiter;
        }
    }

    public class WaitForMainThread : CustomYieldInstruction
    {
        public static readonly WaitForMainThread Default = new WaitForMainThread();

        public override bool keepWaiting => false;
    }

    public class WaitForBackgroundThread
    {
        public static readonly WaitForBackgroundThread Default = new WaitForBackgroundThread();
    }
}
#endif