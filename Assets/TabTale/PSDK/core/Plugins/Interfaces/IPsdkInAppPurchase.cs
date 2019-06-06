using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK 
{
	public interface IPsdkInAppPurchase : IPsdkService {
		bool Setup();
		IPsdkInAppPurchase GetImplementation();
		void PurchaseCampaignCompleteTransaction(bool result);

		void AddProduct(string id, string price, bool isPurchased);
		void ItemPurchased(string id);
	}
}