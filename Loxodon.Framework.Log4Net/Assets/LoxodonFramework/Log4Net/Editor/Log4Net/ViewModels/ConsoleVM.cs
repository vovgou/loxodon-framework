using System;
using UnityEngine;
using System.Collections.Generic;
using Loxodon.Log.Log4Net;
using UnityEditor;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Loxodon.Log.Editors.Log4Net
{
    [Serializable]
    public class ConsoleVM
    {
        private static readonly LoggingContainer EMPTY_PAGE = new LoggingContainer(null, 0);

        private const string LEVEL_MASK_KEY = "Loxodon::LOG::LOG4NET::LEVEL";
        private const string COLUMN_MASK_KEY = "Loxodon::LOG::LOG4NET::COLUMN";
        private const string COLLAPSE_SHOW_KEY = "Loxodon::LOG::LOG4NET::COLLAPSE";
        private const string PORT_KEY = "Loxodon::LOG::LOG4NET::PORT";
        private const string PLAY_STATE_KEY = "Loxodon::LOG::LOG4NET::PLAY_STATE";
        private const string LAST_SAVE_DIR_KEY = "Loxodon::LOG::LOG4NET::LAST_DIR";

        private int levelMask = 31;
        private int columnMask = 7;
        private string filterText;
        private bool collapse;
        private int maxCapacity = 10000;
        private int port = 8085;
        private bool playState = false;
        private int currentIndex = -1;
        private List<TerminalInfo> terminalInfos = new List<TerminalInfo>();
        private List<LoggingContainer> containers = new List<LoggingContainer>();
        private string lastSaveDir = "";

        [NonSerialized]
        private object _lock = new object();
        [NonSerialized]
        private ILogReceiver receiver;

        public ConsoleVM()
        {
        }

        public void OnEnable()
        {
            try
            {
                this.levelMask = EditorPrefs.GetInt(LEVEL_MASK_KEY, 31);
                this.columnMask = EditorPrefs.GetInt(COLUMN_MASK_KEY, 7);
                this.collapse = EditorPrefs.GetBool(COLLAPSE_SHOW_KEY, false);
                this.port = EditorPrefs.GetInt(PORT_KEY, 8085);
                this.playState = EditorPrefs.GetBool(PLAY_STATE_KEY, false);
                this.lastSaveDir = EditorPrefs.GetString(LAST_SAVE_DIR_KEY, "");

                if (this.PlayState)
                    this.Start();

            }
            catch (Exception)
            {
            }
        }

        public void OnDisable()
        {
        }

        public int MaxCapacity
        {
            get { return this.maxCapacity; }
            set { this.maxCapacity = value; }
        }

        public bool IsLevelShow(Level level)
        {
            switch (level)
            {
                case Level.DEBUG:
                    return (levelMask & 1) > 0;
                case Level.INFO:
                    return (levelMask & 2) > 0;
                case Level.WARN:
                    return (levelMask & 4) > 0;
                case Level.ERROR:
                    return (levelMask & 8) > 0;
                case Level.FATAL:
                    return (levelMask & 16) > 0;
                default:
                    return true;
            }
        }

        public void SetLevelShow(Level level, bool show)
        {
            switch (level)
            {
                case Level.DEBUG:
                    if (show)
                        levelMask |= 1;
                    else
                        levelMask &= ~1;
                    break;
                case Level.INFO:
                    if (show)
                        levelMask |= 2;
                    else
                        levelMask &= ~2;
                    break;
                case Level.WARN:
                    if (show)
                        levelMask |= 4;
                    else
                        levelMask &= ~4;
                    break;
                case Level.ERROR:
                    if (show)
                        levelMask |= 8;
                    else
                        levelMask &= ~8;
                    break;
                case Level.FATAL:
                    if (show)
                        levelMask |= 16;
                    else
                        levelMask &= ~16;
                    break;
                default:
                    break;
            }

            EditorPrefs.SetInt(LEVEL_MASK_KEY, this.levelMask);
        }

        public bool IsColumnShow(Columns column)
        {
            switch (column)
            {
                case Columns.TimeStamp:
                    return (columnMask & 1) > 0;
                case Columns.Thread:
                    return (columnMask & 2) > 0;
                case Columns.Logger:
                    return (columnMask & 4) > 0;
                default:
                    return true;
            }
        }

        public void SetColumnShow(Columns column, bool show)
        {
            switch (column)
            {
                case Columns.TimeStamp:
                    if (show)
                        columnMask |= 1;
                    else
                        columnMask &= ~1;
                    break;
                case Columns.Thread:
                    if (show)
                        columnMask |= 2;
                    else
                        columnMask &= ~2;
                    break;
                case Columns.Logger:
                    if (show)
                        columnMask |= 4;
                    else
                        columnMask &= ~4;
                    break;
                default:
                    break;
            }
            EditorPrefs.SetInt(COLUMN_MASK_KEY, this.columnMask);
        }

        public bool Collapse
        {
            get { return this.collapse; }
            set
            {
                if (this.collapse == value)
                    return;

                this.collapse = value;
                EditorPrefs.SetBool(COLLAPSE_SHOW_KEY, this.collapse);
            }
        }

        public string FilterText
        {
            get { return this.filterText; }
            set { this.filterText = value; }
        }

        public TerminalInfo CurrentTerminalInfo
        {
            get { return currentIndex >= 0 && currentIndex < this.terminalInfos.Count ? this.terminalInfos[currentIndex] : null; }
        }

        public bool PlayState
        {
            get { return this.playState; }
            set
            {
                if (this.playState == value)
                    return;
                this.playState = value;
                EditorPrefs.SetBool(PLAY_STATE_KEY, value);
            }
        }

        public bool Started
        {
            get { return this.receiver == null ? false : this.receiver.Started; }
        }

        public int Port
        {
            get { return this.port; }
            set
            {
                if (this.port == value)
                    return;

                this.port = value;
                EditorPrefs.SetInt(PORT_KEY, value);
            }
        }

        public int CurrentIndex
        {
            get { return this.currentIndex; }
            set { this.currentIndex = value; }
        }

        public List<TerminalInfo> TerminalInfos
        {
            get { return this.terminalInfos; }
        }

        public IPAddress GetLocalIPAddress()
        {
            string name = Dns.GetHostName();
            foreach (IPAddress ipa in Dns.GetHostAddresses(name))
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                    return ipa;
            }
            return IPAddress.Loopback;
        }

        public virtual void Start()
        {
            try
            {
                lock (_lock)
                {
                    if (receiver != null)
                    {
                        receiver.MessageReceived -= OnMesageReceived;
                        receiver.Stop();
                        receiver = null;
                    }

                    receiver = new UdpLogReceiver(this.Port);
                    receiver.MessageReceived += OnMesageReceived;
                    receiver.Start();
                }
            }
            catch (Exception e)
            {
                this.playState = false;
                if (receiver != null)
                    receiver.MessageReceived -= OnMesageReceived;

                Debug.LogError(e);
            }
        }

        public virtual void Stop()
        {
            try
            {
                lock (_lock)
                {
                    if (receiver != null)
                    {
                        receiver.MessageReceived -= OnMesageReceived;
                        receiver.Stop();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void OnMesageReceived(TerminalInfo terminalInfo, LoggingData loggingData)
        {
            this.AddLoggingData(terminalInfo, loggingData);
        }

        void AddLoggingData(TerminalInfo terminalInfo, LoggingData loggingData)
        {
            lock (_lock)
            {
                LoggingContainer container = null;
                int index = this.terminalInfos.IndexOf(terminalInfo);
                if (index < 0)
                {
                    this.terminalInfos.Add(terminalInfo);
                    container = new LoggingContainer(terminalInfo, this.maxCapacity);
                    this.containers.Add(container);
                    if (this.currentIndex < 0 || this.currentIndex >= this.terminalInfos.Count)
                        this.currentIndex = 0;
                }
                else {
                    container = this.containers[index];
                }

                container.Add(loggingData);
            }
        }

        public void ClearLoggingData()
        {
            lock (_lock)
            {
                if (this.currentIndex < 0 || this.currentIndex >= this.terminalInfos.Count)
                    return;

                this.terminalInfos.RemoveAt(currentIndex);
                this.containers.RemoveAt(currentIndex);

                if (this.currentIndex >= this.terminalInfos.Count)
                    this.currentIndex -= 1;
            }
        }

        public void SaveLoggingData()
        {
            LoggingContainer container = this.GetCurrentContainer();
            var list = container.GetLoggingList();

            string filename = string.Format("log-{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
            string location = EditorUtility.SaveFilePanel("Save", lastSaveDir, filename, "txt");
            if (string.IsNullOrEmpty(location))
                return;

            FileInfo file = new FileInfo(location);
            DirectoryInfo dir = file.Directory;
            if (!dir.Exists)
                dir.Create();

            this.lastSaveDir = dir.FullName;

            StringBuilder buf = new StringBuilder();
            foreach (LoggingEntry entry in list)
            {
                foreach (LoggingData data in entry.LoggingDatas)
                {
                    buf.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}", data.TimeStamp);
                    buf.AppendFormat(" Thread[{0}]", data.ThreadName);
                    buf.AppendFormat(" {0}", data.Level.ToString());
                    buf.AppendFormat(" {0}", data.LoggerName);
                    buf.AppendFormat(" - {0}", data.Message);
                    buf.Append("\r\n");

                    if (data.LocationInfo != null && data.LocationInfo.StackFrames != null)
                    {
                        foreach (var frame in data.LocationInfo.StackFrames)
                        {
                            buf.Append(frame.FullInfo.Replace(Directory.GetCurrentDirectory().ToString() + @"\", "")).Append("\r\n");
                        }
                    }

                    buf.Append("\r\n");
                }
            }

            File.WriteAllText(location, buf.ToString());
        }

        public LoggingContainer GetCurrentContainer()
        {
            lock (_lock)
            {
                if (this.currentIndex < 0 || this.currentIndex >= this.terminalInfos.Count)
                    return EMPTY_PAGE;

                return this.containers[currentIndex];
            }
        }
    }

    [Serializable]
    public class LoggingContainer
    {
        private TerminalInfo terminalInfo;
        private int capacity = 10000;
        private List<LoggingEntry> loggings = new List<LoggingEntry>();
        private int[] counters = new int[5];

        public LoggingContainer(TerminalInfo terminalInfo, int capacity)
        {
            this.terminalInfo = terminalInfo;
            this.capacity = capacity;
        }

        public TerminalInfo TerminalInfo
        {
            get { return this.terminalInfo; }
        }

        public int Capacity
        {
            get { return this.capacity; }
            set { this.capacity = value; }
        }

        public int GetCount(Level level)
        {
            switch (level)
            {
                case Level.DEBUG:
                    return counters[0];
                case Level.INFO:
                    return counters[1];
                case Level.WARN:
                    return counters[2];
                case Level.ERROR:
                    return counters[3];
                case Level.FATAL:
                    return counters[4];
                default:
                    return counters[0];
            }
        }

        private void UpdateCount(Level level, int value)
        {
            switch (level)
            {
                case Level.DEBUG:
                    counters[0] += value;
                    break;
                case Level.INFO:
                    counters[1] += value;
                    break;
                case Level.WARN:
                    counters[2] += value;
                    break;
                case Level.ERROR:
                    counters[3] += value;
                    break;
                case Level.FATAL:
                    counters[4] += value;
                    break;
                default:
                    break;
            }
        }

        public void Add(LoggingData loggingData)
        {
            lock (this.loggings)
            {
                LoggingEntry last = loggings.Count > 0 ? loggings[loggings.Count - 1] : null;
                if (last != null && last.IsMatch(loggingData))
                {
                    last.Add(loggingData);
                    this.UpdateCount(last.Level, 1);
                }
                else {
                    LoggingEntry logging = new LoggingEntry(loggingData);
                    loggings.Add(logging);
                    this.UpdateCount(logging.Level, 1);
                }

                if (loggings.Count > this.capacity)
                {
                    LoggingEntry oldest = loggings[0];
                    this.UpdateCount(oldest.Level, -1);
                    if (oldest.Count <= 1)
                        loggings.RemoveAt(0);
                    else
                        oldest.RemoveAt(0);
                }
            }
        }

        public void Clear()
        {
            lock (loggings)
            {
                this.counters = new int[5];
                this.loggings.Clear();
            }
        }

        public List<LoggingEntry> GetLoggingList()
        {
            lock (loggings)
            {
                return new List<LoggingEntry>(this.loggings);
            }
        }
    }

    [Serializable]
    public class LoggingEntry
    {
        private List<LoggingData> loggingDatas = new List<LoggingData>();

        public LoggingEntry(LoggingData loggingData)
        {
            loggingDatas.Add(loggingData);
        }

        public List<LoggingData> LoggingDatas
        {
            get { return this.loggingDatas; }
        }

        public LoggingData LoggingData
        {
            get { return this.loggingDatas[0]; }
        }

        public Level Level
        {
            get { return loggingDatas[0].Level; }
        }

        public string Message
        {
            get { return loggingDatas[0].Message; }
        }

        public DateTime TimeStamp
        {
            get { return loggingDatas[0].TimeStamp; }
        }

        public string ThreadName
        {
            get { return loggingDatas[0].ThreadName; }
        }

        public string LoggerName
        {
            get { return loggingDatas[0].LoggerName; }
        }

        public string UserName
        {
            get { return loggingDatas[0].UserName; }
        }

        public Log.Log4Net.LocationInfo LocationInfo
        {
            get { return loggingDatas[0].LocationInfo; }
        }

        public int Count
        {
            get { return this.loggingDatas.Count; }
        }

        public bool IsMatch(LoggingData loggindData)
        {
            return loggindData.Level == this.LoggingData.Level && string.Equals(this.LoggingData.Message, loggindData.Message);
        }

        public void Add(LoggingData loggingData)
        {
            this.loggingDatas.Add(loggingData);
        }

        public void RemoveAt(int index)
        {
            this.loggingDatas.RemoveAt(index);
        }
    }

    [Serializable]
    public enum Columns
    {
        TimeStamp = 0,
        Thread = 1,
        Logger = 2
    }

}