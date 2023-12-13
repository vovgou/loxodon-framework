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
    public class ProgressTask<TProgress> : IProgressTask<TProgress>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProgressTask<TProgress>));

        private Action action;

        private Action preCallbackOnMainThread;
        private Action preCallbackOnWorkerThread;

        private Action postCallbackOnMainThread;
        private Action postCallbackOnWorkerThread;

        private Action<TProgress> progressCallbackOnMainThread;
        private Action<TProgress> progressCallbackOnWorkerThread;

        private Action<Exception> errorCallbackOnMainThread;
        private Action<Exception> errorCallbackOnWorkerThread;

        private Action finishCallbackOnMainThread;
        private Action finishCallbackOnWorkerThread;

        private int running = 0;
        private ProgressResult<TProgress> result;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="runOnMainThread"></param>
        /// <param name="cancelable"></param>
        public ProgressTask(Action<IProgressPromise<TProgress>> task, bool runOnMainThread = false, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this.result = new ProgressResult<TProgress>(!runOnMainThread && cancelable);
            this.result.Callbackable().OnProgressCallback(OnProgressChanged);
            if (runOnMainThread)
            {
                this.action = WrapAction(() =>
                {
                    Executors.RunOnMainThread(() => task(result), true);
                    result.Synchronized().WaitForResult();
                });
            }
            else
            {
                this.action = WrapAction(() =>
                {
                    task(result);
                    result.Synchronized().WaitForResult();
                });
            }
        }

        /// <summary>
        /// run on main thread.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="cancelable"></param>
        public ProgressTask(Func<IProgressPromise<TProgress>, IEnumerator> task, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this.result = new ProgressResult<TProgress>(cancelable);
            this.result.Callbackable().OnProgressCallback(OnProgressChanged);
            this.action = WrapAction(() =>
            {
                Executors.RunOnCoroutine(task(this.result), this.result);
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

        public virtual TProgress Progress
        {
            get { return this.result.Progress; }
        }

        protected virtual Action WrapAction(Action action)
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

        protected virtual IEnumerator DoUpdateProgressOnMainThread()
        {
            while (!result.IsDone)
            {
                try
                {
                    if (this.progressCallbackOnMainThread != null)
                        this.progressCallbackOnMainThread(this.result.Progress);
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("{0}", e);
                }
                yield return null;
            }
        }

        protected virtual void OnProgressChanged(TProgress progress)
        {
            try
            {
                if (this.result.IsDone || this.progressCallbackOnWorkerThread == null)
                    return;

                this.progressCallbackOnWorkerThread(progress);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
        }

        public virtual bool Cancel()
        {
            return this.result.Cancel();
        }

        public virtual IProgressCallbackable<TProgress> Callbackable()
        {
            return result.Callbackable();
        }

        ICallbackable IAsyncResult.Callbackable()
        {
            return (result as IAsyncResult).Callbackable();
        }

        public virtual ISynchronizable Synchronized()
        {
            return result.Synchronized();
        }

        public virtual object WaitForDone()
        {
            return Executors.WaitWhile(() => !IsDone);
        }

        public IProgressTask<TProgress> OnPreExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.preCallbackOnMainThread += callback;
            else
                this.preCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress> OnPostExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.postCallbackOnMainThread += callback;
            else
                this.postCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress> OnError(Action<Exception> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.errorCallbackOnMainThread += callback;
            else
                this.errorCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress> OnProgressUpdate(Action<TProgress> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.progressCallbackOnMainThread += callback;
            else
                this.progressCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress> OnFinish(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.finishCallbackOnMainThread += callback;
            else
                this.finishCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress> Start(int delay)
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

        public IProgressTask<TProgress> Start()
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

                if (this.progressCallbackOnMainThread != null)
                    Executors.RunOnCoroutineNoReturn(DoUpdateProgressOnMainThread());
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
    public class ProgressTask<TProgress, TResult> : IProgressTask<TProgress, TResult>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProgressTask<TProgress, TResult>));

        private Action action;

        private Action preCallbackOnMainThread;
        private Action preCallbackOnWorkerThread;

        private Action<TResult> postCallbackOnMainThread;
        private Action<TResult> postCallbackOnWorkerThread;

        private Action<TProgress> progressCallbackOnMainThread;
        private Action<TProgress> progressCallbackOnWorkerThread;

        private Action<Exception> errorCallbackOnMainThread;
        private Action<Exception> errorCallbackOnWorkerThread;

        private Action finishCallbackOnMainThread;
        private Action finishCallbackOnWorkerThread;

        private int running = 0;
        private ProgressResult<TProgress, TResult> result;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="runOnMainThread"></param>
        /// <param name="cancelable"></param>
        public ProgressTask(Action<IProgressPromise<TProgress, TResult>> task, bool runOnMainThread, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this.result = new ProgressResult<TProgress, TResult>(!runOnMainThread && cancelable);
            this.result.Callbackable().OnProgressCallback(OnProgressChanged);

            if (runOnMainThread)
            {
                this.action = WrapAction(() =>
                {
                    Executors.RunOnMainThread(() => task(result), true);
                    return this.result.Synchronized().WaitForResult();
                });
            }
            else
            {
                this.action = WrapAction(() =>
                {
                    task(result);
                    return this.result.Synchronized().WaitForResult();
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="cancelable"></param>
        public ProgressTask(Func<IProgressPromise<TProgress, TResult>, IEnumerator> task, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this.result = new ProgressResult<TProgress, TResult>(cancelable);
            this.result.Callbackable().OnProgressCallback(OnProgressChanged);
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

        public virtual TProgress Progress
        {
            get { return this.result.Progress; }
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

        protected virtual IEnumerator DoUpdateProgressOnMainThread()
        {
            while (!result.IsDone)
            {
                try
                {
                    if (this.progressCallbackOnMainThread != null)
                        this.progressCallbackOnMainThread(this.result.Progress);
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("{0}", e);
                }

                yield return null;
            }
        }

        protected virtual void OnProgressChanged(TProgress progress)
        {
            try
            {
                if (this.result.IsDone || this.progressCallbackOnWorkerThread == null)
                    return;

                this.progressCallbackOnWorkerThread(progress);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
        }

        public virtual bool Cancel()
        {
            return this.result.Cancel();
        }

        public virtual IProgressCallbackable<TProgress, TResult> Callbackable()
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

        ICallbackable<TResult> IAsyncResult<TResult>.Callbackable()
        {
            return (result as IAsyncResult<TResult>).Callbackable();
        }

        IProgressCallbackable<TProgress> IProgressResult<TProgress>.Callbackable()
        {
            return (result as IProgressResult<TProgress>).Callbackable();
        }

        ISynchronizable IAsyncResult.Synchronized()
        {
            return (result as IAsyncResult).Synchronized();
        }

        public virtual object WaitForDone()
        {
            return Executors.WaitWhile(() => !IsDone);
        }

        public IProgressTask<TProgress, TResult> OnPreExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.preCallbackOnMainThread += callback;
            else
                this.preCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress, TResult> OnPostExecute(Action<TResult> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.postCallbackOnMainThread += callback;
            else
                this.postCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress, TResult> OnError(Action<Exception> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.errorCallbackOnMainThread += callback;
            else
                this.errorCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress, TResult> OnProgressUpdate(Action<TProgress> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.progressCallbackOnMainThread += callback;
            else
                this.progressCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress, TResult> OnFinish(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.finishCallbackOnMainThread += callback;
            else
                this.finishCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress, TResult> Start(int delay)
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

        public IProgressTask<TProgress, TResult> Start()
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

                if (this.progressCallbackOnMainThread != null)
                    Executors.RunOnCoroutineNoReturn(DoUpdateProgressOnMainThread());
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
