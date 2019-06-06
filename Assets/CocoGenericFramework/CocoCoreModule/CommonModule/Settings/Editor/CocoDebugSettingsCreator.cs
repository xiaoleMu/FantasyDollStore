using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using TabTale.Plugins.PSDK;

namespace CocoPlay
{
	public static class CocoDebugSettingsCreator
	{
		[MenuItem ("CocoPlay/Debug/Debug Settings", false, 150)]
		private static void ShowSettings ()
		{
			Selection.activeObject = SettingsData;
		}

		private static CocoDebugSettingsData SettingsData {
			get {
				CocoDebugSettingsData settingsData = Resources.Load<CocoDebugSettingsData> ("CocoDebugSettingsData");
				if (settingsData == null) {
					settingsData = CreateSettingsData ();
				}
				return settingsData;
			}
		}

		private static CocoDebugSettingsData CreateSettingsData ()
		{
			CocoDebugSettingsData settingsData = ScriptableObject.CreateInstance<CocoDebugSettingsData> ();

			CreateAssetFolderRecursively (Path.GetDirectoryName (ASSET_PATH));
			AssetDatabase.CreateAsset (settingsData, ASSET_PATH);

			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();

			return settingsData;
		}

		private static bool CreateAssetFolderRecursively (string folderPath)
		{
			string fullPath = Path.GetFullPath (folderPath);
			if (Directory.Exists (fullPath)) {
				return true;
			}

			string parentFolder = Path.GetDirectoryName (folderPath);
			bool parentExists = CreateAssetFolderRecursively (parentFolder);
			if (parentExists) {
				string folderName = Path.GetFileName (folderPath);
				AssetDatabase.CreateFolder (parentFolder, folderName);
				return true;
			}

			return false;
		}


		#region Json Config

		[MenuItem ("CocoPlay/Debug/Update Debug Settings Form PSDK", false, 151)]
		private static void UpdateFromPSDKConfig ()
		{
			var message = UpdateSettingsDataFromPSDKConfig ();
			EditorUtility.DisplayDialog ("Info", message, "OK");

			ShowSettings ();
		}

		[MenuItem ("CocoPlay/Debug/Commit Debug Settings To PSDK", false, 152)]
		public static void WriteToPSDKConfig ()
		{
			var message = CommitSettingsDataToPSDKConfig ();
			EditorUtility.DisplayDialog ("Info", message, "OK");

			ShowPSDKConfig ();
		}


		#region Update

		public static string UpdateSettingsDataFromPSDKConfig ()
		{
			var psdkConfigData = LoadPSDKConfigData ();
			if (psdkConfigData == null) {
				return "Update Failed!\n- Can NOT load PSDK config json data.";
			}

			var settingAssetData = SettingsData;

			string message;
			if (psdkConfigData.ContainsKey (PSDK_SETTINGS_KEY)) {
				// read from psdk config
				var settingsData = psdkConfigData [PSDK_SETTINGS_KEY] as Dictionary<string, object>;
				if (settingsData == null) {
					return "Update Failed!\n- Debug settings data format ERROR!";
				}

				if (!ConvertToScriptableData (ref settingsData)) {
					return "Update Failed!\n- Debug settings data can NOT convert to scriptable data!";
				}

				// overwrite to asset
				var settingsJson = Json.Serialize (settingsData);
				EditorJsonUtility.FromJsonOverwrite (settingsJson, settingAssetData);

				message = "Debug settings updated from PSDK config.";
				Debug.LogFormat ("{0} {1}", message, EditorJsonUtility.ToJson (settingAssetData));
			} else {
				// no config, disable global
				settingAssetData.IsGlobalEnabled = false;
				message = "Debug settings data NOT be found in PSDK config, disable global settings!";
			}

			EditorUtility.SetDirty (settingAssetData);
			AssetDatabase.SaveAssets ();

			return string.Format("Update Successful. :)\n- {0}", message);
		}

