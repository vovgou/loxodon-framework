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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Loxodon.Framework.Execution;
using UnityEngine;

namespace Loxodon.Framework.Asynchronous
{
    [Flags]
    public enum CoroutineTaskContinuationOptions
    {
        None = 0,
        OnCompleted = 1,
        OnCanceled = 2,
        OnFaulted = 4
    }

    public class CoroutineTask
    {
        private static IEnumerator DoDelay(float secondsDelay)
        {
            yield return new WaitForSecondsRealtime(secondsDelay);
        }

        /// <summary>
        ///  Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="delay">The time span to wait before completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        public static CoroutineTask Delay(TimeSpan delay)
        {
            return Delay((float)delay.TotalSeconds);
        }

        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        public static CoroutineTask Delay(int millisecondsDelay)
        {
            return Delay(millisecondsDelay / 1000.0f);
        }

        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="secondsDelay">The number of seconds to wait before completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        public static CoroutineTask Delay(float secondsDelay)
        {
            return new CoroutineTask(DoDelay(secondsDelay));
        }

        /// <summary>
        /// Create a task to execute on the main thread.
        /// </summary>
        /// <param name="action">The work to execute on the main thread.</param>
        /// <returns>A Task that represents the work queued to execute on the main thread.</returns>
        public static CoroutineTask Run(Action action)
        {
            return new CoroutineTask(action);
        }

        /// <summary>
        /// Create a task to execute on the main thread.
        /// </summary>
        /// <param name="action">The work to execute on the main thread.</param>
        /// <param name="state">The parameter of the work.</param>
        /// <returns>A Task that represents the work queued to execute on the main thread.</returns>
        public static CoroutineTask Run(Action<object> action, object state)
        {
            return new CoroutineTask(action, state);
        }

        /// <summary>
        /// Create a task to execute on the Unity3d's coroutine.
        /// </summary>
        /// <param name="routine">The work to execute on the Unity3d's coroutine.</param>
        /// <returns>A Task that represents the work queued to execute on the Unity3d's coroutine.</returns>
        public static CoroutineTask Run(IEnumerator routine)
        {
            return new CoroutineTask(routine);
        }

        /// <summary>
        /// Create a task to execute on the main thread.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function">The work to execute on the main thread.</param>
        /// <returns>A Task that represents the work queued to execute on the main thread.</returns>
        public static CoroutineTask<TResult> Run<TResult>(Func<TResult> function)
        {
            return new CoroutineTask<TResult>(function);
        }

        /// <summary>
        /// Create a task to execute on the main thread.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function">The work to execute on the main thread.</param>
        /// <param name="state">The parameter of the work.</param>
        /// <returns>A Task that represents the work queued to execute on the main thread.</returns>
        public static CoroutineTask<TResult> Run<TResult>(Func<object, TResult> function, object state)
        {
            return new CoroutineTask<TResult>(function, state);
        }

        /// <summary>
        /// Create a task to execute on the Unity3d's coroutine.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function">The work to execute on the Unity3d's coroutine.</param>
        /// <returns>A Task that represents the work queued to execute on the Unity3d's coroutine.</returns>
        public static CoroutineTask<TResult> Run<TResult>(Func<IPromise<TResult>, IEnumerator> function)
        {
            return new CoroutineTask<TResult>(function);
        }

        /// <summary>
        /// Create a task to execute on the Unity3d's coroutine.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function">The work to execute on the Unity3d's coroutine.</param>
        /// <param name="state">The parameter of the work.</param>
        /// <returns>A Task that represents the work queued to execute on the Unity3d's coroutine.</returns>
        public static CoroutineTask<TResult> Run<TResult>(Func<object, IPromise<TResult>, IEnumerator> function, object state)
        {
            return new CoroutineTask<TResult>(function, state);
        }

