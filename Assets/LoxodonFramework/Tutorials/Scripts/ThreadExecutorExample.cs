using UnityEngine;
using System.Collections;
using System.Text;
using System.Threading;
#if NETFX_CORE
using System.Threading.Tasks;
#endif

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Tutorials
{
	public class ThreadExecutorExample : MonoBehaviour
	{
		private IThreadExecutor executor;

		IEnumerator Start ()
		{
			this.executor = new ThreadExecutor (); 

			IAsyncResult r1 = this.executor.Execute (Task1);
			yield return r1.WaitForDone ();

			IAsyncResult r2 = this.executor.Execute (promise => Task2 (promise));
			yield return r2.WaitForDone ();

			IAsyncResult<string> r3 = this.executor.Execute<string> (promise => Task3 (promise));
			yield return new WaitForSeconds (0.5f);
			r3.Cancel ();
			yield return r3.WaitForDone ();
			Debug.LogFormat ("Task3 IsCalcelled:{0}", r3.IsCancelled);

			IProgressResult<float,string> r4 = this.executor.Execute<float,string> (promise => Task4 (promise));
			while (!r4.IsDone) {
				yield return null;
				Debug.LogFormat ("Task4 Progress:{0}%", Mathf.FloorToInt (r4.Progress * 100));
			}

			Debug.LogFormat ("Task4 Result:{0}", r4.Result);
		}

		void Task1 ()
		{
			Debug.Log ("The task1 is running.");
		}

		void Task2 (IPromise promise)
		{
			Debug.Log ("The task2 start");
#if NETFX_CORE
            Task.Delay(100).Wait();
#else
            Thread.Sleep (100);
#endif
            promise.SetResult (); /* set a result of the task */
			Debug.Log ("The task2 end");
		}

		void Task3 (IPromise<string> promise)
		{
			Debug.Log ("The task3 start");
			StringBuilder buf = new StringBuilder ();
			for (int i = 0; i < 50; i++) {
				/* If the task is cancelled, then stop the task */
				if (promise.IsCancellationRequested) {
					promise.SetCancelled ();			
					break;
				}

				buf.Append (i).Append (" ");
#if NETFX_CORE
                Task.Delay(100).Wait();
#else
                Thread.Sleep(100);
#endif
            }
            promise.SetResult (buf.ToString ()); /* set a result of the task */
			Debug.Log ("The task3 end");
		}

		void Task4 (IProgressPromise<float,string> promise)
		{
			Debug.Log ("The task4 start");
			int n = 10;
			StringBuilder buf = new StringBuilder ();
			for (int i = 1; i <= n; i++) {
				/* If the task is cancelled, then stop the task */
				if (promise.IsCancellationRequested) {
					promise.SetCancelled ();
					break;
				}

				buf.Append (i).Append (" ");

				promise.UpdateProgress (i / (float)n);
#if NETFX_CORE
                Task.Delay(100).Wait();
#else
                Thread.Sleep(100);
#endif
            }
            promise.SetResult (buf.ToString ()); /* set a result of the task */
			Debug.Log ("The task4 end");
		}
	}
}