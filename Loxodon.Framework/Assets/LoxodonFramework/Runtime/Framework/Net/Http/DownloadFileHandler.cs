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
using System.IO;
using System.Text;
using UnityEngine.Networking;

namespace Loxodon.Framework.Net.Http
{
    public class DownloadFileHandler : DownloadHandlerScript
    {
        private readonly FileInfo fileInfo;
        private readonly UnityWebRequest www;
        private readonly FileInfo downloadFileInfo;
        private readonly bool supportBreakpointResume;
        private FileStream downloadFileStream;
        private DownloadInfo downloadInfo;

        public DownloadFileHandler(string fileName) : this(null, new FileInfo(fileName))
        {
        }

        public DownloadFileHandler(UnityWebRequest www, string fileName) : this(www, new FileInfo(fileName))
        {
        }

        public DownloadFileHandler(FileInfo fileInfo) : this(null, fileInfo)
        {
        }

        public DownloadFileHandler(UnityWebRequest www, FileInfo fileInfo) : base(new byte[8192])
        {
            this.fileInfo = fileInfo;
            this.downloadFileInfo = new FileInfo(this.fileInfo.FullName + ".download");
            this.www = www;
            this.supportBreakpointResume = www != null;
            if (supportBreakpointResume && downloadFileInfo.Exists)
            {
                try
                {
                    this.downloadInfo = DownloadInfo.Read(downloadFileInfo);
                    if (this.downloadInfo != null)
                    {
                        if (!string.IsNullOrEmpty(downloadInfo.LastModified))
                            www.SetRequestHeader("If-Range", downloadInfo.LastModified);
                        if (!string.IsNullOrEmpty(downloadInfo.ETag))
                            www.SetRequestHeader("If-Range", downloadInfo.ETag);
                        www.SetRequestHeader("Range", "bytes=" + downloadInfo.DownloadedSize + "-");
                    }
                }
                catch (Exception)
                {
                    downloadFileInfo.Delete();
                }
            }

            if (downloadInfo == null && downloadFileInfo.Exists)
                downloadFileInfo.Delete();

            if (!downloadFileInfo.Directory.Exists)
                downloadFileInfo.Directory.Create();
        }

        private void CreateDownloadFile(DownloadInfo downloadInfo)
        {
            try
            {
                if (downloadFileInfo.Exists)
                    downloadFileInfo.Delete();

                if (!downloadFileInfo.Directory.Exists)
                    downloadFileInfo.Directory.Create();

                using (Stream stream = downloadFileInfo.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.SetLength(downloadInfo.FileSize + DownloadInfo.DOWNLOAD_INFO_OFFSET);
                    stream.Position = downloadInfo.FileSize;
                    downloadInfo.WriteTo(stream);
                }
            }
            catch (Exception e)
            {
                if (downloadFileInfo.Exists)
                    downloadFileInfo.Delete();
                if (www != null)
                    www.Abort();
                throw e;
            }
        }

        protected override byte[] GetData() { return null; }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || data.Length < 1)
                return false;

            if (supportBreakpointResume)
            {
                downloadFileStream.Position = downloadInfo.DownloadedSize;
                downloadFileStream.Write(data, 0, dataLength);
                downloadFileStream.Flush();

                downloadInfo.DownloadedSize += dataLength;

                downloadFileStream.Position = downloadInfo.FileSize;
                downloadInfo.WriteDownloadedTo(downloadFileStream);
                downloadFileStream.Flush();
            }
            else
            {
                downloadFileStream.Write(data, 0, dataLength);
                downloadFileStream.Flush();
                downloadInfo.DownloadedSize += dataLength;
            }
            return true;
        }

        public long TotalSize
        {
            get { return downloadInfo != null ? downloadInfo.FileSize : 0; }
        }

        public long DownloadedSize
        {
            get { return this.downloadInfo != null ? downloadInfo.DownloadedSize : 0; }
        }

        public float DownloadProgress
        {
            get { return GetProgress(); }
        }

        protected override float GetProgress()
        {
            if (this.downloadInfo == null)
                return 0;
            return downloadInfo.GetProgress();
        }

        protected override void CompleteContent()
        {
            FileInfo tmpFileInfo = null;
            try
            {
                if (downloadFileStream != null)
                {
                    downloadFileStream.Dispose();
                    downloadFileStream = null;
                }

                if (supportBreakpointResume)
                {
                    tmpFileInfo = new FileInfo(this.fileInfo.FullName + ".tmp");
                    if (tmpFileInfo.Exists)
                        tmpFileInfo.Delete();

                    File.Move(downloadFileInfo.FullName, tmpFileInfo.FullName);
                    using (Stream stream = tmpFileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        stream.SetLength(downloadInfo.FileSize);
                    }

                    if (fileInfo.Exists)
                        fileInfo.Delete();

                    File.Move(tmpFileInfo.FullName, fileInfo.FullName);
                    //tmpFileInfo.MoveTo(fileInfo.FullName);//This is a bug and this method may fail on the Android platform
                }
                else
                {
                    if (fileInfo.Exists)
                        fileInfo.Delete();

                    File.Move(downloadFileInfo.FullName, fileInfo.FullName);
                }
            }
            catch (Exception e)
            {
                SafeDelete(downloadFileInfo);
                SafeDelete(tmpFileInfo);
                SafeDelete(fileInfo);
                throw e;
            }
        }

        void SafeDelete(FileInfo file)
        {
            try
            {
                if (file.Exists)
                    file.Delete();
            }
            catch (Exception) { }
        }

