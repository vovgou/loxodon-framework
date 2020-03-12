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

using Loxodon.Framework.Execution;
using Loxodon.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

namespace Loxodon.Framework.Localizations
{
    public class AssetBundleLocalizationSourceDataProvider : IDataProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AssetBundleLocalizationSourceDataProvider));

        protected string[] filenames;
        protected string assetBundleUrl;

        public AssetBundleLocalizationSourceDataProvider(string assetBundleUrl, params string[] filenames)
        {
            if (string.IsNullOrEmpty(assetBundleUrl))
                throw new ArgumentNullException("assetBundleUrl");

            this.assetBundleUrl = assetBundleUrl;
            this.filenames = filenames;
        }

        public void Load(CultureInfo cultureInfo, Action<Dictionary<string, object>> onCompleted)
        {
            Executors.RunOnCoroutine(DoLoad(cultureInfo, onCompleted));
        }

        protected virtual IEnumerator DoLoad(CultureInfo cultureInfo, Action<Dictionary<string, object>> onCompleted)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

#if UNITY_2018_1_OR_NEWER
            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(this.assetBundleUrl))
            {
                www.SendWebRequest();
                while (!www.isDone)
                    yield return null;

                DownloadHandlerAssetBundle handler = (DownloadHandlerAssetBundle)www.downloadHandler;
                AssetBundle bundle = handler.assetBundle;
#elif UNITY_2017_1_OR_NEWER
            using (UnityWebRequest www = UnityWebRequest.GetAssetBundle(this.assetBundleUrl))
            {
                www.Send();
                while (!www.isDone)
                    yield return null;

                DownloadHandlerAssetBundle handler = (DownloadHandlerAssetBundle)www.downloadHandler;
                AssetBundle bundle = handler.assetBundle;
#else
            using (WWW www = new WWW(this.assetBundleUrl))
            {
                while (!www.isDone)
                    yield return null;

                AssetBundle bundle = www.assetBundle;
#endif
                try
                {
                    List<string> assetNames = new List<string>(bundle.GetAllAssetNames());
                    foreach (string filename in filenames)
                    {
                        try
                        {
                            string defaultPath = assetNames.Find(p => p.Contains(string.Format("/default/{0}", filename)));//eg:default
                            string twoLetterISOpath = assetNames.Find(p => p.Contains(string.Format("/{0}/{1}", cultureInfo.TwoLetterISOLanguageName, filename)));//eg:zh  en
                            string path = cultureInfo.Name.Equals(cultureInfo.TwoLetterISOLanguageName) ? null : assetNames.Find(p => p.Contains(string.Format("/{0}/{1}", cultureInfo.Name, filename)));//eg:zh-CN  en-US

                            FillData(dict, bundle, defaultPath);
                            FillData(dict, bundle, twoLetterISOpath);
                            FillData(dict, bundle, path);
                        }
                        catch (Exception e)
                        {
                            if (log.IsWarnEnabled)
                                log.WarnFormat("An error occurred when loading localized data from \"{0}\".Error:{1}", filename, e);
                        }
                    }
                }
                finally
                {
                    try
                    {
                        if (bundle != null)
                            bundle.Unload(true);
                    }
                    catch (Exception) { }

                    if (onCompleted != null)
                        onCompleted(dict);
                }
            }
        }

        private void FillData(Dictionary<string, object> dict, AssetBundle bundle, string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            LocalizationSourceAsset sourceAsset = bundle.LoadAsset<LocalizationSourceAsset>(path);
            if (sourceAsset == null)
                return;

            MonolingualSource source = sourceAsset.Source;
            if (source == null)
                return;

            foreach (KeyValuePair<string, object> kv in source.GetData())
            {
                dict[kv.Key] = kv.Value;
            }
        }
    }
}
