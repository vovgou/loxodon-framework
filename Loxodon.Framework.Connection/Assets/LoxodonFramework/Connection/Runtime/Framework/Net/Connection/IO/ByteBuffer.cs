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
using System.Security;

namespace Loxodon.Framework.Net.Connection
{
    public class ByteBuffer : IByteBuffer
    {
        private const int DEFAULT_CAPACITY = 8192;

        protected byte[] array;
        protected int offset;
        protected int length;
        protected bool extensible;

        protected int readerIndex;
        protected int writerIndex;
        protected int markedReaderIndex;
        protected int markedWriterIndex;
        protected int maxCapacity;
        protected bool isBigEndian;
        public ByteBuffer(bool isBigEndian = true) : this(DEFAULT_CAPACITY, isBigEndian)
        {
        }

        public ByteBuffer(int initCapacity, bool isBigEndian = true)
        {
            this.array = new byte[initCapacity];
            this.offset = 0;
            this.length = Int32.MaxValue;
            this.extensible = true;
            this.maxCapacity = Int32.MaxValue;
            this.readerIndex = 0;
            this.writerIndex = 0;
            this.isBigEndian = isBigEndian;
        }

        public ByteBuffer(byte[] array, bool isBigEndian = true) : this(array, 0, array.Length, isBigEndian)
        {
        }

        public ByteBuffer(byte[] array, int offset, int length, bool isBigEndian = true)
        {
            this.array = array;
            this.offset = offset;
            this.length = length;
            this.extensible = false;
            this.maxCapacity = length;
            this.readerIndex = 0;
            this.writerIndex = length;
            this.isBigEndian = isBigEndian;
        }

        public ByteBuffer(ByteBuffer buffer, int offset, int length, bool isBigEndian = true)
        {
            this.array = buffer.array;
            this.offset = buffer.offset + offset;
            this.length = length;
            this.extensible = false;
            this.maxCapacity = length;
            this.readerIndex = 0;
            this.writerIndex = length;
            this.isBigEndian = isBigEndian;
        }

        internal protected byte[] Array { get { return this.array; } }

        internal protected int ArrayOffset { get { return this.offset; } }

        //internal int ArrayLength { get { return this.length; } }

        public byte[] ToArray()
        {
            byte[] buffer = new byte[this.WriterIndex - this.ReaderIndex];
            System.Array.Copy(array, offset + this.ReaderIndex, buffer, 0, buffer.Length);
            return buffer;
        }

        public int Capacity => Math.Min(this.length, this.array.Length - this.offset);

        public bool IsBigEndian
        {
            get { return this.isBigEndian; }
            set { this.isBigEndian = value; }
        }

        public int MaxCapacity
        {
            get => this.maxCapacity;
            set
            {
                if (value < 0)
                    throw new ArgumentException(nameof(MaxCapacity));
                this.maxCapacity = value;
            }
        }

        public virtual int ReaderIndex
        {
            get { return this.readerIndex; }
            set
            {
                if (value < 0 || value > this.writerIndex)
                    throw new ArgumentOutOfRangeException(string.Format("ReaderIndex: {0} (expected: 0 <= readerIndex <= writerIndex({1})", value, writerIndex));
                this.readerIndex = value;
            }
        }

        public virtual int WriterIndex
        {
            get { return this.writerIndex; }
            set
            {
                if (value < this.readerIndex || value > this.Capacity)
                    throw new ArgumentOutOfRangeException(string.Format("WriterIndex: {0} (expected: 0 <= readerIndex({1}) <= writerIndex <= capacity ({2})", value, readerIndex, Capacity));
                this.writerIndex = value;
            }
        }

        public int ReadableBytes => this.WriterIndex - this.ReaderIndex;
        public int WritableBytes => this.Capacity - this.writerIndex;
        public int MaxWritableBytes => this.MaxCapacity - this.writerIndex;

        public virtual bool IsReadable() => this.writerIndex > this.readerIndex;

        public virtual bool IsReadable(int size) => this.writerIndex - this.readerIndex >= size;

        public virtual bool IsWritable() => this.Capacity > this.writerIndex;

        public virtual bool IsWritable(int size) => this.Capacity - this.writerIndex >= size;

        public virtual IByteBuffer Clear()
        {
            this.readerIndex = this.writerIndex = 0;
            return this;
        }

