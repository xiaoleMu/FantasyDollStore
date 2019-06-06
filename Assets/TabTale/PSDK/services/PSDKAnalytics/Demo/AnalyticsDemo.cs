using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

public class AnalyticsDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (PSDKMgr.Instance.Setup()) {
			Debug.Log("AnalyticsDemo - PSDK Setup success!");
			PSDKMgr.Instance.AppIsReady();
		}
	}
	
	public void LogEvent()
	{
		if(PSDKMgr.Instance.GetAnalyticsService() != null){
			Dictionary<string,object> paramDic = new Dictionary<string,object>();
			paramDic.Add("paramKey","paramVal");
			PSDKMgr.Instance.GetAnalyticsService().LogEvent(AnalyticsTargets.ANALYTICS_TARGET_FLURRY | AnalyticsTargets.ANALYTICS_TARGET_TT_ANALYTICS, "testEventName",paramDic,false);
		}
			
	}

	public void LogTimedEvent()
	{
		if(PSDKMgr.Instance.GetAnalyticsService() != null){
			Dictionary<string,object> paramDic = new Dictionary<string,object>();
			paramDic.Add("paramKey","paramVal");
			PSDKMgr.Instance.GetAnalyticsService().LogEvent(AnalyticsTargets.ANALYTICS_TARGET_FLURRY | AnalyticsTargets.ANALYTICS_TARGET_TT_ANALYTICS, "testTimedEventName",paramDic,true);
		}
	}

	public void EndTimedEvent()
	{
		if(PSDKMgr.Instance.GetAnalyticsService() != null){
			Dictionary<string,string> paramDic = new Dictionary<string,string>();
			paramDic.Add("paramKey2","paramVal2");
			PSDKMgr.Instance.GetAnalyticsService().EndLogEvent("testTimedEventName",paramDic);
		}
	}
}
