

#if UNITY_ANDROID

using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	public class AndroidPsdkAppLifeCycleManager  : IPsdkAppLifeCycleManager, IPsdkAndroidService {

		AndroidPsdkServiceMgr _sm;
		AndroidJavaObject _androidLifeCycleManager;

		public AndroidPsdkAppLifeCycleManager(IPsdkServiceManager sm) {
			_sm = sm as AndroidPsdkServiceMgr;
		}

		public bool Setup() {
			return true;
		}

		public void psdkStartedEvent() {
			if (_androidLifeCycleManager == null)
				_androidLifeCycleManager = GetUnityJavaObject();
		}

		public bool  	SetupAppLifecycleManager() {
			return true;
		}

		public AndroidJavaObject GetUnityJavaObject() {

			if (_sm.GetUnityJavaObject() == null) return null;
			
			try {
				if (_androidLifeCycleManager == null) {
					_androidLifeCycleManager = _sm.GetUnityJavaObject().Call<AndroidJavaObject>("getAppLifeCycleMgr");
				}
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}

			return _androidLifeCycleManager;
		}

		public AppLifeCycleResumeState OnResume()
		{
			if (_androidLifeCycleManager == null)
				_androidLifeCycleManager = GetUnityJavaObject();

			if (_androidLifeCycleManager != null)
			{
                               try {
                                        AndroidJavaObject resumeState = _androidLifeCycleManager.Call<AndroidJavaObject>("onResume");
                                        return (AppLifeCycleResumeState) resumeState.Call<int>("ordinal");
                                }
                                catch (System.Exception e) {
                                        Debug.Log("On Resume exception " + e.InnerException.ToString());
                                        Debug.LogException(e);
                                }
			}
			else {
				Debug.LogError("AndroidPsdkAppLifeCycleManager::OnResume null !");
			}
			
			return AppLifeCycleResumeState.ALCRS_RESUME;
		}
		
		public void OnPaused()
		{
			if (_androidLifeCycleManager == null) {
				_androidLifeCycleManager = GetUnityJavaObject();
			}
			if (_androidLifeCycleManager != null)
				_androidLifeCycleManager.Call("onPaused");
		}

		public void SetConfigParams(long sessionTime, long restartTime, long psdkReadyTimeout) {
			if (_androidLifeCycleManager == null) {
				_androidLifeCycleManager = GetUnityJavaObject();
			}
			if (_androidLifeCycleManager != null)
				_androidLifeCycleManager.Call("setConfigParams",sessionTime,restartTime, psdkReadyTimeout);
		}

		public void OnStart(){
			if (_androidLifeCycleManager == null) {
				_androidLifeCycleManager = GetUnityJavaObject();
			}
			if (_androidLifeCycleManager != null)
				_androidLifeCycleManager.Call("onStart");
		}
		
		public void OnStop(){
			if (_androidLifeCycleManager == null) {
				_androidLifeCycleManager = GetUnityJavaObject();
			}
			if (_androidLifeCycleManager != null)
				_androidLifeCycleManager.Call("onStop");
		}
		
		public void OnDestroy(){
			if (_androidLifeCycleManager == null) {
				_androidLifeCycleManager = GetUnityJavaObject();
			}
			if (_androidLifeCycleManager != null)
				_androidLifeCycleManager.Call("onDestroy");
		}


		public bool OnBackPressed()
		{
			if (_androidLifeCycleManager == null) {
				_androidLifeCycleManager = GetUnityJavaObject();
			}
			if (_androidLifeCycleManager != null)
				return _androidLifeCycleManager.Call<bool>("onBackPressed");

			return false;
		}

		public void AppIsReady() {
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("appIsReady");
			else 
				Debug.LogWarning ("Not calling android appIsReady !");
		}

		public IPsdkAppLifeCycleManager GetImplementation() {
			return this;
		}

		private AppLifeCycleResumeState GetResumeStateFromString(string state) {
			AppLifeCycleResumeState[] values = System.Enum.GetValues(typeof(AppLifeCycleResumeState)) as AppLifeCycleResumeState[] ;
			foreach ( AppLifeCycleResumeState item in values) {
				if (item.ToString().Contains (state.ToString()) || state.ToString().Contains(item.ToString()))  // TODO:fix android enum israelp 
					return item;
			}
			Debug.LogError("Didn't match android AppLifeCycleResumeState value of " + state.ToString());
			return AppLifeCycleResumeState.ALCRS_RESUME;
		}

		/// iOS
		public void DidFinishLaunchingWithOptions(){
			;
		}
		public void ApplicationDidEnterBackground(){
			;
		}
		public void ApplicationWillEnterForeground(){
			;
		}
		public void ApplicationDidFinishLaunching(){
			;
		}
		public void WillFinishLaunchingWithOptions(){
			;
		}
		public void ApplicationDidBecomeActive(){
			;
		}
		public void ApplicationWillResignActive(){
			;
		}
		

	}
}
#endif
