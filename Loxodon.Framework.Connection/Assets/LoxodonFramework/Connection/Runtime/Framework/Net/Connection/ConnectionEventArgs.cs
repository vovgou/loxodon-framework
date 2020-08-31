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
    public class ConnectionEventArgs : EventArgs
    {
        public static readonly ConnectionEventArgs ConnectingEventArgs = new ConnectionEventArgs("Connecting");
        public static readonly ConnectionEventArgs ReconnectingEventArgs = new ConnectionEventArgs("Reconnecting");
        public static readonly ConnectionEventArgs ConnectedEventArgs = new ConnectionEventArgs("Connected");
        public static readonly ConnectionEventArgs FailedEventArgs = new ConnectionEventArgs("Failed");
        public static readonly ConnectionEventArgs ExceptionEventArgs = new ConnectionEventArgs("Exception");
        public static readonly ConnectionEventArgs ClosingEventArgs = new ConnectionEventArgs("Closing");
        public static readonly ConnectionEventArgs ClosedEventArgs = new ConnectionEventArgs("Closed");

        public ConnectionEventArgs(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return string.Format("ConnectionEvent:[{0}]", this.Name);
        }
    }
}