        public virtual IByteBuffer MarkReaderIndex()
        {
            this.markedReaderIndex = this.readerIndex;
            return this;
        }

        public virtual IByteBuffer ResetReaderIndex()
        {
            this.readerIndex = this.markedReaderIndex;
            return this;
        }

        public virtual IByteBuffer MarkWriterIndex()
        {
            this.markedWriterIndex = this.writerIndex;
            return this;
        }

        public virtual IByteBuffer ResetWriterIndex()
        {
            this.writerIndex = this.markedWriterIndex;
            return this;
        }

        public virtual IByteBuffer EnsureWritable(int minWritableBytes)
        {
            this.EnsureWritable0(minWritableBytes);
            return this;
        }

        protected internal void EnsureWritable0(int minWritableBytes)
        {
            if (!this.extensible)
                throw new InvalidOperationException("The array length is not extensible");

            if (minWritableBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(minWritableBytes), "expected minWritableBytes to be greater than zero");

            if (minWritableBytes <= this.WritableBytes)
                return;

            if (minWritableBytes > this.MaxCapacity - this.writerIndex)
                throw new IndexOutOfRangeException(string.Format($"writerIndex({0}) + minWritableBytes({1}) exceeds maxCapacity({2}): {3}", writerIndex, minWritableBytes, maxCapacity, this));


            // Normalize the current capacity to the power of 2.
            int newCapacity = this.CalculateNewCapacity(this.writerIndex + minWritableBytes, this.MaxCapacity);

            // Adjust to the new capacity.
            this.AdjustCapacity(newCapacity);
        }

        private int CalculateNewCapacity(int minNewCapacity, int maxCapacity)
        {
            if (minNewCapacity % 512 == 0)
                return minNewCapacity;
            return Math.Min(((minNewCapacity / 512) + 1) * 512, maxCapacity);
        }

        private IByteBuffer AdjustCapacity(int newCapacity)
        {
            if (newCapacity < 0 || newCapacity > this.MaxCapacity)
                throw new ArgumentOutOfRangeException("newCapacity", string.Format($"newCapacity: {0} (expected: 0-{1})", newCapacity, maxCapacity));

            int oldCapacity = this.array.Length;
            if (newCapacity > oldCapacity)
            {
                byte[] newArray = new byte[newCapacity];
                System.Array.Copy(array, 0, newArray, 0, oldCapacity);
                this.array = newArray;
            }
            else if (newCapacity < oldCapacity)
            {
                byte[] newArray = new byte[newCapacity];
                int readerIndex = this.ReaderIndex;
                if (readerIndex < newCapacity)
                {
                    int writerIndex = this.WriterIndex;
                    if (writerIndex > newCapacity)
                    {
                        this.writerIndex = newCapacity;
                    }
                    System.Array.Copy(array, readerIndex, newArray, 0, writerIndex - readerIndex);
                }
                else
                {
                    this.readerIndex = newCapacity;
                    this.writerIndex = newCapacity;
                }
                this.array = newArray;
            }
            return this;
        }

        protected virtual void CheckReadableBytes(int minimumReadableBytes)
        {
            if (this.readerIndex > this.writerIndex - minimumReadableBytes)
                throw new IndexOutOfRangeException(string.Format("readerIndex({0}) + length({1}) exceeds writerIndex({2}): {3}", readerIndex, minimumReadableBytes, writerIndex, this));
        }

        protected virtual void CheckIndex(int index, int length)
        {
            if ((index | length | (index + length) | (Capacity - (index + length))) < 0)
                throw new IndexOutOfRangeException(string.Format("index: {0}, length: {1} (expected: range(0, {2}))", index, length, Capacity));
        }

        protected virtual void CheckSrcIndex(int index, int length, int srcIndex, int srcCapacity)
        {
            this.CheckIndex(index, length);
            if ((srcIndex | length | (srcIndex + length) | (srcCapacity - (srcIndex + length))) < 0)
                throw new IndexOutOfRangeException(string.Format("srcIndex: {0}, length: {1} (expected: range(0, {2}))", srcIndex, length, srcCapacity));
        }

