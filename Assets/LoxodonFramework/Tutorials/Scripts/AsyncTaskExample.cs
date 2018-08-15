using UnityEngine;
using System.Collections;

using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Tutorials
{
    public class AsyncTaskExample : MonoBehaviour
    {
        protected IEnumerator Start()
        {
            AsyncTask task = new AsyncTask(DoTask(), true);

            /* Start the task */
            task.OnPreExecute(() =>
            {
                Debug.Log("The task has started.");
            }).OnPostExecute(() =>
            {
                Debug.Log("The task has completed.");/* only execute successfully */
            }).OnError((e) =>
            {
                Debug.LogFormat("An error occurred:{0}", e);
            }).OnFinish(() =>
            {
                Debug.Log("The task has been finished.");/* completed or error or canceled*/
            }).Start();

            //		/* Cancel the task in three seconds
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
        protected IEnumerator DoTask()
        {
            int n = 10;
            for (int i = 0; i < n; i++)
            {
                Debug.LogFormat("i = {0}", i);
                yield return new WaitForSeconds(0.5f);
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