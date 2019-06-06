using System;
using System.Collections.Generic;

namespace TabTale
{
	public class RewardItemConfigData : IConfigData, ICloneable {
		public string id;
		public float weight;
		public float rangeMultiplayer = 1.0f;
		public List<GameElementData> rewards = new List<GameElementData>();
		public List<GameElementData> restrictions = new List<GameElementData>();
		public bool allowSocialFactor;
		public bool allowRVFactor;
		public string rewardConfigId;
		public int index;

		#region ICloneable implementation

		object ICloneable.Clone ()
		{
			return Clone();
		}

		#endregion

	
		#region IConfigData implementation
		public string GetTableName ()
		{
			return "reward_item_config";
		}
		public string GetId ()
		{
			return id;
		}
		public string ToLogString ()
		{
			return ToString();
		}
		public bool IsBlob()
		{
			return false;
		}
		public IConfigData Clone ()
		{
			RewardItemConfigData c = new RewardItemConfigData();
			c.id = id;
			c.weight = weight;
			c.allowRVFactor = allowRVFactor;
			c.allowSocialFactor = allowSocialFactor;
			c.rewardConfigId = rewardConfigId;
			c.index = index;
			c.rangeMultiplayer = rangeMultiplayer;
			c.rewards = new List<GameElementData>(rewards.Clone());
			c.restrictions = new List<GameElementData>(restrictions.Clone());
			return c;
		}
		#endregion
	}
}