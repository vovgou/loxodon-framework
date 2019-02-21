using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

using Loxodon.Framework.Execution;
using Loxodon.Framework.Localizations;
using Loxodon.Log;

namespace Loxodon.Framework.Examples
{
    /// <summary>
    /// AssetBundle data provider.
    /// dir:
    /// root/default/
    /// 
    /// root/zh/
    /// root/zh-CN/
    /// root/zh-TW/
    /// root/zh-HK/
    /// 
    /// root/en/
    /// root/en-US/
    /// root/en-CA/
    /// root/en-AU/
    /// </summary>
    public class AssetBundleDataProvider : IDataProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AssetBundleDataProvider));

        private string assetBundleUrl;
        private IDocumentParser parser;
        private ICoroutineExecutor executor;

        public AssetBundleDataProvider(string assetBundleUrl, IDocumentParser parser)
        {
            if (string.IsNullOrEmpty(assetBundleUrl))
                throw new ArgumentNullException("assetBundleUrl");

            if (parser == null)
                throw new ArgumentNullException("parser");

            this.assetBundleUrl = assetBundleUrl;
            this.parser = parser;
            this.executor = new CoroutineExecutor();
        }

        public void Load(CultureInfo cultureInfo, Action<Dictionary<string, object>> onCompleted)
        {
            executor.RunOnCoroutine(DoLoad(cultureInfo, onCompleted));
        }

        IEnumerator DoLoad(CultureInfo cultureInfo, Action<Dictionary<string, object>> onCompleted)
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

                    List<string> defaultPaths = assetNames.FindAll(p => p.Contains("/default/"));
                    List<string> twoLetterISOpaths = assetNames.FindAll(p => p.Contains(string.Format("/{0}/", cultureInfo.TwoLetterISOLanguageName)));
                    List<string> paths = assetNames.FindAll(p => p.Contains(string.Format("/{0}/", cultureInfo.Name)));

                    FillData(dict, bundle, defaultPaths);
                    FillData(dict, bundle, twoLetterISOpaths);
                    FillData(dict, bundle, paths);
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

        private void FillData(Dictionary<string, object> dict, AssetBundle bundle, List<string> paths)
        {
            try
            {
                if (paths == null || paths.Count <= 0)
                    return;

                foreach (string path in paths)
                {
                    try
                    {
                        TextAsset text = bundle.LoadAsset<TextAsset>(path);
                        using (MemoryStream stream = new MemoryStream(text.bytes))
                        {
                            var data = parser.Parse(stream);
                            foreach (KeyValuePair<string, object> kv in data)
                            {
                                dict[kv.Key] = kv.Value;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("{0}", e);
                    }
                }
            }
            catch (Exception) { }
        }
    }

}