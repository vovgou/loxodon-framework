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
    public interface IPromise
    {
        /// <summary>
        /// The execution result
        /// </summary>
        object Result { get; }

        /// <summary>
        /// Exception
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Returns  "true" if this task finished.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// Returns "true" if this task was cancelled before it completed normally.
        /// </summary>
        bool IsCancelled { get; }

        /// <summary>
        /// Returns  "true" if there is a cancellation request.
        /// </summary>
        bool IsCancellationRequested { get; }

        /// <summary>
        /// 
        /// </summary>
        void SetCancelled();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        void SetException(string error);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void SetException(Exception exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        void SetResult(object result = null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IPromise<TResult> : IPromise
    {
        /// <summary>
        /// The execution result
        /// </summary>
        new TResult Result { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        void SetResult(TResult result);
    }
}
