using System;
using UnityEngine;
using RSG;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public class TeamInfo
	{
		public string id;
		public string name;
		public int fansCount;
		public string countryId;
		public List<AssetData> assets = new List<AssetData>();

		public TeamInfo() { }

		public TeamInfo(string id, string name, int fansCount, string countryId, List<AssetData> assets = null)
		{
			this.name = name;
			this.fansCount = fansCount;
			this.countryId = countryId;

			if(assets != null)
				this.assets = assets;


		}

		public override string ToString ()
		{
			return LitJson.JsonMapper.ToJson (this);
		}
	}

	public class TopData
	{
		public List<string> topPlayers;
		public List<string> topTeams;
	}

	public class RankChange
	{
		// Getters
		public bool IsPlayerRankChanged { get { return playerRank != previousPlayerRank; } }
		public bool IsTeamRankChanged { get { return teamRank != previousTeamRank; } }

		public RankChange() { }

		public RankChange(int playerRank, int previousPlayerRank, int teamRank, int previousTeamRank)
		{
			this.playerRank = playerRank;
			this.previousPlayerRank = previousPlayerRank;

			this.teamRank = teamRank;
			this.previousTeamRank = previousTeamRank;
		}

		public int teamRank;
		public int previousTeamRank;
		public int playerRank;
		public int previousPlayerRank;

		public override string ToString ()
		{
			return LitJson.JsonMapper.ToJson(this);
		}

	}

	public interface ITeamSyncService
	{
		Promise<List<TeamData>> GetTeamsByName(string namePrefix);

		Promise<List<TeamData>> GetTopTeams(bool calcRankChanges);

		Promise<List<TeamStateData>> GetTeamTopPlayers();

		Promise<RankChange> CheckRankChanges();

		Promise<TopData> RefreshTopIds(); // Get top players ids and top teams ids - used for notifications

		//Promise<TeamStateData> AddTeam(string name, List<AssetData> assets, List<GenericPropertyData> properties);

		Promise<TeamStateData> JoinTeam(string teamId = "");

		Promise<TeamData> GetTeamData(string teamId);

		Promise<int> UpdateTeamScore(int scoreToAdd);

		TimeSpan TimeUntilNextTeamScoreReset();
	}
}

