using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace CocoPlay
{
	public partial class CocoRoleDressItemHolder : CocoHolderBase
	{
		public int price = 0;
		public CocoLockType lockType = CocoLockType.Non;

		[JsonIgnore]
		public CocoDressItemHolder LinkedDressItemHolder { get; set; }


		#region Print

		protected override string PrintContent (string indentations)
		{
			return string.Format ("{0}, price={1}, lockType={2}", base.PrintContent (indentations), price, lockType);
		}

		#endregion
	}


	public class CocoRoleDressSceneHolder : CocoHolderBase
	{
		public List<CocoRoleDressItemHolder> itemHolders = new List<CocoRoleDressItemHolder> ();


		#region Link

		public override void LinkParent (CocoHolderBase parentHolder)
		{
			base.LinkParent (parentHolder);
			LinkSubHolders (itemHolders);
			InitCategoryItemHolderDic ();
		}

		private Dictionary<string, List<CocoRoleDressItemHolder>> m_CategoryItemHolderDic;

		[JsonIgnore]
		public Dictionary<string, List<CocoRoleDressItemHolder>> CategoryItemHolderDic {
			get { return m_CategoryItemHolderDic; }
		}

		private void InitCategoryItemHolderDic ()
		{
			m_CategoryItemHolderDic = new Dictionary<string, List<CocoRoleDressItemHolder>> ();
			var configHolder = (CocoAssetConfigHolder)ParentHolder.ParentHolder;
			var itemHolderDic = configHolder.ItemHolderDic;

			itemHolders.ForEach (holder => {
				var dressItemHolder = itemHolderDic.GetValue (holder.id);
				if (dressItemHolder != null) {
					holder.LinkedDressItemHolder = dressItemHolder;
					dressItemHolder.LinkRoleDressItemHolder (holder);

					List<CocoRoleDressItemHolder> holders;
					var categoryId = dressItemHolder.ParentHolder.id;
					if (m_CategoryItemHolderDic.ContainsKey (categoryId)) {
						holders = m_CategoryItemHolderDic [categoryId];
					} else {
						holders = new List<CocoRoleDressItemHolder> ();
						m_CategoryItemHolderDic.Add (categoryId, holders);
					}
					holders.Add (holder);
				}
			});
		}

		public List<CocoRoleDressItemHolder> GetItemHolders (string categoryId)
		{
			return m_CategoryItemHolderDic.GetValue (categoryId);
		}

		public CocoRoleDressItemHolder GetItemHolder (string itemId)
		{
			CocoRoleDressItemHolder matchedItemHolder = null;

			foreach (var kvpCategoryItemHolders in CategoryItemHolderDic) {
				matchedItemHolder = kvpCategoryItemHolders.Value.Find (itemHolder => itemHolder.id == itemId);
			}

			return matchedItemHolder;
		}

		public CocoRoleDressItemHolder GetItemHolder (string categoryId, string itemId)
		{
			var categoryItemHolders = GetItemHolders (categoryId);
			if (categoryItemHolders == null) {
				return null;
			}

			return categoryItemHolders.Find (itemHolder => itemHolder.id == itemId);
		}

		#endregion


		#region Print

		protected override string PrintContent (string indentations)
		{
			var str = base.PrintContent (indentations) + '\n';
			var subIndentations = string.Format ("{0}\t", indentations);
			str += PrintSubHolders ("itemHolders", itemHolders, subIndentations);
			str += indentations;
			return str;
		}

		#endregion


		#region Get Random Item

		public CocoRoleDressItemHolder GetRandomItemHolder (bool onlyOwned = false)
		{
			if (!onlyOwned) {
				return CocoData.GetRandomItem (itemHolders);
			}

			var stateModel = CocoRoot.GetInstance<CocoDressStateModel> ();
			var randomInItemHolders = ((CocoRoleDressHolder)ParentHolder).GetOwnedItemHoldersInList (itemHolders, stateModel);
			return CocoData.GetRandomItem (randomInItemHolders);
		}

		public CocoRoleDressItemHolder GetRandomItemHolder (string categoryId, bool onlyOwned = false)
		{
			var categoryItemHolders = m_CategoryItemHolderDic.GetValue (categoryId);
			if (!onlyOwned) {
				return CocoData.GetRandomItem (categoryItemHolders);
			}

			var stateModel = CocoRoot.GetInstance<CocoDressStateModel> ();
			var randomInItemHolders = ((CocoRoleDressHolder)ParentHolder).GetOwnedItemHoldersInList (categoryItemHolders, stateModel);
			return CocoData.GetRandomItem (randomInItemHolders);
		}

		#endregion


		#region Get Random Suit

		public List<CocoRoleDressItemHolder> GetRandomSuitItemHolders (bool onlyOwned = false)
		{
			var roleDressHolder = (CocoRoleDressHolder)ParentHolder;
			return roleDressHolder.GetRandomSuitItems (m_CategoryItemHolderDic, (itemHolder) => itemHolder, onlyOwned);
		}

		public List<CocoRoleDressItemHolder> GetRandomSuitItemHolders (List<string> categoryIds, bool onlyOwned = false)
		{
			var suitItemHolders = new List<CocoRoleDressItemHolder> ();

			categoryIds.ForEach (categoryId => {
				var itemHolder = GetRandomItemHolder (categoryId, onlyOwned);
				if (itemHolder != null) {
					suitItemHolders.Add (itemHolder);
				}
			});

			return suitItemHolders;
		}

		public List<string> GetRandomSuitItemIds (bool onlyOwned = false)
		{
			var roleDressHolder = (CocoRoleDressHolder)ParentHolder;
			return roleDressHolder.GetRandomSuitItems (m_CategoryItemHolderDic, (itemHolder) => itemHolder.id, onlyOwned);
		}

		public List<string> GetRandomSuitItemIds (List<string> categoryIds, bool onlyOwned = false)
		{
			var suitItemIds = new List<string> ();

			categoryIds.ForEach (categoryId => {
				var itemHolder = GetRandomItemHolder (categoryId, onlyOwned);
				if (itemHolder != null) {
					suitItemIds.Add (itemHolder.id);
				}
			});

			return suitItemIds;
		}

		#endregion
	}


	public class CocoRoleDressHolder : CocoHolderBase
	{
		public List<CocoRoleDressSceneHolder> sceneHolders = new List<CocoRoleDressSceneHolder> ();


		#region Link

		public override void LinkParent (CocoHolderBase parentHolder)
		{
			base.LinkParent (parentHolder);
			LinkSubHolders (sceneHolders);
			InitSceneHolderDic ();
		}

		private Dictionary<string, CocoRoleDressSceneHolder> m_SceneHolderDic;

		[JsonIgnore]
		public Dictionary<string, CocoRoleDressSceneHolder> SceneHolderDic {
			get { return m_SceneHolderDic; }
		}

		private void InitSceneHolderDic ()
		{
			m_SceneHolderDic = new Dictionary<string, CocoRoleDressSceneHolder> (sceneHolders.Count);
			sceneHolders.ForEach (holder => m_SceneHolderDic.Add (holder.id, holder));
		}

		public CocoRoleDressSceneHolder GetSceneHolder (string sceneId)
		{
			return m_SceneHolderDic.GetValue (sceneId);
		}

		public List<CocoRoleDressSceneHolder> GetSceneHoldersContainCategory (string categoryId)
		{
			var itemInSceneHolders = new List<CocoRoleDressSceneHolder> ();
			sceneHolders.ForEach (sceneHolder => {
				if (sceneHolder.CategoryItemHolderDic.ContainsKey (categoryId)) {
					itemInSceneHolders.Add (sceneHolder);
				}
			});
			return itemInSceneHolders;
		}

		public List<CocoRoleDressSceneHolder> GetSceneHoldersContainItem (string itemId)
		{
			var itemInSceneHolders = new List<CocoRoleDressSceneHolder> ();
			sceneHolders.ForEach (sceneHolder => {
				var itemHolder = sceneHolder.GetItemHolder (itemId);
				if (itemHolder != null) {
					itemInSceneHolders.Add (sceneHolder);
				}
			});
			return itemInSceneHolders;
		}

		public List<CocoRoleDressSceneHolder> GetSceneHoldersContainItem (string categoryId, string itemId)
		{
			var itemInSceneHolders = new List<CocoRoleDressSceneHolder> ();
			sceneHolders.ForEach (sceneHolder => {
				var itemHolder = sceneHolder.GetItemHolder (categoryId, itemId);
				if (itemHolder != null) {
					itemInSceneHolders.Add (sceneHolder);
				}
			});
			return itemInSceneHolders;
		}

		#endregion


		#region Print

		protected override string PrintContent (string indentations)
		{
			var str = string.Format ("{0}\n", base.PrintContent (indentations));
			var subIndentations = string.Format ("{0}\t", indentations);
			str += PrintSubHolders ("sceneHolders", sceneHolders, subIndentations);
			str += indentations;
			return str;
		}

		#endregion


		#region Get Random Item

		public CocoRoleDressItemHolder GetRandomItemHolder (bool onlyOwned = false)
		{
			var sceneHolder = sceneHolders [Random.Range (0, sceneHolders.Count)];
			if (sceneHolder == null) {
				return null;
			}

			return sceneHolder.GetRandomItemHolder (onlyOwned);
		}

		public CocoRoleDressItemHolder GetRandomItemHolder (string categoryId, bool onlyOwned = false)
		{
			var categoryInSceneHolders = GetSceneHoldersContainCategory (categoryId);
			if (categoryInSceneHolders == null || categoryInSceneHolders.Count <= 0) {
				return null;
			}

			var sceneHolder = categoryInSceneHolders [Random.Range (0, categoryInSceneHolders.Count)];
			return sceneHolder.GetRandomItemHolder (categoryId, onlyOwned);
		}

		public CocoRoleDressItemHolder GetRandomItemHolder (string sceneId, string categoryId, bool onlyOwned = false)
		{
			var sceneHolder = GetSceneHolder (sceneId);
			if (sceneHolder == null) {
				return null;
			}

			return sceneHolder.GetRandomItemHolder (categoryId, onlyOwned);
		}

		#endregion


		#region Get Random Suit

		public List<CocoRoleDressItemHolder> GetRandomSuitItemHolders (bool onlyOwned = false)
		{
			var categoryItemHoders = CollectAllCategoryItemHolders (m_SceneHolderDic.Keys);
			return GetRandomSuitItems (categoryItemHoders, (itemHolder) => itemHolder, onlyOwned);
		}

		public List<CocoRoleDressItemHolder> GetRandomSuitItemHolders (List<string> sceneIds, bool onlyOwned = false)
		{
			var categoryItemHoders = CollectAllCategoryItemHolders (sceneIds);
			return GetRandomSuitItems (categoryItemHoders, (itemHolder) => itemHolder, onlyOwned);
		}

		public List<CocoRoleDressItemHolder> GetRandomSuitItemHolders (List<string> sceneIds, List<string> categoryIds, bool onlyOwned = false)
		{
			var categoryIdSet = new HashSet<string> (categoryIds);
			var categoryItemHoders = CollectAllCategoryItemHolders (sceneIds, categoryIdSet);
			return GetRandomSuitItems (categoryItemHoders, (itemHolder) => itemHolder, onlyOwned);
		}

		public List<CocoRoleDressItemHolder> GetRandomSuitItemHolders (string sceneId, bool onlyOwned = false)
		{
			var sceneHolder = GetSceneHolder (sceneId);
			if (sceneHolder == null) {
				return null;
			}

			return sceneHolder.GetRandomSuitItemHolders (onlyOwned);
		}

		public List<CocoRoleDressItemHolder> GetRandomSuitItemHolders (string sceneId, List<string> categoryIds, bool onlyOwned = false)
		{
			var sceneHolder = GetSceneHolder (sceneId);
			if (sceneHolder == null) {
				return null;
			}

			return sceneHolder.GetRandomSuitItemHolders (categoryIds, onlyOwned);
		}

		public List<string> GetRandomSuitItemIds (bool onlyOwned = false)
		{
			var categoryItemHoders = CollectAllCategoryItemHolders (m_SceneHolderDic.Keys);
			return GetRandomSuitItems (categoryItemHoders, (itemHolder) => itemHolder.id, onlyOwned);
		}

		public List<string> GetRandomSuitItemIds (List<string> sceneIds, bool onlyOwned = false)
		{
			var categoryItemHoders = CollectAllCategoryItemHolders (sceneIds);
			return GetRandomSuitItems (categoryItemHoders, (itemHolder) => itemHolder.id, onlyOwned);
		}

		public List<string> GetRandomSuitItemIds (List<string> sceneIds, List<string> categoryIds, bool onlyOwned = false)
		{
			var categoryIdSet = new HashSet<string> (categoryIds);
			var categoryItemHoders = CollectAllCategoryItemHolders (sceneIds, categoryIdSet);
			return GetRandomSuitItems (categoryItemHoders, (itemHolder) => itemHolder.id, onlyOwned);
		}

		public List<string> GetRandomSuitItemIds (string sceneId, bool onlyOwned = false)
		{
			return GetRandomSuitItemIds (new List<string> { sceneId }, onlyOwned);
		}

		public List<string> GetRandomSuitItemIds (string sceneId, List<string> categoryIds, bool onlyOwned = false)
		{
			return GetRandomSuitItemIds (new List<string> { sceneId }, categoryIds, onlyOwned);
		}

		public List<T> GetRandomSuitItems<T> (Dictionary<string, List<CocoRoleDressItemHolder>> categoryItemHolders,
			System.Func<CocoRoleDressItemHolder, T> extractFunc, bool onlyOwned = false)
		{
			var suitItems = new List<T> ();

			var configHolder = (CocoAssetConfigHolder)ParentHolder;
			var suitCategoryIds = configHolder.GetRandomSuitCategoryIds (categoryItemHolders.Keys);
			var count = suitCategoryIds.Count;
			var randomIdGenerator = new CocoRandomIdGenerator (count);
			var stateModel = onlyOwned ? CocoRoot.GetInstance<CocoDressStateModel> () : null;

			for (var i = 0; i < count; i++) {
				var categoryId = suitCategoryIds [randomIdGenerator.RandomId];
				var currCategoryItemHolders = categoryItemHolders [categoryId];

				CocoRoleDressItemHolder itemHolder;
				if (!onlyOwned) {
					itemHolder = CocoData.GetRandomItem (currCategoryItemHolders);
				} else {
					var ownedItemHolders = GetOwnedItemHoldersInList (currCategoryItemHolders, stateModel);
					itemHolder = CocoData.GetRandomItem (ownedItemHolders);
				}
				if (itemHolder != null) {
					suitItems.Add (extractFunc (itemHolder));
				}
			}

			return suitItems;
		}

		#endregion


		#region Un-Owned Items

		public List<CocoRoleDressItemHolder> GetUnOwnedItemHolders (bool containTempOwned = true)
		{
			var unOwnedItemHolders = new List<CocoRoleDressItemHolder> ();
			var stateModel = CocoRoot.GetInstance<CocoDressStateModel> ();

			sceneHolders.ForEach (sceneHolder => {
				var sceneItemHolders = GetUnOwnedItemHoldersInList (sceneHolder.itemHolders, stateModel, containTempOwned);
				if (sceneItemHolders != null && sceneItemHolders.Count > 0) {
					unOwnedItemHolders.AddRange (sceneItemHolders);
				}
			});

			return unOwnedItemHolders;
		}

		public List<CocoRoleDressItemHolder> GetUnOwnedItemHolders (string sceneId, bool containTempOwned = true)
		{
			var sceneHolder = GetSceneHolder (sceneId);
			if (sceneHolder == null) {
				return null;
			}

			var stateModel = CocoRoot.GetInstance<CocoDressStateModel> ();
			return GetUnOwnedItemHoldersInList (sceneHolder.itemHolders, stateModel, containTempOwned);
		}

		public List<CocoRoleDressItemHolder> GetUnOwnedItemHolders (string sceneId, List<string> categoryIds, bool containTempOwned = true)
		{
			var sceneHolder = GetSceneHolder (sceneId);
			if (sceneHolder == null) {
				return null;
			}

			var stateModel = CocoRoot.GetInstance<CocoDressStateModel> ();
			return GetUnOwnedItemHolders (sceneHolder, categoryIds, stateModel, containTempOwned);
		}

		public List<CocoRoleDressItemHolder> GetUnOwnedItemHolders (List<string> categoryIds, bool containTempOwned = true)
		{
			if (categoryIds == null || categoryIds.Count <= 0) {
				return null;
			}

			var unOwnedItemHolders = new List<CocoRoleDressItemHolder> ();
			var stateModel = CocoRoot.GetInstance<CocoDressStateModel> ();

			sceneHolders.ForEach (sceneHolder => {
				var sceneUnOwnedItemHolders = GetUnOwnedItemHolders (sceneHolder, categoryIds, stateModel, containTempOwned);
				if (sceneUnOwnedItemHolders != null) {
					unOwnedItemHolders.AddRange (sceneUnOwnedItemHolders);
				}
			});

			return unOwnedItemHolders;
		}

		private List<CocoRoleDressItemHolder> GetUnOwnedItemHolders (CocoRoleDressSceneHolder sceneHolder, List<string> categoryIds, CocoDressStateModel stateModel,
			bool containTempOwned = true)
		{
			if (categoryIds == null || categoryIds.Count <= 0) {
				return null;
			}

			var unOwnedItemHolders = new List<CocoRoleDressItemHolder> ();

			categoryIds.ForEach (categoryId => {
				if (!sceneHolder.CategoryItemHolderDic.ContainsKey (categoryId)) {
					return;
				}

				var sceneCategoryItemHolders =
					GetUnOwnedItemHoldersInList (sceneHolder.CategoryItemHolderDic [categoryId], stateModel, containTempOwned);
				if (sceneCategoryItemHolders != null && sceneCategoryItemHolders.Count > 0) {
					unOwnedItemHolders.AddRange (sceneCategoryItemHolders);
				}
			});

			return unOwnedItemHolders;
		}

		#endregion


		#region Helper

		private Dictionary<string, List<CocoRoleDressItemHolder>> CollectAllCategoryItemHolders (IEnumerable<string> sceneIds, HashSet<string> categoryIds = null)
		{
			var categoryItemHolders = new Dictionary<string, List<CocoRoleDressItemHolder>> ();

			foreach (var sceneId in sceneIds) {
				var sceneHolder = m_SceneHolderDic.GetValue (sceneId);
				if (sceneHolder == null) {
					continue;
				}

				sceneHolder.CategoryItemHolderDic.ForEach ((categoryId, itemHolders) => {
					if (categoryIds != null && categoryIds.Count > 0) {
						if (!categoryIds.Contains (categoryId)) {
							return;
						}
					}

					if (categoryItemHolders.ContainsKey (categoryId)) {
						categoryItemHolders [categoryId].AddRange (itemHolders);
					} else {
						categoryItemHolders.Add (categoryId, new List<CocoRoleDressItemHolder> (itemHolders));
					}
				});
			}

			return categoryItemHolders;
		}

		public List<CocoRoleDressItemHolder> GetOwnedItemHoldersInList (List<CocoRoleDressItemHolder> inItemHolders, CocoDressStateModel stateModel,
			bool containTempOwned = true)
		{
			if (inItemHolders == null || inItemHolders.Count <= 0) {
				return null;
			}

			var ownedItemHolders = inItemHolders.FindAll (itemHolder => {
				if (itemHolder.price <= 0) {
					return true;
				}
				if (stateModel.IsItemPurchased (itemHolder.id)) {
					return true;
				}
				if (containTempOwned && stateModel.IsTempUnlocked (itemHolder.id)) {
					return true;
				}
				return false;
			});
			return ownedItemHolders;
		}

		public List<CocoRoleDressItemHolder> GetUnOwnedItemHoldersInList (List<CocoRoleDressItemHolder> inItemHolders, CocoDressStateModel stateModel,
			bool containTempOwned = true)
		{
			if (inItemHolders == null || inItemHolders.Count <= 0) {
				return null;
			}

			var unOwnedItemHolders = inItemHolders.FindAll (itemHolder => {
				if (itemHolder.lockType == CocoLockType.Non && itemHolder.price <= 0) {
					return false;
				}
				if (stateModel.IsItemPurchased (itemHolder.id)) {
					return false;
				}
				if (!containTempOwned && stateModel.IsTempUnlocked (itemHolder.id)) {
					return false;
				}
				return true;
			});
			return unOwnedItemHolders;
		}

		#endregion
	}


	public class CocoRoleBodyHolder : CocoHolderBase
	{
		public Dictionary<string, string> bodyBones = new Dictionary<string, string> ();
		public Dictionary<string, string> bodyRenderers = new Dictionary<string, string> ();


		#region Print

		protected override string PrintContent (string indentations)
		{
			var str = string.Format ("{0}\n", base.PrintContent (indentations));
			var subIndentations = string.Format ("{0}\t", indentations);
			str += PrintSubValues ("bodyBones", bodyBones, subIndentations);
			str += PrintSubValues ("bodyRenderers", bodyRenderers, subIndentations);
			str += indentations;
			return str;
		}

		#endregion
	}


	public class CocoRoleHolder : CocoHolderBase
	{
		public string boneItemId = "player";
		public List<string> basicItemIds = new List<string> () { "head", "body" };
		public string dressId = "common";
		public string bodyId = "common";
		public bool enableShadow = false;

		[JsonIgnore]
		public CocoRoleDressHolder DressHolder { get; private set; }

		[JsonIgnore]
		public CocoRoleBodyHolder BodyHolder { get; private set; }

		public override void LinkParent (CocoHolderBase parentHolder)
		{
			base.LinkParent (parentHolder);

			var configHolder = (CocoAssetConfigHolder)ParentHolder;
			DressHolder = configHolder.GetRoleDressHolder (dressId);
			BodyHolder = configHolder.GetRoleBodyHolder (bodyId);
		}


		#region Print

		protected override string PrintContent (string indentations)
		{
			var str = string.Format ("{0} boneItemId={1} dressId={2} bodyId={3} enableShadow={4}\n", base.PrintContent (indentations), boneItemId, dressId,
				bodyId,
				enableShadow);
			var subIndentations = string.Format ("{0}\t", indentations);
			str += PrintSubValues ("basicItemIds", basicItemIds, subIndentations);
			str += indentations;
			return str;
		}

		#endregion
	}
}