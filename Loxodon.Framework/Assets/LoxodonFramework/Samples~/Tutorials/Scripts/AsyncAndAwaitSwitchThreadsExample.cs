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
    public class AsyncAndAwaitSwitchThreadsExample : MonoBehaviour
    {
#if NETFX_CORE || NET_STANDARD_2_0 || NET_4_6
        async void Start()
        {
            //Unity Thread
            Debug.LogFormat("1. ThreadID:{0}",Thread.CurrentThread.ManagedThreadId);

            await new WaitForBackgroundThread();

            //Background Thread
            Debug.LogFormat("2.After the WaitForBackgroundThread.ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);

            await new WaitForMainThread();

            //Unity Thread
            Debug.LogFormat("3.After the WaitForMainThread.ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);

            await Task.Delay(3000).ConfigureAwait(false);

            //Background Thread
            Debug.LogFormat("4.After the Task.Delay.ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);

            await new WaitForSeconds(1f);

            //Unity Thread
            Debug.LogFormat("5.After the WaitForSeconds.ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);
        }
#endif
    }
}
