using System.Collections;
using CocoPlay.ResourceManagement;
using UnityEngine.Networking;

namespace Game
{
	public partial class GameAssetHelperSample
	{
		#region Instance

		private static GameAssetHelperSample _instance;

		public static GameAssetHelperSample Instance {
			get { return _instance ?? (_instance = new GameAssetHelperSample ()); }
		}

		#endregion


		#region Init

		public bool IsInited { get; private set; }

		public IEnumerator Init ()
		{
			if (IsInited) {
				yield break;
			}

#if UNITY_IOS
			ResourceManager.PlatformDirectory = ResourceSettings.PLATFORM_DIRECTORY_IOS;
#else
			ResourceManager.PlatformDirectory = ResourceSettings.PLATFORM_DIRECTORY_GP;
#endif

			var configUri = ResourceStreamingLocation.RuntimeConfigFullFileUri;
			using (var webRequest = UnityWebRequest.Get (configUri)) {
				yield return webRequest.SendWebRequest ();
				if (string.IsNullOrEmpty (webRequest.error)) {
					var configJson = webRequest.downloadHandler.text;
					IsInited = ResourceManager.Init (configJson);
				} else {
					ResourceDebug.Log ("{0}->Init: config load failed, uri [{1}], error [{2}]", GetType ().Name, configUri, webRequest.error);
				}
			}

			ResourceDebug.Log ("{0}->Init: config init finished, uri [{1}], success [{2}]", GetType ().Name, configUri, IsInited);
		}

		#endregion


		#region Load

		public GroupLoadRequest<string> LoadAllLocationsAsync ()
		{
			var locationIds = ResourceManager.GetAllLocationIds ();
			return new GroupLoadRequest<string> (locationIds, ResourceManager.LoadLocationAsync);
		}

		public GroupLoadRequest<string> LoadAllAssetBundlesAsync ()
		{
			var assetBundleIds = ResourceManager.GetAllAssetBundleIds ();
			return new GroupLoadRequest<string> (assetBundleIds, ResourceManager.LoadAssetBundleAsync);
		}

		#endregion
	}
}