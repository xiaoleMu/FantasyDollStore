using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
	internal class IphonePsdkAnalytics : IPsdkAnalytics
	{

		[DllImport("__Internal")]
		private static extern void psdkSetupAnalytics();

		[DllImport("__Internal")]
		private static extern void psdkAnalyticsLogEvent(long targets, string eventName, string eventParamsJson, bool timed);

		[DllImport("__Internal")]
		private static extern void psdkAnalyticsLogEventInternal(long targets, string eventName, string eventParamsJson, bool timed, bool psdkEvent);
		
		[DllImport("__Internal")]
		private static extern void psdkAnalyticsEndLogEvent(string eventName, string eventParamsJson);
		
		[DllImport("__Internal")]
		private static extern void psdkAnalyticsLogComplexEvent(string eventName, string eventParamsJson);

		[DllImport("__Internal")]
		private static extern void psdkAnalyticsReportPurchase (string price, string currency, string productId);

		[DllImport("__Internal")]
		private static extern bool psdkRequestEngagement(string decisionPoint, string parameters);

		[DllImport("__Internal")]
		private static extern void psdkAddExtras (string extrasStrJson);

		[DllImport("__Internal")]
		private static extern void psdkRemoveExtras (string keysStr);
		
		public IphonePsdkAnalytics(IPsdkServiceManager sm) {
			psdkSetupAnalytics();
		}
		
		public IPsdkAnalytics GetImplementation() {
			return this;
		}
		
		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed){
			psdkAnalyticsLogEvent(targets, eventName, PsdkUtils.BuildJsonStringFromDict(eventParams),timed);
		}

		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed, bool psdkEvent)
		{
			psdkAnalyticsLogEventInternal(targets,eventName,PsdkUtils.BuildJsonStringFromDict(eventParams),timed,psdkEvent);
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,object> eventParams){
			psdkAnalyticsEndLogEvent(eventName, eventParams != null ? Json.Serialize (eventParams) : null);
		}
		
		public void LogEvent(string eventName, IDictionary<string,string> eventParams, bool timed){
			psdkAnalyticsLogEvent(AnalyticsTargets.ANALYTICS_PSDK_INTERNAL_TARGETS, eventName, eventParams != null ? Json.Serialize (eventParams) : null,timed);
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,string> eventParams){
			psdkAnalyticsEndLogEvent(eventName, eventParams != null ? Json.Serialize (eventParams) : null);
		}
		
		public void LogComplexEvent(string eventName, IDictionary<string, object> eventParams) {
			psdkAnalyticsLogComplexEvent(eventName, eventParams != null ? Json.Serialize (eventParams) : null);
		}

		public void ReportPurchase(string price, string currency, string productId) {
			psdkAnalyticsReportPurchase (price, currency, productId);
		}

		public void psdkStartedEvent() {
		}

		public bool RequestEngagement(string decisionPoint, Dictionary<string,object> parameters)
		{
			return psdkRequestEngagement(decisionPoint, parameters != null ? Json.Serialize (parameters) : null);
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

			psdkAddExtras (Json.Serialize (extras));

			if (additionalParams != null) {
				foreach (var item in additionalParams) {
					eventParams.Add (item.Key,item.Value);
				}
			}

			LogEvent (AnalyticsTargets.ANALYTICS_TARGET_DELTA_DNA, "levelUp", eventParams, false, true);
		}

		public void MissionStarted (int id, string name, string type, Dictionary<string,object> additionalParams)
		{
			IDictionary<string, object> extras = new Dictionary<string, object>();
			extras ["missionID"] = id;
			extras ["missionName"] = name;
			extras ["missionType"] = type;

			psdkAddExtras (Json.Serialize (extras));

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
			psdkRemoveExtras ("missionID;missionName;missionType");
		}
    }
}
