using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TabTale 
{
	public class AchievementsConfigModel : ConfigModel<AchievementsConfigData>
	{
		public string GetAchievementId(string achievementName)
		{
			AchievementsConfigData data = _configs.FirstOrDefault(config => config.name == achievementName && 
			                               (config.store == GetStoreType() || config.store == "all"));

			return data.achievementId;
		}
		
		private string GetStoreType()
		{
			return (Application.platform == RuntimePlatform.Android) ? "google" : "ios";
		}
		
	}
	
}