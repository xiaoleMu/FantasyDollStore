using System;
using UnityEngine;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;
using System.Xml;
using System.IO;

namespace TabTale.Plugins.PSDK
{
	public class PsdkAnalyticsService : IPsdkAnalytics
	{
		IPsdkAnalytics _impl;
		
		public PsdkAnalyticsService(IPsdkServiceManager sm)
		{
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkAnalytics(sm.GetImplementation()); break;
				#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkAnalytics(sm.GetImplementation()); break;
				#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor: 	_impl = new UnityEditorPsdkAnalytics(sm.GetImplementation()); break;
			default: throw new System.Exception("Platform not supported for Analytics.");
			}
			
			//			#if UNITY_EDITOR            
			//            _impl = new UnityEditorPsdkSplash(sm.GetImplementation());
			//			#elif UNITY_ANDROID
			//			_impl = new AndroidPsdkSplash(sm.GetImplementation());
			//			#elif UNITY_IPHONE
			//			_impl = new IphonePsdkSplash(sm.GetImplementation());
			//			#else
			//            throw new Exception("Platform not supported for Splash.");
			//			#endif
		}

		public void LogTransaction(string transactionName,
		  					IDictionary<string,object> productsReceived,
		   					IDictionary<string,object> productsSpent,
							IDictionary<string,object> otherEventParams)   
							
		{
			Debug.Log("PsdkAnalyticsService::LogTransaction was called");
			Dictionary<string, object> log = new Dictionary<string, object>();
			if (transactionName != null) {
				log.Add("transactionName", transactionName);
			} 
			else {
				log.Add("transactionName", "transaction");
			}

			log.Add("transactionType", "PURCHASE");

			if (productsReceived != null) {
				log.Add("productsReceived", productsReceived);
			}
			else{
				log.Add("productsReceived", new Dictionary<string,object>());
			}

			if (productsSpent != null) {
				log.Add("productsSpent", productsSpent);
			}
			else{
				log.Add("productsSpent", new Dictionary<string,object>());
			}

			int userScore = PsdkEventSystem.Instance.CallGetPlayerScore ();
			if (userScore > -1) {
				log.Add ("userScore", userScore);
			}
				
			if (otherEventParams != null) {
				foreach (var item in otherEventParams) {
					log.Add (item.Key,item.Value);
				}
			}

			_impl.LogEvent (AnalyticsTargets.ANALYTICS_TARGET_DELTA_DNA, "transaction", log, false, true);					
		}



		public IDictionary<string,object> generateProductsReceived(IDictionary<string,object>[] items,
															 IDictionary<string,object> realCurrency, 
															 IDictionary<string,object>[] virtualCurrencies)
		{
			Dictionary<string, object> itemDDNA = new Dictionary<string, object>();
			if (items != null) {
				itemDDNA.Add("items", items);
			}

			if (realCurrency != null) {
				itemDDNA.Add("realCurrency", realCurrency);
			}
				
			if (virtualCurrencies != null) {
				itemDDNA.Add("virtualCurrencies", virtualCurrencies);
			}
				
			return itemDDNA;
		}

		public IDictionary<string,object> generateProductsSpent(IDictionary<string,object>[] items,
															 IDictionary<string,object> realCurrency, 
															 IDictionary<string,object>[] virtualCurrencies)
		{
			return generateProductsReceived(items, realCurrency, virtualCurrencies);
		}

		public IDictionary<string,object> generateItem(int itemAmount, string itemName, string itemType)
		{
			if (itemName == null || itemType == null) {
				Debug.LogError ("no itemName or itemType");
				return null;
			}

			IDictionary<string, object> itemDDNA = new Dictionary<string, object>();
			itemDDNA.Add("itemAmount", itemAmount);
			itemDDNA.Add("itemName", itemName);
			itemDDNA.Add("itemType", itemType);

			IDictionary<string, object> itemEncapsulate = new Dictionary<string,object>();
			itemEncapsulate.Add("item",itemDDNA);

			return itemEncapsulate;
		}

		public IDictionary<string,object> generateVirtualCurrency(int virtualCurrencyAmount, string virtualCurrencyName, string virtualCurrencyType)
		{
			if (virtualCurrencyName == null || virtualCurrencyType == null) {
				Debug.LogError ("no virtualCurrencyName or virtualCurrencyType");
				return null;
			}

			IDictionary<string, object> vcDDNA = new Dictionary<string, object>();
			vcDDNA.Add("virtualCurrencyAmount", virtualCurrencyAmount);
			vcDDNA.Add("virtualCurrencyName", virtualCurrencyName);
			vcDDNA.Add("virtualCurrencyType", virtualCurrencyType);

			IDictionary<string, object> vcEncapsulate = new Dictionary<string,object>();
			vcEncapsulate.Add("virtualCurrency",vcDDNA);

			return vcEncapsulate;
		}

		public IDictionary<string,object> generateRealCurrency(string realCurrencyAmount, string realCurrencyType)
		{
			if (realCurrencyType == null) {
				Debug.LogError ("no realCurrencyType");
				return null;
			}

			decimal dec = Convert.ToDecimal(realCurrencyAmount);
			int priceInt = ConvertCurrency (realCurrencyType, dec);

			IDictionary<string, object> grDDNA = new Dictionary<string, object>();
			grDDNA.Add("realCurrencyAmount", priceInt);
			grDDNA.Add("realCurrencyType", realCurrencyType);

			return grDDNA;
		}

