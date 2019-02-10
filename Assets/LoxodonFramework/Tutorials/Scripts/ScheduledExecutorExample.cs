using UnityEngine;
using System.Collections;

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Tutorials
{
#pragma warning disable
    public class ScheduledExecutorExample : MonoBehaviour
    {
        private IScheduledExecutor scheduled;

        void Start()
        {
            //		this.scheduled = new CoroutineScheduledExecutor (); /* run on the main thread. */
            this.scheduled = new ThreadScheduledExecutor(); /* run on the background thread. */
            this.scheduled.Start();


            IAsyncResult result = this.scheduled.ScheduleAtFixedRate(() =>
            {
                Debug.Log("This is a test.");
            }, 1000, 2000);
        }

        void OnDestroy()
        {
            if (this.scheduled != null)
            {
                this.scheduled.Stop();
                this.scheduled = null;
            }
        }

    }
}