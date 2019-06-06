using System;
using UnityEngine;
using RSG;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace TabTale
{
	public enum EventScoresRequestType 
	{ 
		TopScores, 
		Me 
	}

	public class EventScores
	{
		//TODO: Write the object. See example in unity iap provider PurchaseIAPResult
	}

	public class EventLeaderboardData
	{
		public int rank = -1; // The player's rank if appears in leaderboard
		public List<LeaderboardPlayerData> leaderboardData;
	}

	public enum EventDispatch { noInternet, showCurrent, showAvailable, noEvents, requestFailed }

	public class EventSystemService
	{
		[Inject]
		public ServerTime serverTime { get; set; }

		[Inject]
		public EventConfigModel eventConfigModel { get; set; }

		[Inject]
		public EventStateModel eventStateModel { get; set; }

		[Inject]
		public ConnectionHandler connectionHandler { get; set; }

		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		[Inject]
		public IModalityManager modalityManager { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[Inject]
		public ModelSyncService modelSyncService { get; set; }

		[Inject]
		public IGameDB gameDB { get; set; }

		[Inject]
		public SyncSharedStateSignal syncSharedStateSignal { get; set; }

		[Inject]
		public PlayerInfoService playerInfoService { get; set; }

		[Inject]
		public RemoteImageService remoteImageService { get; set; }

		[Inject]
		public EventEndedSignal eventEndedSignal { get; set; }

		private string Tag
		{
			get { return "EventSystemService".Colored(Colors.blue); }
		}

		private DateTime Now
		{
			get { return serverTime.GetLocalTime(); }
		}

		private const string TableName = "event_state";

		private ICoroutineFactory _coroutineFactory;

		private List<EventStateData> _currentEvents = new List<EventStateData>();

		#region Getters
		public List<EventStateData> GetCurrentEvents()
		{
			List<EventConfigData> availableEvents = GetAvailableEvents();
			if(availableEvents.Count == 0)
			{
				_currentEvents.Clear();
				return _currentEvents;
			}

			List<EventStateData> unConcludedEvents = eventStateModel.GetUnConcludedEvents();
			EventStateData currEvent = unConcludedEvents.FirstOrDefault(ev => ev.id == availableEvents[0].id);

			// Avoid creating a new list for performance reasons, optimized for running on update:

			if(currEvent == null)
			{
				_currentEvents.Clear();
				return _currentEvents;
			}

			if(_currentEvents.Count == 0)
			{
				_currentEvents.Add(currEvent);
			}
			else
			{
				_currentEvents[0] = currEvent;
			}

			return _currentEvents;

		}


		private List<EventConfigData> _nextEvents = new List<EventConfigData> ();

		public List<EventConfigData> GetAvailableEvents()
		{
			// If no events - return the empty list
			if(_nextEvents.Count == 0)
			{
				return _nextEvents;
			}

			// Currently we only support one concurrent event
			EventConfigData nextEvent = _nextEvents[0];

			// Check if the event has ended and populate next events list:
			if(nextEvent.endTime < Now)
			{
				logger.Log(Tag, "Event ended : " + nextEvent.id);
				eventEndedSignal.Dispatch(nextEvent.id);

				PopulateNextEvents();
			}
				
			// Quick retreval of the next event (using a shallow copy, no performance overhead - can be used in an update loop)
			if(_nextEvents.Count > 0 && nextEvent.startTime < Now)
			{
				return _nextEvents.GetRange(0,1);
			}
			else
			{
				return _nextEvents.GetRange(0,0);
			}
		}

		#endregion

		[PostConstruct]
		public void Init()
		{
			_coroutineFactory = GameApplication.Instance.CoroutineFactory;

			PopulateNextEvents();

			LoadAvailableEventBannersToCache();
		}

		private void LoadAvailableEventBannersToCache()
		{
			logger.Log(Tag, "Loading available event banners to cache...");

			foreach (EventConfigData ev in GetAvailableEvents())
			{
				List<AssetData> assets = eventConfigModel.GetAssets (ev.id);
				foreach(AssetData asset in assets)
				{
					if(asset.assetType == AssetType.ServerImage)
					{
						remoteImageService.LoadRemoteImage(asset);
					}
				}
			}
		}

		public bool HaveCurrentEvents ()
		{
			return GetCurrentEvents().Count > 0;
		}

		public bool HaveAvailableEvents ()
		{
			return GetAvailableEvents().Count > 0;
		}

		private void PopulateNextEvents()
		{
			var events = eventConfigModel.GetAllConfigs ();

			// Get all future events, sort them by start time
			_nextEvents = events.Where(ev => ev.endTime > Now).OrderBy( ev => ev.startTime).ToList();
		}

		//		public EventScores GetScores(EventScoresRequestType requestType)
		//		{
		//			return 
		//		}

		#region Server Requests

		public IPromise<EventStateData> RegisterToEvent()
		{
			var promise = new Promise<EventStateData>();

			return promise;
		}

		public IPromise<EventStateData> RegisterToEvent(string eventID)
		{
			var promise = new Promise<EventStateData>();

			Action<ConnectionHandler.RequestResult, string> HandleRegisterToEventResponse = (ConnectionHandler.RequestResult result, string response) => {

				logger.Log(Tag,"HandleRegisterToEventResponse (result=" + result + ")\nresponse=" + response);

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					modelSyncService.HandleErrorResponse(result, response);
					promise.Reject(new Exception(response));
					return;
				}

				// Parse the response and save the event to db

				JsonData data = JsonMapper.ToObject(response);
				string id = data ["id"].ToString ();
				Debug.Log ("Saving sharedItem: table=" + TableName + " id=" + id + " data=" + data.ToJson ().SSubstring (200));
				gameDB.SaveSharedState (TableName, id, data.ToJson ());

				syncSharedStateSignal.Dispatch(new List<string> { TableName });

				EventStateData eventStateData = eventStateModel.GetEvent(id);
				if(eventStateData == null)
				{
					logger.LogError(Tag, "HandleRegisterToEventResponse : Could not load new registered event");
					promise.Reject(new Exception("Could not load event"));
					return;
				}


				promise.Resolve(eventStateData);
			};

			_coroutineFactory.StartCoroutine(() => connectionHandler.SendRequest(ConnectionHandler.RequestType.EventSystemRegistration, HandleRegisterToEventResponse, eventID, null));
			return promise;
		}

		public IPromise<EventLeaderboardData> GetTopLeaderboard(string eventID)
		{
			var promise = new Promise <EventLeaderboardData>();

			Action<ConnectionHandler.RequestResult, string> HandleGetTopLeaderboardResponse = (ConnectionHandler.RequestResult result, string response) => {

				logger.Log(Tag,"HandleGetTopLeaderboardResponse (result=" + result + ")\nresponse=" + response);

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					promise.Resolve(new EventLeaderboardData());
					return;
				}
					
				EventLeaderboardData topData = new EventLeaderboardData();

				// Parse out the leaderboard data from the response
				JsonData responseJsonObject = (JsonData)JsonMapper.ToObject (response);
				topData.leaderboardData = JsonMapper.ToObject<List<LeaderboardPlayerData>>(responseJsonObject["topLeaderboard"].ToJson());

				int tryFindPlayerInLeaderboard = topData.leaderboardData.FirstIndex(l => l.playerId.ToString() == playerInfoService.PlayerId);
				if(tryFindPlayerInLeaderboard != -1)
				{
					topData.rank = topData.leaderboardData[tryFindPlayerInLeaderboard].rank;
				}

				promise.Resolve(topData);
			};

			_coroutineFactory.StartCoroutine(() => connectionHandler.SendRequest(ConnectionHandler.RequestType.GetTopLeaderboardRequest, HandleGetTopLeaderboardResponse, eventID, null));
			return promise;
		}

		public IPromise<EventLeaderboardData> GetRankAndNeighbours(string eventID)
		{
			var promise = new Promise<EventLeaderboardData>();

			Action<ConnectionHandler.RequestResult, string> HandleRankAndNeighboursResponse = (ConnectionHandler.RequestResult result, string response) => {

				logger.Log(Tag,"HandleGetRankAndNeighboursResponse (result=" + result + ")\nresponse=" + response);

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					modelSyncService.HandleErrorResponse(result, response);
					promise.Reject(new Exception(response));
					return;
				}

				EventLeaderboardData leaderboardData = new EventLeaderboardData();
				JsonData responseJsonObject = (JsonData)JsonMapper.ToObject (response);

				leaderboardData.leaderboardData = JsonMapper.ToObject<List<LeaderboardPlayerData>>(responseJsonObject["neighborsLeaderboard"].ToJson());
				leaderboardData.rank = (int)responseJsonObject["rank"];

				promise.Resolve(leaderboardData);
			};

			_coroutineFactory.StartCoroutine(() => connectionHandler.SendRequest(ConnectionHandler.RequestType.GetRankAndNeighboursRequest, HandleRankAndNeighboursResponse, eventID, null));
			return promise;
		}

		#endregion

		public void UpdateEventScore (string eventID, int score)
		{
			EventStateData eventData = eventStateModel.GetEvent(eventID);
			if(eventData.score < score)
			{
				eventData.score = score;
				eventStateModel.SaveEvent(eventData);

				logger.Log(Tag, String.Format("Updated Event:{0} score:{1}",eventID,score));
			}
		}

		public string GetRandomLevelForEvent(string eventID)
		{
			List<String> levels = eventConfigModel.GetLevels (eventID);

			return levels[UnityEngine.Random.Range(0, levels.Count)];

		}

		public EventDispatch CheckEventState()
		{
			if (!networkCheck.HasInternetConnection ()) 
			{
				return EventDispatch.noInternet;	
			} 
			else 
			{
				if (HaveCurrentEvents ()) 
				{
					return EventDispatch.showCurrent;
				} 
				if (HaveAvailableEvents ()) 
				{
					return EventDispatch.showAvailable;
				} 
				else 
				{
					return EventDispatch.noEvents;
				}
			}
		}

		public TimeSpan TimeUntilEventEnds(string eventId)
		{
			DateTime endEventTime = eventConfigModel.GetEventEndDate(eventId);
			TimeSpan difference = endEventTime - serverTime.GetLocalTime ();

			return difference;
		}

		public string TimeUntilEventEndsFormatted(string eventId, string dailyTimeFormat = "{0} Days", string intraDayTimeFormat = "{0:0}:{1:00}:{2:00}")
		{
			TimeSpan difference = TimeUntilEventEnds(eventId);

			if(difference.Ticks < 0)
			{
				return "";

			}

			int hours = (int) difference.TotalHours;

			if (hours > 24) 
			{
				int daysLeft = (int)difference.TotalDays;
				string time = string.Format(dailyTimeFormat, daysLeft.ToString());
				return time;
			} 
			else 
			{
				int hourss = (int) difference.Hours;
				int minutes = (int) difference.Minutes;
				int seconds = (int) difference.Seconds;
				string time = String.Format(intraDayTimeFormat, hourss, minutes, seconds);
				return time;
			}

		}

		public List<EventStateData> GetEndedEvents()
		{
			var endedEvents = new List<EventStateData>();
			foreach(EventStateData ev in eventStateModel.GetUnConcludedEvents())
			{
				if (eventConfigModel.EventExists (ev.id)) {
					if (eventConfigModel.GetEventEndDate (ev.id) < serverTime.GetLocalTime ()) {
						endedEvents.Add (ev);
					}
				}
			}

			return endedEvents;
		}

	}
}

