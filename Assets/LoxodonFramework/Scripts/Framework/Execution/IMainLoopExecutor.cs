using System;

namespace Loxodon.Framework.Execution
{
    public interface IMainLoopExecutor
    {
        void RunOnMainThread(Action action, bool waitForExecution = false);

        TResult RunOnMainThread<TResult>(Func<TResult> func);
    }
}
