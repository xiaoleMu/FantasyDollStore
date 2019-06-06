using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;
//using Json = TabTale.Plugins.PSDK.PSDKMiniJSON;


namespace TabTale.Plugins.PSDK {

	public class IphonePsdkExternalConfiguration : IPsdkExternalConfiguration {

		[DllImport ("__Internal")]
		private static extern void setupDelegate();

		[DllImport ("__Internal")]
		private static extern string psdkExtConfigurationGetExperimentGroup ();

		public bool  Setup() {
			setupDelegate ();
			return true;
		}

		public string GetGooglePlayLicenceKey()
		{
			return null;
		}

		public IPsdkExternalConfiguration GetImplementation()
		{
			return this;
		}

		public void psdkStartedEvent() {
		}

		public string GetExperimentGroup () {
			return psdkExtConfigurationGetExperimentGroup ();
		}
	}

}
