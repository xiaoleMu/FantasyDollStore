using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;

namespace TabTale
{
	public class RewardedAdResultHandlerCommand : Command
    {
		[Inject]
		public bool rewardedAdWatched { get; set; }
		[Inject]
		public GameElementData reward { get; set; }

        [Inject]
        public InventoryStateModel inventoryStateModel { get; set; }
        [Inject]
        public CurrencyStateModel currencyStateModel { get; set; }

        public override void Execute ()
        {
			Debug.Log ("RewardedAdResultHandlerCommand.Execute - watched ad result:" + rewardedAdWatched);

			if(reward.type == GameElementType.Currency)
			{
				currencyStateModel.IncreaseCurrency(reward);
			}
			else if(reward.type == GameElementType.Item)
			{
				inventoryStateModel.IncreaseQuantity(reward.key, reward.value);
			}
        }
    }
    
}