        /// <summary>
        ///  Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <remarks>
        /// <para>
        /// If any of the supplied tasks completes in a faulted state, the returned task will also complete in a Faulted state, 
        /// where its exceptions will contain the aggregation of the set of unwrapped exceptions from each of the supplied tasks.
        /// </para>
        /// <para>
        /// If none of the supplied tasks faulted but at least one of them was canceled, the returned task will end in the Canceled state.
        /// </para>
        /// <para>
        /// If none of the tasks faulted and none of the tasks were canceled, the resulting task will end in the Completed state.   
        /// </para>
        /// </remarks>
        public static CoroutineTask WhenAll(params CoroutineTask[] tasks)
        {
            AsyncResult result = new AsyncResult(true);
            try
            {
                if (tasks == null)
                    throw new ArgumentNullException("tasks");

                int count = tasks.Length;
                int curr = count;
                bool isCancelled = false;
                List<Exception> exceptions = new List<Exception>();
                for (int i = 0; i < count; i++)
                {
                    var task = tasks[i];
                    task.asyncResult.Callbackable().OnCallback((ar) =>
                    {
                        isCancelled |= ar.IsCancelled;
                        if (ar.Exception != null)
                            exceptions.Add(ar.Exception);

                        if (Interlocked.Decrement(ref curr) <= 0)
                        {
                            if (isCancelled)
                                result.SetCancelled();
                            else if (exceptions.Count > 0)
                                result.SetException(new AggregateException(exceptions));
                            else
                                result.SetResult();
                        }
                    });
                }
            }
            catch (Exception e)
            {
                result.SetException(e);
            }
            return new CoroutineTask(result);
        }

