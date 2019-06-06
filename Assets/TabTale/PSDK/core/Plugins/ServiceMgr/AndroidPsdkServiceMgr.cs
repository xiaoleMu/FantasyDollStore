#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {

	public class AndroidPsdkServiceMgr : IPsdkServiceManager, IPsdkAndroidService {

		public event System.Action<AppLifeCycleResumeState> onResumeEvent;

		private AndroidJavaObject _serviceManagerJavaObject;
		private AndroidJavaClass  _serviceManagerJavaClass;

		IPsdkServiceManager _rootPsdkServiceMgr;

		public AndroidPsdkServiceMgr(IPsdkServiceManager rootPsdkServiceMgr) {
			_rootPsdkServiceMgr = rootPsdkServiceMgr;
		}

		public bool Setup(string configJson, string language = null)
		{
			JavaClass.CallStatic ("setUnitySyncDelegate", new UnitySyncDelegate ());


			AndroidJavaObject startupDelegate =
				new AndroidJavaObject("com.tabtale.publishingsdk.unity.UnityStartupDelegate");

			if ( startupDelegate == null) {
				Debug.LogError("AndroidPsdkServiceManager::Setup NULL Startup delegate ");
				return false;
			}

			_serviceManagerJavaObject = JavaClass.CallStatic<AndroidJavaObject>("unityStart", configJson, PsdkUtils.CurrentActivity, language, startupDelegate);

			if (null != _serviceManagerJavaObject)
				return true;

			Debug.LogError("AndroidPsdkServiceManager::Setup NULL PSDK Service Manager ");
			return false;
		}

		public IPsdkAppsFlyer GetAppsFlyerService(){
			return null;
		}

//		public IPsdkAppShelf 				GetAppShelf() {
//			return _rootPsdkServiceMgr.GetAppShelf();
//		}

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


		public AndroidJavaClass JavaClass {
			get {
				if (null == _serviceManagerJavaClass)
					_serviceManagerJavaClass = new AndroidJavaClass("com.tabtale.publishingsdk.core.ServiceManager");
				return _serviceManagerJavaClass;
			}
		}

		public AndroidJavaObject GetUnityJavaObject() {
			return _serviceManagerJavaObject;
		}


		public string ConfigJson {
			get { return _rootPsdkServiceMgr.ConfigJson; }
		}

		public AppLifeCycleResumeState OnResume()
		{
			return _rootPsdkServiceMgr.GetAppLifecycleManager().OnResume();
		}

		public void OnPaused()
		{
			_rootPsdkServiceMgr.GetAppLifecycleManager().OnPaused();
		}

		public string GetStore() {
			#if AMAZON
				return "amazon";
			#else
				return "google";
			#endif
		}

		public void	SetLanguage(string language) {
			_serviceManagerJavaObject.Call("setLanguage",language);
		}



		public IPsdkSplash 					GetSplashService(){
			// not being used
			return _rootPsdkServiceMgr.GetSplashService();
		}
	
		public void AppIsReady() {
			// Stub, used only in PSDKMgr
		}
		public bool OnBackPressed() {
			// Stub
			return false;
		}
		public void PurchaseAd() {
			_serviceManagerJavaObject.Call("purchaseAd");
		}

		public string GetAppID() {
			return null;
		}

		public void ReportLevel(int level) {
			_serviceManagerJavaObject.Call ("reportLevel", level);
		}

		public void ValidateReceiptAndReport (string receipt, string price, string currency, string productId)
		{
			using(AndroidJavaObject purchaseValidationService = _serviceManagerJavaObject.Call<AndroidJavaObject> ("getPurchaseValidationService")){
				AndroidJavaObject purchaseValidationDelegate = new AndroidJavaObject ("com.tabtale.publishingsdk.unity.UnityPurchaseValidationDelegate");
				purchaseValidationService.Call ("validateReceiptAndReport", purchaseValidationDelegate, receipt, price, currency, productId);
			}
		}

		public bool NativePsdkStarted {
			get {
				return _rootPsdkServiceMgr.NativePsdkStarted;
			}
		}

		public void SetSceneName(string sceneName)
		{
			_serviceManagerJavaObject.Call("setSceneName",sceneName);
		}

	}
}
#endif
