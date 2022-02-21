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
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace Loxodon.Framework.Net.Connection
{
    public class BinaryReader : IDisposable
    {
        private const int DEFAULT_BUFFER_SIZE = 4192;
        private Stream input;
        private bool leaveOpen;
        private bool isBigEndian;
        private byte[] buffer;

        public BinaryReader(Stream input, bool leaveOpen) : this(input, leaveOpen, true)
        {
        }

        public BinaryReader(Stream input, bool leaveOpen, bool isBigEndian)
        {
            this.input = input;
            this.leaveOpen = leaveOpen;
            this.isBigEndian = isBigEndian;
            this.buffer = new byte[DEFAULT_BUFFER_SIZE];
        }

        public virtual Stream BaseStream { get { return this.input; } }

        public virtual bool IsBigEndian
        {
            get { return this.isBigEndian; }
            set { this.isBigEndian = value; }
        }

        public virtual async Task<byte> ReadByte()
        {
            await Read(buffer, 0, 1).ConfigureAwait(false);
            return buffer[0];
        }

        public virtual async Task<ushort> ReadUInt16()
        {
            await Read(buffer, 0, 2).ConfigureAwait(false);
            if (isBigEndian)
                return (ushort)(buffer[1] | buffer[0] << 8);
            else
                return (ushort)(buffer[0] | buffer[1] << 8);
        }

        public virtual async Task<short> ReadInt16()
        {
            await Read(buffer, 0, 2).ConfigureAwait(false);
            if (isBigEndian)
                return (short)(buffer[1] | buffer[0] << 8);
            else
                return (short)(buffer[0] | buffer[1] << 8);
        }

        public virtual async Task<uint> ReadUInt32()
        {
            await Read(buffer, 0, 4).ConfigureAwait(false);
            if (isBigEndian)
                return (uint)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24);
            else
                return (uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
        }

        public virtual async Task<int> ReadInt32()
        {
            await Read(buffer, 0, 4).ConfigureAwait(false);
            if (isBigEndian)
                return (int)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24);
            else
                return (int)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
        }

        public virtual async Task<ulong> ReadUInt64()
        {
            await Read(buffer, 0, 8).ConfigureAwait(false);
            if (isBigEndian)
            {
                uint lo = (uint)(buffer[7] | buffer[6] << 8 | buffer[5] << 16 | buffer[4] << 24);
                uint hi = (uint)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24);
                return ((ulong)hi) << 32 | lo;
            }
            else
            {
                uint lo = (uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
                uint hi = (uint)(buffer[4] | buffer[5] << 8 | buffer[6] << 16 | buffer[7] << 24);
                return ((ulong)hi) << 32 | lo;
            }
        }

        public virtual async Task<long> ReadInt64()
        {
            await Read(buffer, 0, 8).ConfigureAwait(false);
            if (isBigEndian)
            {
                uint lo = (uint)(buffer[7] | buffer[6] << 8 | buffer[5] << 16 | buffer[4] << 24);
                uint hi = (uint)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24);
                return ((long)hi) << 32 | lo;
            }
            else
            {
                uint lo = (uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
                uint hi = (uint)(buffer[4] | buffer[5] << 8 | buffer[6] << 16 | buffer[7] << 24);
                return ((long)hi) << 32 | lo;
            }
        }

        public virtual async Task<float> ReadSingle()
        {
            CheckDisposed();
            uint value = await ReadUInt32().ConfigureAwait(false);
            return ToSingle(value);
        }

        public virtual async Task<double> ReadDouble()
        {
            CheckDisposed();
            ulong value = await ReadUInt64().ConfigureAwait(false);
            return ToDouble(value);
        }

        public virtual async Task<int> Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            int n = 0;
            while (n < count)
            {
                int len = await this.input.ReadAsync(buffer, offset + n, count - n).ConfigureAwait(false);
                if (len <= 0)
                    throw new IOException("Stream is closed.");

                n += len;
            }
            return n;
        }

        public virtual async Task<int> Read(IByteBuffer buffer, int count)
        {
            CheckDisposed();
            if (buffer is ByteBuffer buf)
            {
                buf.EnsureWritable(count);
                int n = 0;
                int offset = buf.ArrayOffset + buf.WriterIndex;
                while (n < count)
                {
                    int len = await this.input.ReadAsync(buf.Array, offset + n, count - n).ConfigureAwait(false);
                    if (len <= 0)
                        throw new IOException("Stream is closed.");

                    n += len;
                }
                buf.WriterIndex += n;
                return n;
            }
            else
            {
                int n = 0;
                while (n < count)
                {
                    int len = await this.input.ReadAsync(this.buffer, 0, Math.Min(this.buffer.Length, count - n)).ConfigureAwait(false);
                    if (len <= 0)
                        throw new IOException("Stream is closed.");

                    buffer.Write(this.buffer, 0, len);
                    n += len;
                }
                return n;
            }
        }

        [SecuritySafeCritical]
        protected unsafe float ToSingle(uint value)
        {
            return *((float*)&value);
        }

        [SecuritySafeCritical]
        protected unsafe double ToDouble(ulong value)
        {
            return *((double*)&value);
        }

        protected void CheckDisposed()
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(BinaryReader));
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (!leaveOpen && input != null)
                {
                    this.input.Close();
                    this.input = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
