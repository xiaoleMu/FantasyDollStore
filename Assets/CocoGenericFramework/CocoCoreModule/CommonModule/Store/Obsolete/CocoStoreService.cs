// ----------------------------------------------------------------
// Coco游戏商店功能模块。
// 1. 提供商店商品数据添加，查询, 购买功能，Android自动恢复购买
//
//
// * 严禁添加任何关联代码，只能外部调用它 ！！！
// ----------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale;
using System.Linq;
using Game;

namespace CocoPlay.Store
{
	public class CocoStoreService : GameView
	{
		[Inject]
		public InventoryStateModel inventoryStateModel { get; set; }

		[Inject]
		public IAPConfigModel iapConfigModel { get; set; }

		[Inject]
		public IBillingService billingService { get; set; }

		[Inject]
		public PurchasesRestoredSignal restoredSignal { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		private static CocoStoreService instance = null;

		public static CocoStoreService Instance {
			get { return instance; }
		}

		protected override void OnRegister ()
		{
			base.OnRegister ();
			if (instance != null) {
				Destroy (this);
				return;
			}

			instance = this;

			List<CocoStoreItem> pAllStoreItems = CocoStoreData.Instance.allProductItems;
			CocoStoreData.Instance.allProductItemsDic.Clear ();
			for (int i = 0; i < pAllStoreItems.Count; i++) {
				CocoStoreData.Instance.allProductItemsDic.Add (pAllStoreItems [i].productId, pAllStoreItems [i]);
			}

			restoredSignal.AddListener (OnRestoreEvent);
			StartCoroutine (StartRestoreProgress ());
		}

		protected override void OnUnRegister ()
		{
			restoredSignal.RemoveListener (OnRestoreEvent);
			base.OnUnRegister ();
		}


		#region Restore 恢复购买

		private IEnumerator StartRestoreProgress ()
		{
			while (!billingService.IsInitialised)
				yield return new WaitForSeconds (0.5f);

			OnRestoreEvent (true);
		}

		private void OnRestoreEvent (bool success)
		{
			Debug.LogError ("--------------restore ------------- cocostoreService");

			if (success) {
				foreach (CocoStoreID productId in GetAllProductIds) {
					string pId = iapConfigModel.GetIAPId (GetProductIapString (productId));

					if (billingService.IsPurchased (pId)) {
						PurchasedProduct (productId);
					}
				}

				CocoSignals.updatePriceSignal.Dispatch ();
				CocoSignals.refreshButtonRvSignal.Dispatch ();
			}
		}

		#endregion


		#region 查询是否购买， 购买

		//通过GameProductId 判断是否已经购买, 优先判断最大包
		public bool IsPurchasedByProductId (CocoStoreID pId)
		{
			CocoStoreItem pStoreItem = GetStoreItemById (pId);
			if (pStoreItem == null) {
				Debug.LogError ("There is no CocoStoreItem for the product. Please check !(该商品不存在，检查设置): " + pId);
				return false;
			}

			return inventoryStateModel.HasItem (pStoreItem.iosIapString) || billingService.IsPurchased (iapConfigModel.GetIAPId (pStoreItem.iosIapString));
		}

		//通过商品的IapId 判断是否已经购买 例如com.cocoplay.iceskater.fullversion。  优先判断最大包
		public bool IsPurchasedByIapString (string pId)
		{
			if (!GetAllProductIapStrings.Contains (pId)) {
				Debug.LogError ("There is no iap id for the product. Please check !(该商品不存在，检查设置): " + pId);
				return false;
			}

			return inventoryStateModel.HasItem (pId) || billingService.IsPurchased (iapConfigModel.GetIAPId (pId));
		}

		//购买一个商品。（会设置关联数据）
		public void PurchasedProduct (CocoStoreID pId)
		{
			CocoStoreItem pStoreItem = GetStoreItemById (pId);
			if (pStoreItem == null) {
				Debug.LogError ("There is no CocoStoreItem for the product. Please check !(该商品不存在，检查设置): " + pId);
				return;
			}

			inventoryStateModel.SetQuantity (pStoreItem.iosIapString, 1);

			//Unlock all related store iap items
			//解锁相关联的商品
			for (int i = 0; i < pStoreItem.relatedStoreItems.Count; i++) {
				CocoStoreItem pRelatedItem = GetStoreItemById (pStoreItem.relatedStoreItems [i]);

				if (pStoreItem == null) {
					Debug.LogError ("There is no CocoStoreItem for the product. Please check !(该商品不存在，检查设置): " + pStoreItem.relatedStoreItems [i]);
					continue;
				}

				inventoryStateModel.SetQuantity (pRelatedItem.iosIapString, 1);
			}

			foreach (CocoStoreItem pItem in CocoStoreData.Instance.allProductItems) {
				if (pItem.productId == pId)
					continue;

				bool pAllRelatedItemsUnlocked = pItem.relatedStoreItems.Count > 0 ? true : false;
				for (int i = 0; i < pItem.relatedStoreItems.Count; i++) {
					if (!IsPurchasedByProductId (pItem.relatedStoreItems [i])) {
						pAllRelatedItemsUnlocked = false;
						break;
					}
				}

				if (pAllRelatedItemsUnlocked) {
					inventoryStateModel.SetQuantity (pItem.iosIapString, 1);
				}
			}
		}

		public string GetProductIapString (CocoStoreID pProductId)
		{
			if (CocoStoreData.Instance.allProductItemsDic.ContainsKey (pProductId))
				return CocoStoreData.Instance.allProductItemsDic [pProductId].iosIapString;

			return string.Empty;
		}

		private CocoStoreItem GetStoreItemById (CocoStoreID pProductId)
		{
			if (CocoStoreData.Instance.allProductItemsDic.ContainsKey (pProductId))
				return CocoStoreData.Instance.allProductItemsDic [pProductId];

			return null;
		}

		public List<string> GetAllProductIapStrings {
			get { return CocoStoreData.Instance.allProductItemsDic.Values.Select (s => (s.iosIapString)).ToList<string> (); }
		}

		public List<CocoStoreID> GetAllProductIds {
			get { return CocoStoreData.Instance.allProductItemsDic.Keys.ToList<CocoStoreID> (); }
		}

		#endregion
	}
}
