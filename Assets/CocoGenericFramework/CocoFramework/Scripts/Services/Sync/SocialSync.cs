using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using strange.extensions.context.api;
using strange.extensions.signal.impl;
using System;

namespace TabTale
{
	public class SocialConnectData
	{
		public SocialNetwork network = SocialNetwork.NoNetwork;
		public string email = "";
		public string photoUrl = "";
		public string userName = "";
		public string userId = "";
		public string userToken;
		public bool firstTimeConnect = false;
		public bool initiatedByUser = false;
	}

	public class SocialSync : ISocialSync
	{
		//public enum NetworkType { Facebook = 0, GameCenter, PlayServices, NoNetwork };

		[Inject]
		public SocialStateModel socialStateModel{get;set;}
		[Inject]
		public IGameDB gameDB {get;set;}
		[Inject]
		public ModelSyncService modelSyncService{get;set;}
		[Inject]
		public ConnectionAllowUserToSwitchUserSignal connectionAllowUserToSwitchUserSignal { get; set; }
		[Inject]
		public ConnectionAllowUserToStartNewGameSignal connectionAllowUserToStartNewGameSignal { get; set; }
		[Inject]
		public SocialNetworkServerVerificationCompleteSignal handleSocialServerVerificationSignal { get; set; }  
		[Inject]
		public SocialSyncCompletedSignal socialSyncCompletedSignal { get; set; }
		[Inject]
		public RelationshipScoreStateModel relationshipScoreStateModel { get; set; }
		[Inject]
		public GeneralParameterConfigModel generalParameterConfigModel { get; set; }
		[Inject]
		public RelationshipsScoresUpdateSignal relationshipsScoresUpdateSignal { get; set; }
		[Inject]
		public SyncSharedStateSignal syncSharedStateSignal{get;set;}
		[Inject]
		public SyncStatesSignal syncStatesSignal{ get; set; }
		[Inject]
		public SocialSyncStartedSignal socialSyncStartedSignal{ get; set; }
		[Inject]
		public ILogger logger { get; set; }

		ICoroutineFactory _coroutineFactory;
		protected int _loggerModule; 

		private ConnectionHandler _connectionHandler;

		private RelationshipsScoresResponseHandler _relationShipScoreResponseHandler;
		
		private string keyPlayerId = "playerId";
		private string keyPlayerName = "playerName";
		private string keySocialType = "socialType";
		private string keySocialStateData= "socialState";
		private string keyLastStateUpdate = "lastStateUpdate";
		private string _configKeyFakeFacebookId = "fakeSFacebookId";
		private string _configKeyFakeGameCenterId = "fakeGameCenterId";
		private string _configKeyFakePlayServicesId = "fakePlayServicesId";
		private string _fakeFacebookId = "fakeSFacebookId";
		private string _fakeGameCenterId = "fakeGameCenterId";
		private string _fakePlayServicesId = "fakePlayServicesId";
		
		private string _lastRecievedPlayerId;

		public Dictionary<SocialNetwork, bool> _socialConnectionStatus;
		public Dictionary<SocialNetwork, string> _socialUserId;

		private const string Tag = "SocialSync";

		public SocialSync()
		{
		}

		[PostConstruct]
		public void Init()
		{
			Debug.Log ("SocialSync.Init");

			_coroutineFactory = GameApplication.Instance.CoroutineFactory;
			_connectionHandler = new ConnectionHandler();
			_connectionHandler.Init(gameDB,_coroutineFactory);
			_loggerModule = CoreLogger.RegisterModule(GetType().Name);

			SocialStateData socialData = socialStateModel.GetState();

			_socialConnectionStatus = new Dictionary<SocialNetwork, bool>();
			_socialConnectionStatus.Add(SocialNetwork.Facebook, (socialData.facebookId != ""));
			_socialConnectionStatus.Add(SocialNetwork.GameCenter, (socialData.gameCenterId != ""));
			_socialConnectionStatus.Add(SocialNetwork.PlayServices, (socialData.googlePlayId != ""));
			_socialUserId = new Dictionary<SocialNetwork, string>();
			_socialUserId.Add(SocialNetwork.Facebook, socialData.facebookId);
			_socialUserId.Add(SocialNetwork.GameCenter, socialData.gameCenterId);
			_socialUserId.Add(SocialNetwork.PlayServices, socialData.googlePlayId);
			
			Data.DataElement connConfig = GameApplication.GetConfiguration("connection");
			if (!connConfig.IsNull)
			{
				_fakeFacebookId = (!connConfig.ContainsKey(_configKeyFakeFacebookId)) ? "0" : (string)connConfig[_configKeyFakeFacebookId];
				_fakeGameCenterId = (!connConfig.ContainsKey(_configKeyFakeGameCenterId)) ? "0" : (string)connConfig[_configKeyFakeGameCenterId];
				_fakePlayServicesId = (!connConfig.ContainsKey(_configKeyFakePlayServicesId)) ? "0" : (string)connConfig[_configKeyFakePlayServicesId];
			}
		}
		
