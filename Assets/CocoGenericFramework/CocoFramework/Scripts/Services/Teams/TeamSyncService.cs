using UnityEngine;
using System.Collections;
using RSG;
using System;
using System.Globalization;
using LitJson;
using System.Collections.Generic;

namespace TabTale
{
	public class TeamSyncService : ITeamSyncService
	{
		[Inject]
		public TeamStateModel teamStateModel { get; set; }

		[Inject]
		public TeamSyncConnectionHandler connectionHandler { get; set; }

		[Inject]
		public TeamSyncConnectionHandler notificationConnectionHandler { get; set; }

		// We have to use different connection handlers since they keep state,
		// Which can causes race conditions in heavy network traffic games.
		// This duplication should be removed once the connection handler is fixed
		[Inject]
		public TeamSyncConnectionHandler topTeamsConnectionHandler { get; set; }

		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		[Inject]
		public RegionalService regionalService { get; set; }

		[Inject]
		public PlayerInfoService playerInfoService { get; set; }

		[Inject]
		public ServerTime serverTime { get; set; }

		[Inject]
		public GeneralParameterConfigModel generalParameters { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		[Inject]
		public RankChangedSignal rankChangedSignal { get; set; }

		[Inject]
		public TeamScoreUpdatedSignal teamScoreUpdatedSignal { get; set; }

		private static string Tag { get { return "TeamSyncService".Colored(Colors.teal).Bold(); } }

		private string _resetTime;

		[PostConstruct]
		public void Init()
		{
			_resetTime = generalParameters.GetString("TeamsResetTime", "00:00:00");
		}

		public Promise<List<TeamData>> GetTeamsByName(string namePrefix)
		{
			var promise = new Promise<List<TeamData>>();

			Action<ConnectionHandler.RequestResult, string> HandleGetTeamsByName = (result,response) => { 

				logger.Log(Tag,"HandleGetTeamsByNameResponse (result=" + result + ")\nresponse=" + response);

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					promise.Reject(new Exception(response));
					return;
				}

				List<TeamData> teams = JsonMapper.ToObject<List<TeamData>>(response);

				promise.Resolve(teams);
			};

			routineRunner.StartCoroutine(connectionHandler.SendRequest(TeamSyncConnectionHandler.TeamRequestType.GetTeamsByName, HandleGetTeamsByName, 0, null, namePrefix));

			return promise;
		}

		public Promise<List<TeamData>> GetTopTeams(bool calcRankChanges)
		{
			var promise = new Promise<List<TeamData>>();

			Action<ConnectionHandler.RequestResult, string> HandleGetTeamsByName = (result,response) => { 

				logger.Log(Tag,"HandleGetTopTeamsResponse (result=" + result + ")\nresponse=" + response);

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					promise.Reject(new Exception(response));
					return;
				}

				List<TeamData> teams = JsonMapper.ToObject<List<TeamData>>(response);

				promise.Resolve(teams);
			};

			regionalService.GetCountryCode().Done( countryCode => {

				logger.Log(Tag, "CountryCode :" + countryCode);
				TeamData teamData = CreateTeamData(countryCode);

				routineRunner.StartCoroutine(topTeamsConnectionHandler.SendRequest(TeamSyncConnectionHandler.TeamRequestType.GetTeamLeaderboard, HandleGetTeamsByName, 0, teamData, "", calcRankChanges));
			});
				

			return promise;
		}

		public Promise<List<TeamStateData>> GetTeamTopPlayers()
		{
			var teamData = new TeamData();
			teamData.id = teamStateModel.GetTeamId();

				
			var promise = new Promise<List<TeamStateData>>();

			Action<ConnectionHandler.RequestResult, string> HandleGetTopPlayers = (result,response) => { 

				logger.Log(Tag,"HandleGetTopPlayersResponse (result=" + result + ")\nresponse=" + response);

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					promise.Reject(new Exception(response));
					return;
				}

				List<TeamStateData> topPlayers = JsonMapper.ToObject<List<TeamStateData>>(response);

				promise.Resolve(topPlayers);
			};

