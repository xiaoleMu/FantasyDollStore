using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TabTale.Plugins.PSDK {

	public class UnityEditorPsdkServiceMgr : IPsdkServiceManager {

		public event System.Action<AppLifeCycleResumeState> onResumeEvent;

		IPsdkServiceManager _rootPsdkServiceMgr;

		public UnityEditorPsdkServiceMgr(IPsdkServiceManager rootPsdkServiceMgr) {
			_rootPsdkServiceMgr = rootPsdkServiceMgr;
		}

		public bool Setup(string configJson, string language = null) {
			Debug.Log("UnityEditor::initializePsdkService stub");
			PSDKMgr.Instance.AppIsReady ();
			return true;
		}


		
		public IPsdkAppLifeCycleManager  	GetAppLifecycleManager() {
			return _rootPsdkServiceMgr.GetAppLifecycleManager() ;
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

		public IPsdkAppsFlyer GetAppsFlyerService(){
			return null;
		}

		public void AppIsReady() {
			// Stub, used only in PSDKMgr
	}
		
		public string ConfigJson {
			get { return _rootPsdkServiceMgr.ConfigJson; }
		}

		public AppLifeCycleResumeState OnResume()
		{
			Debug.Log("UnityEditor::OnResume stub");
			return AppLifeCycleResumeState.ALCRS_RESUME;
		}
		
		public void OnPaused()
		{
			Debug.Log("UnityEditor::OnPaused stub");
		}

		public string GetStore() {
			#if UNITY_IOS
				return "apple";
			#elif AMAZON
				return "amazon";
			#else
				return "google";
			#endif
		}


		public IPsdkSplash 					GetSplashService(){
			// not being used
			return _rootPsdkServiceMgr.GetSplashService();
		}
			
		public bool OnBackPressed() {
			// Stub
			return false;
		}


		public void	SetLanguage(string language) {
		}

		public void PurchaseAd() {
		}

		public void ReportLevel(int level){
			
		}
		
		public bool NativePsdkStarted {
			get {
				return _rootPsdkServiceMgr.NativePsdkStarted;
			}
		}

		
		public string GetAppID() {
			return null;
		}

		public void ValidateReceiptAndReport (string receipt, string price, string currency, string productId)
		{
			UnityEngine.Debug.Log ("validate receipt and report was called");
		}

		#if UNITY_ANDROID
		public AndroidJavaClass JavaClass {
			get {return null;}
		}
		#endif

		public void SetSceneName(string sceneName)
		{
			
		}	

	}
}
