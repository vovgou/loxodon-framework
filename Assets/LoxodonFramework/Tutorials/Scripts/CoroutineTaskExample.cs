using Loxodon.Framework.Asynchronous;
using System.Collections;
using UnityEngine;

namespace Loxodon.Framework.Tutorials
{
    public class CoroutineTaskExample : MonoBehaviour
    {
        IEnumerator Start()
        {
            CoroutineTask task = new CoroutineTask(DoTask())
                .ContinueWith(
                    DoContinueTask(),
                    CoroutineTaskContinuationOptions.OnCompleted
                    | CoroutineTaskContinuationOptions.OnFaulted
                ).ContinueWith(
                    () => { Debug.Log("The task is completed"); }
                );

            yield return task.WaitForDone();

            Debug.LogFormat("IsDone:{0} IsCompleted:{1} IsFaulted:{2} IsCancelled:{3}",
                task.IsDone, task.IsCompleted, task.IsFaulted, task.IsCancelled);
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
                Debug.LogFormat("Task:i = {0}", i);
                yield return new WaitForSeconds(0.5f);
            }
        }

        protected IEnumerator DoContinueTask()
        {
            int n = 10;
            for (int i = 0; i < n; i++)
            {
                Debug.LogFormat("ContinueTask:i = {0}", i);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
