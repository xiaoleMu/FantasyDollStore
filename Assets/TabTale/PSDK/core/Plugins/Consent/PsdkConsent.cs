using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale.Plugins.PSDK {
	
	public class PsdkConsent : IPsdkConsent {

		IPsdkConsent _impl;

		public PsdkConsent(IPsdkServiceManager sm) {
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkConsent (); break;
				#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkConsent (sm.GetImplementation ()); break;
				#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:		_impl = new UnityPsdkConsent (); break;
			default:
				throw new System.Exception ("Platform not supported for PsdkConsent.");
			}
		}

		public ConsentType GetConsent()
		{
			return _impl.GetConsent();
		}
			
		public void SetConsent(ConsentType consentType)
		{
			_impl.SetConsent(consentType);
		}
		public void ForgetUser()
		{
			_impl.ForgetUser();
		}

		public bool ShouldShowPrivacyPolicyOption()
		{
			ConsentType currentConsent = GetConsent();
			if(currentConsent == ConsentType.NPA || currentConsent == ConsentType.PA){
				return true;
			}
			return false;
		}

		public void ShowPrivacySettings() 
		{
			_impl.ShowPrivacySettings();
		}

		public void psdkStartedEvent() {
		}

	}
}