        protected virtual void CheckDstIndex(int index, int length, int dstIndex, int dstCapacity)
        {
            this.CheckIndex(index, length);
            if ((dstIndex | length | (dstIndex + length) | (dstCapacity - (dstIndex + length))) < 0)
                throw new IndexOutOfRangeException(string.Format("dstIndex: {0}, length: {1} (expected: range(0, {2}))", dstIndex, length, dstCapacity));
        }

        public virtual byte GetByte(int index)
        {
            this.CheckIndex(index, 1);
            return array[offset + index];
        }

        public virtual short GetInt16(int index)
        {
            this.CheckIndex(index, 2);
            index += offset;
            if (isBigEndian)
                return (short)(array[index + 1] | array[index] << 8);
            else
                return (short)(array[index] | array[index + 1] << 8);
        }

        public virtual ushort GetUInt16(int index)
        {
            return (ushort)GetInt16(index);
        }

        public virtual int GetInt32(int index)
        {
            this.CheckIndex(index, 4);
            index += offset;
            if (isBigEndian)
                return (int)(array[index + 3] | array[index + 2] << 8 | array[index + 1] << 16 | array[index] << 24);
            else
                return (int)(array[index] | array[index + 1] << 8 | array[index + 2] << 16 | array[index + 3] << 24);
        }

        public virtual uint GetUInt32(int index)
        {
            return (uint)GetInt32(index);
        }

        public virtual long GetInt64(int index)
        {
            this.CheckIndex(index, 8);
            index += offset;
            if (isBigEndian)
            {
                uint lo = (uint)(array[index + 7] | array[index + 6] << 8 | array[index + 5] << 16 | array[index + 4] << 24);
                uint hi = (uint)(array[index + 3] | array[index + 2] << 8 | array[index + 1] << 16 | array[index] << 24);
                return ((long)hi) << 32 | lo;
            }
            else
            {
                uint lo = (uint)(array[index] | array[index + 1] << 8 | array[index + 2] << 16 | array[index + 3] << 24);
                uint hi = (uint)(array[index + 4] | array[index + 5] << 8 | array[index + 6] << 16 | array[index + 7] << 24);
                return ((long)hi) << 32 | lo;
            }
        }

        public virtual ulong GetUInt64(int index)
        {
            return (ulong)GetInt64(index);
        }

        public virtual double GetDouble(int index)
        {
            return ToDouble(GetInt64(index));
        }

        public virtual float GetFloat(int index)
        {
            return ToSingle(GetInt32(index));
        }

        public virtual long GetVariableInt(int index)
        {
            int len = ReadVariableIntLength(GetByte(index));
            return GetVariableInt(index, len);
        }

        public virtual long Get7BitEncodedInt(int index)
        {
            long value;
            if (this.isBigEndian)
                this.Get7BitEncodedIntBE(index, out value);
            else
                this.Get7BitEncodedIntLE(index, out value);
            return value;
        }

        public virtual IByteBuffer GetBytes(int index, byte[] destination)
        {
            this.GetBytes(index, destination, 0, destination.Length);
            return this;
        }

        public virtual IByteBuffer GetBytes(int index, byte[] destination, int dstIndex, int length)
        {
            this.CheckDstIndex(index, length, dstIndex, destination.Length);
            System.Array.Copy(this.array, offset + index, destination, dstIndex, length);
            return this;
        }

        public virtual IByteBuffer GetBytes(int index, IByteBuffer destination)
        {
            this.GetBytes(index, destination, destination.WritableBytes);
            return this;
        }

        public virtual IByteBuffer GetBytes(int index, IByteBuffer destination, int length)
        {
            this.GetBytes(index, destination, destination.WriterIndex, length);
            destination.WriterIndex += length;
            return this;
        }

        public virtual IByteBuffer GetBytes(int index, IByteBuffer destination, int dstIndex, int length)
        {
            this.CheckDstIndex(index, length, dstIndex, destination.Capacity);
            if (destination is ByteBuffer dest)
            {
                this.GetBytes(index, dest.Array, dest.ArrayOffset + dstIndex, length);
            }
            else
            {
                destination.Set(dstIndex, this.array, offset + index, length);
            }
            return this;
        }

        public virtual IByteBuffer Slice(int length)
        {
            return Slice(this.readerIndex, length);
        }

        public virtual IByteBuffer Slice(int index, int length)
        {
            return new ByteBuffer(this, index, length);
        }

