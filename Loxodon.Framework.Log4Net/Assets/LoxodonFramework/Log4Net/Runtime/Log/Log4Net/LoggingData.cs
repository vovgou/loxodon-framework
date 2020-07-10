using System;
using UnityEngine;
using System.Collections.Generic;

namespace Loxodon.Log.Log4Net
{
    [Serializable]
    public class LoggingData : ISerializationCallbackReceiver
    {
        private string identity;
        private string loggerName;
        private Level level;
        private string message;
        private string threadName;
        private LocationInfo locationInfo;
        private string userName;
        private string exceptionString;
        private string domain;
        private long timeStampTicks;
        private DateTime timeStamp;


        public LoggingData(log4net.Core.LoggingEventData data)
        {
            this.identity = data.Identity;
            this.loggerName = data.LoggerName;
            this.level = Level.DEBUG;
            this.message = data.Message;
            this.threadName = data.ThreadName;
            this.timeStampTicks = data.TimeStamp.Ticks;
            this.locationInfo = new LocationInfo(data.LocationInfo);
            this.exceptionString = data.ExceptionString;
            this.domain = data.Domain;
            this.level = ConvertLevel(data.Level.Value);
            this.timeStamp = new DateTime(this.timeStampTicks);
            this.userName = GetDeviceName();
        }

        private Level ConvertLevel(int value)
        {
            if (value <= 30000)
                return Level.DEBUG;
            if (value <= 40000)
                return Level.INFO;
            if (value <= 60000)
                return Level.WARN;
            if (value <= 70000)
                return Level.ERROR;
            return Level.FATAL;
        }

        private string GetDeviceName()
        {
            try
            {
                var deviceName = SystemInfo.deviceName;
                if (!string.IsNullOrEmpty(deviceName) && !deviceName.Contains("unknown"))
                    return deviceName;

                deviceName = SystemInfo.deviceModel;
                if (!string.IsNullOrEmpty(deviceName) && !deviceName.Contains("unknown"))
                    return deviceName;

                deviceName = SystemInfo.operatingSystem;
                if (!string.IsNullOrEmpty(deviceName) && !deviceName.Contains("unknown"))
                    return deviceName;

                deviceName = SystemInfo.graphicsDeviceName;
                if (!string.IsNullOrEmpty(deviceName) && !deviceName.Contains("unknown"))
                    return deviceName;

                deviceName = SystemInfo.processorType;
                if (!string.IsNullOrEmpty(deviceName) && !deviceName.Contains("unknown"))
                    return deviceName;
                return "<unknown>";
            }
            catch (Exception)
            {
                return "<unknown>";
            }
        }

        public void OnBeforeSerialize()
        {
            this.timeStampTicks = this.timeStamp.Ticks;
        }

        public void OnAfterDeserialize()
        {
            this.timeStamp = new DateTime(this.timeStampTicks);
        }

        public string Identity
        {
            get { return this.identity; }
        }

        public string LoggerName
        {
            get { return this.loggerName; }
        }

        public Level Level
        {
            get { return this.level; }
        }

        public string Message
        {
            get { return this.message; }
        }

        public string ThreadName
        {
            get { return this.threadName; }
        }

        public DateTime TimeStamp
        {
            get { return this.timeStamp; }
        }

        public LocationInfo LocationInfo
        {
            get { return this.locationInfo; }
        }

        public string UserName
        {
            get { return this.userName; }
        }

        public string ExceptionString
        {
            get { return this.exceptionString; }
        }

        public string Domain
        {
            get { return this.domain; }
        }
    }

    [Serializable]
    public class LocationInfo
    {
        private string className;
        private string fileName;
        private int lineNumber;
        private string methodName;
        private string fullInfo;
        private StackFrameItem[] stackFrames;

        public LocationInfo(log4net.Core.LocationInfo locationInfo)
        {
            this.className = locationInfo.ClassName;
            this.fileName = locationInfo.FileName;
            if (!int.TryParse(locationInfo.LineNumber, out this.lineNumber))
                this.lineNumber = 0;
            this.methodName = locationInfo.MethodName;

            if (string.IsNullOrEmpty(this.fileName))
                this.fullInfo = this.className + '.' + this.methodName + "()";
            else
                this.fullInfo = this.className + '.' + this.methodName + '(' + this.fileName + ':' + this.lineNumber + ')';

            var frames = locationInfo.StackFrames;
            if (frames == null)
                return;

            List<StackFrameItem> list = new List<StackFrameItem>();
            foreach (var frame in frames)
            {
                try
                {
                    if (frame == null)
                        continue;

                    var method = frame.Method;
                    string methodName = method.Name;
                    string className = frame.ClassName;
                    StackFrameItem item = new StackFrameItem(className, methodName, frame.FileName, int.Parse(frame.LineNumber));
                    list.Add(item);
                }
                catch (Exception) { }
            }

            this.stackFrames = list.ToArray();
        }

        public LocationInfo(string className, string methodName, string fileName, int lineNumber)
        {
            this.className = className;
            this.fileName = fileName;
            this.lineNumber = lineNumber;
            this.methodName = methodName;
            this.fullInfo = this.className + '.' + this.methodName + '(' + this.fileName +
                ':' + this.lineNumber + ')';
        }

        public string ClassName
        {
            get { return className; }
        }

        public string FileName
        {
            get { return fileName; }
        }

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public string MethodName
        {
            get { return methodName; }
        }

        public string FullInfo
        {
            get { return fullInfo; }
        }

        public StackFrameItem[] StackFrames
        {
            get { return stackFrames; }
        }

    }

    [Serializable]
    public class StackFrameItem
    {
        private int lineNumber;
        private string fileName;
        private string className;
        private string fullInfo;
        private string methodName;

        public StackFrameItem(string className, string methodName, string fileName, int lineNumber)
        {
            this.className = className;
            this.methodName = methodName;
            this.fileName = fileName;
            this.lineNumber = lineNumber;

            if (string.IsNullOrEmpty(this.fileName))
                this.fullInfo = this.className + '.' + this.methodName + "()";
            else
                this.fullInfo = this.className + '.' + this.methodName + '(' + this.fileName + ':' + this.lineNumber + ')';

        }

        public string ClassName
        {
            get { return className; }
        }

        public string FileName
        {
            get { return fileName; }
        }

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public string MethodName
        {
            get { return methodName; }
        }

        public string FullInfo
        {
            get { return fullInfo; }
        }
    }
}