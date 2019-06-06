using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using strange.extensions.signal.impl;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;


namespace TabTale
{
	// Used for building json of updates from client to server of state and shared
	public class SyncUpdates
	{
		public Dictionary<string, string> playerData;
		public Dictionary<string, List<string>> listStateData;
	}

	public class ModelSyncService : ISyncService
	{
		[Inject]
		public ModelSyncStartSignal modelSyncStartSignal{get;set;}
		
		[Inject]
		public SyncStatesSignal syncStatesSignal{get;set;}

		[Inject]
		public SyncConfigsSignal syncConfigsSignal{get;set;}
		
		[Inject]
		public SyncSharedStateSignal syncSharedStateSignal{get;set;}

		[Inject]
		public ModelSyncCompletedSignal modelSyncCompletedSignal { get; set; }

		[Inject]
		public ConnectionDisconnectedSignal connectionDisconnectedSignal { get; set; }

		[Inject]
		public IGameDB _gameDB {get;set;}

		[Inject]
		public ServerTime _serverTime {get;set;}

		[Inject]
		public ILogger logger {get;set;}

		private string Tag { get { return "ModelSync".Colored(Colors.teal).Bold(); } }

		private ICoroutineFactory _coroutineFactory;
		private ConnectionHandler _connectionHandler;
		public string keyPlayerId = "playerId";
		private string keyLastStateUpdate = "lastStateUpdate";
		private string keyLastConfigUpdate = "lastConfigUpdate";
		private string keyLastSharedStateUpdate = "lastSharedStateUpdate";
		private string keyToken = "token";
		private string keyMaxProgress = "maxProgress";
		private string keyRunFullSync = "runFullSync";
		private string keyPlayerData = "playerData";
		private string keyGameData = "gameData";
		private string keySharedStateData = "sharedData";
		private string keyListStateData = "listStateData";
		private string keyError = "error";
		private string keyPlayerCreateDate = "createdDate";
		private string keyServerTime = "serverTime";

		private bool _dataSyncEnabled = true;
		private bool _keepRunning = true;
		private bool _loggedIn = false;

		private IPromise promise = new Promise();

		public enum SyncLoopState { LoggingIn, GettingSync, Connected, DisconnectedOnHold };

		public enum FlowDisconnectReason { Undefined, NoInternet, CantReachServer, ServerInternalError, ExpiredToken, SwitchUser, ClientUpdateStarted };

		private enum SyncType {	Continuous, OnDemand, Off}

		public SyncLoopState _syncState = SyncLoopState.LoggingIn;
		public FlowDisconnectReason _flowDisconnectReason = FlowDisconnectReason.Undefined;
		IGeneralDialogService _generalDialog;


		public HashSet<string> _typesToConvert = new HashSet<string>();
		public HashSet<string> _fieldsToEncode = new HashSet<string>();

		private SyncType syncType;

		public float syncLoopLength = 5.0f;

		private const string SYNC_TYPE_KEY = "syncType";
		private const string SYNC_LOOP_KEY = "syncLoopLength";
		private const string SYNC_ENABLED_KEY = "syncEnabled";

		private bool initialized = false;
		private bool _timedOut = false;
		private bool _editorMode = false;

		public bool IsTimeOut()
		{
			return _timedOut;
		}

		public ModelSyncService ()
		{
		}

		[PostConstruct]
		public void Init()
		{
			_connectionHandler = new ConnectionHandler();
			_coroutineFactory = GameApplication.Instance.CoroutineFactory;
			_connectionHandler.Init(_gameDB,_coroutineFactory);

			_typesToConvert = new System.Collections.Generic.HashSet<string> {"fan_bonus_config"};

			initialized = true;
		}

		public void Init (bool editorMode)
		{
			_editorMode = editorMode;
		}

