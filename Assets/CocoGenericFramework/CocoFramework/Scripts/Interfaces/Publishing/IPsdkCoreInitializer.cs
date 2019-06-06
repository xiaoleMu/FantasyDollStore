using UnityEngine;
using System.Collections;
using strange.extensions.promise.api;

namespace TabTale 
{
	/// <summary>
	/// Initializes the PSDK Core
	/// </summary>
	public interface IPsdkCoreInitializer
	{
		IPromise Init();
	}
}