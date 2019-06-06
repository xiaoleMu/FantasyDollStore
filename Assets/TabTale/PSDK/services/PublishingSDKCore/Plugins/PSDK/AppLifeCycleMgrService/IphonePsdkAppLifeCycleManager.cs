using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;



namespace TabTale.Plugins.PSDK {
	public class IphonePsdkAppLifeCycleManager : IPsdkAppLifeCycleManager  {

		
		[DllImport ("__Internal")]
		private static extern int psdkAlmOnResume();
		
		[DllImport ("__Internal")]
		private static extern void psdkAlmOnPaused();

		[DllImport ("__Internal")]
		private static extern void psdkAlmOnStart();
		
		[DllImport ("__Internal")]
		private static extern void psdkAlmOnStop();
		
		[DllImport ("__Internal")]
		private static extern void psdkAlmOnDestroy();
		
		[DllImport ("__Internal")]
		private static extern void psdkSetConfigParams(long sessionTime, long restartTime, long psdkReadyTimeout);

		[DllImport("__Internal")]
		private static extern void psdkAlmAppIsReady();
		
		[DllImport("__Internal")]
		private static extern void psdkAlmApplicationWillResignActive();

		/// iOS
		[DllImport("__Internal")]
		private static extern void psdkAlmDidFinishLaunchingWithOptions();
		[DllImport("__Internal")]
		private static extern void psdkAlmApplicationDidEnterBackground();
		[DllImport("__Internal")]
		private static extern void psdkAlmApplicationWillEnterForeground();
		[DllImport("__Internal")]
		private static extern void psdkAlmApplicationDidFinishLaunching();
		[DllImport("__Internal")]
		private static extern void psdkAlmWillFinishLaunchingWithOptions();
		[DllImport("__Internal")]
		private static extern void psdkAlmApplicationDidBecomeActive();



		public IphonePsdkAppLifeCycleManager(IPsdkServiceManager sm) {
		}

		public bool Setup() {
			return true;
		}

		public void psdkStartedEvent() {
		}

		public AppLifeCycleResumeState OnResume() {
			AppLifeCycleResumeState rs = (AppLifeCycleResumeState) psdkAlmOnResume();
			return rs;
		}
		
		public void OnPaused() {
			psdkAlmOnPaused();
		}

		public void OnStart(){
			psdkAlmOnStart();
		}
		

		public void OnStop(){
			psdkAlmOnStop();
		}
		
		public void OnDestroy(){
			psdkAlmOnDestroy ();
		}

		public void ApplicationWillResignActive() {
			psdkAlmApplicationWillResignActive ();
		}

		public bool OnBackPressed()
		{ // do nothing, stub, used only for android
			return false;
		}

		public void AppIsReady() {
			psdkAlmAppIsReady();
		}

		public void SetConfigParams(long sessionTime, long restartTime, long psdkReadyTimeout) {
			psdkSetConfigParams(sessionTime, restartTime, psdkReadyTimeout);
		}



		public IPsdkAppLifeCycleManager GetImplementation() {
			return this;
		}

		/// iOS
		public void DidFinishLaunchingWithOptions(){
			psdkAlmDidFinishLaunchingWithOptions();
		}
		public void ApplicationDidEnterBackground(){
			psdkAlmApplicationDidEnterBackground();
		}
		public void ApplicationWillEnterForeground(){
			psdkAlmApplicationWillEnterForeground();
		}
		public void ApplicationDidFinishLaunching(){
			psdkAlmApplicationDidFinishLaunching();
		}
		public void WillFinishLaunchingWithOptions(){
			psdkAlmWillFinishLaunchingWithOptions();
		}
		public void ApplicationDidBecomeActive(){
			psdkAlmApplicationDidBecomeActive();
		}



	}
}

