using UnityEngine;
using System.Collections.Generic;
using System;

namespace TabTale
{
	public class EventConfigData : IConfigData
	{
		public string id;
		public string name;
		public string description;
		public string objectiveId;
		public DateTime startTime;
		public DateTime endTime;
		public List<string> 				levels = new List<string>();
		public List<string> 				affinityItems = new List<string>();
		public List<EventPrizeData> 		rankPrizes = new List<EventPrizeData>();
		public List<EventPrizeData> 		progressionPrizes = new List<EventPrizeData>();
		public List<AssetData> 				assets = new List<AssetData>();
		public List<GenericPropertyData> 	properties = new List<GenericPropertyData>();
		public List<GameElementData> 		restrictions = new List<GameElementData>();	

		#region ConfigData implementation
		public string GetTableName ()
		{
			return "event_config";
		}

		public string GetId ()
		{
			return id;
		}

		public string ToLogString ()
		{
			string res = @"EventConfigData: Id: {0}, Name: {1}, Description: {2}, ObjectiveId: {3}, StartTime: {4}, EndTime: {5}, 
			Levels: {6}, AffinityItems: {7}, Rank Prizes: {8}, Progression Prizes: {9}, Assets: {10}, Properties:{11}";
			
			return string.Format(res, id, name, description, objectiveId, startTime, endTime, levels.ArrayString(), affinityItems.ArrayString(),
				rankPrizes.ArrayString(), progressionPrizes.ArrayString(), assets.ArrayString(), properties.ArrayString());
		}

		public bool IsBlob()
		{
			return false;
		}

		public IConfigData Clone ()
		{
			EventConfigData c = new EventConfigData();
			c.id = id;
			c.name = name;
			c.description = description;
			c.objectiveId = objectiveId;
			c.startTime = startTime;
			c.endTime = endTime;
			c.levels = new List<string>(levels.Clone());
			c.affinityItems = new List<string>(affinityItems.Clone());
			c.rankPrizes = new List<EventPrizeData>(rankPrizes.Clone());
			c.progressionPrizes = new List<EventPrizeData>(progressionPrizes.Clone());
			c.assets = new List<AssetData>(assets.Clone());
			c.properties = new List<GenericPropertyData>(properties.Clone());

			return c;
		}
		#endregion
	}
}

