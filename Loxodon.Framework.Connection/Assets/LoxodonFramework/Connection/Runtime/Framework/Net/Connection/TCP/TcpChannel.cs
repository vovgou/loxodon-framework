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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Loxodon.Framework.Net.Connection
{
    public class TcpChannel : ChannelBase
    {
        protected readonly SemaphoreSlim connectLock = new SemaphoreSlim(1, 1);
        protected TcpClient client;
        public TcpChannel(IMessageDecoder<IMessage> decoder, IMessageEncoder<IMessage> encoder) : this(decoder, encoder, null)
        {
        }

        public TcpChannel(IMessageDecoder<IMessage> decoder, IMessageEncoder<IMessage> encoder, IHandshakeHandler handshakeHandler) : base(decoder, encoder, handshakeHandler)
        {
        }

        public override bool Connected { get { return client != null ? client.Connected && connected : false; } }

        public override async Task Connect(string hostname, int port, int timeoutMilliseconds)
        {
            await connectLock.WaitAsync();
            try
            {
                if (client != null)
                {
                    client.Close();
                    client = null;
                }

                this.connected = false;
                client = await Task.Run(async () =>
                {
                    IPAddress[] addresses = await Dns.GetHostAddressesAsync(hostname);
                    List<IPAddress> ipv4Addresses = new List<IPAddress>();
                    List<IPAddress> ipv6Addresses = new List<IPAddress>();
                    foreach (IPAddress address in addresses)
                    {
                        if (address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipv4Addresses.Add(address);
                        }
                        else if (address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            ipv6Addresses.Add(address);
                        }
                    }

                    Exception lastex = null;
                    TcpClient ipv6Client = null;
                    TcpClient ipv4Client = null;
                    IPAddress nat64Address = null;
                    if (Socket.OSSupportsIPv4)
                    {
                        ipv4Client = new TcpClient(AddressFamily.InterNetwork);
                        ipv4Client.NoDelay = NoDelay;
                        ipv4Client.ReceiveBufferSize = ReceiveBufferSize;
                        ipv4Client.SendBufferSize = SendBufferSize;
                    }

                    if (Socket.OSSupportsIPv6)
                    {
                        ipv6Client = new TcpClient(AddressFamily.InterNetworkV6);
                        ipv6Client.NoDelay = NoDelay;
                        ipv6Client.ReceiveBufferSize = ReceiveBufferSize;
                        ipv6Client.SendBufferSize = SendBufferSize;

                        //Try to connect to the server through the NAT64 gateway
                        if (Regex.IsMatch(hostname, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                        {
                            nat64Address = IPAddress.Parse("64:ff9b::" + hostname);
                            ipv6Addresses.Add(nat64Address);
                        }
                    }

                    if (ipv4Client != null && ipv4Addresses.Count > 0)
                    {
                        foreach (var address in ipv4Addresses)
                        {
                            try
                            {
                                var result = ipv4Client.BeginConnect(address, port, null, null);
                                if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
                                {
                                    ipv4Client.EndConnect(result);

                                    if (ipv6Client != null)
                                        ipv6Client.Close();

                                    return ipv4Client;
                                }
                                else
                                {
                                    ipv4Client.Close();
                                    throw new SocketException((int)SocketError.TimedOut);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                                    throw;
                                lastex = ex;
                            }
                        }
                    }

                    if (ipv6Client != null && ipv6Addresses.Count > 0)
                    {
                        foreach (var address in ipv6Addresses)
                        {
                            try
                            {
                                var result = ipv6Client.BeginConnect(address, port, null, null);
                                if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
                                {
                                    ipv6Client.EndConnect(result);
                                    if (ipv4Client != null)
                                        ipv4Client.Close();
                                    return ipv6Client;
                                }
                                else
                                {
                                    ipv6Client.Close();
                                    throw new SocketException((int)SocketError.TimedOut);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                                    throw;

                                if (address != nat64Address)
                                    lastex = ex;
                            }
                        }
                    }

                    if (ipv4Client != null)
                        ipv4Client.Close();
                    if (ipv6Client != null)
                        ipv6Client.Close();

                    if (lastex != null)
                        throw lastex;

                    throw new SocketException((int)SocketError.NotConnected);
                });

                var stream = await this.WrapStream(client.GetStream());
                reader = new BinaryReader(stream, false, IsBigEndian);
                writer = new BinaryWriter(stream, false, IsBigEndian);

                if (handshakeHandler != null)
                    await handshakeHandler.OnHandshake(this);
                this.connected = true;
            }
            catch (Exception)
            {
                if (client != null)
                {
                    client.Close();
                    client = null;
                }

                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }

                if (writer != null)
                {
                    writer.Dispose();
                    writer = null;
                }
                throw;
            }
            finally
            {
                connectLock.Release();
            }
        }

        public override async Task Close()
        {
            await connectLock.WaitAsync();
            try
            {
                if (client != null)
                {
                    this.connected = false;
                    int delayTime = 0;
                    var state = client.LingerState;
                    if (state != null && state.Enabled)
                        delayTime = state.LingerTime;

                    client.Close();
                    client.Dispose();
                    client = null;

                    if (delayTime > 0)
                        await Task.Delay(delayTime);
                }

                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }

                if (writer != null)
                {
                    writer.Dispose();
                    writer = null;
                }
            }
            finally
            {
                connectLock.Release();
            }
        }
    }
}
