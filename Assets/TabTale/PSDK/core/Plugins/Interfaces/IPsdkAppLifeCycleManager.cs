using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {
	public enum AppLifeCycleResumeState
	{
		ALCRS_NONE,
		ALCRS_RESUME,
		ALCRS_NEW_SESSION,
		ALCRS_RESTART_APP
	}

	public interface IPsdkAppLifeCycleManager : IPsdkService  {
		bool Setup();
		AppLifeCycleResumeState OnResume();
		void OnPaused();
		void SetConfigParams(long sessionTime, long restartTime, long psdkReadyTimeout = 10);
		IPsdkAppLifeCycleManager GetImplementation();
		bool OnBackPressed();
		void AppIsReady();
		void OnStop();
		void OnStart();
		void OnDestroy();

		/// iOS
		void DidFinishLaunchingWithOptions();
		void ApplicationDidEnterBackground();
		void ApplicationWillEnterForeground();
		void ApplicationDidFinishLaunching();
		void WillFinishLaunchingWithOptions();
		void ApplicationDidBecomeActive();
		void ApplicationWillResignActive();

	}
}
