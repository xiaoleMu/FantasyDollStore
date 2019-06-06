using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.Plugins.PSDK {
	/// <summary>
	/// Psdk Splash Interface
	/// </summary>
	/// 
	/// 

	public class AnalyticsTargets{
		public const long ANALYTICS_TARGET_FLURRY = 1;
		public const long ANALYTICS_TARGET_TT_ANALYTICS = 1 << 1;
		public const long ANALYTICS_TARGET_DELTA_DNA = 1 << 2;
		public const long ANALYTICS_PSDK_INTERNAL_TARGETS = ANALYTICS_TARGET_FLURRY | ANALYTICS_TARGET_TT_ANALYTICS;
	}

	public interface IPsdkAnalytics  : IPsdkService {

		void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed); 

		void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed, bool psdkEvent);
		
		void EndLogEvent(string eventName, IDictionary<string,object> eventParams);


		void LogTransaction(string transactionName,
		  					IDictionary<string,object> productsReceived,
		   					IDictionary<string,object> productsSpent,
							IDictionary<string,object> otherEventParams);   

		IDictionary<string,object> generateProductsReceived(IDictionary<string,object>[] items,
															 IDictionary<string,object> realCurrency, 
															 IDictionary<string,object>[] virtualCurrencies);

		IDictionary<string,object> generateProductsSpent(IDictionary<string,object>[] items,
														 IDictionary<string,object> realCurrency, 
														 IDictionary<string,object>[] virtualCurrencies);

		IDictionary<string,object> generateItem(int itemAmount, string itemName, string itemType);

		IDictionary<string,object> generateVirtualCurrency(int virtualCurrencyAmount, string virtualCurrencyName, string virtualCurrencyType);

		IDictionary<string,object> generateRealCurrency(string realCurrencyAmount, string realCurrencyType);

		void TutorialStep (bool isMandatory, int tutorialStepID, string tutorialName, string tutorialStepName, Dictionary<string,object> additionalParams);
		void ReachedMainScreen (Dictionary<string,object> additionalParams);
		void LevelUp (string skinName, string levelUpName, int level, Dictionary<string,object> additionalParams);

		//backwards compatibility
		void LogEvent(string eventName, IDictionary<string,string> eventParams, bool timed); 
	
		void EndLogEvent(string eventName, IDictionary<string,string> eventParams);
	
		void LogComplexEvent(string eventName, IDictionary<string, object> eventParams);

		void ReportPurchase(string price, string currency, string productId);

		bool RequestEngagement(string decisionPoint, Dictionary<string,object> parameters);

		/// <summary>
		/// Gets the iPhone/Android implementation.
		/// </summary>
		/// <returns>The implementation.</returns>
		IPsdkAnalytics GetImplementation();

		void MissionStarted (int id, string name, string type, Dictionary<string,object> additionalParams);
		void MissionComplete (Dictionary<string,object> additionalParams);
	}
}
