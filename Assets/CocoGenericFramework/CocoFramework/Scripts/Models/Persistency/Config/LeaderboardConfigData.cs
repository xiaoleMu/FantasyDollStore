using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	[Serializable]
	public class LeaderboardConfigData : IConfigData 
	{
		public string id;
		public string name;
		public string leaderboardId;
		public string store;
		
		public LeaderboardConfigData() {}
		public string GetTableName() { return "leaderboard_config"; }
		public string GetId() { return id; }
		public string ToLogString() { return ("{ LeaderboardConfigData: " + "id:" + id + " name:" + name + " leaderboardId:" + leaderboardId + " store:" + store + "}"); }

		public bool IsBlob()
		{
			return false;
		}

		public IConfigData Clone()
		{
			LeaderboardConfigData config = new LeaderboardConfigData();
			config.id = id;
			config.name = name;
			config.leaderboardId = leaderboardId;
			config.store = store;
			return config;
		}
	}
}