		public IPromise TryConnectToServer()
		{
			if (!initialized) 
			{
				promise.Dispatch();
				return promise;
			}

			// Avoid running model sync when uploading gld
			if(IsUploadScene())
			{
				logger.Log (Tag,"Running in upload mode - skipping connection with the server...");
				promise.Dispatch();
				return promise;
			}


			if(Application.isEditor && ! GsdkSettingsData.Instance.IsConnectionEnabled) 
			{ 
				logger.Log (Tag,"Editor Connection is disabled in gsdk settings - will not fetch GLD from the server");
				promise.Dispatch();
				return promise;
			}

			syncType = SyncType.OnDemand;

			//Load configs
			Data.DataElement connConfig = GameApplication.GetConfiguration("connection");
			string sType = (connConfig.IsNull || !connConfig.ContainsKey(SYNC_TYPE_KEY)) ? "Continuous" : (string)connConfig[SYNC_TYPE_KEY];
			syncLoopLength =  (connConfig.IsNull || !connConfig.ContainsKey(SYNC_LOOP_KEY)) ? syncLoopLength : (float)connConfig[SYNC_LOOP_KEY];
			bool sEnabled = (connConfig.IsNull || !connConfig.ContainsKey(SYNC_ENABLED_KEY)) ? true : (bool)connConfig[SYNC_ENABLED_KEY];

			if(!sEnabled)
			{
				_dataSyncEnabled = false;
			}

			try 
			{
				syncType = (SyncType)Enum.Parse(typeof(SyncType),sType,true);	
			} catch (Exception ex) 
			{
				logger.LogError(Tag,string.Format("Wrong configuraton in connection config file, the {0} key is not of the type SyncType \n Message received:{1}",SYNC_TYPE_KEY,ex.Message));
			}

			if(syncType != SyncType.Off)
			{
				_coroutineFactory.StartCoroutine(() => WaitForState(SyncLoopState.Connected));
			}

			switch (syncType) 
			{
				case SyncType.OnDemand:
					_coroutineFactory.StartCoroutine(() => StartOnDemandMode(false));
					break;
				case SyncType.Continuous:
					_coroutineFactory.StartCoroutine(AsyncLoop);
					break;
				case SyncType.Off:
					promise.Dispatch();
					break;
				default:
					break;
			}

			return promise;
		}
		
		public IEnumerator WaitForState (SyncLoopState state)
		{
			float waitInterval = 1.0f;
			float timeOut = syncLoopLength;
			logger.Log(Tag, "Waiting for state=" + state + " (current=" + _syncState + ")");
			float secs = 0f;
			while (_syncState != state && secs < syncLoopLength) 
			{
				yield return new WaitForSeconds (waitInterval);
				secs+= waitInterval;
			}
			logger.Log(Tag, String.Format("ModelSyncService state: {0} Waited for state={1} {2} seconds.", _syncState, state, secs));

			// Finished waiting without reaching state
			if(_syncState != state)
			{
				_timedOut = true;
				logger.Log(Tag, "ModelSync - Timing out...");
				promise.Dispatch();
				modelSyncCompletedSignal.Dispatch();
			}
		}

		public IEnumerator WaitWhileInState (float secs=1.0f, SyncLoopState state=SyncLoopState.Connected)
		{
			_timedOut = false;
			float remainingSecs = secs;
			while (remainingSecs > 0f) {
				float waitNow = remainingSecs > 1f ? 1f : remainingSecs;
				remainingSecs -= waitNow;
				yield return new WaitForSeconds (waitNow);
				if (_syncState != state)
					break;
			}
		}


		[Obsolete("Please use the Start method and configure the mode in the connection file")]
		public IEnumerator AsyncLoop ()
		{
			while (_keepRunning) {
				switch (_syncState) {
				case SyncLoopState.LoggingIn:
					//yield return _mono.StartCoroutine (TryGetVersion ());
					yield return _coroutineFactory.StartCoroutine (TryLogin);
					break;
				case SyncLoopState.GettingSync:
					yield return _coroutineFactory.StartCoroutine (TryGetPlayerData);
					break;
				case SyncLoopState.Connected:
					yield return _coroutineFactory.StartCoroutine (() => WaitWhileInState (5.0f, SyncLoopState.Connected));
					yield return _coroutineFactory.StartCoroutine (TrySyncData);
					//yield return _mono.StartCoroutine (TrySyncPlayerState ());
					break;
				case SyncLoopState.DisconnectedOnHold:
					yield return _coroutineFactory.StartCoroutine (() => WaitWhileInState (1.0f, SyncLoopState.DisconnectedOnHold));
					break;
				}
			}
		}

		public void SwitchUserAndRestartFlow (string newPlayerId)
		{
			_syncState = SyncLoopState.LoggingIn;
			_gameDB.SaveSyncInfo (keyToken, "");

			TTPlayerPrefs.SetValue("SwitchPlayerInProgress", true);
			TTPlayerPrefs.SetValue(keyPlayerId, newPlayerId);
			//_gameDB.SaveSyncInfo(keyPlayerId, newPlayerId);
			_gameDB.SaveSyncInfo(keyLastStateUpdate, "2015-01-01T12:00:00Z");
			_gameDB.SaveSyncInfo(keyLastSharedStateUpdate, "2015-01-01T12:00:00Z");
			_timedOut = false;

			if(modelSyncStartSignal!=null)
				modelSyncStartSignal.Dispatch();

			_coroutineFactory.StartCoroutine(() => StartOnDemandMode(true));
		}

		public void RestartFlow(bool sync)
		{
			_syncState = SyncLoopState.GettingSync;

			if(modelSyncStartSignal!=null)
				modelSyncStartSignal.Dispatch();
			
			_coroutineFactory.StartCoroutine(() => StartOnDemandMode(sync));
		}

