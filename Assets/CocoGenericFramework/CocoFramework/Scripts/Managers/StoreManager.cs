using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RSG;

namespace TabTale
{
	public class StoreManager : IStoreManager
	{
		[Inject]
		public IAPConfigModel iapConfigModel {get;set;}

		[Inject]
		public ItemConfigModel itemConfigModel {get;set;}

		[Inject]
		public ItemGroupConfigModel itemGroupConfigModel {get;set;}

		[Inject]
		public InventoryStateModel inventoryStateModel {get;set;}
		
		[Inject]
		public CurrencyStateModel currencyStateModel {get;set;}

		[Inject]
		public RewardConfigModel rewardConfigModel {get;set;}
		
		[Inject]
		public RestrictionVerifier restrictionVerifier {get;set;}

		[Inject]
		public IBillingService billingService {get;set;}

		[Inject]
		public RequestPurchaseInAppItemSignal requestPurchaseInAppItemSignal {get;set;}

		[Inject]
		public CollectGameElementsSignal collectGameElementsSignal {get;set;}

		[Inject]
		public IAPPurchaseDoneSignal iapPurchaseDoneSignal {get;set;}

		[Inject]
		public RewardItemBoughtSignal rewardItemBoughtSignal {get;set;}

		[Inject]
		public ILogger logger {get;set;}

		private const string Tag = "StoreManager";

		private bool IsPurchaseInProgress 
		{ 
			get { return billingService.IsPurchaseInProgress; }
		}

		#region IStoreManager implementation

		public IPromise<BuyItemResult> BuyItem(string itemId, CurrencyType currencyType = CurrencyType.AutoDetect)
		{
			var promise = new Promise<BuyItemResult>();

			ItemConfigData itemData = itemConfigModel.GetItem(itemId);

			if(currencyType == CurrencyType.HardCurrency)
			{
				BuyItemByIAP (itemId, promise);
				return promise;
			}

			if(currencyType == CurrencyType.VirtualCurrency)
			{
				BuyItemResult result = BuyItemWithVC(itemData);
				promise.Resolve(result);
				return promise;
			}

			// Auto detect virtual currency or hard currency;

			if(itemConfigModel.HasAnyCosts(itemData))
			{
				BuyItemResult result = BuyItemWithVC(itemData);
				promise.Resolve(result);
			}
			else if(itemConfigModel.IsInAppItem(itemData))
			{
				BuyItemByIAP (itemId, promise);
			}
			else
			{
				Debug.LogError("Attempted to purchase an item with no defined costs");
				promise.Resolve( new BuyItemResult(BuyItemResultCode.Error));
			}

			return promise;
		}

		public IPromise<BuyItemResult> BuyItemGroup(string itemGroupId)
		{
			var promise = new Promise<BuyItemResult>();

			ItemGroupConfigData itemGroupData = itemGroupConfigModel.GetGroup(itemGroupId);
			if(itemGroupData == null)
			{
				logger.LogError(Tag, "Attempted to buy a non existing group with id " + itemGroupId);
				promise.Resolve(new BuyItemResult(BuyItemResultCode.Error));
				return promise;
			}

			switch(itemGroupData.type)
			{
			case ItemGroupConfigType.Pack:
				BuyItemPack(itemGroupData, promise);
				break;
			case ItemGroupConfigType.Upgrades:
				BuyUpgrade(itemGroupData, promise);
				break;
			}

			return promise;
		}

		#endregion

		#region internal

		private BuyItemResult BuyItemWithVC(ItemConfigData itemConfigData)
		{
			var result = new BuyItemResult(BuyItemResultCode.Error);

			if(itemConfigData.costs.Count > 0)
				result = BuyItemByCosts(itemConfigData);
			else if(itemConfigData.alternativeCosts.Count > 0)
				result = BuyItemByAlternativeCosts(itemConfigData);

			return result;
		}
		
