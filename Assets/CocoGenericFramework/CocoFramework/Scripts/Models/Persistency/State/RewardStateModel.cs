using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace TabTale
{
	public class RewardStateModel : StateModel<RewardStateData> 
	{

		[Inject]
		public RewardConfigModel rewardConfigModel {get;set;}

		[Inject]
		public RewardItemConfigModel rewardItemConfigModel {get;set;}

		[Inject]
		public ServerTime serverTime {get;set;}

		[Inject]
		public CurrencyStateModel currencyStateModel {get;set;}

		[Inject]
		public InventoryStateModel inventoryStateModel {get;set;}


		public RewardStateData GetData()
		{
			return _data.Clone() as RewardStateData;
		}

		public List<int> GetRewardIndexHistory(string rewardConfigId)
		{
			var res = from r in  GetRewardItemHistory(rewardConfigId)
				select r.index;
			return res.ToList();
		}

		public List<RewardItemConfigData> GetRewardItemHistory(string rewardConfigId)
		{
			var res = from reward in _data.rewards
				join rewardItem in rewardItemConfigModel.GetRewards(rewardConfigId) 
					on reward.rewardItemConfigId equals rewardItem.id
					where reward.rewardConfigId == rewardConfigId 
					select rewardItem;
			var tmp = new List<RewardItemConfigData>( res.ToList().Clone());
			return tmp;
		}

		public List<RewardItemData> GetRewardHistory(string rewardConfigId)
		{
			var res = from reward in _data.rewards
				join rewardItem in rewardItemConfigModel.GetRewards(rewardConfigId) 
					on reward.rewardItemConfigId equals rewardItem.id
					where reward.rewardConfigId == rewardConfigId 
					select reward;
			
			return new List<RewardItemData>(res.ToList().Clone());
		}

		public int GetLastRewardIndex(string id)
		{
			return GetRewardIndexHistory(id).LastOrDefault();
		}

		public RewardItemConfigData GetLastReward(string rewardConfigId)
		{
			return GetRewardItemHistory(rewardConfigId).LastOrDefault();
		}

		//Returns the next reward that the player should recieve in the specified rewardConfigId.
		public RewardItemConfigData NextReward(string rewardConfigId)
		{
			//If not advent
			List<RewardItemConfigData> possibleRewards = rewardItemConfigModel.GetRewards(rewardConfigId);

			RewardItemConfigData lastReward = GetLastReward(rewardConfigId);

			//skip rewards if the time for them have passed (advent only)
			int rewardSkip = RewardSkip(rewardConfigId);

			for (int i = 0; lastReward != null && i < possibleRewards.Count; i++) 
			{
				if (possibleRewards[i].id == lastReward.id) 
				{
					//Returns the next in line or the first if were at the end. (MATH!)
					if (rewardConfigModel.GetRewardType(rewardConfigId) != RewardType.Advent)
					{
						return possibleRewards[(i+rewardSkip) % possibleRewards.Count];
					}
					else 
					{
						int index = i+rewardSkip >=	possibleRewards.Count ? 0 : i+rewardSkip;
						return possibleRewards[index];
					}
				}
			}
			return possibleRewards[0];

		}

		public string GetLastRewardedDate(string rewardConfigId)
		{
			List<RewardItemData> rewards = GetRewardHistory(rewardConfigId);
			return rewards.Count > 0 ? rewards.Last().rewardDate : "2015-01-01T00:00:00Z";
		}

		public bool ClaimReward(string rewardConfigId, string rewardItemConfigId)
		{
			RewardItemData newItem = new RewardItemData();
			newItem.rewardConfigId = rewardConfigId;
			newItem.rewardItemConfigId = rewardItemConfigId;
			newItem.rewardDate = serverTime.ToServerString(serverTime.GetLocalTime());

			//if this was claimed before, it means were looping so we need to clear the rewards list.
			if (_data.rewards.Any(item=> item.rewardConfigId == rewardConfigId && item.rewardItemConfigId == rewardItemConfigId))
			{
				ResetRewards(rewardConfigId);
			}
			_data.rewards.Add(newItem);
			return Save();
		}

		public void ResetRewards(string rewardConfigId)
		{
			_data.rewards.RemoveAll(r => r.rewardConfigId == rewardConfigId);
		}

		private int RewardSkip (string rewardConfigId)
		{
			if (rewardConfigModel.GetRewardType(rewardConfigId) != RewardType.Advent)
			{
				return 1;
			}

			DateTime lastRewardDate = serverTime.ToLocal(GetLastRewardedDate(rewardConfigId));
			DateTime now = serverTime.GetLocalTime();

			if (rewardConfigModel.GetDelayType(rewardConfigId) == DelayType.Days)
			{
				now = serverTime.GetDateStart(now);
			}

			TimeSpan timePassed = now - lastRewardDate;
			List<int> delays = rewardConfigModel.GetDelays(rewardConfigId);
			if (delays.Count != 1) {
				Debug.LogError(string.Format("RewardConfigId {0} is Advent but has more than one delay or none at all, set to only have one delay value",rewardConfigId));
				return 0;
			}
			
			//Ticks of the delay.
			long delayTicks = rewardConfigModel.GetDelayType(rewardConfigId) == DelayType.Days ? TimeSpan.FromDays(delays[0]).Ticks : TimeSpan.FromHours(delays[0]).Ticks;
			
			long result = timePassed.Ticks / delayTicks;

			return Mathf.Max((int)result,1);
		}



	}
}