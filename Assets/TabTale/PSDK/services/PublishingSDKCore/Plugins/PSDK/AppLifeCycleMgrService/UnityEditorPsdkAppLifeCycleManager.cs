using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK
{
	public class UnityEditorPsdkAppLifeCycleManager : IPsdkAppLifeCycleManager
	{
		float _pausedTime = Time.realtimeSinceStartup;
		AppLifeCycleResumeState _nextState = AppLifeCycleResumeState.ALCRS_RESTART_APP;
		bool _appIsReady = false;
		bool _splashIsDone = false;
		IPsdkServiceManager _sm;

		bool _callAppIsReadyForTimeOut = true;

		public UnityEditorPsdkAppLifeCycleManager (IPsdkServiceManager sm)
		{
			_sm = sm;
		}
		
		public bool Setup ()
		{
			return true;
		}
		
		public void psdkStartedEvent ()
		{
		}
		
		public AppLifeCycleResumeState OnResume ()
		{
			AppLifeCycleResumeState state = _nextState; 
			if ((Time.realtimeSinceStartup - _pausedTime) >= 360f) {
				state = AppLifeCycleResumeState.ALCRS_NEW_SESSION;
			}
			if ((Time.realtimeSinceStartup - _pausedTime) >= 3000f) {
				state = AppLifeCycleResumeState.ALCRS_RESTART_APP;
			}
			_nextState = AppLifeCycleResumeState.ALCRS_RESUME;
			return state;
		}
		
		public void OnPaused ()
		{
			_pausedTime = Time.realtimeSinceStartup;
		}
		
		public bool OnBackPressed ()
		{ // do nothing, stub, used only for android
			return false;
		}
		
		public void AppIsReady ()
		{
			_appIsReady = true;
			
			sendPsdkReadyMsg ();
		}
		
		private void sendPsdkReadyMsg ()
		{


			if (_appIsReady && (_splashIsDone || (! isSplashServiceEnabled() ))) {
				GameObject psdkSplashListner = GameObject.Find ("PsdkEventSystem");
				if (psdkSplashListner != null) {
					MonoBehaviour mb = (MonoBehaviour) psdkSplashListner.GetComponent("PsdkEventSystem");
					_callAppIsReadyForTimeOut = false;
					psdkSplashListner.SendMessage ("OnPSDKReady", "");
				}
			}
			
		}

		bool isSplashServiceEnabled ()
		{

			IPsdkSplash splash = _sm.GetSplashService ();
			if (splash == null) {
				return false;
			} 


			IDictionary<string,object> psdkConfig = TabTale.Plugins.PSDK.Json.Deserialize (_sm.ConfigJson) as IDictionary<string,object>;
			if (psdkConfig == null) {
				return false;
			}

			if (! psdkConfig.ContainsKey ("splash")) {
				return false;
			}

			IDictionary<string,object> psdkContent = psdkConfig ["splash"] as IDictionary<string,object>;
			if (psdkContent == null) {
				return false;
			}

			object obj;
			if (psdkContent.TryGetValue ("enabled", out obj)) {
				string enabledValue = obj as string;
				if (enabledValue.ToLower () == "yes") {
					return true;
				}
			}
						
			return false;
		}

		public void SetConfigParams (long sessionTime, long restartTime, long psdkReadyTimeout)
		{
			GameObject psdkSplashListner = GameObject.Find ("PsdkEventSystem");
			if (psdkSplashListner != null) {
				MonoBehaviour mb = (MonoBehaviour) psdkSplashListner.GetComponent("PsdkEventSystem");
				_callAppIsReadyForTimeOut = true;
				mb.StartCoroutine(psdkReadyTimeOutCoro(psdkReadyTimeout));
			}
		}
		
		public IPsdkAppLifeCycleManager GetImplementation ()
		{
			return this;
		}

		public void SplashIsDone ()
		{
			_splashIsDone = true;
			sendPsdkReadyMsg ();
		}

		public void OnStart(){
		}
		
		public void OnDestroy(){
		}

		public void OnStop(){
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

		IEnumerator psdkReadyTimeOutCoro(long timeout) {
			yield return new WaitForSeconds(timeout);
			if (_callAppIsReadyForTimeOut)
				AppIsReady ();
		}

	}
}
