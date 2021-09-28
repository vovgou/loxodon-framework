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
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using BinaryReader = Loxodon.Framework.Net.Connection.BinaryReader;
using BinaryWriter = Loxodon.Framework.Net.Connection.BinaryWriter;

namespace Loxodon.Framework.Examples
{
    public class Server
    {
        TcpListener tcpListener;
        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;

        bool secure;
        X509Certificate cert;
        RemoteCertificateValidationCallback remoteCertificateValidationCallback;
        List<TcpClient> clients = new List<TcpClient>();
        bool started = false;
        int port;
        public Server(int port)
        {
            this.port = port;
        }

        public bool Started { get { return started; } }

        public void Secure(bool secure, X509Certificate cert = null)
        {
            Secure(secure, cert, null);
        }

        public void Secure(bool secure, X509Certificate cert, RemoteCertificateValidationCallback remoteCertificateValidationCallback)
        {
            this.secure = secure;
            this.cert = cert;
            this.remoteCertificateValidationCallback = remoteCertificateValidationCallback;
        }

        public void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

            tcpListener = TcpListener.Create(port);
            tcpListener.Start();
            started = true;
            Task.Run(() =>
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    TcpClient client = tcpListener.AcceptTcpClient();
                    Work(client, cancellationToken);
                }
            }, cancellationToken);
        }

        protected virtual void Work(TcpClient client, CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                try
                {
                    clients.Add(client);
                    Stream stream = null;
                    if (secure)
                    {
                        try
                        {
                            SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateClientCertificate));
                            await sslStream.AuthenticateAsServerAsync(cert, false, SslProtocols.Tls | SslProtocols.Ssl3 | SslProtocols.Ssl2, false);
                            stream = sslStream;
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogErrorFormat("AuthenticateAsServerAsync,Exception:{0}", e);
                            throw;
                        }
                    }
                    else
                    {
                        stream = client.GetStream();
                    }

                    BinaryReader reader = new BinaryReader(stream, false);
                    BinaryWriter writer = new BinaryWriter(stream, false);

                    //这里服务器共用了客户端的编码解码器
                    DefaultDecoder decoder = new DefaultDecoder();
                    DefaultEncoder encoder = new DefaultEncoder();

                    while (true)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            cancellationToken.ThrowIfCancellationRequested();

                        //读取一条消息
                        var message = await decoder.Decode(reader);

                        Request request = message as Request;
                        if (request != null)
                        {
                            //收到请求，返回一个响应消息
                            Debug.LogFormat("Server Received:{0}", request.ToString());

                            Response response = new Response();
                            response.Status = 200;
                            response.Sequence = request.Sequence;//必须与请求配对
                            response.ContentType = 0;

                            if(request.CommandID==0)
                                response.Content = Encoding.UTF8.GetBytes("pong");
                            else
                                response.Content = Encoding.UTF8.GetBytes("The server responds to the client");

                            //写入一条消息
                            await encoder.Encode(response, writer);
                            Debug.LogFormat("Server Sent:{0}", response.ToString());
                            continue;
                        }

                        Notification notification = message as Notification;
                        if (notification != null)
                        {
                            //收到通知，返回一个通知消息
                            Debug.LogFormat("Server Received:{0}", notification.ToString());

                            Notification response = new Notification();
                            response.CommandID = 11;
                            response.ContentType = 0;
                            response.Content = Encoding.UTF8.GetBytes("The server sends a notification to the client");

                            //写入一条消息
                            await encoder.Encode(response, writer);
                            Debug.LogFormat("Server Sent:{0}", response.ToString());
                            continue;
                        }
                    }
                }
                finally
                {
                    if (client != null)
                    {
                        clients.Remove(client);
                        client.Close();
                    }
                }

            }, cancellationToken);
        }

        protected virtual bool ValidateClientCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (remoteCertificateValidationCallback != null)
                return remoteCertificateValidationCallback(sender, certificate, chain, sslPolicyErrors);

            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if (certificate == null)
                return true;

            return false;
        }


        public void Stop()
        {
            started = false;
            if (tcpListener != null)
            {
                tcpListener.Stop();
                tcpListener = null;
            }
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;
            }

            if (clients != null)
            {
                foreach (var client in clients)
                {
                    client.Close();
                }
            }
        }
    }
}
