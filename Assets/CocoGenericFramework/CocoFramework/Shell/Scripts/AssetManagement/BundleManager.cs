using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.AssetManagement 
{
	public class BundleManager : TaskFactory
	{
		class LoadedBundle
		{
			public AssetBundle bundle;
			public int version = 0;
			public float lastRequest = Time.time;
			public float initialRequest = Time.time;
			public int numRequests = 1;

			public float Age
			{
				get { return Time.time - initialRequest; }
			}

			public float RequestFrequency
			{
				get { return (float)numRequests / Age; }
			}
		}

		public int memoryCachedBundles = 3;

		IDictionary<string, LoadedBundle> _bundles = new Dictionary<string, LoadedBundle>();

		private void CollectGarbage()
		{
			//check if we need to remove a bundle
			if(_bundles.Count <= memoryCachedBundles)
				return;

			//look for the best candidate to be collected as gargabe - right now it's
			//the bundle least frequently used:

			string bundleToRemove = "";
			float minFrequency = float.MaxValue;

			foreach(KeyValuePair<string, LoadedBundle> kvp in _bundles)
			{
				if(kvp.Value.RequestFrequency < minFrequency)
				{
					minFrequency = kvp.Value.RequestFrequency;
					bundleToRemove = kvp.Key;
				}
			}

			if(bundleToRemove != "")
			{
				_bundles[bundleToRemove].bundle.Unload(false);
				_bundles.Remove(bundleToRemove);
			}
		}

		public ServerAddress bundleServerAddress = new ServerAddress() { ip = "localhost", port = 8080};

		public Coroutine GetBundle(string bundleUrl, System.Action<AssetBundle> handler, int version = 0)
		{
			return StartCoroutine(DownloadAndCache(bundleUrl, handler, version));
		}

		public AssetBundle GetBundle(string bundleUrl, int version = 0)
		{
			LoadedBundle loadedBundle;
			if(_bundles.TryGetValue(bundleUrl, out loadedBundle))
			{
				if(loadedBundle.version == version)
				{
					loadedBundle.lastRequest = Time.time;
					loadedBundle.numRequests++;
					return loadedBundle.bundle;
				}
			}

			return null;
		}

		string GetBundleUrl(string bundleName)
		{
			return string.Format("http://{0}/{1}.unity3d", bundleServerAddress, bundleName);
		}

		public bool maintainBundleCache = true;

		public IEnumerator DownloadAndCache(string bundleName, System.Action<AssetBundle> handler, int version)
		{
			LoadedBundle loadedBundle;
			if(_bundles.TryGetValue(bundleName, out loadedBundle))
			{
				if(loadedBundle != null)
				{
					if(loadedBundle.version == version)
					{
						loadedBundle.lastRequest = Time.time;
						loadedBundle.numRequests++;
						handler(loadedBundle.bundle);
						yield break;
					}
				}
			}

			// Wait for the Caching system to be ready
			while (!Caching.ready)
				yield return null;

			if (!maintainBundleCache) {
#if UNITY_2017_1_OR_NEWER
				Caching.ClearCache ();
#else
				Caching.CleanCache ();
#endif
			}

			// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
			string bundleUrl = GetBundleUrl(bundleName);
			CoreLogger.LogInfo("BundleManager", string.Format("for bundle {0}, full url is {1}", bundleName, bundleUrl));
			using(WWW www = WWW.LoadFromCacheOrDownload (bundleUrl, version))
			{
				yield return www;
				if (www.error != null)
				{
					CoreLogger.LogWarning(name, string.Format("unable to retrieve bundle {0}:{1}", bundleUrl, www.error));
					handler(null);
					yield break;
				}

				CoreLogger.LogDebug("BundleManager", string.Format("successfully retrieved bundle {0} from {1}", bundleName, bundleUrl));

				if(loadedBundle != null)
				{
					CoreLogger.LogDebug("BundleManager", string.Format("New version ({0}) of bundle {1} retrieved - unloading old version ({2}) and caching new one", version, bundleName, loadedBundle.version));
					loadedBundle.bundle.Unload(false);
					loadedBundle.bundle = www.assetBundle;
				} else
				{
					CollectGarbage();

					CoreLogger.LogDebug("BundleManager", string.Format("caching version {0} of bundle {1}", version, bundleName));
					_bundles[bundleName] = new LoadedBundle() { bundle = www.assetBundle, version = version};
				}

				handler(www.assetBundle);
			}
		}
	}
}