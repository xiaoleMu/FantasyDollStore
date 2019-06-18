using System;
using System.Collections.Generic;
using Game;
using UnityEngine.Purchasing;
using CocoPlay.Store;

namespace CocoPlay
{
	public class CocoStoreConfigData : ICocoStoreConfigData
	{
		#region Init

		[PostConstruct]
		public void Init ()
		{
			LoadItemDatas ();
			InitItemMaps ();

			// fill slots from GetSlot method for backward compatibility, will remove in future
#pragma warning disable 618
			InitItemSlots ();
 #pragma warning restore 618

			InitItemParents ();
		}

		private void LoadItemDatas ()
		{
			var storeData = CocoStoreData.Instance;
			if (storeData == null || storeData.allProductItems == null) {
				return;
			}

			foreach (var storeItem in storeData.allProductItems) {
				//for backward compatibility, will remove in future
				if (_itemDatas.ContainsKey (storeItem.productId)) {
					var itemData = _itemDatas [storeItem.productId];
					if (!string.IsNullOrEmpty (storeItem.itemIdString)) {
						itemData.Key = storeItem.itemIdString;
					}
					if (storeItem.relatedStoreItems.Count > 0) {
						itemData.SlotIds = storeItem.relatedStoreItems;
					}
					continue;
				}

				_itemDatas [storeItem.productId] = new CocoStoreItemData {
					Id = storeItem.productId,
					Key = string.IsNullOrEmpty (storeItem.itemIdString) ? storeItem.iosIapString : storeItem.itemIdString,
					SlotIds = storeItem.relatedStoreItems
				};
			}
		}

		private void InitItemMaps ()
		{
			foreach (var itemData in _itemDatas.Values) {
				if (!_itemKeyMaps.ContainsKey (itemData.Key)) {
					_itemKeyMaps.Add (itemData.Key, itemData.Id);
				}
			}
		}

		private void InitItemParents ()
		{
			foreach (var itemData in _itemDatas.Values) {
				if (itemData.SlotIds == null) {
					continue;
				}

				foreach (var slotId in itemData.SlotIds) {
					var slotItemData = GetItemData (slotId);
					if (slotItemData != null) {
						slotItemData.ParentIds.Add (itemData.Id);
					}
				}
			}
		}

		#endregion


		#region Items

		private readonly Dictionary<CocoStoreID, CocoStoreItemData> _itemDatas = new Dictionary<CocoStoreID, CocoStoreItemData> ();

		private readonly Dictionary<string, CocoStoreID> _itemKeyMaps = new Dictionary<string, CocoStoreID> ();

		public List<CocoStoreItemData> ItemDatas {
			get { return new List<CocoStoreItemData> (_itemDatas.Values); }
		}

		public CocoStoreItemData GetItemData (CocoStoreID itemId)
		{
			return _itemDatas.ContainsKey (itemId) ? _itemDatas [itemId] : null;
		}

		public CocoStoreItemData GetItemData (string itemKey)
		{
			if (!_itemKeyMaps.ContainsKey (itemKey)) {
				return null;
			}

			var itemId = _itemKeyMaps [itemKey];
			return GetItemData (itemId);
		}

		public IEnumerable<CocoStoreID> ItemIds {
			get { return _itemDatas.Keys; }
		}

		#endregion


		#region Backward compatibility

		[Obsolete ("for backward compatibility, will remove in future. please use 'CocoStoreData' config instead.")]
		protected void AddItem (CocoStoreID id, string key)
		{
			if (_itemDatas.ContainsKey (id)) {
				_itemDatas [id].Key = key;
				return;
			}

			var itemData = new CocoStoreItemData {
				Id = id,
				Key = key
			};
			_itemDatas.Add (id, itemData);
		}

		[Obsolete ("for backward compatibility, will remove in future. please use 'GetItemData' instead.")]
		public string GetStoreKey (CocoStoreID id)
		{
			return _itemDatas.ContainsKey (id) ? _itemDatas [id].Key : string.Empty;
		}

		[Obsolete ("for backward compatibility, will remove in future. please use 'ItemDatas' instead.")]
		public List<string> GetStoreKeyList ()
		{
			var keys = new List<string> (_itemDatas.Count);
			foreach (var itemData in _itemDatas.Values) {
				keys.Add (itemData.Key);
			}
			return keys;
		}

		[Obsolete ("for backward compatibility, will remove in future. please use 'ItemDatas' instead.")]
		public virtual Dictionary<CocoStoreID, List<CocoStoreID>> GetSlot ()
		{
			var slots = new Dictionary<CocoStoreID, List<CocoStoreID>> (_itemDatas.Count);
			foreach (var itemData in _itemDatas.Values) {
				slots.Add (itemData.Id, itemData.SlotIds);
			}
			return slots;
		}

		[Obsolete ("for backward compatibility, will remove in future. please use 'ItemDatas' instead.")]
		protected Dictionary<CocoStoreID, string> m_ConfigData {
			get {
				var itemIdMaps = new Dictionary<CocoStoreID, string> (_itemDatas.Count);
				foreach (var itemData in _itemDatas) {
					itemIdMaps.Add (itemData.Key, itemData.Value.Key);
				}
				return itemIdMaps;
			}
		}

		/// <summary>
		/// fill slots from GetSlot method for backward compatibility
		/// </summary>
		[Obsolete ("for backward compatibility, will remove in future.")]
		private void InitItemSlots ()
		{
			var slots = GetSlot ();
			foreach (var slot in slots) {
				var itemData = GetItemData (slot.Key);
				if (itemData == null) {
					continue;
				}

				foreach (var slotId in slot.Value) {
					if (!itemData.SlotIds.Contains (slotId)) {
						itemData.SlotIds.Add (slotId);
					}
				}
			}
		}

		#endregion
	}


	public class CocoStoreItemData
	{
		public CocoStoreID Id;
		public string Key;
		public ProductType ProductType = ProductType.NonConsumable;

		public List<CocoStoreID> SlotIds = new List<CocoStoreID> ();
		public readonly HashSet<CocoStoreID> ParentIds = new HashSet<CocoStoreID> ();


		public bool IsPurchased = false;
	}
}