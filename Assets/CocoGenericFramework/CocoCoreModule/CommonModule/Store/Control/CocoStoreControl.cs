using System;
using System.Collections.Generic;
using Game;
using TabTale;
using TabTale.Plugins.PSDK;
using UnityEngine;
using UnityEngine.Purchasing;

namespace CocoPlay
{
	public class CocoStoreControl : GameView
	{
		protected override void Awake ()
		{
			if (!RegisterInstance ()) {
				return;
			}

			base.Awake ();

			Init ();
		}

		protected override void AddListeners ()
		{
			base.AddListeners ();

			PurchasesRestoredSignal.AddListener (OnPurchasesRestored);
			IAPPurchaseDoneSignal.AddListener (OnIAPPurchaseDone);
		}

		protected override void RemoveListeners ()
		{
			PurchasesRestoredSignal.RemoveListener (OnPurchasesRestored);
			IAPPurchaseDoneSignal.RemoveListener (OnIAPPurchaseDone);

			base.RemoveListeners ();
		}


		#region Instance

		private static CocoStoreControl _instance;

		public static CocoStoreControl Instance {
			get { return _instance; }
		}

		private bool RegisterInstance ()
		{
			if (_instance == null) {
				_instance = this;
				return true;
			}

			if (_instance == this) {
				return true;
			}

			Destroy (this);
			return false;
		}

		#endregion


		#region Properties

		[Inject]
		public IBillingService BillingService { get; set; }

		[Inject]
		public NetworkCheck Network { get; set; }

		private PsdkBillingService _psdkBilling;

		public PsdkBillingService PSDKBilling {
			get { return _psdkBilling ?? (_psdkBilling = PSDKMgr.Instance.GetBilling () as PsdkBillingService); }
		}

		private bool IsBillingAvailable {
			get {
				// disable network check on iOS
#if !UNITY_IOS
				if (!Network.HasInternetConnection ()) {
					return false;
				}
#endif
				return BillingService.IsInitialised;
			}
		}

		private bool IsManualRestoreAvailable {
			get {
#if UNITY_IOS
				return true;
#else
				return false;
#endif
			}
		}

		#endregion


		#region Init

		[Inject]
		public ICocoStoreConfigData StoreConfigData { get; set; }

		private bool _isInitialised;

		public bool IsInitialised {
			get { return _isInitialised; }
		}

		private void Init ()
		{
			if (BillingService.IsInitialised) {
				OnBillingInit (BillerErrors.NO_ERROR);
			} else {
				PsdkEventSystem.Instance.onBillingInit += OnBillingInit;
			}
		}

		private void OnBillingInit (BillerErrors errors)
		{
			InitDone (errors == BillerErrors.NO_ERROR);
		}

		private void InitDone (bool result)
		{
			if (!result) {
				return;
			}

			RefreshAllItemState ();
			StoreUpdateStateSignal.Dispatch ();

			_isInitialised = true;
		}

		private void RefreshAllItemState ()
		{
			var psdkBilling = PSDKBilling;
			if (psdkBilling == null) {
				return;
			}

			var products = PSDKBilling.storeController.products;
			foreach (var product in products.all) {
				var itemKey = psdkBilling.GetMainId (product.definition.storeSpecificId);
				var itemData = StoreConfigData.GetItemData (itemKey);
				if (itemData == null) {
					continue;
				}

				itemData.ProductType = product.definition.type;
				UpdateItemPurchaseState (itemData, psdkBilling.IsPurchased (itemData.Key));
			}

			RefreshNoAdsState ();
		}

		#endregion


		#region Purchase

		[Inject]
		public CocoStoreUpdateStateSignal StoreUpdateStateSignal { get; set; }

		[Inject]
		public StoreManager StoreManager { get; set; }

		public bool IsPurchased (CocoStoreID id)
		{
			return IsPurchased (id, true);
		}

