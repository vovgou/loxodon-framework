using System;

namespace Loxodon.Framework.Asynchronous
{
    public class ImmutableProgressResult<TProgress> : ProgressResult<TProgress>
    {
        public ImmutableProgressResult(TProgress progress) : base(false)
        {
            this.UpdateProgress(progress);
            this.SetResult(null);
        }

        public ImmutableProgressResult(object result, TProgress progress) : base(false)
        {
            this.UpdateProgress(progress);
            this.SetResult(result);
        }

        public ImmutableProgressResult(Exception exception, TProgress progress) : base(false)
        {
            this.UpdateProgress(progress);
            this.SetException(exception);
        }
    }

    public class ImmutableProgressResult<TProgress, TResult> : ProgressResult<TProgress, TResult>
    {
        public ImmutableProgressResult(TProgress progress) : base(false)
        {
            this.UpdateProgress(progress);
            this.SetResult(default(TResult));
        }

        public ImmutableProgressResult(TResult result, TProgress progress) : base(false)
        {
            this.UpdateProgress(progress);
            this.SetResult(result);
        }

        public ImmutableProgressResult(Exception exception, TProgress progress) : base(false)
        {
            this.UpdateProgress(progress);
            this.SetException(exception);
        }
    }
}