		#region social network user Sync activation
		
		public void ConnectToNetwork(SocialConnectData socialConnectData)
		{
			string userIdAfterOverride = socialConnectData.userId;
			if (socialConnectData.network == SocialNetwork.Facebook && _fakeFacebookId != "0")
				userIdAfterOverride = _fakeFacebookId;
			if (socialConnectData.network == SocialNetwork.GameCenter && _fakeGameCenterId != "0")
				userIdAfterOverride = _fakeGameCenterId;
			if (socialConnectData.network == SocialNetwork.PlayServices && _fakePlayServicesId != "0")
				userIdAfterOverride = _fakePlayServicesId;
			if (userIdAfterOverride == socialConnectData.userId)
				logger.Log(Tag,"SocialSync.ConnectToNetwork: network=" + socialConnectData.network.ToString() + " userId=" + socialConnectData.userId 
				          + " (not overriden by config) token=" + socialConnectData.userToken);
			else
				logger.Log(Tag,"SocialSync.ConnectToNetwork: network=" + socialConnectData.network.ToString() + " userId=" + socialConnectData.userId 
				          + " (after config override=" + userIdAfterOverride + ") token=" + socialConnectData.userToken);
			socialConnectData.userId = userIdAfterOverride;

			_coroutineFactory.StartCoroutine(() => ConnectToNetworkCoro(socialConnectData));
		}

		private IEnumerator ConnectToNetworkCoro(SocialConnectData socialConnectData)
		{


			_socialUserId[socialConnectData.network] = socialConnectData.userId;
			if (socialStateModel.GetNetworkUserId(socialConnectData.network) == socialConnectData.userId) // 1. If no change done - dont call server
			{
				logger.Log(Tag,"ConnectToNetwork - connected to " + socialConnectData.network.ToString());
				_socialConnectionStatus[socialConnectData.network] = true;
				if (socialConnectData.initiatedByUser)
				{
					handleSocialServerVerificationSignal.Dispatch(socialConnectData.network);
				}

				socialSyncCompletedSignal.Dispatch();
				yield break;
			}

			logger.Log(Tag,"ConnectToNetworkCoro - attempting to set user id of " + socialConnectData.network.ToString() + " to " + socialConnectData.userId);

			logger.Log(Tag,"ConnectToNetworkCoro - waiting for ModelSync to connect");
			yield return _coroutineFactory.StartCoroutine(() => modelSyncService.WaitForState(ModelSyncService.SyncLoopState.Connected));
			
			logger.Log(Tag,"ConnectToNetworkCoro - sending request");
			string previousUserId = socialStateModel.GetNetworkUserId(socialConnectData.network);
			bool firstTimeConnect = (previousUserId == null || previousUserId.Length == 0); // this will dictate GET/POST request (by adding body if true)
			logger.Log(Tag,"firstTimeConnect= " + firstTimeConnect + " previousUserId=[" + previousUserId + "]");

			socialConnectData.firstTimeConnect = firstTimeConnect;

			socialSyncStartedSignal.Dispatch();

			yield return _coroutineFactory.StartCoroutine(() => _connectionHandler.SendRequest(ConnectionHandler.RequestType.ConnectToSocialNetwork, 
			                                                                 HandleConnectToNetworkResponse, "", socialConnectData));
		}

		#endregion
		
		#region Sync response functions


