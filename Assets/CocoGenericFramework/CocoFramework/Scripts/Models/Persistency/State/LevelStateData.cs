using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class LevelStateData : ISharedData
	{
		public string id;
		public int score;
		public int xpGained;
		public MatchStatus status;
		public List<ItemData> itemsInUse = new List<ItemData>();
		public List<GameElementData> progress = new List<GameElementData>();
		public List<GameElementData> currenciesEarned = new List<GameElementData>();
		public List<GameElementData> properties = new List<GameElementData>();

		#region IStateData implementation
		
		public string GetTableName ()
		{
			return "level_state";
		}

		public string GetId()
		{
			return id;
		}
		
		public string ToLogString ()
		{
			return string.Format ("[LevelSharedData: LevelConfigId={0}, Score={1}, XpGained={2}, Status={3}, ItemsInUse={4}, Progress={5}, CurrenciesEarned={6}, Properties={7}]", 
			                      	id, score, xpGained, status, itemsInUse.ArrayString(), progress.ArrayString(), currenciesEarned.ArrayString(), properties.ArrayString());
		}
		
		public ISharedData Clone ()
		{
			LevelStateData c = new LevelStateData();
			c.id = id;
			c.score = score;
			c.xpGained = xpGained;
			c.status = status;
			c.progress = new List<GameElementData>(progress.Clone());
			c.currenciesEarned = new List<GameElementData>(currenciesEarned.Clone());
			c.properties = new List<GameElementData>(properties.Clone());
			
			return c;
		}
		
		#endregion
	}
}