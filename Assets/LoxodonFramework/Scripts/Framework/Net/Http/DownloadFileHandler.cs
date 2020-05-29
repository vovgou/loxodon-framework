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

using System.IO;
using UnityEngine.Networking;

namespace Loxodon.Framework.Net.Http
{
    public class DownloadFileHandler : DownloadHandlerScript
    {
        private int totalSize = -1;
        private int completedSize = 0;
        private FileInfo fileInfo;
        private FileInfo tmpFileInfo;
        private FileStream fileStream;

        public DownloadFileHandler(string fileName) : this(new FileInfo(fileName))
        {
        }

        public DownloadFileHandler(FileInfo fileInfo) : base(new byte[8192])
        {
            this.fileInfo = fileInfo;
            this.tmpFileInfo = new FileInfo(this.fileInfo.FullName + ".tmp");
            if (this.tmpFileInfo.Exists)
                tmpFileInfo.Delete();

            if (!tmpFileInfo.Directory.Exists)
                tmpFileInfo.Directory.Create();

            this.fileStream = tmpFileInfo.Create();
        }

        protected override byte[] GetData() { return null; }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || data.Length < 1)
                return false;

            fileStream.Write(data, 0, dataLength);
            fileStream.Flush();
            completedSize += dataLength;
            return true;
        }

        protected override float GetProgress()
        {
            if (totalSize <= 0)
                return 0;
            return (float)completedSize / totalSize;
        }

        protected override void CompleteContent()
        {
            if (fileStream != null)
            {
                fileStream.Dispose();
                fileStream = null;
            }

            if (fileInfo.Exists)
                fileInfo.Delete();

            tmpFileInfo.MoveTo(fileInfo.FullName);
        }

        protected override void ReceiveContentLength(int contentLength)
        {
            this.totalSize = contentLength;
        }

        ~DownloadFileHandler()
        {
            if (fileStream != null)
            {
                fileStream.Dispose();
                fileStream = null;
            }
        }
    }
}
