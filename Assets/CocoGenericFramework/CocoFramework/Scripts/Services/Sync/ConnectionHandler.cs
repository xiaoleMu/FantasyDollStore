using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TabTale.Analytics;
using TabTale.Plugins.PSDK;

namespace TabTale
{
	public class ConnectionHandler 
	{
		private IGameDB _gameDB;

		[Inject]
		public IGameDB gameDB { get; set; }

		[Inject]
		public SocialStateModel socialStateModel { get; set; }

		[Inject]
		public EventStateModel eventStateModel { get; set; }

		protected ICoroutineFactory _coroutineFactory;

		protected int MaxRequestRetries = 3;
		protected float RequestFailWaitTime = 0.5f;

		protected string Tag { get { return "ConnectionHandler".Colored(Colors.green).Bold(); } }

		protected string _url = "https://enter-server-adress.appspot.com/";
		protected string _defaultUrl = "https://enter-server-adress.appspot.com/";
		
		protected const string _getVersionRequest ="version";
		protected const string _loginRequest ="players/login";
		protected const string _savePlayerRequest ="players/";
		protected const string _getPlayerRequest ="players/";
		protected const string _relationshipsScoresRequest = "realtionshipsScore/";
		protected const string _refreshRelationshipsSuffix = "/refreshRelationships";
		protected const string _connectSocialNetworkRequestPrefix ="players/";
		protected const string _connectSocialNetworkRequestSuffix ="/socialstate";
		protected const string _receiptValidationRequest = "https://tt-rvs.tabtale.info/check";
		protected const string _tabtaleReceiptValidationRequest = "https://tabtale-rvs.tabtale.info/check";
		protected const string _getAvatarRequestPrefix ="players/";
		protected const string _getAvatarRequestSuffix ="/avatar";

		protected const string _getItemRequest = "items";
		protected const string _syncItemRequest = "";
		
		protected const string _keyPlayerId = "playerId";
		protected const string _keyLastStateUpdate = "lastStateUpdate";
		protected const string _keyLastConfigUpdate = "lastConfigUpdate";
		protected const string _keyLastSharedUpdate = "lastSharedUpdate";
		protected const string _keyToken = "token";
		protected const string _keyRevision = "revision";
		protected const string _keySocialNetworkType = "type";
		protected const string _keySocialNetworkUserId = "id";
		protected const string _keySocialNetworkToken = "socialToken";
		protected const string _keySocialNetworkPlayerName = "playerName";
		protected const string _keySocialNetworkPlayerEmail = "playerEmail";
		protected const string _keySocialNetworkPlayerImage = "playerImage";
		protected const string _keyClientVersion = "clientVersion";
		protected const string _keyClientConfigVersion = "clientConfigVersion";
		protected const string _configKeyURL = "server";
		protected const string _configKeyClientVersion = "clientVersion";
		protected const string _configKeyClientConfigVersion = "clientConfigVersion";

		protected const string _keyMaxProgress = "maxProgress";
		protected const string _keyPlatform = "platform";

		protected const string _configUpload = "admin/gamedata/";

		protected const string _analyticsReviveReport = "analytics/reviveReport";
		protected const string _analyticsLevelReport = "analytics/levelReport";
		protected const string _analyticsCinemaReport = "analytics/cinemaReport";

		protected UTF8Encoding _encoding = new System.Text.UTF8Encoding ();
		protected byte[] _emptyBody;
		protected string _requestUrl = "";
		protected Dictionary<string,string> _headers = new Dictionary<string,string>();
		protected bool _internetAvailable = true;
		protected RequestResult _requestResult = RequestResult.Ok;
		protected string _requestData = "";
		
		public void Init(IGameDB gameDB,ICoroutineFactory coroutineFactory)
		{
			_gameDB = gameDB;
			_coroutineFactory = coroutineFactory;

			_emptyBody = _encoding.GetBytes(" ");	

			Data.DataElement connConfig = GameApplication.GetConfiguration("connection");
			_url = (connConfig.IsNull || !connConfig.ContainsKey(_configKeyURL)) ? _defaultUrl : (string)connConfig[_configKeyURL];

			// TODO - replace this with real versioning mechanism TBD
			string clientVersion = (connConfig.IsNull || !connConfig.ContainsKey(_configKeyClientVersion)) ? "1.0" : 
				(string)connConfig[_configKeyClientVersion];

			string clientConfigVersion = (connConfig.IsNull || !connConfig.ContainsKey(_configKeyClientConfigVersion)) ? "1.0" : 
				(string)connConfig[_configKeyClientConfigVersion];

			_headers = new Dictionary<string,string>();
			_headers.Add(_keyToken, "");
			_headers.Add (_keyClientVersion, clientVersion);
			_headers.Add (_keyClientConfigVersion, clientConfigVersion);
			_headers.Add("Content-Length", "1");

		}

