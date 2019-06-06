using System;
using TabTale.Analytics;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace TabTale.Analytics
{
	public enum GeneralAnalyticsEvents
	{
		IapShopImpression,
		IapShop,
		AdShow
	}

	public partial class AnalyticsEvents
	{
		[Inject] public IAnalyticsService analyticsService { get; set; }

		private const int MAX_REPORT_MINUTES = 300;

		private IDictionary<string,object> eventParams = new Dictionary<string, object>();

		private int GamePlayMinutes
		{
			get
			{
				int minutesPlayed = (int)(Time.realtimeSinceStartup / 60.0f);
				return (minutesPlayed < MAX_REPORT_MINUTES) ? minutesPlayed : MAX_REPORT_MINUTES;
			}
		}

		private void SetDefaultEventParams()
		{
			eventParams.Clear ();
			eventParams ["game_time"] = GamePlayMinutes;
		}

		public void ReportGenericEvent(GeneralAnalyticsEvents eventName)
		{
			ReportGenericEvent(eventName.ToName());
		}

		public void ReportGenericEvent(string eventName)
		{
			SetDefaultEventParams();
			analyticsService.LogEvent(AnalyticsTargets.ANALYTICS_TARGET_FLURRY, eventName, eventParams, false);
		}

		public void ReportGenericEvent(string eventName, Dictionary<string,object> additionalEventParams)
		{
			SetDefaultEventParams();
			additionalEventParams.ForEach (eventParams.Add);
			analyticsService.LogEvent(AnalyticsTargets.ANALYTICS_TARGET_FLURRY, eventName, eventParams, false);

			foreach (KeyValuePair<string, object> entry in eventParams) 
			{
#if UNITY_2017_1_OR_NEWER
				Debug.unityLogger.Log ("<color=green>" + entry.Key + " :: " + entry.Value + "</color>");
#else
				Debug.logger.Log ("<color=green>" + entry.Key + " :: " + entry.Value + "</color>");
#endif
			}
		}

		public void ReportIAPPurchase(string itemId)
		{
			SetDefaultEventParams();
			eventParams["item"] = itemId;
			analyticsService.LogEvent(AnalyticsTargets.ANALYTICS_TARGET_FLURRY, "iap_purchase", eventParams, false);
		}

		public void ReportIAPPurchaseCompleted(string itemId)
		{
			SetDefaultEventParams();
			eventParams["item"] = itemId;
			analyticsService.LogEvent(AnalyticsTargets.ANALYTICS_TARGET_FLURRY, "iap_purchase_completed", eventParams, false);
		}
	}

	public static class GeneralAnalyticsEventsExtensions
	{
		public static string ToName(this GeneralAnalyticsEvents eventName)
		{
			switch(eventName)
			{
			case GeneralAnalyticsEvents.IapShopImpression:
				return "iap_shop_impression";
			case GeneralAnalyticsEvents.IapShop:
				return "iap_shop";
			case GeneralAnalyticsEvents.AdShow:
				return "ad_show";
			default:
				return "";
			}
		}
	}
}

