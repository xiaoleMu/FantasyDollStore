using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	[Serializable]
	public class AchievementsConfigData : IConfigData 
	{
		public string id;
		public string name;
		public string achievementId;
		public int progress;
		public string store;
		
		public AchievementsConfigData() {}
		public string GetTableName() { return "achievement_config"; }
		public string GetId() { return id; }
		public string ToLogString() { return ("{ Achievement: " + "id:" + id + " name:" + name + 
			                                      " achievementId:" + achievementId + "progress:" + progress + " store:" + store + "}"); }

		public bool IsBlob()
		{
			return false;
		}

		public IConfigData Clone()
		{
			AchievementsConfigData config = new AchievementsConfigData();
			config.id = id;
			config.name = name;
			config.achievementId = achievementId;
			config.progress = progress;
			config.store = store;
			return config;
		}
	}
}