		private bool IsPurchased (CocoStoreID id, bool legacyNoAds)
		{
			if (IsGodModeEnabled) {
				return true;
			}

			// for backward compatibility, will remove in future
 #pragma warning disable 618
			if (IsLegacyFullVersionPurchased) {
 #pragma warning restore 618
				return true;
			}

			// for backward compatibility, will remove in future
 #pragma warning disable 618
			if (legacyNoAds && IsLegacyNoAdsPurchased (id)) {
 #pragma warning restore 618
				return true;
			}

			var purchased = IsPurchased (id, new HashSet<CocoStoreID> (), true);
			// Debug.LogError ("-------- " + id + " -------- " + purchased);
			return purchased;
		}

		private bool IsPurchased (CocoStoreID id, HashSet<CocoStoreID> excludedParentIds, bool checkSlots)
		{
			var itemData = StoreConfigData.GetItemData (id);
			if (itemData == null) {
				return false;
			}

			// check if self is purchased
			if (itemData.IsPurchased) {
				return true;
			}

			excludedParentIds.Add (itemData.Id);

			// check if any parent is purchased
			foreach (var parentId in itemData.ParentIds) {
				if (excludedParentIds.Contains (parentId)) {
					continue;
				}

				if (IsPurchased (parentId, excludedParentIds, false)) {
					return true;
				}
			}

			// check slots
			if (!checkSlots || itemData.SlotIds.Count <= 0) {
				return false;
			}

			// check if all slots are purchased
			foreach (var slotId in itemData.SlotIds) {
				if (!IsPurchased (slotId, excludedParentIds, true)) {
					return false;
				}
			}

			return true;
		}

		public void Purchase (CocoStoreID id, Action<BuyItemResult> doneAction = null)
		{
			var itemData = StoreConfigData.GetItemData (id);
			if (itemData == null) {
				if (doneAction != null) {
					doneAction (new BuyItemResult (BuyItemResultCode.Error));
				}
				return;
			}

			if (!IsBillingAvailable) {
				if (doneAction != null) {
					doneAction (new BuyItemResult (BuyItemResultCode.Error));
				}
//				CocoMainController.ShowNoInternetPopup ();
				return;
			}

			StoreManager.BuyItem (itemData.Key).Done (result => OnPurchaseDone (itemData, result, doneAction));
		}

		private void OnPurchaseDone (CocoStoreItemData itemData, BuyItemResult result, Action<BuyItemResult> doneAction)
		{
			if (doneAction != null) {
				doneAction (result);
			}

			if (result.resultCode != BuyItemResultCode.Success) {
				return;
			}

			UpdateItemPurchaseState (itemData, true);
			RefreshNoAdsState ();

			StoreUpdateStateSignal.Dispatch ();
		}

		private void UpdateItemPurchaseState (CocoStoreItemData itemData, bool purchased)
		{
			switch (itemData.ProductType) {
			case ProductType.NonConsumable:
			case ProductType.Subscription:
				itemData.IsPurchased = purchased;
				break;
			}
		}

		#endregion


		#region Restore

		[Inject]
		public RequestRestoreSignal RequestRestoreSignal { get; set; }

		[Inject]
		public PurchasesRestoredSignal PurchasesRestoredSignal { get; set; }

		[Inject]
		public IAPPurchaseDoneSignal IAPPurchaseDoneSignal { get; set; }

		private bool _isRestoreManually;

		private void OnPurchasesRestored (bool result)
		{
			_isRestoreManually = false;
			InitDone (result);
		}

		private void OnIAPPurchaseDone (PurchaseIAPResult iapResult)
		{
			if (!IsAutoRestoreInProgress) {
				return;
			}

			var itemKey = PSDKBilling.GetMainId (iapResult.purchasedItem.id);
			var itemData = StoreConfigData.GetItemData (itemKey);
			if (itemData == null) {
				return;
			}

			var resultCode = (iapResult.result == PurchaseIAPResultCode.Success) ? BuyItemResultCode.Success : BuyItemResultCode.InAppPurchaseFailed;
			var buyItemResult = new BuyItemResult(resultCode, iapResult);
			OnPurchaseDone (itemData, buyItemResult, null);
		}

		private bool IsAutoRestoreInProgress {
			get {
				if (_isRestoreManually) {
					return false;
				}

				var psdkBilling = PSDKBilling;
				if (psdkBilling == null) {
					return false;
				}

				return psdkBilling.IsRestoreInProgress () && !psdkBilling.IsPurchaseInProgress ();
			}
		}

