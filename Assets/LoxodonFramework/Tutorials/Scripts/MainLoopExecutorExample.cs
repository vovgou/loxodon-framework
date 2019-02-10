using UnityEngine;
using System.Collections;
using System.Threading;

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

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