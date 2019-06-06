using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Data;
using System.Linq;

namespace TabTale.AssetManagement 
{
	public class AssetManager : BundleManager, IAssetManager
	{
		#region Loading Tasks

		class LoadFromBundleTask<T> : CoroutineTask<T>
			where T : Object
		{
			AssetBundleRequest _bundleRequest = null;
			ResourceRequest _request;

			public LoadFromBundleTask(AssetManager assetManager, string path)
			{
				var action = EnumerableAction.Create (() => LoadAsset (assetManager, path), () => _result, int.MaxValue, "");
				Init (assetManager, action);
			}
			
			IEnumerator LoadAsset(AssetManager assetManager, string path)
			{
				if(!assetManager.CatalogReady)
					yield return _coroutineFactory.StartCoroutine(() => assetManager.CatalogWaiter());					

				string bundleName = assetManager.GetBundleName(path);
				if(bundleName != "")
				{
					yield return _coroutineFactory.StartCoroutine(() => WaitForBundle(assetManager, bundleName));
				} else
				{
					CoreLogger.LogWarning(assetManager.LoggerModule, string.Format("unable to find asset bundle for asset {0}", path));
				}

				if(_bundle == null)
				{
					//unable to retrieve bundle
					if(bundleName != "")
						CoreLogger.LogWarning(assetManager.LoggerModule, string.Format("unable to load bundle {0} for asset {1} - will try to load locally", bundleName, path));

					_request = Resources.LoadAsync(path);
					yield return _request;
					_result = _request.asset as T;
					if(_result != null)
						InvokeResultSet();
                    yield break;
				}

				string assetName = assetManager.GetAssetName(path);
				_bundleRequest = _bundle.LoadAssetAsync(assetName, typeof(T));
				CoreLogger.LogInfo(assetManager.LoggerModule, string.Format("requesting load of asset {0} from bundle {1} - waiting...", assetName, bundleName));
				yield return _bundleRequest;

				_result = _bundleRequest.asset as T;
				if(_result == null)
				{
					CoreLogger.LogWarning(assetManager.LoggerModule, string.Format("failed to retrieve asset {0} from bundle {1} - will try to load locally", path, bundleName));
					_request = Resources.LoadAsync(path);
					yield return _request;
					_result = _request.asset as T;
                    yield break;
                }

				InvokeResultSet();

				CoreLogger.LogInfo(assetManager.LoggerModule, string.Format("finished loading asset {0}", path));				                
            }
            
			AssetBundle _bundle;
			IEnumerator WaitForBundle(BundleManager bundleManager, string bundleName)
			{
				yield return bundleManager.GetBundle(bundleName, (b) => { 
					_bundle = b;
				});
			}

			public override float Progress
			{
				get { return _bundleRequest.progress; }
            }
		}

		#endregion

		#region The Asset Catalog

		//get the name of the bundle containing the asset
		public string GetBundleName(string assetPath)
		{
			if(_catalog == null)
				return "";

			return _catalog[assetPath]["bundle"];
		}

		//get the name of the asset inside the bundle
		public string GetAssetName(string assetPath)
		{
			if(_catalog == null)
				return "";

			return _catalog[assetPath]["name"];
		}

		int _loggerModule = 0;
		public int LoggerModule
		{
			get { return _loggerModule; }
		}

		void Start()
		{
			_loggerModule = LoggerModules.AssetManager;
		}

		///This is a dictionary which linkes asset paths to the bundles in which they supposedly
		/// reside. It is created from a json object downloaded from the server. The AssetManager
		/// holds an instance of this class.
		class AssetCatalog
		{
			//the data as it arrived from the server, in json format
			DataElement _data;

			//the reverse lookup of the server data - for each resource name, its location in a bundle - the key is the resource
			//name, each value contains two strings - the bundle name, and the name of the asset as it appears in the bundle
			DataObject _index = new DataObject();

			public AssetCatalog(DataElement data)
			{
				_data = data;

				//assetbundle_set is an array of bundles
				foreach(DataElement bundle in _data["assetbundle_set"])
				{
					string bundleName = bundle["path"];

					//each bundle is as an object, with an array in asset_set
					foreach(DataElement asset in bundle["asset_set"])
					{
						string assetPath = asset["path"];
						_index[assetPath] = new DataObject() { {"name", asset["name"]}, {"bundle", bundleName} };
					}
				}
			}

			public DataElement this[string key]
			{
				get { return _index[key]; }
			}

			public AssetCatalog()
			{
			}
		}

		public bool CatalogReady
		{
			get { return _catalog != null; }
		}

		/// <summary>
		/// A CoRoutine which waits for the catalog object to be available (with a timeout). 
		/// Note that when the object is available it may still be an invalid catalog (due
		/// to an error in retrieval), but the catalog object will not be null and so can
		/// be accessed.
		/// </summary>
		/// <returns>The waiter.</returns>
		/// <param name="timeout">Timeout.</param>
		public IEnumerator CatalogWaiter()
		{
			while(_catalog != null)
			{
				yield return null;
			}
		}

