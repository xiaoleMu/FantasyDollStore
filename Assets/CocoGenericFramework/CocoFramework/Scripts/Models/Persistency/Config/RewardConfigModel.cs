using UnityEngine;
using System.Collections.Generic;
using System.Linq;
namespace TabTale
{
	public class RewardConfigModel : ConfigModel<RewardConfigData> 
	{
		public RewardConfigData GetById(string id)
		{
			var reward =_configs.FirstOrDefault(x =>  x.GetId() == id );

			if(reward == null) 
				return reward;
			
			return reward.Clone() as RewardConfigData;
		}

		public RewardType GetRewardType(string id)
		{
			var reward = GetById(id);

			if(reward == null) 
				return default(RewardType);
			
			return reward.rewardType;
		}

		public List<GameElementData> GetRestrictions(string id)
		{
			return GetById(id).restrictions;
		}

		public List<int> GetDelays(string id)
		{
			return GetById(id).delays;
		}

		public DelayType GetDelayType(string id)
		{
			return GetById(id).delayType;;
		}


	}
}