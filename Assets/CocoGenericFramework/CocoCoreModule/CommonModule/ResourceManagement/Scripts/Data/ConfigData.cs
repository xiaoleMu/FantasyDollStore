using System.Collections.Generic;

namespace CocoPlay.ResourceManagement
{
	public class ConfigData
	{
		public List<LocationData> LocationDatas = new List<LocationData> ();

		public void Init ()
		{
			InitDataDic ();
			InitAssetBundleDependencies ();
		}


		#region Key

		private static string GetResourceKey (string path, string type)
		{
			return string.Format ("{0}|{1}", type, path);
		}

		public static string GetResourceKey (string path, System.Type type)
		{
			return GetResourceKey (path, type.FullName);
		}

		#endregion


		#region Dictionary

		private readonly Dictionary<string, LocationData> _locationDataDic = new Dictionary<string, LocationData> ();

		private readonly Dictionary<string, AssetBundleData> _assetBundleDataDic = new Dictionary<string, AssetBundleData> ();

		private readonly Dictionary<string, ResourceData> _resourceDataDic = new Dictionary<string, ResourceData> ();

		private readonly Dictionary<string, SceneData> _sceneDataDic = new Dictionary<string, SceneData> ();

		private void InitDataDic ()
		{
			_locationDataDic.Clear ();
			_assetBundleDataDic.Clear ();
			_resourceDataDic.Clear ();
			_sceneDataDic.Clear ();

			foreach (var locationData in LocationDatas) {
				if (_locationDataDic.ContainsKey (locationData.Id)) {
					ResourceDebug.Log ("{0}->InitData: location [{1}] duplicated !", GetType ().Name, locationData.Id);
					continue;
				}

				_locationDataDic.Add (locationData.Id, locationData);

				// collect asset bundle data
				foreach (var assetBundleData in locationData.AssetBundleDatas) {
					if (_assetBundleDataDic.ContainsKey (assetBundleData.Id)) {
						ResourceDebug.Log ("{0}->InitData: asset bundle [{1}] duplicated !", GetType ().Name, assetBundleData.Id);
						continue;
					}

					assetBundleData.InLocation = locationData;
					_assetBundleDataDic.Add (assetBundleData.Id, assetBundleData);

					// collect resource data
					foreach (var resourceData in assetBundleData.ResourceDatas) {
						var key = GetResourceKey (resourceData.Path, resourceData.Type);
						if (_resourceDataDic.ContainsKey (key)) {
							ResourceDebug.Log ("{0}->InitData: resource [{1}({2})] duplicated !", GetType ().Name, key, assetBundleData.Id);
							continue;
						}

						resourceData.InAssetBundle = assetBundleData;
						_resourceDataDic.Add (key, resourceData);
					}

					// collect scene data
					foreach (var sceneData in assetBundleData.SceneDatas) {
						if (_sceneDataDic.ContainsKey (sceneData.SceneName)) {
							ResourceDebug.Log ("{0}->InitData: scene [{1}({2})] duplicated !", GetType ().Name, sceneData.SceneName,
								assetBundleData.Id);
							continue;
						}

						sceneData.InAssetBundle = assetBundleData;
						_sceneDataDic.Add (sceneData.SceneName, sceneData);
					}
				}
			}
		}

		private void InitAssetBundleDependencies ()
		{
			foreach (var assetBundleData in _assetBundleDataDic.Values) {
				if (assetBundleData.Dependencies == null) {
					assetBundleData.DependencyDatas = null;
					continue;
				}

				var dependencyIds = assetBundleData.Dependencies;
				var dependencyDatas = new List<AssetBundleData> (dependencyIds.Count);

				foreach (var dependencyId in dependencyIds) {
					var dependencyData = GetAssetBundleData (dependencyId);
					if (dependencyData == null) {
						ResourceDebug.Log ("{0}->GetDependencyDatas: can NOT found dependency data [{1}] for asset bundle [{2}]!",
							GetType ().Name, dependencyId, assetBundleData.Id);
						continue;
					}

					dependencyDatas.Add (dependencyData);
				}

				assetBundleData.DependencyDatas = dependencyDatas;
			}
		}

		public ResourceData GetResourceData (string path, System.Type type)
		{
			var key = GetResourceKey (path, type);
			return _resourceDataDic.ContainsKey (key) ? _resourceDataDic [key] : null;
		}

		public SceneData GetSceneData (string sceneName)
		{
			return _sceneDataDic.ContainsKey (sceneName) ? _sceneDataDic [sceneName] : null;
		}

		public AssetBundleData GetAssetBundleData (string assetBundleName)
		{
			return _assetBundleDataDic.ContainsKey (assetBundleName) ? _assetBundleDataDic [assetBundleName] : null;
		}

		public LocationData GetLocationData (string locationId)
		{
			return _locationDataDic.ContainsKey (locationId) ? _locationDataDic [locationId] : null;
		}

		#endregion
	}
}