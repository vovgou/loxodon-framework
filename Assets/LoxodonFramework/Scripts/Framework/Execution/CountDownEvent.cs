using System.Threading;

namespace Loxodon.Framework.Execution
{
    /// <summary>
    /// 
    /// </summary>
    public class CountDownEvent
    {
        private int max = 1;
        private int count = 0;
        private AutoResetEvent reset = new AutoResetEvent(false);

        public CountDownEvent(int max)
        {
            this.max = max;
            this.count = 0;
        }

        public bool Set()
        {
            Interlocked.Decrement(ref count);
            return reset.Set();
        }

        public bool Wait()
        {
            if (Interlocked.Increment(ref count) >= max)
                return reset.WaitOne();
            return false;
        }
    }
}

