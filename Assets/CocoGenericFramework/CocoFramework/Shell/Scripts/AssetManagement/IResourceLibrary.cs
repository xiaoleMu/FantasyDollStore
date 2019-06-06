using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface IResourceLibrary
	{
		/// <summary>
		/// Loads a requested resource only if it can be done immediately.
		/// </summary>
		/// <returns>The resource.</returns>
		/// <param name="name">Name of the resource (its path in the asset DB)</param>
		/// <typeparam name="T">The type of resource to be loaded.</typeparam>
		T GetResource<T>(string name) where T : Object;
		T GetResource<T>(string name, bool isCascading) where T : Object;
	}
}
