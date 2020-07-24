using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Loxodon.Log.Log4Net;

namespace Loxodon.Log.Editors.Log4Net
{
    public class UdpLogReceiver : ILogReceiver
    {
        private bool started;

        private int port;

        private UdpClient client;

        private BinaryFormatter formatter = new BinaryFormatter();

        private Dictionary<string, TerminalInfo> terminalInfos = new Dictionary<string, TerminalInfo>();

        public event MessageHandler MessageReceived;

        public bool Started
        {
            get { return this.started; }
            protected set { this.started = value; }
        }

        public int Port
        {
            get { return this.port; }
        }

        public UdpLogReceiver(int port)
        {
            this.port = port;

        }

        public IPEndPoint LocalEndPoint
        {
            get { return (IPEndPoint)client.Client.LocalEndPoint; }
        }

        public virtual void Start()
        {
            try
            {
                client = new UdpClient(this.port);
                client.BeginReceive(ReceiveUdpMessage, client);
                this.Started = true;
            }
            catch (Exception e)
            {
                this.Started = false;
                throw e;
            }
        }

        public virtual void Stop()
        {
            try
            {
                if (client != null)
                {
                    client.Close();
                    client = null;
                }
                this.Started = false;
            }
            catch (Exception) { }
        }

        private void ReceiveUdpMessage(IAsyncResult result)
        {
            UdpClient client = (UdpClient)result.AsyncState;
            if (client == null)
                return;

            try
            {
                var remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
                Byte[] buffer = client.EndReceive(result, ref remoteIPEndPoint);
                if (buffer != null)
                {
                    LoggingData loggingData = (LoggingData)formatter.Deserialize(new MemoryStream(buffer));
                    try
                    {
                        if (this.MessageReceived != null)
                        {
                            TerminalInfo terminalInfo;
                            string key = remoteIPEndPoint.ToString();
                            if (!this.terminalInfos.TryGetValue(key, out terminalInfo))
                            {
                                terminalInfo = new TerminalInfo(loggingData.UserName, remoteIPEndPoint.Address.ToString(), remoteIPEndPoint.Port);
                                this.terminalInfos.Add(key, terminalInfo);
                            }

                            this.MessageReceived(terminalInfo, loggingData);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

            client.BeginReceive(ReceiveUdpMessage, result.AsyncState);
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                this.Stop();
                disposed = true;
            }
        }

        ~UdpLogReceiver()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
