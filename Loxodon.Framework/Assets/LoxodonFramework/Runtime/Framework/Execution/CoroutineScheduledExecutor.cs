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

using Loxodon.Framework.Asynchronous;
using Loxodon.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Loxodon.Framework.Execution
{
    public interface ITime
    {
        float Time { get; }
    }

    public class CoroutineScheduledExecutor : AbstractExecutor, IScheduledExecutor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CoroutineScheduledExecutor));

        private ComparerImpl<IDelayTask> comparer = new ComparerImpl<IDelayTask>();
        private List<IDelayTask> queue = new List<IDelayTask>();
        private bool running = false;
        public CoroutineScheduledExecutor() : this(false)
        {
        }

        public CoroutineScheduledExecutor(bool timeScaled)
        {
            Time = timeScaled ? new ScaledTime() : (ITime)new UnscaledTime();
        }

        public CoroutineScheduledExecutor(ITime time)
        {
            Time = time != null ? time : new UnscaledTime();
        }

        internal ITime Time { get; private set; }

        private void Add(IDelayTask task)
        {
            queue.Add(task);
            queue.Sort(comparer);
        }

        private bool Remove(IDelayTask task)
        {
            if (queue.Remove(task))
            {
                queue.Sort(comparer);
                return true;
            }
            return false;
        }

        public void Start()
        {
            if (this.running)
                return;

            this.running = true;

            InterceptableEnumerator ie = InterceptableEnumerator.Create(DoStart());
            ie.RegisterCatchBlock(e => { this.running = false; });
            Executors.RunOnCoroutineNoReturn(ie);
        }

        protected virtual IEnumerator DoStart()
        {
            while (running)
            {
                while (running && (queue.Count <= 0 || queue[0].Delay.Ticks > 0))
                {
                    yield return null;
                }

                if (!running)
                    yield break;

                IDelayTask task = queue[0];
                queue.RemoveAt(0);
                task.Run();
            }
        }

        public void Stop()
        {
            if (!this.running)
                return;

            this.running = false;
            List<IDelayTask> list = new List<IDelayTask>(queue);
            foreach (IDelayTask task in list)
            {
                if (task != null && !task.IsDone)
                    task.Cancel();
            }
            this.queue.Clear();
        }

        protected virtual void Check()
        {
            if (!this.running)
                throw new RejectedExecutionException("The ScheduledExecutor isn't started.");
        }

        public virtual Asynchronous.IAsyncResult Schedule(Action command, long delay)
        {
            return this.Schedule(command, new TimeSpan(delay * TimeSpan.TicksPerMillisecond));
        }

        public virtual Asynchronous.IAsyncResult Schedule(Action command, TimeSpan delay)
        {
            this.Check();
            return new OneTimeDelayTask(this, command, delay);
        }

        public virtual IAsyncResult<TResult> Schedule<TResult>(Func<TResult> command, long delay)
        {
            return this.Schedule(command, new TimeSpan(delay * TimeSpan.TicksPerMillisecond));
        }

        public virtual IAsyncResult<TResult> Schedule<TResult>(Func<TResult> command, TimeSpan delay)
        {
            this.Check();
            return new OneTimeDelayTask<TResult>(this, command, delay);
        }

        public virtual Asynchronous.IAsyncResult ScheduleAtFixedRate(Action command, long initialDelay, long period)
        {
            return this.ScheduleAtFixedRate(command, new TimeSpan(initialDelay * TimeSpan.TicksPerMillisecond), new TimeSpan(period * TimeSpan.TicksPerMillisecond));
        }

        public virtual Asynchronous.IAsyncResult ScheduleAtFixedRate(Action command, TimeSpan initialDelay, TimeSpan period)
        {
            this.Check();
            return new FixedRateDelayTask(this, command, initialDelay, period);
        }

        public virtual Asynchronous.IAsyncResult ScheduleWithFixedDelay(Action command, long initialDelay, long delay)
        {
            return this.ScheduleWithFixedDelay(command, new TimeSpan(initialDelay * TimeSpan.TicksPerMillisecond), new TimeSpan(delay * TimeSpan.TicksPerMillisecond));
        }

        public virtual Asynchronous.IAsyncResult ScheduleWithFixedDelay(Action command, TimeSpan initialDelay, TimeSpan delay)
        {
            this.Check();
            return new FixedDelayDelayTask(this, command, initialDelay, delay);
        }

        public virtual void Dispose()
        {
            this.Stop();
        }



        interface IDelayTask : Asynchronous.IAsyncResult
        {
            TimeSpan Delay { get; }

            void Run();
        }

        class OneTimeDelayTask : AsyncResult, IDelayTask
        {
            private long startTime;
            private TimeSpan delay;
            private Action command;
            private CoroutineScheduledExecutor executor;
            private ITime time;
            public OneTimeDelayTask(CoroutineScheduledExecutor executor, Action command, TimeSpan delay)
            {
                this.time = executor.Time;
                this.startTime = (long)(time.Time * TimeSpan.TicksPerSecond);
                this.delay = delay;
                this.executor = executor;
                this.command = command;
                this.executor.Add(this);
            }

            public virtual TimeSpan Delay { get { return new TimeSpan(startTime + delay.Ticks - (long)(time.Time * TimeSpan.TicksPerSecond)); } }

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;

                if (!this.executor.Remove(this))
                    return false;

                this.cancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public virtual void Run()
            {
                try
                {
                    if (this.IsDone)
                        return;

                    if (this.IsCancellationRequested)
                    {
                        this.SetCancelled();
                    }
                    else
                    {
                        command();
                        this.SetResult();
                    }
                }
                catch (Exception e)
                {
                    this.SetException(e);
#if DEBUG
                    if (log.IsWarnEnabled)
                        log.Warn(e);
#endif
                }
            }
        }

        class OneTimeDelayTask<TResult> : AsyncResult<TResult>, IDelayTask
        {
            private long startTime;
            private TimeSpan delay;
            private Func<TResult> command;
            private CoroutineScheduledExecutor executor;
            private ITime time;

            public OneTimeDelayTask(CoroutineScheduledExecutor executor, Func<TResult> command, TimeSpan delay)
            {
                this.time = executor.Time;
                this.startTime = (long)(time.Time * TimeSpan.TicksPerSecond);
                this.delay = delay;
                this.executor = executor;
                this.command = command;
                this.executor.Add(this);
            }

            public virtual TimeSpan Delay { get { return new TimeSpan(startTime + delay.Ticks - (long)(time.Time * TimeSpan.TicksPerSecond)); } }

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;

                if (!this.executor.Remove(this))
                    return false;

                this.cancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public virtual void Run()
            {
                try
                {
                    if (this.IsDone)
                        return;

                    if (this.IsCancellationRequested)
                    {
                        this.SetCancelled();
                    }
                    else
                    {
                        this.SetResult(command());
                    }
                }
                catch (Exception e)
                {
                    this.SetException(e);
#if DEBUG
                    if (log.IsWarnEnabled)
                        log.Warn(e);
#endif
                }
            }
        }

        class FixedRateDelayTask : AsyncResult, IDelayTask
        {
            private long startTime;
            private TimeSpan initialDelay;
            private TimeSpan period;
            private CoroutineScheduledExecutor executor;
            private Action command;
            private int count = 0;
            private ITime time;
            public FixedRateDelayTask(CoroutineScheduledExecutor executor, Action command, TimeSpan initialDelay, TimeSpan period) : base()
            {
                this.time = executor.Time;
                this.startTime = (long)(time.Time * TimeSpan.TicksPerSecond);
                this.initialDelay = initialDelay;
                this.period = period;
                this.executor = executor;
                this.command = command;
                this.executor.Add(this);
            }

            public virtual TimeSpan Delay { get { return new TimeSpan(startTime + initialDelay.Ticks + period.Ticks * count - (long)(time.Time * TimeSpan.TicksPerSecond)); } }

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;

                this.executor.Remove(this);
                this.cancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public virtual void Run()
            {
                try
                {
                    if (this.IsDone)
                        return;

                    if (this.IsCancellationRequested)
                    {
                        this.SetCancelled();
                    }
                    else
                    {
                        Interlocked.Increment(ref count);
                        this.executor.Add(this);
                        command();
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    if (log.IsWarnEnabled)
                        log.Warn(e);
#endif
                }
            }
        }

        class FixedDelayDelayTask : AsyncResult, IDelayTask
        {
            private TimeSpan delay;
            private long nextTime;
            private CoroutineScheduledExecutor executor;
            private Action command;
            private ITime time;
            public FixedDelayDelayTask(CoroutineScheduledExecutor executor, Action command, TimeSpan initialDelay, TimeSpan delay) : base()
            {
                this.time = executor.Time;
                this.delay = delay;
                this.executor = executor;
                this.command = command;
                this.nextTime = (long)(time.Time * TimeSpan.TicksPerSecond + initialDelay.Ticks);
                this.executor.Add(this);
            }

            public virtual TimeSpan Delay { get { return new TimeSpan(nextTime - (long)(time.Time * TimeSpan.TicksPerSecond)); } }

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;

                this.executor.Remove(this);
                this.cancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public virtual void Run()
            {
                try
                {
                    if (this.IsDone)
                        return;

                    if (this.IsCancellationRequested)
                    {
                        this.SetCancelled();
                    }
                    else
                    {
                        command();
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    if (log.IsWarnEnabled)
                        log.Warn(e);
#endif
                }
                finally
                {
                    if (this.IsCancellationRequested)
                    {
                        this.SetCancelled();
                    }
                    else
                    {
                        this.nextTime = (long)(time.Time * TimeSpan.TicksPerSecond + this.delay.Ticks);
                        this.executor.Add(this);
                    }
                }
            }
        }

        class ComparerImpl<T> : IComparer<T> where T : IDelayTask
        {
            public int Compare(T x, T y)
            {
                if (x.Delay.Ticks == y.Delay.Ticks)
                    return 0;

                return x.Delay.Ticks > y.Delay.Ticks ? 1 : -1;
            }
        }

        public class ScaledTime : ITime
        {
            public float Time => UnityEngine.Time.time;
        }

        public class UnscaledTime : ITime
        {
            public float Time => UnityEngine.Time.unscaledTime;
        }
    }
}