		[PostConstruct]
		public void Init()
		{
			_coroutineFactory = GameApplication.Instance.CoroutineFactory;
			Init(gameDB, _coroutineFactory);
		}

		public enum RequestType { Login=1, GetData, SyncData, ReceiptValidation, ConnectToSocialNetwork, GetVersion, GetAvatar,UploadConfig, GetRelationshipsScores, PushNotificationData, GetServerTime, RefreshRelationships, EventSystemRegistration, GetTopLeaderboardRequest, GetRankAndNeighboursRequest, GetCountry }
		
		public enum RequestResult { NoInternet=0, OkWithErrorInside=1, CantReachServer=2, ClientUpdateRequired=17, Continue=100, Ok=200, MovedPermanently=301, BadRequest=400, UnAuthorized=401, NotFound=404, InternalServerError=500  } ;
		
		private void BuildGetVersionRequest()
		{
			_requestUrl = String.Format("{0}{1}", _url, _getVersionRequest);
		}
		private void BuildLoginRequest()
		{
			string playerId = TTPlayerPrefs.GetValue(_keyPlayerId, "0");
			//string playerId = _gameDB.LoadSyncInfo(_keyPlayerId);
			string platform = "Other";
#if UNITY_ANDROID
				platform = "Android";
#elif UNITY_IOS
					platform = "iOS";
#endif
			_requestUrl = String.Format("{0}{1}?{2}={3}&{4}={5}",
			                                  _url, _loginRequest, _keyPlayerId, playerId,_keyPlatform,platform);
		}
		private void BuildGetDataRequest()
		{
			string playerId = TTPlayerPrefs.GetValue(_keyPlayerId, "0");
			//string playerId = _gameDB.LoadSyncInfo(_keyPlayerId);
			string lastStateUpdate = _gameDB.LoadSyncInfo(_keyLastStateUpdate);
			string lastConfigUpdate = _gameDB.LoadSyncInfo(_keyLastConfigUpdate);
			string lastSharedUpdate = _gameDB.LoadSyncInfo(_keyLastSharedUpdate);
			string maxProgress = _gameDB.LoadSyncInfo(_keyMaxProgress);

			_requestUrl = String.Format("{0}{1}{2}?{3}={4}&{5}={6}&{7}={8}&{9}={10}",
			                                  _url, _getPlayerRequest, playerId, _keyLastStateUpdate, lastStateUpdate, 
			                            _keyLastConfigUpdate, lastConfigUpdate, _keyLastSharedUpdate, lastSharedUpdate,_keyMaxProgress,maxProgress);
		}
		private void BuildGetRelationshipsScoresRequest(string levelConfigId)
		{
			string playerId = TTPlayerPrefs.GetValue(_keyPlayerId, "0");
			
			_requestUrl = String.Format("{0}{1}{2}/{3}{4}",
			                            _url, _getPlayerRequest, playerId, _relationshipsScoresRequest, levelConfigId);
		}
		private void BuildSyncStateRequest()
		{
			string playerId = TTPlayerPrefs.GetValue(_keyPlayerId, "0");
			//string playerId = _gameDB.LoadSyncInfo(_keyPlayerId);
			string lastStateUpdate = _gameDB.LoadSyncInfo(_keyLastStateUpdate);
			string lastSharedUpdate = _gameDB.LoadSyncInfo(_keyLastSharedUpdate);
			string maxProgress = _gameDB.LoadSyncInfo(_keyMaxProgress);
			
			_requestUrl = String.Format("{0}{1}{2}?{3}={4}&{5}={6}&{7}={8}",
			                                  _url, _savePlayerRequest, playerId,
			                                  _keyLastStateUpdate, lastStateUpdate, _keyLastSharedUpdate, lastSharedUpdate,
			                           			_keyMaxProgress, maxProgress);
		}
		private void BuildReceiptValidationRequest(string data)
		{
			if(PSDKMgr.Instance.BundleIdentifier.Contains("tabtale"))
			{
				_requestUrl = _tabtaleReceiptValidationRequest;
			}
			else
			{
				_requestUrl = _receiptValidationRequest;	
			}

		}
		private void BuildConnectToNetworkRequest(SocialConnectData socialConnectData)
		{
			string playerId = TTPlayerPrefs.GetValue(_keyPlayerId, "0");
			//string playerId = _gameDB.LoadSyncInfo(_keyPlayerId);

			_requestUrl = String.Format("{0}{1}{2}{3}?{4}={5}&{6}={7}&{8}={9}&{10}={11}&{12}={13}&{14}={15}",
			                                  _url, _connectSocialNetworkRequestPrefix, playerId,_connectSocialNetworkRequestSuffix,
			                                  _keySocialNetworkType, socialConnectData.network, _keySocialNetworkUserId, socialConnectData.userId,
			                                  _keySocialNetworkToken, socialConnectData.userToken,
			                            	  _keySocialNetworkPlayerName, WWW.EscapeURL(socialConnectData.userName),
			                            	  _keySocialNetworkPlayerEmail, WWW.EscapeURL(socialConnectData.email),
			                            	  _keySocialNetworkPlayerImage, WWW.EscapeURL(socialConnectData.photoUrl));
		}
		private void BuildGetAvatarRequest(string avatarPlayerId)
		{
			_requestUrl = String.Format("{0}{1}{2}{3}", _url, _getAvatarRequestPrefix, avatarPlayerId,_getAvatarRequestSuffix);
		}