		private IEnumerator StartOnDemandMode (bool sync)
		{
			_timedOut = false;
			while (_syncState != SyncLoopState.Connected && _syncState != SyncLoopState.DisconnectedOnHold && !_timedOut) 
			{
				switch (_syncState) 
				{
					case SyncLoopState.LoggingIn:
						//yield return _mono.StartCoroutine (TryGetVersion ());
						yield return _coroutineFactory.StartCoroutine (TryLogin);
						break;
					case SyncLoopState.GettingSync:
						yield return _coroutineFactory.StartCoroutine (TryGetPlayerData);
						break;
				}
			}
			if (sync) 
			{
				if(_syncState == SyncLoopState.Connected)
					yield return _coroutineFactory.StartCoroutine (TrySyncData);
			}
		}

		public void RestartFlowAfterUserSwitch ()
		{
			_syncState = SyncLoopState.LoggingIn;

			if(modelSyncStartSignal!=null)
				modelSyncStartSignal.Dispatch();

		}
		public void RestartFlowAfterBackground ()
		{
			_syncState = SyncLoopState.LoggingIn;

			if(modelSyncStartSignal!=null)
				modelSyncStartSignal.Dispatch();

		}

		public void ResumeConnectionAfterHold ()
		{
			_syncState = SyncLoopState.LoggingIn;

			if(modelSyncStartSignal!=null)
				modelSyncStartSignal.Dispatch();

		}

		public void SyncData()
		{
			if (syncType != SyncType.OnDemand) 
			{
				logger.LogWarning(Tag, "SyncData will only work when syncType is OnDemand, sync type is now "+syncType.ToString());
				return;
			}

			if (!_dataSyncEnabled)
			{
				logger.LogWarning(Tag, "DataSync is disabled, will not attempt to sync player state");
				return;
			}
				

			_timedOut = false;

			if (_syncState == SyncLoopState.Connected) 
			{
				_coroutineFactory.StartCoroutine(TrySyncData);
			}
			else if (!_loggedIn)// If the login process wasn't completed, run it again.
			{
				_syncState = SyncLoopState.LoggingIn;
				_coroutineFactory.StartCoroutine(() => StartOnDemandMode(true));
			}
			else{
				logger.LogWarning(Tag, "Sync Data was called before sync state is ready, listen to the ModelSyncCompletedSignal to know when it is safe to sync data. Current Sync State:" + _syncState);
			}
		}

		//This is for editor scripts.
		public IEnumerator UploadConfig(IConfigData data)
		{
			if(!IsUploadScene() || !Application.isEditor)
			{
				logger.LogError (Tag, "Uploading is only allowed from upload scene & in unity editor");
				yield break;
			}


			JsonData jsonData = JsonMapper.ToObject(JsonMapper.ToJson(data,true));
			jsonData = PreSend(jsonData,data.GetId(),data.GetTableName(), IsBlobObject(data));
			jsonData["id"] = data.GetId();
			JsonData jd = new JsonData();
			jd.Add(jsonData);

//			Debug.Log(jd.ToJson());

			string tableName = data.GetTableName();

			tableName = AdjustTableToServerTableName(tableName, data);

			
			yield return _coroutineFactory.StartCoroutine (() => _connectionHandler.SendRequest (ConnectionHandler.RequestType.UploadConfig, HandleUploadConfigRepsonse,jd.ToJson(),null,tableName));
		}

		private string AdjustTableToServerTableName(string tableName, IConfigData data)
		{
			// Server table names are of different format due to historical reasons
			try
			{
				var type = data.GetType();
				MethodInfo methodInfo = type.GetMethod("GetServerTableName");
				if(methodInfo != null)
				{
					return (string)methodInfo.Invoke(data, null);
				}
				else
				{
					tableName = tableName.Replace("_Config","");
					tableName = tableName.Replace("_config","");
					tableName = Regex.Replace(tableName, "_[a-z]", m => m.ToString().Replace("_","").ToUpper());
					tableName = FirstCharToUpper(tableName);
				}
			}
			catch(AmbiguousMatchException)
			{
				// ambiguous means there is more than one result,
				// which means: a method with that name does exist
				return tableName;
			}



			return tableName;
		}

		public static string FirstCharToUpper(string input)
		{
			if (String.IsNullOrEmpty(input))
				throw new ArgumentException("Error - empty or null input as table name");
			return input.First().ToString().ToUpper() + input.Substring(1);
		}

		private bool IsUploadScene()
		{
			return Application.loadedLevelName.Contains("Upload");
		}

		#region Sync activation functions
		
		private IEnumerator TryGetVersion ()
		{
			logger.Log(Tag, "TryGetVersion");
			_gameDB.SaveSyncInfo (keyToken, "");
			yield return _coroutineFactory.StartCoroutine (() => _connectionHandler.SendRequest (ConnectionHandler.RequestType.GetVersion, HandleGetVersionResponse));
		}

