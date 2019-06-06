using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TabTale.Plugins.PSDK {
	
	public class Psdk : MonoBehaviour, IPsdk {

		#region interface
		public static IPsdk Instance { get; set; }

		public event Action OnMuteSound;
		public event Action OnUnmuteSound;

		public event Action OnSessionStartShown;
		public event Action OnSessionStartHidden;

		public event Action<bool /*isAvailable*/> OnRewardedVideoAvailabilityChanged;

		public event Action<TabTale.InAppPurchasableItem /*item*/, bool /*isRestore*/> OnPurchaseOrRestoreSucceeded;
		public event Action<TabTale.InAppPurchasableItem /*item*/> OnPurchaseFailed;

		public event Action OnGameServicesLoginSucceeded;
		public event Action OnGameServicesLoginFailed;
		public event Action OnGameServicesLogout;

		public event Action OnNewSession;
		public event Action OnRestartGame;


		public bool IsRewardedVideoAvailable()
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetRewardedAdsService() == null){
				Debug.LogError("Psdk: IsRewardedVideoAvailable called although rewarded ads service does not exist. Will return false");
				return false;
			}
			return PSDKMgr.Instance.GetRewardedAdsService().IsAdReady();
		}

		public void ShowRewardedVideo(Action<bool> resultDelegate)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetRewardedAdsService() == null){
				Debug.LogError("Psdk: ShowRewardedVideo called although rewarded ads service does not exist. Will return false");
				return;
			}

			var rewardedAdsService = PSDKMgr.Instance.GetRewardedAdsService();

			if (rewardedAdsService.IsAdReady())
			{
				mRewardedVideoResultDelegate = resultDelegate;
				rewardedAdsService.ShowAd();
			}
			else
			{
				resultDelegate(false);
			}
		}

		public bool ShowInterstitial(InterstitialLocation interstitialLocation, Action completionDelegate)
		{
			return ShowLocation(InterstitialLocationToString(interstitialLocation), completionDelegate);

		}

		public bool ShowAppShelf()
		{
			return ShowLocation(PsdkLocation.moreApps);
		}

		public void ReportPurchase(string price, string currencyCode, string productId)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetAnalyticsService() == null){
				Debug.LogError("Psdk: ReportPurchase called although analyitcs service does not exist. Will not send event.");
				return;
			}
			PSDKMgr.Instance.GetAnalyticsService().ReportPurchase(price, currencyCode, productId);
		}

		public void DisableAdsPermanently()
		{
			PSDKMgr.Instance.PurchaseAd();
		}

		public void LogEvent(string eventName, IDictionary<string,object> eventParameters, long analyticsAgents)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetAnalyticsService() == null){
				Debug.LogError("Psdk: LogEvent called although analyitcs service does not exist. Will not send event.");
				return;
			}
			PSDKMgr.Instance.GetAnalyticsService().LogEvent(analyticsAgents, eventName, eventParameters, false);
		}

		public void LogTimedEvent(string eventName, IDictionary<string,object> eventParameters)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetAnalyticsService() == null){
				Debug.LogError("Psdk: LogTimedEvent called although analyitcs service does not exist. Will not send event.");
				return;
			}
			PSDKMgr.Instance.GetAnalyticsService().LogEvent(AnalyticsTargets.ANALYTICS_TARGET_FLURRY | AnalyticsTargets.ANALYTICS_TARGET_DELTA_DNA, eventName, eventParameters, true);
		}

		public void EndTimedEvent(string eventName, IDictionary<string,object> eventParameters)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetAnalyticsService() == null){
				Debug.LogError("Psdk: EndTimedEvent called although analyitcs service does not exist. Will not send event.");
				return;
			}
			PSDKMgr.Instance.GetAnalyticsService().EndLogEvent(eventName, eventParameters);
		}

		public void PurchaseItem(string itemId)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetBilling() == null){
				Debug.LogError("Psdk: PurchaseItem called although analyitcs service does not exist. Item will not be purchased.");
				return;
			}
			PSDKMgr.Instance.GetBilling().PurchaseItem(itemId);
		}

		public void RestorePurchases(string itemId)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetBilling() == null){
				Debug.LogError("Psdk: RestorePurchases called although analyitcs service does not exist. Items will not be restored.");
				return;
			}
			PSDKMgr.Instance.GetBilling().RestorePurchases();
		}

		public void ShowRateUs()
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetRateUs() == null){
				Debug.LogError("Psdk: ShowRateUs called although analyitcs service does not exist. Will not show Rate Us.");
				return;
			}
			PSDKMgr.Instance.GetRateUs().Show();
		}

		public bool IsLoggedInGameServices()
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetSocial() == null){
				Debug.LogError("Psdk: IsLoggedInGameServices called although analyitcs service does not exist. Will return false.");
				return false;
			}
			return PSDKMgr.Instance.GetSocial().IsAuthenticated();
		}

		public void LoginGameServices()
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetSocial() == null){
				Debug.LogError("Psdk: LoginGameServices called although analyitcs service does not exist. Will not log in.");
				return;
			}
			PSDKMgr.Instance.GetSocial().Authenticate();
		}

		public void LogoutGameServices()
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetSocial() == null){
				Debug.LogError("Psdk: LogoutGameServices called although analyitcs service does not exist. Will not log out.");
				return;
			}

			PSDKMgr.Instance.GetSocial().SignOut();

			if (OnGameServicesLogout != null)
				OnGameServicesLogout();
		}

		public void ShowLeaderboard()
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetSocial() == null){
				Debug.LogError("Psdk: ShowLeaderboard called although analyitcs service does not exist. Will not show.");
				return;
			}
			PSDKMgr.Instance.GetSocial().ShowLeaderboard();
		}

		public void ReportScore(string leaderboardId, long score)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetSocial() == null){
				Debug.LogError("Psdk: ReportScore called although analyitcs service does not exist. Will not report score.");
				return;
			}
			PSDKMgr.Instance.GetSocial().SetPlayerScore(leaderboardId, score);
		}

		public void ShareScreenshot()
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetSocial() == null){
				Debug.LogError("Psdk: ShareScreenshot called although analyitcs service does not exist. Will not share.");
				return;
			}
			PSDKMgr.Instance.GetShare().ShareScreenshot();
		}
			
		public void ShareImage(string pathToImage)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetSocial() == null){
				Debug.LogError("Psdk: ShareImage called although analyitcs service does not exist. Will not share.");
				return;
			}
			PSDKMgr.Instance.GetShare().ShareImage(pathToImage);
		}

		public void ShareVideo(string pathToVideo)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetSocial() == null){
				Debug.LogError("Psdk: ShareScreenshot called although analyitcs service does not exist. Will not share.");
				return;
			}
			PSDKMgr.Instance.GetShare().ShareVideo(pathToVideo);
		}

		public void ShareAppLink()
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetSocial() == null){
				Debug.LogError("Psdk: ShareAppLink called although analyitcs service does not exist. Will not share.");
				return;
			}
			PSDKMgr.Instance.GetShare().ShareAppLink();
		}

		public void ShowBanners()
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetBannersService() == null){
				Debug.LogError("Psdk: ShowBanners called although analyitcs service does not exist. Will not show.");
				return;
			}
			PSDKMgr.Instance.GetBannersService().Show();
		}

		public void HideBanners()
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetBannersService() == null){
				Debug.LogError("Psdk: HideBanners called although analyitcs service does not exist. Will not hide.");
				return;
			}
			PSDKMgr.Instance.GetBannersService().Hide();
		}

		public IDictionary<string,object> GetGLDData(string name)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetGameLevelDataService() == null){
				Debug.LogError("Psdk: GetGLDData called although analyitcs service does not exist. Will get data.");
				return null;
			}
			return PSDKMgr.Instance.GetGameLevelDataService().GetData(name);
		}

		#endregion

		#region Life Cycle
		private void Awake()
		{
			if (Instance != null)
			{
				DestroyImmediate(gameObject);
				return;
			}

			#if UNITY_IOS && IOS_LOGGER
			IosLogger.RegisterLogger();
			#endif

			DontDestroyOnLoad(gameObject);
			Instance = this;

			if (!PSDKMgr.Instance.Setup())
			{
				Debug.LogError("PSDK failed to initialize.");
				return;
			}
			SubscribeToPsdkEvents();
			PSDKMgr.Instance.GetBilling().InitBilling();
			PSDKMgr.Instance.GetSocial().Init();
			PSDKMgr.Instance.AppIsReady();
		}

		private void OnDestroy()
		{
			if ((Instance as Psdk) != this)
				return;

			UnsubscribeFromPsdkEvents();
		}

		private void SubscribeToPsdkEvents()
		{
			PsdkEventSystem.Instance.onPsdkReady += OnPsdkReady;
			PsdkEventSystem.Instance.onResumeEvent += OnResumeEvent;
			PsdkEventSystem.Instance.pauseGameMusicEvent += OnPauseGameMusicEvent;
			PsdkEventSystem.Instance.onLocationLoadedEvent += OnLocationLoadedEvent;
			PsdkEventSystem.Instance.onLocationFailedEvent += OnLocationFailedEvent;
			PsdkEventSystem.Instance.onClosedEvent += OnClosedEvent;
			PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent += OnRewardedAdDidClosedWithResultEvent;
			PsdkEventSystem.Instance.onRewardedAdIsReadyEvent += OnRewardedAdIsReadyEvent;
			PsdkEventSystem.Instance.onRewardedAdIsNotReadyEvent += OnRewardedAdIsNotReadyEvent;
			PsdkEventSystem.Instance.onBillingPurchased += OnBillingPurchased;
			PsdkEventSystem.Instance.onSocialAuthenticate += OnSocialLoggedIn;
		}

		private void UnsubscribeFromPsdkEvents()
		{
			if (PsdkEventSystem.Instance == null)
				return;

			PsdkEventSystem.Instance.onPsdkReady -= OnPsdkReady;
			PsdkEventSystem.Instance.onResumeEvent -= OnResumeEvent;
			PsdkEventSystem.Instance.pauseGameMusicEvent -= OnPauseGameMusicEvent;
			PsdkEventSystem.Instance.onLocationLoadedEvent -= OnLocationLoadedEvent;
			PsdkEventSystem.Instance.onLocationFailedEvent -= OnLocationFailedEvent;
			PsdkEventSystem.Instance.onClosedEvent -= OnClosedEvent;
			PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent -= OnRewardedAdDidClosedWithResultEvent;
			PsdkEventSystem.Instance.onRewardedAdIsReadyEvent -= OnRewardedAdIsReadyEvent;
			PsdkEventSystem.Instance.onRewardedAdIsNotReadyEvent -= OnRewardedAdIsNotReadyEvent;
			PsdkEventSystem.Instance.onBillingPurchased -= OnBillingPurchased;
			PsdkEventSystem.Instance.onSocialAuthenticate -= OnSocialLoggedIn;
		}

		#endregion

		#region Event Handlers
		private void OnPsdkReady()
		{
			ShowSessionStart();
		}

		private void OnResumeEvent(AppLifeCycleResumeState resumeState)
		{
			PSDKMgr.Instance.AppIsReady();

			Action eventToFire = null;
			if (resumeState == AppLifeCycleResumeState.ALCRS_NEW_SESSION)
				eventToFire = OnNewSession;
			else if (resumeState == AppLifeCycleResumeState.ALCRS_RESTART_APP)
				eventToFire = OnRestartGame;

			if (eventToFire != null)
				eventToFire();
		}

		private void OnPauseGameMusicEvent(bool isMute)
		{
			if (isMute)
			{
				if (OnMuteSound != null)
					OnMuteSound();
			}
			else
			{
				if (OnUnmuteSound != null)
					OnUnmuteSound();
			}
		}

		private void OnLocationLoadedEvent(string location, long attributes)
		{
			if (location == PsdkLocation.sessionStart)
				ShowSessionStart();
		}

		void OnLocationFailedEvent(string location, string psdkError)
		{
		}

		void OnClosedEvent(string location, long attributes)
		{
			if (location == PsdkLocation.sessionStart)
				FireOnSessionStartHiddenEvent();

			Action completionDelegate = null;
			if (mLocationManagerCompletionDelegates.TryGetValue(location, out completionDelegate))
			{
				mLocationManagerCompletionDelegates.Remove(location);
				if (completionDelegate != null)
					completionDelegate();
			}

		}

		private void OnRewardedAdDidClosedWithResultEvent(bool isSuccess)
		{
			if (mRewardedVideoResultDelegate != null)
				mRewardedVideoResultDelegate(isSuccess);

			mRewardedVideoResultDelegate = null;
		}

		private void OnBillingPurchased(PurchaseIAPResult purchaseIAPResult)
		{
			if (purchaseIAPResult.result == PurchaseIAPResultCode.Success && purchaseIAPResult.error == BillerErrors.NO_ERROR)
			{
				if (PSDKMgr.Instance.GetBilling().IsNoAdsItem(purchaseIAPResult.purchasedItem.id))
					DisableAdsPermanently();

				if (OnPurchaseOrRestoreSucceeded != null)
					OnPurchaseOrRestoreSucceeded(purchaseIAPResult.purchasedItem, PSDKMgr.Instance.GetBilling().IsRestoreInProgress());
			}
			else if (purchaseIAPResult.result == PurchaseIAPResultCode.Failed)
			{
				if (OnPurchaseFailed != null)
					OnPurchaseFailed(purchaseIAPResult.purchasedItem);
			}
		}

		private void OnSocialLoggedIn(bool isSuccess)
		{
			if (isSuccess)
			{
				if (OnGameServicesLoginSucceeded != null)
					OnGameServicesLoginSucceeded();
			}
			else
			{
				if (OnGameServicesLoginFailed != null)
					OnGameServicesLoginFailed();
			}
		}

		#endregion

		#region Implementation


		public void ShowSessionStart()
		{
			ShowLocation(PsdkLocation.sessionStart);
		}

		private bool ShowLocation(string location, Action completionDelegate = null)
		{
			if(PSDKMgr.Instance == null || PSDKMgr.Instance.GetLocationManagerService() == null){
				Debug.LogError("Psdk: ShowLocation called although location mgr service does not exist. Will return false");
				return false;
			}

			var locationManager = PSDKMgr.Instance.GetLocationManagerService();
			locationManager.ReportLocation(location);
			if (locationManager.IsLocationReady(location))
			{
				if (location == PsdkLocation.sessionStart)
					FireOnSessionStartShownEvent();

				mLocationManagerCompletionDelegates.Add(location, completionDelegate);
				if (locationManager.Show(location) != LocationMgrAttributes.LOCATION_MGR_ATTR_NO_SOURCE){
					return true;
				} 
				else {
					Debug.LogError("Psdk: ShowLocation for " + location + " failed.");
					if (completionDelegate != null)
						completionDelegate();
					return false;
				}
			}
			else
			{
				if (completionDelegate != null)
					completionDelegate();
				return false;
			}
		}

		private void OnRewardedAdIsReadyEvent()
		{
			if (OnRewardedVideoAvailabilityChanged != null)
				OnRewardedVideoAvailabilityChanged(true);
		}

		private void OnRewardedAdIsNotReadyEvent()
		{
			if (OnRewardedVideoAvailabilityChanged != null)
				OnRewardedVideoAvailabilityChanged(false);
		}

		private void FireOnSessionStartShownEvent()
		{
			if (OnSessionStartShown != null)
				OnSessionStartShown();
		}

		private void FireOnSessionStartHiddenEvent()
		{
			if (OnSessionStartHidden != null)
				OnSessionStartHidden();
		}

		public static string InterstitialLocationToString(InterstitialLocation location)
		{
			string result = null;
			switch (location){
			case InterstitialLocation.SESSION_START:
				result = PsdkLocation.sessionStart;
				break;
			case InterstitialLocation.ACHIEVEMENT_WON:
				result = PsdkLocation.achievementWon;
				break;
			case InterstitialLocation.BACK_TO_MAIN_MENU:
				result = PsdkLocation.backToMainMenu;
				break;
			case InterstitialLocation.FAIL:
				result = PsdkLocation.fail;
				break;
			case InterstitialLocation.GENERIC_LOCATION:
				result = PsdkLocation.genericLocation;
				break;
			case InterstitialLocation.IN_SCENE:
				result = PsdkLocation.inScene;
				break;
			case InterstitialLocation.POST_LEVEL:
				result = PsdkLocation.postLevel;
				break;
			case InterstitialLocation.PRE_LEVEL:
				result = PsdkLocation.preLevel;
				break;
			case InterstitialLocation.REPLAY_LEVEL:
				result = PsdkLocation.replayLevel;
				break;
			case InterstitialLocation.RETRY:
				result = PsdkLocation.retry;
				break;
			case InterstitialLocation.SCENE_TRANSITIONS:
				result = PsdkLocation.sceneTransitions;
				break;
			default:
				break;
			}
			return result;
		}

		private Action<bool> mRewardedVideoResultDelegate;
		private Dictionary<string, Action> mLocationManagerCompletionDelegates = new Dictionary<string, Action>();

		#endregion

	}
}