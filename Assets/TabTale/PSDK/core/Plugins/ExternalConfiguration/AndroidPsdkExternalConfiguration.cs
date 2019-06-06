#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {

	public class AndroidPsdkExternalConfiguration : IPsdkExternalConfiguration, IPsdkAndroidService {

		AndroidPsdkServiceMgr _sm;
		AndroidJavaObject _externalConfigurationService;

		public AndroidPsdkExternalConfiguration(IPsdkServiceManager sm) {
			_sm = sm as AndroidPsdkServiceMgr;
		}

		public bool  Setup() {

			if (null == _sm) {
				Debug.LogError("AndroidPsdkServiceManager::SetupRewardedAds NULL PSDK Service Manager ");
				return false;
			}

			AndroidJavaObject configurationDelegate = new AndroidJavaObject ("com.tabtale.publishingsdk.unity.UnityConfigurationDelegate");
			if(configurationDelegate == null){
				Debug.LogError ("AndroidPsdkExternalConfiguration:: Setup NULL configuration delegate ");
				return false;
			}

			_sm.JavaClass.CallStatic("setConfigurationDelegate", configurationDelegate);

			return true;
		}

		public AndroidJavaObject GetUnityJavaObject()
		{
			try {
				if (null == _externalConfigurationService)
					_externalConfigurationService = _sm.GetUnityJavaObject().Call<AndroidJavaObject>("getExtConfiguration");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}

			return _externalConfigurationService;
		}

		public string GetGooglePlayLicenceKey()
		{
			if(GetUnityJavaObject() != null){
				return GetUnityJavaObject ().Call<string> ("getGooglePlayLicenceKey");
			}
			else {
				return null;
			}
		}

		public IPsdkExternalConfiguration GetImplementation()
		{
			return this;
		}

		public void psdkStartedEvent() {
			if (_externalConfigurationService == null)
				_externalConfigurationService = GetUnityJavaObject();

		}

		public string GetExperimentGroup () {
			if(GetUnityJavaObject() != null){
				return GetUnityJavaObject ().Call<string> ("getExperimentGroup");
			}
			else {
				return null;
			}
		}
	}
}
#endif
