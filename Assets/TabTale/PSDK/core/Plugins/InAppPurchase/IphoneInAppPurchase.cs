using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace TabTale.Plugins.PSDK {
	public class IphoneInAppPurchase : IPsdkInAppPurchase {

		[DllImport ("__Internal")]
		private static extern void psdkPurchaseCampaignCompleteTransaction(bool result);

		[DllImport ("__Internal")]
		private static extern void psdkItemPurchased(string itemId);

		[DllImport ("__Internal")]
		private static extern void psdkAddProduct(string productId, string price, bool isPurchased);

		public bool  Setup() {
			return true;
		}

		public IPsdkInAppPurchase GetImplementation()
		{
			return this;
		}

		public void PurchaseCampaignCompleteTransaction(bool result){
			psdkPurchaseCampaignCompleteTransaction(result);
		}

		public void psdkStartedEvent() {

		}

		public void AddProduct(string id, string price, bool isPurchased)
		{
			psdkAddProduct(id,price,isPurchased);
		}

		public void ItemPurchased(string id)
		{
			psdkItemPurchased(id);
		}
	}
}