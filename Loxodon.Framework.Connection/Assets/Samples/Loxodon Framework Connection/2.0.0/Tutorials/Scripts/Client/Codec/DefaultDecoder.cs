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

        public async Task<IMessage> Decode(BinaryReader reader)
        {
            await readLock.WaitAsync();
            try
            {
                int count = await reader.ReadInt32();
                MessageType messageType = (MessageType)await reader.ReadByte();
                switch (messageType)
                {
                    case MessageType.Notification:
                        {
                            Notification notification = new Notification();
                            notification.CommandID = await reader.ReadInt32();
                            notification.Sequence = await reader.ReadUInt32();
                            notification.ContentType = await reader.ReadInt16();

                            byte[] buffer = new byte[count - 11];
                            await reader.Read(buffer, 0, buffer.Length);
                            notification.Content = buffer;
                            return notification;
                        }
                    case MessageType.Response:
                        {
                            Response response = new Response();
                            response.Status = await reader.ReadInt32();
                            response.Sequence = await reader.ReadUInt32();
                            response.ContentType = await reader.ReadInt16();

                            byte[] buffer = new byte[count - 11];
                            await reader.Read(buffer, 0, buffer.Length);
                            response.Content = buffer;
                            return response;
                        }
                    case MessageType.Request:
                        {
                            //客户端解码器不需要解码Request类型
                            Request request = new Request();
                            request.CommandID = await reader.ReadInt32();
                            request.Sequence = await reader.ReadUInt32();
                            request.ContentType = await reader.ReadInt16();

                            byte[] buffer = new byte[count - 11];
                            await reader.Read(buffer, 0, buffer.Length);
                            request.Content = buffer;
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
