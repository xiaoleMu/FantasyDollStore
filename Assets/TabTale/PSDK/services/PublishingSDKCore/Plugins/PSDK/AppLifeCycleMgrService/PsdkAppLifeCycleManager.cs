using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	public class PsdkAppLifeCycleManager : IPsdkAppLifeCycleManager {

		IPsdkAppLifeCycleManager _impl;
		IPsdkServiceManager _sm;

		public PsdkAppLifeCycleManager(IPsdkServiceManager sm) {

			_sm = sm;

			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkAppLifeCycleManager(sm.GetImplementation()); break;
			#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkAppLifeCycleManager(sm.GetImplementation()); break;
			#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor: 	_impl = new UnityEditorPsdkAppLifeCycleManager(sm.GetImplementation()); break;
			default: throw new System.Exception("Platform not supported for PsdkAppLifeCycleManager.");
			}

//			#if UNITY_IPHONE && !UNITY_EDITOR
//			_impl = new IphonePsdkAppLifeCycleManager(sm.GetImplementation());
//			#elif UNITY_ANDROID  && !UNITY_EDITOR
//			_impl = new AndroidPsdkAppLifeCycleManager(sm.GetImplementation());
//			#elif UNITY_EDITOR
//			_impl = new UnityEditorPsdkAppLifeCycleManager(sm.GetImplementation());
//			#endif
		}
		
		public bool Setup() {
			return true;
		}

		public void psdkStartedEvent() {
			_impl.psdkStartedEvent();
		}

		public AppLifeCycleResumeState OnResume() {
			if (_sm.NativePsdkStarted) 
				return _impl.OnResume ();
			else 
				return AppLifeCycleResumeState.ALCRS_NONE;
		}


		public void OnPaused() {
			if (_sm.NativePsdkStarted) {
				_impl.OnPaused ();
			}
		}

		public bool OnBackPressed() {
			if (_sm.NativePsdkStarted) {
				return _impl.OnBackPressed ();
			}
			return false;
		}
		
		public void AppIsReady() {
			if (_sm.NativePsdkStarted) {
				_impl.AppIsReady ();
			}
		}
		

		public void SetConfigParams(long sessionTime, long restartTime, long psdkReadyTimeout) {
			if (_sm.NativePsdkStarted) 
				_impl.SetConfigParams (sessionTime, restartTime, psdkReadyTimeout);
		}

		public IPsdkAppLifeCycleManager GetImplementation() {
			return _impl;
		}

		public void OnStart(){
			if (_sm.NativePsdkStarted) 
				_impl.OnStart();
		}

		public void OnDestroy(){
			if (_sm.NativePsdkStarted) 
				_impl.OnDestroy();
		}

		public void OnStop(){
			if (_sm.NativePsdkStarted) 
				_impl.OnStop();
		}
		

		/// iOS
		public void DidFinishLaunchingWithOptions(){
			if (_sm.NativePsdkStarted) 
				_impl.DidFinishLaunchingWithOptions();
		}
		public void ApplicationDidEnterBackground(){
			if (_sm.NativePsdkStarted) 
				_impl.ApplicationDidEnterBackground();
		}
		public void ApplicationWillEnterForeground(){
			if (_sm.NativePsdkStarted) 
				_impl.ApplicationWillEnterForeground();
		}
		public void ApplicationDidFinishLaunching(){
			if (_sm.NativePsdkStarted) 
				_impl.ApplicationDidFinishLaunching();
		}
		public void WillFinishLaunchingWithOptions(){
			if (_sm.NativePsdkStarted) 
				_impl.WillFinishLaunchingWithOptions();
		}
		public void ApplicationDidBecomeActive(){
			if (_sm.NativePsdkStarted) 
				_impl.ApplicationDidBecomeActive();
		}
		public void ApplicationWillResignActive(){
			if (_sm.NativePsdkStarted) 
				_impl.ApplicationWillResignActive();
		}
	}
}
