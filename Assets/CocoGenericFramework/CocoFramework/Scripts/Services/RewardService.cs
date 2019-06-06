using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TabTale
{
	public class RewardData
	{
		public List<GameElementData> rewards;
		public string rewardItemId;

		public RewardData(List<GameElementData> rewards, string rewardItemId)
		{
			this.rewards = rewards;
			this.rewardItemId = rewardItemId;
		}

		public RewardData() { }
	}

	public class RewardService
	{
		[Inject]
		public RewardStateModel rewardStateModel {get;set;}

		[Inject]
		public RewardConfigModel rewardConfigModel {get;set;}

		[Inject]
		public RewardItemConfigModel rewardItemConfigModel {get;set;}

		[Inject]
		public InventoryStateModel inventoryStateModel {get;set;}

		[Inject]
		public CurrencyStateModel currencyStateModel {get;set;}

		[Inject]
		public ProgressStateModel progressStateModel {get;set;}

		[Inject]
		public ServerTime serverTime {get;set;}

		[Inject]
		public ISocialService socialService {get;set;}

		[Inject]
		public CollectGameElementsSignal collectGameElementsSignal {get;set;}

		[Inject]
		public ILogger logger {get;set;}

		private Dictionary<string,bool> rvFactors = new Dictionary<string, bool>();
		private bool socialFactor;

		private string Tag = "RewardService";


		/// <summary>
		/// Check is the player is eligble to receive a reward from the specified rewardConfigId
		/// </summary>
		/// <returns><c>true</c>, if eligable, <c>false</c> otherwise.</returns>
		/// <param name="rewardConfigId">Reward config identifier.</param>
		public bool EligbleForReward(string rewardConfigId)
		{
			return VerifyDelayPassed(rewardConfigId) && VerifyRestrictions(rewardConfigId);
		}


		/// <summary>
		/// Collects the reward for specified rewardConfigId, if it eligable
		/// </summary>
		/// <returns>The rewards that were claimed</returns>
		/// <param name="rewardConfigId">Reward config identifier.</param>
		public List<GameElementData> CollectReward(string rewardConfigId)
		{
			if (!EligbleForReward(rewardConfigId)) 
			{
				return new List<GameElementData>();
			}

			RewardItemConfigData rewardClaimed = rewardStateModel.NextReward(rewardConfigId);

			if (TimeForConsecutiveRewardPassed(rewardConfigId)) 
			{
				rewardStateModel.ResetRewards(rewardConfigId);
				rewardClaimed = rewardStateModel.NextReward(rewardConfigId);
			}

			List<GameElementData> rewards = ClaimItemConfigId(rewardClaimed.id);

			rewardStateModel.ClaimReward(rewardConfigId,rewardClaimed.id);
			return rewards;
		}
			
	
		/// <summary>
		/// Gets a random reward from all the available rewards in rewardConfigId, by thier weight.
		/// </summary>
		/// <returns>The rewards that were claimed</returns>
		/// <param name="rewardConfigId">Reward config identifier.</param>
		public List<GameElementData> CollectRandomReward(string rewardConfigId)
		{
			var ids = from reward in rewardItemConfigModel.GetRewards(rewardConfigId) select reward.GetId();
			return CollectRandomReward(rewardConfigId,ids).rewards;
		}

		public List<GameElementData> CollectImmediateRandomReward(string rewardConfigId)
		{
			var ids = from reward in rewardItemConfigModel.GetRewards(rewardConfigId) select reward.GetId();
			return CollectRandomReward(rewardConfigId,ids,false).rewards;
		}
		/// <summary>
		/// Gets a random reward from a subset of the available rewards in rewardConfigId, by thier weight.
		/// The subset is chosen with from all the rewards with uniform distribution.
		/// </summary>
		/// <returns>The rewards that were claimed.</returns>
		/// <param name="rewardConfigId">Reward config identifier.</param>
		/// <param name="subsetSize">Subset size.</param>
		public RewardData CollectRandomReward(string rewardConfigId, int subsetSize)
		{
			List<string> subset = new List<string>();

			List<RewardItemConfigData> possibleRewards = rewardItemConfigModel.GetRewards(rewardConfigId);

			for (int i = 0; i < subsetSize; i++) {
				int selected = UnityEngine.Random.Range(0,possibleRewards.Count);
				subset.Add(possibleRewards[selected].GetId());
				possibleRewards.RemoveAt(selected);
			}

			return CollectRandomReward(rewardConfigId,subset);

		}

		/// <summary>
		/// Gets a random reward from a subset of the available rewards in rewardConfigId, by thier weight.
		/// The subset is specified by a list of the reward indexes.
		/// </summary>
		/// <returns>The rewards that were claimed.</returns>
		/// <param name="rewardConfigId">Reward config identifier.</param>
		/// <param name="subset">A list of RewardItemConfigIds that are the specified subset.</param>
		public RewardData CollectRandomReward(string rewardConfigId, IEnumerable<string> subset, bool checkIfEligible = true)
		{
			if (checkIfEligible && !EligbleForReward(rewardConfigId))
			{
				return new RewardData();
			}

			//Create subset of itemData to get weights.
			var rewardSubset = from rewardItem in rewardItemConfigModel.GetRewards(rewardConfigId)
				join rewardSubsetItem in subset on rewardItem.id equals rewardSubsetItem
					select rewardItem;


			float normalizingFactor = WeightNormalizingFactor(rewardSubset);

			float selected = UnityEngine.Random.value;
			float weightSum = 0;


			//The weights are normalized so the sum will be equal to one.
			foreach (var item in rewardSubset) 
			{
				weightSum += item.weight * normalizingFactor;

				if (selected <= weightSum ) 
				{
					List<GameElementData> claimed = ClaimItemConfigId(item.GetId());
					rewardStateModel.ClaimReward(rewardConfigId,item.GetId());
					return new RewardData(claimed, item.GetId());
				}
			}
			
			return new RewardData();
		}

		/// <summary>
		/// Get an ordered list of all possible rewards in the current prize, sorted by their index.
		/// </summary>
		/// <returns>A list of all possible rewards </returns>
		/// <param name="rewardConfigId">Identifier.</param>
		public List<RewardItemConfigData> RewardsList(string rewardConfigId)
		{
			return rewardItemConfigModel.GetRewards(rewardConfigId);
		}

		/// <summary>
		/// Resets the state of the reward system for a specific id.
		/// So it will act as though its the first day of the reward cycle.
		/// </summary>
		/// <param name="rewardConfigId">Id of rewardConfig to be reset.</param>
		public void ResetState(string rewardConfigId)
		{
			rewardStateModel.ResetRewards(rewardConfigId);
		}

		/// <summary>
		/// Activates social factor in the specified rewardConfigId
		/// </summary>
		/// <param name="id">the rewardConfigId for which the socialFactor will be activated.</param>
		public void ActivateSocialFactor(string rewardConfigId)
		{
			socialFactor = true;
		}

		/// <summary>
		/// Activates the RV factor.
		/// </summary>
		/// <param name="rewardConfigId">Reward config identifier.</param>
		public void ActivateRVFactor(string rewardConfigId)
		{
			rvFactors[rewardConfigId] = true;
		}

		/// <summary>
		/// For Consecutive rewards only:
		/// Check current time to see if more than 1 day passed since last reward was given, if so then reset the reward state
		/// Can be used to update UI items in advance
		/// </summary>
		/// <param name="rewardConfigId">Reward config identifier.</param>
		public void UpdateConsecutiveStatus(string rewardConfigId)
		{
			if (TimeForConsecutiveRewardPassed(rewardConfigId)) 
			{
				rewardStateModel.ResetRewards(rewardConfigId);
			}
		}

		private bool TimeForConsecutiveRewardPassed(string rewardConfigId)
		{
			if (rewardConfigModel.GetRewardType(rewardConfigId) != RewardType.Consecutive)
			{
				return false;
			}

			RewardItemConfigData nextReward = rewardStateModel.NextReward(rewardConfigId);
			List<int> delays = rewardConfigModel.GetDelays(rewardConfigId);
			int delay = delays[nextReward.index % delays.Count];
			
			DateTime todayStart = serverTime.GetLocalTime();
			DateTime lastReward = serverTime.ToLocal(rewardStateModel.GetLastRewardedDate(rewardConfigId));

			todayStart = serverTime.GetDateStart (todayStart);
			lastReward = serverTime.GetDateStart(lastReward);

			//true if more than <delay> days have passed. (because we set the hours to be the same, it will only be in multiples of 24)
			return todayStart - lastReward > TimeSpan.FromHours(delay*24);

		}

		private bool VerifyDelayPassed(string rewardConfigId)
		{
			RewardItemConfigData nextReward = rewardStateModel.NextReward(rewardConfigId);
			List<int> delays = rewardConfigModel.GetDelays(rewardConfigId);

			if(delays.Count == 0)
				return true;
			
			int delay = delays[nextReward.index % delays.Count];
			
			DateTime todayStart = serverTime.GetLocalTime();
			DateTime lastReward = serverTime.ToLocal(rewardStateModel.GetLastRewardedDate(rewardConfigId));
           
            switch (rewardConfigModel.GetDelayType(rewardConfigId)) 
			{

			case DelayType.Days:
				//todayStart = serverTime.GetDateStart (todayStart);
				//lastReward = serverTime.GetDateStart(lastReward);
				return todayStart - lastReward >= TimeSpan.FromHours(24*delay);
				
			case DelayType.Hours:
				return todayStart - lastReward >= TimeSpan.FromHours(delay);

            case DelayType.Minutes:
                return todayStart - lastReward >= TimeSpan.FromMinutes(delay);
			
			case DelayType.None:
				return true;

                default:
                   
				return false; //or true?
			}
           

        }


		private bool VerifyRestrictions(string rewardConfigId)
		{
			RestrictionVerifier r = new RestrictionVerifier();
			List<GameElementData> restrictions = rewardConfigModel.GetRestrictions(rewardConfigId);
			return r.VerifyRestrictions(restrictions);
		}

		private float WeightNormalizingFactor(IEnumerable<RewardItemConfigData> rewards)
		{
			float weightSum = rewards.Sum(r => r.weight);
			return 1/weightSum;
		}

		List<GameElementData> ClaimItemConfigId (string rewardItemConfigId)
		{
			RewardItemConfigData rewardItem = rewardItemConfigModel.GetItemById(rewardItemConfigId);
			List<GameElementData> rewards = rewardItemConfigModel.GetRewardsForItem(rewardItemConfigId);

			bool applyRvFactor = rvFactors.ContainsKey(rewardItem.rewardConfigId) ? rvFactors[rewardItem.rewardConfigId] : false;
			applyRvFactor = applyRvFactor && rewardItem.allowRVFactor;

			RewardConfigData rewardConfig = rewardConfigModel.GetById(rewardItem.rewardConfigId);

			var items = new List<GameElementData>();
				
			//increase items by factors.
			foreach (var item in rewards) 
			{
				if (applyRvFactor) 
				{
					item.value = Mathf.RoundToInt(item.value * rewardConfig.rvFactor);
				}

				if (rewardItem.allowSocialFactor && socialFactor && (!applyRvFactor || (applyRvFactor && rewardConfig.doubleFactor) )) 
				{
					item.value = Mathf.RoundToInt(item.value * rewardConfig.socialFactor);
				}

				if(rewardItem.rangeMultiplayer > 1.0f)
				{
					float randomMultiplayer = UnityEngine.Random.Range(1.0f, rewardItem.rangeMultiplayer);
					Debug.Log("RewardService - Applying random range multiplier : " + randomMultiplayer);
				}
					
				items.Add(item);
			}

			collectGameElementsSignal.Dispatch(items);

			//reset rvFactor.
			rvFactors[rewardItem.rewardConfigId] = false;

			return rewards;

		}
	}
}