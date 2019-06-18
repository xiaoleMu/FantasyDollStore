using UnityEngine;
using System.Collections.Generic;
using Game;
using TabTale;


namespace CocoPlay
{
	public class CocoRoleDressData
	{
		Dictionary<string, CocoDressItemHolder> m_ItemHolderDic = null;

		public CocoRoleDressData (Dictionary<string, CocoDressItemHolder> itemHolderDic)
		{
			m_ItemHolderDic = itemHolderDic;
		}


		#region Curr Dress

		// dress
		Dictionary<string, CocoDressItemModelSet> m_CurrDressModelSetDic = new Dictionary<string, CocoDressItemModelSet> ();

		public void AddDress (CocoDressItemModelSet modelSet)
		{
			m_CurrDressModelSetDic.Add (modelSet.ItemHolder.id, modelSet);
			CheckChangeForAddItem (modelSet);
		}

		public void RemoveDress (string itemid)
		{
			CocoDressItemModelSet modelSet = m_CurrDressModelSetDic.GetValue (itemid);
			if (modelSet == null) {
				return;
			}

			CheckChangeForRemoveItem (modelSet);
			m_CurrDressModelSetDic.Remove (itemid);
		}

		public CocoDressItemModelSet GetDress (string itemId)
		{
			return m_CurrDressModelSetDic.GetValue (itemId);
		}

		public bool ItemIsDressed (string itemId)
		{
			return m_CurrDressModelSetDic.ContainsKey (itemId);
		}

		public List<string> GetDressIdsByCategory (string categoryId)
		{
			List<string> dressIds = new List<string> ();

			m_CurrDressModelSetDic.ForEach (modelSet => {
				if (modelSet.ItemHolder.ParentHolder.id == categoryId) {
					dressIds.Add (modelSet.ItemHolder.id);
				}
			});

			return dressIds;
		}

		public List<CocoDressItemModelSet> GetDressItemsByCategory (string categoryId)
		{
			List<CocoDressItemModelSet> dressItems = new List<CocoDressItemModelSet> ();

			m_CurrDressModelSetDic.ForEach (modelSet => {
				if (modelSet.ItemHolder.ParentHolder.id == categoryId) {
					dressItems.Add (modelSet);
				}
			});

			return dressItems;
		}

		public List<string> GetAllDressIds ()
		{
			return new List<string> (m_CurrDressModelSetDic.Keys);
		}

		public List<CocoDressItemModelSet> GetAllDressItems ()
		{
			return new List<CocoDressItemModelSet> (m_CurrDressModelSetDic.Values);
		}

		public List<string> GetAllNonMainItemIds ()
		{
			List<string> itemIds = new List<string> ();

			m_CurrDressModelSetDic.ForEach ((itemId, modelSet) => {
				CocoDressCategoryHolder categoryHolder = modelSet.ItemHolder.ParentHolder as CocoDressCategoryHolder;
				if (categoryHolder != null && !categoryHolder.isMain) {
					itemIds.Add (itemId);
				}
			});

			return itemIds;
		}

		#endregion


		#region Basic

		string m_BoneItemId = null;

		public string BoneItemId {
			get { return m_BoneItemId; }
		}

		List<string> m_BasicItemIds;

		public List<string> BasicItemIds {
			get { return m_BasicItemIds; }
		}

		/// dictionary <cover layer id, item id>
		Dictionary<CocoDressCoverLayer, CocoDressItemHolder> m_LastCoverItemHolderDic = new Dictionary<CocoDressCoverLayer, CocoDressItemHolder> ();

		List<CocoDressCoverLayer> m_SortedLastCovers = new List<CocoDressCoverLayer> ();

		Dictionary<string, SkinnedMeshRenderer> m_RendererDic = new Dictionary<string, SkinnedMeshRenderer> ();

		public void InitBasicCoverItemDic (string boneItemId, List<string> basicItemIds)
		{
			m_BoneItemId = boneItemId;
			m_BasicItemIds = basicItemIds;

			m_LastCoverItemHolderDic.Clear ();
			m_SortedLastCovers.Clear ();
		}

		public void InitRendererDic (List<CocoDressItemModelSet> modelSets)
		{
			m_RendererDic.Clear ();

			modelSets.ForEach (modelSet => {
				modelSet.ItemRenderers.ForEach (smr => {
					if (!m_RendererDic.ContainsKey (smr.name)) {
						m_RendererDic.Add (smr.name, smr);
					}
				});
			});
		}

		public SkinnedMeshRenderer GetRenderer (string rendererName)
		{
			return m_RendererDic.GetValue (rendererName);
		}

		void CheckChangeForAddItem (CocoDressItemModelSet modelSet)
		{
			modelSet.ItemRenderers.ForEach (smr => {
				if (m_RendererDic.ContainsKey (smr.name)) {
					m_RendererDic [smr.name] = smr;
				} else {
					m_RendererDic.Add (smr.name, smr);
				}
			});
		}

