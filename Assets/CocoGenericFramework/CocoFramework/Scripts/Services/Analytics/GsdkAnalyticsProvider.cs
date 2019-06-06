using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace TabTale.Analytics
{
	public class GsdkAnalyticsProvider : IGsdkAnalyticsProvider
	{	
		[Inject]
		public IGameDB gameDB { get; set; }

		[Inject] 
		public IRoutineRunner routineRunner { get; set; }

		private ICoroutineFactory _coroutineFactory;
		
		private ConnectionHandler _connectionHandler;

		[PostConstruct]
		public void Init()
		{
			_coroutineFactory = GameApplication.Instance.CoroutineFactory;
			_connectionHandler = new ConnectionHandler();
			_connectionHandler.Init(gameDB,_coroutineFactory);
		}
		private void LogGsdkEvent(GsdkAnalyticsEventType eventType, JsonData data, string customEndPoint = null)
		{
			routineRunner.StartCoroutine(_connectionHandler.SendAnalyticsReport (eventType, data.ToJson(), customEndPoint));
		}
		
		public void ReviveReport (string levelConfigId,bool rvAvailable,bool lifeAvailable,bool videoUsed,bool lifeUsed, bool iapBuy, int reviveCounter, int popupType)
		{
			int levelId = 0;
			if(! int.TryParse(levelConfigId, out levelId)) 
				return;

			// If the player used iap to revive, the iap was usually used to get life, but we won't want to account for that since the life was purchased by iap
			if(iapBuy)
			{
				lifeUsed = false;
			}

			JsonData data = new JsonData();
			bool nothingUsed = !(videoUsed || lifeUsed || iapBuy);
			data["playerId"]= TTPlayerPrefs.GetValue("playerId","0");
			data["createdDate"]=gameDB.LoadSyncInfo("createdDate");
			data["configVersion"]=gameDB.LoadSyncInfo("configVersion");
			#if UNITY_IOS
			data["platform"]="iOS";
			#else
			data["platform"]="Android";
			#endif
			data["level"]=levelId;
			data["videoAvailable"]=rvAvailable;
			data["lifeAvailable"]=lifeAvailable;
			data["videoUsed"]=videoUsed;
			data["lifeUsed"]=lifeUsed;
			data["nothingUsed"]=nothingUsed;
			data["iapBuy"]=iapBuy;
			data["ReviveCounter"]=reviveCounter;
			data["popupType"]=popupType;
			
			LogGsdkEvent(GsdkAnalyticsEventType.ReviveReport,data);
		}
		
		public void LevelReport(string levelConfigId, bool won ,bool firstTime, bool firstWin, int score)
		{
			int levelId = 0;
			if(! int.TryParse(levelConfigId, out levelId)) 
				return;
			
			JsonData data = new JsonData();
			data["playerId"] = TTPlayerPrefs.GetValue("playerId","0");
			data["createdDate"] = gameDB.LoadSyncInfo("createdDate");
			data["configVersion"] = gameDB.LoadSyncInfo("configVersion");
			#if UNITY_IOS
			data["platform"] = "iOS";
			#else
			data["platform"] = "Android";
			#endif
			data["level"] = levelId;
			data["win"] = won;
			data["firstTime"] = firstTime;
			data["firstWin"] = firstWin;
			data["score"] = score;
			
			LogGsdkEvent(GsdkAnalyticsEventType.LevelReport,data);
		}
		
		public void CinemaReport (string seriesId, string episodeId, string actionType, int duration, string completedStatus)
		{
			JsonData data = new JsonData();
			data["playerId"]= TTPlayerPrefs.GetValue("playerId","0");
			data["createdDate"]=gameDB.LoadSyncInfo("createdDate");
			data["configVersion"]=gameDB.LoadSyncInfo("configVersion");
			#if UNITY_IOS
			data["platform"]="iOS";
			#else
			data["platform"]="Android";
			#endif
			
			data["seriesId"] = seriesId;
			data["episodeId"] = episodeId;
			data["actionType"] = actionType;
			data["duration"] = duration;
			data["completedStatus"] = completedStatus;
			
			LogGsdkEvent(GsdkAnalyticsEventType.CinemaReport,data);
		}

		public void CustomReport(string endPoint, string data)
		{
			JsonData jsonData = new JsonData(data);

			routineRunner.StartCoroutine(_connectionHandler.SendAnalyticsReport (GsdkAnalyticsEventType.CustomReport, data, endPoint));
		}
	}
}