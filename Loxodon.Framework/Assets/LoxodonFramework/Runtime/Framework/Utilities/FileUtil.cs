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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Loxodon.Log;

namespace Loxodon.Framework.Utilities
{
    public static class FileUtil
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FileUtil));
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

            byte[] data = ReadAllBytes(path);
            if (!HasBOMFlag(data))
                return encoding.GetString(data);

            return encoding.GetString(data, 3, data.Length - 3);
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

#if UNITY_ANDROID
            if (path.IndexOf(".obb", StringComparison.OrdinalIgnoreCase) > 0 && log.IsWarnEnabled)
                log.Warn("Unable to read the content in the \".obb\" file, please click the link for help, and enable access to the OBB file. https://github.com/cocowolf/loxodon-framework/blob/master/docs/faq.md");
#endif

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

        public static bool IsZipArchive(string path)
        {
            if (Regex.IsMatch(path, @"(jar:file:///)|(\.jar)|(\.apk)|(\.obb)|(\.zip)", RegexOptions.IgnoreCase))
                return true;
            return false;
        }

        static bool HasBOMFlag(byte[] data)
        {
            if (data == null || data.Length < 3)
                return false;

            if (data[0] == 239 && data[1] == 187 && data[2] == 191)
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