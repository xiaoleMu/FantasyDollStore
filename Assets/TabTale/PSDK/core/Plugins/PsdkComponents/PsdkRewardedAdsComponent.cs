using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace TabTale.Plugins.PSDK {
	
	public class PsdkRewardedAdsComponent : MonoBehaviour {

		public UnityEvent OnLoaded;
		public UnityEvent OnFailed;
		public UnityEvent OnShown;
		public UnityEvent<bool> OnClosed;

		// Use this for initialization
		void Start () {
			PsdkEventSystem.Instance.onRewardedAdIsReadyEvent += OnAdLoaded;
			PsdkEventSystem.Instance.onRewardedAdIsNotReadyEvent += OnAdFailed;
			PsdkEventSystem.Instance.onRewardedAdWillShowEvent += OnAdWillShow;
			PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent += OnAdClosed;

		}

		void OnDestroy()
		{
			PsdkEventSystem.Instance.onRewardedAdIsReadyEvent -= OnAdLoaded;
			PsdkEventSystem.Instance.onRewardedAdIsNotReadyEvent -= OnAdFailed;
			PsdkEventSystem.Instance.onRewardedAdWillShowEvent -= OnAdWillShow;
			PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent -= OnAdClosed;
		}

		public void Show()
		{
			if(IsReady())
				PSDKMgr.Instance.GetRewardedAdsService().ShowAd();
		}

		public bool IsReady()
		{
			if(PSDKMgr.Instance != null && PSDKMgr.Instance.GetRewardedAdsService() != null)
				return PSDKMgr.Instance.GetRewardedAdsService().IsAdReady();
			return false;
		}

		public bool IsPlaying()
		{
			if(PSDKMgr.Instance != null && PSDKMgr.Instance.GetRewardedAdsService() != null)
				PSDKMgr.Instance.GetRewardedAdsService().IsAdPlaying();
			return false;
		}

		private void OnAdLoaded()
		{
			if(OnLoaded != null)
				OnLoaded.Invoke();
		}

		private void OnAdFailed()
		{
			if(OnFailed != null)
				OnFailed.Invoke();
		}

		private void OnAdWillShow()
		{
			if(OnShown != null)
				OnShown.Invoke();
		}

		private void OnAdClosed(bool shouldReward)
		{
			if(OnClosed != null)
				OnClosed.Invoke(shouldReward);
		}
	}
}