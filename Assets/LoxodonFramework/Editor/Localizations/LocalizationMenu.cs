using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

using Loxodon.Framework.Localizations;

namespace Loxodon.Framework.Editors
{
    public static class LocalizationMenu
	{
		private const string MENU_NAME = "Assets/Loxodon/Localization Make";
		private const string CLASS_NAME_KEY = "_LOXODON_LOCALIZATION_CLASS_NAME_KEY";
		private const string OUTPUT_DIR_KEY = "_LOXODON_LOCALIZATION_OUTPUT_DIR_KEY";
		private const string DEFAULT_OUTPUT_DIR = "Assets/Gen/";
		private const string DEFAULT_CLASS_NAME = "R";
		private const string EXTENSION = ".xml";

		[MenuItem (MENU_NAME, false, 0)]
		static void Generate ()
		{
			var selections = Selection.GetFiltered (typeof(TextAsset), SelectionMode.DeepAssets);
			if (selections == null || selections.Length <= 0)
				return;

			var dir = EditorPrefs.GetString (OUTPUT_DIR_KEY, DEFAULT_OUTPUT_DIR);
			var className = EditorPrefs.GetString (CLASS_NAME_KEY, DEFAULT_CLASS_NAME);
			string location = EditorUtility.SaveFilePanel ("Generate Code", dir, className, "cs");
			if (string.IsNullOrEmpty (location))
				return;

			dir = GetRelativeDirectory (location);
			className = GetClassName (location);

			EditorPrefs.SetString (OUTPUT_DIR_KEY, dir);
			EditorPrefs.SetString (CLASS_NAME_KEY, className);

			CodeGenerator generator = new CodeGenerator ();
			XmlDocumentParser parser = new XmlDocumentParser ();
			Dictionary<string,object> data = new Dictionary<string, object> ();
			foreach (var s in selections) {
				try {
					string path = AssetDatabase.GetAssetPath (s);
					if (!path.ToLower ().EndsWith (EXTENSION))
						continue;
					
					var dict = parser.Parse (new MemoryStream ((s as TextAsset).bytes));
					foreach (KeyValuePair<string,object> kv in dict) {
						data [kv.Key] = kv.Value;
					}
				} catch (Exception) {
				}
			}
			if (data.Count <= 0)
				return;
			
			var code = generator.Generate (className, data);
			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);
			
			File.WriteAllText (location, code);

			AssetDatabase.Refresh ();
		}

		static string GetRelativeDirectory (string location)
		{
			int start = location.LastIndexOf ("Assets");
            if (start < 0)
                return "Assets";

			int end = location.LastIndexOf ("/") + 1;
			return location.Substring (start, (end - start));
		}

		static string GetClassName (string location)
		{
			int start = location.LastIndexOf ("/") + 1;
			int end = location.LastIndexOf (".cs");
			return location.Substring (start, (end - start));
		}

		[MenuItem (MENU_NAME, true)]
		static bool IsValidated ()
		{
			var selections = Selection.GetFiltered (typeof(TextAsset), SelectionMode.DeepAssets);
			if (selections == null || selections.Length <= 0)
				return false;
			
			foreach (var s in selections) {
				string path = AssetDatabase.GetAssetPath (s);
				if (path.ToLower ().EndsWith (EXTENSION))
					return true;
			}
			return false;
		}
	}
}