		private void BuildGetPushNotifURLRequest(string token)
		{
			string playerId = TTPlayerPrefs.GetValue(_keyPlayerId, "0");
		
			_requestUrl = String.Format ("{0}{1}{2}{3}={4}", _url, "players", playerId, "pushToken?registerToken" ,token);
		}
		#region event system requests
		private void BuildEventRegistrationRequest(string eventID)
		{
			string playerId = TTPlayerPrefs.GetValue(_keyPlayerId, "0");
			string playerName = WWW.EscapeURL(socialStateModel.GetPlayerName ());

			_requestUrl = String.Format ("{0}{1}/{2}/{3}/{4}{5}={6}", _url, "events", eventID , "players", playerId, "?playerName", playerName);
		}

		//bild event top leaderboard reqiuest
		private void BuildTopLeaderboardRequest(string eventID)
		{
			string ID = eventID;
			int bucketId = eventStateModel.GetPlayerBucket(eventID); 
			_requestUrl = String.Format ("{0}{1}/{2}/{3}/{4}/{5}/{6}", _url, "events", ID, "bucket", bucketId, "leaderboard","top");
		}

		//bild event RankAndNeighbours reqiuest
		private void BuildRankAndNeighboursRequestRequest(string eventID)
		{
			string ID = eventID;
			int bucketId = eventStateModel.GetPlayerBucket(eventID); 
			int score = eventStateModel.GetEventScore(eventID);
			_requestUrl = String.Format ("{0}{1}/{2}/{3}/{4}/{5}/{6}?{7}={8}", _url, "events", ID, "bucket", bucketId, "leaderboard","neighbors","score", score);
		}
		#endregion
			
		private void BuildRefreshRelationshipsRequest()
		{	
			string playerId = TTPlayerPrefs.GetValue(_keyPlayerId, "0");
			
			_requestUrl = String.Format ("{0}{1}/{2}{3}", _url, "players", playerId, _refreshRelationshipsSuffix);
		}

		private void BuildGetServerTimeRequest(){
			_requestUrl = String.Format ("{0}serverTime",_url);
		}


		private void BuildGetCountryRequest()
		{
			_requestUrl = String.Format("{0}{1}",_url, "country");
		}

		private void BuildUploadConfigDataRequest(string tableName)
		{
			_requestUrl = String.Format("{0}{1}{2}", _url, _configUpload, tableName);
		}

		protected void SetHeaders(int bodyLength)
		{
			if ( _gameDB != null)
				_headers[_keyToken] = _gameDB.LoadSyncInfo(_keyToken);
			_headers["Content-Length"] = bodyLength.ToString();
			_headers["Content-Type"] = "application/json";
		}
		
