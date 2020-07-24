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
#if NETFX_CORE || NET_STANDARD_2_0 || NET_4_6
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;
#endif

namespace Loxodon.Framework.Tutorials
{
    public class AsyncAndAwaitExample : MonoBehaviour
    {
#if NETFX_CORE || NET_STANDARD_2_0 || NET_4_6
        async void Start()
        {
            await new WaitForSeconds(2f);
            Debug.Log("WaitForSeconds  End");

            await Task.Delay(1000);
            Debug.Log("Delay  End");

            UnityWebRequest www = await UnityWebRequest.Get("http://www.baidu.com").SendWebRequest();

            if (!www.isHttpError && !www.isNetworkError)
                Debug.Log(www.downloadHandler.text);

            int result = await Calculate();
            Debug.LogFormat("Calculate Result = {0} Calculate Task End,Current Thread ID:{1}", result, Thread.CurrentThread.ManagedThreadId);

            await new WaitForMainThread();
            Debug.LogFormat("Switch to the main thread,Current Thread ID:{0}", Thread.CurrentThread.ManagedThreadId);

            await new WaitForSecondsRealtime(1f);
            Debug.Log("WaitForSecondsRealtime  End");

            await DoTask(5);
            Debug.Log("DoTask End");
        }

        IAsyncResult<int> Calculate()
        {
            return Executors.RunAsync<int>(() =>
            {
                Debug.LogFormat("Calculate Task ThreadId:{0}", Thread.CurrentThread.ManagedThreadId);
                int total = 0;
                for (int i = 0; i < 20; i++)
                {
                    total += i;
                    try
                    {
                        Thread.Sleep(100);
                    }
                    catch (Exception) { }
                }
                return total;
            });
        }

        IEnumerator DoTask(int n)
        {
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < n; i++)
            {
                yield return null;
            }
        }
#endif
    }
}
