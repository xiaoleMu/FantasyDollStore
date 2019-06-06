using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {
	
	public interface IPsdkBilling : IPsdkService 
	{
		bool IsInitialised ();

		bool IsPurchaseInProgress ();

		bool IsRestoreInProgress ();

		void InitBilling ();

		void RestorePurchases();

		void PurchaseItem(string itemId);

		bool IsPurchased(string itemId);

		bool IsConsumable(string itemId);

		string GetLocalizedPriceString(string itemId);

		string GetISOCurrencySymbol(string itemId);

		decimal GetPriceInLocalCurrency(string itemId);

		void ClearTransactions();

		bool IsNoAdsItem(string iapId);

		string GetMainId(string iapId);

	}
}

namespace TabTale {

	public class PurchaseIAPResult
	{
		public PurchaseIAPResult(InAppPurchasableItem purchasedItem, PurchaseIAPResultCode result = PurchaseIAPResultCode.Success, BillerErrors error = BillerErrors.NO_ERROR)
		{
			this.purchasedItem = purchasedItem;
			this.result = result;
			this.error = error;
		}

		public InAppPurchasableItem purchasedItem;
		public PurchaseIAPResultCode result;
		public BillerErrors error;

	}

	public enum PurchaseIAPResultCode
	{
		Success, Cancelled, Failed
	}

	public enum BillerErrors
	{
		NO_ERROR,
		PURCHASING_UNAVAILABLE,
		ATTEMPTING_TO_PURCHASE_PRODUCT_WITH_SAME_RECEIPT,
		PAYMENT_DECLINED,
		PRODUCT_UNAVAILABLE,
		REMOTE_VALIDATION_FAILED,
		APP_NOT_KNOWN,
		NO_PRODUCTS_AVAILABLE,
		UNKNOWN
	}

	public struct InAppPurchasableItem
	{
		public string id;
		public string name;
		public string description;
		public string localizedPrice;
		public string receipt;
		public string transactionId;
		public string currency;
	}
}