		#endregion

		#region IResourceLibrary implementation

		public T GetResource<T>(string name) where T : Object
		{
			return GetResource<T>(name, false);
		}

		public T GetResource<T> (string path, bool isCascading) where T : Object
		{
			float startTime = Time.realtimeSinceStartup;
			
			//if there is no catalog, or if there is and we cannot find the bundle name, 
			//or if we can and the bundle is not in the cache, 
			//just try and load the plain old Unity way
			
			string bundleName = "";
			if(_catalog != null)
				bundleName = _catalog[path]["bundle"];
			
			AssetBundle bundle = null;
			if(bundleName != null)
				bundle = GetBundle(path);

			T resource;
			if(bundle == null)
			{
				CoreLogger.LogDebug(_loggerModule, string.Format("unable to obtain bundle {0} from cache; loading {1} locally in {2} seconds", 
				                                             bundleName, path, Time.realtimeSinceStartup - startTime));
				try
				{
					resource = Resources.Load<T>(path);
				} catch
				{
					return null;
				}

				return resource;
			}
			
			resource = bundle.LoadAsset(path) as T;
			CoreLogger.LogDebug(_loggerModule, string.Format("resource {0} requested immediately, loaded from bundle {1} in {2} seconds", path, bundleName, Time.realtimeSinceStartup - startTime));
			return resource;
		}

		#endregion

		#region IAssetManager implementation

		public System.Func<IEnumerator> GetBundleCacher(IEnumerable<string> bundleNames)
		{
			IList<System.Func<IEnumerator>> actions = new List<System.Func<IEnumerator>>();
			foreach(string bundle in bundleNames.Where(n => n != ""))
			{
				System.Func<IEnumerator> action = () => DownloadAndCache(bundle, ab => {}, 0);
				actions.Add(action);
			}
			return EnumerableAction.Serialize(actions);
		}

		public System.Func<IEnumerator> GetResourceCacher(IEnumerable<string> resourceNames)
		{
			IList<string> bundlelessResources = new List<string>();
			DefaultingDictionary<string, string> bundleNames = new DefaultingDictionary<string, string>();
			foreach(string assetName in resourceNames)
			{
				string bundleName = GetBundleName(assetName);
				if(bundleName != "")
				{
					bundleNames[bundleName] = bundleName;
				} else
				{
					bundlelessResources.Add(assetName);
				}
			}

			System.Func<IEnumerator> bundleCacher = GetBundleCacher(bundleNames.Keys);

			return bundleCacher;
		}
	
		public void LoadResources(IEnumerable<string> resourceNames, System.Action<DefaultingDictionary<string, Object>> handler)
		{
			DefaultingDictionary<string, Object> resources = new DefaultingDictionary<string, Object>();

			int pending = resourceNames.Count();
			if(pending == 0)
			{
				handler(resources);
				return;
			}

			foreach(string name in resourceNames)
			{
				ITask<Object> loader = LoadResource<Object>(name);
				loader.Start(result => {
					if(result.IsOk())
					{
						resources[name] = loader.Result;
					}
					pending--;
					if(pending == 0)
					{
						handler(resources);
					}
				});
			}
		}

		public ITask<TResource> LoadResource<TResource>(string path) where TResource : Object
		{
			CoreLogger.LogDebug(_loggerModule, string.Format("resource {0} requested - adding a loader task from bundle", path));
			return new LoadFromBundleTask<TResource>(this, path);
		}

		#endregion

		#region RESTful api

		public ServerAddress assetServerAddress = new ServerAddress() { ip = "localhost", port = 8000};
		
		AssetCatalog _catalog;
		
		IEnumerator LoadValue(string url, System.Action<DataElement> handler)
		{
			CoreLogger.LogInfo(_loggerModule, string.Format("url {0} requested", url));
			
			WWW www = new WWW(url);
			yield return www;
			
			if(www.error != null)
			{
				CoreLogger.LogWarning(_loggerModule, string.Format("failed to retrieve url - {0}", www.error));
				handler(null);
				yield break;
			}
			
			CoreLogger.LogTrace(_loggerModule, string.Format("requested url {0} returned {1}", url, www.text));
			
			DataElement dataElement = DataElement.Parse(www.text);
			handler(dataElement);
		}

		#endregion

		#region IService Implementation
		
		public ITask GetInitializer(IServiceResolver resolver)
		{
			return resolver.TaskFactory.FromEnumerableAction(Init);
		}
		
		IEnumerator Init()
		{
			return LoadValue(
				string.Format("http://{0}/api/assetcatalogs", assetServerAddress), 
				data => {
					if(data as DataArray != null)
					{						
						CoreLogger.LogInfo(_loggerModule, string.Format("received catalog: {0}", data));
						_catalog = new AssetCatalog(data[0]);
					} else
					{
						CoreLogger.LogWarning(_loggerModule, "response is not a json array - no catalog will be available");
						_catalog = new AssetCatalog();
					}				
				}
			);
		}
		
		#endregion
	}
}