using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class CollectGameElementsCommand : GameCommand 
	{
		[Inject]
		public List<GameElementData> gameElements { get; set; }

		[Inject]
		public InventoryStateModel inventoryStateModel { get; set; }

		[Inject]
		public CurrencyStateModel currencyStateModel { get; set; }

		[Inject]
		public ProgressStateModel progressStateModel { get; set; }

		[Inject]
		public ItemConfigModel itemConfigModel { get; set; }

		public override void Execute ()
		{
			logger.Log(Tag, "Execute");

			foreach(GameElementData gameElement in gameElements)
			{
				logger.Log(Tag,"<color=green>Collecting game element :</color> " + gameElement);

				switch(gameElement.type)
				{
				case GameElementType.Currency:
					currencyStateModel.IncreaseCurrency(gameElement);
					break;
				case GameElementType.Item: 
					//TODO: Expand to support:
					// 1. Automatic upgrade of items
					// 2. Automatic setting the item to be in use
					ItemConfigData itemData = itemConfigModel.GetItem(gameElement.key);
					if(itemData.itemType == ItemType.Currency)
					{
						currencyStateModel.IncreaseCurrency(itemConfigModel.ToGameElement(itemData));
					}
					else
					{
						inventoryStateModel.IncreaseQuantity(gameElement);
					}
					break;
				case GameElementType.State:
					progressStateModel.IncreaseProgress(gameElement);
					break;
				}
			}
	
		}
	}
}