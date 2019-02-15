using System.Threading;

namespace Loxodon.Framework.Execution
{
    /// <summary>
    /// 
    /// </summary>
    public class CountFinishedEvent
    {
        private readonly object _lock = new object();
        private readonly ManualResetEvent resetEvent = new ManualResetEvent(false);
        private int count = 0;

        public CountFinishedEvent(int count)
        {
            this.count = count;
        }

        public bool Reset()
        {
            lock (_lock)
            {
                return this.resetEvent.Reset();
            }
        }

        public bool Set()
        {
            lock (_lock)
            {
                if (--count <= 0)
                    return resetEvent.Set();
                return false;
            }
        }

        public bool Wait()
        {
            lock (_lock)
            {
                return resetEvent.WaitOne();
            }
        }

        public bool Wait(int millisecondsTimeout)
        {
            lock (_lock)
            {
                return resetEvent.WaitOne(millisecondsTimeout);
            }
        }
    }
}
