using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using strange.extensions.signal.impl;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

namespace TabTale 
{
	public class UnityIAPProvider : IBillingService
	{
		[Inject]
		public PurchasedIAPSignal purchasedIAPSignal { get; set; }

		[Inject]
		public BillerInitDoneSignal billerInitDoneSignal { get; set; }

		[Inject]
		public PurchasesRestoredSignal purchasesRestoredSignal { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		private const string Tag = "UnityIAPProvider";

		private IPsdkBilling _psdkBilling;

		#region properties

		public bool IsInitialised
		{
			get { return _psdkBilling.IsInitialised(); }
		}

		private bool _isPurchaseInProgress = false;
		public bool IsPurchaseInProgress
		{
			get { return _psdkBilling.IsPurchaseInProgress(); }
		}
			
		public bool IsRestoreInProgress
		{
			get { return _psdkBilling.IsRestoreInProgress(); }
		}

		#endregion

		[PostConstruct]
		public void Init()
		{
			Debug.Log("UnityIAPProvider.Init");

			_psdkBilling = PSDKMgr.Instance.GetBilling();

			PsdkEventSystem.Instance.onBillingPurchaseRestored += OnTransactionsRestored;
			PsdkEventSystem.Instance.onBillingPurchased += OnBillingPurchased;

		}

		void OnBillingPurchased (PurchaseIAPResult result)
		{
			purchasedIAPSignal.Dispatch(result);
		}

		// Initialisation validation
		private bool IsInitialisedWithLogging
		{
			get 
			{ 
				bool isInitialized = _psdkBilling.IsInitialised();
				if( ! isInitialized)
				{
					logger.LogError(Tag, "Attempted to use purchasing api when unity iap is not initialized. check Failed to initialize in the log for more details");
				}
				return isInitialized;

			}
		}

		public void InitBilling()
		{
			logger.Log(Tag,"InitBilling");

			_psdkBilling.InitBilling();
		}

		public void RestorePurchases()
		{
			_psdkBilling.RestorePurchases();
		}

		void OnTransactionsRestored(bool success)
		{
			purchasesRestoredSignal.Dispatch(success);
		}
		
		public void PurchaseItem(string itemId)
		{
			_psdkBilling.PurchaseItem(itemId);
		}
			
		public bool IsPurchased(string itemId)
		{
			return _psdkBilling.IsPurchased(itemId);
		}

		public bool IsConsumable(string itemId)
		{
			return _psdkBilling.IsConsumable(itemId);
		}

		public string GetLocalizedPriceString(string itemId)
		{
			return _psdkBilling.GetLocalizedPriceString(itemId);
		}

		public string GetISOCurrencySymbol(string itemId)
		{
			return _psdkBilling.GetISOCurrencySymbol(itemId);
		}

		public decimal GetPriceInLocalCurrency(string itemId)
		{
			return _psdkBilling.GetPriceInLocalCurrency(itemId);
		}

		public void ClearTransactions()
		{
			_psdkBilling.ClearTransactions();
		}
	}
}
