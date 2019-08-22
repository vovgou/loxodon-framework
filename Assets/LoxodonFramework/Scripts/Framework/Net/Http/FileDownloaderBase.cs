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

using Loxodon.Framework.Asynchronous;
using System;
using System.IO;
using UnityEngine;

namespace Loxodon.Framework.Net.Http
{
    public abstract class FileDownloaderBase : IFileDownloader
    {
        private Uri baseUri;
        private int maxTaskCount;

        public FileDownloaderBase() : this(null, SystemInfo.processorCount * 2)
        {
        }

        public FileDownloaderBase(Uri baseUri, int maxTaskCount)
        {
            this.BaseUri = baseUri;
            this.MaxTaskCount = maxTaskCount;
        }

        public virtual Uri BaseUri
        {
            get { return this.baseUri; }
            set
            {
                if (value != null && !this.IsAllowedAbsoluteUri(value))
                    throw new NotSupportedException(string.Format("Invalid uri:{0}", value == null ? "null" : value.OriginalString));

                this.baseUri = value;
            }
        }

        public virtual int MaxTaskCount
        {
            get { return this.maxTaskCount; }
            set { this.maxTaskCount = Mathf.Max(value > 0 ? value : SystemInfo.processorCount * 2, 1); }
        }

        protected virtual bool IsAllowedAbsoluteUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                return false;

            if ("http".Equals(uri.Scheme) || "https".Equals(uri.Scheme) || "ftp".Equals(uri.Scheme))
                return true;

            if ("file".Equals(uri.Scheme) && uri.OriginalString.IndexOf("jar:") < 0)
                return true;

            return false;
        }

        protected virtual Uri GetAbsoluteUri(Uri relativePath)
        {
            if (this.baseUri == null || this.IsAllowedAbsoluteUri(relativePath))
                return relativePath;

            return new Uri(this.baseUri, relativePath);
        }

        public virtual IProgressResult<ProgressInfo, FileInfo> DownloadFileAsync(Uri path, string fileName)
        {
            return DownloadFileAsync(path, new FileInfo(fileName));
        }

        public abstract IProgressResult<ProgressInfo, FileInfo> DownloadFileAsync(Uri path, FileInfo fileInfo);

        public abstract IProgressResult<ProgressInfo, ResourceInfo[]> DownloadFileAsync(ResourceInfo[] infos);
    }
}
