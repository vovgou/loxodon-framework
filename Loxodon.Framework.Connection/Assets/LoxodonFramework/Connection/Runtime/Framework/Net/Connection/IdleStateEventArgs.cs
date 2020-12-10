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