		private BuyItemResult BuyItemByCosts(ItemConfigData itemConfigData)
		{
			return BuyItemByCostArray(itemConfigData.id, itemConfigData.costs);
		}
		
		private BuyItemResult BuyItemByAlternativeCosts(ItemConfigData itemConfigData)
		{
			return BuyItemByCostArray(itemConfigData.id, itemConfigData.alternativeCosts);
		}
		
		private void BuyItemByIAP(string itemId, Promise<BuyItemResult> promise)
		{
			ItemConfigData itemData = itemConfigModel.GetItem(itemId);

			IAPConfigData iapData = iapConfigModel.GetIAP(itemData.iapConfigId);
			if(iapData == null)
			{
				logger.LogError(Tag, ".PurchaseItemByIAP - Could not find iap data for item : " + itemData.id);
				promise.Resolve(new BuyItemResult(BuyItemResultCode.Error));
			}

			if( IsPurchaseInProgress )
			{
				logger.Log(Tag, ".PurchaseItemByIAP - Purchase is in progress - ignoring new purchase request for item : " + itemData.id);
				promise.Resolve(new BuyItemResult(BuyItemResultCode.Error));
			}
			else
			{
				iapPurchaseDoneSignal.AddOnce(iapResult => HandleIapResult (iapResult, promise));

				requestPurchaseInAppItemSignal.Dispatch(iapData);
			}
		}

		private void BuyItemPack(ItemGroupConfigData itemGroupData, Promise<BuyItemResult> promise)
		{
			if(itemGroupData.items.Count == 0)
			{
				logger.LogError(Tag, "BuyItemPack - Group contains no items");
				promise.Resolve(new BuyItemResult(BuyItemResultCode.Error));
				return;
			}

			string itemIdToBuy = itemGroupData.items[0];

			// For item groups with more than 
			if(itemGroupData.items.Count > 1)
			{
				ItemConfigData packCost = itemGroupConfigModel.GetPackCost(itemGroupData.id);
				if(packCost == null)
				{
					logger.LogError(Tag, "Cannot find Pack cost - item groups with more than one item require a pack cost item");
					promise.Resolve(new BuyItemResult(BuyItemResultCode.Error));
					return;
				}

				itemIdToBuy = packCost.id;
			}

			// Pay the costs, then give the player the rest of the items in the group
			BuyItem(itemIdToBuy).Done( buyItemResult => {

				if(buyItemResult.resultCode.Equals(BuyItemResultCode.Success))
				{
					foreach(string itemId in itemGroupConfigModel.GetItemIds(itemGroupData.id))
					{
						ItemConfigData item = itemConfigModel.GetItem(itemId);

						if(item.itemType == ItemType.PackCost)
							continue;

						GameElementData ge = itemConfigModel.ToGameElement(item);
						var items = new List<GameElementData> { ge };

						collectGameElementsSignal.Dispatch(items);
					}
				}
				else
				{
					logger.Log(Tag,"BuyItemPack - Will not collect game elements since buy attempt failed. Result code: " + buyItemResult.resultCode);
				}

				promise.Resolve(buyItemResult);
			});
		}