		private void HandleConnectToNetworkResponse(ConnectionHandler.RequestResult result, string response)
		{
			logger.Log(Tag,"HandleConnectToNetworkResponse (result=" + result + ")\nresponse=" + response);

			if (result != ConnectionHandler.RequestResult.Ok)
			{
				modelSyncService.HandleErrorResponse(result, response);
				return;
			}

			JsonData responseJsonObject= JsonMapper.ToObject(response);
			_lastRecievedPlayerId = (string)responseJsonObject[keyPlayerId];
			NewSocialPlayerData playerData = new NewSocialPlayerData();
			playerData.id = _lastRecievedPlayerId;

			// Response body contains SocialStateData:
			if(responseJsonObject.Keys.Contains(keySocialStateData))
			{
				logger.Log (Tag, "Got social update");
				JsonData jsonData = (JsonData)responseJsonObject[keySocialStateData];
				string data = jsonData.ToJson();

				if (data.CompareTo ("{}") != 0) 
				{
					gameDB.SyncState (keySocialStateData, data);
					syncStatesSignal.Dispatch(new List<string> { keySocialStateData });
				}
			}

			//playerData.socialType = SocialStateData.GetNetworkFromString((string)responseJsonObject[keySocialType]);

			string currentPlayerId = TTPlayerPrefs.GetValue(keyPlayerId, "0");
			//string currentPlayerId = gameDB.LoadSyncInfo(keyPlayerId);

			if (_lastRecievedPlayerId.CompareTo("0") == 0)
			{
				logger.Log(Tag, "HandleConnectToNetworkResponse - got \"0\", no action required");
				//Logger.LogDebug ("SocialSync.HandleConnectToNetworkResponse - got \"0\", after changing network user => asking player to choose start new game or keep playing without being connected to this network.");
				//_socialConnectionStatus[playerData.socialType] = false;
				//connectionAllowUserToStartNewGameSignal.Dispatch(playerData);

				socialSyncCompletedSignal.Dispatch();
			}
			else if (currentPlayerId.CompareTo(_lastRecievedPlayerId) != 0)
			{
				logger.Log(Tag,"HandleConnectToNetworkResponse - got new user id from server: " + _lastRecievedPlayerId +", switching to it from " 
				                + currentPlayerId);
				//Logger.LogDebug ("SocialSync.HandleConnectToNetworkResponse - got new user id from server, allowing player to replace the game playerId.\nOld="
				//           + currentPlayerId + " new=" + _lastRecievedPlayerId);
				//_socialConnectionStatus[playerData.socialType] = false;
				//connectionAllowUserToSwitchUserSignal.Dispatch(playerData);
				SwitchUser(playerData.id);
			}
			else
			{
				logger.Log(Tag,"HandleConnectToNetworkResponse - got same user id, first time connected to this network => saving new network user id in state.");
				socialStateModel.SetNetworkUserId(playerData.socialType, _socialUserId[playerData.socialType]);
				//_socialConnectionStatus[playerData.socialType] = true;

				socialSyncCompletedSignal.Dispatch();
				
			}
		}

		public void SwitchUser(string playerId)
		{
			if (playerId.CompareTo("0") == 0)
				logger.Log(Tag,"SwitchUser - staring new game");
			else
				logger.Log(Tag,"SwitchUser - switching to playerId " + playerId);
			gameDB.SaveSyncInfo(keyPlayerId, playerId);
			gameDB.SaveSyncInfo(keyLastStateUpdate, "");
			modelSyncService.SwitchUserAndRestartFlow(playerId);
		}


		#endregion

		private const string RELATIONS_SCORES_KEY_PREFIX = "rslt_";
		private const string RELATIONS_SCORES_REQUEST_INTERVAL = "RelationsScoreRequestInterval";

		public void RefreshRelationships()
		{
			ConnectionHandler connectionHandler = new ConnectionHandler();
			connectionHandler.Init(gameDB,_coroutineFactory);

			_coroutineFactory.StartCoroutine (() => _connectionHandler.SendRequest (ConnectionHandler.RequestType.RefreshRelationships, (result, response) => { } ));
		}

