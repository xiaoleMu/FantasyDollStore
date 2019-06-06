using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.AssetManagement 
{
	public interface IAssetManager : IService, IResourceLibrary
	{
		/// <summary>
		/// Loads a requested resource asynchronously, from the local database (if available), 
		/// or from a bundle. 
		/// </summary>
		/// <returns>A task object which loads the resource - when Done is fired, the resource is here.</returns>
		/// <param name="name">Name of the resource (its path in the asset DB)</param>
		/// <typeparam name="T">Type of the resource to be loaded</typeparam>
		ITask<T> LoadResource<T>(string name) where T : Object;

		void LoadResources(IEnumerable<string> resourceNames, System.Action<DefaultingDictionary<string, Object>> handler);

		System.Func<IEnumerator> GetBundleCacher(IEnumerable<string> bundleNames);
		System.Func<IEnumerator> GetResourceCacher(IEnumerable<string> resourceNames);
	}
}
