#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
    public class PsdkAppLifeCycleMgrStub : IPsdkAppLifeCycleManager
    {
        public void AppIsReady()
        {
            
        }

        public void ApplicationDidBecomeActive()
        {
            
        }

        public void ApplicationDidEnterBackground()
        {
            
        }

        public void ApplicationDidFinishLaunching()
        {
            
        }

        public void ApplicationWillEnterForeground()
        {
            
        }

        public void ApplicationWillResignActive()
        {
            
        }

        public void DidFinishLaunchingWithOptions()
        {
            
        }

        public IPsdkAppLifeCycleManager GetImplementation()
        {
            return this;
        }

        public bool OnBackPressed()
        {
            return false;
        }

        public void OnDestroy()
        {
            
        }

        public void OnPaused()
        {
            
        }

        public AppLifeCycleResumeState OnResume()
        {
            PsdkEventSystem.Instance.StartCoroutine(CallOnPsdkReadyAfterDelay());
            return AppLifeCycleResumeState.ALCRS_RESUME;
        }

        IEnumerator CallOnPsdkReadyAfterDelay()
        {
            yield return new WaitForSeconds(0.5f);
            PsdkEventSystem.Instance.OnPSDKReady();
        }

        public void OnStart()
        {
            
        }

        public void OnStop()
        {
            
        }

        public void psdkStartedEvent()
        {
            
        }

        public void SetConfigParams(long sessionTime, long restartTime, long psdkReadyTimeout = 10)
        {
            
        }

        public bool Setup()
        {
            return true;
        }

        public void WillFinishLaunchingWithOptions()
        {
            
        }
    }
}
#endif