using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Loxodon.Framework.Execution;
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
		private string assetBundleUrl;
		private IDocumentParser parser;
		private ICoroutineExecutor executor;

		public AssetBundleDataProvider (string assetBundleUrl, IDocumentParser parser)
		{
			if (string.IsNullOrEmpty (assetBundleUrl))
				throw new ArgumentNullException ("assetBundleUrl");

			if (parser == null)
				throw new ArgumentNullException ("parser");

			this.assetBundleUrl = assetBundleUrl;
			this.parser = parser;
			this.executor = new CoroutineExecutor ();
		}

		public void Load (CultureInfo cultureInfo, Action<Dictionary<string, object>> onCompleted)
		{			
			executor.RunOnCoroutine (DoLoad (cultureInfo, onCompleted));
		}

		IEnumerator DoLoad (CultureInfo cultureInfo, Action<Dictionary<string, object>> onCompleted)
		{
			Dictionary<string, object> dict = new Dictionary<string, object> ();
			using (WWW www = new WWW (this.assetBundleUrl)) {
				
				while (!www.isDone)
					yield return null;

				AssetBundle bundle = www.assetBundle;

				List<string> assetNames = new List<string> (bundle.GetAllAssetNames ());

				List<string> defaultPaths = assetNames.FindAll (p => p.Contains ("/default/"));
				List<string> paths = assetNames.FindAll (p => p.Contains (string.Format ("/{0}/", cultureInfo.Name)));
				if (paths == null || paths.Count <= 0)
					paths = assetNames.FindAll (p => p.Contains (string.Format ("/{0}/", cultureInfo.TwoLetterISOLanguageName)));

				if ((defaultPaths == null || defaultPaths.Count <= 0) && (paths == null || paths.Count <= 0)) {
					if (onCompleted != null)
						onCompleted (dict);
					yield break;
				}

				foreach (string path in defaultPaths) {
					TextAsset text = bundle.LoadAsset<TextAsset> (path);
					using (MemoryStream stream = new MemoryStream (text.bytes)) {
						var data = parser.Parse (stream);
						foreach (KeyValuePair<string, object> kv in data) {
							dict [kv.Key] = kv.Value;
						}
					}
				}

				foreach (string path in paths) {
					TextAsset text = bundle.LoadAsset<TextAsset> (path);
					using (MemoryStream stream = new MemoryStream (text.bytes)) {
						var data = parser.Parse (stream);
						foreach (KeyValuePair<string, object> kv in data) {
							dict [kv.Key] = kv.Value;
						}
					}
				}

				bundle.Unload (true);

				if (onCompleted != null)
					onCompleted (dict);
			}
		}
	}

}