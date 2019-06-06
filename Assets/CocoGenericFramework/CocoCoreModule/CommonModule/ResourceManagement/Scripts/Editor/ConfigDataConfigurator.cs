using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class ConfigDataConfigurator
	{
		#region Load / Save

		public ConfigData LoadConfigData (string platformDirectory)
		{
			var configFilePath = ResourceStreamingLocation.GetVirtualConfigFullFilePath (platformDirectory);
			var configData = CocoData.LoadFromJsonFile<ConfigData> (configFilePath) ?? new ConfigData ();
			configData.Init ();
			return configData;
		}

		public void SaveConfigData (ConfigData configData, string platformDirectory)
		{
			if (configData == null) {
				return;
			}

			var configFilePath = ResourceStreamingLocation.GetVirtualConfigFullFilePath (platformDirectory);
			CocoData.SaveToJsonFile (configData, configFilePath, true);
		}

		#endregion


		#region Generate

		public string GetLocationId (LocationType locationType, string odrTag)
		{
			switch (locationType) {
			case LocationType.Streaming:
				return ResourceSettings.LOCATION_ID_STREAMING;
			case LocationType.Server:
				return ResourceSettings.LOCATION_ID_SERVER;
			}

			return odrTag;
		}

		public AssetBundleData GenerateAssetBundleData (string assetBundleName, ResourceEditorConfigGenerateOptions generateOptions)
		{
			var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle (assetBundleName);
			if (assetPaths == null) {
				return null;
			}

			var assetDic = new Dictionary<string, string> ();
			var topDirectoryKey = Path.DirectorySeparatorChar + ResourceSettings.RUNTIME_RESOURCES_DIRECTORY + Path.DirectorySeparatorChar;

			var resourceDatas = new List<ResourceData> ();
			var sceneDatas = new List<SceneData> ();

			foreach (var assetPath in assetPaths) {
				var assetType = AssetDatabase.GetMainAssetTypeAtPath (assetPath);

				if (assetType == typeof(SceneAsset)) {
					// scene
					var sceneName = Path.GetFileNameWithoutExtension (assetPath);
					AddSceneData (sceneName, assetPath, assetDic, sceneDatas);
				} else {
					var runtimeType = GetAssetRuntimeType (assetType);

					// resource
					var assetId = GetResourceId (assetPath, topDirectoryKey);
					if (string.IsNullOrEmpty (assetId)) {
						continue;
					}

					AddResourceData (assetId, runtimeType, assetPath, assetDic, resourceDatas);

					if (generateOptions.CollectSprite && runtimeType == typeof(Texture2D)) {
						AddSubResourceData (assetId, typeof(Sprite), assetPath, assetDic, resourceDatas);
					}
				}
			}

			if (resourceDatas.Count <= 0 && sceneDatas.Count <= 0) {
				return null;
			}

			var dependencies = new List<string> (AssetDatabase.GetAssetBundleDependencies (assetBundleName, true));

			var assetBundleData = new AssetBundleData {
				Id = assetBundleName,
				ResourceDatas = resourceDatas,
				SceneDatas = sceneDatas,
				Dependencies = dependencies
			};
			return assetBundleData;
		}

		private System.Type GetAssetRuntimeType (System.Type assetType)
		{
			if (assetType == null) {
				return null;
			}

			var fullName = assetType.FullName;
			if (string.IsNullOrEmpty (fullName)) {
				return assetType;
			}

			if (!fullName.StartsWith ("UnityEditor")) {
				return assetType;
			}

			return GetAssetRuntimeType (assetType.BaseType);
		}

		private string GetResourceId (string assetPath, string topDirectoryKey)
		{
			var index = assetPath.LastIndexOf (topDirectoryKey, System.StringComparison.Ordinal);
			if (index < 0) {
				return Path.GetFileNameWithoutExtension (assetPath);
			}

			index += topDirectoryKey.Length;
			var length = assetPath.Length - index - Path.GetExtension (assetPath).Length;
			var resourceId = assetPath.Substring (index, length);
			return resourceId;
		}

		private void AddResourceData (string assetId, System.Type assetType, string assetPath, Dictionary<string, string> assetDic, List<ResourceData> inDatas)
		{
			var key = ConfigData.GetResourceKey (assetId, assetType);
			if (assetDic.ContainsKey (key)) {
				Debug.LogErrorFormat ("{0}->AddResourceData: [{1}]: same asset key [{2}] already exists for [{3}].",
					GetType ().Name, assetPath, key, assetDic [key]);
				return;
			}

			inDatas.Add (new ResourceData { Path = assetId, Type = assetType.FullName });
			assetDic.Add (key, assetPath);
		}

		private void AddSubResourceData (string assetId, System.Type assetType, string assetPath, Dictionary<string, string> assetDic,
			List<ResourceData> inDatas)
		{
			var subAsset = AssetDatabase.LoadAssetAtPath (assetPath, assetType);
			if (subAsset == null) {
				return;
			}

			AddResourceData (assetId, assetType, assetPath, assetDic, inDatas);
		}

		private void AddSceneData (string sceneName, string assetPath, Dictionary<string, string> assetDic, List<SceneData> inDatas)
		{
			var key = ConfigData.GetResourceKey (sceneName, typeof(SceneAsset));
			if (assetDic.ContainsKey (key)) {
				Debug.LogErrorFormat ("{0}->AddSceneData: [{1}]: same scene key [{2}] already exists for [{3}].",
					GetType ().Name, assetPath, key, assetDic [key]);
				return;
			}

			inDatas.Add (new SceneData { SceneName = sceneName });
			assetDic.Add (key, assetPath);
		}

		#endregion


		#region Copy

		public bool CopyConfigToTarget (string targetPlatform)
		{
			var originPath = ResourceStreamingLocation.GetVirtualConfigFullFilePath (targetPlatform);
			if (!File.Exists (originPath)) {
				return false;
			}

			var targetPath = ResourceStreamingLocation.GetConfigFullFilePath (targetPlatform);
			var targetDirectory = Path.GetDirectoryName (targetPath);
			if (targetDirectory == null) {
				return false;
			}
			if (!Directory.Exists (targetDirectory)) {
				Directory.CreateDirectory (targetDirectory);
			}

			File.Copy (originPath, targetPath, true);
			return true;
		}

		#endregion


		#region Draw On GUI

		public Vector2 DrawConfigData (ConfigData configData, ResourceEditorFoldout<ResourceEditorFoldout<ResourceEditorFoldout<bool>>> foldout,
			Vector2 scrollPosition)
		{
			if (configData == null) {
				return scrollPosition;
			}

			scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition, GUI.skin.box);

			var locationFoldouts = foldout.Contents;
			foreach (var locationData in configData.LocationDatas) {
				var locationFoldout = GetOrCreateDictionaryValue (locationFoldouts, locationData.Id);
				DrawLocationData (locationData, locationFoldout);
			}

			EditorGUILayout.EndScrollView ();

			return scrollPosition;
		}

		private void DrawLocationData (LocationData locationData, ResourceEditorFoldout<ResourceEditorFoldout<bool>> foldout)
		{
			EditorGUILayout.BeginVertical ();

			var locationLabel = string.Format ("Location: {0} [{1}]", locationData.Id, locationData.Location);
			foldout.IsOut = EditorGUILayout.Foldout (foldout.IsOut, locationLabel);

			if (foldout.IsOut) {
				EditorGUI.indentLevel++;
				var assetBundleFoldouts = foldout.Contents;
				foreach (var assetBundleData in locationData.AssetBundleDatas) {
					var assetBundleFoldout = GetOrCreateDictionaryValue (assetBundleFoldouts, assetBundleData.Id);
					DrawAssetBundleData (assetBundleData, assetBundleFoldout);
				}
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndVertical ();
		}

		private void DrawAssetBundleData (AssetBundleData assetBundleData, ResourceEditorFoldout<bool> foldout)
		{
			var assetBundleLabel = string.Format ("[asset bundle] - {0}", assetBundleData.Id);
			foldout.IsOut = EditorGUILayout.Foldout (foldout.IsOut, assetBundleLabel);
			var dataFoldouts = foldout.Contents;

			if (!foldout.IsOut) {
				return;
			}

			EditorGUI.indentLevel++;

			DrawListDatas (dataFoldouts, "resources", assetBundleData.ResourceDatas, data => data.Type, data => data.Path);
			DrawListDatas (dataFoldouts, "scenes", assetBundleData.SceneDatas, data => "** Scene **", data => data.SceneName);
			DrawListDatas (dataFoldouts, "dependencies", assetBundleData.Dependencies, data => "** AssetBundle **", data => data);

			EditorGUI.indentLevel--;
		}

		private void DrawListDatas<T> (Dictionary<string, bool> foldouts, string title,
			ICollection<T> datas, System.Func<T, string> dataLabelKeyFunc, System.Func<T, string> dataLabelValueFunc)
		{
			if (datas.Count <= 0) {
				return;
			}

			// title
			var foldout = GetOrCreateDictionaryValue (foldouts, title);
			var titleLabel = string.Format ("- {0}: [{1}]", title, datas.Count);
			foldout = EditorGUILayout.Foldout (foldout, titleLabel);
			foldouts [title] = foldout;

			// data
			if (!foldout) {
				return;
			}

			EditorGUI.indentLevel++;
			foreach (var data in datas) {
				var label = string.Format ("[{0}] - {1}", dataLabelKeyFunc (data), dataLabelValueFunc (data));
				EditorGUILayout.LabelField (label);
			}
			EditorGUI.indentLevel--;
		}

		private T GetOrCreateDictionaryValue<T> (Dictionary<string, T> dict, string key) where T : new ()
		{
			if (dict.ContainsKey (key)) {
				return dict [key];
			}

			var val = new T ();
			dict.Add (key, val);
			return val;
		}

		#endregion
	}
}