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

namespace Loxodon.Framework.Net.Connection
{
    public interface IByteBuffer
    {
        byte[] ToArray();

        int Capacity { get; }

        int MaxCapacity { get; }

        int ReaderIndex { get; set; }

        int WriterIndex { get; set; }

        int ReadableBytes { get; }

        int WritableBytes { get; }

        int MaxWritableBytes { get; }

        bool IsReadable();

        bool IsReadable(int size);

        bool IsWritable();

        bool IsWritable(int size);

        IByteBuffer Clear();

        IByteBuffer MarkReaderIndex();

        IByteBuffer ResetReaderIndex();

        IByteBuffer MarkWriterIndex();

        IByteBuffer ResetWriterIndex();

        byte GetByte(int index);

        short GetInt16(int index);

        ushort GetUInt16(int index);

        int GetInt32(int index);

        uint GetUInt32(int index);

        long GetInt64(int index);

        ulong GetUInt64(int index);

        float GetFloat(int index);

        double GetDouble(int index);

        long GetVariableInt(int index);

        long Get7BitEncodedInt(int index);

        IByteBuffer GetBytes(int index, byte[] destination);

        IByteBuffer GetBytes(int index, byte[] destination, int dstIndex, int length);

        IByteBuffer GetBytes(int index, IByteBuffer destination);

        IByteBuffer GetBytes(int index, IByteBuffer destination, int length);

        IByteBuffer GetBytes(int index, IByteBuffer destination, int dstIndex, int length);

        IByteBuffer Slice(int index, int length);

        IByteBuffer Slice(int length);

        IByteBuffer Set(int index, byte value);

        IByteBuffer Set(int index, short value);

        IByteBuffer Set(int index, ushort value);

        IByteBuffer Set(int index, int value);

        IByteBuffer Set(int index, uint value);

        IByteBuffer Set(int index, long value);

        IByteBuffer Set(int index, ulong value);

        IByteBuffer Set(int index, float value);

        IByteBuffer Set(int index, double value);

        IByteBuffer Set(int index, byte[] src);

        IByteBuffer Set(int index, byte[] src, int srcIndex, int length);

        IByteBuffer Set(int index, IByteBuffer src);

        IByteBuffer Set(int index, IByteBuffer src, int length);

        IByteBuffer Set(int index, IByteBuffer src, int srcIndex, int length);

        IByteBuffer SetVariableInt(int index, long value);

        IByteBuffer Set7BitEncodedInt(int index, long value);

        byte ReadByte();

        short ReadInt16();

        ushort ReadUInt16();

        int ReadInt32();

        uint ReadUInt32();

        long ReadInt64();

        ulong ReadUInt64();

        float ReadFloat();

        double ReadDouble();

        long ReadVariableInt();

        long Read7BitEncodedInt();

        IByteBuffer ReadBytes(int length);

        IByteBuffer ReadBytes(IByteBuffer destination);

        IByteBuffer ReadBytes(IByteBuffer destination, int length);

        IByteBuffer ReadBytes(IByteBuffer destination, int dstIndex, int length);

        IByteBuffer ReadBytes(byte[] destination);

        IByteBuffer ReadBytes(byte[] destination, int dstIndex, int length);

        IByteBuffer ReadSlice(int length);

        IByteBuffer ReadSlice(int index, int length);

        IByteBuffer Write(byte value);

        IByteBuffer Write(short value);

        IByteBuffer Write(ushort value);

        IByteBuffer Write(int value);

        IByteBuffer Write(uint value);

        IByteBuffer Write(long value);

        IByteBuffer Write(ulong value);

        IByteBuffer Write(float value);

        IByteBuffer Write(double value);

        IByteBuffer Write(IByteBuffer src);

        IByteBuffer Write(IByteBuffer src, int length);

        IByteBuffer Write(IByteBuffer src, int srcIndex, int length);

        IByteBuffer Write(byte[] src);

        IByteBuffer Write(byte[] src, int srcIndex, int length);

        IByteBuffer WriteVariableInt(long value);

        IByteBuffer Write7BitEncodedInt(long value);
    }
}
