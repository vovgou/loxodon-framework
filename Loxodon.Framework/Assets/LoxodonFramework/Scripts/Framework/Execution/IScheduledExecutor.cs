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
    public interface IScheduledExecutor:IDisposable
    {
        /// <summary>
        /// Start the service.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the service.
        /// </summary>
        void Stop();

        /// <summary>
        /// Creates and executes a task that becomes enabled after the given delay.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        IAsyncResult<TResult> Schedule<TResult>(Func<TResult> command, long delay);

        /// <summary>
        /// Creates and executes a task that becomes enabled after the given delay.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        IAsyncResult<TResult> Schedule<TResult>(Func<TResult> command, TimeSpan delay);

        /// <summary>
        /// Creates and executes a one-shot action that becomes enabled after the given delay.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        Asynchronous.IAsyncResult Schedule(Action command, long delay);

        /// <summary>
        /// Creates and executes a one-shot action that becomes enabled after the given delay.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        Asynchronous.IAsyncResult Schedule(Action command, TimeSpan delay);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given period; that is executions will commence after initialDelay then initialDelay+period, then initialDelay + 2 * period, and so on.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="initialDelay">millisecond</param>
        /// <param name="period">millisecond</param>
        /// <returns></returns>
        Asynchronous.IAsyncResult ScheduleAtFixedRate(Action command, long initialDelay, long period);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given period; that is executions will commence after initialDelay then initialDelay+period, then initialDelay + 2 * period, and so on.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="initialDelay"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        Asynchronous.IAsyncResult ScheduleAtFixedRate(Action command, TimeSpan initialDelay, TimeSpan period);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given delay between the termination of one execution and the commencement of the next.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="initialDelay">millisecond</param>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        Asynchronous.IAsyncResult ScheduleWithFixedDelay(Action command, long initialDelay, long delay);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given delay between the termination of one execution and the commencement of the next.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="initialDelay"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        Asynchronous.IAsyncResult ScheduleWithFixedDelay(Action command, TimeSpan initialDelay, TimeSpan delay);
    }
}
