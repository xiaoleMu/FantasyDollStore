using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale.Plugins.PSDK {

	public class UnityPsdkConsent : IPsdkConsent {

		public void SetConsent(ConsentType consentType)
		{
		}
		public void ForgetUser()
		{
		}
		public ConsentType GetConsent()
		{
			return ConsentType.PA;
		}

		public void psdkStartedEvent() {
		}
		public bool ShouldShowPrivacyPolicyOption()
		{
			return false;
		}

		public void ShowPrivacySettings() 
		{
		}
	}
}