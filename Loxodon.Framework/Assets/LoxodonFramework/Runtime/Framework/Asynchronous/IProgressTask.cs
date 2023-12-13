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

namespace Loxodon.Framework.Asynchronous
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProgress"></typeparam>
    [Obsolete("This type will be removed in version 3.0")]
    public interface IProgressTask<TProgress> : IProgressResult<TProgress>
    {
        /// <summary>
        /// Triggered when a task starts.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress> OnPreExecute(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when the task is completed.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress> OnPostExecute(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when an error occurs.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress> OnError(Action<Exception> callback, bool runOnMainThread = true);

        /// <summary>
        /// Always call the end of the task.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress> OnFinish(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Update the progress.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress> OnProgressUpdate(Action<TProgress> callback, bool runOnMainThread = true);

        /// <summary>
        /// Start the task after the given delay.
        /// </summary>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        IProgressTask<TProgress> Start(int delay);

        /// <summary>
        /// Start the task.
        /// </summary>
        /// <returns></returns>
        IProgressTask<TProgress> Start();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProgress"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    [Obsolete("This type will be removed in version 3.0")]
    public interface IProgressTask<TProgress, TResult> : IProgressResult<TProgress, TResult>
    {
        /// <summary>
        /// Triggered when a task starts.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> OnPreExecute(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when the task is completed.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> OnPostExecute(Action<TResult> callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when an error occurs.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> OnError(Action<Exception> callback, bool runOnMainThread = true);

        /// <summary>
        /// Always call the end of the task.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> OnFinish(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Update the progress.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> OnProgressUpdate(Action<TProgress> callback, bool runOnMainThread = true);

        /// <summary>
        /// Start the task after the given delay.
        /// </summary>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> Start(int delay);

        /// <summary>
        /// Start the task.
        /// </summary>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> Start();
    }
}
