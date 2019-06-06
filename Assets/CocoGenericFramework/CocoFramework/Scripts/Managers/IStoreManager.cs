using UnityEngine;
using System.Collections;
using RSG;

namespace TabTale
{
	public interface IStoreManager 
	{
		IPromise<BuyItemResult> BuyItem(string itemId, CurrencyType currencyType = CurrencyType.AutoDetect);

		IPromise<BuyItemResult> BuyItemGroup(string itemGroupId);

		bool AnyItemAvailableToBuy(CurrencyType currencyType = CurrencyType.AutoDetect);
	}

	public enum CurrencyType
	{
		AutoDetect,
		VirtualCurrency,
		HardCurrency
	}

	public enum BuyItemResultCode
	{
		Success,
		RestrictionsNotMet,
		InsufficientCurrency,
		InAppPurchaseFailed,
		Error
	}

	public class BuyItemResult
	{
		public BuyItemResultCode resultCode;
		public PurchaseIAPResult purchaseIAPResult;

		public BuyItemResult() { }

		public BuyItemResult(BuyItemResultCode resultCode, PurchaseIAPResult iapResult = null)
		{
			this.resultCode = resultCode;
			this.purchaseIAPResult = iapResult;
		}
	}
}