        public virtual IByteBuffer Set(int index, byte value)
        {
            this.CheckIndex(index, 1);
            array[offset + index] = value;
            return this;
        }

        public virtual IByteBuffer Set(int index, short value)
        {
            this.CheckIndex(index, 2);
            index += offset;
            if (isBigEndian)
            {
                array[index++] = (byte)(value >> 8 & 0xFF);
                array[index++] = (byte)(value & 0xFF);
            }
            else
            {
                array[index++] = (byte)(value & 0xFF);
                array[index++] = (byte)(value >> 8 & 0xFF);
            }
            return this;
        }

        public virtual IByteBuffer Set(int index, ushort value)
        {
            return this.Set(index, (short)value);
        }

        public virtual IByteBuffer Set(int index, int value)
        {
            this.CheckIndex(index, 4);
            index += offset;
            if (isBigEndian)
            {
                array[index++] = (byte)(value >> 24 & 0xFF);
                array[index++] = (byte)(value >> 16 & 0xFF);
                array[index++] = (byte)(value >> 8 & 0xFF);
                array[index++] = (byte)(value & 0xFF);
            }
            else
            {
                array[index++] = (byte)(value & 0xFF);
                array[index++] = (byte)(value >> 8 & 0xFF);
                array[index++] = (byte)(value >> 16 & 0xFF);
                array[index++] = (byte)(value >> 24 & 0xFF);
            }
            return this;
        }

        public virtual IByteBuffer Set(int index, uint value)
        {
            return Set(index, (int)value);
        }

        public virtual IByteBuffer Set(int index, long value)
        {
            this.CheckIndex(index, 8);
            index += offset;
            if (isBigEndian)
            {
                array[index++] = (byte)(value >> 56 & 0xFF);
                array[index++] = (byte)(value >> 48 & 0xFF);
                array[index++] = (byte)(value >> 40 & 0xFF);
                array[index++] = (byte)(value >> 32 & 0xFF);
                array[index++] = (byte)(value >> 24 & 0xFF);
                array[index++] = (byte)(value >> 16 & 0xFF);
                array[index++] = (byte)(value >> 8 & 0xFF);
                array[index++] = (byte)(value & 0xFF);
            }
            else
            {
                array[index++] = (byte)(value & 0xFF);
                array[index++] = (byte)(value >> 8 & 0xFF);
                array[index++] = (byte)(value >> 16 & 0xFF);
                array[index++] = (byte)(value >> 24 & 0xFF);
                array[index++] = (byte)(value >> 32 & 0xFF);
                array[index++] = (byte)(value >> 40 & 0xFF);
                array[index++] = (byte)(value >> 48 & 0xFF);
                array[index++] = (byte)(value >> 56 & 0xFF);
            }
            return this;
        }

        public virtual IByteBuffer Set(int index, ulong value)
        {
            return this.Set(index, (long)value);
        }

        public virtual IByteBuffer Set(int index, float value)
        {
            return this.Set(index, ToInt32(value));
        }

        public virtual IByteBuffer Set(int index, double value)
        {
            return this.Set(index, ToInt64(value));
        }

        public virtual IByteBuffer SetVariableInt(int index, long value)
        {
            int len = GetVariableIntLength(value);
            SetVariableInt(index, value, len);
            return this;
        }

        public virtual IByteBuffer Set7BitEncodedInt(int index, long value)
        {
            int len = Get7BitEncodedLength(value);
            if (isBigEndian)
                Set7BitEncodedIntBE(index, value, len);
            else
                Set7BitEncodedIntLE(index, value, len);
            return this;
        }

        public virtual IByteBuffer Set(int index, byte[] src)
        {
            this.Set(index, src, 0, src.Length);
            return this;
        }

        public virtual IByteBuffer Set(int index, byte[] src, int srcIndex, int length)
        {
            this.CheckSrcIndex(index, length, srcIndex, src.Length);
            System.Array.Copy(src, srcIndex, this.array, offset + index, length);
            return this;
        }

        public virtual IByteBuffer Set(int index, IByteBuffer src)
        {
            this.Set(index, src, src.ReadableBytes);
            return this;
        }

        public virtual IByteBuffer Set(int index, IByteBuffer src, int length)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            this.CheckIndex(index, length);
            if (length > src.ReadableBytes)
                throw new IndexOutOfRangeException(string.Format("length({0}) exceeds src.readableBytes({1}) where src is: {2}", length, src.ReadableBytes, src));