		public void RequestRestore ()
		{
			if (!IsManualRestoreAvailable) {
				return;
			}

			if (!IsBillingAvailable) {
//				CocoMainController.ShowNoInternetPopup ();
				return;
			}

			_isRestoreManually = true;
			RequestRestoreSignal.Dispatch ();
		}

		#endregion


		#region NoAds

		[Inject]
		public SettingsStateModel SettingsStateModel { get; set; }

		public bool IsNoAds {
			get { return !SettingsStateModel.ShowingAds (); }
		}

		public void RefreshNoAdsState ()
		{
			var noAds = IsAnyNoAdsItemPurchased;
			if (noAds == IsNoAds) {
				return;
			}

			SettingsStateModel.SetNoAds (noAds);
			Debug.LogError ("noAds: " + noAds);
		}

		private bool IsAnyNoAdsItemPurchased {
			get {
				var psdkBilling = PSDKBilling;
				if (psdkBilling == null) {
					return false;
				}

				foreach (var itemData in StoreConfigData.ItemDatas) {
					if (IsNoAdsItemPurchased (itemData, psdkBilling)) {
						return true;
					}
				}
				return false;
			}
		}

		private bool IsNoAdsItemPurchased (CocoStoreItemData itemData, PsdkBillingService psdkBilling)
		{
			return IsPurchased (itemData.Id, false) && psdkBilling.IsNoAdsItem (itemData.Key);
		}

		#endregion


		#region God Mode

		[Inject]
		public CocoGlobalRecordModel GlobalRecordModel { get; set; }

		private const string GOD_MODE_KEY = "store_god_mode";

		private bool _isGodModeInited;

		private bool _isGodModeEnabled;

		public bool IsGodModeEnabled {
			get {
				if (!_isGodModeInited) {
					_isGodModeEnabled = GlobalRecordModel.GetBool (GOD_MODE_KEY);
					_isGodModeInited = true;
				}

				return _isGodModeEnabled;
			}
			private set { _isGodModeEnabled = value; }
		}

		public void EnableGodMode ()
		{
			if (IsGodModeEnabled) {
				return;
			}

			IsGodModeEnabled = true;
			GlobalRecordModel.SetData (GOD_MODE_KEY, IsGodModeEnabled);

			StoreUpdateStateSignal.Dispatch ();
		}

		#endregion


		#region Other

		public string GetPriceString (CocoStoreID id, bool useISOCurrencySymbol = false)
		{
			var itemData = StoreConfigData.GetItemData (id);
			if (itemData == null) {
				return string.Empty;
			}

			if (!useISOCurrencySymbol) {
				return BillingService.GetLocalizedPriceString (itemData.Key);
			}

			var symbol = BillingService.GetISOCurrencySymbol (itemData.Key);
			return string.Format ("{0} {1}", symbol, BillingService.GetPriceInLocalCurrency (itemData.Key));
		}

		#endregion


		#region Backward compatibility

		[Obsolete ("for backward compatibility, will remove in future.")]
		private bool IsLegacyFullVersionPurchased {
			get {
				var fullVersionItemData = StoreConfigData.GetItemData (CocoStoreID.FullVersion);
				if (fullVersionItemData == null) {
					return false;
				}

				return fullVersionItemData.SlotIds.Count <= 0 && fullVersionItemData.IsPurchased;
			}
		}

		[Obsolete ("for backward compatibility, will remove in future.")]
		private bool IsLegacyNoAdsPurchased (CocoStoreID id)
		{
			return id == CocoStoreID.NoAds && IsNoAds;
		}

		[Obsolete ("for backward compatibility, will remove in future. please use 'Purchase' instead.")]
		public void Buy (CocoStoreID id, Action<BuyItemResult> doneAction = null)
		{
			Purchase (id, doneAction);
		}

		[Obsolete ("for backward compatibility, will remove in future. please use 'EnableGodMode' instead.")]
		public void GetGod ()
		{
			EnableGodMode ();
		}

		#endregion
	}
}