using System;

namespace Loxodon.Framework.Net.Connection
{
    public class IdleStateEventArgs : EventArgs
    {
        public static readonly IdleStateEventArgs FirstReaderIdleStateEvent = new IdleStateEventArgs(true, IdleState.ReaderIdle);
        public static readonly IdleStateEventArgs ReaderIdleStateEvent = new IdleStateEventArgs(false, IdleState.ReaderIdle);
        public static readonly IdleStateEventArgs FirstWriterIdleStateEvent = new IdleStateEventArgs(true, IdleState.WriterIdle);
        public static readonly IdleStateEventArgs WriterIdleStateEvent = new IdleStateEventArgs(false, IdleState.WriterIdle);
        public static readonly IdleStateEventArgs FirstAllIdleStateEvent = new IdleStateEventArgs(true, IdleState.AllIdle);
        public static readonly IdleStateEventArgs AllIdleStateEvent = new IdleStateEventArgs(false, IdleState.AllIdle);

        IdleStateEventArgs(bool first, IdleState state)
        {
            this.State = state;
            this.IsFirst = first;
        }

        public IdleState State { get; }

        public bool IsFirst { get; }

        public override string ToString()
        {
            return string.Format("IdleStateEvent[first:{0} state:{1}]", this.IsFirst, this.State);
        }
    }

    public enum IdleState
    {
        ReaderIdle,
        WriterIdle,
        AllIdle
    }
}