		public void RefreshRelationsScores (string levelConfigId)
		{
			int n;
			bool isNumeric = int.TryParse(levelConfigId, out n);
			if(!isNumeric)
			{
				Debug.LogError("SocialSync.RefreshRelationsScores - cannot get score for a non-numeric level config id. levelConfigId=" + levelConfigId);
			}

			string key = RELATIONS_SCORES_KEY_PREFIX + levelConfigId;
			string lastTimeString = PlayerPrefs.GetString(key, "0");
			DateTime lastTime = DateTime.FromFileTimeUtc(long.Parse(lastTimeString));

			bool shouldRefreshScores = false;
			if(relationshipScoreStateModel.GetScores(levelConfigId) == null)
			{
				Debug.Log ("SocialSync.RefreshRelationsScores - No scores found for level");
				shouldRefreshScores = true;
			}

			if(DateTime.UtcNow > lastTime.AddHours(generalParameterConfigModel.GetFloat(RELATIONS_SCORES_REQUEST_INTERVAL, 1.0f)))
			{
				Debug.Log ("SocialSync.RefreshRelationsScores - Relations score request interval time passed");
				shouldRefreshScores = true;
			}

			if(GsdkSettingsData.Instance.SocialScoreRefreshCoolDownEnabled == false)
			{
				Debug.Log ("SocialSync.RefreshRelationsScores - Cooldown is disabled");
				shouldRefreshScores = true;

			}

			if(shouldRefreshScores)
			{
				Debug.Log ("SocialSync.RefreshRelationsScores - Attempting to refresh friends scores");
				_relationShipScoreResponseHandler = new RelationshipsScoresResponseHandler(this, levelConfigId);
				_coroutineFactory.StartCoroutine (() => _connectionHandler.SendRequest (ConnectionHandler.RequestType.GetRelationshipsScores, _relationShipScoreResponseHandler.HandleGetRelationshipsScoresResponse, levelConfigId));
			}
			else
			{
				Debug.Log ("SocialSync.RefreshRelationsScores - Skipping scores refresh...");
			}
		}
		
		private class RelationshipsScoresResponseHandler
		{
			private SocialSync outer;
			private string levelConfigId;
			
			public RelationshipsScoresResponseHandler (SocialSync outer, string levelConfigId)
			{
				this.levelConfigId = levelConfigId;
				this.outer = outer;
			}
			
			public void HandleGetRelationshipsScoresResponse (ConnectionHandler.RequestResult result, string response)
			{
				PlayerPrefs.SetString(RELATIONS_SCORES_KEY_PREFIX + levelConfigId, DateTime.UtcNow.ToFileTimeUtc().ToString());
				PlayerPrefs.Save();
				
				if(result != ConnectionHandler.RequestResult.Ok)
				{
					Debug.Log("error getting relationships scores");
					return;
				}

				outer.logger.Log(Tag, "HandleGetRelationshipsScoresResponse (result=" + result + ")\nresponse=" + response.SSubstring(200));

				JsonData responseData = JsonMapper.ToObject (response);
				if(responseData.IsArray)
				{
					HashSet<string> levels = new HashSet<string>();
					string table = outer.relationshipScoreStateModel.GetTableName();

					if(responseData.Count > 0)
					{
						for(int i=0; i < responseData.Count; ++i)
						{
							JsonData data = (JsonData)responseData [i];
							string id = data ["id"].ToString ();
							Debug.Log ("Saving sharedItem: table=" + table + " id=" + id + " data=" + data.ToJson ().SSubstring (200));
							outer.gameDB.SaveSharedState (table, id, data.ToJson ());
							levels.Add(id);
						}

						List<string> affectedTableNames = new List<string>();
						affectedTableNames.Add (outer.relationshipScoreStateModel.GetTableName());
						outer.syncSharedStateSignal.Dispatch(affectedTableNames);

						outer.relationshipsScoresUpdateSignal.Dispatch(levels); //FIXME: for correctness better move to the model, the model should dispatch it
					}
				}
				else
				{
					Debug.LogWarning("relationships scores response bad format A");
				}
			}
		}
	}

	public class ConnectionAllowUserToSwitchUserSignal: Signal<NewSocialPlayerData>
	{
		
	}
	
	public class ConnectionAllowUserToStartNewGameSignal: Signal<NewSocialPlayerData>
	{
		
	}

	public class SocialSyncStartedSignal : Signal
	{
		
	}

	public class SocialSyncCompletedSignal: Signal
	{

	}

}
