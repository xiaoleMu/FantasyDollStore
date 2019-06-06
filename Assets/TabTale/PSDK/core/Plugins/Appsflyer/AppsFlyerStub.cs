using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale.Plugins.PSDK {
	public class AppsFlyerStub : IPsdkAppsFlyer {

		public void ReportPurchase(string price, string currency)
		{
			
		}

		public IPsdkAppsFlyer GetImplementation()
		{
			return this;
		}

		public void psdkStartedEvent()
		{
			
		}
	}
}