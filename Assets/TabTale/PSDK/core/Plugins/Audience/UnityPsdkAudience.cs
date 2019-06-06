using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {

	public class UnityPsdkAudience : IPsdkAudience {

		public bool  Setup() {
			return true;
		}

		public IPsdkAudience GetImplementation()
		{
			return this;
		}
		public void SetBirthYear (int birthYear)
		{
		}
		public int GetAge ()
		{
			return 0;
		}
		public PSDKAudienceMode GetAudienceMode ()
		{
			return PSDKAudienceMode.CHILDREN;
		}
		public void psdkStartedEvent() {

		}
	}

}