		private IEnumerator TryLogin ()
		{
			logger.Log(Tag, "TryLogin");

			_gameDB.SaveSyncInfo (keyToken, "");
			yield return _coroutineFactory.StartCoroutine (() => _connectionHandler.SendRequest (ConnectionHandler.RequestType.Login, HandleLoginResponse));
		}

		private IEnumerator TryGetPlayerData ()
		{
			logger.Log(Tag, "TryGetPlayerData");
			yield return _coroutineFactory.StartCoroutine (() => _connectionHandler.SendRequest (ConnectionHandler.RequestType.GetData, HandleGetdataResponse));
		}
		
		/*private IEnumerator TrySyncPlayerState ()
		{
			//Debug.Log("SyncServer()");
			Dictionary<string, string> outdatedStates = new Dictionary<string, string> ();
			outdatedStates.Clear ();
			_gameDB.GetOutdatedData (ref outdatedStates);
			if (outdatedStates.Count == 0) {
				//Debug.Log ("No outdated states in db - sync not required");
				yield break;
			}
			Debug.Log ("SyncServer: Found " + outdatedStates.Count + " outdated states, syncing to server");
			
			JsonData statesJson = new JsonData ();
			statesJson.SetJsonType (JsonType.Object);
			foreach (string key in outdatedStates.Keys) {
				JsonData state = JsonMapper.ToObject (outdatedStates [key]);
				statesJson [key] = state;
			}
			
			string data = statesJson.ToJson ();
			Debug.Log ("Outdated stats for sync:\n" + statesJson.ToJson ());
			
			yield return _mono.StartCoroutine (_connectionHandler.SendRequest (ConnectionHandler.RequestType.SyncData, HandleSaveStateResponse, data));
		}*/

		public IEnumerator TrySyncData()
		{
			SyncUpdates updates = _gameDB.GetOutdatedData();
			//string data = JsonMapper.ToJson(updates);
			//Logger.LogDebug(s_loggerModule, "TrySyncData updates:\n" + data);
			
			JsonData data = new JsonData();
			data.SetJsonType(JsonType.Object);

			// Sync of State Table
			JsonData statesJson = new JsonData ();
			statesJson.SetJsonType (JsonType.Object);
			foreach (string key in updates.playerData.Keys) {
				JsonData state = JsonMapper.ToObject (updates.playerData [key]);
				statesJson [key] = state;
			}
			data[keyPlayerData] = statesJson;

			// Sync of ListState Tables
			if (updates.listStateData.Count > 0)
			{
				JsonData listStateJson = new JsonData ();
				listStateJson.SetJsonType(JsonType.Object);
				foreach (string key in updates.listStateData.Keys)
				{
					List<string> statesList = updates.listStateData[key];
					JsonData listJson = new JsonData ();
					listJson.SetJsonType(JsonType.Array);
					foreach (string stateItem in statesList)
					{
						JsonData state = JsonMapper.ToObject (stateItem);
						listJson.Add(state);
					}
					listStateJson[key] = listJson;
				}
				data[keyListStateData] = listStateJson;
			}
			
			string data2 = data.ToJson();
			logger.Log(Tag, "TrySyncData updates:\n" + data2);
			yield return _coroutineFactory.StartCoroutine (() => _connectionHandler.SendRequest (ConnectionHandler.RequestType.SyncData, HandleSyncStateResponse, data2));
		}

		#endregion


		//These methods provide a wrapper to the normal json serialisation process, if a json is too complex, it can simply wrap it as a blob in base64. 

		#region CustomSyncMethods

		private JsonData PreSend(JsonData jsonData,string key, string type = "", bool isBlob = false)
		{
			//if data should be a blob, convert the JSON to a base64 encoded string, and put it in the Base64Data class
			//and return that class.
			//the optional param type is for when the key is the id, and you want to convert all values of certain type (same db table)
			//so instead of adding all the values you add the table to the _typesToConvert
			//{"data": <BASE64 ENCODED DATA>}
			if (isBlob) 
			{
					var dataWrapper = new JsonData();
					string encoded = Base64Encode(jsonData.ToJson());
					dataWrapper["data"] = encoded;
					return dataWrapper;
			}

			if (jsonData.IsObject) 
			{	
				List<string> keys = new List<string>(jsonData.Keys);
				foreach (var k in keys) 
				{
					if (_fieldsToEncode.Contains(k)) 
					{
						if (jsonData[k] != null) 
						{
							string newVal = "";
							newVal = Base64Encode(jsonData[k].ToJson());
							jsonData[k].SetString(newVal);
						}
						else 
						{
							jsonData[k] = new JsonData("");
						}
					}
				}
			}

			return jsonData;

		}

