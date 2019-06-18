using UnityEngine;
using System.Collections.Generic;
using LitJson;
using TabTale;

namespace CocoPlay
{
	public abstract class CocoLockableStateData : IStateData
	{
		#region IStateData implementation

		public abstract string GetStateName ();

		public virtual string ToLogString ()
		{
			return string.Format ("{0}: m_TempUnlockedItemIdSet [{1}], purchasedItemIds [{2}]", GetStateName (), m_TempUnlockedItemIdSet.Count, purchasedItemIds.Count);
		}

		public IStateData Clone ()
		{
			CocoLockableStateData data = Create ();
			data.CloneContent (this);
			return data;
		}

		#endregion


		#region Init

		public virtual void Init ()
		{
			m_PurchasedItemIdSet = new HashSet<string> (purchasedItemIds);
		}

		public abstract CocoLockableStateData Create ();

		public virtual void CloneContent (CocoLockableStateData source)
		{
			// temp unlock
			m_TempUnlockedItemIdSet = new HashSet<string> (source.m_TempUnlockedItemIdSet);

			// purchase
			purchasedItemIds = new List<string> (source.purchasedItemIds);
			m_PurchasedItemIdSet = new HashSet<string> (source.m_PurchasedItemIdSet);
		}

		#endregion


		#region Item Unlock (Temporary)

		HashSet<string> m_TempUnlockedItemIdSet = new HashSet<string> ();

		public void AddTempUnlockItem (string itemId)
		{
			if (m_TempUnlockedItemIdSet.Contains (itemId)) {
				return;
			}

			m_TempUnlockedItemIdSet.Add (itemId);
		}

		public void RemoveTempUnlockItem (string itemId)
		{
			m_TempUnlockedItemIdSet.Remove (itemId);
		}

		public bool IsItemTempUnlocked (string itemId)
		{
			return m_TempUnlockedItemIdSet.Contains (itemId);
		}

		public void ResetTempUnlockSet ()
		{
			m_TempUnlockedItemIdSet.Clear ();
		}

		#endregion


		#region Purchase

		public List<string> purchasedItemIds = new List<string> ();

		HashSet<string> m_PurchasedItemIdSet = new HashSet<string> ();

		public void AddPurchaseItem (string itemId)
		{
			m_PurchasedItemIdSet.Add (itemId);
			purchasedItemIds.Add (itemId);
		}

		public void RemovePurchaseItem (string itemId)
		{
			if (m_PurchasedItemIdSet.Remove (itemId)) {
				purchasedItemIds.Remove (itemId);
			}
		}

		public bool IsItemPurchased (string itemId)
		{
			return m_PurchasedItemIdSet.Contains (itemId);
		}

		#endregion

	}
}
