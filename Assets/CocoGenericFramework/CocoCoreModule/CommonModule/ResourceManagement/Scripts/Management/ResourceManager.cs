using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CocoPlay.ResourceManagement
{
	public class ResourceManager : MonoBehaviour
	{
		#region Settings

		public static string ServerRootUri {
			get { return ResourceServerLocation.RuntimeRootUri; }
			set { ResourceServerLocation.RuntimeRootUri = value; }
		}

		private static string _platformDirectory = string.Empty;

		public static string PlatformDirectory {
			get {
				if (!string.IsNullOrEmpty (_platformDirectory)) {
					return _platformDirectory;
				}

				ResourceDebug.Log ("ResourceManager->PlatformDirectory: platform directory NOT be set !");
				return string.Empty;
			}
			set { _platformDirectory = value; }
		}

		#endregion


		#region Config

		private static ConfigData _config;

		public static bool Init (string configJson)
		{
			_config = JsonMapper.ToObject<ConfigData> (configJson);
			if (_config == null) {
				return false;
			}

			_config.Init ();
			RegisterShaderFixAction ();
			return true;
		}

		private static ResourceData GetResourceData (string path, System.Type type)
		{
			return _config != null ? _config.GetResourceData (path, type) : null;
		}

		private static SceneData GetSceneData (string sceneName)
		{
			return _config != null ? _config.GetSceneData (sceneName) : null;
		}

		private static LocationData GetLocationData (string locationId)
		{
			return _config != null ? _config.GetLocationData (locationId) : null;
		}

		internal static AssetBundleData GetAssetBundleData (string assetBundleId)
		{
			return _config != null ? _config.GetAssetBundleData (assetBundleId) : null;
		}

		public static List<string> GetAllLocationIds ()
		{
			if (_config == null) {
				return new List<string> ();
			}

			var locationIds = new List<string> ();
			foreach (var locationData in _config.LocationDatas) {
				locationIds.Add (locationData.Id);
			}
			return locationIds;
		}

		public static List<string> GetAllAssetBundleIds ()
		{
			if (_config == null) {
				return new List<string> ();
			}

			var assetBundleIds = new List<string> ();
			foreach (var locationData in _config.LocationDatas) {
				foreach (var assetBundleData in locationData.AssetBundleDatas) {
					assetBundleIds.Add (assetBundleData.Id);
				}
			}
			return assetBundleIds;
		}

		private static List<AssetBundleData> GetAssetBundleDependenceDatas (AssetBundleData assetBundleData)
		{
			var dependencyDatas = new List<AssetBundleData> ();

			foreach (var dependency in assetBundleData.Dependencies) {
				var dependencyData = GetAssetBundleData (dependency);
				if (dependencyData == null) {
					ResourceDebug.Log ("ResourceManager->GetAssetBundleDependenceDatas: assetBundle data [{0}]: dependency data [{1}] NOT found.",
							assetBundleData.Id, dependency);
					continue;
				}

				var depthDependencyDatas = GetAssetBundleDependenceDatas (dependencyData);
				if (depthDependencyDatas.Count > 0) {
					dependencyDatas.AddRange (depthDependencyDatas);
				}

				dependencyDatas.Add (dependencyData);
			}

			return dependencyDatas;
		}

		#endregion


		#region Resource

		public static Object Load (string path, System.Type type)
		{
			var data = GetResourceData (path, type);
			if (data == null) {
				return Resources.Load (path, type);
			}

			var assetBundleHolder = GetLoadedAssetBundleHolder (data.InAssetBundle.Id);
			if (assetBundleHolder == null) {
				ResourceDebug.Log ("ResourceManager->Load: can NOT load because the asset bundle [{0}] not loaded.", data.InAssetBundle.Id);
				return null;
			}

			var assetName = Path.GetFileName (path);
			return assetBundleHolder.Entity.LoadAsset (assetName, type);
		}

		public static T Load<T> (string path) where T : Object
		{
			return (T)Load (path, typeof(T));
		}

		public static ResourceLoadRequest LoadAsync (string path, System.Type type)
		{
			var data = GetResourceData (path, type);
			if (data == null) {
				return new ResourceDefaultLoadRequest (path, type);
			}

			var assetBundleHolder = GetLoadedAssetBundleHolder (data.InAssetBundle.Id);
			if (assetBundleHolder == null) {
				ResourceDebug.Log ("ResourceManager->LoadAsync: can NOT load because the asset bundle [{0}] not loaded.", data.InAssetBundle.Id);
				return null;
			}

			var assetName = Path.GetFileName (path);
			return new ResourceInAssetBundleLoadRequest (assetName, type, assetBundleHolder.Entity);
		}

		public static ResourceLoadRequest LoadAsync<T> (string path) where T : Object
		{
			return LoadAsync (path, typeof(T));
		}

		#endregion


		#region Scene

		public static void LoadScene (string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
		{
			var data = GetSceneData (sceneName);
			if (data == null) {
				SceneManager.LoadScene (sceneName, mode);
				return;
			}

			var assetBundleHolder = GetLoadedAssetBundleHolder (data.InAssetBundle.Id);
			if (assetBundleHolder == null) {
				ResourceDebug.Log ("ResourceManager->LoadScene: can NOT load because the asset bundle [{0}] not loaded.", data.InAssetBundle.Id);
				return;
			}

			SceneManager.LoadScene (sceneName, mode);
		}

		public static AsyncOperation LoadSceneAsync (string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
		{
			var data = GetSceneData (sceneName);
			if (data == null) {
				return SceneManager.LoadSceneAsync (sceneName, mode);
			}

			var assetBundleHolder = GetLoadedAssetBundleHolder (data.InAssetBundle.Id);
			if (assetBundleHolder == null) {
				ResourceDebug.Log ("ResourceManager->LoadSceneAsync: can NOT load because the asset bundle [{0}] not loaded.",
						data.InAssetBundle.Id);
				return null;
			}

			return SceneManager.LoadSceneAsync (sceneName, mode);
		}

		#endregion


		#region Location

		private static readonly LocationUnit _locationUnit = new LocationUnit ();

		public static LocationHolder GetLoadedLocationHolder (string locationId)
		{
			return _locationUnit.GetLoadedHolder (locationId);
		}

		public static LocationHolder LoadLocation (string locationId)
		{
			var data = GetLocationData (locationId);
			if (data == null) {
				ResourceDebug.Log ("ResourceManager->LoadLocation: location data [{0}] NOT be configured.", locationId);
				return null;
			}

			return _locationUnit.Load (data);
		}

		public static LocationLoadRequest LoadLocationAsync (string locationId)
		{
			var data = GetLocationData (locationId);
			if (data == null) {
				ResourceDebug.Log ("ResourceManager->LoadLocation: location data [{0}] NOT be configured.", locationId);
				return null;
			}

			return _locationUnit.LoadAsync (data);
		}

		#endregion


		#region Asset Bundle

		private static readonly AssetBundleUnit _assetBundleUnit = new AssetBundleUnit ();

		public static AssetBundleHolder GetLoadedAssetBundleHolder (string assetBundleId)
		{
			return _assetBundleUnit.GetLoadedHolder (assetBundleId);
		}

		public static AssetBundleHolder LoadAssetBundle (string assetBundleId)
		{
			var data = GetAssetBundleData (assetBundleId);
			if (data == null) {
				ResourceDebug.Log ("ResourceManager->LoadAssetBundle: assetBundle data [{0}] NOT be configured.", assetBundleId);
				return null;
			}

			return _assetBundleUnit.Load (data);
		}

		public static AssetBundleLoadRequest LoadAssetBundleAsync (string assetBundleId)
		{
			var data = GetAssetBundleData (assetBundleId);
			if (data == null) {
				ResourceDebug.Log ("ResourceManager->LoadAssetBundleAsync: assetBundle data [{0}] NOT be configured.", assetBundleId);
				return null;
			}

			return _assetBundleUnit.LoadAsync (data);
		}

		public static void UnloadAssetBundle (string assetBundleId)
		{
			var data = GetAssetBundleData (assetBundleId);
			if (data == null) {
				ResourceDebug.Log ("ResourceManager->UnloadAssetBundle: assetBundle data [{0}] NOT exists.", assetBundleId);
				return;
			}

			_assetBundleUnit.Unload (data);
		}

		#endregion


		#region Helper

		public static ResourceAsyncHelper AsyncHelper {
			get { return ResourceAsyncHelper.Instance; }
		}

		#endregion


		#region Editor Only

		private static void RegisterShaderFixAction ()
		{
			if (!Application.isEditor) {
				return;
			}

			SceneManager.sceneLoaded += (scene, sceneMode) => FixShaderOnEditor (scene);
		}

		public static void FixShaderOnEditor (Material material)
		{
			if (material == null) {
				return;
			}

			var materialShader = material.shader;
			var shader = Shader.Find (materialShader.name);
			if (shader != null && shader.GetInstanceID () != materialShader.GetInstanceID ()) {
				material.shader = shader;
			}
		}

		public static void FixShaderOnEditor (Renderer renderer)
		{
			if (renderer == null) {
				return;
			}

			foreach (var material in renderer.sharedMaterials) {
				FixShaderOnEditor (material);
			}
		}

		public static void FixShaderOnEditor (GameObject go)
		{
			if (go == null) {
				return;
			}

			var goRenderers = go.GetComponentsInChildren<Renderer> (true);
			foreach (var goRenderer in goRenderers) {
				FixShaderOnEditor (goRenderer);
			}
		}

		public static void FixShaderOnEditor (Scene scene)
		{
			if (!scene.IsValid ()) {
				return;
			}

			// sky box
			FixShaderOnEditor (RenderSettings.skybox);

			// object renderers
			var goes = scene.GetRootGameObjects ();
			foreach (var go in goes) {
				FixShaderOnEditor (go);
			}
		}

		#endregion
	}
}