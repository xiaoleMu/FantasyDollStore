using UnityEngine;
using System.Collections;
using System;

namespace TabTale
{
	public class TeamSyncConnectionHandler : ConnectionHandler
	{
		[Inject]
		public TeamStateModel teamStateModel { get; set; }

		[Inject]
		public SocialStateModel socialStateModel { get; set; }

		[Inject]
		public RegionalService regionalService { get; set; }

		public enum TeamRequestType
		{
			AddTeam = 1,
			JoinTeam,
			GetTeamsByName,
			GetTeamLeaderboard,
			GetPlayersLeaderboard, 
			GetTop,
			GetTeam,
			UpdateScore
		}

		public IEnumerator SendRequest (TeamRequestType type, Action<RequestResult, string> callback, int score = 0, TeamData teamData = null, string name = null, bool flag = false)
		{
			bool requestSuccess = false;
			int requestTriesCount = 0;
			while (!requestSuccess) {
				if (requestTriesCount > 0) {
					yield return new WaitForSeconds (RequestFailWaitTime);
//					Debug.Log("Request [" + type + "] failed with result [" + _requestResult + "] retrying #" + requestTriesCount);
				}

				requestTriesCount++;
				if (requestTriesCount == MaxRequestRetries) {
					Debug.Log ("Failed to perform request 3 times. Canceling request.");
					break;
				}

				switch (type) {
				case TeamRequestType.AddTeam:
					yield return _coroutineFactory.StartCoroutine (() => SendAddTeamRequest (teamData));
					break;
				case TeamRequestType.JoinTeam:
					yield return _coroutineFactory.StartCoroutine (() => SendJoinTeamRequest (teamData));
					break;
				case TeamRequestType.GetTeamsByName:
					yield return _coroutineFactory.StartCoroutine (() => SendGetTeamsByNameRequest (name));
					break;
				case TeamRequestType.GetTeamLeaderboard:
					yield return _coroutineFactory.StartCoroutine (() => SendGetTopTeamsRequest (teamData, flag));
					break;
				case TeamRequestType.GetPlayersLeaderboard:
					yield return _coroutineFactory.StartCoroutine (() => SendGetTopPlayersRequest (teamData));
					break;
				case TeamRequestType.GetTop:
					yield return _coroutineFactory.StartCoroutine (() => SendGetTopRequest (teamData));
					break;
				case TeamRequestType.GetTeam:
					yield return _coroutineFactory.StartCoroutine (() => SendGetTeamRequest (teamData));
					break;
				case TeamRequestType.UpdateScore:
					yield return _coroutineFactory.StartCoroutine (() => SendUpdateScoreRequest (score));
					break;
				}

				if (_requestResult != RequestResult.NoInternet && _requestResult != RequestResult.CantReachServer && _requestResult != RequestResult.InternalServerError)
					requestSuccess = true;
			}

			callback (_requestResult, _requestData);
		}

