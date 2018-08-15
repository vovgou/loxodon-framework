using System;
using System.Collections;
using System.Collections.Generic;
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
            //Executors.Create();

            //int n = Environment.CurrentManagedThreadId;

            //Debug.LogFormat("ThreadId:{0}",n);

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

            Loxodon.Framework.Asynchronous.IAsyncResult result = Executors.RunOnCoroutine(DoRun());

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