		/*private void PrintHeadersAndStatus(WWW www)
		{
			if (www.responseHeaders == null)
				return;
			foreach(string key in www.responseHeaders.Keys)
			{
				Debug.Log ("Got header: " + key + "=" + www.responseHeaders[key]);
			}
		}*/

		public IEnumerator CheckInternetConnection()
		{
			WWW wwwGoogle = new WWW(NetworkCheck.NetworkCheckUrl);
			yield return wwwGoogle;
			
			_internetAvailable = (wwwGoogle.isDone && wwwGoogle.bytesDownloaded > 0);
			Debug.Log ("Check internet connection result = " + _internetAvailable);
		}
		protected IEnumerator EvaluateRequestStatus (WWW www)
		{
			if (www == null)
			{
				Debug.Log("www is null. Handling as CantReachServer.");
				_requestResult = RequestResult.CantReachServer;
				_requestData = "";
				yield break;
			}

			string statusHeaderKey = "STATUS";
			string ttStatusHeaderKey = "TTSTATUS";
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				statusHeaderKey = "NULL";
				break;
			case RuntimePlatform.OSXEditor:
				statusHeaderKey = "STATUS";
				break;
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.IPhonePlayer:
				statusHeaderKey = "STATUS";
				break;
			default:
				statusHeaderKey = "";
				break;
			}
			
			if (www.error != null)
			{
				if (www.isDone && www.bytesDownloaded == 0)
				{
					yield return _coroutineFactory.StartCoroutine(() => CheckInternetConnection());
					_requestData = www.error;
					if (!_internetAvailable)
					{
						Debug.Log ("Request result - no internet");
						_requestResult = RequestResult.NoInternet;
						yield break;
					}
					Debug.Log ("Request result - can't reach server");
					_requestResult = RequestResult.CantReachServer;
					yield break;
				}

				//Debug.Log ("WWW error = [" + www.error + "]\nText=" + www.text);
				_requestResult = RequestResult.InternalServerError;
				yield break;
			}
			
			_requestData = www.text;
			//Debug.Log ("WWW isDone = " + www.isDone + " bytesDownloaded=" + www.bytesDownloaded);
			//Debug.Log ("Checking for status key = " + statusHeaderKey + "\ntext=" + (www.text == null ? "null" : www.text)  
			//           + "\nfielderrorvalue=" + (www.error == null ? "null" : www.error) );

			if (www.text.Contains("{\"error\":{"))
			{
				_requestResult = RequestResult.OkWithErrorInside;
				yield break;
			}

			if (www.responseHeaders == null || www.responseHeaders.Count == 0)
			{
				Debug.Log("Response has no headers, no error field, no server error item in message (can't identify status) - handling as CantReachServer");
				_requestResult = RequestResult.CantReachServer;
				yield break;
			}