		private void BuyUpgrade(ItemGroupConfigData itemGroupData, Promise<BuyItemResult> promise)
		{
			// Handle upgrade: If not final item in upgrade list, Remove the item from inUse in inventory, add the next item to the inventory and set it inUse

			string currentItemId = itemGroupConfigModel.GetUsedItemId(itemGroupData);
			string nextUpgradeId = itemGroupConfigModel.GetNextUpgradeItemId(currentItemId, itemGroupData.id);

			bool isGroupItemInInventory = ! string.IsNullOrEmpty(currentItemId);
			bool itemHasUpgrade = ! string.IsNullOrEmpty(nextUpgradeId);

			string itemIdToBuy = null;
			if( ! isGroupItemInInventory)
				itemIdToBuy = itemGroupData.items[0];
			else if(itemHasUpgrade)
				itemIdToBuy = nextUpgradeId;
			else
			{
				logger.LogError(Tag, "Cannot upgrade item since - no upgrade available for item in group " + itemGroupData.id);
				promise.Resolve(new BuyItemResult(BuyItemResultCode.Error));
				return;
			}

			BuyItem (itemIdToBuy).Done ( buyItemResult => {

				if(buyItemResult.resultCode.Equals(BuyItemResultCode.Success))
				{
					if(isGroupItemInInventory)
					{
						inventoryStateModel.SetItemUnUsed(currentItemId);
					}

					inventoryStateModel.SetItemInUse(itemIdToBuy);
				}
				else
				{
					logger.Log(Tag,"BuyUpgrade - Will not collect item since buy attempt failed. Result code: " + buyItemResult.resultCode);
				}

				promise.Resolve(buyItemResult);
			});
		}


		private BuyItemResult BuyItemByCostArray(string itemId, IEnumerable<GameElementData> costs)
		{
			if(! restrictionVerifier.VerifyRestrictions(itemConfigModel.GetRestrictions(itemId)))
			{
				Debug.Log("StoreManager.BuyItemByCostArray - Restrictions not met for item - " + itemId);
				return new BuyItemResult(BuyItemResultCode.RestrictionsNotMet);
			}
			else if(! currencyStateModel.SufficientCurrency(costs))
			{
				Debug.Log("StoreManager.BuyItemByCostArray - Insufficient currency for item - " + itemId);
				return new BuyItemResult(BuyItemResultCode.InsufficientCurrency);
			}

			currencyStateModel.DecreaseCurrency (costs);

			ItemConfigData itemData = itemConfigModel.GetItem(itemId);

			GameElementData ge = itemConfigModel.ToGameElement(itemData);
			var items = new List<GameElementData> { ge };

			collectGameElementsSignal.Dispatch(items);

			if(itemData.itemType == ItemType.Reward)
			{
				rewardItemBoughtSignal.Dispatch(itemData);
			}

			return new BuyItemResult(BuyItemResultCode.Success);
		}

		public bool AnyItemAvailableToBuy(CurrencyType currencyType = CurrencyType.AutoDetect)
		{
			var items = itemConfigModel.GetAllItems();

			var itemIds = items.Select(item => item.id);

			// Remove items that are in inventory:
			inventoryStateModel.GetState().items.ForEach(item => {
				if(itemIds.Contains(item.itemConfigId))
				{
					items.RemoveAt(items.FirstIndex(i => i.id == item.itemConfigId));
				}
			});

			if(currencyType != CurrencyType.AutoDetect)
			{
				// Remove items that are not of the relevant currency type
				items.RemoveAll(item => item.costs.Any(cost => cost.key != currencyType.ToString()) ||
					item.alternativeCosts.Any(alterCost => alterCost.key != currencyType.ToString()));
			}

			// If any item has costs and there's enough VC to buy it - then it's available to purchase
			if(items.Where (itemConfigModel.HasAnyCosts).Any (HasSufficientCurrencyForItem))
			{
				return true;
			}
				

			return false;
		}

		#endregion

		private bool HasSufficientCurrencyForItem(ItemConfigData item)
		{
			if(item.costs.Count > 0 && currencyStateModel.SufficientCurrency(item.costs) ||
				item.alternativeCosts.Count > 0 && currencyStateModel.SufficientCurrency(item.alternativeCosts))
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		#region Handle In App Purchases

		private void HandleIapResult(PurchaseIAPResult iapResult, Promise<BuyItemResult> promise)
		{
			BuyItemResultCode resultCode = (iapResult.result == PurchaseIAPResultCode.Success) ? BuyItemResultCode.Success : BuyItemResultCode.InAppPurchaseFailed;
			var buyItemResult = new BuyItemResult(resultCode, iapResult);

			promise.Resolve(buyItemResult);
		}

		#endregion
	}
}