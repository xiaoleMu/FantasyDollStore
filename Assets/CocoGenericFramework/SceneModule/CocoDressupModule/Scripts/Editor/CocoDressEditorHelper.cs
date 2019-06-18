using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace CocoPlay
{
	public static class CocoDressEditorHelper
	{
		#region Text Dictionary

		private class LanguageTextDictionary
		{
			private readonly Dictionary<SystemLanguage, Dictionary<string, string>> m_LanguageTextDic =
				new Dictionary<SystemLanguage, Dictionary<string, string>> ();

			public void Set (SystemLanguage language, string key, string text)
			{
				Dictionary<string, string> textDic;
				if (m_LanguageTextDic.ContainsKey (language)) {
					textDic = m_LanguageTextDic [language];
				} else {
					textDic = new Dictionary<string, string> { { key, text } };
					m_LanguageTextDic.Add (language, textDic);
				}

				if (!textDic.ContainsKey (key)) {
					textDic.Add (key, text);
				}
			}

			public string Get (string key)
			{
				var text = Get (key, Application.systemLanguage);
				if (string.IsNullOrEmpty (text)) {
					text = Get (key, SystemLanguage.English);
				}
				if (string.IsNullOrEmpty (text)) {
					text = key;
				}
				return text;
			}

			public string Get (string key, SystemLanguage language)
			{
				var textDic = m_LanguageTextDic.GetValue (language);
				return textDic != null ? textDic.GetValue (key) : string.Empty;
			}
		}


		private static readonly LanguageTextDictionary _textDic = new LanguageTextDictionary ();

		public static void SetLanguageText (SystemLanguage language, string key, string text)
		{
			_textDic.Set (language, key, text);
		}

		public static string GetLanguageText (string key)
		{
			return _textDic.Get (key);
		}

		#endregion


		#region GUI

		public static bool PathField (string label, ref string path, string rootDirectory, bool isDirectory)
		{
			var changed = false;

			EditorGUILayout.BeginHorizontal ();
			var newPath = EditorGUILayout.DelayedTextField (label, path);
			changed |= (newPath != path);

			if (GUILayout.Button ("Select ...")) {
				var fullPath = isDirectory
					? EditorUtility.OpenFolderPanel (label, Path.Combine (rootDirectory, path), "")
					: EditorUtility.OpenFilePanel (label, Path.Combine (rootDirectory, path), "");
				if (!string.IsNullOrEmpty (fullPath) && fullPath.StartsWith (rootDirectory)) {
					newPath = fullPath.Substring (rootDirectory.Length + 1);
					changed |= (newPath != path);
				}
			}
			EditorGUILayout.EndHorizontal ();

			path = newPath;
			return changed;
		}

		public static bool Button (string label, Color color, FontStyle fontStyle = FontStyle.Normal)
		{
			var originColor = GUI.contentColor;
			GUI.contentColor = color;
			var originFontStyle = GUI.skin.button.fontStyle;
			GUI.skin.button.fontStyle = fontStyle;

			var result = GUILayout.Button (label);

			GUI.skin.button.fontStyle = originFontStyle;
			GUI.contentColor = originColor;

			return result;
		}

		#endregion


		#region Asest

		public static List<T> CollectAllAssets<T> (string path, string searchPattern = "*", bool searchRecursively = false) where T : Object
		{
			var list = new List<T> ();
			var files = Directory.GetFiles (path, searchPattern, searchRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

			foreach (var file in files) {
				if (!file.Contains (".DS_Store") && !file.Contains (".meta")) {
					var asset = AssetDatabase.LoadAssetAtPath<T> (file);
					if (asset != null) {
						list.Add (asset);
					}
				}
			}

			return list;
		}

		public static string GetFullPath (string path)
		{
			return Path.Combine (Application.dataPath, path);
		}

		public static string GetRelativePath (string path, string rootDirectory)
		{
			if (!path.StartsWith (rootDirectory)) {
				return path;
			}

			return path.Length > rootDirectory.Length ? path.Substring (rootDirectory.Length + 1) : "";
		}

		#endregion
	}
}