            this.Set(index, src, src.ReaderIndex, length);
            src.ReaderIndex += length;
            return this;
        }

        public virtual IByteBuffer Set(int index, IByteBuffer src, int srcIndex, int length)
        {
            this.CheckSrcIndex(index, length, srcIndex, src.Capacity);
            if (src is ByteBuffer buf)
                this.Set(index, buf.Array, buf.ArrayOffset + srcIndex, length);
            else
                src.GetBytes(srcIndex, this.array, this.offset + index, length);
            return this;
        }

        public virtual byte ReadByte()
        {
            this.CheckReadableBytes(1);
            byte v = this.GetByte(this.readerIndex);
            this.readerIndex += 1;
            return v;
        }

        public virtual short ReadInt16()
        {
            this.CheckReadableBytes(2);
            short v = this.GetInt16(this.readerIndex);
            this.readerIndex += 2;
            return v;
        }

        public virtual ushort ReadUInt16()
        {
            return (ushort)(this.ReadInt16());
        }

        public virtual int ReadInt32()
        {
            this.CheckReadableBytes(4);
            int v = this.GetInt32(this.readerIndex);
            this.readerIndex += 4;
            return v;
        }

        public virtual uint ReadUInt32()
        {
            return (uint)ReadInt32();
        }

        public virtual long ReadInt64()
        {
            this.CheckReadableBytes(8);
            long v = this.GetInt64(this.readerIndex);
            this.readerIndex += 8;
            return v;
        }

        public virtual ulong ReadUInt64()
        {
            return (ulong)ReadInt64();
        }

        public virtual double ReadDouble()
        {
            return ToDouble(ReadInt64());
        }

        public virtual float ReadFloat()
        {
            return ToSingle(ReadInt32());
        }

        public virtual long ReadVariableInt()
        {
            int index = this.readerIndex;
            this.CheckReadableBytes(1);
            int len = ReadVariableIntLength(GetByte(index));
            this.CheckReadableBytes(len);
            long value = GetVariableInt(index, len);
            this.readerIndex += len;
            return value;
        }

        public virtual long Read7BitEncodedInt()
        {
            long value;
            int len;
            int index = this.readerIndex;
            if (isBigEndian)
                len = Get7BitEncodedIntBE(index, out value);
            else
                len = Get7BitEncodedIntLE(index, out value);
            this.readerIndex += len;
            return value;
        }

        public virtual IByteBuffer ReadBytes(int length)
        {
            this.CheckReadableBytes(length);
            if (length == 0)
                return null;

            byte[] buffer = new byte[length];
            this.ReadBytes(buffer, 0, length);
            return new ByteBuffer(buffer, 0, length);
        }

        public virtual IByteBuffer ReadBytes(IByteBuffer destination)
        {
            this.ReadBytes(destination, destination.WritableBytes);
            return this;
        }

        public virtual IByteBuffer ReadBytes(IByteBuffer destination, int length)
        {
            if (length > destination.WritableBytes)
                throw new IndexOutOfRangeException(string.Format("length({0}) exceeds destination.WritableBytes({1}) where destination is: {2}", length, destination.WritableBytes, destination));

            this.ReadBytes(destination, destination.WriterIndex, length);
            destination.WriterIndex += length;
            return this;
        }

        public virtual IByteBuffer ReadBytes(IByteBuffer destination, int dstIndex, int length)
        {
            this.CheckReadableBytes(length);
            this.GetBytes(this.readerIndex, destination, dstIndex, length);
            this.readerIndex += length;
            return this;
        }

        public virtual IByteBuffer ReadBytes(byte[] destination)
        {
            this.ReadBytes(destination, 0, destination.Length);
            return this;
        }

        public virtual IByteBuffer ReadBytes(byte[] destination, int dstIndex, int length)
        {
            this.CheckReadableBytes(length);
            this.GetBytes(this.readerIndex, destination, dstIndex, length);
            this.readerIndex += length;
            return this;
        }

        public virtual IByteBuffer Write(byte value)
        {
            this.EnsureWritable0(1);
            this.Set(this.writerIndex, value);
            this.writerIndex += 1;
            return this;
        }

