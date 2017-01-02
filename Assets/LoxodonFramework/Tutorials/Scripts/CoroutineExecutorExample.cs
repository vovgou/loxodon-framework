using UnityEngine;
using System.Collections;
using System.Text;

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Tutorials
{
	public class CoroutineExecutorExample : MonoBehaviour
	{
		private ICoroutineExecutor executor;

		IEnumerator Start ()
		{
			this.executor = new CoroutineExecutor (); 

			IAsyncResult r1 = this.executor.RunOnCoroutine (Task1 ());
			yield return r1.WaitForDone ();

			IAsyncResult r2 = this.executor.RunOnCoroutine (promise => Task2 (promise));
			yield return r2.WaitForDone ();

			IAsyncResult<string> r3 = this.executor.RunOnCoroutine<string> (promise => Task3 (promise));
			yield return new WaitForSeconds (0.5f);
			r3.Cancel ();
			yield return r3.WaitForDone ();
			Debug.LogFormat ("Task3 IsCalcelled:{0}", r3.IsCancelled);

			IProgressResult<float,string> r4 = this.executor.RunOnCoroutine<float,string> (promise => Task4 (promise));
			while (!r4.IsDone) {
				yield return null;
				Debug.LogFormat ("Task4 Progress:{0}%", Mathf.FloorToInt (r4.Progress * 100));
			}

			Debug.LogFormat ("Task4 Result:{0}", r4.Result);
		}

		IEnumerator Task1 ()
		{
			Debug.Log ("The task1 start");
			yield return null;
			Debug.Log ("The task1 end");
		}

		IEnumerator Task2 (IPromise promise)
		{
			Debug.Log ("The task2 start");
			yield return null;
			promise.SetResult (); /* set a result of the task */
			Debug.Log ("The task2 end");
		}

		IEnumerator Task3 (IPromise<string> promise)
		{
			Debug.Log ("The task3 start");
			StringBuilder buf = new StringBuilder ();
			for (int i = 0; i < 50; i++) {
				/* If the task is cancelled, then stop the task */
				if (promise.IsCancellationRequested) {
					promise.SetCancelled ();			
					yield break;
				}

				buf.Append (i).Append (" ");
				yield return null;
			}
			promise.SetResult (buf.ToString ()); /* set a result of the task */
			Debug.Log ("The task3 end");
		}

		IEnumerator Task4 (IProgressPromise<float,string> promise)
		{
			Debug.Log ("The task4 start");
			int n = 10;
			StringBuilder buf = new StringBuilder ();
			for (int i = 1; i <= n; i++) {
				/* If the task is cancelled, then stop the task */
				if (promise.IsCancellationRequested) {
					promise.SetCancelled ();
					yield break;
				}

				buf.Append (i).Append (" ");

				promise.UpdateProgress (i / (float)n);
				yield return null;
			}
			promise.SetResult (buf.ToString ()); /* set a result of the task */
			Debug.Log ("The task4 end");
		}
	}
}