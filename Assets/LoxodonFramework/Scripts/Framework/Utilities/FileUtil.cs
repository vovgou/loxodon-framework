using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Loxodon.Framework.Utilities
{
    public static class FileUtil
    {
        private static List<IZipAccessor> list = new List<IZipAccessor>();

        public static void Register(IZipAccessor zipAccessor)
        {
            if (list.Contains(zipAccessor))
                return;
            list.Add(zipAccessor);
            list.Sort((x, y) => y.Priority.CompareTo(x.Priority));
        }

        public static void Unregister(IZipAccessor zipAccessor)
        {
            if (!list.Contains(zipAccessor))
                return;
            list.Remove(zipAccessor);
        }

        public static string[] ReadAllLines(string path)
        {
            return ReadAllLines(path, Encoding.UTF8);

        }
        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            if (!IsZipArchive(path))
                return File.ReadAllLines(path, encoding);

            string line;
            List<string> lines = new List<string>();
            using (var stream = OpenReadInZip(path))
            {
                using (StreamReader sr = new StreamReader(stream, encoding, true))
                {
                    while ((line = sr.ReadLine()) != null)
                        lines.Add(line);
                }
            }
            return lines.ToArray();
        }

        public static string ReadAllText(string path)
        {
            return ReadAllText(path, Encoding.UTF8);
        }

        public static string ReadAllText(string path, Encoding encoding)
        {
            if (!IsZipArchive(path))
                return File.ReadAllText(path, encoding);

            using (var stream = OpenReadInZip(path))
            {
                using (StreamReader sr = new StreamReader(stream, encoding, true))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static byte[] ReadAllBytes(string path)
        {
            if (!IsZipArchive(path))
                return File.ReadAllBytes(path);

            using (var stream = OpenReadInZip(path))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public static Stream OpenRead(string path)
        {
            if (!IsZipArchive(path))
                return File.OpenRead(path);

            return OpenReadInZip(path);
        }

        public static bool Exists(string path)
        {
            if (!IsZipArchive(path))
                return File.Exists(path);

            return ExistsInZip(path);
        }

        private static Stream OpenReadInZip(string path)
        {
            for (int i = 0; i < list.Count; i++)
            {
                IZipAccessor zipAccessor = list[i];
                if (zipAccessor.Support(path))
                    return zipAccessor.OpenRead(path);
            }

            throw new NotSupportedException(path);
        }

        private static bool ExistsInZip(string path)
        {
            for (int i = 0; i < list.Count; i++)
            {
                IZipAccessor zipAccessor = list[i];
                if (zipAccessor.Support(path))
                    return zipAccessor.Exists(path);
            }

            throw new NotSupportedException(path);
        }

        private static bool IsZipArchive(string path)
        {
            if (Regex.IsMatch(path, @"(jar:file:///)|(\.jar)|(\.apk)|(\.obb)|(\.zip)", RegexOptions.IgnoreCase))
                return true;
            return false;
        }

        public interface IZipAccessor
        {
            int Priority { get; }

            bool Support(string path);

            Stream OpenRead(string path);

            bool Exists(string path);
        }
    }
}