using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	public class PsdkRewardedAds : IPsdkRewardedAds {

		
		IPsdkRewardedAds _impl;

		public PsdkRewardedAds(IPsdkServiceManager sm) {
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkRewardedAds (sm.GetImplementation ()); break;
			#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkRewardedAds (sm.GetImplementation ()); break;
			#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:		_impl = new UnityEditorPsdkRewardedAds (sm.GetImplementation ()); break;
			default:
				throw new System.Exception ("Platform not supported for AppShelf.");
			}
		}

		public bool Setup() {
			bool rc = _impl.Setup();
			return rc;
		}
		
		
		public bool ShowAd() {
			if (! _impl.IsAdReady () && PSDKMgr.Instance.DebugMode) {
				PsdkRewardedAdsButtons.Instance.Show();
			}
			
			PsdkEventSystem.Instance.PauseGameMusicEventNotification(LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC , true, PsdkEventSystem.MUSIC_PAUSE_CALLER_REWARDED_ADS);
			bool ret = _impl.ShowAd();
			if (!ret){
				PsdkEventSystem.Instance.PauseGameMusicEventNotification(LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC , false, PsdkEventSystem.MUSIC_PAUSE_CALLER_REWARDED_ADS);
			}
			return ret;
		}
		
		
		public bool IsAdReady() {
			bool ready = _impl.IsAdReady();
			if (! ready && PSDKMgr.Instance.DebugMode) {
				return PsdkRewardedAdsButtons.Instance.AdIsReady;
			}
			return ready;
		}
		
		
		public bool IsAdPlaying() {
			bool isPlaying = _impl.IsAdPlaying();
			if (! isPlaying && PSDKMgr.Instance.DebugMode) {
				if (PsdkRewardedAdsButtons.Instance.AdIsPlaying)
					return true;
			}
			return isPlaying;
		}

		public void psdkStartedEvent() {
			_impl.psdkStartedEvent();
		}

		public IPsdkRewardedAds GetImplementation() {
			return _impl;
		}

	}
}