		void CheckChangeForRemoveItem (CocoDressItemModelSet modelSet)
		{
			modelSet.ItemRenderers.ForEach (smr => {
				if (m_RendererDic.ContainsKey (smr.name)) {
					m_RendererDic.Remove (smr.name);
				}
			});

			CocoDressItemHolder itemHolder = modelSet.ItemHolder;
			CocoDressCategoryHolder categoryHolder = (CocoDressCategoryHolder)itemHolder.ParentHolder;
			if (!categoryHolder.isMain) {
				return;
			}

			// record last item, in order to resume it
			var layer = itemHolder.CoverLayer;
			if (m_LastCoverItemHolderDic.ContainsKey (layer)) {
				m_LastCoverItemHolderDic [layer] = itemHolder;
				int index = m_SortedLastCovers.IndexOf (layer);
				for (int i = index + 1; i < m_SortedLastCovers.Count; i++) {
					m_SortedLastCovers [i - 1] = m_SortedLastCovers [i];
				}
				m_SortedLastCovers [m_SortedLastCovers.Count - 1] = layer;
				//Debug.LogError ("update " + layer + " -> " + itemHolder.id);
			} else {
				m_LastCoverItemHolderDic.Add (layer, itemHolder);
				//Debug.LogError ("add " + layer + " -> " + itemHolder.id);
				m_SortedLastCovers.Add (layer);
			}
		}

		#endregion


		#region Conflict Process

		public List<string> PrepareAddDressItems (List<string> itemIds, out List<CocoDressItemConflictData> conflictDatas)
		{
			// check conflict
			Dictionary<string, CocoDressItemConflictData> itemConflictDataDic = new Dictionary<string, CocoDressItemConflictData> ();
			itemIds.ForEach (itemId => {
				CocoDressItemHolder itemHolder = m_ItemHolderDic.GetValue (itemId);
				if (itemHolder != null) {
					List<CocoDressItemConflictData> itemConflictDatas = GetDressConflictDatas (itemHolder);
					itemConflictDatas.ForEach (conflictData => {
						if (!itemConflictDataDic.ContainsKey (conflictData.itemHolder.id)) {
							itemConflictDataDic.Add (conflictData.itemHolder.id, conflictData);
						}
					});
				} else {
					Debug.LogErrorFormat ("{0}->PrepareAddDressItems: item [{1}] NOT exists in asset config !", GetType ().Name, itemId);
				}
			});

			// find should added items
			List<string> shouldAddedItemIds = new List<string> ();

			// curr items
			List<string> currItemIds = new List<string> ();
			m_CurrDressModelSetDic.Keys.ForEach (itemId => {
				if (!itemConflictDataDic.ContainsKey (itemId)) {
					currItemIds.Add (itemId);
				}
			});

			// try add expected items
			for (int i = itemIds.Count - 1; i >= 0; i--) {
				CocoDressItemHolder expectedItemHolder = m_ItemHolderDic.GetValue (itemIds [i]);
				TryFillUpDressItem (expectedItemHolder, shouldAddedItemIds, currItemIds);
			}

			// try add last items
			for (int i = m_SortedLastCovers.Count - 1; i >= 0; i--) {
				CocoDressItemHolder lastItemHolder = m_LastCoverItemHolderDic [m_SortedLastCovers [i]];
				TryFillUpDressItem (lastItemHolder, shouldAddedItemIds, currItemIds);
			}

			// try add basic items
			for (int i = m_BasicItemIds.Count - 1; i >= 0; i--) {
				CocoDressItemHolder basicItemHolder = m_ItemHolderDic.GetValue (m_BasicItemIds [i]);
				TryFillUpDressItem (basicItemHolder, shouldAddedItemIds, currItemIds, true);
			}

			//string str = "PrepareAddDressItems should add: ";
			//shouldAddedItemIds.ForEach (itemId => str += itemId + ", ");
			//str += " conflict: ";
			//itemConflictDataDic.Keys.ForEach (itemId => str += itemId + ", ");
			//Debug.LogError (str);

			conflictDatas = new List<CocoDressItemConflictData> (itemConflictDataDic.Values);
			return shouldAddedItemIds;
		}

