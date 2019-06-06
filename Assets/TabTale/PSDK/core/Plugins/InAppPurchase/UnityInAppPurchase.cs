using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {
	public class UnityInAppPurchase : IPsdkInAppPurchase {

		public bool  Setup() {
			return true;
		}

		public IPsdkInAppPurchase GetImplementation()
		{
			return this;
		}

		public void PurchaseCampaignCompleteTransaction(bool result){
		}

		public void psdkStartedEvent() {

		}

		public void AddProduct(string id, string price, bool isPurchased)
		{
		}

		public void ItemPurchased(string id)
		{
		}
	}
}