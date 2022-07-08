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

using UnityEngine;
using System.Collections;

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Tutorials
{
    public class InterceptableEnumeratorExample : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return this.TestInterceptException();

            //yield return this.TestInterceptMoveNextMethod();
        }

        protected IEnumerator TestInterceptException()
        {
            ProgressResult<float, bool> result = new ProgressResult<float, bool>(true);

            /* Register a callback */
            result.Callbackable().OnProgressCallback(p => Debug.LogFormat("Progress:{0}%", p * 100));

            result.Callbackable().OnCallback((r) =>
            {
                Debug.LogFormat("The task is finished. IsCancelled:{0} Result:{1} Exception:{2}", r.IsCancelled, r.Result, r.Exception);
            });

            InterceptableEnumerator routine = InterceptableEnumerator.Create(DoTask(result));

            routine.RegisterCatchBlock((e) =>
            {
                Debug.LogError(e);
            });

            routine.RegisterFinallyBlock(() =>
            {
                Debug.Log("this is a finally block.");
            });

            /* Start the task */
            this.StartCoroutine(routine);
            yield break;
        }

        protected IEnumerator TestInterceptMoveNextMethod()
        {
            ProgressResult<float, bool> result = new ProgressResult<float, bool>(true);

            /* Register a callback */
            result.Callbackable().OnProgressCallback(p => Debug.LogFormat("Progress:{0}%", p * 100));

            result.Callbackable().OnCallback((r) =>
            {
                Debug.LogFormat("The task is finished. IsCancelled:{0} Result:{1} Exception:{2}", r.IsCancelled, r.Result, r.Exception);
            });

            InterceptableEnumerator routine = InterceptableEnumerator.Create(DoTask(result));

            /* if result.IsCancellationRequested == true ,the task will be cancelled. */
            routine.RegisterConditionBlock(() => !(result.IsCancellationRequested));

            routine.RegisterFinallyBlock(() =>
            {
                Debug.Log("this is a finally block.");
            });

            /* Start the task */
            this.StartCoroutine(routine);

            yield return new WaitForSeconds(0.5f);
            result.Cancel();
        }


        /// <summary>
        /// Simulate a task.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="promise">Promise.</param>
        protected IEnumerator DoTask(IProgressPromise<float, bool> promise)
        {
            int n = 50;
            for (int i = 0; i < n; i++)
            {
                promise.UpdateProgress((float)i / n);
                yield return new WaitForSeconds(0.1f);
                if (i == 20)
                    throw new System.Exception("This is a test, not a bug.");
            }
            promise.SetResult(true);
        }
    }
}
