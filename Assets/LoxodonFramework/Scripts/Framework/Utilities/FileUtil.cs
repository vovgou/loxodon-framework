using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Loxodon.Framework.Utilities
{
    public static class FileUtil
    {
        public static string[] ReadAllLines(string path)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (InApk(path))
                return AndroidFileUtil.ReadAllLines(path);
#endif
            return File.ReadAllLines(path);

        }
        public static string[] ReadAllLines(string path, Encoding encoding)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (InApk(path))
                return AndroidFileUtil.ReadAllLines(path, encoding);
#endif
            return File.ReadAllLines(path, encoding);
        }

        public static string ReadAllText(string path)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (InApk(path))
                return AndroidFileUtil.ReadAllText(path);
#endif
            return File.ReadAllText(path);
        }

        public static string ReadAllText(string path, Encoding encoding)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (InApk(path))
                return AndroidFileUtil.ReadAllText(path, encoding);
#endif
            return File.ReadAllText(path, encoding);
        }

        public static byte[] ReadAllBytes(string path)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (InApk(path))
                return AndroidFileUtil.ReadAllBytes(path);
#endif
            return File.ReadAllBytes(path);
        }

        public static bool Exists(string path)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (InApk(path))
                return AndroidFileUtil.Exists(path);
#endif
            return File.Exists(path);
        }

        private static bool InApk(string path)
        {
            if (Regex.IsMatch(path, @"(jar:file:///)|(\.jar)|(\.apk)|(\.obb)", RegexOptions.IgnoreCase))
                return true;
            return false;
        }
    }

}