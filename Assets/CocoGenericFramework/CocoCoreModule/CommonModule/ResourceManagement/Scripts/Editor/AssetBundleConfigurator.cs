using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class AssetBundleConfigurator
	{
		#region Init

		public List<ResourceEditorAssetBundleData> InitAssetBundleDatas (string[] assetBundleNames, ConfigData configData)
		{
			var assetBundleEditorDatas = new List<ResourceEditorAssetBundleData> ();
			if (assetBundleNames == null) {
				return assetBundleEditorDatas;
			}

			foreach (var assetBundleName in assetBundleNames) {
				var assetBundleEditorData = new ResourceEditorAssetBundleData { Id = assetBundleName };
				FillAssetBundleLocationInfo (assetBundleEditorData, configData);
				assetBundleEditorDatas.Add (assetBundleEditorData);
			}

			return assetBundleEditorDatas;
		}

		private void FillAssetBundleLocationInfo (ResourceEditorAssetBundleData data, ConfigData configData)
		{
			if (configData == null) {
				return;
			}

			var assetBundleData = configData.GetAssetBundleData (data.Id);
			if (assetBundleData == null || assetBundleData.InLocation == null) {
				return;
			}

			data.Location = assetBundleData.InLocation.Location;
			if (data.Location == LocationType.ODR) {
				data.OdrTag = assetBundleData.InLocation.Id;
			}
		}

		#endregion


		#region Build

		public bool BuildAssetBundles (List<LocationData> locationDatas, string targetPlatform, ResourceEditorAssetBundleBuildOptions buildOptions)
		{
			var outputPath = ResourceOutputLocation.GetAssetBundleFullRootPath (targetPlatform);

			var options = BuildAssetBundleOptions.None;
			switch (buildOptions.CompressOption) {
			case CompressOption.Uncompressed:
				options |= BuildAssetBundleOptions.UncompressedAssetBundle;
				break;
			case CompressOption.ChunkBasedLZ4:
				options |= BuildAssetBundleOptions.ChunkBasedCompression;
				break;
			}

			if (!Directory.Exists (outputPath)) {
				Directory.CreateDirectory (outputPath);
			}

			if (buildOptions.ClearFolders) {
				ResourceEditorHelper.ClearDirectory (outputPath);
			}

			var buildManifest = BuildPipeline.BuildAssetBundles (outputPath, options, buildOptions.BuildTarget);
			if (buildManifest == null) {
				return false;
			}

			MoveAssetBundleToVirtualLocations (locationDatas, outputPath, targetPlatform, buildOptions.ClearFolders);
			return true;
		}

		private void MoveAssetBundleToVirtualLocations (List<LocationData> locationDatas, string originDirectory, string targetPlatform, bool clearFolders)
		{
			var virtualStreamingPath = ResourceStreamingLocation.GetVirtualAssetBundleFullRootPath (targetPlatform);
			var virtualServerPath = ResourceServerLocation.GetVirtualAssetBundleFullRootPath (targetPlatform);
			var virtualOdrPath = ResourceODRLocation.VirtualFullRootPath;

			if (clearFolders) {
				ResourceEditorHelper.ClearDirectory (virtualStreamingPath);
				ResourceEditorHelper.ClearDirectory (virtualServerPath);
				if (targetPlatform == ResourceSettings.PLATFORM_DIRECTORY_IOS) {
					ResourceEditorHelper.ClearDirectory (virtualOdrPath);
				}
			}

			foreach (var locationData in locationDatas) {
				var targetDirectory = string.Empty;

				switch (locationData.Location) {
				case LocationType.Streaming:
					targetDirectory = virtualStreamingPath;
					break;
				case LocationType.Server:
					targetDirectory = virtualServerPath;
					break;
				case LocationType.ODR:
					targetDirectory = Path.Combine (virtualOdrPath, locationData.Id);
					break;
				}

				if (string.IsNullOrEmpty (targetDirectory)) {
					continue;
				}

				if (!Directory.Exists (targetDirectory)) {
					Directory.CreateDirectory (targetDirectory);
				}

				foreach (var assetBundleData in locationData.AssetBundleDatas) {
					MoveAssetBundle (assetBundleData.Id, originDirectory, targetDirectory);
				}
			}
		}

		private void MoveAssetBundle (string assetBundleTag, string originDirectory, string targetDirectory)
		{
			var originPath = Path.Combine (originDirectory, assetBundleTag);
			var targetPath = Path.Combine (targetDirectory, assetBundleTag);
			ResourceEditorHelper.MoveFile (originPath, targetPath);

			// manifest
			var manifestName = assetBundleTag + ".manifest";
			originPath = Path.Combine (originDirectory, manifestName);
			targetPath = Path.Combine (targetDirectory, manifestName);
			ResourceEditorHelper.MoveFile (originPath, targetPath);
		}

		#endregion


		#region Target

		public bool CopyAssetBundlesToStreamingTarget (List<LocationData> locationDatas, string targetPlatform)
		{
			var virtualStreamingPath = ResourceStreamingLocation.GetVirtualAssetBundleFullRootPath (targetPlatform);
			if (!Directory.Exists (virtualStreamingPath)) {
				return true;
			}

			var targetStreamingPath = ResourceStreamingLocation.GetAssetBundleFullRootPath (targetPlatform);

			// copy
			foreach (var locationData in locationDatas) {
				if (locationData.Location != LocationType.Streaming) {
					continue;
				}

				if (!Directory.Exists (targetStreamingPath)) {
					Directory.CreateDirectory (targetStreamingPath);
				}

				foreach (var assetBundleData in locationData.AssetBundleDatas) {
					CopyAssetBundle (assetBundleData.Id, virtualStreamingPath, targetStreamingPath);
				}
			}

			return true;
		}

		private void CopyAssetBundle (string assetBundleTag, string originDirectory, string targetDirectory)
		{
			var originPath = Path.Combine (originDirectory, assetBundleTag);
			if (!File.Exists (originPath)) {
				Debug.LogErrorFormat ("{0}->CopyAssetBundle: asset bundle [{1}] NOT found in path [{2}]", GetType ().Name, assetBundleTag, originPath);
				return;
			}

			var targetPath = Path.Combine (targetDirectory, assetBundleTag);
			File.Copy (originPath, targetPath, true);

			// manifest
			var manifestName = assetBundleTag + ".manifest";
			originPath = Path.Combine (originDirectory, manifestName);
			if (!File.Exists (originPath)) {
				return;
			}

			targetPath = Path.Combine (targetDirectory, manifestName);
			File.Copy (originPath, targetPath, true);
		}

		#endregion


		#region Draw On GUI

		public Vector2 DrawAssetBundleDatas (List<ResourceEditorAssetBundleData> assetBundleEditorDatas, Vector2 scrollPosition)
		{
			scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition, GUI.skin.box);

			if (assetBundleEditorDatas != null) {
				foreach (var assetBundleEditorData in assetBundleEditorDatas) {
					EditorGUILayout.BeginHorizontal ();

					var location = (LocationType)EditorGUILayout.EnumPopup (assetBundleEditorData.Id, assetBundleEditorData.Location);
					SwitchAssetBundleDatasLocation (assetBundleEditorData, location);

					if (assetBundleEditorData.Location == LocationType.ODR) {
						assetBundleEditorData.OdrTag = EditorGUILayout.TextField (assetBundleEditorData.OdrTag);
					} else {
						EditorGUILayout.LabelField ("");
					}


					EditorGUILayout.EndHorizontal ();
				}
			}

			EditorGUILayout.EndScrollView ();

			return scrollPosition;
		}

		public void SwitchAllAssetBundleDataLocations (List<ResourceEditorAssetBundleData> assetBundleEditorDatas, LocationType locationType)
		{
			foreach (var assetBundleEditorData in assetBundleEditorDatas) {
				SwitchAssetBundleDatasLocation (assetBundleEditorData, locationType);
			}
		}

		private void SwitchAssetBundleDatasLocation (ResourceEditorAssetBundleData assetBundleEditorData, LocationType locationType)
		{
			if (locationType == assetBundleEditorData.Location) {
				return;
			}

			assetBundleEditorData.Location = locationType;

			if (locationType == LocationType.ODR) {
				assetBundleEditorData.OdrTag = "odr_" + assetBundleEditorData.Id;
			}
		}

		#endregion
	}
}