using UnityEngine;
using System.Collections.Generic;
using LitJson;
using TabTale;

namespace CocoPlay
{
	public abstract class CocoLockableStateModel<T> : StateModel<T>, ICocoLockStateModel where T : CocoLockableStateData, new ()
	{

		#region Init

		protected override bool ValidateInit ()
		{
			_data.Init ();
			return base.ValidateInit ();
		}

		#endregion


		#region ICocoLockStateModel implementation (Unlock)

		public void TempUnlockItem (string itemId)
		{
			_data.AddTempUnlockItem (itemId);
		}

		public bool IsTempUnlocked (string itemId)
		{
			return _data.IsItemTempUnlocked (itemId);
		}

		public void ClearTempUnlockFlags ()
		{
			_data.ResetTempUnlockSet ();
		}
		
		public void RemoveTempUnlockedItem(string itemId)
		{
			if (IsTempUnlocked(itemId))
			{
				_data.RemoveTempUnlockItem(itemId);
			}
		}

		#endregion


		#region ICocoLockStateModel implementation (Purchase)

		public void PurchaseItem (string itemId)
		{
			if (_data.IsItemPurchased (itemId)) {
				return;
			}

			_data.AddPurchaseItem (itemId);
			Save ();
		}
		
		public void PurchaseItem (string pItemId,bool pIsSave)
		{
			if (_data.IsItemPurchased (pItemId)) {
				return;
			}

			_data.AddPurchaseItem (pItemId);
			if(pIsSave)
				Save ();
		}
		
		public void PurchaseItems (string[] pItemIds,bool pIsSave = true)
		{
			foreach (var itemId in pItemIds)
			{
				if (!_data.IsItemPurchased (itemId))
					_data.AddPurchaseItem (itemId);
			}

			if(pIsSave)
				Save ();
		}
	

		public bool IsItemPurchased (string itemId)
		{
			return _data.IsItemPurchased (itemId);
		}
		
		public void RemovePurchaseItem(string itemId)
		{
			if (IsItemPurchased(itemId))
			{
				_data.RemovePurchaseItem(itemId);
			}

		}

		#endregion
	}
}
