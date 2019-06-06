using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;


namespace TabTale.Plugins.PSDK {

	public class IphonePsdkServiceMgr : IPsdkServiceManager {

	const string PSDK_IPHONE_STORE = "apple";


	[DllImport ("__Internal")]
	private static extern bool psdkSetup(string configJson, string language);

	[DllImport ("__Internal")]
	private static extern void psdkSetLanguage(string language);

	[DllImport ("__Internal")]
	private static extern void psdkServiceMgr_PurchaseAd();

    [DllImport ("__Internal")]
	private static extern string psdkServiceManager_getAppID();

	[DllImport ("__Internal")]
	private static extern void psdkServiceManager_reportLevel(int level);

	[DllImport ("__Internal")]
	private static extern void psdkValidatePurchase(string price, string currency, string productId);

	[DllImport ("__Internal")]
	private static extern void psdkSetSceneName(string sceneName);

	public event System.Action<AppLifeCycleResumeState> onResumeEvent;



	IPsdkServiceManager _rootPsdkServiceMgr;

	public IphonePsdkServiceMgr(IPsdkServiceManager rootPsdkServiceMgr) {
		_rootPsdkServiceMgr = rootPsdkServiceMgr;
	}

	public bool Setup(string configJson, string language = null) {

		bool rc = psdkSetup(configJson,language);
		return rc;
	}

	public IPsdkAppsFlyer GetAppsFlyerService(){
		return null;
	}

	public IPsdkAppLifeCycleManager  	GetAppLifecycleManager() {
			return _rootPsdkServiceMgr.GetAppLifecycleManager();
	}

	public IPsdkLocationManagerService GetLocationManagerService(){
			return _rootPsdkServiceMgr.GetLocationManagerService();
	}

	public IPsdkGameLevelData 					GetGameLevelDataService(){
			return _rootPsdkServiceMgr.GetGameLevelDataService();
	}

	public IPsdkRewardedAds 					GetRewardedAdsService(){
				return _rootPsdkServiceMgr.GetRewardedAdsService();
	}

		public IPsdkBanners 					GetBannersService(){
			return _rootPsdkServiceMgr.GetBannersService();
		}

		public IPsdkAnalytics 					GetAnalyticsService(){
			return _rootPsdkServiceMgr.GetAnalyticsService();
		}


	public IPsdkServiceManager 		GetImplementation() {
		return this;
	}

	public AppLifeCycleResumeState OnResume() {
		return _rootPsdkServiceMgr.GetAppLifecycleManager().OnResume();
	}


	public void OnPaused() {
			_rootPsdkServiceMgr.GetAppLifecycleManager().OnPaused();
		}

	public string GetStore() {
		return PSDK_IPHONE_STORE;
	}

	public void	SetLanguage(string language) {
		psdkSetLanguage(language);
	}


	public IPsdkSplash 					GetSplashService(){
		// not being used
		return _rootPsdkServiceMgr.GetSplashService();
	}

	public string ConfigJson {
		get { return _rootPsdkServiceMgr.ConfigJson; }
	}

	public void AppIsReady() {
		// Stub, used only in PSDKMgr
	}

	public bool OnBackPressed() {
		// Stub
		return false;
	}

	public void PurchaseAd() {
		psdkServiceMgr_PurchaseAd();
	}


	public bool NativePsdkStarted {
		get {
			return _rootPsdkServiceMgr.NativePsdkStarted;
		}
	}

	public string GetAppID() {
		return psdkServiceManager_getAppID ();
	}

	public void ReportLevel(int level)
	{
		psdkServiceManager_reportLevel (level);
	}


	public void ValidateReceiptAndReport (string receipt, string price, string currency, string productId)
	{
			psdkValidatePurchase (price, currency, productId);
	}

	public void SetSceneName(string sceneName)
	{
			psdkSetSceneName(sceneName);
	}

	#if UNITY_ANDROID
	public AndroidJavaClass JavaClass {
		get {return null;}
	}
	#endif

}
}