		private static bool ConvertToScriptableData (ref Dictionary<string, object> settingsData)
		{
			Dictionary<string, object> entityData;
			if (settingsData.ContainsKey (SETTINGS_ENTITY_KEY)) {
				entityData = settingsData [SETTINGS_ENTITY_KEY] as Dictionary<string, object>;
			} else {
				entityData = settingsData;
				settingsData = new Dictionary<string, object> { { SETTINGS_ENTITY_KEY, entityData } };
			}

			if (entityData == null) {
				return false;
			}

			// add entity headers
			foreach (var entityHeader in _settingsEntityHeaders) {
				settingsData [entityHeader.Key] = entityHeader.Value;
			}

			return true;
		}

		#endregion


		#region Commit

		private static string CommitSettingsDataToPSDKConfig ()
		{
			var psdkConfigData = LoadPSDKConfigData ();
			if (psdkConfigData == null) {
				return "Commit Failed!\nCan NOT load PSDK config json data.";
			}

			var settingsJson = EditorJsonUtility.ToJson (SettingsData);
			var settingsData = Json.Deserialize (settingsJson) as Dictionary<string, object>;
			if (settingsData == null) {
				return "Commit Failed! Convert settings Data to json failed.";
			}

			if (!ConvertToEntityData (ref settingsData)) {
				return "Commit Failed!\n- Debug settings data can NOT convert to entity data!";
			}

			// write in psdk config
			psdkConfigData [PSDK_SETTINGS_KEY] = settingsData;
			SavePSDKConfigData (psdkConfigData);

			return "Update Successful. :)";
		}

		private static bool ConvertToEntityData (ref Dictionary<string, object> settingsData)
		{
			if (settingsData.ContainsKey (SETTINGS_ENTITY_KEY)) {
				settingsData = settingsData [SETTINGS_ENTITY_KEY] as Dictionary<string, object>;
			}

			if (settingsData == null) {
				return false;
			}

			// remove entity headers
			foreach (var entityHeaderKey in _settingsEntityHeaders.Keys) {
				settingsData.Remove (entityHeaderKey);
			}

			return true;
		}

		#endregion


		#region Helper

		private const string ASSET_PATH = "Assets/_Game/Resources/CocoDebugSettingsData.asset";

		private const string PSDK_SETTINGS_KEY = "cocoDebugSettings";
		private const string SETTINGS_ENTITY_KEY = "MonoBehaviour";

		private static readonly Dictionary<string, object> _settingsEntityHeaders = new Dictionary<string, object> {
			{ "m_Enabled", true },
			{ "m_EditorHideFlags", 0 },
			{ "m_Name", "CocoDebugSettingsData" },
			{ "m_EditorClassIdentifier", "" }
		};


		private static string PSDKConfigFilePath {
			get {
				var fileName = "psdk_ios.json";
#if UNITY_ANDROID
#if AMAZON
				fileName = "psdk_amazon.json";
#else
				fileName = "psdk_google.json";
#endif
#endif
				return Path.Combine (Application.streamingAssetsPath, fileName);
			}
		}

		private static void ShowPSDKConfig ()
		{
			var path = PSDKConfigFilePath;
			var assetPath = "Assets" + path.Substring (Application.dataPath.Length);
			var psdkConfig = AssetDatabase.LoadAssetAtPath<Object> (assetPath);
			if (psdkConfig != null) {
				Selection.activeObject = psdkConfig;
			}
		}

		private static Dictionary<string, object> LoadPSDKConfigData ()
		{
			var psdkConfigPath = PSDKConfigFilePath;
			if (!File.Exists (psdkConfigPath)) {
				return null;
			}

			var json = File.ReadAllText (psdkConfigPath);
			var configData = Json.Deserialize (json) as Dictionary<string, object>;
			return configData;
		}

		private static void SavePSDKConfigData (Dictionary<string, object> psdkConfigData)
		{
			var psdkConfigPath = PSDKConfigFilePath;

			var json = Json.Serialize (psdkConfigData);
			var originalIndent = JsonFormatter.Indent;
			JsonFormatter.Indent = "  ";
			json = JsonFormatter.PrettyPrint (json);
			JsonFormatter.Indent = originalIndent;

			File.WriteAllText (psdkConfigPath, json);
		}

		#endregion

		#endregion
	}
}