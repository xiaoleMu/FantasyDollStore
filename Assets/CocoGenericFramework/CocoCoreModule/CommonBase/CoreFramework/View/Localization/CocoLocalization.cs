using UnityEngine;
using System.Collections.Generic;

namespace CocoPlay.Localization
{
    public class CocoLocalization
    {
		public delegate byte[] LoadFunction (string path);

		/// <summary>
		/// Want to have Localization loading be custom instead of just Resources.Load? Set this function.
		/// </summary>

		public static LoadFunction loadFunction;

		/// <summary>
		/// Whether the localization dictionary has been loaded.
		/// </summary>

		public static bool localizationHasBeenSet = false;

		// Loaded languages, if any
		static string[] mLanguages = null;

		// Key = Value dictionary (single language)
		static Dictionary<string, string> mOldDictionary = new Dictionary<string, string>();

		// Key = Values dictionary (multiple languages)
		static Dictionary<string, string[]> mDictionary = new Dictionary<string, string[]>();

		// Index of the selected language within the multi-language dictionary
		static int mLanguageIndex = -1;

		// Currently selected language
		static string mLanguage;

		const string DEFAULT_LANGUAGE = "language_en";

		/// <summary>
		/// Localization dictionary. Dictionary key is the localization key. Dictionary value is the list of localized values (columns in the CSV file).
		/// Be very careful editing this via code, and be sure to set the "KEY" to the list of languages.
		/// </summary>

		public static Dictionary<string, string[]> dictionary
		{
			get
			{
				if (!localizationHasBeenSet) language = PlayerPrefs.GetString("Language", DEFAULT_LANGUAGE);
				return mDictionary;
			}
			set
			{
				localizationHasBeenSet = (value != null);
				mDictionary = value;
			}
		}

		/// <summary>
		/// List of loaded languages. Available if a single Localization.csv file was used.
		/// </summary>

		public static string[] knownLanguages
		{
			get
			{
				if (!localizationHasBeenSet) LoadDictionary(PlayerPrefs.GetString("Language", DEFAULT_LANGUAGE));
				return mLanguages;
			}
		}

    /// <summary>
    /// Name of the currently active language.
    /// </summary>

        public static void SetLanguage(string language)
        {
            CocoLocalization.language = language;
        }
    
		public static string language
		{
			get
			{
				if (string.IsNullOrEmpty(mLanguage))
				{
					string[] lan = knownLanguages;
					mLanguage = PlayerPrefs.GetString("Language", lan != null ? lan[0] : DEFAULT_LANGUAGE);
					LoadAndSelect(mLanguage);
				}
				return mLanguage;
			}
			set
			{
				if (mLanguage != value || !IsDictionaryLoaded)
				{
					mLanguage = value;
					LoadAndSelect (value);
				}
			}
		}

		/// <summary>
		/// Load the specified localization dictionary.
		/// </summary>

		static bool LoadDictionary (string value)
		{
			// Try to load the Localization CSV
			byte[] bytes = null;

			if (!localizationHasBeenSet)
			{
				if (loadFunction == null)
				{
					TextAsset asset = Resources.Load<TextAsset>("GameLocalization");
					if (asset != null) bytes = asset.bytes;
				}
				else bytes = loadFunction("GameLocalization");

				// Try to load the localization file
				if (LoadCSV (bytes)) {
					// Debug.LogError ("CocoLocalization.LoadDictionary: " + value + ", dict " + mDictionary.Count);
					localizationHasBeenSet = true;
					return true;
				}
			}

			// If this point was reached, the localization file was not present
			if (string.IsNullOrEmpty(value)) return false;

			// Not a referenced asset -- try to load it dynamically
			bytes = null;
			if (loadFunction == null)
			{
				TextAsset asset = Resources.Load<TextAsset>(value);
				if (asset != null) bytes = asset.bytes;
			}
			else bytes = loadFunction(value);

			// not loaded, try to load default
			if (bytes == null || bytes.Length <= 0) {
				if (loadFunction == null) {
					TextAsset asset = Resources.Load<TextAsset> (DEFAULT_LANGUAGE);
					if (asset != null) bytes = asset.bytes;
				} else bytes = loadFunction (DEFAULT_LANGUAGE);
			}

			Set (value, bytes);
			return IsDictionaryLoaded;
		}

		/// <summary>
		/// Load the specified language.
		/// </summary>
		private static bool LoadAndSelect (string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (mDictionary.Count == 0 && !LoadDictionary(value)) return false;
				if (SelectLanguage(value)) return true;
			}

			// Old style dictionary
			if (mOldDictionary.Count > 0) return true;

			// Either the language is null, or it wasn't found
			mOldDictionary.Clear();
			mDictionary.Clear();
			if (string.IsNullOrEmpty(value)) PlayerPrefs.DeleteKey("Language");
			return false;
		}

		/// <summary>
		/// Load the specified asset and activate the localization.
		/// </summary>

		public static void Load (TextAsset asset)
		{
			GameByteReader reader = new GameByteReader(asset);
			Set(asset.name, reader.ReadDictionary());
		}

		/// <summary>
		/// Set the localization data directly.
		/// </summary>

		public static void Set (string languageName, byte[] bytes)
		{
			GameByteReader reader = new GameByteReader(bytes);
			Set(languageName, reader.ReadDictionary());
		}

		/// <summary>
		/// Load the specified CSV file.
		/// </summary>

		public static bool LoadCSV (TextAsset asset, bool merge = false) { return LoadCSV(asset.bytes, asset, merge); }

		/// <summary>
		/// Load the specified CSV file.
		/// </summary>

