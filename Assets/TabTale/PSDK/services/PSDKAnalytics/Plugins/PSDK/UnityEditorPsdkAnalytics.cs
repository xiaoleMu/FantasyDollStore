using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TabTale.Plugins.PSDK;
//using Json = TabTale.Plugins.PSDK.PSDKMiniJSON;

namespace TabTale.Plugins.PSDK
{
	public class UnityEditorPsdkAnalytics : IPsdkAnalytics 
    {

		public UnityEditorPsdkAnalytics(IPsdkServiceManager sm) {
		}

		public IPsdkAnalytics GetImplementation() {
			return this;
		}

		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed){}
		
		public void EndLogEvent(string eventName, IDictionary<string,object> eventParams){
		}

		public void psdkStartedEvent() {
		}

		public void LogEvent(string eventName, IDictionary<string,string> eventParams, bool timed){
			Debug.Log ("PSDK Analytic Event: " + eventName + " -> " + PsdkUtils.BuildJsonStringFromDict(eventParams) + ", timed:" + timed);
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,string> eventParams){
			Debug.Log ("PSDK Analytic End Event: " + eventName + " -> " + PsdkUtils.BuildJsonStringFromDict(eventParams));
		}

		public void LogComplexEvent(string eventName, IDictionary<string, object> eventParams) {
			Debug.Log ("PSDK Analytic Log Complex Event: " + eventName + " -> " + Json.Serialize(eventParams));
		}

		public void ReportPurchase(string price, string currency, string productId) {
		}

		public bool RequestEngagement(string decisionPoint, Dictionary<string,object> parameters)
		{
			return false;
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
		}

		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed, bool psdkEvent)
		{

		}

		public void MissionStarted (int id, string name, string type, Dictionary<string,object> additionalParams)
		{
		}
		public void MissionComplete (Dictionary<string,object> additionalParams)
		{
		}
    }
}
