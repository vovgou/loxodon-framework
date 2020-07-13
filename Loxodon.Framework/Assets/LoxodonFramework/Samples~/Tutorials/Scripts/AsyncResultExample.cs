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
using System.Collections;
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Tutorials
{

	public class AsyncResultExample : MonoBehaviour
	{

		protected IEnumerator Start ()
		{
			AsyncResult result = new AsyncResult (true);

			/* Start the task */
			this.StartCoroutine (DoTask (result));

			/* Cancel the task in three seconds.*/
			this.StartCoroutine (DoCancel (result));
	
			/* wait for the task finished. */
			yield return result.WaitForDone ();

            //		while (!result.IsDone)
            //			yield return null;

            Debug.LogFormat ("IsDone:{0} IsCanceled:{1} Exception:{2}", result.IsDone, result.IsCancelled, result.Exception);
		}

		/// <summary>
		/// Simulate a task.
		/// </summary>
		/// <returns>The task.</returns>
		/// <param name="promise">Promise.</param>
		protected IEnumerator DoTask (IPromise promise)
		{
			for (int i = 0; i < 20; i++) {
				/* If the task is cancelled, then stop the task */
				if (promise.IsCancellationRequested) {		
					promise.SetCancelled ();
					yield break;
				}

				Debug.LogFormat ("i = {0}", i);
				yield return new WaitForSeconds (0.5f);
			}

			promise.SetResult ();
		}

		/// <summary>
		/// Cancel a task.
		/// </summary>
		/// <returns>The cancel.</returns>
		/// <param name="result">Result.</param>
		protected IEnumerator DoCancel (IAsyncResult result)
		{
			yield return new WaitForSeconds (3f);
			result.Cancel ();
		}
	}

}