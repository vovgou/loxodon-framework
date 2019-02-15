#if !UNITY_5_3_OR_NEWER
using System;
using System.Collections;

namespace Loxodon.Framework.Execution
{
    public class WaitWhile : IEnumerator
    {
        private Func<bool> predicate;
        public WaitWhile(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            return predicate();
        }

        public void Reset()
        {
        }
    }
}
#endif
