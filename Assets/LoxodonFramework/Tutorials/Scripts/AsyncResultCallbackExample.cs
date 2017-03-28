using UnityEngine;
using System.Collections;
using Loxodon.Framework.Asynchronous;

public class AsyncResultCallbackExample : MonoBehaviour
{

    void Start()
    {
        AsyncResult result = new AsyncResult();

        /* Register a callback */
        result.Callbackable().OnCallback((r) =>
        {
            Debug.LogFormat("The task is finished. IsDone:{0} Exception:{1}", r.IsDone, r.Exception);
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