		//this will unwrap a json wrapped in the Base64Data wrapper.
		private JsonData PostReceive(JsonData jsonData,string key, string type ="")
		{
			if(IsBlobData(jsonData)) 
			{
				//decode the "data" part of the wrapper to get the actual json text.
				string decoded = Base64Decode(jsonData["data"].ToString());
				var unwrappedData =  JsonMapper.ToObject(decoded);

				return unwrappedData;
			}

			List<string> keys = new List<string>(jsonData.Keys);
			foreach (var k in keys) {
				if (_fieldsToEncode.Contains(k)) {
					jsonData[k] = JsonMapper.ToObject(Base64Decode(jsonData[k].ToString()));
				}
			}
			
			return jsonData;
		}

		// Avoiding reflection for improved performance
		private bool IsBlobData(JsonData jsonData)
		{ 
			return (jsonData.Keys.Count <= 3  && jsonData.Keys.Contains("data")); // Means it's a base64 class
		}

		private bool IsBlobObject(IConfigData data)
		{
			try
			{
				var type = data.GetType();
				MethodInfo methodInfo = type.GetMethod("IsBlob");

				if(methodInfo == null)
					return false;

				return (bool)methodInfo.Invoke(data, null);
			}
			catch(AmbiguousMatchException)
			{
				// ambiguous means there is more than one result,
				// which means: a method with that name does exist
				return true;
			}
		}

		public static string Base64Decode(string base64EncodedData) {
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}

		public static string Base64Encode(string plainText) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}


		#endregion

		#region Sync response functions

		public void HandleUploadConfigRepsonse(ConnectionHandler.RequestResult result, string response)
		{
			if (result != ConnectionHandler.RequestResult.Ok) {
				logger.LogError(Tag, "Error response: "+result+" response: "+response);
				HandleErrorResponse (result, response);
				return;
			}

			logger.Log(Tag, "HandleUploadConfigRepsonse (result=" + result + ")\nresponse=" + response.SSubstring(200));

		}

		public void HandleErrorResponse (ConnectionHandler.RequestResult result, string response)
		{
			logger.Log(Tag.Italics(), "ModelSync.HandleErrorResponse (result=" + result + ")\nresponse: " + response.SSubstring(200));

			string title = "";
			string message = "";
			string button = "";
			bool showDialog = false;
			bool recoverUnidentifiedUSer = false;

			switch (result) {
			case (ConnectionHandler.RequestResult.NoInternet):
				_flowDisconnectReason = FlowDisconnectReason.NoInternet;
				title = "Connection Error";
				message = "Unable to connect with server. Check you internet connection and try again";
				button = "Try Again";
				break;
			case (ConnectionHandler.RequestResult.CantReachServer):
				title = "Connection Error";
				message = "Unable to connect with server. Please try again";
				button = "Try Again";
				_flowDisconnectReason = FlowDisconnectReason.CantReachServer;
				break;
			case (ConnectionHandler.RequestResult.InternalServerError):
				_flowDisconnectReason = FlowDisconnectReason.ServerInternalError;
				break;
			case (ConnectionHandler.RequestResult.UnAuthorized):
				title = "Progress Detected";
				message = "You have been playing on another device. Please reload game to load your progress";
				button = "Reload";
				_flowDisconnectReason = FlowDisconnectReason.ExpiredToken;
				break;
			case (ConnectionHandler.RequestResult.OkWithErrorInside): 
				JsonData responseErrorJsonOnject = JsonMapper.ToObject (response);
				JsonData errorObject = (JsonData)responseErrorJsonOnject [keyError];
				ServerErrorData error = JsonMapper.ToObject<ServerErrorData> (errorObject.ToJson ());
				logger.Log(Tag, "Handling server error message: " + error.ToString ());
				if (error.code == (int)ConnectionHandler.RequestResult.UnAuthorized) {
					_flowDisconnectReason = FlowDisconnectReason.ExpiredToken;
					title = "Progress Detected";
					message = "You have been playing on another device. Please reload game to load your progress";
					button = "Reload";
					break;
				}
				if (error.code == (int)ConnectionHandler.RequestResult.ClientUpdateRequired) {
					_flowDisconnectReason = FlowDisconnectReason.ClientUpdateStarted;
					title = error.title;
					message = "You must update your game - update initiated (url=" + error.message + ").";
					button = "";
					break;
				}
				
				if (error.code == (int)ConnectionHandler.RequestResult.InternalServerError
				    && error.message == "User not foundPlayer not found")
				{
					
					logger.Log(Tag, "ModelSync.HandleErrorResponse() - PlayerId not found on server => staring new game");

					recoverUnidentifiedUSer = true;
					//_gameDB.SaveSyncInfo(keyPlayerId, "0");
					TTPlayerPrefs.SetValue(keyPlayerId, "0");
					_gameDB.SaveSyncInfo(keyLastStateUpdate, "");
					RestartFlowAfterUserSwitch();
				}

				title = "Unidentified Server Error";
				message = "error " + error.ToString ();
				button = "reload";
				break;
			}

			if (!recoverUnidentifiedUSer)
				_syncState = SyncLoopState.DisconnectedOnHold;

			if(connectionDisconnectedSignal!=null)
				connectionDisconnectedSignal.Dispatch(_flowDisconnectReason);

			if (showDialog)
				ShowErrorDialog (title, message, button);
		}

