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

using Loxodon.Framework.Examples.Messages;
using Loxodon.Framework.Net.Connection;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BinaryReader = Loxodon.Framework.Net.Connection.BinaryReader;

namespace Loxodon.Framework.Examples
{
    public class DefaultDecoder : IMessageDecoder<IMessage>
    {
        protected readonly SemaphoreSlim readLock = new SemaphoreSlim(1, 1);
        protected ByteBuffer buffer = new ByteBuffer();
        public async Task<IMessage> Decode(BinaryReader reader)
        {
            await readLock.WaitAsync();
            try
            {
                int count = await reader.ReadInt32();

                buffer.Clear();
                await reader.Read(buffer, count);

                MessageType messageType = (MessageType)buffer.ReadByte();
                switch (messageType)
                {
                    case MessageType.Notification:
                        {
                            Notification notification = new Notification();
                            notification.CommandID = buffer.ReadInt32();
                            notification.Sequence = buffer.ReadUInt32();
                            notification.ContentType = buffer.ReadInt16();

                            byte[] data = new byte[count - 11];
                            buffer.ReadBytes(data, 0, data.Length);
                            notification.Content = data;
                            return notification;
                        }
                    case MessageType.Response:
                        {
                            Response response = new Response();
                            response.Status = buffer.ReadInt32();
                            response.Sequence = buffer.ReadUInt32();
                            response.ContentType = buffer.ReadInt16();

                            byte[] data = new byte[count - 11];
                            buffer.ReadBytes(data, 0, data.Length);
                            response.Content = data;
                            return response;
                        }
                    case MessageType.Request:
                        {
                            //客户端解码器不需要解码Request类型
                            Request request = new Request();
                            request.CommandID = buffer.ReadInt32();
                            request.Sequence = buffer.ReadUInt32();
                            request.ContentType = buffer.ReadInt16();

                            byte[] data = new byte[count - 11];
                            buffer.ReadBytes(data, 0, data.Length);
                            request.Content = data;
                            return request;
                        }
                }

                throw new IOException();
            }
            finally
            {
                readLock.Release();
            }
        }
    }
}
