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

using System.Collections;
using UnityEngine;
using Loxodon.Framework.Execution;
#if NETFX_CORE
using System.Threading.Tasks;
#endif
namespace Loxodon.Framework.Tutorials
{
    public class ExecutorExample : MonoBehaviour
    {

        IEnumerator Start()
        {
            Executors.RunAsync(() =>
            {
                Debug.LogFormat("RunAsync ");
            });


            Executors.RunAsync(() =>
            {
#if NETFX_CORE
            Task.Delay(1000).Wait();
#endif
            Executors.RunOnMainThread(() =>
                {
                    Debug.LogFormat("RunOnMainThread Time:{0} frame:{1}", Time.time, Time.frameCount);
                }, true);
            });

            Executors.RunOnMainThread(() =>
            {
                Debug.LogFormat("RunOnMainThread 2 Time:{0} frame:{1}", Time.time, Time.frameCount);
            }, false);

            Asynchronous.IAsyncResult result = Executors.RunOnCoroutine(DoRun());

            yield return result.WaitForDone();

            Debug.LogFormat("============finished=============");

        }

        IEnumerator DoRun()
        {
            for (int i = 0; i < 10; i++)
            {
                Debug.LogFormat("i = {0}", i);
                yield return null;
            }
        }
    }
}
