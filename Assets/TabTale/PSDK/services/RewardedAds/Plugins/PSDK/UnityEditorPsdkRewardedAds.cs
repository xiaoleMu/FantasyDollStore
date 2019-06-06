using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	public class UnityEditorPsdkRewardedAds : IPsdkRewardedAds {

		private bool _isPlaying = false;
		private bool _isBannerWasShown = false;

		public UnityEditorPsdkRewardedAds(IPsdkServiceManager sm) {
		}

		public void psdkStartedEvent() {
			Debug.Log ("UnityEditorPsdkRewardedAds psdkStartedEvent");
		}

		public bool Setup() {
			Dictionary<string,string> keysDict = null;
			Debug.Log ("UnityEditorPsdkRewardedAds::Setup " );
			if (keysDict != null) 
				foreach(KeyValuePair<string,string> kvp in keysDict)
					Debug.Log ("UnityEditorPsdkRewardedAds::Setup   " + kvp.Key + "=" + kvp.Value);
			return true;
		}
		
		
		public bool ShowAd() {
			Debug.Log ("UnityEditorPsdkRewardedAds::ShowAd");
			PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent += adClosed;

			if (PSDKMgr.Instance.GetBannersService () != null)
				_isBannerWasShown = PSDKMgr.Instance.GetBannersService ().IsActive ();

			if (_isBannerWasShown)
				PSDKMgr.Instance.GetBannersService ().Hide ();

			_isPlaying = true;

			ShowRewardedAdsButtons ();
			return true;
		}
		
		
		public bool IsAdReady() {
			Debug.Log ("UnityEditorPsdkRewardedAds::IsAdReady");
			return true;
		}
		
		
		public bool IsAdPlaying() {
			Debug.Log ("UnityEditorPsdkRewardedAds::IsAdPlaying " + _isPlaying);
			return _isPlaying;
		}

		public IPsdkRewardedAds GetImplementation() {
			return this;
		}

		void ShowRewardedAdsButtons() {
			PsdkRewardedAdsButtons.Instance.Show ();
		}

		void adClosed(bool rewarded) {
			PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent -= adClosed;
			Debug.Log ("UnityEditorPsdkRewardedAds::adClosed ! rewarded: " + rewarded);

			if (_isBannerWasShown)
				PSDKMgr.Instance.GetBannersService ().Show ();

			_isPlaying = false;
		}
	}
}
