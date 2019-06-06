

using System.Collections.Generic;
using TabTale.Analytics;
using TabTale;
using UnityEngine;

namespace CocoPlay
{
	public class CocoFlurry
	{

		private AnalyticsService _service;
		private static CocoFlurry _instance;

		private static CocoFlurry Instance {
			get {
				if (_instance == null) {
					_instance = new CocoFlurry ();

					var strangeRoot = GameApplication.Instance.ModuleContainer.Get<StrangeRoot> ();
					var gameRoot = strangeRoot.GameRoot;
					var binder = ((GameContext)gameRoot.context).injectionBinder;
					_instance._service = binder.GetInstance<AnalyticsService> ();
				}
				return _instance;
			}
		}
		
#if !NO_FLURRY
		public static void LogEvent (string pEventName, Dictionary<string, object> eventParams)
		{
			if (CocoDebugSettingsData.Instance.IsFlurryLogEnabled) {
				var logMsg = string.Format ("CocoFlurry->LogEvent: [{0}] event - [{1}]: params: {{", Time.frameCount, pEventName);
				if (eventParams != null) {
					foreach (var param in eventParams) {
						logMsg = string.Format ("{0} <{1} - {2}>", logMsg, param.Key, param.Value);
					}
				}
				logMsg += " }]";
				Debug.LogError (logMsg);
			}

			Instance._service.LogEvent (pEventName, eventParams);
		}

		public static void LogEvent (string pEventName)
		{
			var eventParams = new Dictionary<string, object> (0);
			LogEvent (pEventName, eventParams);
		}

		public static void LogEvent (string pEventName, string eventKey, object eventValue)
		{
			var eventParams = new Dictionary<string, object> (1) { { eventKey, eventValue } };
			LogEvent (pEventName, eventParams);
		}

		public static void TimeStart (string pEventName)
		{
			if (CocoDebugSettingsData.Instance.IsFlurryLogEnabled) {
				Debug.LogErrorFormat ("CocoFlurry->TimeStart: [{0}] event [{1}]", Time.frameCount, pEventName);
			}

			var eventParams = new Dictionary<string, object> ();
			Instance._service.LogEvent (pEventName, eventParams, true);
		}

		public static void TimeEnd (string pEventName)
		{
			if (CocoDebugSettingsData.Instance.IsFlurryLogEnabled) {
				Debug.LogErrorFormat ("CocoFlurry->TimeEnd: [{0}] event [{1}]", Time.frameCount, pEventName);
			}

			var eventParams = new Dictionary<string, object> ();
			Instance._service.EndLogEvent (pEventName, eventParams);
		}
#endif

		#region Dalte DNA

		public static void LogEventDeltaDNA (string pEventName, Dictionary<string, object> eventParams)
		{
			if (CocoDebugSettingsData.Instance.IsFlurryLogEnabled) {
				var logMsg = string.Format ("CocoFlurry->LogEventDeltaDNA: [{0}] event - [{1}]: params: {{", Time.frameCount, pEventName);
				if (eventParams != null) {
					foreach (var param in eventParams) {
						logMsg = string.Format ("{0} <{1} - {2}>", logMsg, param.Key, param.Value);
					}
				}
				logMsg += " }]";
				Debug.LogError (logMsg);
			}

			Instance._service.LogEvent (AnalyticsTargets.ANALYTICS_TARGET_DELTA_DNA, pEventName, eventParams, false);
		}

		public static void LogEventDeltaDNA (string pEventName)
		{
			var eventParams = new Dictionary<string, object> (0);
			LogEventDeltaDNA (pEventName, eventParams);
		}

		#endregion
	}
}
