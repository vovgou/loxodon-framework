using UnityEngine;
using System.Collections;
using System.Text;
#if NETFX_CORE
using System.Threading.Tasks;
#else
using System.Threading;
#endif

using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Tutorials
{
    public class ProgressTaskExample2 : MonoBehaviour
    {
        protected IEnumerator Start()
        {
            ProgressTask<float, string> task = new ProgressTask<float, string>(new System.Action<IProgressPromise<float, string>>(DoTask), false, true);

            /* Start the task */
            task.OnPreExecute(() =>
            {
                Debug.Log("The task has started.");
            }).OnPostExecute((result) =>
            {
                Debug.LogFormat("The task has completed. result:{0}", result);/* only execute successfully */
            }).OnProgressUpdate((progress) =>
            {
                Debug.LogFormat("The current progress:{0}%", (int)(progress * 100));
            }).OnError((e) =>
            {
                Debug.LogFormat("An error occurred:{0}", e);
            }).OnFinish(() =>
            {
                Debug.Log("The task has been finished.");/* completed or error or canceled*/
            }).Start();

            //		/* Cancel the task in three seconds.*/
            this.StartCoroutine(DoCancel(task));

            /* wait for the task finished. */
            yield return task.WaitForDone();

            Debug.LogFormat("IsDone:{0} IsCanceled:{1} Exception:{2}", task.IsDone, task.IsCancelled, task.Exception);
        }

        /// <summary>
        /// Simulate a task.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="promise">Promise.</param>
        protected void DoTask(IProgressPromise<float, string> promise)
        {
            try
            {
                int n = 50;
                float progress = 0f;
                StringBuilder buf = new StringBuilder();
                for (int i = 0; i < n; i++)
                {
                    /* If the task is cancelled, then stop the task */
                    if (promise.IsCancellationRequested)
                    {
                        promise.SetCancelled();
                        break;
                    }

                    progress = i / (float)n;
                    buf.Append(" ").Append(i);
                    promise.UpdateProgress(progress);/* update the progress of task. */
#if NETFX_CORE
                     Task.Delay(200).Wait();
#else
                    Thread.Sleep(200);
#endif
                }
                promise.UpdateProgress(1f);
                promise.SetResult(buf.ToString()); /* update the result. */
            }
            catch (System.Exception e)
            {
                promise.SetException(e);
            }
        }

        /// <summary>
        /// Cancel a task.
        /// </summary>
        /// <returns>The cancel.</returns>
        /// <param name="result">Result.</param>
        protected IEnumerator DoCancel(IAsyncResult result)
        {
            yield return new WaitForSeconds(3f);
            result.Cancel();
        }
    }
}