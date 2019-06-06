using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace TabTale.Plugins.PSDK {

	public class IphonePsdkConsent : IPsdkConsent {

		[DllImport ("__Internal")]
		private static extern string psdkGetConsent();

		[DllImport ("__Internal")]
		private static extern void psdkSetConsent(string consentType);

		[DllImport ("__Internal")]
		private static extern void psdkForgetUser();

		[DllImport ("__Internal")]
		private static extern void psdkShowPrivacySettings();

		public ConsentType GetConsent()
		{
			string resultStr = psdkGetConsent();
			ConsentType result = ConsentType.UNKNOWN;
			if(resultStr != null)
				result = PsdkUtils.StringToConsentType(resultStr);
			return result;
		}

		public void SetConsent(ConsentType consentType)
		{
			psdkSetConsent(consentType.ToString());
		}
		public void ForgetUser()
		{
			psdkForgetUser();
		}

		public void psdkStartedEvent() {
		}

		public bool ShouldShowPrivacyPolicyOption()
		{
			return false;
		}

		public void ShowPrivacySettings() 
		{
			psdkShowPrivacySettings();
		}
	
	}
}