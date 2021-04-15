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

#if !NETFX_CORE && !UNITY_WSA && !UNITY_WSA_10_0
using System;
using System.IO;
using System.Security.Cryptography;

namespace Loxodon.Framework.Security.Cryptography
{
    public class AesCTRCryptoStream : Stream
    {
        private readonly object _lock = new object();
        private Stream stream;

        private bool leaveOpen;
        private bool canRead;
        private bool canSeek;
        private bool canWrite;

        private byte[] writeBuffer;
        private byte[] readBuffer;
        private AesCTRCryptoTransform transform;

        public AesCTRCryptoStream(Stream stream, AesCTRCryptoTransform transform, CryptoStreamMode streamMode) : this(stream, transform, streamMode, false)
        {
        }

        public AesCTRCryptoStream(Stream stream, AesCTRCryptoTransform transform, CryptoStreamMode streamMode, bool leaveOpen)
        {
            this.stream = stream;
            this.transform = transform;
            this.leaveOpen = leaveOpen;

            this.canRead = stream.CanRead;
            this.canSeek = stream.CanSeek;
            this.canWrite = stream.CanWrite;

            if (streamMode == CryptoStreamMode.Read && !this.canRead)
                throw new ArgumentException("The stream is not readable", "stream");

            if (streamMode == CryptoStreamMode.Write && !this.canWrite)
                throw new ArgumentException("The stream is not writable", "stream");


            this.transform.Position = stream.Position;
            if (streamMode == CryptoStreamMode.Read)
            {
                this.readBuffer = new byte[8192];
            }
            else
            {
                this.writeBuffer = new byte[8192];
            }
        }

        public override bool CanRead { get { return canRead; } }

        public override bool CanSeek { get { return canSeek; } }

        public override bool CanWrite { get { return canWrite; } }

        public override long Length { get { return stream.Length; } }

        public override long Position
        {
            get { return stream.Position; }
            set
            {
                if (stream.Position == value)
                    return;

                this.Seek(value, SeekOrigin.Begin);
            }
        }

        public override void Flush()
        {
            this.stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_lock)
            {
                int remainingSize = count;
                while (remainingSize > 0)
                {
                    int length = stream.Read(readBuffer, 0, Math.Min(readBuffer.Length, remainingSize));
                    if (length <= 0)
                        return count - remainingSize;

                    transform.TransformBlock(readBuffer, 0, length, buffer, offset);
                    offset += length;
                    remainingSize -= length;
                }
                return count;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (_lock)
            {
                long position = this.stream.Seek(offset, origin);
                transform.Position = position;
                return position;
            }
        }

        public override void SetLength(long value)
        {
            this.stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_lock)
            {
                int remainingSize = count;
                while (remainingSize > 0)
                {
                    int length = transform.TransformBlock(buffer, offset, Math.Min(writeBuffer.Length, remainingSize), writeBuffer, 0);
                    if (length <= 0)
                        return;

                    stream.Write(writeBuffer, 0, length);
                    offset += length;
                    remainingSize -= length;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (!leaveOpen)
                    {
                        if (stream != null)
                        {
                            stream.Close();
                            stream = null;
                        }
                    }
                }
            }
            finally
            {
                try
                {
                    if (readBuffer != null)
                        Array.Clear(readBuffer, 0, readBuffer.Length);
                    if (writeBuffer != null)
                        Array.Clear(writeBuffer, 0, writeBuffer.Length);

                    readBuffer = null;
                    writeBuffer = null;
                    this.canRead = false;
                    this.canWrite = false;
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }
    }
}
#endif