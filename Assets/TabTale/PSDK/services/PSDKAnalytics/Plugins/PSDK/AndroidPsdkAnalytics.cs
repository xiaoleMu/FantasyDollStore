#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
	public class AndroidPsdkAnalytics : IPsdkAnalytics , IPsdkAndroidService
	{
		
		
		private AndroidPsdkServiceMgr _psdkServiceMgr;
		private AndroidJavaObject _javaObject;
		
		public AndroidPsdkAnalytics(IPsdkServiceManager serviceMgr) {
			_psdkServiceMgr = serviceMgr as AndroidPsdkServiceMgr;

			AndroidJavaObject javaAnalyticsDelegate = new AndroidJavaObject("com.tabtale.publishingsdk.unity.UnityAnalyticsDelegate");
			if (javaAnalyticsDelegate == null) {
				Debug.LogError("com.tabtale.publishingsdk.unity.UnityAnalyticsDelegate NULL");
			}
			else {
				_psdkServiceMgr.JavaClass.CallStatic("setAnalyticsDelegate", javaAnalyticsDelegate); 
			}

		}
		
		
		public IPsdkAnalytics GetImplementation() {
			return this;
		}
		
		public AndroidJavaObject GetUnityJavaObject() {
			try {
				if (null == _javaObject)
					_javaObject = _psdkServiceMgr.GetUnityJavaObject().Call<AndroidJavaObject>("getAnalytics");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}
			
			return _javaObject;
		}
		
		public void psdkStartedEvent() {
			GetUnityJavaObject();
		}
		
		
		public void LogEvent(long targets, string eventName, IDictionary<string, object> eventParams, bool timed) {
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("logEvent",targets,eventName,PsdkUtils.CreateJavaJSONObjectFromDictionary(eventParams), timed);
			else {
				Debug.LogWarning ("Not calling android psdk analytics LogComplexEvent !, cause object is null");
				Debug.Log ("Event was not sent: " + eventName + " -> " + Json.Serialize(eventParams));
			}
		}

		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed, bool psdkEvent)
		{
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("logEvent",targets,eventName,PsdkUtils.CreateJavaJSONObjectFromDictionary(eventParams), timed, psdkEvent);
			else {
				Debug.LogWarning ("Not calling android psdk analytics LogComplexEvent !, cause object is null");
				Debug.Log ("Event was not sent: " + eventName + " -> " + Json.Serialize(eventParams));
			}
		}
		
		public void EndLogEvent(string eventName, IDictionary<string, object> eventParams) {
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("endLogEvent",eventName,PsdkUtils.CreateJavaJSONObjectFromDictionary(eventParams));
			else {
				Debug.LogWarning ("Not calling android psdk analytics LogComplexEvent !, cause object is null");
				Debug.Log ("EndLogEvent public void ReportPurchase(string price, string currency, string productId) {was not sent: " + eventName + " -> " + Json.Serialize(eventParams));
			}
		}
		
		
		public void LogEvent(string eventName, IDictionary<string,string> eventParams, bool timed){
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("logEvent",eventName,PsdkUtils.CreateJavaHashMapFromDictionary(eventParams),timed);
			else {
				Debug.LogWarning ("Not calling android psdk analytics LogEvent !, cause object is null");
				Debug.Log ("Event was not sent: " + eventName + " -> " + PsdkUtils.BuildJsonStringFromDict(eventParams) + ", timed:" + timed);
			}
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,string> eventParams){
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("endLogEvent",eventName,PsdkUtils.CreateJavaHashMapFromDictionary(eventParams));
			else {
				Debug.LogWarning ("Not calling android psdk analytics LogEvent !, cause object is null");
				Debug.Log ("End timed Event was not sent: " + eventName + " -> " + PsdkUtils.BuildJsonStringFromDict(eventParams));
			}
		}
		
		public void LogComplexEvent(string eventName, IDictionary<string, object> eventParams) {
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("logComplexEvent",eventName,PsdkUtils.CreateJavaJSONObjectFromDictionary(eventParams));
			else {
				Debug.LogWarning ("Not calling android psdk analytics LogComplexEvent !, cause object is null");
				Debug.Log ("Complex Event was not sent: " + eventName + " -> " + Json.Serialize(eventParams));
			}
		}

		public void ReportPurchase(string price, string currency, string productId) {
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("reportPurchase",price,currency,productId);
			else {
				Debug.LogWarning ("Not calling android psdk analytics reportPurchase !, cause object is null");
			}
		}


		public bool RequestEngagement(string decisionPoint, Dictionary<string,object> parameters)
		{
			bool retVal = false;
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				retVal = sjo.Call<bool>("requestEngagement",decisionPoint,PsdkUtils.CreateJavaJSONObjectFromDictionary(parameters));
			else {
				Debug.LogWarning ("Not calling android psdk analytics RequestEngagement !, cause object is null");
			}
			return retVal;
		}

		public void LogTransaction(string transactionName, IDictionary<string, object> productsReceived, IDictionary<string, object> productsSpent, IDictionary<string, object> otherEventParams)
        {
        }

        public IDictionary<string, object> generateProductsReceived(IDictionary<string, object>[] items, IDictionary<string, object> realCurrency, IDictionary<string, object>[] virtualCurrencies)
        {
        	return null;
        }

        public IDictionary<string, object> generateProductsSpent(IDictionary<string, object>[] items, IDictionary<string, object> realCurrency, IDictionary<string, object>[] virtualCurrencies)
        {
        	return null;
        }

        public IDictionary<string, object> generateItem(int itemAmount, string itemName, string itemType)
        {
        	return null;
        }

        public IDictionary<string, object> generateVirtualCurrency(int virtualCurrencyAmount, string virtualCurrencyName, string virtualCurrencyType)
        {
        	return null;
        }

        public IDictionary<string, object> generateRealCurrency(string realCurrencyAmount, string realCurrencyType)
        {
        	return null;
        }

		public void TutorialStep(bool isMandatory, int tutorialStepID, string tutorialName, string tutorialStepName, Dictionary<string,object> additionalParams)
		{
		}

		public void ReachedMainScreen(Dictionary<string,object> additionalParams)
		{
		}

		public void LevelUp (string skinName, string levelUpName, int level, Dictionary<string,object> additionalParams)
		{
			IDictionary<string, object> extras = new Dictionary<string, object>();
			extras ["userLevel"] = level;

			IDictionary<string, object> eventParams = new Dictionary<string, object>();
			eventParams ["levelUpName"] = levelUpName;
			eventParams ["skinName"] = skinName;

			if (additionalParams != null) {
				foreach (var item in additionalParams) {
					eventParams.Add (item.Key,item.Value);
				}
			}
			
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("addExtras", new object[] { PsdkUtils.CreateJavaJSONObjectFromDictionary(extras) });
			else {
				Debug.LogWarning ("Not calling android psdk analytics LevelUp ! cause object is null");
			}

			LogEvent (AnalyticsTargets.ANALYTICS_TARGET_DELTA_DNA, "levelUp", eventParams, false, true);
		
		}

		public void MissionStarted (int id, string name, string type, Dictionary<string,object> additionalParams)
		{
			IDictionary<string, object> extras = new Dictionary<string, object>();
			extras ["missionID"] = id;
			extras ["missionName"] = name;
			extras ["missionType"] = type;

			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("addExtras", new object[] { PsdkUtils.CreateJavaJSONObjectFromDictionary(extras) });
			else {
				Debug.LogWarning ("Not calling android psdk analytics MissionStarted ! cause object is null");
			}

			if (additionalParams != null) {
				foreach (var item in additionalParams) {
					extras.Add (item.Key,item.Value);
				}
			}

			LogEvent (AnalyticsTargets.ANALYTICS_TARGET_DELTA_DNA, "missionStarted", extras, false, true);
		}
		public void MissionComplete (Dictionary<string,object> additionalParams)
		{
			LogEvent (AnalyticsTargets.ANALYTICS_TARGET_DELTA_DNA, "missionCompleted", additionalParams, false, true);

			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("removeExtras", new object[] { "missionID;missionName;missionType" });
			else {
				Debug.LogWarning ("Not calling android psdk analytics MissionCompleted ! cause object is null");
			}
		}
	}
}
#endif
