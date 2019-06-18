using UnityEngine;
using System.Collections;

namespace CocoPlay
{
	public interface ICocoLockStateModel
	{
		#region Temp Unlock

		void TempUnlockItem (string itemId);

		bool IsTempUnlocked (string itemId);

		void ClearTempUnlockFlags ();

		#endregion


		#region Purchase

		void PurchaseItem (string itemId);

		bool IsItemPurchased (string itemId);

		#endregion
	}
}