			if (www.responseHeaders.ContainsKey(ttStatusHeaderKey))
			{
				_requestResult = (RequestResult)(Int32.Parse(www.responseHeaders[ttStatusHeaderKey]));
				if (_requestResult == RequestResult.Continue)
					_requestResult = RequestResult.Ok;
				yield break;
			}
			string status = "";
			if ( (statusHeaderKey != "") && (www.responseHeaders.ContainsKey(statusHeaderKey)) )
			{
				status = www.responseHeaders[statusHeaderKey];
				
				if (status == "HTTP/1.1 100 Continue" || status == "HTTP/1.1 200 OK")
					_requestResult = RequestResult.Ok;
				else if (status == "HTTP/1.1 301 Moved Permanently")
					_requestResult = RequestResult.MovedPermanently;
				else if (status == "HTTP/1.1 400 Bad Request")
					_requestResult = RequestResult.BadRequest;
				else if (status == "HTTP/1.1 401 Unauthorized")
					_requestResult = RequestResult.UnAuthorized;
				else if (status == "HTTP/1.1 404 Not Found")
					_requestResult = RequestResult.NotFound;
				else if (status == "HTTP/1.1 500 Internal ServerError")
					_requestResult = RequestResult.InternalServerError;
				else if (status == "HTTP/1.1 500 Internal Server Error")
					_requestResult = RequestResult.InternalServerError;
				else
				{
					Debug.Log("Got response status: \"" + status + "\" (handling as internal server error)");
					_requestResult = RequestResult.InternalServerError;
				}
				yield break;
			}
			else
			{
				if (www.responseHeaders.ContainsValue ("HTTP/1.1 100 Continue") || www.responseHeaders.ContainsValue ("HTTP/1.1 200 OK"))
					_requestResult = RequestResult.Ok;
				else if (www.responseHeaders.ContainsValue ("HTTP/1.1 301 Moved Permanently"))
					_requestResult = RequestResult.MovedPermanently;
				else if (www.responseHeaders.ContainsValue ("HTTP/1.1 400 Bad Request"))
					_requestResult = RequestResult.BadRequest;
				else if (www.responseHeaders.ContainsValue ("HTTP/1.1 401 Unauthorized"))
					_requestResult = RequestResult.UnAuthorized;
				else if (www.responseHeaders.ContainsValue ("HTTP/1.1 404 Not Found"))
					_requestResult = RequestResult.NotFound;
				else if (www.responseHeaders.ContainsValue ("HTTP/1.1 500 Internal ServerError"))
					_requestResult = RequestResult.InternalServerError;
				else if (www.responseHeaders.ContainsValue ("HTTP/1.1 500 Internal Server Error"))
					_requestResult = RequestResult.InternalServerError;
				else 
				{
					Debug.Log("Got response status: \"" + status + "\" (handling as internal server error)");
					_requestResult = RequestResult.InternalServerError;
				}
				yield break;
			}

		}

		public IEnumerator SendRequest(RequestType type, Action<RequestResult, string> callback, string data = "", 
		                               SocialConnectData socialConnectData = null, string uploadTableName = "")
		{
			
			bool requestSuccess = false;
			int requestTriesCount = 0;
			while (!requestSuccess)
			{
				if (requestTriesCount > 0)
				{
					yield return new WaitForSeconds(RequestFailWaitTime);
					Debug.Log("Request [" + type + "] failed with result [" + _requestResult + "] retrying #" + requestTriesCount);
				}

				requestTriesCount++;
				if (requestTriesCount == MaxRequestRetries)
				{
					Debug.Log ("Failed to perform request 3 times. Canceling request.");
					break;
				}

				switch (type)
				{
				case RequestType.Login:
					yield return _coroutineFactory.StartCoroutine(SendLoginRequest);
					break;
				case RequestType.GetData:
					yield return _coroutineFactory.StartCoroutine(SendGetDataRequest);
					break;
				case RequestType.SyncData:
					yield return _coroutineFactory.StartCoroutine(() => SendSyncStateRequest(data));
					break;
				case RequestType.ReceiptValidation:
					yield return _coroutineFactory.StartCoroutine(() => SendReceiptValidationRequest(data));
					break;
				case RequestType.ConnectToSocialNetwork:
					yield return _coroutineFactory.StartCoroutine(() => SendConnectToNetworkRequest(socialConnectData));
					break;
				case RequestType.GetVersion:
					yield return _coroutineFactory.StartCoroutine(SendGetVersionRequest);
					break;
				case RequestType.GetAvatar:
					yield return _coroutineFactory.StartCoroutine(() => SendGetAvatarRequest(data));
					break;
				case RequestType.UploadConfig:
					if (!string.IsNullOrEmpty(uploadTableName))
					{
						yield return _coroutineFactory.StartCoroutine(() => UploadConfigData(uploadTableName,data));
					}
					break;
				case RequestType.GetRelationshipsScores:
					yield return _coroutineFactory.StartCoroutine(() => SendGetRelationshipsScoresRequest(data));
					break;
				case RequestType.RefreshRelationships:
					yield return _coroutineFactory.StartCoroutine(SendRefreshRelationshipsRequest);
					break;
				case RequestType.PushNotificationData:
					yield return _coroutineFactory.StartCoroutine(() => SendGetPushTokenRequest(data));
					break;
				case RequestType.GetServerTime:
					yield return _coroutineFactory.StartCoroutine(() => SendGetServerTimeRequest());
					break;
				case RequestType.EventSystemRegistration:
					yield return _coroutineFactory.StartCoroutine(() => SendRegisterEventRequest(data));
					break;
				case RequestType.GetTopLeaderboardRequest:
					yield return _coroutineFactory.StartCoroutine(() => SendTopLeaderboardRequest(data));
					break;
				case RequestType.GetRankAndNeighboursRequest:
					yield return _coroutineFactory.StartCoroutine(() => SendRankAndNeighboursRequest(data));
					break;
				case RequestType.GetCountry:
					yield return _coroutineFactory.StartCoroutine(SendGetCountryRequest);
					break;

				}
				if (_requestResult != RequestResult.NoInternet && _requestResult != RequestResult.CantReachServer && _requestResult != RequestResult.InternalServerError)
					requestSuccess = true;
			}

			callback(_requestResult, _requestData);

		}

