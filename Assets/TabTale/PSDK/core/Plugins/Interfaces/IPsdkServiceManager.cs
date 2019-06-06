using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {

	public interface IPsdkService {
		void psdkStartedEvent();
	}

	public interface IPsdkAndroidService {
		#if UNITY_ANDROID
		AndroidJavaObject GetUnityJavaObject();
		#endif
	}

	public interface IPsdkServiceManager  {

		event System.Action<AppLifeCycleResumeState> onResumeEvent;

		bool 	Setup(string configJson = null, string language = null);

		IPsdkAppLifeCycleManager  	GetAppLifecycleManager();
		IPsdkLocationManagerService GetLocationManagerService();
		IPsdkGameLevelData 			GetGameLevelDataService();
		IPsdkRewardedAds 			GetRewardedAdsService();
		IPsdkSplash					GetSplashService();
		IPsdkBanners 				GetBannersService();
		IPsdkAnalytics 				GetAnalyticsService();
		IPsdkAppsFlyer				GetAppsFlyerService();

		IPsdkServiceManager 		GetImplementation();

		string GetStore();
		string GetAppID();
		string ConfigJson { get; }
		void SetLanguage(string language);
		void ReportLevel (int level);
		void ValidateReceiptAndReport (string receipt, string price, string currency, string productId);

		// AppIsReady should be called when application finished the loading phase;
		// and then the PsdkStartupListnerService::onSplashDone event will be called;
		void AppIsReady();
		bool OnBackPressed();
		void PurchaseAd();
		void SetSceneName(string sceneName);



		// internal
		bool NativePsdkStarted {get;}

		#if UNITY_ANDROID
		AndroidJavaClass JavaClass { get; }
		#endif

	}
}
