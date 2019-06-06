using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class ResourceEditorWindow : EditorWindow
	{
		[MenuItem ("CocoPlay/Configurator/Resource Configurator...", false, 1)]
		private static void Init ()
		{
			var window = GetWindow<ResourceEditorWindow> ("Resource Configurator", true);
			window.minSize = new Vector2 (880, 480);
		}

		private void OnEnable ()
		{
			InitEditorConfigData ();
		}

		private void OnDisable ()
		{
			CleanEditorConfigData ();
		}

		private void OnGUI ()
		{
			DrawEditorConfigData ();
		}


		#region Editor Config

		private ResourceEditorConfigData _editorConfigData;
		private string[] _platforms;

		private ResourceEditorConfigData LoadEditorConfigData ()
		{
			var fullPath = ResourceOutputLocation.EditorConfigFullFilePath;
			var editorConfigData = CocoData.LoadFromJsonFile<ResourceEditorConfigData> (fullPath) ?? new ResourceEditorConfigData ();

			if (editorConfigData.PlatformDatas.Count <= 0) {
				FillDefaultPlatformDatas (editorConfigData.PlatformDatas);
			}

			return editorConfigData;
		}

		private void SaveEditorConfigData (ResourceEditorConfigData editorConfigData)
		{
			var fullPath = ResourceOutputLocation.EditorConfigFullFilePath;
			CocoData.SaveToJsonFile (editorConfigData, fullPath, true);
		}

		private void InitEditorConfigData ()
		{
			_editorConfigData = LoadEditorConfigData ();

			var assetBundleNames = AssetDatabase.GetAllAssetBundleNames ();

			foreach (var platformData in _editorConfigData.PlatformDatas) {
				// load config
				LoadConfigData (platformData);

				// asset bundle datas
				InitAssetBundleDatas (platformData, assetBundleNames);
			}

			// current platform
			InitCurrPlatform ();
		}

		private void CleanEditorConfigData ()
		{
			foreach (var platformData in _editorConfigData.PlatformDatas) {
				// save config
				if (platformData.ConfigData != null) {
					SaveConfigData (platformData);
				}
			}

			SaveEditorConfigData (_editorConfigData);
			_editorConfigData = null;

			AssetDatabase.Refresh ();
		}

		private void DrawEditorConfigData ()
		{
			EditorGUILayout.BeginVertical ();

			// platform selection
			DrawPlatformSelection ();
			EditorGUILayout.Separator ();

			// platform content
			var currEditorPlatformData = _editorConfigData.PlatformDatas [_editorConfigData.CurrPlatformIndex];
			DrawPlatformData (currEditorPlatformData);

			EditorGUILayout.EndVertical ();
		}

		private void InitCurrPlatform ()
		{
			var count = _editorConfigData.PlatformDatas.Count;
			_platforms = new string[count];
			for (var i = 0; i < count; i++) {
				var editorPlatformData = _editorConfigData.PlatformDatas [i];
				_platforms [i] = editorPlatformData.Platform;
			}

			if (_editorConfigData.CurrPlatformIndex < 0 || _editorConfigData.CurrPlatformIndex >= count) {
				_editorConfigData.CurrPlatformIndex = 0;
			}
		}

		private void DrawPlatformSelection ()
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space ();
			_editorConfigData.CurrPlatformIndex = EditorGUILayout.Popup ("Platform:", _editorConfigData.CurrPlatformIndex, _platforms);
			EditorGUILayout.Space ();
			EditorGUILayout.EndHorizontal ();
		}

		#endregion


		#region Platform Data

		private void FillDefaultPlatformDatas (List<ResourceEditorPlatformData> platformDatas)
		{
			var platformData = new ResourceEditorPlatformData {
				Platform = ResourceSettings.PLATFORM_DIRECTORY_IOS,
				AssetBundleBuildOptions = { BuildTarget = BuildTarget.iOS }
			};
			platformDatas.Add (platformData);

			platformData = new ResourceEditorPlatformData {
				Platform = ResourceSettings.PLATFORM_DIRECTORY_GP,
				AssetBundleBuildOptions = { BuildTarget = BuildTarget.Android }
			};
			platformDatas.Add (platformData);
		}

		private void DrawPlatformData (ResourceEditorPlatformData editorPlatformData)
		{
			EditorGUILayout.BeginVertical ();

			EditorGUILayout.BeginHorizontal ();
			// asset bundle
			DrawAssetBundleLocations (editorPlatformData);
			EditorGUILayout.Separator ();
			// config
			DrawConfigPreview (editorPlatformData);
			EditorGUILayout.EndHorizontal ();

			//EditorGUILayout.BeginHorizontal ();
			// build
			DrawAssetBundleBuilds (editorPlatformData);
			// copy
			DrawTargetCopy (editorPlatformData);
			//EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
		}

		#endregion


		#region Asset Bundle

		private readonly AssetBundleConfigurator _assetBundleConfigurator = new AssetBundleConfigurator ();

		private Vector2 _assetBundleScrollPosition = Vector2.zero;
		private LocationType _oneKeyLocation = LocationType.ODR;

		private void DrawAssetBundleLocations (ResourceEditorPlatformData editorPlatformData)
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);

			// location settings
			ResourceEditorHelper.WideLabelField ("Locations Settings: ----", EditorStyles.boldLabel);
			_assetBundleScrollPosition = _assetBundleConfigurator.DrawAssetBundleDatas (editorPlatformData.EditorAssetBundleDatas, _assetBundleScrollPosition);


			// one-key set
			EditorGUILayout.BeginHorizontal (GUI.skin.box);
			_oneKeyLocation = (LocationType)EditorGUILayout.EnumPopup ("One-key Location:", _oneKeyLocation);
			if (GUILayout.Button ("One-key Switch")) {
				_assetBundleConfigurator.SwitchAllAssetBundleDataLocations (editorPlatformData.EditorAssetBundleDatas, _oneKeyLocation);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
		}

		private void DrawAssetBundleBuilds (ResourceEditorPlatformData editorPlatformData)
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);

			ResourceEditorHelper.WideLabelField ("Asset Bundle Build: ----", EditorStyles.boldLabel);

			// options
			EditorGUILayout.BeginVertical (GUI.skin.box);
			var buildOptions = editorPlatformData.AssetBundleBuildOptions;
			buildOptions.ClearFolders = EditorGUILayout.Toggle ("Clear Folders:", buildOptions.ClearFolders);
			EditorGUILayout.BeginHorizontal ();
			buildOptions.CompressOption = (CompressOption)EditorGUILayout.EnumPopup ("Compress Options:", buildOptions.CompressOption);
			buildOptions.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup ("Build Target:", buildOptions.BuildTarget);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();

			if (GUILayout.Button ("Build Asset Bundles (in virtual)")) {
				EditorApplication.delayCall += () => BuildAssetBundles (editorPlatformData);
			}

			EditorGUILayout.EndVertical ();
		}

		private void InitAssetBundleDatas (ResourceEditorPlatformData editorPlatformData, string[] assetBundleNames)
		{
			editorPlatformData.EditorAssetBundleDatas = _assetBundleConfigurator.InitAssetBundleDatas (assetBundleNames, editorPlatformData.ConfigData);
		}

		private void BuildAssetBundles (ResourceEditorPlatformData editorPlatformData)
		{
			var locationDatas = editorPlatformData.ConfigData.LocationDatas;
			var options = editorPlatformData.AssetBundleBuildOptions;
			var result = _assetBundleConfigurator.BuildAssetBundles (locationDatas, editorPlatformData.Platform, options);

			AssetDatabase.Refresh ();

			var message = string.Format ("Asset bundles build : [{0}].", result ? "Success" : "Failed");
			EditorUtility.DisplayDialog ("Info", message, "OK");
		}

		#endregion


		#region Config

		private readonly ConfigDataConfigurator _configDataConfigurator = new ConfigDataConfigurator ();

		private Vector2 _configShowPosition = Vector2.zero;

		private void DrawConfigPreview (ResourceEditorPlatformData editorPlatformData)
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);

			// preview
			ResourceEditorHelper.WideLabelField ("Config Preview: ----", EditorStyles.boldLabel);
			_configShowPosition = _configDataConfigurator.DrawConfigData (editorPlatformData.ConfigData, editorPlatformData.ConfigFoldout, _configShowPosition);

			// update
			EditorGUILayout.BeginHorizontal (GUI.skin.box);
			var generateOptions = editorPlatformData.ConfigGenerateOptions;
			generateOptions.CollectSprite = EditorGUILayout.Toggle ("Collect Sprite:", generateOptions.CollectSprite);
			if (GUILayout.Button ("Update Config Data (in virtual)")) {
				EditorApplication.delayCall += () => UpdateConfigData (editorPlatformData);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
		}

		private void LoadConfigData (ResourceEditorPlatformData editorPlatformData)
		{
			editorPlatformData.ConfigData = _configDataConfigurator.LoadConfigData (editorPlatformData.Platform);
			editorPlatformData.ConfigFoldout.Contents.Clear ();
		}

		private void SaveConfigData (ResourceEditorPlatformData editorPlatformData)
		{
			_configDataConfigurator.SaveConfigData (editorPlatformData.ConfigData, editorPlatformData.Platform);
		}

		private void UpdateConfigData (ResourceEditorPlatformData editorPlatformData)
		{
			if (editorPlatformData.EditorAssetBundleDatas == null) {
				return;
			}

			var locationDatas = new Dictionary<string, LocationData> ();

			if (editorPlatformData.EditorAssetBundleDatas != null) {
				foreach (var assetBundleEditorData in editorPlatformData.EditorAssetBundleDatas) {
					var assetBundleData = _configDataConfigurator.GenerateAssetBundleData (assetBundleEditorData.Id, editorPlatformData.ConfigGenerateOptions);
					if (assetBundleData == null) {
						continue;
					}

					// add in location data
					var locationId = _configDataConfigurator.GetLocationId (assetBundleEditorData.Location, assetBundleEditorData.OdrTag);
					LocationData locationData;
					if (locationDatas.ContainsKey (locationId)) {
						locationData = locationDatas [locationId];
					} else {
						locationData = new LocationData { Id = locationId, Location = assetBundleEditorData.Location };
						locationDatas.Add (locationId, locationData);
					}

					locationData.AssetBundleDatas.Add (assetBundleData);
				}
			}

			editorPlatformData.ConfigData.LocationDatas = new List<LocationData> (locationDatas.Values);
			editorPlatformData.ConfigData.Init ();
			editorPlatformData.ConfigFoldout.Contents.Clear ();
			SaveConfigData (editorPlatformData);

			EditorUtility.DisplayDialog ("Info", "Update Config to Virtual Streaming: Finished.", "OK");
		}

		#endregion


		#region Target Copy

		private void DrawTargetCopy (ResourceEditorPlatformData editorPlatformData)
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);

			ResourceEditorHelper.WideLabelField ("Target Copy: ----", EditorStyles.boldLabel);

			EditorGUILayout.BeginVertical (GUI.skin.box);
			var copyOptions = editorPlatformData.TargetCopyOptions;
			copyOptions.ClearFolders = EditorGUILayout.Toggle ("Clear Folders:", copyOptions.ClearFolders);
			copyOptions.ClearOtherPlatforms = EditorGUILayout.Toggle ("Clear Other Platforms:", copyOptions.ClearOtherPlatforms);
			if (GUILayout.Button ("Copy to Streaming Assets")) {
				EditorApplication.delayCall += () => CopyToTarget (editorPlatformData);
			}
			EditorGUILayout.EndVertical ();

			EditorGUILayout.EndVertical ();
		}

		private void CopyToTarget (ResourceEditorPlatformData editorPlatformData)
		{
			var copyOptions = editorPlatformData.TargetCopyOptions;
			var result = CopyToStreamingAssets (editorPlatformData.Platform, copyOptions, _configDataConfigurator, _assetBundleConfigurator);
			var message = string.Format ("Copy to streaming assets : [{0}].", result ? "Success" : "Failed");
			EditorUtility.DisplayDialog ("Info", message, "OK");
		}

		public static bool CopyToStreamingAssets (string platform, ResourceEditorTargetCopyOptions copyOptions,
			ConfigDataConfigurator configDataConfigurator, AssetBundleConfigurator assetBundleConfigurator)
		{
			var configData = configDataConfigurator.LoadConfigData (platform);
			if (configData == null) {
				return false;
			}

			// options
			var rootDirectory = ResourceStreamingLocation.FullRootPath;
			if (copyOptions.ClearFolders) {
				var platformRootDirectory = Path.Combine (rootDirectory, platform);
				ResourceEditorHelper.ClearDirectory (platformRootDirectory);
			}
			if (copyOptions.ClearOtherPlatforms) {
				DeleteOtherPlatformDirectory (ResourceStreamingLocation.FullRootPath, platform);
			}

			// copy config
			var result = configDataConfigurator.CopyConfigToTarget (platform);

			// config asset bundles
			var locationDatas = configData.LocationDatas;
			result &= assetBundleConfigurator.CopyAssetBundlesToStreamingTarget (locationDatas, platform);

			return result;
		}

		private static void DeleteOtherPlatformDirectory (string rootDirectory, string targetPlatform)
		{
			var otherPlatforms = ResourceEditorHelper.CollectBrotherDirectoryNames (rootDirectory, targetPlatform);
			if (otherPlatforms == null) {
				return;
			}

			foreach (var otherPlatform in otherPlatforms) {
				var otherPlatformRootDirectory = ResourceSettings.CombinePath (rootDirectory, otherPlatform);
				ResourceEditorHelper.DeleteDirectory (otherPlatformRootDirectory);
			}
		}

		#endregion
	}
}