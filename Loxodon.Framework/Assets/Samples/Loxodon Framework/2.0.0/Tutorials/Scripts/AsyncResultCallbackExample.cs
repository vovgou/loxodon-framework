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

namespace Loxodon.Framework.Tutorials
{
    public class AsyncResultCallbackExample : MonoBehaviour
    {

        void Start()
        {
            AsyncResult result = new AsyncResult();

            /* Register a callback */
            result.Callbackable().OnCallback((r) =>
            {
                if (r.Exception != null)
                {
                    Debug.LogFormat("The task is finished.IsDone:{0} Exception:{1}", r.IsDone, r.Exception);
                    return;
                }

                Debug.LogFormat("The task is finished. IsDone:{0} Result:{1}", r.IsDone, r.Result);
            });

            /* Start the task */
            this.StartCoroutine(DoTask(result));
        }

        /// <summary>
        /// Simulate a task.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="promise">Promise.</param>
        protected IEnumerator DoTask(IPromise promise)
        {
            yield return new WaitForSeconds(0.5f);
            promise.SetResult();
        }

    }
}
