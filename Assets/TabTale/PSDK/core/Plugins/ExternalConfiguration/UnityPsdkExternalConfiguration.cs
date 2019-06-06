using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {

	public class UnityPsdkExternalConfiguration : IPsdkExternalConfiguration {

		public bool  Setup() {
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
			return null;
		}
	}

}
