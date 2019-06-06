using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.Plugins.PSDK {
	/// <summary>
	/// Psdk Splash Interface
	/// </summary>
	public interface IPsdkAppsFlyer  : IPsdkService {
		/// <summary>
		/// Reports the purchase to AppsFlyer.
		/// </summary>
		/// <param name="price">Price.</param>
		/// <param name="currency">Currency.</param>
		void ReportPurchase(string price, string currency); 

		/// <summary>
		/// Gets the iPhone/Android implementation.
		/// </summary>
		/// <returns>The implementation.</returns>
		IPsdkAppsFlyer GetImplementation();
	}
}