        /// <summary>
        /// Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <remarks>
        /// <para>
        /// If any of the supplied tasks completes in a faulted state, the returned task will also complete in a Faulted state, 
        /// where its exceptions will contain the aggregation of the set of unwrapped exceptions from each of the supplied tasks.  
        /// </para>
        /// <para>
        /// If none of the supplied tasks faulted but at least one of them was canceled, the returned task will end in the Canceled state.
        /// </para>
        /// <para>
        /// If none of the tasks faulted and none of the tasks were canceled, the resulting task will end in the Completed state.  
        /// The Result of the returned task will be set to an array containing all of the results of the supplied tasks in the 
        /// same order as they were provided. 
        /// </para>
        /// </remarks>
        public static CoroutineTask<TResult[]> WhenAll<TResult>(params CoroutineTask<TResult>[] tasks)
        {
            AsyncResult<TResult[]> result = new AsyncResult<TResult[]>(true);
            try
            {
                if (tasks == null)
                    throw new ArgumentNullException("tasks");

                int count = tasks.Length;
                int curr = count;
                bool isCancelled = false;
                List<Exception> exceptions = new List<Exception>();
                TResult[] array = new TResult[count];
                for (int i = 0; i < count; i++)
                {
                    int index = i;
                    var t = tasks[index];
                    t.asyncResult.Callbackable().OnCallback((ar) =>
                    {
                        try
                        {
                            isCancelled |= ar.IsCancelled;
                            if (ar.Exception != null)
                                exceptions.Add(ar.Exception);
                            else
                                array[index] = (TResult)ar.Result;
                        }
                        finally
                        {
                            if (Interlocked.Decrement(ref curr) <= 0)
                            {
                                if (isCancelled)
                                    result.SetCancelled();
                                else if (exceptions.Count > 0)
                                    result.SetException(new AggregateException(exceptions));
                                else
                                    result.SetResult(array);
                            }
                        }
                    });
                }
            }
            catch (Exception e)
            {
                result.SetException(e);
            }
            return new CoroutineTask<TResult[]>(result);
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of one of the supplied tasks.  The return Task's Result is the task that completed.</returns>
        /// <remarks>
        /// The returned task will complete when any of the supplied tasks has completed.  The returned task will always end in the Completed state 
        /// with its Result set to the first task to complete.  This is true even if the first task to complete ended in the Canceled or Faulted state.
        /// </remarks>
        public static CoroutineTask<CoroutineTask> WhenAny(params CoroutineTask[] tasks)
        {
            AsyncResult<CoroutineTask> result = new AsyncResult<CoroutineTask>(true);
            try
            {
                if (tasks == null)
                    throw new ArgumentNullException("tasks");

                int count = tasks.Length;
                for (int i = 0; i < count; i++)
                {
                    var task = tasks[i];
                    task.asyncResult.Callbackable().OnCallback((ar) =>
                    {
                        if (!result.IsDone)
                            result.SetResult(task);
                    });
                }
            }
            catch (Exception e)
            {
                result.SetException(e);
            }
            return new CoroutineTask<CoroutineTask>(result);
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of one of the supplied tasks.  The return Task's Result is the task that completed.</returns>
        /// <remarks>
        /// The returned task will complete when any of the supplied tasks has completed.  The returned task will always end in the Completed state 
        /// with its Result set to the first task to complete.  This is true even if the first task to complete ended in the Canceled or Faulted state.
        /// </remarks>
        public static CoroutineTask<CoroutineTask<TResult>> WhenAny<TResult>(params CoroutineTask<TResult>[] tasks)
        {
            AsyncResult<CoroutineTask<TResult>> result = new AsyncResult<CoroutineTask<TResult>>(true);
            try
            {
                if (tasks == null)
                    throw new ArgumentNullException("tasks");

                int count = tasks.Length;
                for (int i = 0; i < count; i++)
                {
                    var task = tasks[i];
                    task.asyncResult.Callbackable().OnCallback((ar) =>
                    {
                        if (!result.IsDone)
                            result.SetResult(task);
                    });
                }
            }
            catch (Exception e)
            {
                result.SetException(e);
            }

            return new CoroutineTask<CoroutineTask<TResult>>(result);
        }

        private AsyncResult asyncResult;

        public Exception Exception { get { return asyncResult.Exception; } }

        public bool IsCompleted { get { return asyncResult.IsDone && asyncResult.Exception == null; } }

        public bool IsCancelled { get { return asyncResult.IsDone && asyncResult.IsCancelled; } }

        public bool IsFaulted { get { return asyncResult.IsDone && !asyncResult.IsCancelled && asyncResult.Exception != null; } }

        public bool IsDone { get { return asyncResult.IsDone; } }

        protected internal CoroutineTask(AsyncResult asyncResult)
        {
            this.asyncResult = asyncResult;
        }

        public CoroutineTask(Action action) : this(new AsyncResult())
        {
            Executors.RunOnMainThread(() =>
            {
                try
                {
                    action();
                    asyncResult.SetResult();
                }
                catch (Exception e)
                {
                    asyncResult.SetException(e);
                }
            });
        }

        public CoroutineTask(Action<object> action, object state) : this(new AsyncResult())
        {
            Executors.RunOnMainThread(() =>
            {
                try
                {
                    action(state);
                    asyncResult.SetResult();
                }
                catch (Exception e)
                {
                    asyncResult.SetException(e);
                }
            });
        }

        public CoroutineTask(IEnumerator routine) : this(new AsyncResult(true))
        {
            try
            {
                if (routine == null)
                    throw new ArgumentNullException("routine");

                Executors.RunOnCoroutine(routine, this.asyncResult);
            }
            catch (Exception e)
            {
                this.asyncResult.SetException(e);
            }
        }

        public object WaitForDone()
        {
            return this.asyncResult.WaitForDone();
        }
#if NETFX_CORE || NET_STANDARD_2_0 || NET_4_6
        public virtual IAwaiter<object> GetAwaiter()
        {
            return new AsyncResultAwaiter<AsyncResult>(asyncResult);
        }
#endif

        protected bool IsExecutable(IAsyncResult ar, CoroutineTaskContinuationOptions continuationOptions)
        {
            bool executable = (continuationOptions == CoroutineTaskContinuationOptions.None);
            if (!executable)
                executable = (ar.Exception == null && (continuationOptions & CoroutineTaskContinuationOptions.OnCompleted) > 0);
            if (!executable)
                executable = (ar.IsCancelled && (continuationOptions & CoroutineTaskContinuationOptions.OnCanceled) > 0);
            if (!executable)
                executable = (!ar.IsCancelled && ar.Exception != null && (continuationOptions & CoroutineTaskContinuationOptions.OnFaulted) > 0);
            return executable;
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CoroutineTask"/> completes.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the <see cref="CoroutineTask"/> completes.
        /// </param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        /// as <see cref="CoroutineTaskContinuationOptions.OnCanceled">OnCanceled</see>
        /// , as well as execution options, such as 
        /// <see cref="CoroutineTaskContinuationOptions.OnCompleted">OnCompleted</see>.
        /// </param>
        /// <returns>A new continuation <see cref="CoroutineTask"/>.</returns>
        /// <remarks>
        /// The returned <see cref="CoroutineTask"/> will not be scheduled for execution until the current task has
        /// completed.
        /// </remarks>
        public CoroutineTask ContinueWith(Action continuationAction, CoroutineTaskContinuationOptions continuationOptions = CoroutineTaskContinuationOptions.None)
        {
            AsyncResult result = new AsyncResult(true);
            this.asyncResult.Callbackable().OnCallback(ar =>
            {
                try
                {
                    bool executable = IsExecutable(ar, continuationOptions);
                    if (!executable)
                    {
                        result.SetCancelled();
                        return;
                    }

                    continuationAction();
                    result.SetResult();
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return new CoroutineTask(result);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CoroutineTask"/> completes.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the <see cref="CoroutineTask"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        /// as <see cref="CoroutineTaskContinuationOptions.OnCanceled">OnCanceled</see>
        /// , as well as execution options, such as 
        /// <see cref="CoroutineTaskContinuationOptions.OnCompleted">OnCompleted</see>.
        /// </param>
        /// <returns>A new continuation <see cref="CoroutineTask"/>.</returns>
        /// <remarks>
        /// The returned <see cref="CoroutineTask"/> will not be scheduled for execution until the current task has
        /// completed.
        /// </remarks>
        public CoroutineTask ContinueWith(Action<CoroutineTask> continuationAction, CoroutineTaskContinuationOptions continuationOptions = CoroutineTaskContinuationOptions.None)
        {
            AsyncResult result = new AsyncResult(true);
            this.asyncResult.Callbackable().OnCallback(ar =>
            {
                try
                {
                    bool executable = IsExecutable(ar, continuationOptions);
                    if (!executable)
                    {
                        result.SetCancelled();
                        return;
                    }

                    continuationAction(this);
                    result.SetResult();
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return new CoroutineTask(result);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CoroutineTask"/> completes.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the <see cref="CoroutineTask"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="state">The parameter of the action.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        /// as <see cref="CoroutineTaskContinuationOptions.OnCanceled">OnCanceled</see>
        /// , as well as execution options, such as 
        /// <see cref="CoroutineTaskContinuationOptions.OnCompleted">OnCompleted</see>.
        /// </param>
        /// <returns>A new continuation <see cref="CoroutineTask"/>.</returns>
        /// <remarks>
        /// The returned <see cref="CoroutineTask"/> will not be scheduled for execution until the current task has
        /// completed.
        /// </remarks>
        public CoroutineTask ContinueWith(Action<CoroutineTask, object> continuationAction, object state, CoroutineTaskContinuationOptions continuationOptions = CoroutineTaskContinuationOptions.None)
        {
            AsyncResult result = new AsyncResult(true);
            this.asyncResult.Callbackable().OnCallback(ar =>
            {
                try
                {
                    bool executable = IsExecutable(ar, continuationOptions);
                    if (!executable)
                    {
                        result.SetCancelled();
                        return;
                    }

                    continuationAction(this, state);
                    result.SetResult();
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return new CoroutineTask(result);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CoroutineTask"/> completes.
        /// </summary>
        /// <param name="continuationRoutine">
        /// An action to run when the <see cref="CoroutineTask"/> completes.
        /// </param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        /// as <see cref="CoroutineTaskContinuationOptions.OnCanceled">OnCanceled</see>
        /// , as well as execution options, such as 
        /// <see cref="CoroutineTaskContinuationOptions.OnCompleted">OnCompleted</see>.
        /// </param>
        /// <returns>A new continuation <see cref="CoroutineTask"/>.</returns>
        /// <remarks>
        /// The returned <see cref="CoroutineTask"/> will not be scheduled for execution until the current task has
        /// completed.
        /// </remarks>
        public CoroutineTask ContinueWith(IEnumerator continuationRoutine, CoroutineTaskContinuationOptions continuationOptions = CoroutineTaskContinuationOptions.None)
        {
            AsyncResult result = new AsyncResult(true);
            this.asyncResult.Callbackable().OnCallback(ar =>
            {
                try
                {
                    bool executable = IsExecutable(ar, continuationOptions);
                    if (!executable)
                    {
                        result.SetCancelled();
                        return;
                    }

                    Executors.RunOnCoroutine(continuationRoutine, result);
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return new CoroutineTask(result);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CoroutineTask"/> completes.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the <see cref="CoroutineTask"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        /// as <see cref="CoroutineTaskContinuationOptions.OnCanceled">OnCanceled</see>
        /// , as well as execution options, such as 
        /// <see cref="CoroutineTaskContinuationOptions.OnCompleted">OnCompleted</see>.
        /// </param>
        /// <returns>A new continuation <see cref="CoroutineTask"/>.</returns>
        /// <remarks>
        /// The returned <see cref="CoroutineTask"/> will not be scheduled for execution until the current task has
        /// completed.
        /// </remarks>
        public CoroutineTask ContinueWith(Func<CoroutineTask, IEnumerator> continuationFunction, CoroutineTaskContinuationOptions continuationOptions = CoroutineTaskContinuationOptions.None)
        {
            AsyncResult result = new AsyncResult(true);
            this.asyncResult.Callbackable().OnCallback(ar =>
            {
                try
                {
                    bool executable = IsExecutable(ar, continuationOptions);
                    if (!executable)
                    {
                        result.SetCancelled();
                        return;
                    }

                    Executors.RunOnCoroutine(continuationFunction(this), result);
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return new CoroutineTask(result);
        }


        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CoroutineTask"/> completes.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the <see cref="CoroutineTask"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="state">The parameter of the action.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        /// as <see cref="CoroutineTaskContinuationOptions.OnCanceled">OnCanceled</see>
        /// , as well as execution options, such as 
        /// <see cref="CoroutineTaskContinuationOptions.OnCompleted">OnCompleted</see>.
        /// </param>
        /// <returns>A new continuation <see cref="CoroutineTask"/>.</returns>
        /// <remarks>
        /// The returned <see cref="CoroutineTask"/> will not be scheduled for execution until the current task has
        /// completed.
        /// </remarks>
        public CoroutineTask ContinueWith(Func<CoroutineTask, object, IEnumerator> continuationFunction, object state, CoroutineTaskContinuationOptions continuationOptions = CoroutineTaskContinuationOptions.None)
        {
            AsyncResult result = new AsyncResult(true);
            this.asyncResult.Callbackable().OnCallback(ar =>
            {
                try
                {
                    bool executable = IsExecutable(ar, continuationOptions);
                    if (!executable)
                    {
                        result.SetCancelled();
                        return;
                    }

                    Executors.RunOnCoroutine(continuationFunction(this, state), result);
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return new CoroutineTask(result);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CoroutineTask"/> completes.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the <see cref="CoroutineTask"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        /// as <see cref="CoroutineTaskContinuationOptions.OnCanceled">OnCanceled</see>
        /// , as well as execution options, such as 
        /// <see cref="CoroutineTaskContinuationOptions.OnCompleted">OnCompleted</see>.
        /// </param>
        /// <returns>A new continuation <see cref="CoroutineTask"/>.</returns>
        /// <remarks>
        /// The returned <see cref="CoroutineTask"/> will not be scheduled for execution until the current task has
        /// completed.
        /// </remarks>
        public CoroutineTask<TResult> ContinueWith<TResult>(Func<CoroutineTask, TResult> continuationFunction, CoroutineTaskContinuationOptions continuationOptions = CoroutineTaskContinuationOptions.None)
        {
            AsyncResult<TResult> result = new AsyncResult<TResult>(true);
            this.asyncResult.Callbackable().OnCallback(ar =>
            {
                try
                {
                    bool executable = IsExecutable(ar, continuationOptions);
                    if (!executable)
                    {
                        result.SetCancelled();
                        return;
                    }

                    TResult value = continuationFunction(this);
                    result.SetResult(value);
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return new CoroutineTask<TResult>(result);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CoroutineTask"/> completes.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the <see cref="CoroutineTask"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="state">The parameter of the action.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        /// as <see cref="CoroutineTaskContinuationOptions.OnCanceled">OnCanceled</see>
        /// , as well as execution options, such as 
        /// <see cref="CoroutineTaskContinuationOptions.OnCompleted">OnCompleted</see>.
        /// </param>
        /// <returns>A new continuation <see cref="CoroutineTask"/>.</returns>
        /// <remarks>
        /// The returned <see cref="CoroutineTask"/> will not be scheduled for execution until the current task has
        /// completed.
        /// </remarks>
        public CoroutineTask<TResult> ContinueWith<TResult>(Func<CoroutineTask, object, TResult> continuationFunction, object state, CoroutineTaskContinuationOptions continuationOptions = CoroutineTaskContinuationOptions.None)
        {
            AsyncResult<TResult> result = new AsyncResult<TResult>(true);
            this.asyncResult.Callbackable().OnCallback(ar =>
            {
                try
                {
                    bool executable = IsExecutable(ar, continuationOptions);
                    if (!executable)
                    {
                        result.SetCancelled();
                        return;
                    }

                    TResult value = continuationFunction(this, state);
                    result.SetResult(value);
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return new CoroutineTask<TResult>(result);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CoroutineTask"/> completes.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the <see cref="CoroutineTask"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        /// as <see cref="CoroutineTaskContinuationOptions.OnCanceled">OnCanceled</see>
        /// , as well as execution options, such as 
        /// <see cref="CoroutineTaskContinuationOptions.OnCompleted">OnCompleted</see>.
        /// </param>
        /// <returns>A new continuation <see cref="CoroutineTask"/>.</returns>
        /// <remarks>
        /// The returned <see cref="CoroutineTask"/> will not be scheduled for execution until the current task has
        /// completed.
        /// </remarks>
        public CoroutineTask<TResult> ContinueWith<TResult>(Func<CoroutineTask, IPromise<TResult>, IEnumerator> continuationFunction, CoroutineTaskContinuationOptions continuationOptions = CoroutineTaskContinuationOptions.None)
        {
            AsyncResult<TResult> result = new AsyncResult<TResult>(true);
            this.asyncResult.Callbackable().OnCallback(ar =>
            {
                try
                {
                    bool executable = IsExecutable(ar, continuationOptions);
                    if (!executable)
                    {
                        result.SetCancelled();
                        return;
                    }

                    Executors.RunOnCoroutine(continuationFunction(this, result), result);
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return new CoroutineTask<TResult>(result);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CoroutineTask"/> completes.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the <see cref="CoroutineTask"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="state">The parameter of the action.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        /// as <see cref="CoroutineTaskContinuationOptions.OnCanceled">OnCanceled</see>
        /// , as well as execution options, such as 
        /// <see cref="CoroutineTaskContinuationOptions.OnCompleted">OnCompleted</see>.
        /// </param>
        /// <returns>A new continuation <see cref="CoroutineTask"/>.</returns>
        /// <remarks>
        /// The returned <see cref="CoroutineTask"/> will not be scheduled for execution until the current task has
        /// completed.
        /// </remarks>
        public CoroutineTask<TResult> ContinueWith<TResult>(Func<CoroutineTask, object, IPromise<TResult>, IEnumerator> continuationFunction, object state, CoroutineTaskContinuationOptions continuationOptions = CoroutineTaskContinuationOptions.None)
        {
            AsyncResult<TResult> result = new AsyncResult<TResult>(true);
            this.asyncResult.Callbackable().OnCallback(ar =>
            {
                try
                {
                    bool executable = IsExecutable(ar, continuationOptions);
                    if (!executable)
                    {
                        result.SetCancelled();
                        return;
                    }

                    Executors.RunOnCoroutine(continuationFunction(this, state, result), result);
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return new CoroutineTask<TResult>(result);
        }

    }

    public class CoroutineTask<TResult> : CoroutineTask
    {
        private AsyncResult<TResult> asyncResult;

        public TResult Result { get { return (TResult)this.asyncResult.Result; } }

        protected internal CoroutineTask(AsyncResult<TResult> asyncResult) : base(asyncResult)
        {
            this.asyncResult = asyncResult;
        }

        public CoroutineTask(Func<TResult> function) : this(new AsyncResult<TResult>())
        {
            Executors.RunOnMainThread(() =>
            {
                try
                {
                    TResult value = function();
                    this.asyncResult.SetResult(value);
                }
                catch (Exception e)
                {
                    this.asyncResult.SetException(e);
                }
            });
        }

        public CoroutineTask(Func<object, TResult> function, object state) : this(new AsyncResult<TResult>())
        {
            Executors.RunOnMainThread(() =>
            {
                try
                {
                    TResult value = function(state);
                    this.asyncResult.SetResult(value);
                }
                catch (Exception e)
                {
                    this.asyncResult.SetException(e);
                }
            });
        }

        public CoroutineTask(Func<IPromise<TResult>, IEnumerator> function) : this(new AsyncResult<TResult>(true))
        {
            try
            {
                Executors.RunOnCoroutine(function(this.asyncResult), this.asyncResult);
            }
            catch (Exception e)
            {
                this.asyncResult.SetException(e);
            }
        }

        public CoroutineTask(Func<object, IPromise<TResult>, IEnumerator> function, object state) : this(new AsyncResult<TResult>(true))
        {
            try
            {
                Executors.RunOnCoroutine(function(state, this.asyncResult), this.asyncResult);
            }
            catch (Exception e)
            {
                this.asyncResult.SetException(e);
            }
        }
#if NETFX_CORE || NET_STANDARD_2_0 || NET_4_6
        public new IAwaiter<TResult> GetAwaiter()
        {
            return new AsyncResultAwaiter<AsyncResult<TResult>, TResult>(this.asyncResult);
        }
#endif
    }
}
