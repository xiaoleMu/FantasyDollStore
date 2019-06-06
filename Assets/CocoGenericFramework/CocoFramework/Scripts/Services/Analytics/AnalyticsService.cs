using UnityEngine;
using System.Collections.Generic;
using strange.extensions.command.impl;
using TabTale.Plugins.PSDK;
using LitJson;
using System.Collections;

namespace TabTale.Analytics
{
	public class AnalyticsService : IAnalyticsService
	{
		[Inject]
		public IGameDB gameDB { get; set; }

		[Inject]
		public IGsdkAnalyticsProvider gsdkAnalytics { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		private IPsdkAnalytics _psdkAnalytics;
		private IPsdkAnalytics PsdkAnalytics
		{
			get 
			{ 
				if(_psdkAnalytics != null)
				{
					return _psdkAnalytics;
				}
				else
				{
					_psdkAnalytics = PSDKMgr.Instance.GetAnalyticsService(); 
					return _psdkAnalytics;
				}
			}
		}

		private long _targets = AnalyticsTargets.ANALYTICS_TARGET_FLURRY;
		public long Targets
		{
			get { return _targets; }
			set { _targets = value; }
		}

		#region IAnalyticsService implementation

		public void LogEvent(string eventName, IDictionary<string,object> eventParams, bool timed = false)
		{
			PsdkAnalytics.LogEvent(_targets, eventName, eventParams, timed);
		}

		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed)
		{
			PsdkAnalytics.LogEvent(targets, eventName, eventParams, timed);
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,object> eventParams)
		{
			PsdkAnalytics.EndLogEvent(eventName, eventParams);
		}
		
		public void ReportPurchase(string price, string currency, string productId)
		{
			PsdkAnalytics.ReportPurchase(price,currency,productId);
		}

		public void ReviveReport (string levelConfigId,bool rvAvailable,bool lifeAvailable,bool videoUsed,bool lifeUsed, bool iapBuy, int reviveCounter, int popupType)
		{
			gsdkAnalytics.ReviveReport(levelConfigId,rvAvailable,lifeAvailable,videoUsed,lifeUsed,iapBuy,reviveCounter,popupType);
		}

		public void LevelReport(string levelConfigId, bool won ,bool firstTime, bool firstWin, int score)
		{
			gsdkAnalytics.LevelReport(levelConfigId,won,firstTime,firstWin,score);
		}
		
		public void CinemaReport (string seriesId, string episodeId, string actionType, int duration, string completedStatus)
		{
			gsdkAnalytics.CinemaReport(seriesId,episodeId,actionType,duration,completedStatus);
		}

		public void CustomReport (string endPoint, string data)
		{
			gsdkAnalytics.CustomReport(endPoint, data);
		}

		#endregion

	}
}