		public void LogRequest(string requestName, string requestBody)
		{
			string log = "SERVER REQUEST: " + requestName + ((requestBody == null) ? " (GET) " : " (POST) ") + "info: \nUrl=" + _requestUrl;
			if (_headers == null || _headers.Count == 0)
				log += "\nHeaders: none";
			else
			{
				log = log + "\nHeaders: ";
				foreach(string key in _headers.Keys)
					log = log + "\"" + key + "\"=\"" + _headers[key] + "\" ";
			}
			log = log + ((requestBody == null) ? "\nBody: none" : "\nBody: " + requestBody);
#if UNITY_2017_1_OR_NEWER
			Debug.unityLogger.Log(Tag, log);
#else
			Debug.logger.Log(Tag, log);
#endif
		}

		public static string FirstCharToUpper(string input)
		{
			if (String.IsNullOrEmpty(input))
				throw new ArgumentException("ARGH!");
			return input.First().ToString().ToUpper() + input.Substring(1);
		}
		
		public IEnumerator UploadConfigData(string tableName , string data)
		{
			BuildUploadConfigDataRequest(tableName);
			SetHeaders(data.Length);
			if (data.Length < 100) {
				
				LogRequest("UploadConfigDataRequest", data);
			}
			else {
				LogRequest("UploadConfigDataRequest", data.Substring(0,100));
			}

			WWW wwwRequest = new WWW(_requestUrl, _encoding.GetBytes(data), _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		public IEnumerator SendGetVersionRequest()
		{
			BuildGetVersionRequest();
			SetHeaders(0);
			LogRequest("GetVersionRequest", null);

			WWW wwwRequest = new WWW(_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		public IEnumerator SendLoginRequest()
		{
			BuildLoginRequest();
			SetHeaders(1);
			LogRequest("LoginRequest", null);

			WWW wwwRequest = new WWW(_requestUrl, _emptyBody, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		public IEnumerator SendGetDataRequest()
		{
			BuildGetDataRequest();
			SetHeaders(0);
			LogRequest("GetDataRequest", null);

			WWW wwwRequest = new WWW(_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		public IEnumerator SendRefreshRelationshipsRequest()
		{
			BuildRefreshRelationshipsRequest();
			SetHeaders(1);
			LogRequest("RefreshRelationshipsRequest", "");
			
			WWW wwwRequest = new WWW(_requestUrl, _emptyBody, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		public IEnumerator SendGetRelationshipsScoresRequest(string levelConfigId)
		{
			BuildGetRelationshipsScoresRequest(levelConfigId);
			SetHeaders(0);
			LogRequest("GetRelationshipsScoresRequest", null);
			
			WWW wwwRequest = new WWW(_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		// push notif data

		public IEnumerator SendGetPushTokenRequest(string token)
		{
			//FIXME: Rename
			BuildGetPushNotifURLRequest(token);
			SetHeaders(0);
			LogRequest("GetTokenRequest", null);

			WWW wwwRequest = new WWW(_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		// event registration

		public IEnumerator SendRegisterEventRequest(string eventId)
		{
			BuildEventRegistrationRequest(eventId);
			SetHeaders(1);
			LogRequest("RegisterEventRequest", "");

			WWW wwwRequest = new WWW(_requestUrl, _emptyBody, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		// event top score leaderboard request
		public IEnumerator SendTopLeaderboardRequest(string eventId)
		{
			BuildTopLeaderboardRequest(eventId);
			SetHeaders(0);
			LogRequest("TopEventLeaderboardRequest", null);

			WWW wwwRequest = new WWW(_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}


		// event top RankAndNeighbours Request
		public IEnumerator SendRankAndNeighboursRequest(string eventId)
		{
			BuildRankAndNeighboursRequestRequest(eventId);
			SetHeaders(0);
			LogRequest("RankAndNeighboursRequest", null);

			WWW wwwRequest = new WWW(_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		public IEnumerator SendGetCountryRequest()
		{
			BuildGetCountryRequest();
			SetHeaders(0);
			LogRequest("GetCountryRequest", null);

			WWW wwwRequest = new WWW(_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		public IEnumerator SendGetServerTimeRequest(){
			BuildGetServerTimeRequest();
			SetHeaders(0);
			LogRequest("GetServerTime", null);
			
			WWW wwwRequest = new WWW(_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		public IEnumerator SendSyncStateRequest(string data)
		{
			BuildSyncStateRequest();
			SetHeaders(data.Length);
			LogRequest("SaveRequest", data);

			WWW wwwRequest = new WWW(_requestUrl, _encoding.GetBytes(data), _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		IEnumerator SendReceiptValidationRequest(string data)
		{
			BuildReceiptValidationRequest(data);
			SetHeaders(data.Length);

			if(Debug.isDebugBuild)
			{
				LogRequest("SendReceiptValidationRequest", data); 
			}

			WWW wwwRequest = new WWW(_requestUrl, _encoding.GetBytes(data), _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}


		public IEnumerator SendConnectToNetworkRequest(SocialConnectData socialConnectData)
		{
			BuildConnectToNetworkRequest(socialConnectData);
			byte[] body = "FirstTimeConnect".ToByteArray();

			if (socialConnectData.firstTimeConnect)
			{
				SetHeaders(body.Length);
				LogRequest("ConnectToNetworkRequest", "FirstTimeConnect");

				WWW wwwRequest = new WWW(_requestUrl, body, _headers);
				yield return wwwRequest;
				yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
			}
			else
			{
				SetHeaders(0);
				LogRequest("ConnectToNetworkRequest", null);

				WWW wwwRequest = new WWW(_requestUrl, null, _headers);
				yield return wwwRequest;
				yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
			}

		}
		
		public IEnumerator SendGetAvatarRequest(string avatarPlayerId)
		{
			BuildGetAvatarRequest(avatarPlayerId);
			SetHeaders(0);
			LogRequest("GetAvatarRequest", null);
			
			WWW wwwRequest = new WWW(_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		public IEnumerator SendAnalyticsReport(GsdkAnalyticsEventType eventType, string data, string customEndPoint = null)
		{
			switch(eventType)
			{
			case GsdkAnalyticsEventType.LevelReport:
				BuildAnalyticsLevelReport();
				SetHeaders(data.Length);
				LogRequest("AnalyticsLevelReport", data);
				break;
			case GsdkAnalyticsEventType.ReviveReport:
				BuildAnalyticsReviveReport();
				SetHeaders(data.Length);
				LogRequest("AnalyticsReviveReport", data);
				break;
			case GsdkAnalyticsEventType.CinemaReport:
				BuildAnalyticsCinemaReport();
				SetHeaders(data.Length);
				LogRequest("AnalyticsCinemaReport", data);
				break;
			case GsdkAnalyticsEventType.CustomReport:
				if(string.IsNullOrEmpty(customEndPoint))
				{
					Debug.LogError("Custom end point missing for a custom analytics report");
					yield break;
				}
				_requestUrl = String.Format("{0}{1}", _url, customEndPoint);
				SetHeaders(data.Length);
				LogRequest(string.Format("{0}(endPoint:{1})","AnalyticsCustomReport",customEndPoint), data);
				break;
			default:
				break;
			}
			
			WWW wwwRequest = new WWW(_requestUrl, _encoding.GetBytes(data), _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine(() => EvaluateRequestStatus(wwwRequest));
		}

		private void BuildAnalyticsLevelReport()
		{
			//analytics/levelReport
			_requestUrl = String.Format("{0}{1}", _url, _analyticsLevelReport);
		}
		
		private void BuildAnalyticsReviveReport()
		{
			//analytics/reviveReport
			_requestUrl = String.Format("{0}{1}", _url, _analyticsReviveReport);
		}
		
		private void BuildAnalyticsCinemaReport()
		{
			_requestUrl = String.Format("{0}{1}", _url, _analyticsCinemaReport);
		}


	}
}