        public virtual IByteBuffer Write(short value)
        {
            this.EnsureWritable0(2);
            this.Set(this.writerIndex, value);
            this.writerIndex += 2;
            return this;
        }

        public virtual IByteBuffer Write(ushort value)
        {
            this.EnsureWritable0(2);
            this.Set(this.writerIndex, value);
            this.writerIndex += 2;
            return this;
        }

        public virtual IByteBuffer Write(int value)
        {
            this.EnsureWritable0(4);
            this.Set(this.writerIndex, value);
            this.writerIndex += 4;
            return this;
        }

        public virtual IByteBuffer Write(uint value)
        {
            this.EnsureWritable0(4);
            this.Set(this.writerIndex, value);
            this.writerIndex += 4;
            return this;
        }

        public virtual IByteBuffer Write(long value)
        {
            this.EnsureWritable0(8);
            this.Set(this.writerIndex, value);
            this.writerIndex += 8;
            return this;
        }

        public virtual IByteBuffer Write(ulong value)
        {
            this.EnsureWritable0(8);
            this.Set(this.writerIndex, value);
            this.writerIndex += 8;
            return this;
        }

        public virtual IByteBuffer Write(float value)
        {
            this.EnsureWritable0(4);
            this.Set(this.writerIndex, value);
            this.writerIndex += 4;
            return this;
        }

        public virtual IByteBuffer Write(double value)
        {
            this.EnsureWritable0(8);
            this.Set(this.writerIndex, value);
            this.writerIndex += 8;
            return this;
        }

        public virtual IByteBuffer WriteVariableInt(long value)
        {
            int len = GetVariableIntLength(value);
            this.EnsureWritable0(len);
            int index = this.writerIndex;
            this.SetVariableInt(index, value, len);
            this.writerIndex += len;
            return this;
        }

        public virtual IByteBuffer Write7BitEncodedInt(long value)
        {
            int len = Get7BitEncodedLength(value);
            this.EnsureWritable0(len);
            int index = this.writerIndex;
            if (this.isBigEndian)
                Set7BitEncodedIntBE(index, value, len);
            else
                Set7BitEncodedIntLE(index, value, len);
            this.writerIndex += len;
            return this;
        }

        public virtual IByteBuffer Write(IByteBuffer src)
        {
            return Write(src, src.ReaderIndex, src.ReadableBytes);
        }

        public virtual IByteBuffer Write(IByteBuffer src, int length)
        {
            return Write(src, src.ReaderIndex, length);
        }

        public virtual IByteBuffer Write(IByteBuffer src, int srcIndex, int length)
        {
            if (length > src.ReadableBytes)
                throw new IndexOutOfRangeException(string.Format("length({0}) exceeds src.readableBytes({1}) where src is: {2}", length, src.ReadableBytes, src));

            this.EnsureWritable0(length);
            this.Set(this.writerIndex, src, srcIndex, length);
            this.writerIndex += length;
            return this;
        }

        public virtual IByteBuffer Write(byte[] src)
        {
            return this.Write(src, 0, src.Length);
        }

        public virtual IByteBuffer Write(byte[] src, int srcIndex, int length)
        {
            this.EnsureWritable0(length);
            this.Set(this.writerIndex, src, srcIndex, length);
            this.writerIndex += length;
            return this;
        }

        public virtual IByteBuffer ReadSlice(int length)
        {
            return ReadSlice(this.readerIndex, length);
        }

        public virtual IByteBuffer ReadSlice(int index, int length)
        {
            this.CheckReadableBytes(length);
            IByteBuffer slice = this.Slice(this.readerIndex, length);
            this.readerIndex += length;
            return slice;
        }

        protected virtual long GetVariableInt(int index, int len)
        {
            switch (len)
            {
                case 1:
                    return GetByte(index);
                case 2:
                    return GetInt16(index) & 0x3fff;
                case 4:
                    return GetInt32(index) & 0x3fffffff;
                case 8:
                    return GetInt64(index) & 0x3fffffffffffffffL;
                default:
                    throw new ArgumentException();
            }
        }