		public void ShowErrorDialog (string title, string message, string button)
		{
			GeneralDialogData data = new GeneralDialogData ();
			data.title = title;
			data.message = message;
			data.hasCloseButton = false;
			if (button.Length > 0)
				data.buttons.Add (new DialogButtonData (button, ResumeConnectionAfterHold));
			_generalDialog.Show (data);
		}
		
		private void HandleGetVersionResponse (ConnectionHandler.RequestResult result, string response)
		{
			logger.Log (Tag, "HandleGetVersionResponse (result=" + result + ")\nresponse=" + response.SSubstring(200));
			if (result != ConnectionHandler.RequestResult.Ok) {
				HandleErrorResponse (result, response);
				return;
			}

			logger.Log (Tag,"HandleGetVersionResponse - no update required");
		}

		private void HandleLoginResponse (ConnectionHandler.RequestResult result, string response)
		{
			if(_timedOut)
			{
				logger.LogWarning (Tag, "HandleGetdataResponse - ModelSyncCompleted timed out");
				return;
			}

			logger.Log (Tag,"HandleLoginResponse (result=" + result + ")\nresponse=" + response.SSubstring(300));
			if (result != ConnectionHandler.RequestResult.Ok) {
				HandleErrorResponse (result, response);
				return;
			}

			JsonData responseJsonObject = JsonMapper.ToObject (response);
			
			string playerId = ((long)responseJsonObject [keyPlayerId]).ToString ();
			logger.Log (Tag,"Got playerId:" + playerId);

			bool isNewPlayer = TTPlayerPrefs.GetValue(keyPlayerId, "0") == "0";
			
			TTPlayerPrefs.SetValue(keyPlayerId, playerId);

			//_gameDB.SaveSyncInfo (keyPlayerId, ((long)responseJsonObject [keyPlayerId]).ToString ());
			logger.Log (Tag,"Got token:" + (string)responseJsonObject [keyToken]);
			_gameDB.SaveSyncInfo (keyToken, (string)responseJsonObject [keyToken]);
			
			_syncState = SyncLoopState.GettingSync;
		}

		private void UpdateSyncInfo(JsonData responseObject)
		{
			if (responseObject.Keys.Contains (keyLastStateUpdate))
			{
				logger.Log (Tag,"Got lastStateUpdate:" + (string)responseObject [keyLastStateUpdate]);
				_gameDB.SaveSyncInfo (keyLastStateUpdate, (string)responseObject [keyLastStateUpdate]);
			}
			if (responseObject.Keys.Contains (keyLastConfigUpdate))
			{
				logger.Log (Tag,"Got lastConfigUpdate:" + (string)responseObject [keyLastConfigUpdate]);
				_gameDB.SaveSyncInfo (keyLastConfigUpdate, (string)responseObject [keyLastConfigUpdate]);
				
				if (_serverTime == null)
					logger.Log (Tag,"_serverTime is null");
				_serverTime.Set((string)responseObject [keyLastConfigUpdate]);
			}
			if (responseObject.Keys.Contains (keyLastSharedStateUpdate))
			{
				logger.Log (Tag,"Got lastSharedStateUpdate:" + (string)responseObject [keyLastSharedStateUpdate]);
				_gameDB.SaveSyncInfo (keyLastSharedStateUpdate, (string)responseObject [keyLastSharedStateUpdate]);
			}

			if (responseObject.Keys.Contains (keyPlayerCreateDate))
			{
				logger.Log (Tag,"Got createdDate:" + (string)responseObject [keyPlayerCreateDate]);
				_gameDB.SaveSyncInfo (keyPlayerCreateDate, (string)responseObject [keyPlayerCreateDate]);
			}

			if (responseObject.Keys.Contains (keyMaxProgress))
			{
				int remoteMaxProgress = (int)responseObject [keyMaxProgress];
				
				logger.Log (Tag,"ModelSync - got MaxProgress:" + remoteMaxProgress);
				
				if(remoteMaxProgress > int.Parse(_gameDB.LoadSyncInfo(keyMaxProgress)))
				{
					_gameDB.SaveSyncInfo (keyMaxProgress, (string)responseObject [keyMaxProgress]);
				}
				else
				{
					logger.Log (Tag,"ModelSync - got a lower remote maxProgress, ignoring it");
				}
			}
		}

