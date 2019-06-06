using System;
using System.Collections.Generic;

namespace TabTale
{
	public class CollectMysteryBoxCommand : GameCommand
	{
		[Inject]
		public ItemConfigData mysteryBoxItem { get; set; }

		[Inject]
		public RewardService rewardService { get; set; }

		[Inject]
		public RewardConfigModel rewardConfigModel { get; set; }

		[Inject]
		public RewardItemConfigModel rewardItemConfigModel { get; set; }

		[Inject]
		public InventoryStateModel inventoryStateModel { get; set; }

		[Inject]
		public CollectGameElementsSignal collectGameElementsSignal { get; set; }

		[Inject]
		public MysteryBoxOpenedSignal mysteryBoxOpenedSignal { get; set; }

		private string 				_rewardConfigId;
		private RewardConfigData	_rewardConfigData;


		public override void Execute ()
		{
			logger.Log(Tag, "Execute");

			if(! SetupAndValidateRewardData())
				return;

			// Get the reward & collect it
			var ids = from reward in rewardItemConfigModel.GetRewards(_rewardConfigId) select reward.GetId();
			RewardData rewardData = rewardService.CollectRandomReward(_rewardConfigId,ids);

			// Remove open mystery box from inventory:
			inventoryStateModel.DecreaseQuantity(mysteryBoxItem.id, 1);

			// Pass the reward data along
			var mbData = new MysteryBoxData(rewardData.rewards, rewardData.rewardItemId);
			mysteryBoxOpenedSignal.Dispatch(mbData);

		}

		private bool SetupAndValidateRewardData()
		{
			_rewardConfigId = mysteryBoxItem.itemTypeKey;
			_rewardConfigData = rewardConfigModel.GetById(_rewardConfigId);

			if( ! mysteryBoxItem.itemType.Equals(ItemType.Reward))
			{
				logger.LogError(Tag, "CollectMysteryBox - input item is not of reward type : " + mysteryBoxItem.id);
				return false;
			}

			if(_rewardConfigData == null)
			{
				logger.LogError(Tag, "Cannot find reward with id : " + mysteryBoxItem.itemTypeKey);
				return false;
			}

			if(_rewardConfigData.rewardType != RewardType.MysteryBox)
			{
				logger.LogError(Tag,"Attempted to collect mystery box using a reward of a different type. Reward Id: " + _rewardConfigId);
				return false;
			}

			return true;
		}
	}
}

