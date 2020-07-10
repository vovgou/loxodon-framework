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
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Loxodon.Framework.Localizations
{
    public class AssetBundleCsvDataProvider : IDataProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultCsvDataProvider));

        private string assetBundleUrl;
        private IDocumentParser parser;
        private ICoroutineExecutor executor;

        public AssetBundleCsvDataProvider(string assetBundleUrl, IDocumentParser parser)
        {
            if (string.IsNullOrEmpty(assetBundleUrl))
                throw new ArgumentNullException("assetBundleUrl");

            if (parser == null)
                throw new ArgumentNullException("parser");

            this.assetBundleUrl = assetBundleUrl;
            this.parser = parser;
            this.executor = new CoroutineExecutor();
        }

        public void Load(CultureInfo cultureInfo, Action<Dictionary<string, object>> onLoadCompleted)
        {
            executor.RunOnCoroutine(DoLoad(cultureInfo, onLoadCompleted));
        }

        protected virtual IEnumerator DoLoad(CultureInfo cultureInfo, Action<Dictionary<string, object>> onLoadCompleted)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(this.assetBundleUrl))
            {
                www.SendWebRequest();
                while (!www.isDone)
                    yield return null;

                DownloadHandlerAssetBundle handler = (DownloadHandlerAssetBundle)www.downloadHandler;
                AssetBundle bundle = handler.assetBundle;
                try
                {
                    TextAsset[] texts = bundle.LoadAllAssets<TextAsset>();
                    FillData(dict, texts, cultureInfo);
                }
                finally
                {
                    try
                    {
                        if (bundle != null)
                            bundle.Unload(true);
                    }
                    catch (Exception) { }

                    if (onLoadCompleted != null)
                        onLoadCompleted(dict);
                }
            }
        }

        private void FillData(Dictionary<string, object> dict, TextAsset[] texts, CultureInfo cultureInfo)
        {
            try
            {
                if (texts == null || texts.Length <= 0)
                    return;

                foreach (TextAsset text in texts)
                {
                    try
                    {
                        using (MemoryStream stream = new MemoryStream(text.bytes))
                        {
                            var data = parser.Parse(stream, cultureInfo);
                            foreach (KeyValuePair<string, object> kv in data)
                            {
                                dict[kv.Key] = kv.Value;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("An error occurred when loading localized data from \"{0}\".Error:{1}", text.name, e);
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
