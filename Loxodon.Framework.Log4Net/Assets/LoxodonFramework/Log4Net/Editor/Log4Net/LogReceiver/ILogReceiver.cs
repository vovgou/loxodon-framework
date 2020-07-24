using System;
using Loxodon.Log.Log4Net;

namespace Loxodon.Log.Editors.Log4Net
{

    public delegate void MessageHandler(TerminalInfo terminalInfo, LoggingData loggingData);

    public interface ILogReceiver : System.IDisposable
    {
        event MessageHandler MessageReceived;

        bool Started { get; }

        void Start();

        void Stop();

    }

    [Serializable]
    public class TerminalInfo
    {
        private string id;
        private string name;
        private string ipAddress;
        private int port;

        public TerminalInfo(string name, string ipAddress, int port)
        {
            this.name = name;
            this.ipAddress = ipAddress;
            this.port = port;
            this.id = string.Format("{0}:{1}", ipAddress, port);
        }

        public string ID
        {
            get { return this.id; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string IPAddress
        {
            get { return this.ipAddress; }
        }

        public int Port
        {
            get { return this.port; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TerminalInfo))
                return false;

            TerminalInfo other = (TerminalInfo)obj;

            if (!string.Equals(this.id, other.id))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}:{2}]", this.name, this.ipAddress, this.port);
        }
    }
}
