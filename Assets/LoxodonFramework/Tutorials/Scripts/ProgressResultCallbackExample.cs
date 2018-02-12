using UnityEngine;
using System.Collections;
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Tutorials
{

    public class ProgressResultCallbackExample : MonoBehaviour
    {
        void Start()
        {
            ProgressResult<float, bool> result = new ProgressResult<float, bool>();

            /* Register a callback */
            result.Callbackable().OnProgressCallback(p =>
            {
                Debug.LogFormat("Progress:{0}%", p * 100);
            });

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
        protected IEnumerator DoTask(IProgressPromise<float, bool> promise)
        {
            int n = 50;
            for (int i = 0; i < n; i++)
            {
                promise.UpdateProgress((float)i / n);
                yield return new WaitForSeconds(0.1f);
            }
            promise.SetResult(true);
        }
    }
}
