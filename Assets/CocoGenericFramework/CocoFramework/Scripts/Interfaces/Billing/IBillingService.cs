using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using TabTale.Plugins.PSDK;

namespace TabTale 
{
	public class PurchasedIAPSignal			: Signal<PurchaseIAPResult> { }
	public class BillerInitDoneSignal		: Signal<BillerErrors> { }
	public class PurchasesRestoredSignal	: Signal<bool> { }

	public interface IBillingService
	{
		PurchasedIAPSignal purchasedIAPSignal { get; }

		BillerInitDoneSignal billerInitDoneSignal { get; }

		PurchasesRestoredSignal purchasesRestoredSignal { get; }

		bool IsInitialised { get; }

		bool IsPurchaseInProgress { get; }

		bool IsRestoreInProgress { get; }

		void InitBilling ();

		void RestorePurchases();

		void PurchaseItem(string itemId);

		bool IsPurchased(string itemId);

		bool IsConsumable(string itemId);

		string GetLocalizedPriceString(string itemId);

		string GetISOCurrencySymbol(string itemId);

		decimal GetPriceInLocalCurrency(string itemId);

		void ClearTransactions();

	}
}