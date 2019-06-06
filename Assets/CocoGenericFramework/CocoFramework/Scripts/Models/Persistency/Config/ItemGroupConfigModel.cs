using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale
{
	public partial class ItemGroupConfigModel : ConfigModel<ItemGroupConfigData>
	{
		[Inject]
		public ItemConfigModel itemConfigModel { get; set; }

		[Inject]
		public InventoryStateModel inventoryStateModel { get; set; }

		#region Getters

		public ItemGroupConfigData GetGroup (string id)
		{
			return _configs.FirstOrDefault (group => group.id == id).Clone () as ItemGroupConfigData;
		}

		public ItemGroupConfigData GetGroupByName (string name)
		{
			return _configs.FirstOrDefault (group => group.name == name).Clone () as ItemGroupConfigData;
		}

		public ItemGroupConfigData GetGroupByCategory (System.Enum category)
		{
			return _configs.FirstOrDefault (group => group.category == category.ToString());
		}

		public List<ItemGroupConfigData> GetAllGroups()
		{
			return _configs.OrderBy(g => g.index).ToList();
		}

		public int GetIndex (string id)
		{
			return _configs.FirstOrDefault (group => group.GetId () == id).index;
		}

		public ItemGroupConfigType GetItemGroupType (string id)
		{
			return _configs.FirstOrDefault (group => group.GetId () == id).type;
		}

		public List<string> GetItemIds (string id)
		{
			return _configs.FirstOrDefault (group => group.GetId () == id).items;
		}

		public List<GameElementData> GetRestrictions (string id)
		{
			return _configs.FirstOrDefault (group => group.GetId () == id).restrictions;
		}

		public List<AssetData> GetAssets (string id)
		{
			return _configs.FirstOrDefault (group => group.GetId () == id).assets;
		}

		#endregion

		#region Upgrades 

		public string GetUsedItemId(string groupId)
		{
			return GetUsedItemId(GetGroup(groupId));
		}

		public string GetUsedItemId(ItemGroupConfigData itemGroup)
		{
			if(itemGroup.type != ItemGroupConfigType.Upgrades)
			{
				Debug.LogError("Attempted to get next item upgrade for an item group of type: " + itemGroup.type);
				return string.Empty;
			}

			foreach(var itemData in inventoryStateModel.GetItemsInUse())
			{
				int index = itemGroup.items.FindIndex(itemId => itemId.Equals(itemData.itemConfigId));

				if(index >= 0)
					return itemGroup.items[index];
			}

			return string.Empty;
		}

		public string GetNextUpgradeItemId(string itemId, string groupId)
		{
			ItemGroupConfigData itemGroup = GetGroup(groupId);

			// Sanity check - verify the specified group is of an Upgrades type:
			if(itemGroup.type != ItemGroupConfigType.Upgrades)
			{
				Debug.LogError("Attempted to get next item upgrade for an item group of type: " + itemGroup.type);
				return "";
			}

			return itemGroup.items.SkipWhile(i => i != itemId).Skip(1).FirstOrDefault();
		}

		#endregion

		#region
		public ItemConfigData GetPackCost(string groupId)
		{
			ItemGroupConfigData itemGroup = GetGroup(groupId);

			if(itemGroup.type != ItemGroupConfigType.Pack)
			{
				Debug.LogError("Attempted to get cost for an item group type that is not a pack: " + itemGroup.type);
				return null;
			}

			string packCostItemId = itemGroup.items.FirstOrDefault(i => itemConfigModel.GetItemType(i) == ItemType.PackCost);
			if(string.IsNullOrEmpty(packCostItemId))
			{
				Debug.LogError("Cannot find a pack cost item for item group with id " + groupId);
				return null;
			}

			return itemConfigModel.GetItem(packCostItemId);
		}


		#endregion
	}
}
