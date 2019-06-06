using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TabTale 
{
	public class LeaderboardConfigModel : ConfigModel<LeaderboardConfigData>
	{
		public string GetLeaderboardId(string leaderboardName)
		{
			LeaderboardConfigData data = _configs.FirstOrDefault(config => config.name == leaderboardName && 
			                              (config.store == GetStoreType() || config.store == "all"));

			return data.leaderboardId;
		}
		
		private string GetStoreType()
		{
			return (Application.platform == RuntimePlatform.Android) ? "google" : "ios";
		}
		
	}
	
}