		public static bool LoadCSV (byte[] bytes, bool merge = false) { return LoadCSV(bytes, null, merge); }

		/// <summary>
		/// Load the specified CSV file.
		/// </summary>

		static bool LoadCSV (byte[] bytes, TextAsset asset, bool merge = false)
		{
			if (bytes == null) return false;
			GameByteReader reader = new GameByteReader(bytes);

			// The first line should contain "KEY", followed by languages.
			GameBetterList<string> temp = reader.ReadCSV();

			// There must be at least two columns in a valid CSV file
			if (temp.size < 2) return false;

			// Clear the dictionary
			if (!merge || temp.size - 1 != mLanguage.Length)
			{
				merge = false;
				mDictionary.Clear();
			}

			temp[0] = "KEY";
			mLanguages = new string[temp.size - 1];
			for (int i = 0; i < mLanguages.Length; ++i)
				mLanguages[i] = temp[i + 1];

			// Read the entire CSV file into memory
			while (temp != null)
			{
				AddCSV(temp, !merge);
				temp = reader.ReadCSV();
			}
			return true;
		}

		/// <summary>
		/// Select the specified language from the previously loaded CSV file.
		/// </summary>

		static bool SelectLanguage (string language)
		{
			mLanguageIndex = -1;

			if (mDictionary.Count == 0) return false;

			string[] keys;

			if (mDictionary.TryGetValue("KEY", out keys))
			{
				for (int i = 0; i < keys.Length; ++i)
				{
					if (keys[i] == language)
					{
						mOldDictionary.Clear();
						mLanguageIndex = i;
						mLanguage = language;
						PlayerPrefs.SetString("Language", mLanguage);
//						UIRoot.Broadcast("OnLocalize");
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Helper function that adds a single line from a CSV file to the localization list.
		/// </summary>

		static void AddCSV (GameBetterList<string> values, bool warnOnDuplicate = true)
		{
			if (values.size < 2) return;
			string key = values[0];
			if (string.IsNullOrEmpty(key)) return;

			string[] temp = new string[values.size - 1];
			for (int i = 1; i < values.size; ++i) temp[i - 1] = values[i];

			if (mDictionary.ContainsKey(key))
			{
				mDictionary[key] = temp;
				if (warnOnDuplicate) Debug.LogWarning("Localization key '" + key + "' is already present");
			}
			else
			{
				try
				{
					mDictionary.Add(key, temp);
				}
				catch (System.Exception ex)
				{
					Debug.LogError("Unable to add '" + key + "' to the Localization dictionary.\n" + ex.Message);
				}
			}
		}

		/// <summary>
		/// Load the specified asset and activate the localization.
		/// </summary>

		public static void Set (string languageName, Dictionary<string, string> dictionary)
		{
			mLanguage = languageName;
			PlayerPrefs.SetString("Language", mLanguage);
			mOldDictionary = dictionary;
			mLanguageIndex = -1;
			mLanguages = new[] { languageName };
//			UIRoot.Broadcast("OnLocalize");

			localizationHasBeenSet = mOldDictionary != null && mOldDictionary.Count > 0;
			// Debug.LogError ("CocoLocalization.Set: " + languageName + ", old dict " + mOldDictionary.Count);
		}

		/// <summary>
		/// Localize the specified value.
		/// </summary>

		public static string Get (string key)
		{
			// Debug.LogError ("CocoLocalization.Get: " + key + ", localizationHasBeenSet " + localizationHasBeenSet);

			// Ensure we have a language to work with
			if (!localizationHasBeenSet) language = PlayerPrefs.GetString("Language", DEFAULT_LANGUAGE);

			string val;
			string[] vals;
			#if UNITY_IPHONE || UNITY_ANDROID
			string mobKey = key + " Mobile";

			if (mLanguageIndex != -1 && mDictionary.TryGetValue(mobKey, out vals))
			{
				if (mLanguageIndex < vals.Length)
					return vals[mLanguageIndex];
			}
			else if (mOldDictionary.TryGetValue(mobKey, out val)) return val;
			#endif
			if (mLanguageIndex != -1 && mDictionary.TryGetValue(key, out vals))
			{
				if (mLanguageIndex < vals.Length)
					return vals[mLanguageIndex];
			}
			else if (mOldDictionary.TryGetValue(key, out val)) return val;

			#if UNITY_EDITOR
			Debug.LogWarning("Localization key not found: '" + key + "'");
			#endif
			return key;
		}

		/// <summary>
		/// Localize the specified value and format it.
		/// </summary>

		public static string Format (string key, params object[] parameters) { return string.Format(Get(key), parameters); }

		[System.Obsolete("Localization is now always active. You no longer need to check this property.")]
		public static bool isActive { get { return true; } }

		[System.Obsolete("Use Localization.Get instead")]
		public static string Localize (string key) { return Get(key); }

		/// <summary>
		/// Returns whether the specified key is present in the localization dictionary.
		/// </summary>

		public static bool Exists (string key)
		{
			// Ensure we have a language to work with
			if (!localizationHasBeenSet) language = PlayerPrefs.GetString("Language", DEFAULT_LANGUAGE);

			#if UNITY_IPHONE || UNITY_ANDROID
			string mobKey = key + " Mobile";
			if (mDictionary.ContainsKey(mobKey)) return true;
			else if (mOldDictionary.ContainsKey(mobKey)) return true;
			#endif
			return mDictionary.ContainsKey(key) || mOldDictionary.ContainsKey(key);
		}


	    public static bool IsDictionaryLoaded {
		    get { return mDictionary.Count > 0 || mOldDictionary.Count > 0; }
	    }



	}
}

