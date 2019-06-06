using UnityEngine;
using System.Collections;
using TabTale.Publishing;

namespace TabTale
{
	public class NullBillingService : IBillingService
	{
		public PurchasedIAPSignal purchasedIAPSignal { get; set; }

		public BillerInitDoneSignal billerInitDoneSignal { get; set; }

		public PurchasesRestoredSignal purchasesRestoredSignal { get; set; }

		public bool IsInitialised { get { return true;} }

		public bool IsPurchaseInProgress { get { return false; } }

		public bool IsRestoreInProgress { get { return false; } }
		
		public void InitBilling () { Debug.Log ("Init Billing"); }
		
		public void RestorePurchases() { }
		
		public void PurchaseItem(string itemId) { }
		
		public bool IsPurchased(string itemId) { return false; }

		public bool IsConsumable(string itemId) { return false; }

		public string GetLocalizedPriceString(string itemId) { return ""; }

		public string GetISOCurrencySymbol(string itemId) {return "USD";}
		
		public decimal GetPriceInLocalCurrency(string itemId) {return 0;}

		public void ClearTransactions() { }

	}
}