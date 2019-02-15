using System;

namespace Loxodon.Framework.Execution
{
    public class MainLoopExecutor : AbstractExecutor, IMainLoopExecutor
    {
        public virtual void RunOnMainThread(Action action, bool waitForExecution = false)
        {
            Executors.RunOnMainThread(action, waitForExecution);
        }

        public virtual TResult RunOnMainThread<TResult>(Func<TResult> func)
        {
            return Executors.RunOnMainThread(func);
        }
    }
}
