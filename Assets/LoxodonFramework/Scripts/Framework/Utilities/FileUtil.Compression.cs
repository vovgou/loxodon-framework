#if UNITY_ANDROID && NET_STANDARD_2_0
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