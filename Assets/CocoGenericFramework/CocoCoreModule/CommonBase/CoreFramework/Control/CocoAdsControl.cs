using System;
using UnityEngine;
using System.Collections;
using TabTale;
using TabTale.Publishing;
using strange.extensions.signal.impl;

namespace CocoPlay
{
	public class UpdateRvStatusSignal : Signal
	{
	}


	public class CocoAdsControl : GameView
	{
		#region Interstitial (Legacy)

		[Inject]
		public ILocationManager m_locationManager { get; set; }

		protected override void OnRegister ()
		{
			base.OnRegister ();

			m_LoseFocusTime = DateTime.Now;
		}

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		protected override void AddListeners ()
		{
			base.AddListeners ();
			rewardAdReadySignal.AddListener (RewardAdReady);
			rewardAdResultSignal.AddListener (RewardResultEvent);
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected override void RemoveListeners ()
		{
			rewardAdReadySignal.RemoveListener (RewardAdReady);
			rewardAdResultSignal.RemoveListener (RewardResultEvent);
			base.RemoveListeners ();
		}

		/// <summary>
		/// Gets the insterstitial enable.
		/// </summary>
		/// <returns><c>true</c>, if insterstitial enable was gotten, <c>false</c> otherwise.</returns>
		[Obsolete ("this method be deprecated, please use 'CanShowInterstitial' instead.")]
		public bool GetInsterstitialEnable (ApplicationLocation location)
		{
			return CanShowInterstitial (location);
		}

		/// <summary>
		/// Shows the interstitial.
		/// </summary>
		/// <returns>The interstitial.</returns>
		/// <param name="onfinished">Onfinished.</param>
		[Obsolete ("this be deprecated, please use 'ShowInterstitial' instead.")]
		public IEnumerator showInterstitial (Action<bool, bool> onfinished = null)
		{
			yield return showInterstitial (null, onfinished);
		}

		[Obsolete ("this be deprecated, please use 'PsdkLocation.inScene' instead.")]
		public const string Interstitial_In_Scene = "inScene";

		/// <summary>
		/// Shows the interstitial.
		/// </summary>
		/// <returns>The interstitial.</returns>
		/// <param name="location">Location.</param>
		/// <param name="onfinished">OnFinished Action</param>
		[Obsolete ("this be deprecated, please use 'ShowInterstitial' instead.")]
		public IEnumerator showInterstitial (ApplicationLocation location, Action<bool, bool> onfinished = null)
		{
			var isShown = false;
			var isDone = false;
			Action onShown = () => isShown = true;
			Action onDone = () => isDone = true;

			yield return ShowInterstitial (location, onShown, onDone);

			if (onfinished != null) {
				onfinished (isDone, isShown);
			}
		}

		#endregion


		#region Interstitial

		[Inject]
		public RequestInterstitialSignal RequestInterstitialSignal { get; set; }

		[Inject]
		public InterstitialDoneSignal InterstitialDoneSignal { get; set; }

		[Inject]
		public InterstitialShownSignal InterstitialShownSignal { get; set; }

		public IEnumerator ShowInterstitial (Action onShown = null, Action onDone = null)
		{
			yield return ShowInterstitial (ApplicationLocation.SceneTransitions, onShown, onDone);
		}

		public IEnumerator ShowInterstitial (ApplicationLocation location, Action onShown = null, Action onDone = null)
		{
			// check if can show
			if (!CanShowInterstitial (location)) {
				if (onDone != null) {
					onDone ();
				}
				yield break;
			}

			// use default location if null
			if (location == null) {
				location = ApplicationLocation.SceneTransitions;
			}

			// result status
			var isShown = false;
			var isDone = false;
			Action<LocationResult> shownAction = result => {
				if (result.location != location) {
					return;
				}

				isShown = result.success;
			};
			Action<LocationResult> doneAction = result => {
				if (result.location != location) {
					return;
				}

				isDone = true;
			};

			// request show
			InterstitialDoneSignal.AddOnce (doneAction);
			InterstitialShownSignal.AddOnce (shownAction);
			Debug.LogWarning ("Test : Interstitial Start !!!");
			RequestInterstitialSignal.Dispatch (location);

			// wait for done
			var wait = new WaitForEndOfFrame ();
			while (!isDone) {
				yield return wait;

				if (!isShown) {
					continue;
				}

				if (onShown != null) {
					onShown ();
				}
				isShown = false;
			}

			// end done
			yield return wait;
			if (onDone != null) {
				onDone ();
			}
		}

		public bool CanShowInterstitial (ApplicationLocation location)
		{
			// check debug settings
			if (CocoDebugSettingsData.Instance.IsSkipInterstitialEnabled) {
				return false;
			}

#if !InsterstitialNotNeedCheckGameDuration
			// check ftue and game duration
			if (!CocoRoot.GetInstance<CocoGlobalRecordModel> ().FirstTimeFlowOver && CocoRoot.GetInstance<CocoGlobalRecordModel> ().GameDuration < 3) {
				return false;
			}
#endif
			// check no ads and location ready
//			if (CocoStoreControl.Instance.IsNoAds || !m_locationManager.IsReady (location)) {
//				return false;
//			}

			return true;
		}

		#endregion


		#region RV

		[Inject]
		public UpdateRvStatusSignal updateRvStatusSignal { get; set; }

		[Inject]
		public RewardedAdReadySignal rewardAdReadySignal { get; set; }

		private DateTime m_LoseFocusTime;

		/// <summary>
		/// Raises the application focus event.
		/// </summary>
		/// <param name="focusStatus">If set to <c>true</c> focus status.</param>
		private void OnApplicationFocus (bool focusStatus)
		{
			if (focusStatus) {
#if ABTEST
				var recordModele = CocoRoot.GetInstance<CocoGlobalRecordModel>();
				if(recordModele.CurGPType != GPType.Test_B){
					if((System.DateTime.Now - m_LoseFocusTime).TotalMinutes > 5)
					{
						#if COCO_FAKE
						CocoRoot.GetInstance<CocoGlobalData>().ClearRvRelease();
						#else
						CocoRoot.GetInstance<Game.GameGlobalData>().ClearRvRelease();
						#endif
						updateRvStatusSignal.Dispatch();
					}
				}
#else
				if ((DateTime.Now - m_LoseFocusTime).TotalMinutes > 5) {
#if COCO_FAKE
					CocoRoot.GetInstance<CocoGlobalData>().ClearRvRelease();
#else
					CocoRoot.GetInstance<Game.GameGlobalData> ().ClearRvRelease ();
#endif
					updateRvStatusSignal.Dispatch ();
				}
#endif
			} else {
				m_LoseFocusTime = DateTime.Now;
			}
		}

		/// <summary>
		/// Rewards the ad ready.
		/// </summary>
		/// <param name="pIsReady">If set to <c>true</c> p is ready.</param>
		private void RewardAdReady (bool pIsReady)
		{
			m_IsRVReady = pIsReady;
			m_IsRVStateInited = true;

//			if (m_RequestingStateControl == null) {
//				updateRvStatusSignal.Dispatch ();
//			}
		}

		[Inject]
		public RequestRewardedAdsSignal requestRewardAdsSignal { get; set; }

		[Inject]
		public RewardedAdResultSignal rewardAdResultSignal { get; set; }

		[Inject]
		public IRewardedAds rewardedAds { get; set; }

//		private ICocoLockableStateControl m_RequestingStateControl;
		private bool m_IsRVReady;
		private bool m_IsRVStateInited;

		private int m_RVResultFrameCount = -1;

		public bool RVReady {
			get {
				if (!m_IsRVStateInited) {
					m_IsRVReady = rewardedAds.IsAdReady ();
					m_IsRVStateInited = true;
				}
				return m_IsRVReady;
			}
		}

		public bool IsRVRequesting {
			get { return m_RVResultFrameCount >= Time.frameCount; }
		}

//		public bool RequesetRV (ICocoLockableStateControl stateControl)
//		{
//			if (!RVReady) {
//				return false;
//			}
//			if (m_RequestingStateControl != null) {
//				return false;
//			}

//			m_RequestingStateControl = stateControl;
//			GameElementData adData = new GameElementData (GameElementType.State, stateControl.GetRVKey (), 0);
//			requestRewardAdsSignal.Dispatch (adData);
//			return true;
//		}

		protected void RewardResultEvent (bool result, GameElementData pData)
		{
			m_RVResultFrameCount = Time.frameCount;

//			if (m_RequestingStateControl != null) {
//				if (result && pData.key == m_RequestingStateControl.GetRVKey ()) {
//					m_RequestingStateControl.OnRvReleased ();
//				}
//				m_RequestingStateControl = null;
//			}

			updateRvStatusSignal.Dispatch ();
		}

		#endregion


		#region MoreApps

		[Inject]
		public RequestAppShelfSignal requestAppShelfSignal { get; set; }

		/// <summary>
		/// Determines whether this instance is more apps ready.
		/// </summary>
		/// <returns><c>true</c> if this instance is more apps ready; otherwise, <c>false</c>.</returns>
		public bool IsMoreAppsReady ()
		{
			return m_locationManager.IsReady (ApplicationLocation.MoreApps);
		}

		/// <summary>
		/// Shows the more apps.
		/// </summary>
		public void showMoreApps ()
		{
			requestAppShelfSignal.Dispatch ();
		}

		#endregion
	}
}