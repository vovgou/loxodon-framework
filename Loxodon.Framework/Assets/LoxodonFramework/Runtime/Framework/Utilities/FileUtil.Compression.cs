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

#if UNITY_ANDROID && (CSHARP_ZIP || (NET_STANDARD_2_0 && !UNITY_2019_3_OR_NEWER) || (NET_STANDARD_2_0 && UNITY_2020_3_OR_NEWER) || UNITY_2021_2_OR_NEWER)
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Loxodon.Framework.Utilities
{
    public class CompressionZipAccessor : FileUtil.IZipAccessor
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnInitialized()
        {
            FileUtil.Register(new CompressionZipAccessor());
        }

        private object _lock = new object();
        private Dictionary<string, ZipArchive> zipArchives = new Dictionary<string, ZipArchive>();

        protected string GetCompressedFileName(string url)
        {
            url = Regex.Replace(url, @"^jar:file:///", "");
            return url.Substring(0, url.LastIndexOf("!"));
        }

        protected string GetCompressedEntryName(string url)
        {
            return url.Substring(url.LastIndexOf("!") + 2);
        }

        protected ZipArchive GetZipArchive(string path)
        {
            lock (_lock)
            {
                ZipArchive zip;
                if (this.zipArchives.TryGetValue(path, out zip))
                    return zip;

                zip = new ZipArchive(File.OpenRead(path));
                this.zipArchives.Add(path, zip);
                return zip;
            }
        }

        public int Priority { get { return 100; } }

        public bool Exists(string path)
        {
            try
            {
                var zipFilename = this.GetCompressedFileName(path);
                string entryName = GetCompressedEntryName(path);
                var zip = this.GetZipArchive(zipFilename);
                if (zip == null)
                    return false;
                if (zip.GetEntry(entryName) == null)
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Stream OpenRead(string path)
        {
            var zipFilename = this.GetCompressedFileName(path);
            string entryName = GetCompressedEntryName(path);
            var zip = this.GetZipArchive(zipFilename);
            if (zip == null)
                throw new FileNotFoundException(path);

            var entry = zip.GetEntry(entryName);
            if (entry == null)
                throw new FileNotFoundException(path);
            return entry.Open();
        }

        public bool Support(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            string fullname = path.ToLower();
            if ((fullname.IndexOf(".apk") > 0 || fullname.IndexOf(".obb") > 0) && fullname.LastIndexOf("!/assets/") > 0)
                return true;

            return false;
        }
    }
}
#endif