		private void HandleSharedStateResponse(JsonData sharedItems, ICollection<string> affectedSharedStateTables)
		{
			foreach (string sharedItem in sharedItems.Keys) { 
				logger.Log (Tag,"Got sharedItem updates for sharedItem: " + sharedItem);
				JsonData sharedItemUpdates = (JsonData)sharedItems [sharedItem];
				if (sharedItemUpdates == null)
					break;
				int updatesCount = sharedItemUpdates.Count;
				for (int index = 0; index<updatesCount; index++) {
					SaveSharedState(sharedItem, sharedItemUpdates, index);
					affectedSharedStateTables.Add(sharedItem);
				}
			}
		}

		private void SaveSharedState (string table, JsonData sharedItemUpdates, int index)
		{
			JsonData data = (JsonData)sharedItemUpdates [index];
			string id = data ["id"].ToString ();
			logger.Log (Tag,"Saving sharedItem: table=" + table + " id=" + id + " data=" + data.ToJson ().SSubstring (200));
			_gameDB.SaveSharedState (table, id, data.ToJson ());
		}

		private void HandleGetdataResponse (ConnectionHandler.RequestResult result, string response)
		{
			if(CheckSyncTimeout())
				return;

			logger.Log (Tag,"HandleGetdataResponse (result=" + result + ")\nresponse=" + response.SSubstring(200));
			if (result != ConnectionHandler.RequestResult.Ok) {
				HandleErrorResponse (result, response);
				return;
			}
			
			JsonData responseJsonOnject = JsonMapper.ToObject (response);

			if( ! _editorMode )
				UpdateSyncInfo(responseJsonOnject);

			ICollection<string> affectedConfigTables = new List<string>();
			ICollection<string> affectedSharedStateTables = new List<string>();
			ICollection<string> affectedStates = new List<string>();


			if (responseJsonOnject.Keys.Contains (keyGameData)) {
				//Debug.Log ("Got player data update " + responseJsonOnject[key].GetType());
				JsonData configs = (JsonData)responseJsonOnject [keyGameData];
				foreach (string config in configs.Keys) { 
					logger.Log (Tag,"Got config updates for config: " + config);
					//TODO: Add the pre and post send here too.
					JsonData configUpdates = (JsonData)configs [config];
					int updatesCount = configUpdates.Count;
					for (int index = 0; index<updatesCount; index++) {
						JsonData data = (JsonData)configUpdates [index];
						string id = data ["id"].ToString ();

						data = PostReceive(data,id,config);

						logger.Log (Tag,"Saving config: table=" + config + " id=" + id +" data=" + data.ToJson ().SSubstring(200)); 
						_gameDB.SaveConfig (config, id, data.ToJson ());
						affectedConfigTables.Add(config);
					}
				}
			}

			// Running in editor mode would update only config tables - for syncing only the gld form the server
			if(_editorMode)
				return;

			if (responseJsonOnject.Keys.Contains (keySharedStateData)) {
				//Debug.Log ("Got shared data update " + responseJsonOnject[key].GetType());
				JsonData sharedItems = (JsonData)responseJsonOnject [keySharedStateData];
				HandleSharedStateResponse(sharedItems, affectedSharedStateTables);
			}
			if (responseJsonOnject.Keys.Contains (keyListStateData)) {
				//Debug.Log ("Got liststate data update " + responseJsonOnject[key].GetType());
				JsonData stateItems = (JsonData)responseJsonOnject [keyListStateData];
				HandleSharedStateResponse(stateItems, affectedSharedStateTables);
			}
			if (responseJsonOnject.Keys.Contains (keyPlayerData)) {
				JsonData states = (JsonData)responseJsonOnject [keyPlayerData];
				foreach (string state in states.Keys) { 
//					Debug.Log ("Got state update: key=" + state + " value=" + ((JsonData)states [state]).ToJson ());
					JsonData decoded = PostReceive(states[state],state);
					string data = decoded.ToJson ();

					if (data.CompareTo ("{}") != 0) {
						_gameDB.SyncState (state, data);
						affectedStates.Add(state);
					}
				}
			}

			if (affectedConfigTables.Count > 0)
				if(syncConfigsSignal!=null)
					syncConfigsSignal.Dispatch(affectedConfigTables);
			
			if (affectedSharedStateTables.Count > 0)
				if(syncSharedStateSignal!=null)
					syncSharedStateSignal.Dispatch(affectedSharedStateTables);

			if (affectedStates.Count > 0)
				if(syncStatesSignal!=null)
					syncStatesSignal.Dispatch(affectedStates);

			_loggedIn = true;
			_syncState = SyncLoopState.Connected;

			if (responseJsonOnject.Keys.Contains (keyRunFullSync))
			{
				bool shouldRunFullSync = (bool)responseJsonOnject[keyRunFullSync];
				if(shouldRunFullSync)
				{
					logger.Log (Tag,"ModelSync - Running full sync");
					_gameDB.SetStateDataAsOutdated();
					_coroutineFactory.StartCoroutine (TrySyncData);
				}
			}

			logger.Log (Tag,"Sending event OnModelSyncCompleted");
			if(modelSyncCompletedSignal!=null)
			{
				_timedOut = false;
				modelSyncCompletedSignal.Dispatch();
				promise.Dispatch();
			}
		}

