using UnityEngine;
using System.Collections.Generic;

namespace TabTale.Analytics
{
	public class NullAnalyticsProvider : IAnalyticsService 
	{
		public long Targets { get; set; }

		public void LogEvent(string eventName, IDictionary<string,object> eventParams, bool timed)
		{
			Debug.Log("NullAnalyticsProvider - LogEvent : " + eventName);
		}

		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed)
		{
			Debug.Log("NullAnalyticsProvider - LogEvent : " + eventName);	
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,object> eventParams)
		{
			Debug.Log("NullAnalyticsProvider - EndLogEvent : " + eventName);
		}
		
		public void ReportPurchase(string price, string currency, string productId)
		{
			Debug.Log("NullAnalyticsProvider - ReportPurchase : " + productId);
		}

		public void ReviveReport(string levelConfigId,bool rvAvailable,bool lifeAvailable,bool videoUsed,bool lifeUsed, bool iapBuy, int reviveCounter, int popupType)
		{
			Debug.Log("NullAnalyticsProvider - ReviveReport : " + levelConfigId);
		}
		
		public void LevelReport(string levelConfigId, bool won ,bool firstTime, bool firstWin, int score)
		{
			Debug.Log("NullAnalyticsProvider - LevelReport : " + levelConfigId);
		}
		
		public void CinemaReport (string seriesId, string episodeId, string actionType, int duration, string completedStatus)
		{
			Debug.Log("NullAnalyticsProvider - CinemaReport : " + seriesId + "," + episodeId);
		}

		public void CustomReport (string endPoint, string data)
		{
			Debug.Log("NullAnalyticsProvider - CustomReport : " + endPoint + "," + data.ToString());
		}
	}
}