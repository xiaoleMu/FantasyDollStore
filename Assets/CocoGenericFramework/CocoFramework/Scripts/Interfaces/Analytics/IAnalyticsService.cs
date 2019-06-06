using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace TabTale.Analytics 
{
	public class AnalyticsTargets
	{
		public const long ANALYTICS_TARGET_FLURRY = 1;
		public const long ANALYTICS_TARGET_TT_ANALYTICS = 1 << 1;
		public const long ANALYTICS_TARGET_DELTA_DNA = 1 << 2;
		public const long ANALYTICS_PSDK_INTERNAL_TARGETS = ANALYTICS_TARGET_FLURRY | ANALYTICS_TARGET_TT_ANALYTICS;
	}

	public enum GsdkAnalyticsEventType
	{
		LevelReport,
		ReviveReport,
		CinemaReport,
		CustomReport
	}

	public interface IAnalyticsService : IGsdkAnalyticsProvider
	{
		long Targets { get; set; }

		void LogEvent(string eventName, IDictionary<string,object> eventParams, bool timed); 

		void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed); 
		
		void EndLogEvent(string eventName, IDictionary<string,object> eventParams);
		
		void ReportPurchase(string price, string currency, string productId);
	}

	public interface IGsdkAnalyticsProvider
	{
		void ReviveReport(string levelConfigId,bool rvAvailable,bool lifeAvailable,bool videoUsed,bool lifeUsed, bool iapBuy, int reviveCounter, int popupType);
		
		void LevelReport(string levelConfigId, bool won ,bool firstTime, bool firstWin, int score);
		
		void CinemaReport(string seriesId, string episodeId, string actionType, int duration, string completedStatus);

		void CustomReport(string endPoint, string data);
	}
}