#if UNITY_2019_3_OR_NEWER
        protected override void ReceiveContentLengthHeader(ulong contentLength)
#else
        protected override void ReceiveContentLength(int contentLength)
#endif
        {
            //On the IOS platform, this method is called multiple times, ensuring that only the first call is valid to avoid program errors.
            if (downloadFileStream != null)
                return;

            if (!supportBreakpointResume)
            {
                this.downloadInfo = new DownloadInfo();
                this.downloadInfo.DownloadedSize = 0;
                this.downloadInfo.FileSize = (int)contentLength;
                this.downloadFileStream = downloadFileInfo.Create();
                return;
            }

            if (www.responseCode == 200)//206 breakpoint resume.
            {
                this.downloadInfo = new DownloadInfo();
                this.downloadInfo.DownloadedSize = 0;
                this.downloadInfo.FileSize = (int)contentLength;
                this.downloadInfo.ETag = www.GetResponseHeader("ETag");
                this.downloadInfo.LastModified = www.GetResponseHeader("Last-Modified");
                this.CreateDownloadFile(this.downloadInfo);
            }

            this.downloadFileStream = downloadFileInfo.OpenWrite();
        }

#if UNITY_2021_3_OR_NEWER
        public override void Dispose()
        {
            if (downloadFileStream != null)
            {
                downloadFileStream.Dispose();
                downloadFileStream = null;
            }
            base.Dispose();
        }

#else
        ~DownloadFileHandler()
        {
            if (downloadFileStream != null)
            {
                downloadFileStream.Dispose();
                downloadFileStream = null;
            }
            this.Dispose();
        }
#endif

        class DownloadInfo
        {
            public const int DOWNLOAD_INFO_OFFSET = 128;
            public long FileSize { get; set; }
            public long DownloadedSize { get; set; }
            public string LastModified { get; set; }
            public string ETag { get; set; }

            private byte[] buffer = new byte[256];
            public static DownloadInfo Read(FileInfo fileInfo)
            {
                if (!fileInfo.Exists || fileInfo.Length <= DOWNLOAD_INFO_OFFSET)
                    return null;

                using (Stream stream = fileInfo.OpenRead())
                {
                    stream.Position = fileInfo.Length - DOWNLOAD_INFO_OFFSET;
                    return new DownloadInfo().ReadFrom(stream);
                }
            }

            public float GetProgress()
            {
                if (FileSize <= 0)
                    return 0;
                return (float)DownloadedSize / FileSize;
            }

            public DownloadInfo ReadFrom(Stream stream)
            {
                this.DownloadedSize = ReadLong(stream, buffer);
                this.FileSize = ReadLong(stream, buffer);
                this.LastModified = ReadString(stream, buffer);
                this.ETag = ReadString(stream, buffer);
                return this;
            }

            public DownloadInfo WriteTo(Stream stream)
            {
                Write(stream, buffer, DownloadedSize);
                Write(stream, buffer, FileSize);
                Write(stream, buffer, LastModified);
                Write(stream, buffer, ETag);
                return this;
            }

            public DownloadInfo WriteDownloadedTo(Stream stream)
            {
                Write(stream, buffer, DownloadedSize);
                return this;
            }

            public static void Write(Stream stream, byte[] buffer, int value)
            {
                buffer[0] = (byte)value;
                buffer[1] = (byte)(value >> 8);
                buffer[2] = (byte)(value >> 16);
                buffer[3] = (byte)(value >> 24);
                stream.Write(buffer, 0, 4);
            }

            public static void Write(Stream stream, byte[] buffer, long value)
            {
                buffer[0] = (byte)value;
                buffer[1] = (byte)(value >> 8);
                buffer[2] = (byte)(value >> 16);
                buffer[3] = (byte)(value >> 24);
                buffer[4] = (byte)(value >> 32);
                buffer[5] = (byte)(value >> 40);
                buffer[6] = (byte)(value >> 48);
                buffer[7] = (byte)(value >> 56);
                stream.Write(buffer, 0, 8);
            }

            public static void Write(Stream stream, byte[] buffer, string value)
            {
                int len = string.IsNullOrEmpty(value) ? 0 : Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, 2);
                buffer[0] = (byte)len;
                buffer[1] = (byte)(len >> 8);
                stream.Write(buffer, 0, len + 2);
            }

            public static long ReadLong(Stream stream, byte[] buffer)
            {
                stream.Read(buffer, 0, 8);
                uint lo = (uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
                uint hi = (uint)(buffer[4] | buffer[5] << 8 | buffer[6] << 16 | buffer[7] << 24);
                return ((long)hi) << 32 | lo;
            }

            public static int ReadInt(Stream stream, byte[] buffer)
            {
                stream.Read(buffer, 0, 4);
                return (buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
            }

            public static string ReadString(Stream stream, byte[] buffer)
            {
                stream.Read(buffer, 0, 2);
                int len = (buffer[0] | buffer[1] << 8);
                if (len <= 0)
                    return null;

                stream.Read(buffer, 0, len);
                return Encoding.UTF8.GetString(buffer, 0, len);
            }
        }
    }
}