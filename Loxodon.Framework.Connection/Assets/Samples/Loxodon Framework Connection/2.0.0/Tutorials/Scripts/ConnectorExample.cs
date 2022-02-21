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
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Loxodon.Framework.Examples
{
    public class ConnectorExample : MonoBehaviour
    {
        Server server;

        DefaultConnector<Request, Response, Notification> connector;
        ISubscription<EventArgs> eventSubscription;
        ISubscription<EventArgs> idleEventSubscription;
        ISubscription<Notification> messageSubscription;

        int port = 8000;
        void Start()
        {
            //初始化服务器
            server = new Server(port);

            //开启TLS加密，这是可选的，可用不设置
            TextAsset textAsset = Resources.Load<TextAsset>("vovgou.pfx");
            X509Certificate2 cert = new X509Certificate2(textAsset.bytes, "123456");
            server.Secure(true, cert, (sender, certificate, chain, sslPolicyErrors) =>
             {
                 //服务器设置不要求客户端证书，服务器方不校验客户端的协议，直接返回true
                 return true;
             });

            //----------------------

            //创建TcpChannel，TcpChannel中不要在插入HandshakeHandler，已经移到DefaultConnector类中
            //如果使用Kcp协议，此处替换为KcpChannel，针对Kcp的支持在Loxodon.Framework.Connection.KCP包中
            //var channel = new KcpChannel(new KcpSetting(), new CodecFactory());
            var channel = new TcpChannel(new DefaultDecoder(), new DefaultEncoder());
            channel.NoDelay = true;
            channel.IsBigEndian = true;//默认使用大端字节序，一般网络字节流用大端

            //如果服务器没有开启TLS加密，可用不设置
            channel.Secure(true, "vovgou.com", null, (sender, certificate, chain, sslPolicyErrors) =>
             {
                 //客户端方校验服务器端的自签名协议
                 if (sslPolicyErrors == SslPolicyErrors.None)
                     return true;

                 if (certificate != null && certificate.GetCertHashString() == "3C33D870E7826E9E83B4476D6A6122E497A6D282")
                     return true;

                 return false;
             });

            //HandshakeHandler 不要放在Channel中，请放入Connector中，这样更合理
            IHandshakeHandler handshakeHandler = new HandshakeHandler();

            //每个20秒空闲则触发空闲事件，并且每隔20秒触发一次,时间为0则关闭空闲事件，首次空闲First为true。示例中只开启了读写都空闲的事件
            IdleStateMonitor idleStateMonitor = new IdleStateMonitor(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(20f));
            connector = new DefaultConnector<Request, Response, Notification>(channel, idleStateMonitor, handshakeHandler);
            connector.AutoReconnect = true;//开启自动重连，只重连一次，失败后不再重试，建议使用心跳包保证连接可用

            //订阅事件,收到ConnectionEventArgs参数
            eventSubscription = connector.Events().Filter(e =>
            {
                //消息过滤，只订阅ConnectionEventArgs类型的事件
                //if (e is ConnectionEventArgs)
                //    return true;
                //return false;
                return true;
            }).ObserveOn(SynchronizationContext.Current).Subscribe((e) =>
            {
                Debug.LogFormat("Client Received Event:{0}", e);
            });

            //订阅通知
            //使用ObserveOn(SynchronizationContext.Current)切换消息处理线程为当前的UI线程
            messageSubscription = connector.Received().Filter(notification =>
            {
                //过滤消息，只监听CommandID在0-100之间的消息
                if (notification.CommandID > 0 && notification.CommandID <= 100)
                    return true;
                return false;
            }).ObserveOn(SynchronizationContext.Current).Subscribe(notification =>
            {
                Debug.LogFormat("Client Received Notification:{0}", notification);
            });

            //订阅连接空闲事件，发生心跳消息
            idleEventSubscription = connector.Events()
                .Filter(e => e is IdleStateEventArgs)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(e =>
                {
                    try
                    {
                        if (e is IdleStateEventArgs idleStateEventArgs)
                        {
                            if (idleStateEventArgs.IsFirst && (idleStateEventArgs.State == IdleState.ReaderIdle))
                            {
                                //send a ping message
                                SendHeartbeatMessage();
                            }
                        }
                    }
                    catch (Exception) { }
                });
        }

        async void Connect()
        {
            try
            {
                await connector.Connect("127.0.0.1", port, 1000);
                Debug.LogFormat("连接成功");
            }
            catch (Exception e)
            {
                Debug.LogFormat("连接异常：{0}", e);
            }
        }

        async void Send(Request request)
        {
            try
            {
                Response response = await connector.Send(request);
                Debug.LogFormat("The client received a response message successfully,Message:{0}", response);

            }
            catch (Exception e)
            {
                Debug.LogFormat("The client failed to send a request,Exception:{0}", e);
            }
        }

        async void Send(Notification notification)
        {
            try
            {
                await connector.Send(notification);
                Debug.LogFormat("The client has successfully sent a notification");
            }
            catch (Exception e)
            {
                Debug.LogFormat("The client failed to send a notification,Exception:{0}", e);
            }
        }

        async void SendHeartbeatMessage()
        {
            try
            {
                Request request = new Request();
                request.CommandID = 0;
                request.ContentType = 0;
                request.Content = Encoding.UTF8.GetBytes("ping");
                Response response = await connector.Send(request);
            }
            catch (TimeoutException e)
            {
                //Timeout
                if (connector.State == ConnectionState.Connected)
                    await connector.Reconnect();
            }
            catch (Exception e)
            {
                //Exception
                Debug.LogFormat("{0}", e);
            }
        }

        void OnGUI()
        {
            int x = 50;
            int y = 50;
            int width = 200;
            int height = 100;
            int i = 0;
            int padding = 10;

            GUI.skin.button.fontSize = 25;

            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), server.Started ? "Stop Server" : "Start Server"))
            {
                if (server.Started)
                    server.Stop();
                else
                    server.Start();
            }

            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), connector.Connected ? "Disconnect" : "Connect"))
            {
                if (connector.Connected)
                    _ = connector.Disconnect();
                else
                    Connect();
            }

            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), "Send Request"))
            {
                Request request = new Request();
                request.CommandID = 20;
                request.ContentType = 0;
                request.Content = Encoding.UTF8.GetBytes("this is a request.");
                Send(request);
            }

            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), "Send Notification"))
            {
                Notification notification = new Notification();
                notification.CommandID = 10;
                notification.ContentType = 0;
                notification.Content = Encoding.UTF8.GetBytes("this is a notification.");
                Send(notification);
            }
        }

        private void OnDestroy()
        {
            if (eventSubscription != null)
            {
                eventSubscription.Dispose();
                eventSubscription = null;
            }

            if (idleEventSubscription != null)
            {
                idleEventSubscription.Dispose();
                idleEventSubscription = null;
            }

            if (messageSubscription != null)
            {
                messageSubscription.Dispose();
                messageSubscription = null;
            }

            if (connector != null)
            {
                _ = connector.Shutdown();
                connector.Dispose();
                connector = null;
            }

            if (server != null)
            {
                server.Stop();
                server = null;
            }
        }
    }
}
