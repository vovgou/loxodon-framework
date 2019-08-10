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
