using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.Plugins.PSDK {
	/// <summary>
	/// Psdk Splash Interface
	/// </summary>
	public interface IPsdkSplash  : IPsdkService {
		/// <summary>
		/// Setups the splash.
		/// </summary>
		/// <returns><c>true</c>, if splash was setuped, <c>false</c> otherwise.</returns>
		bool SetupSplash();

		/// <summary>
		/// Gets the iPhone/Android implementation.
		/// </summary>
		/// <returns>The implementation.</returns>
		IPsdkSplash GetImplementation();
	}
}
