using UnityEngine;
using System.Collections;

namespace TabTale.SceneManager
{
	public class AssetPreloader : SceneEntry, IResourceLibrary
	{
		public string[] resourceNames;

		DefaultingDictionary<string, Object> _resources = new DefaultingDictionary<string, Object>();

		#region IResourceLibrary

		public TResource GetResource<TResource>(string name) where TResource : Object
		{
			return GetResource<TResource>(name, false);
		}

		public TResource GetResource<TResource>(string name, bool isCascading) where TResource : Object
		{
			return _resources[name] as TResource;
		}

		#endregion

		IEnumerator Load()
		{
			DefaultingDictionary<string, Object> resources = null;
			GameApplication.Instance.AssetManager.LoadResources(resourceNames, r => { resources = r; });
			while(resources == null)
			{
				yield return null;
			}

			_done = true;
			_resources = resources;
			
			yield break;
		}

		void Start()
		{
			StartCoroutine(Load ());
		}
	}
}
