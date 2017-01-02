using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

using Loxodon.Framework.Localizations;

namespace Loxodon.Framework.Examples
{
	/// <summary>
	/// Resources data provider.
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
	public class ResourcesDataProvider : IDataProvider
	{
		private string root;
		private IDocumentParser parser;

		public ResourcesDataProvider (string root, IDocumentParser parser)
		{
			if (string.IsNullOrEmpty (root))
				throw new ArgumentNullException ("root");

			if (parser == null)
				throw new ArgumentNullException ("parser");

			this.root = root;
			this.parser = parser;
		}

		protected string GetDefaultPath ()
		{
			return GetPath ("default");
		}

		protected string GetPath (string dir)
		{
			StringBuilder buf = new StringBuilder ();
			buf.Append (this.root);
			if (!this.root.EndsWith ("/"))
				buf.Append ("/");
			buf.Append (dir);
			return buf.ToString ();
		}

		public void Load (CultureInfo cultureInfo, Action<Dictionary<string, object>> onCompleted)
		{
			Dictionary<string, object> dict = new Dictionary<string, object> ();
			TextAsset[] defaultTexts = Resources.LoadAll<TextAsset> (GetDefaultPath ());

			TextAsset[] texts = Resources.LoadAll<TextAsset> (GetPath (cultureInfo.Name));//eg:zh-CN  en-US
			if (texts == null || texts.Length == 0)
				texts = Resources.LoadAll<TextAsset> (GetPath (cultureInfo.TwoLetterISOLanguageName));//eg:zh  en                      

			if ((defaultTexts == null || defaultTexts.Length <= 0) && (texts == null || texts.Length <= 0)) {
				if (onCompleted != null)
					onCompleted (dict);
				return;
			}

			foreach (TextAsset text in defaultTexts) {
				using (MemoryStream stream = new MemoryStream (text.bytes)) {
					var data = parser.Parse (stream);
					foreach (KeyValuePair<string, object> kv in data) {
						dict [kv.Key] = kv.Value;
					}
				}
			}

			foreach (TextAsset text in texts) {
				using (MemoryStream stream = new MemoryStream (text.bytes)) {
					var data = parser.Parse (stream);
					foreach (KeyValuePair<string, object> kv in data) {
						dict [kv.Key] = kv.Value;
					}
				}
			}

			if (onCompleted != null)
				onCompleted (dict);
		}
	}
}