        protected virtual void SetVariableInt(int index, long value, int len)
        {
            switch (len)
            {
                case 1:
                    this.Set(index, (byte)value);
                    break;
                case 2:
                    this.Set(index, (short)value);
                    this.Set(index, (byte)(GetByte(index) | 0x40));
                    break;
                case 4:
                    this.Set(index, (int)value);
                    this.Set(index, (byte)(GetByte(index) | 0x80));
                    break;
                case 8:
                    this.Set(index, value);
                    this.Set(index, (byte)(GetByte(index) | 0xc0));
                    break;
                default:
                    throw new ArgumentException(nameof(value));
            }
        }

        protected virtual int Get7BitEncodedIntLE(int index, out long value)
        {
            int count = 0;
            byte b;
            value = 0;
            do
            {
                if (count++ == 9)  // 9 bytes max per Int64, shift += 7
                    throw new FormatException("More than 63 bit");

                b = GetByte(index++);
                value |= (long)((b & 0x7F) << 7);
            } while ((b & 0x80) != 0);
            return count;
        }

        protected virtual int Get7BitEncodedIntBE(int index, out long value)
        {
            int count = 0;
            byte b;
            value = 0;
            do
            {
                if (count++ == 9)
                    throw new FormatException("More than 63 bit");

                b = GetByte(index++);
                value = (value << 7) | (long)(b & 0x7F);
            } while ((b & 0x80) != 0);
            return count;
        }

        protected virtual void Set7BitEncodedIntLE(int index, long value, int len)
        {
            ulong v = (ulong)value;
            while (v >= 0x80)
            {
                Set(index++, (byte)(v | 0x80));
                v >>= 7;
            }
            Set(index++, (byte)v);
        }

        protected virtual void Set7BitEncodedIntBE(int index, long value, int len)
        {
            ulong v = (ulong)value;
            index += len;
            Set(--index, (byte)(v & 0x7F));
            while (v >= 0x80)
            {
                v >>= 7;
                Set(--index, (byte)(v | 0x80));
            }
        }

        public static int Get7BitEncodedLength(long value)
        {
            if (value < 0)
                throw new ArgumentException(string.Format("value: {0} ,expected: 0 <= value <= 0x7FFFFFFFFFFFFFFFL", value));

            if (value <= 0x7F)
            {
                return 1;
            }
            if (value <= 0x3FFF)
            {
                return 2;
            }
            if (value <= 0x1FFFFF)
            {
                return 3;
            }
            if (value <= 0xFFFFFFF)
            {
                return 4;
            }
            if (value <= 0x7FFFFFFF)
            {
                return 5;
            }
            if (value <= 0x3FFFFFFFFFFL)
            {
                return 6;
            }
            if (value <= 0x1FFFFFFFFFFFFL)
            {
                return 7;
            }
            if (value <= 0xFFFFFFFFFFFFFFL)
            {
                return 8;
            }
            if (value <= 0x7FFFFFFFFFFFFFFFL)
            {
                return 9;
            }

            throw new ArgumentException(string.Format("value: {0} ,expected: 0 <= value <= 0x7FFFFFFFFFFFFFFFL", value));
        }


        public static int GetVariableIntLength(long value)
        {
            if (value < 0)
                throw new ArgumentException(string.Format("value: {0} ,expected: 0 <= value <= 4611686018427387903L", value));

            if (value <= 63)
            {
                return 1;
            }
            if (value <= 16383)
            {
                return 2;
            }
            if (value <= 1073741823)
            {
                return 4;
            }
            if (value <= 4611686018427387903L)
            {
                return 8;
            }

            throw new ArgumentException(string.Format("value: {0} ,expected: 0 <= value <= 4611686018427387903L", value));
        }

        public static int ReadVariableIntLength(byte b)
        {
            byte val = (byte)(b >> 6);
            if ((val & 1) != 0)
            {
                if ((val & 2) != 0)
                {
                    return 8;
                }
                return 2;
            }
            if ((val & 2) != 0)
            {
                return 4;
            }
            return 1;
        }

        [SecuritySafeCritical]
        protected static unsafe float ToSingle(int value)
        {
            return *((float*)&value);
        }

        [SecuritySafeCritical]
        protected static unsafe double ToDouble(long value)
        {
            return *((double*)&value);
        }

        [SecuritySafeCritical]
        protected static unsafe int ToInt32(float value)
        {
            return *((int*)&value);
        }

        [SecuritySafeCritical]
        protected static unsafe long ToInt64(double value)
        {
            return *((long*)&value);
        }
    }
}
