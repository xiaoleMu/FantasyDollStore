#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale.Plugins.PSDK {

	public class AndroidPsdkConsent : IPsdkConsent {

		AndroidPsdkServiceMgr _sm;
		AndroidJavaObject _consentMgrObject;

		public AndroidPsdkConsent(IPsdkServiceManager sm) {
			_sm = sm as AndroidPsdkServiceMgr;
		}

		public AndroidJavaObject GetUnityJavaObject()
		{
			try {
				if (null == _consentMgrObject)
					_consentMgrObject = _sm.GetUnityJavaObject().Call<AndroidJavaObject>("getConsentInstructor");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}

			return _consentMgrObject;
		}

		public ConsentType GetConsent()
		{
			_consentMgrObject = GetUnityJavaObject();
			ConsentType result = ConsentType.UNKNOWN;
			string consentStr = null;
			if(_consentMgrObject != null)
				consentStr = _consentMgrObject.Call<string>("getConsentStr");
			if(consentStr != null){
				result = PsdkUtils.StringToConsentType(consentStr);
			}
			return result;
				
		}

		public void SetConsent(ConsentType consentType)
		{
			_consentMgrObject = GetUnityJavaObject();
			if(_consentMgrObject != null)
				_consentMgrObject.Call("setConsent",consentType.ToString());
		}

		public void ForgetUser()
		{
			_consentMgrObject = GetUnityJavaObject();
			if(_consentMgrObject != null){
				_consentMgrObject.Call("forgetUser");
			}
		}

		public void ShowPrivacySettings() 
		{
			_consentMgrObject = GetUnityJavaObject();
			if(_consentMgrObject != null){
				_consentMgrObject.Call("showPrivacySettings");
			}
		}

		public void psdkStartedEvent() {
		}
		
		public bool ShouldShowPrivacyPolicyOption()
		{
			return false;
		}
	}
}
#endif
