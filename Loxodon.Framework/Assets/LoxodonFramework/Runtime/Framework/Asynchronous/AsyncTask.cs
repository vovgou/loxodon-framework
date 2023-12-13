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
using System.Threading;
using System.Collections;

#if NETFX_CORE
using System.Threading.Tasks;
#endif

using Loxodon.Log;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Asynchronous
{
    [Obsolete("This type will be removed in version 3.0")]
    public class AsyncTask : IAsyncTask
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AsyncTask));

        private Action action;

        private Action preCallbackOnMainThread;
        private Action preCallbackOnWorkerThread;

        private Action postCallbackOnMainThread;
        private Action postCallbackOnWorkerThread;

        private Action<Exception> errorCallbackOnMainThread;
        private Action<Exception> errorCallbackOnWorkerThread;

        private Action finishCallbackOnMainThread;
        private Action finishCallbackOnWorkerThread;

        private int running = 0;
        private AsyncResult result;

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <param name="runOnMainThread"></param>
        public AsyncTask(Action task, bool runOnMainThread = false)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            this.result = new AsyncResult();
            if (runOnMainThread)
            {
                this.action = WrapAction(() =>
                {
                    Executors.RunOnMainThread(task, true);
                    this.result.SetResult();
                });
            }
            else {
                this.action = WrapAction(() =>
                {
                    task();
                    this.result.SetResult();
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="runOnMainThread"></param>
        public AsyncTask(Action<IPromise> task, bool runOnMainThread = false, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            this.result = new AsyncResult(!runOnMainThread && cancelable);
            if (runOnMainThread)
            {
                this.action = WrapAction(() =>
                {
                    Executors.RunOnMainThread(() => task(result), true);
                    this.result.Synchronized().WaitForResult();
                });
            }
            else {
                this.action = WrapAction(() =>
                {
                    task(result);
                    this.result.Synchronized().WaitForResult();
                });
            }
        }

        /// <summary>
        /// run on main thread
        /// </summary>
        /// <param name="task"></param>
        public AsyncTask(IEnumerator task, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            this.result = new AsyncResult(cancelable);
            this.action = WrapAction(() =>
            {
                Executors.RunOnCoroutine(task, result);
                this.result.Synchronized().WaitForResult();
            });
        }

        public virtual object Result
        {
            get { return this.result.Result; }
        }

        public virtual Exception Exception
        {
            get { return this.result.Exception; }
        }

        public virtual bool IsDone
        {
            get { return this.result.IsDone; }
        }

        public virtual bool IsCancelled
        {
            get { return this.result.IsCancelled; }
        }

        protected virtual Action WrapAction(Action action)
        {
            Action wrapAction = () =>
            {
                try
                {
                    try
                    {
                        if (preCallbackOnWorkerThread != null)
                            preCallbackOnWorkerThread();
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("{0}", e);
                    }

                    if (this.result.IsCancellationRequested)
                    {
                        this.result.SetCancelled();
                        return;
                    }

                    action();
                }
                catch (Exception e)
                {
                    this.result.SetException(e);
                }
                finally
                {
                    try
                    {
                        if (this.Exception != null)
                        {
                            if (this.errorCallbackOnMainThread != null)
                                Executors.RunOnMainThread(() => this.errorCallbackOnMainThread(this.Exception), true);

                            if (this.errorCallbackOnWorkerThread != null)
                                this.errorCallbackOnWorkerThread(this.Exception);
                        }
                        else
                        {
                            if (this.postCallbackOnMainThread != null)
                                Executors.RunOnMainThread(this.postCallbackOnMainThread, true);

                            if (this.postCallbackOnWorkerThread != null)
                                this.postCallbackOnWorkerThread();
                        }
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("{0}", e);
                    }

                    try
                    {
                        if (this.finishCallbackOnMainThread != null)
                            Executors.RunOnMainThread(this.finishCallbackOnMainThread, true);

                        if (this.finishCallbackOnWorkerThread != null)
                            this.finishCallbackOnWorkerThread();
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("{0}", e);
                    }

                    Interlocked.Exchange(ref this.running, 0);
                }
            };
            return wrapAction;
        }

        public virtual bool Cancel()
        {
            return this.result.Cancel();
        }

        public virtual ICallbackable Callbackable()
        {
            return result.Callbackable();
        }

        public virtual ISynchronizable Synchronized()
        {
            return result.Synchronized();
        }

        public virtual object WaitForDone()
        {
            return Executors.WaitWhile(() => !IsDone);
        }

        public IAsyncTask OnPreExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.preCallbackOnMainThread += callback;
            else
                this.preCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask OnPostExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.postCallbackOnMainThread += callback;
            else
                this.postCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask OnError(Action<Exception> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.errorCallbackOnMainThread += callback;
            else
                this.errorCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask OnFinish(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.finishCallbackOnMainThread += callback;
            else
                this.finishCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask Start(int delay)
        {
            if (delay <= 0)
                return this.Start();

            Executors.RunAsyncNoReturn(() =>
            {
#if NETFX_CORE
                Task.Delay(delay).Wait();
#else
                Thread.Sleep(delay);
#endif
                if (this.IsDone || this.running == 1)
                    return;

                this.Start();
            });
            return this;
        }

        public IAsyncTask Start()
        {
            if (this.IsDone)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The task has been done!");

                return this;
            }

            if (Interlocked.CompareExchange(ref this.running, 1, 0) == 1)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The task is running!");

                return this;
            }

            try
            {
                if (this.preCallbackOnMainThread != null)
                    Executors.RunOnMainThread(this.preCallbackOnMainThread, true);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }

            Executors.RunAsync(this.action);

            return this;
        }
    }

    [Obsolete("This type will be removed in version 3.0")]
    public class AsyncTask<TResult> : IAsyncTask<TResult>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AsyncTask<TResult>));

        private Action action;

        private Action preCallbackOnMainThread;
        private Action preCallbackOnWorkerThread;

        private Action<TResult> postCallbackOnMainThread;
        private Action<TResult> postCallbackOnWorkerThread;

        private Action<Exception> errorCallbackOnMainThread;
        private Action<Exception> errorCallbackOnWorkerThread;

        private Action finishCallbackOnMainThread;
        private Action finishCallbackOnWorkerThread;

        private int running = 0;
        private AsyncResult<TResult> result;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="runOnMainThread"></param>
        public AsyncTask(Func<TResult> task, bool runOnMainThread = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this.result = new AsyncResult<TResult>();

            if (runOnMainThread)
            {
                this.action = WrapAction(() =>
                {
                    return Executors.RunOnMainThread(task);
                });
            }
            else {
                this.action = WrapAction(() => task());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="runOnMainThread"></param>
        public AsyncTask(Action<IPromise<TResult>> task, bool runOnMainThread = false, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this.result = new AsyncResult<TResult>(!runOnMainThread && cancelable);

            if (runOnMainThread)
            {
                this.action = WrapAction(() =>
                {
                    Executors.RunOnMainThread(() => task(this.result));
                    return this.result.Synchronized().WaitForResult();
                });
            }
            else {
                this.action = WrapAction(() =>
                {
                    task(this.result);
                    return this.result.Synchronized().WaitForResult();
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        public AsyncTask(Func<IPromise<TResult>, IEnumerator> task, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this.result = new AsyncResult<TResult>(cancelable);
            this.action = WrapAction(() =>
            {
                Executors.RunOnCoroutine(task(this.result), this.result);
                return this.result.Synchronized().WaitForResult();
            });
        }

        public virtual TResult Result
        {
            get { return this.result.Result; }
        }

        object Asynchronous.IAsyncResult.Result
        {
            get { return this.result.Result; }
        }

        public virtual Exception Exception
        {
            get { return this.result.Exception; }
        }

        public virtual bool IsDone
        {
            get { return this.result.IsDone; }
        }

        public virtual bool IsCancelled
        {
            get { return this.result.IsCancelled; }
        }

        protected virtual Action WrapAction(Func<TResult> action)
        {
            Action wrapAction = () =>
            {
                try
                {
                    try
                    {
                        if (this.preCallbackOnWorkerThread != null)
                            this.preCallbackOnWorkerThread();
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("{0}", e);
                    }

                    if (this.result.IsCancellationRequested)
                    {
                        this.result.SetCancelled();
                        return;
                    }

                    TResult obj = action();
                    this.result.SetResult(obj);
                }
                catch (Exception e)
                {
                    this.result.SetException(e);
                }
                finally
                {
                    try
                    {
                        if (this.Exception != null)
                        {
                            if (this.errorCallbackOnMainThread != null)
                                Executors.RunOnMainThread(() => this.errorCallbackOnMainThread(this.Exception), true);

                            if (this.errorCallbackOnWorkerThread != null)
                                this.errorCallbackOnWorkerThread(this.Exception);

                        }
                        else
                        {
                            if (this.postCallbackOnMainThread != null)
                                Executors.RunOnMainThread(() => this.postCallbackOnMainThread(this.Result), true);

                            if (this.postCallbackOnWorkerThread != null)
                                this.postCallbackOnWorkerThread(this.Result);
                        }
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("{0}", e);
                    }

                    try
                    {
                        if (this.finishCallbackOnMainThread != null)
                            Executors.RunOnMainThread(this.finishCallbackOnMainThread, true);

                        if (this.finishCallbackOnWorkerThread != null)
                            this.finishCallbackOnWorkerThread();
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("{0}", e);
                    }

                    Interlocked.Exchange(ref this.running, 0);
                }
            };
            return wrapAction;
        }

        public virtual bool Cancel()
        {
            return this.result.Cancel();
        }

        public virtual ICallbackable<TResult> Callbackable()
        {
            return result.Callbackable();
        }

        public virtual ISynchronizable<TResult> Synchronized()
        {
            return result.Synchronized();
        }

        ICallbackable IAsyncResult.Callbackable()
        {
            return (result as IAsyncResult).Callbackable();
        }

        ISynchronizable IAsyncResult.Synchronized()
        {
            return (result as IAsyncResult).Synchronized();
        }

        public virtual object WaitForDone()
        {
            return Executors.WaitWhile(() => !IsDone);
        }

        public IAsyncTask<TResult> OnPreExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.preCallbackOnMainThread += callback;
            else
                this.preCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask<TResult> OnPostExecute(Action<TResult> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.postCallbackOnMainThread += callback;
            else
                this.postCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask<TResult> OnError(Action<Exception> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.errorCallbackOnMainThread += callback;
            else
                this.errorCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask<TResult> OnFinish(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.finishCallbackOnMainThread += callback;
            else
                this.finishCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask<TResult> Start(int delay)
        {
            if (delay <= 0)
                return this.Start();

            Executors.RunAsyncNoReturn(() =>
            {
#if NETFX_CORE
                Task.Delay(delay).Wait();
#else
                Thread.Sleep(delay);
#endif
                if (this.IsDone || this.running == 1)
                    return;

                this.Start();
            });

            return this;
        }

        public IAsyncTask<TResult> Start()
        {
            if (this.IsDone)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The task has been done!");
                return this;
            }

            if (Interlocked.CompareExchange(ref this.running, 1, 0) == 1)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The task is running!");
                return this;
            }

            try
            {
                if (this.preCallbackOnMainThread != null)
                    Executors.RunOnMainThread(this.preCallbackOnMainThread, true);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }

            Executors.RunAsync(this.action);

            return this;
        }
    }
}