		public IEnumerator SendAddTeamRequest (TeamData teamInfo)
		{
			BuildAddTeamRequest ();

			string teamStr = LitJson.JsonMapper.ToJson (teamInfo);
			byte[] body = teamStr.ToByteArray ();

			SetHeaders (body.Length);
			LogRequest ("AddTeamRequest", "");

			WWW wwwRequest = new WWW (_requestUrl, body, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine (() => EvaluateRequestStatus (wwwRequest));
		}

		public IEnumerator SendJoinTeamRequest (TeamData teamData)
		{
			BuildJoinTeamRequest (teamData);
			SetHeaders (1);
			LogRequest ("JoinTeamRequest", "");

			WWW wwwRequest = new WWW (_requestUrl, _emptyBody, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine (() => EvaluateRequestStatus (wwwRequest));
		}

		public IEnumerator SendGetTeamsByNameRequest (string namePrefix)
		{
			BuildGetTeamsByNameRequest (namePrefix);
			SetHeaders (0);
			LogRequest ("GetTeamsByNameRequest", null);

			WWW wwwRequest = new WWW (_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine (() => EvaluateRequestStatus (wwwRequest));
		}

		public IEnumerator SendGetTopTeamsRequest (TeamData teamData, bool calcRankChanges)
		{
			BuildGetTopTeamsRequest (teamData, calcRankChanges);
			SetHeaders (0);
			LogRequest ("GetTopTeamsRequest", null);

			WWW wwwRequest = new WWW (_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine (() => EvaluateRequestStatus (wwwRequest));
		}

		public IEnumerator SendGetTopPlayersRequest (TeamData teamData)
		{
			BuildGetTopPlayersRequest (teamData);
			SetHeaders (0);
			LogRequest ("GetTopPlayersRequest", null);

			WWW wwwRequest = new WWW (_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine (() => EvaluateRequestStatus (wwwRequest));
		}

		public IEnumerator SendGetTopRequest (TeamData teamData)
		{
			BuildGetTopRequest (teamData);
			SetHeaders (0);
			LogRequest ("GetTopRequest", null);

			WWW wwwRequest = new WWW (_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine (() => EvaluateRequestStatus (wwwRequest));
		}

		public IEnumerator SendGetTeamRequest (TeamData teamData)
		{
			BuildGetTeamRequest (teamData);
			SetHeaders (0);
			LogRequest ("GetTeamRequest", null);

			WWW wwwRequest = new WWW (_requestUrl, null, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine (() => EvaluateRequestStatus (wwwRequest));
		}

		public IEnumerator SendUpdateScoreRequest (int score)
		{
			BuildUpdateScoreRequest (score);
			SetHeaders (1);
			LogRequest ("UpdateScoreRequest", "");

			WWW wwwRequest = new WWW (_requestUrl, _emptyBody, _headers);
			yield return wwwRequest;
			yield return _coroutineFactory.StartCoroutine (() => EvaluateRequestStatus (wwwRequest));
		}
			
		private void BuildAddTeamRequest ()
		{
			string playerName = socialStateModel.GetPlayerName ();
			string playerImage = socialStateModel.GetProfileImage().value;

			_requestUrl = String.Format ("{0}{1}?{2}={3}&{4}={5}", _url, "teams", "playerName",WWW.EscapeURL(playerName), "playerImage",WWW.EscapeURL(playerImage));
		}

		private void BuildJoinTeamRequest (TeamData teamData)
		{
			string playerName = socialStateModel.GetPlayerName ();
			string playerImage = socialStateModel.GetProfileImage().value;

			_requestUrl = String.Format ("{0}{1}/{2}/{3}?{4}={5}&{6}={7}", _url, "teams", teamData.id,"players","playerName",WWW.EscapeURL(playerName), "playerImage",WWW.EscapeURL(playerImage));
		}

		private void BuildGetTeamsByNameRequest (string namePrefix)
		{
			_requestUrl = String.Format ("{0}{1}?{2}={3}",_url,"teams","query",namePrefix);
		}

		private void BuildGetTopTeamsRequest (TeamData teamData, bool calcRankChanges)
		{

			_requestUrl = String.Format ("{0}{1}/{2}?{3}={4}",_url,"teams","leaderboard","rankChange",calcRankChanges);
		}

		private void BuildGetTopPlayersRequest (TeamData teamData)
		{
			_requestUrl = String.Format ("{0}{1}/{2}/{3}",_url,"teams",teamData.id,"leaderboard");
		}

		private void BuildGetTopRequest (TeamData teamData)
		{
			_requestUrl = String.Format ("{0}{1}/{2}/{3}?{4}={5}",_url,"teams",teamData.id,"top","country",teamData.teamInfo.countryId);
		}

		private void BuildGetTeamRequest(TeamData teamInfo)
		{
			_requestUrl = String.Format ("{0}{1}/{2}",_url,"teams",teamInfo.id);
		}

		private void BuildUpdateScoreRequest (int score)
		{
			_requestUrl = String.Format ("{0}{1}/{2}/{3}?{3}={4}",_url,"teams", teamStateModel.GetTeamId(), "score", score);
		}
	}
}