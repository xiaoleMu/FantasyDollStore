using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	public class IphonePsdkRewardedAds : IPsdkRewardedAds {

		[DllImport ("__Internal")]
		private static extern bool psdkSetupRewardedAds();

		[DllImport ("__Internal")]
		private static extern bool psdkRewardedAds_ShowAd();
		
		[DllImport ("__Internal")]
		private static extern bool psdkRewardedAds_IsAdReady();
		
		[DllImport ("__Internal")]
		private static extern bool psdkRewardedAds_IsAdPlaying();
		

		public IphonePsdkRewardedAds(IPsdkServiceManager sm) {
		}



		public bool Setup() {
			return psdkSetupRewardedAds();
		}


		public bool ShowAd() {
			return psdkRewardedAds_ShowAd();
		}


		public bool IsAdReady() {
			return psdkRewardedAds_IsAdReady();
		}


		public bool IsAdPlaying() {
			return psdkRewardedAds_IsAdPlaying();
		}


		public void psdkStartedEvent() {
		}

		public IPsdkRewardedAds GetImplementation() {
			return this;
		}
	}
}
