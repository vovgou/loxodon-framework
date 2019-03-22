using System;
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Execution
{
    public class ThreadExecutor : AbstractExecutor, IThreadExecutor
    {
        public virtual Asynchronous.IAsyncResult Execute(Action action)
        {
            AsyncResult result = new AsyncResult(true);
            Executors.RunAsyncNoReturn(() =>
            {
                try
                {
                    if (result.IsCancellationRequested)
                    {
                        result.SetCancelled();
                        return;
                    }

                    action();
                    result.SetResult();
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return result;
        }

        public virtual IAsyncResult<TResult> Execute<TResult>(Func<TResult> func)
        {
            AsyncResult<TResult> result = new AsyncResult<TResult>(true);
            Executors.RunAsyncNoReturn(() =>
            {
                try
                {
                    if (result.IsCancellationRequested)
                    {
                        result.SetCancelled();
                        return;
                    }

                    TResult tr = func();
                    result.SetResult(tr);
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return result;
        }

        public virtual Asynchronous.IAsyncResult Execute(Action<IPromise> action)
        {
            AsyncResult result = new AsyncResult(true);
            Executors.RunAsyncNoReturn(() =>
            {
                try
                {
                    if (result.IsCancellationRequested)
                    {
                        result.SetCancelled();
                        return;
                    }

                    action(result);
                    if (!result.IsDone)
                        result.SetResult(null);
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }

        public virtual IProgressResult<TProgress> Execute<TProgress>(Action<IProgressPromise<TProgress>> action)
        {
            ProgressResult<TProgress> result = new ProgressResult<TProgress>(true);
            Executors.RunAsyncNoReturn(() =>
            {
                try
                {
                    if (result.IsCancellationRequested)
                    {
                        result.SetCancelled();
                        return;
                    }

                    action(result);
                    if (!result.IsDone)
                        result.SetResult(null);
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }

        public virtual IAsyncResult<TResult> Execute<TResult>(Action<IPromise<TResult>> action)
        {
            AsyncResult<TResult> result = new AsyncResult<TResult>(true);
            Executors.RunAsyncNoReturn(() =>
            {
                try
                {
                    if (result.IsCancellationRequested)
                    {
                        result.SetCancelled();
                        return;
                    }

                    action(result);
                    if (!result.IsDone)
                        result.SetResult(null);
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }

        public virtual IProgressResult<TProgress, TResult> Execute<TProgress, TResult>(Action<IProgressPromise<TProgress, TResult>> action)
        {
            ProgressResult<TProgress, TResult> result = new ProgressResult<TProgress, TResult>(true);
            Executors.RunAsyncNoReturn(() =>
            {
                try
                {
                    if (result.IsCancellationRequested)
                    {
                        result.SetCancelled();
                        return;
                    }

                    action(result);
                    if (!result.IsDone)
                        result.SetResult(null);
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }
    }
}
