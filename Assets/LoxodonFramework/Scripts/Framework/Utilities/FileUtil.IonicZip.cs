#if UNITY_ANDROID && IONIC_ZIP
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Loxodon.Framework.Utilities
{
    public class IonicZipAccessor : FileUtil.IZipAccessor
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnInitialized()
        {
            FileUtil.Register(new IonicZipAccessor());
        }

        private object _lock = new object();
        private Dictionary<string, ZipFile> zipArchives = new Dictionary<string, ZipFile>();

        protected string GetCompressedFileName(string url)
        {
            url = Regex.Replace(url, @"^jar:file:///", "");
            return url.Substring(0, url.LastIndexOf("!"));
        }

        protected string GetCompressedEntryName(string url)
        {
            return url.Substring(url.LastIndexOf("!") + 2);
        }

        protected ZipFile GetZipArchive(string path)
        {
            lock (_lock)
            {
                ZipFile zip;
                if (this.zipArchives.TryGetValue(path, out zip))
                    return zip;

                zip = new ZipFile(path);
                this.zipArchives.Add(path, zip);
                return zip;
            }
        }

        public int Priority { get { return 75; } }

        public bool Exists(string path)
        {
            try
            {
                var zipFilename = this.GetCompressedFileName(path);
                string entryName = GetCompressedEntryName(path);
                var zip = this.GetZipArchive(zipFilename);
                if (zip == null)
                    return false;
                if (!zip.ContainsEntry(entryName))
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

            if (!zip.ContainsEntry(entryName))
                throw new FileNotFoundException(path);

            var entry = zip[entryName];
            return entry.OpenReader();
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
