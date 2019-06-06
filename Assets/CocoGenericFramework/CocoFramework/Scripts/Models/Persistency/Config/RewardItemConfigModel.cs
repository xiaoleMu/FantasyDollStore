using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public class RewardItemConfigModel : ConfigModel<RewardItemConfigData> 
	{
		public RewardItemConfigData GetItemById(string rewardItemConfigId)
		{
			return _configs.FirstOrDefault(x=> x.GetId() == rewardItemConfigId).Clone() as RewardItemConfigData;
		}

		public List<GameElementData> GetRewardsForItem(string rewardItemConfigId)
		{
			var res = (from reward in _configs where reward.GetId() == rewardItemConfigId select reward.rewards).FirstOrDefault();
			if (res == null) {
				return new List<GameElementData>();
			}
			return res.Clone().ToList();
		}

		public List<RewardItemConfigData> GetRewards(string rewardConfigId)
		{
			var res = from reward in _configs 
				where reward.rewardConfigId == rewardConfigId 
				orderby reward.index 
				select reward;

			return res.ToList().Clone().ToList();
		}
		
		public int GetRewardsLength(string rewardConfigId)
		{
			return GetRewards(rewardConfigId).Count;
		}

	}
}