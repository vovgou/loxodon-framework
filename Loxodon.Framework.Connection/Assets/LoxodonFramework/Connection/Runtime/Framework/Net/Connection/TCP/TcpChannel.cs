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
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Loxodon.Framework.Net.Connection
{
    public class TcpChannel : ChannelBase
    {
        private const int DEFAULT_TIMEOUT = 5000;
        protected readonly SemaphoreSlim connectLock = new SemaphoreSlim(1, 1);
        protected TcpClient client;
        protected AddressFamily family = AddressFamily.InterNetwork;
        protected bool adaptiveAddressFamily = true;
        public TcpChannel(IMessageDecoder<IMessage> decoder, IMessageEncoder<IMessage> encoder) : base(decoder, encoder)
        {
        }

        [Obsolete("Please move the handshake handler to the DefaultConnector.")]
        public TcpChannel(IMessageDecoder<IMessage> decoder, IMessageEncoder<IMessage> encoder, IHandshakeHandler handshakeHandler) : base(decoder, encoder, handshakeHandler)
        {
        }

        public TcpChannel(AddressFamily family, IMessageDecoder<IMessage> decoder, IMessageEncoder<IMessage> encoder) : base(decoder, encoder)
        {
            if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
                throw new ArgumentException("family");

            this.family = family;
            this.adaptiveAddressFamily = false;
        }

        [Obsolete("Please move the handshake handler to the DefaultConnector.")]
        public TcpChannel(AddressFamily family, IMessageDecoder<IMessage> decoder, IMessageEncoder<IMessage> encoder, IHandshakeHandler handshakeHandler) : base(decoder, encoder, handshakeHandler)
        {
            if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
                throw new ArgumentException("family");

            this.family = family;
            this.adaptiveAddressFamily = false;
        }

        public TcpChannel(ICodecFactory<IMessage> codecFactory) : base(codecFactory)
        {
        }

        [Obsolete("Please move the handshake handler to the DefaultConnector.")]
        public TcpChannel(ICodecFactory<IMessage> codecFactory, IHandshakeHandler handshakeHandler) : base(codecFactory, handshakeHandler)
        {
        }

        public TcpChannel(AddressFamily family, ICodecFactory<IMessage> codecFactory) : base(codecFactory)
        {
            if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
                throw new ArgumentException("family");

            this.family = family;
            this.adaptiveAddressFamily = false;
        }

        [Obsolete("Please move the handshake handler to the DefaultConnector.")]
        public TcpChannel(AddressFamily family, ICodecFactory<IMessage> codecFactory, IHandshakeHandler handshakeHandler) : base(codecFactory, handshakeHandler)
        {
            if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
                throw new ArgumentException("family");

            this.family = family;
            this.adaptiveAddressFamily = false;
        }

        public override bool Connected { get { return client != null ? client.Connected && connected : false; } }

        public override async Task Connect(string hostname, int port, int timeoutMilliseconds, CancellationToken cancellationToken)
        {
            if (timeoutMilliseconds <= 0)
                timeoutMilliseconds = DEFAULT_TIMEOUT;

            if (!await connectLock.WaitAsync(timeoutMilliseconds, cancellationToken).ConfigureAwait(false))
                throw new TimeoutException();
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

                    cancellationToken.ThrowIfCancellationRequested();

                    Exception lastex = null;
                    TcpClient ipv6Client = null;
                    TcpClient ipv4Client = null;
                    IPAddress nat64Address = null;
                    if ((adaptiveAddressFamily && Socket.OSSupportsIPv4) || family == AddressFamily.InterNetwork)
                    {
                        ipv4Client = new TcpClient(AddressFamily.InterNetwork);
                        ipv4Client.NoDelay = NoDelay;
                        ipv4Client.ReceiveBufferSize = ReceiveBufferSize;
                        ipv4Client.SendBufferSize = SendBufferSize;
                    }

                    if ((adaptiveAddressFamily && Socket.OSSupportsIPv6) || family == AddressFamily.InterNetworkV6)
                    {
                        ipv6Client = new TcpClient(AddressFamily.InterNetworkV6);
                        ipv6Client.NoDelay = NoDelay;
                        ipv6Client.ReceiveBufferSize = ReceiveBufferSize;
                        ipv6Client.SendBufferSize = SendBufferSize;

                        //Try to connect to the server through the NAT64 gateway
                        if (Regex.IsMatch(hostname, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                        {
                            nat64Address = IPAddress.Parse("64:ff9b::" + hostname);
                        }
                    }

                    foreach (var address in addresses)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        try
                        {
                            if (address.AddressFamily == AddressFamily.InterNetwork && ipv4Client != null)
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

                            if (address.AddressFamily == AddressFamily.InterNetworkV6 && ipv6Client != null)
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
                        }
                        catch (Exception ex)
                        {
                            if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                                throw;
                            lastex = ex;
                        }
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        if (nat64Address != null)
                        {
                            //Try to connect to the server through the NAT64 gateway
                            var result = ipv6Client.BeginConnect(nat64Address, port, null, null);
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
                    }
                    catch (Exception ex)
                    {
                        if (lastex == null)
                            lastex = ex;
                    }

                    if (ipv4Client != null)
                        ipv4Client.Close();
                    if (ipv6Client != null)
                        ipv6Client.Close();

                    if (lastex != null)
                        throw lastex;

                    throw new SocketException((int)SocketError.NotConnected);
                }, cancellationToken).ConfigureAwait(false);

                var stream = await this.WrapStream(client.GetStream()).ConfigureAwait(false);
                reader = new BinaryReader(stream, false, IsBigEndian);
                writer = new BinaryWriter(stream, false, IsBigEndian);

                if (this.codecFactory != null)
                {
                    this.decoder = this.codecFactory.CreateDecoder();
                    this.encoder = this.codecFactory.CreateEncoder();
                }

#pragma warning disable CS0618 
                if (handshakeHandler != null)
                    await handshakeHandler.OnHandshake(this).ConfigureAwait(false);
#pragma warning restore CS0618 

                cancellationToken.ThrowIfCancellationRequested();
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
            await connectLock.WaitAsync().ConfigureAwait(false);
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
                        await Task.Delay(delayTime).ConfigureAwait(false);
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
