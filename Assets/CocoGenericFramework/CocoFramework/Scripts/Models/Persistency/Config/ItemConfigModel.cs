using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;


namespace TabTale
{
	public enum ItemType
	{
		Item,
		Currency,
		Reward,
		PackCost
	}

	public partial class ItemConfigModel : ConfigModel<ItemConfigData>
	{
		public const string NO_ADS_ID = "NoAds";

		[Inject]
		public IAPConfigModel iapConfigModel {get;set;}

		[Inject]
		public CurrencyStateModel currencyStateModel {get;set;}

		#region Getters

		public ItemConfigData GetNoAdsItem ()
		{
			return _configs.FirstOrDefault(x => x.iapConfigId.Equals (NO_ADS_ID)).Clone() as ItemConfigData;
		}

		public IAPConfigData GetNoAdsIAPItem()
		{
			return GetIAPConfigData(GetNoAdsItem().iapConfigId);
		}

		public IAPConfigData GetIAPConfigData(string itemId)
		{
			return iapConfigModel.GetIAP(_configs.FirstOrDefault(x => x.id == itemId).iapConfigId).Clone() as IAPConfigData;
		}

		public ItemConfigData GetItem(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).Clone() as ItemConfigData;
		}

		public ItemConfigData GetItemByIapConfigId(string iapconfigId)
		{
			return _configs.FirstOrDefault(item => item.iapConfigId == iapconfigId).Clone() as ItemConfigData;
			
		}

		public IEnumerable<ItemConfigData> GetItemsByCategory(System.Enum category)
		{
			return _configs.Where(item => item.category == category.ToString()).OrderBy(item => item.index);
		}

		public List<ItemConfigData> GetAllItems()
		{
			return _configs.OrderBy(i => i.index).ToList();
		}
		
		public string GetName(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).name;
		}

		public string GetIAPConfigID(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).iapConfigId;
		}

		public int GetQuantity(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).quantity;
		}

		public string GetCategory(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).category;
		}

		public int GetIndex(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).index;
		}

		[Obsolete("DEPRECATED - Use ItemType Instead",false)]
		public string GetCurrencyKey(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).currencyKey;
		}
			
		public ItemType GetItemType(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).itemType;
		}

		public string GetItemTypeKey(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).itemTypeKey;
		}

		public List<GameElementData> GetCosts(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).costs;
		}

		public List<GameElementData> GetAlternativeCosts(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).alternativeCosts;
		}

		public List<GameElementData> GetRestrictions (string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).restrictions;
		}

		public List<AssetData> GetAssets (string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).assets;
		}

		public AssetData GetAsset(string itemId, string assetId)
		{
			return GetAssets(itemId).FirstOrDefault(x=>x.id == assetId);
		}

		public List<AssetData> GetAssets(string itemId, AssetType assetType)
		{
			return GetAssets(itemId).Where(item=>item.assetType == assetType).ToList();
		}

		public List<GenericPropertyData> GetProperties(string itemId)
		{
			return _configs.FirstOrDefault(item => item.id == itemId).properties;
		}

		#endregion

		public bool IsInAppItem(ItemConfigData item)
		{
			return (! string.IsNullOrEmpty(item.iapConfigId));
		}

		public bool HasAnyCosts(ItemConfigData item)
		{
			return item.costs.Count > 0 || item.alternativeCosts.Count > 0;
		}

		public GameElementData ToGameElement(ItemConfigData item)
		{
			GameElementType eType = GameElementType.Item;
			string key = item.id;

			switch(item.itemType)
			{
			case ItemType.Currency:
				eType = GameElementType.Currency;
				key = item.itemTypeKey;
				break;
			// A Pack cost does not give any game elements, it only significes costs - so we'll give 0 soft currencies
			case ItemType.PackCost:
				return new GameElementData(GameElementType.Currency, CurrenciesType.soft.ToString(), 0);
			}

			GameElementData ge = new GameElementData(eType, key, item.quantity);

			return ge;
		}
	}
}