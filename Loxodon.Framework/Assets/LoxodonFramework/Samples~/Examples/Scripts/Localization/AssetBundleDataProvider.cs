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
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

using Loxodon.Framework.Asynchronous;
using Loxodon.Log;
using System.Threading.Tasks;
using Loxodon.Framework.Localizations;

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

        public AssetBundleDataProvider(string assetBundleUrl, IDocumentParser parser)
        {
            if (string.IsNullOrEmpty(assetBundleUrl))
                throw new ArgumentNullException("assetBundleUrl");

            if (parser == null)
                throw new ArgumentNullException("parser");

            this.assetBundleUrl = assetBundleUrl;
            this.parser = parser;
        }

        public virtual async Task<Dictionary<string, object>> Load(CultureInfo cultureInfo)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(this.assetBundleUrl))
            {
                await www.SendWebRequest();

                DownloadHandlerAssetBundle handler = (DownloadHandlerAssetBundle)www.downloadHandler;
                AssetBundle bundle = handler.assetBundle;
                if (bundle == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Failed to load Assetbundle from \"{0}\".", this.assetBundleUrl);
                    return dict;
                }
                try
                {
                    List<string> assetNames = new List<string>(bundle.GetAllAssetNames());
                    List<string> defaultPaths = assetNames.FindAll(p => p.Contains("/default/"));//eg:default
                    List<string> twoLetterISOpaths = assetNames.FindAll(p => p.Contains(string.Format("/{0}/", cultureInfo.TwoLetterISOLanguageName)));//eg:zh  en
                    List<string> paths = cultureInfo.Name.Equals(cultureInfo.TwoLetterISOLanguageName) ? null : assetNames.FindAll(p => p.Contains(string.Format("/{0}/", cultureInfo.Name)));//eg:zh-CN  en-US

                    FillData(dict, bundle, defaultPaths, cultureInfo);
                    FillData(dict, bundle, twoLetterISOpaths, cultureInfo);
                    FillData(dict, bundle, paths, cultureInfo);
                }
                finally
                {
                    try
                    {
                        if (bundle != null)
                            bundle.Unload(true);
                    }
                    catch (Exception) { }
                }
                return dict;
            }
        }

        private void FillData(Dictionary<string, object> dict, AssetBundle bundle, List<string> paths, CultureInfo cultureInfo)
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
                            log.WarnFormat("An error occurred when loading localized data from \"{0}\".Error:{1}", path, e);
                    }
                }
            }
            catch (Exception) { }
        }
    }

}