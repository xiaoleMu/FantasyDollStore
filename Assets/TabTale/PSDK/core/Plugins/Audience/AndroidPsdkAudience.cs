#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {

	public class AndroidPsdkAudience : IPsdkAudience, IPsdkAndroidService {

		AndroidPsdkServiceMgr _sm;
		AndroidJavaObject _audienceObject;

		public AndroidPsdkAudience(IPsdkServiceManager sm) {
			_sm = sm as AndroidPsdkServiceMgr;
		}

		public bool  Setup() {
			if (null == _sm) {
				Debug.LogError("AndroidPsdkServiceManager::SetupRewardedAds NULL PSDK Service Manager ");
				return false;
			}

			return true;
		}

		public AndroidJavaObject GetUnityJavaObject()
		{
			try {
				if (null == _audienceObject)
					_audienceObject = _sm.GetUnityJavaObject().Call<AndroidJavaObject>("getAudience");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}

			return _audienceObject;
		}

		public void psdkStartedEvent() {
			if (_audienceObject == null)
				_audienceObject = GetUnityJavaObject();

		}

		public IPsdkAudience GetImplementation()
		{
			return this;
		}

		public void SetBirthYear (int birthYear)
		{
			if(GetUnityJavaObject() != null){
				GetUnityJavaObject().Call("setBirthYear",new object[] {birthYear});
			}
		}
		public int GetAge ()
		{
			if(GetUnityJavaObject() != null){
				return GetUnityJavaObject().Call<int>("getAge");
			}
			return -1;
		}
		public PSDKAudienceMode GetAudienceMode ()
		{
			PSDKAudienceMode retVal = PSDKAudienceMode.NON_CHILDREN;

			if (GetUnityJavaObject () != null) {
				AndroidJavaObject resultObj = GetUnityJavaObject().Call<AndroidJavaObject>("getAudienceMode");
				if(resultObj != null){
					string resultString = resultObj.Call<string> ("toString");
					if(resultString != null){
						switch(resultString){
							case "children":
								retVal = PSDKAudienceMode.CHILDREN;
								break;
							case "non-children":
								retVal = PSDKAudienceMode.NON_CHILDREN;
								break;
							case "mixed":
								retVal = PSDKAudienceMode.MIXED_UNKNOWN;
								break;
							case "mixed-children":
								retVal = PSDKAudienceMode.MIXED_CHILDREN;
								break;
							case "mixed-non-children":
								retVal = PSDKAudienceMode.MIXED_NON_CHILDREN;
								break;
							default:
								break;
						}
					}
				}
			}
			return retVal;
		}
	}
}
#endif
