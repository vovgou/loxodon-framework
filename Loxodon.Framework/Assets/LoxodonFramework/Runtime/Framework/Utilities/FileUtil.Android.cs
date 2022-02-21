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

#if UNITY_ANDROID
using Loxodon.Log;
using System;
using System.IO;
using UnityEngine;

namespace Loxodon.Framework.Utilities
{
    public class ZipAccessorForAndroidStreamingAssets : FileUtil.IZipAccessor
    {
        private const string ACTIVITY_JAVA_CLASS = "com.unity3d.player.UnityPlayer";
        private const string ASSET_MANGER_CLASS_NAME = "android.content.res.AssetManager";

        private static readonly ILog log = LogManager.GetLogger(typeof(ZipAccessorForAndroidStreamingAssets));

        private static AndroidJavaObject assetManager;

        protected static AndroidJavaObject AssetManager
        {
            get
            {
                if (assetManager != null)
                    return assetManager;

                try
                {
                    using (AndroidJavaClass activityClass = new AndroidJavaClass(ACTIVITY_JAVA_CLASS))
                    {
                        using (var context = activityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                        {
                            assetManager = context.Call<AndroidJavaObject>("getAssets");
                        }
                    }
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Failed to get the AssetManager from the Activity, try to get it from android.content.res.AssetManager", e);
                }

                try
                {
                    if (assetManager == null)
                    {
                        using (AndroidJavaClass assetManagerClass = new AndroidJavaClass(ASSET_MANGER_CLASS_NAME))
                        {
                            assetManager = assetManagerClass.GetStatic<AndroidJavaObject>("getSystem");
                        }
                    }
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Failed to get the AssetManager from android.content.res.AssetManager", e);
                }
                return assetManager;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnInitialized()
        {
            FileUtil.Register(new ZipAccessorForAndroidStreamingAssets());
        }

        protected string GetAssetFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            int start = path.LastIndexOf("!/assets/");
            if (start < 0)
                return path;

            return path.Substring(start + 9);
        }

        public int Priority { get { return 0; } }

        public bool Exists(string path)
        {
            try
            {
                using (AndroidJavaObject fileDescriptor = AssetManager.Call<AndroidJavaObject>("openFd", GetAssetFilePath(path)))
                {
                    if (fileDescriptor != null)
                        return true;
                }
            }
            catch (Exception) { }

            return false;
        }

        public Stream OpenRead(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("the filename is null or empty.");

            return new InputStreamWrapper(AssetManager.Call<AndroidJavaObject>("open", GetAssetFilePath(path)));
        }

        public bool Support(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            string fullname = path.ToLower();
            if (fullname.IndexOf(".apk") > 0 && fullname.LastIndexOf("!/assets/") > 0)
                return true;

            return false;
        }

        public class InputStreamWrapper : Stream
        {
            private object _lock = new object();
            private long length = 0;
            private long position = 0;
            private AndroidJavaObject inputStream;
            public InputStreamWrapper(AndroidJavaObject inputStream)
            {
                this.inputStream = inputStream;
                this.length = inputStream.Call<int>("available");
            }

            public override bool CanRead { get { return this.position < this.length; } }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override long Length
            {
                get { return this.length; }
            }

            public override long Position
            {
                get { return this.position; }
                set { throw new NotSupportedException(); }
            }

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                lock (_lock)
                {
                    int ret = 0;
                    IntPtr array = IntPtr.Zero;
                    try
                    {
                        array = AndroidJNI.NewByteArray(count);
                        var method = AndroidJNIHelper.GetMethodID(inputStream.GetRawClass(), "read", "([B)I");
                        ret = AndroidJNI.CallIntMethod(inputStream.GetRawObject(), method, new[] { new jvalue() { l = array } });
                        if (ret <= 0)
                            return ret;

                        byte[] data = AndroidJNI.FromByteArray(array);
                        //Array.Copy(data, 0, buffer, offset, ret);
                        Buffer.BlockCopy(data, 0, buffer, 0, ret);
                        this.position += ret;
                    }
                    finally
                    {
                        if (array != IntPtr.Zero)
                            AndroidJNI.DeleteLocalRef(array);
                    }
                    return ret;
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (inputStream != null)
                {
                    inputStream.Call("close");
                    inputStream.Dispose();
                    inputStream = null;
                }
            }
        }
    }
}
#endif