			routineRunner.StartCoroutine(connectionHandler.SendRequest(TeamSyncConnectionHandler.TeamRequestType.GetPlayersLeaderboard, HandleGetTopPlayers, 0, teamData));

			return promise;
		}

		public Promise<RankChange> CheckRankChanges()
		{
			var promise = new Promise<RankChange>();
			/*
			if( ! teamStateModel.PlayerHasATeam())
			{
				promise.Resolve(new RankChange());
				return promise;
			}
				
			int prevTeamRank = TTPlayerPrefs.GetValue("TeamRank", 0);
			int prevPlayerRank = TTPlayerPrefs.GetValue("PlayerRank", 0);

			RefreshTopIds()
				.Catch( _ => {
					promise.Resolve(new RankChange());
				})
				.Done( topData => {

				long playerId = playerInfoService.NumericalPlayerId;

				var playerRank = topData.topPlayers.FindIndex(pId => pId == playerId) + 1;
				var teamRank = topData.topTeams.FindIndex(tId => tId == teamStateModel.GetTeamId()) + 1;

				RankChange rankChange = new RankChange(playerRank, prevPlayerRank, teamRank, prevTeamRank);

				if(rankChange.IsPlayerRankChanged || rankChange.IsTeamRankChanged)
				{
					rankChangedSignal.Dispatch(rankChange);
				}

				// Update previous ranks:
				TTPlayerPrefs.SetValue("TeamRank", teamRank);
				TTPlayerPrefs.SetValue("PlayerRank", playerRank);

				promise.Resolve(rankChange);
			});
			*/
			return promise;
		}

		public Promise<TopData> RefreshTopIds()
		{
			var promise = new Promise<TopData>();

			if(! teamStateModel.PlayerHasATeam())
			{
				promise.Reject(new Exception("Error : Attempted to update team score before a team was joined"));
				return promise;
			}

			Action<ConnectionHandler.RequestResult, string> HandleGetTop = (result,response) => { 

				logger.Log(Tag,"HandleRefreshTopIdsResponse (result=" + result + ")\nresponse=" + response);

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					promise.Reject(new Exception(response));
					return;
				}

				TopData topData = JsonMapper.ToObject<TopData>(response);
				promise.Resolve(topData);
			};

			regionalService.GetCountryCode().Done( countryCode => {

				logger.Log(Tag, "CountryCode :" + countryCode);
				TeamData teamData = CreateTeamData(countryCode);

				routineRunner.StartCoroutine(connectionHandler.SendRequest(TeamSyncConnectionHandler.TeamRequestType.GetTop, HandleGetTop, 0, teamData));
			});

			return promise;
		}

		/*
		public Promise<TeamData> AddTeam(string name, List<AssetData> assets, List<GenericPropertyData> properties)
		{
			TeamInfo teamInfo = new TeamInfo()

			var promise = new Promise<TeamStateData>();

			
			Action<ConnectionHandler.RequestResult, string> HandleAddTeam = (result,response) => { 

				logger.Log(Tag,"HandleAddTeamResponse (result=" + result + ")\nresponse=" + response);

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					promise.Reject(new Exception(response));
					return;
				}

				TeamStateData teamData = JsonMapper.ToObject<TeamStateData>(response);

				teamStateModel.SetState(teamData);

				promise.Resolve(teamData);
			};

			routineRunner.StartCoroutine(connectionHandler.SendRequest(TeamSyncConnectionHandler.TeamRequestType.AddTeam, HandleAddTeam, 0, teamInfo));

			return promise;
		}
		*/

		public Promise<TeamData> GetTeamData(string teamId)
		{
			TeamData teamData = new TeamData();
			teamData.id = teamId;

			var promise = new Promise<TeamData>();
			Action<ConnectionHandler.RequestResult, string> HandleGetTeam = (result,response) => { 

				logger.Log(Tag,"HandleGetTeamDataResponse (result=" + result + ")\nresponse=" + response);

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					string persistedScore = TTPlayerPrefs.GetValue("TeamScore",0).ToString();
					promise.Reject(new Exception(persistedScore));
					return;
				}

				TeamData responseTeamData = JsonMapper.ToObject<TeamData>(response);

				NotifyTeamScoreIsUpdated(responseTeamData.score);

				promise.Resolve(responseTeamData);
			};

			routineRunner.StartCoroutine(notificationConnectionHandler.SendRequest(TeamSyncConnectionHandler.TeamRequestType.GetTeam, HandleGetTeam, 0, teamData));

			return promise;
		}

		public Promise<TeamStateData> JoinTeam(string teamId = "")
		{
			if(teamId.IsNullOrEmpty())
			{
				teamId = "0";
			}

			var teamData = new TeamData();
			teamData.id = teamId;

			var promise = new Promise<TeamStateData>();
			Action<ConnectionHandler.RequestResult, string> HandleJoinTeam = (result,response) => { 

				logger.Log(Tag,"HandleJoinTeamResponse (result=" + result + ")\nresponse=" + response);

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					promise.Reject(new Exception(response));
					return;
				}

				TeamStateData teamStateData = JsonMapper.ToObject<TeamStateData>(response);

				teamStateModel.SetState(teamStateData);

				promise.Resolve(teamStateData);
			};

			routineRunner.StartCoroutine(connectionHandler.SendRequest(TeamSyncConnectionHandler.TeamRequestType.JoinTeam, HandleJoinTeam, 0, teamData));

			return promise;
		}

		private class TeamScore
		{
			public int score;
		}

		public Promise<int> UpdateTeamScore(int scoreToAdd)
		{
			var promise = new Promise<int>();

			if(! teamStateModel.PlayerHasATeam())
			{
				promise.Reject(new Exception("Error : Attempted to update team score before a team was joined"));
				return promise;
			}

			Action<ConnectionHandler.RequestResult, string> HandleUpdateScore = (result,response) => { 

				logger.Log(Tag,"HandleUpdateTeamScoreResponse (result=" + result + ")\nresponse=" + response);

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					promise.Reject(new Exception(response));
					return;
				}

				int teamScore = JsonMapper.ToObject<TeamScore>(response).score;

				NotifyTeamScoreIsUpdated(teamScore);

				promise.Resolve(teamScore);
			};

			routineRunner.StartCoroutine(topTeamsConnectionHandler.SendRequest(TeamSyncConnectionHandler.TeamRequestType.UpdateScore, HandleUpdateScore, scoreToAdd));

			return promise;
		}

		public TimeSpan TimeUntilNextTeamScoreReset()
		{
			TimeSpan resetTimeSpan;
			TimeSpan.TryParse(_resetTime, out resetTimeSpan);

			DateTime currentTime = serverTime.GetLocalTime().ToUniversalTime();
			DateTime resetDate = currentTime.Date + resetTimeSpan;

			// If time of day is bigger than reset time of day then add 1 day (since next reset is tomorrow)
			TimeSpan currentTimeInDay = currentTime - currentTime.Date;
			if(currentTimeInDay > resetTimeSpan)
			{
				resetDate = resetDate.AddDays(1);
			}

			TimeSpan timeLeft = resetDate - currentTime;

			return timeLeft;
			
		}

		private TeamData CreateTeamData(string countryCode)
		{
			TeamData teamData = new TeamData();
			teamData.id = teamStateModel.GetTeamId();

			string playerTeamCountry = teamStateModel.GetTeamCountry();
			if(! String.IsNullOrEmpty(playerTeamCountry))
			{
				teamData.teamInfo.countryId = playerTeamCountry;
			}

			return teamData;
		}

		private void NotifyTeamScoreIsUpdated(int teamScore)
		{
			TTPlayerPrefs.SetValue("TeamScore",teamScore);
			teamScoreUpdatedSignal.Dispatch(teamScore);
		}
	}
}