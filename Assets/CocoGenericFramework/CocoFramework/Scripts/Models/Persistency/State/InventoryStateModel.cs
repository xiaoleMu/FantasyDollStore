using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public class InventoryStateModel : StateModel<InventoryStateData>
	{
		[Inject]
		public ItemConfigModel itemConfigModel { get; set; }

		#region General

		public int ItemsCount ()
		{
			return _data.items.Count;
		}

		#endregion

		#region Getters

		private int GetIndex(ItemData itemData)
		{
			return _data.items.FirstIndex(item => item.itemConfigId == itemData.itemConfigId);
		}
		
		private int GetIndex(string itemConfigId)
		{
			return _data.items.FirstIndex(item => item.itemConfigId == itemConfigId);
		}

		public ItemData GetItem (string itemConfigId)
		{
			int index = GetIndex(itemConfigId);
			if ( index != -1) 
			{
				return _data.items [index].Clone () as ItemData;
			}
			return null;
		}

		public List<ItemData> GetItemsByCategory (System.Enum category)
		{
			var result = from item in _data.items where item.category == category.ToString() select item;
			return result.ToList();
		}

		#endregion

		#region Item In Use

		public bool HasItem(string itemConfigId)
		{
			return GetIndex(itemConfigId) != -1;
		}

		public bool ItemInUse (string itemConfigId)
		{
			int index = GetIndex(itemConfigId);
			if ( index != -1) 
			{
				return _data.items[index].inUse;
			}

			return false;
		}

		public bool SetItemInUse (string itemConfigId)
		{
			return SetItemUsability(itemConfigId, true);
		}

		public bool SetItemUnUsed (string itemConfigId)
		{
			return SetItemUsability(itemConfigId, false);
		}

		private bool SetItemUsability (string itemConfigId, bool isUsable)
		{
			int index = GetIndex(itemConfigId);
			if ( index != -1) 
			{
				_data.items [index].inUse = isUsable;
				return Save ();
			}
			
			return false;
		}

		public List<ItemData> GetItemsInUse ()
		{
			var result = from i in _data.items where i.inUse select i; 
			return result.ToList();
		}

		public bool SetAllItemsUnUsed ()
		{
			for (int i = 0; i < _data.items.Count ;i++) 
			{
				_data.items[i].inUse = false;
			}

			return Save ();
		}

		#endregion

		#region Item Quantity

		public int GetQuantity(string itemConfigId)
		{
			if (!HasItem(itemConfigId)) 
			{
				return 0;
			}
			return GetItem(itemConfigId).quantity;
		}

		public bool IncreaseQuantity (GameElementData gameElement)
		{
			return IncreaseQuantity(gameElement.key, gameElement.value);
		}

		public bool IncreaseQuantity (string itemConfigId, int amount)
		{
			int index = GetIndex(itemConfigId);
			if ( index != -1) 
			{
				_data.items [index].quantity += amount;
			} 
			else 
			{	
				ItemData newItem = new ItemData ();
				newItem.itemConfigId = itemConfigId;
				newItem.inUse = false;
				newItem.quantity = amount;

				ItemConfigData itemConfigData = itemConfigModel.GetItem(itemConfigId);
				if(itemConfigData == null)
				{
					Debug.LogError("Attempted to access non existing item config");
					return false;
				}
				newItem.category = itemConfigData.category;

				_data.items.Add(newItem);
			}
			return Save ();
		}

		public bool SetQuantity(string itemConfigId, int amount)
		{
			int index = GetIndex(itemConfigId);
			if ( index != -1) 
			{
				_data.items [index].quantity = amount;
			} 
			else 
			{
				ItemData newItem = new ItemData ();
				newItem.itemConfigId = itemConfigId;
				newItem.inUse = false;
				newItem.quantity = amount;

				ItemConfigData itemConfigData = itemConfigModel.GetItem(itemConfigId);
				if(itemConfigData == null)
				{
					Debug.LogError("Attempted to access non existing item config");
					return false;
				}
				newItem.category = itemConfigData.category;
				
				_data.items.Add(newItem);
			}
			return Save ();
		}

		public bool DecreaseQuantity (string itemConfigId, int amount)
		{
			int index = GetIndex(itemConfigId);
			if ( index != -1) 
			{
				if (_data.items[index].quantity >= amount) 
				{
					
					_data.items [index].quantity -= amount;
				}
				else 
				{
					Debug.LogWarning(string.Format("Tried to decrease {0} by {1} when only {2} are available",itemConfigId,amount,_data.items[index].quantity));
				}
			}
			return Save ();
		}

		#endregion

		#region Restrictions

		public bool VerifyRestriction(GameElementData restriction)
		{
			return restriction.type == GameElementType.Item  && GetQuantity(restriction.key) >= restriction.value;
		}
	
		#endregion
	}
}