		public int ConvertCurrency(string code, decimal value) {
			IDictionary<string, int> ISO4217 = getISOValue ();
			if (ISO4217.ContainsKey(code)) {
				return decimal.ToInt32(value * (decimal) Math.Pow(10, ISO4217[code]));
			} else {
				Debug.LogWarning("Failed to find currency for: " + code);
				return 0;
			}
		}

		private IDictionary<string, int> getISOValue()
		{
			IDictionary<string, int> ISO4217 = new Dictionary<string, int>();
			string iso4217_file_to_string = PsdkUtils.ReadStreamingAssetsFile("iso_4217.xml");
			using (XmlReader reader = XmlReader.Create(new StringReader(iso4217_file_to_string))) {
				bool expectingCode = false;
				bool expectingValue = false;
				string pulledCode = null;
				string pulledValue = null;

				while (reader.Read()) {
					switch (reader.NodeType) {
					case XmlNodeType.Element:
						if (reader.Name.Equals("Ccy")) {
							expectingCode = true;
						} else if (reader.Name.Equals("CcyMnrUnts")) {
							expectingValue = true;
						}
						break;

					case XmlNodeType.Text:
						if (expectingCode) {
							pulledCode = reader.Value;
						} else if (expectingValue) {
							pulledValue = reader.Value;
						}
						break;

					case XmlNodeType.EndElement:
						if (reader.Name.Equals("Ccy")) {
							expectingCode = false;
						} else if (reader.Name.Equals("CcyMnrUnts")) {
							expectingValue = false;
						} else if (reader.Name.Equals("CcyNtry")) {
							if (!string.IsNullOrEmpty(pulledCode)
								&& !string.IsNullOrEmpty(pulledValue)) {
								int value;
								try {
									value = int.Parse(pulledValue);
								} catch (FormatException) {
									value = 0;
								}

								ISO4217[pulledCode] = value;
							}

							expectingCode = false;
							expectingValue = false;
							pulledCode = null;
							pulledValue = null;
						}
						break;
					}
				}
			}

			return ISO4217;
		}

		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed)
		{
			_impl.LogEvent (targets, eventName, eventParams, timed);
		}

		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed, bool psdkEvent)
		{
			_impl.LogEvent(targets,eventName,eventParams,timed,psdkEvent);
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,object> eventParams)
		{
			_impl.EndLogEvent (eventName, eventParams);
		}
		
		public void LogEvent(string eventName, IDictionary<string,string> eventParams, bool timed){
			IDictionary<string,object> ep = null;
			if (eventParams != null) {
				ep = new Dictionary<string,object> ();
				foreach (var item in eventParams) {
					ep.Add (item.Key,item.Value);
				}
			}
			_impl.LogEvent (AnalyticsTargets.ANALYTICS_PSDK_INTERNAL_TARGETS, eventName,ep,timed);
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,string> eventParams){
			_impl.EndLogEvent (eventName,eventParams);
		}
		
		public void LogComplexEvent(string eventName, IDictionary<string, object> eventParams) {
			_impl.LogComplexEvent (eventName,eventParams);
		}

		public void ReportPurchase(string price, string currency, string productId) {
			_impl.ReportPurchase (price, currency, productId);
		}
		
		public IPsdkAnalytics GetImplementation() {
			return _impl;
		}

		public bool RequestEngagement(string decisionPoint, Dictionary<string,object> parameters)
		{
			return _impl.RequestEngagement(decisionPoint,parameters);
		}
		
		public void psdkStartedEvent() {
			_impl.psdkStartedEvent();
		}

		public void TutorialStep(bool isMandatory, int tutorialStepID, string tutorialName, string tutorialStepName, Dictionary<string,object> additionalParams)
		{
			IDictionary<string, object> itemDDNA = new Dictionary<string, object>();
			itemDDNA.Add("stepType", isMandatory?"mandatory":"optional");
			itemDDNA.Add("tutorialStepID", tutorialStepID);
			itemDDNA.Add("tutorialName", tutorialName);
			itemDDNA.Add("tutorialStepName", tutorialStepName);

			if (additionalParams != null) {
				foreach (var item in additionalParams) {
					itemDDNA.Add (item.Key,item.Value);
				}
			}

			LogEvent(AnalyticsTargets.ANALYTICS_TARGET_DELTA_DNA, "tutorialStep", itemDDNA, false, true);
		}

		public void ReachedMainScreen(Dictionary<string,object> additionalParams)
		{
			LogEvent (AnalyticsTargets.ANALYTICS_TARGET_DELTA_DNA, "mainScreen", additionalParams, false, true);
		}

		public void LevelUp (string skinName, string levelUpName, int level, Dictionary<string,object> additionalParams)
		{
			_impl.LevelUp (skinName, levelUpName, level, additionalParams);
		}

		public void MissionStarted (int id, string name, string type, Dictionary<string,object> additionalParams)
		{
			_impl.MissionStarted (id, name, type, additionalParams);
		}
		public void MissionComplete (Dictionary<string,object> additionalParams)
		{
			_impl.MissionComplete (additionalParams);
		}
	}
}