		private void HandleSyncStateResponse (ConnectionHandler.RequestResult result, string response)
		{
			if(CheckSyncTimeout())
				return;

			if (result != ConnectionHandler.RequestResult.Ok)
			{
				HandleErrorResponse(result, response);
				return;
			}
			
			ICollection<string> affectedSharedStateTables = new List<string>();
			JsonData responseJsonOnject = JsonMapper.ToObject (response);
			
			if (responseJsonOnject.Keys.Contains (keyLastStateUpdate))
			{
				logger.Log(Tag,"Got lastStateUpdate:" + (string)responseJsonOnject [keyLastStateUpdate]);
				_gameDB.SaveSyncInfo (keyLastStateUpdate, (string)responseJsonOnject [keyLastStateUpdate]);
			}
			if (responseJsonOnject.Keys.Contains (keyLastSharedStateUpdate))
			{
				logger.Log(Tag,"Got lastSharedStateUpdate:" + (string)responseJsonOnject [keyLastSharedStateUpdate]);
				_gameDB.SaveSyncInfo (keyLastSharedStateUpdate, (string)responseJsonOnject [keyLastSharedStateUpdate]);
			}
			if (responseJsonOnject.Keys.Contains (keyPlayerCreateDate))
			{
				logger.Log(Tag, "Got createdDate:" + (string)responseJsonOnject [keyPlayerCreateDate]);
				_gameDB.SaveSyncInfo (keyPlayerCreateDate, (string)responseJsonOnject [keyPlayerCreateDate]);
			}
			if (responseJsonOnject.Keys.Contains (keyMaxProgress))
			{
				logger.Log(Tag,"Got MaxProgress:" + (string)responseJsonOnject [keyMaxProgress]);
				_gameDB.SaveSyncInfo (keyMaxProgress, (string)responseJsonOnject [keyMaxProgress]);
			}
			if (responseJsonOnject.Keys.Contains (keyServerTime))
			{
				_serverTime.Set((string)responseJsonOnject [keyServerTime]);
			}
			if (responseJsonOnject.Keys.Contains (keyListStateData)) 
			{
				//Debug.Log ("Got shared data update " + responseJsonOnject[key].GetType());
				JsonData sharedItems = (JsonData)responseJsonOnject [keyListStateData];
				HandleSharedStateResponse(sharedItems, affectedSharedStateTables);
			}
			
			if (affectedSharedStateTables.Count > 0)
				if(syncSharedStateSignal!=null)
					syncSharedStateSignal.Dispatch(affectedSharedStateTables);

			_gameDB.SetSyncStatus (SyncStatus.InProgress, SyncStatus.Updated);
		}

		private bool CheckSyncTimeout()
		{
			if(_timedOut)
			{
				logger.LogWarning (Tag, "ModelSync - Stopping sync flow due to time out");
				return true;
			}
			else
				return false;
		}

		#region Server Time
		public void GetServerTime()
		{
			_coroutineFactory.StartCoroutine(TryGetServerTime);
		}

		private IEnumerator TryGetServerTime()
		{
			yield return _coroutineFactory.StartCoroutine (() => _connectionHandler.SendRequest (ConnectionHandler.RequestType.GetServerTime, HandleServerTimeResponse));
		}

		private void HandleServerTimeResponse(ConnectionHandler.RequestResult result, string response)
		{
			if (result != ConnectionHandler.RequestResult.Ok) 
			{
				HandleErrorResponse (result, response);
				return;
			}

			JsonData responseJsonObject = JsonMapper.ToObject (response);
			if (responseJsonObject.Keys.Contains (keyServerTime))
			{
				_serverTime.Set((string)responseJsonObject [keyServerTime]);
			}
		}

		#endregion

		public int GetMaxProgress()
		{
			return Int32.Parse(_gameDB.LoadSyncInfo(keyMaxProgress));
		}

		public void ReportMaxProgress(int maxProgress)
		{
			_gameDB.SaveSyncInfo(keyMaxProgress,  maxProgress.ToString());
		}



		#endregion
	}

	public class Base64Data
	{
		public string data;
	}
}
