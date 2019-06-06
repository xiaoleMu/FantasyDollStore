using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

public class PublishingSDKCoreDemo : MonoBehaviour {

	void Start () {

		PsdkEventSystem.Instance.onResumeEvent 			+= OnResume;
		PsdkEventSystem.Instance.onConfigurationReady 	+= OnConfigurationReady;
		PsdkEventSystem.Instance.onPsdkReady 			+= OnPsdkReady;

		if (PSDKMgr.Instance.Setup()) {
			Debug.Log("PublishingSDKCoreDemo - PSDK Setup success!");
			PSDKMgr.Instance.AppIsReady();
		}
	}
	
	public void OnResume(AppLifeCycleResumeState resumeState)
	{
		Debug.Log("PublishingSDKCoreDemo - OnResume - resumeState = " + resumeState.ToString());
	}

	public void OnConfigurationReady()
	{
		Debug.Log("PublishingSDKCoreDemo - OnConfigurationReady");
	}

	public void OnPsdkReady()
	{
		Debug.Log("PublishingSDKCoreDemo - OnPsdkReady");
	}
}