		public List<string> PrepareRemoveDressItems (List<string> itemIds, out List<CocoDressItemConflictData> conflictDatas)
		{
			// make will removed items as conflict datas
			Dictionary<string, CocoDressItemConflictData> itemConflictDataDic = new Dictionary<string, CocoDressItemConflictData> ();
			itemIds.ForEach (itemId => {
				if (!itemConflictDataDic.ContainsKey (itemId)) {
					CocoDressItemHolder itemHolder = m_ItemHolderDic.GetValue (itemId);
					if (itemHolder != null) {
						CocoDressItemConflictData conflictData = new CocoDressItemConflictData ();
						conflictData.itemHolder = itemHolder;
						itemConflictDataDic.Add (itemId, conflictData);
					} else {
						Debug.LogErrorFormat ("{0}->PrepareAddDressItems: item [{1}] NOT exists in asset config !", GetType ().Name, itemId);
					}
				}
			});

			// find should added items
			List<string> shouldAddedItemIds = new List<string> ();

			// curr items
			List<string> currItemIds = new List<string> ();
			m_CurrDressModelSetDic.Keys.ForEach (itemId => {
				if (!itemConflictDataDic.ContainsKey (itemId)) {
					currItemIds.Add (itemId);
				}
			});

			// try add last items
			for (int i = m_SortedLastCovers.Count - 1; i >= 0; i--) {
				CocoDressItemHolder lastItemHolder = m_LastCoverItemHolderDic [m_SortedLastCovers [i]];
				if (itemConflictDataDic.ContainsKey (lastItemHolder.id)) {
					// should removed, skip it
					continue;
				}

				TryFillUpDressItem (lastItemHolder, shouldAddedItemIds, currItemIds);
			}

			// try add basic items
			for (int i = m_BasicItemIds.Count - 1; i >= 0; i--) {
				CocoDressItemHolder basicItemHolder = m_ItemHolderDic.GetValue (m_BasicItemIds [i]);
				TryFillUpDressItem (basicItemHolder, shouldAddedItemIds, currItemIds, true);
			}

			// try add will remove items (if not other choice)
			foreach (var itemId in itemIds) {
				CocoDressItemHolder willRemoveItemHolder = m_ItemHolderDic.GetValue (itemId);
				TryFillUpDressItem (willRemoveItemHolder, shouldAddedItemIds, currItemIds, true);
			}

			// if should add, don't remove them
			HashSet<string> shouldAddedItemIdSet = new HashSet<string> (shouldAddedItemIds);
			shouldAddedItemIdSet.ForEach (itemId => {
				if (itemConflictDataDic.ContainsKey (itemId)) {
					itemConflictDataDic.Remove (itemId);
					shouldAddedItemIds.Remove (itemId);
				}
			});

			//string str = "PrepareRemoveDressItems should add: ";
			//shouldAddedItemIds.ForEach (itemId => str += itemId + ", ");
			//str += " conflict: ";
			//itemConflictDataDic.Keys.ForEach (itemId => str += itemId + ", ");
			//Debug.LogError (str);

			conflictDatas = new List<CocoDressItemConflictData> (itemConflictDataDic.Values);
			return shouldAddedItemIds;
		}

		void TryFillUpDressItem (CocoDressItemHolder itemHolder, List<string> shouldAddedItemIds, List<string> currItemIds, bool onlyInMainCategory = false)
		{
			if (itemHolder == null) {
				return;
			}

			// check if in main category
			if (onlyInMainCategory) {
				CocoDressCategoryHolder categoryHolder = (CocoDressCategoryHolder)itemHolder.ParentHolder;
				if (!categoryHolder.isMain) {
					return;
				}
			}

			// check if conflict with will added items
			List<CocoDressItemConflictData> lastConflictDatas = GetDressConflictDatas (itemHolder, shouldAddedItemIds);
			if (lastConflictDatas.Count > 0) {
				return;
			}

			// check if conflict with current existed items
			lastConflictDatas = GetDressConflictDatas (itemHolder, currItemIds);
			if (lastConflictDatas.Count > 0) {
				return;
			}

			// Debug.LogError ("fill: " + itemHolder.id);
			shouldAddedItemIds.Add (itemHolder.id);
		}

		List<CocoDressItemConflictData> GetDressConflictDatas (CocoDressItemHolder itemHolder)
		{
			List<CocoDressItemConflictData> conflictDatas = new List<CocoDressItemConflictData> ();

			if (itemHolder != null) {
				m_CurrDressModelSetDic.ForEach (modelSet => {
					CocoDressItemConflictData conflictData = GetDressConflictData (itemHolder, modelSet.ItemHolder);
					if (conflictData != null) {
						conflictDatas.Add (conflictData);
					}
				});
			}

			return conflictDatas;
		}

		List<CocoDressItemConflictData> GetDressConflictDatas (CocoDressItemHolder itemHolder, List<string> checkedItemIds)
		{
			List<CocoDressItemConflictData> conflictDatas = new List<CocoDressItemConflictData> ();

			if (itemHolder != null && checkedItemIds != null) {
				checkedItemIds.ForEach (itemId => {
					CocoDressItemHolder checkedItemHolder = m_ItemHolderDic.GetValue (itemId);
					CocoDressItemConflictData conflictData = GetDressConflictData (itemHolder, checkedItemHolder);
					if (conflictData != null) {
						conflictDatas.Add (conflictData);
					}
				});
			}

			return conflictDatas;
		}

		CocoDressItemConflictData GetDressConflictData (CocoDressItemHolder newItemHolder, CocoDressItemHolder oldItemHolder)
		{
			if (newItemHolder.IsCoverLayerConflicted (oldItemHolder) || newItemHolder == oldItemHolder) {
				CocoDressItemConflictData conflictData = new CocoDressItemConflictData ();
				conflictData.itemHolder = oldItemHolder;
				return conflictData;
			}

			return null;
		}

		#endregion
	}


	public class CocoDressItemConflictData
	{
		public CocoDressItemHolder itemHolder = null;
	}


	public interface ICocoRoleCustomData
	{
	}
}