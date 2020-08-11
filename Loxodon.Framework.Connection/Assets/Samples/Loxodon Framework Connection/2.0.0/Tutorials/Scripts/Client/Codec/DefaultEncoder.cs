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
using BinaryWriter = Loxodon.Framework.Net.Connection.BinaryWriter;

namespace Loxodon.Framework.Examples
{
    public class DefaultEncoder : IMessageEncoder<IMessage>
    {
        protected readonly SemaphoreSlim writeLock = new SemaphoreSlim(1, 1);
        protected uint sequence = 0;
        public async Task Encode(IMessage message, BinaryWriter writer)
        {
            await writeLock.WaitAsync();
            try
            {
                Notification notification = message as Notification;
                if (notification != null)
                {
                    //分配Sequence，通知类型如果服务器不校验序列号也可以不要
                    notification.Sequence = ++sequence;

                    byte[] buffer = notification.Content;
                    int count = buffer != null ? buffer.Length + 11 : 11;
                    writer.Write(count);
                    writer.Write((byte)notification.Type);
                    writer.Write(notification.CommandID);
                    writer.Write(notification.Sequence);
                    writer.Write(notification.ContentType);
                    if (buffer != null)
                        writer.Write(buffer, 0, buffer.Length);
                    await writer.FlushAsync();
                    return;
                }

                Request request = message as Request;
                if (request != null)
                {
                    //分配Sequence，服务器返回的Response的Sequence必须与请求配对
                    request.Sequence = ++sequence;

                    byte[] buffer = request.Content;
                    int count = buffer != null ? buffer.Length + 11 : 11;
                    writer.Write(count);
                    writer.Write((byte)request.Type);
                    writer.Write(request.CommandID);
                    writer.Write(request.Sequence);
                    writer.Write(request.ContentType);
                    if (buffer != null)
                        writer.Write(buffer, 0, buffer.Length);
                    await writer.FlushAsync();
                    return;
                }

                //客户端编码器不需要处理Response类型
                Response response = message as Response;
                if (response != null)
                {
                    byte[] buffer = response.Content;
                    int count = buffer != null ? buffer.Length + 11 : 11;
                    writer.Write(count);
                    writer.Write((byte)response.Type);
                    writer.Write(response.Status);
                    writer.Write(response.Sequence);
                    writer.Write(response.ContentType);
                    if (buffer != null)
                        writer.Write(buffer, 0, buffer.Length);
                    await writer.FlushAsync();
                    return;
                }

                throw new IOException();
            }
            finally
            {
                writeLock.Release();
            }
        }
    }
}
