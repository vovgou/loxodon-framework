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

using Loxodon.Framework.Execution;
using System.Threading;
using UnityEngine;

namespace Loxodon.Framework.Tutorials
{
    public class MainLoopExecutorExample : MonoBehaviour
	{
		private IMainLoopExecutor executor;

		void Start ()
		{
			this.executor = new MainLoopExecutor (); 

			Debug.LogFormat ("ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);

			Executors.RunAsync(() => {
			
				this.executor.RunOnMainThread (Task1, true);

				this.executor.RunOnMainThread<string> (Task2);

				Debug.LogFormat ("run on the backgound thread. ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);
			});
		}

		void Task1 ()
		{		
			Debug.LogFormat ("This is a task1,run on the main thread. ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);
		}

		string Task2 ()
		{
			Debug.LogFormat ("This is a task2,run on the main thread. ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);
			return this.name;
		}
	}
}