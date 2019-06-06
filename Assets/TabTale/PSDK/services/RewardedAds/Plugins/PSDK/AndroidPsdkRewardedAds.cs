#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;
using System.Runtime.InteropServices;

namespace TabTale.Plugins.PSDK {
	public class AndroidPsdkRewardedAds : IPsdkRewardedAds, IPsdkAndroidService {

		AndroidPsdkServiceMgr _sm;
		AndroidJavaObject _rewardedAdsService;

		public AndroidPsdkRewardedAds(IPsdkServiceManager sm) {
			_sm = sm as AndroidPsdkServiceMgr;
		}

		public bool  Setup() {
			
			if (null == _sm) {
				Debug.LogError("AndroidPsdkServiceManager::SetupRewardedAds NULL PSDK Service Manager ");
				return false;
			}

			AndroidJavaObject rewardedAdsDelegate = 
				new AndroidJavaObject("com.tabtale.publishingsdk.unity.UnityRewardedAdsDelegate");

			_sm.JavaClass.CallStatic("setRewardedAdsDelegate", rewardedAdsDelegate); 

			return true;
		}

		public void psdkStartedEvent() {
			if (_rewardedAdsService == null)
				_rewardedAdsService = GetUnityJavaObject();
			
		}

		public bool ShowAd() {
			
			if (_rewardedAdsService == null)
				_rewardedAdsService = GetUnityJavaObject();
			
			
			if (_rewardedAdsService != null) {
				 return _rewardedAdsService.Call<bool>("showAd");
			}
			else 
				Debug.LogWarning("_rewardedAdsService null !!!");
			
			return false;
		}
		
		
		public bool IsAdReady() {

			if (_rewardedAdsService == null)
				_rewardedAdsService = GetUnityJavaObject();
			
			if (null == _rewardedAdsService)
				return false;
			
			return _rewardedAdsService.Call<bool>("isAdReady");
		}
			
		
		
		public bool IsAdPlaying() {
			
			if (_rewardedAdsService == null)
				_rewardedAdsService = GetUnityJavaObject();
			
			if (null == _rewardedAdsService)
				return false;
			
			return _rewardedAdsService.Call<bool>("isAdPlaying");

		}

		public AndroidJavaObject GetUnityJavaObject() {
			try {
				if (null == _rewardedAdsService)
					_rewardedAdsService = _sm.GetUnityJavaObject().Call<AndroidJavaObject>("getRewardedAdsService");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}

				return _rewardedAdsService;
		}

		public IPsdkRewardedAds GetImplementation() {
			return this;
		}

	}
}
#endif
