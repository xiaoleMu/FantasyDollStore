using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale.Plugins.PSDK {

	public interface IPsdkConsent : IPsdkService 
	{
		ConsentType GetConsent();
		void SetConsent(ConsentType consentType);
		void ForgetUser();
		bool ShouldShowPrivacyPolicyOption();
		void ShowPrivacySettings();
	}

	public enum ConsentType
	{
		PA,NPA,UA,NE,UNKNOWN
	}

	public enum ConsentFormType
	{
		NONE, ANY, NO_PURCHASE
	}
}
