using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Build
{
	public class PreprocessSettingsData : ScriptableObject
	{
		#region Global

		private static PreprocessSettingsData _instance;

		public static PreprocessSettingsData Instance {
			get {
				if (_instance == null) {
					_instance = AssetDatabase.LoadAssetAtPath<PreprocessSettingsData> (ASSET_PATH);
				}
				if (_instance == null) {
					_instance = CreateInstance<PreprocessSettingsData> ();
					_instance.InitPlatformSettings ();
					CreateAssetFolder (ASSET_FOLDER);
					AssetDatabase.CreateAsset (_instance, ASSET_PATH);
				}
				return _instance;
			}
		}

		private const string FILE_NAME = "PreprocessSettingsData.asset";

		private const string ASSET_FOLDER = "Assets/_Game/CocoPlay/GameCommon/_MultiPlatformFolder/Editor";

		private const string ASSET_PATH = ASSET_FOLDER + "/" + FILE_NAME;

		private static void CreateAssetFolder (string folder)
		{
			if (AssetDatabase.IsValidFolder (folder) || string.IsNullOrEmpty (folder)) {
				return;
			}

			var parentFolder = Path.GetDirectoryName (folder);
			CreateAssetFolder (parentFolder);

			var subFolder = Path.GetFileName (folder);
			AssetDatabase.CreateFolder (parentFolder, subFolder);
		}

		#endregion


		#region Platform

		public enum Platform
		{
			Unknown = 0,
			iOS = 1,
			GooglePlay = 2,
			Amazon = 3
		}


		[Serializable]
		public class PlatformSettingsData
		{
			public Platform Platform;
			public bool enableObfuscator;
		}


		[SerializeField]
		private List<PlatformSettingsData> _platformSettings = new List<PlatformSettingsData> ();

		private void InitPlatformSettings ()
		{
			_platformSettings.Clear ();

			AddPlatformSettings (new PlatformSettingsData {
				Platform = Platform.iOS,
				enableObfuscator = true
			});
			AddPlatformSettings (new PlatformSettingsData {
				Platform = Platform.GooglePlay,
				enableObfuscator = false
			});
			AddPlatformSettings (new PlatformSettingsData {
				Platform = Platform.Amazon,
				enableObfuscator = false
			});
		}

		private void AddPlatformSettings (PlatformSettingsData data)
		{
			_platformSettings.Add (data);
		}

		public PlatformSettingsData GetPlatformSettings (Platform platform)
		{
			foreach (var settingsData in _platformSettings) {
				if (settingsData.Platform == platform) {
					return settingsData;
				}
			}

			return null;
		}

		#endregion
	}
}