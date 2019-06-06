using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.Purchasing.Security;
using System.Collections.Generic;
using TabTale;
using UnityEngine.Purchasing.MiniJSON;

namespace TabTale.Plugins.PSDK {
	public class PsdkBillingService : IPsdkBilling, IStoreListener {
		//Subscription need it
		public static PsdkBillingService Instance = null;

		private IStoreController _storeController;
		private IAppleExtensions _appleExtensions;
		private bool _isInitialised = false;
		private bool _isPurchaseInProgress = false;
		private bool _isRestoreInProgress = false;
		private InAppPurchasableItem _iapInValidation;
		private PurchaseEventArgs _iapInValidationArgs;
		private string _noAdsIapIds = "";

		private Dictionary<string,string> _iapMap;

		public PsdkBillingService(IPsdkServiceManager sm){}

		public bool IsInitialised()
		{
			return _isInitialised;
		}

        //Subscription need it
		public IStoreController storeController {
            get { return _storeController; }
        }

		public bool IsPurchaseInProgress()
		{
			return _isPurchaseInProgress;
		}
			
		public bool IsRestoreInProgress()
		{
			return _isRestoreInProgress;
		}
			
		public void InitBilling()
		{
			//Subscription need it
			if (Instance == null)
                Instance = this;

			if (_isInitialised) {
				Debug.Log("Billing::InitBilling already init");
				return;
			}
			Debug.Log("Billing::InitBilling");

			// On Android restore is called automatically on app start, we want to be aware of it and not report to analytics as revenue
			if(Application.platform == RuntimePlatform.Android)
			{
				_isRestoreInProgress = true;
			}

			var module = StandardPurchasingModule.Instance();

			// The FakeStore supports: no-ui (always succeeding), basic ui (purchase pass/fail), and 
			// developer ui (initialization, purchase, failure code setting). These correspond to 
			// the FakeStoreUIMode Enum values passed into StandardPurchasingModule.useFakeStoreUIMode.
			module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;


			var builder = ConfigurationBuilder.Instance(module);

			string googlePublicKey = PSDKMgr.Instance.GetExternalConfiguration().GetGooglePlayLicenceKey();
			if(Application.platform == RuntimePlatform.Android)
			{
				if(string.IsNullOrEmpty(googlePublicKey))
				{
					Debug.Log("Billing::InitBilling:Missing google public key");
				}

			}

			ReadProductInfo(builder);

			#if !AMAZON
			builder.Configure<IGooglePlayConfiguration>().SetPublicKey(googlePublicKey);
			#endif

			UnityPurchasing.Initialize(this, builder);
		}


		public void RestorePurchases()
		{
			// If Purchasing has not yet been set up ...
			if (!IsInitialisedWithLogging())
			{
				// ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
				Debug.Log("Billing::RestorePurchases: FAIL. Not initialized.");
				return;
			}

			// If we are running on an Apple device ... 
			if (Application.platform == RuntimePlatform.IPhonePlayer || 
				Application.platform == RuntimePlatform.OSXPlayer)
			{
				// ... begin restoring purchases
				Debug.Log("Billing::RestorePurchases started ...");

				_isRestoreInProgress = true;

				// Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
				_appleExtensions.RestoreTransactions(OnTransactionsRestored);
			}
			else
			{
				// We are not running on an Apple device. No work is necessary to restore purchases.
				Debug.Log("Billing::RestorePurchases: FAIL. Not supported on this platform. Current = " + Application.platform);
			}
		}

		void OnTransactionsRestored(bool success)
		{
			Debug.Log("Billing::TransactionsRestored : " + success);

			_isRestoreInProgress = false;

			PsdkEventSystem.Instance.NotifyOnBillingPurchaseRestored(success);
		}

		public void PurchaseItem(string itemId)
		{
			Debug.Log("Billing::PurchaseItem:" + itemId);

			if(_isPurchaseInProgress)
			{
				Debug.Log("Billing::PurchaseItem:attempted to buy iap while another purchase is in progress. id: " + itemId);
				return;
			}

			_isPurchaseInProgress = true;

			// An actual attempt to purchase was made, this means a restore is not in progress
			_isRestoreInProgress = false;

			string finalId = GetFinalId(itemId);         
            if (_storeController == null) {
                _isPurchaseInProgress = false;
                return;
            }

			if (finalId != null)
				_storeController.InitiatePurchase (_storeController.products.WithStoreSpecificID (finalId));
			else { 
				Debug.LogError ("Billing::PurchaseItem:" + itemId + " does not exist as iapId or id");
				_isPurchaseInProgress = false;
			}
		}

		public bool IsPurchased(string itemId)
		{
			string finalId = GetFinalId(itemId);
			if(! IsInitialisedWithLogging())
				return false;

			Product item = _storeController.products.WithStoreSpecificID(finalId != null ? finalId : "");

			if(item == null)
			{
				Debug.Log("Billing::IsPurchased:Error, Cannot find item - " + finalId);
				return false;
			}

			// Unity do not persist the receipt between app restarts, so we save the purchases locally
			return IsSavedPurchased(item.definition.id);
		}

		public bool IsConsumable(string itemId)
		{
			if(! IsInitialisedWithLogging())
				return false;
			string finalId = GetFinalId(itemId);
			if(finalId != null)
				return _storeController.products.WithStoreSpecificID(finalId).definition.type == ProductType.Consumable;
			else 
				return false;
		}

		public string GetLocalizedPriceString(string itemId)
		{
			Debug.Log ("Billing::GetLocalizedPriceString:" + itemId);

			if(! IsInitialisedWithLogging())
				return "";

			string finalId = GetFinalId(itemId);
			Product item = null;
			if(finalId != null)
				item = _storeController.products.WithStoreSpecificID(finalId);

			if(item == null)
			{
				Debug.Log ("Billing::GetLocalizedPriceString:Error, cannot find item by id: " + itemId);
				return "";
			}

			return item.metadata.localizedPriceString;
		}

		public string GetISOCurrencySymbol(string itemId)
		{
			if(! IsInitialisedWithLogging())
				return "";
			string finalId = GetFinalId(itemId);
			Product item = null;
			if(finalId != null)
				item = _storeController.products.WithStoreSpecificID(finalId);
			if(item == null)
			{
				Debug.Log ("Billing::GetISOCurrencySymbol:Error, cannot find item by id: " + itemId);
				return "";
			}

			return item.metadata.isoCurrencyCode;
		}

		public decimal GetPriceInLocalCurrency(string itemId)
		{
			if(! IsInitialisedWithLogging())
				return 0;
			string finalId = GetFinalId(itemId);
			Product item = null;
			if(finalId != null)
				item = _storeController.products.WithStoreSpecificID(finalId);
			if(item == null)
			{
				Debug.Log ("Billing::GetPriceInLocalCurrency:Error, cannot find item by id: " + itemId);
				return 0;
			}

			return item.metadata.localizedPrice;

		}

		public void ClearTransactions()
		{
			PlayerPrefs.DeleteKey("PSDKBillingIAPs");
		}

		public bool IsNoAdsItem(string itemId)
		{
			string finalId = GetFinalId(itemId);
			Product item = null;
			if(finalId != null)
				return _noAdsIapIds.Contains(finalId);
			else
				return false;
		}

		private void reportPurchaseCampaignResult(bool sucess)
		{
			Debug.Log("PsdkBillingService::reportPurchaseCampaignResult - sucess - " + sucess.ToString());
			PSDKMgr.Instance.GetInAppPurchase().PurchaseCampaignCompleteTransaction(sucess);
		}


		#region IStoreListener implementation

		public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
		{
			Debug.Log("Billing::OnInitialized");

			_isInitialised = true;
			_storeController = controller;

			if (Application.platform == RuntimePlatform.IPhonePlayer || 
				Application.platform == RuntimePlatform.OSXPlayer)
			{
				// Fetch the Apple store-specific subsystem.
				_appleExtensions = extensions.GetExtension<IAppleExtensions> ();

				// On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
				// On non-Apple platforms this will have no effect; OnDeferred will never be called.
				_appleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
			}

			PsdkEventSystem.Instance.NotifyOnBillingInit(BillerErrors.NO_ERROR);

			foreach(Product product in _storeController.products.all){
				PSDKMgr.Instance.GetInAppPurchase().AddProduct(product.definition.storeSpecificId,product.metadata.localizedPriceString,IsPurchased(product.definition.storeSpecificId));
			}

		}

		private void OnDeferred(Product item)
		{
			Debug.Log("Billing::OnDeferred:Purchase deferred: " + item.definition.id);
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			_isInitialised = false;

			Debug.Log("Billing::Unity billing failed to initialize!");
			switch (error)
			{
			case InitializationFailureReason.AppNotKnown:
				Debug.Log("Billing::Failed to initialize - Is your App correctly uploaded on the relevant publisher console?");
				PsdkEventSystem.Instance.NotifyOnBillingInit(BillerErrors.APP_NOT_KNOWN);
				break;
			case InitializationFailureReason.PurchasingUnavailable:
				// Ask the user if billing is disabled in device settings.
				Debug.Log("Billing::Failed to initialize - Billing disabled!");
				PsdkEventSystem.Instance.NotifyOnBillingInit(BillerErrors.PURCHASING_UNAVAILABLE);
				break;
			case InitializationFailureReason.NoProductsAvailable:
				// Developer configuration error; check product metadata.
				Debug.Log("Billing::Failed to initialize - No products available for purchase!");
				PsdkEventSystem.Instance.NotifyOnBillingInit(BillerErrors.NO_PRODUCTS_AVAILABLE);
				break;
			}
		}

		public void OnPurchaseFailed (Product item, PurchaseFailureReason reason)
		{
			Debug.Log("Billing::OnPurchaseFailed:" + item.metadata.localizedTitle);

			PurchaseIAPResultCode resultCode = PurchaseIAPResultCode.Failed;
			BillerErrors error = BillerErrors.NO_ERROR;
			InAppPurchasableItem inApp = CreatePurchasableInAppItem(item);

			switch(reason)
			{
			case PurchaseFailureReason.ExistingPurchasePending:
				error = BillerErrors.ATTEMPTING_TO_PURCHASE_PRODUCT_WITH_SAME_RECEIPT;
				break;
			case PurchaseFailureReason.PaymentDeclined:
				error = BillerErrors.PAYMENT_DECLINED;
				break;
			case PurchaseFailureReason.ProductUnavailable:
				error = BillerErrors.PRODUCT_UNAVAILABLE;
				break;
			case PurchaseFailureReason.PurchasingUnavailable:
				error = BillerErrors.PURCHASING_UNAVAILABLE;
				break;
			case PurchaseFailureReason.SignatureInvalid:
				error = BillerErrors.REMOTE_VALIDATION_FAILED;
				break;
			case PurchaseFailureReason.Unknown:
				error = BillerErrors.UNKNOWN;
				break;
			case PurchaseFailureReason.UserCancelled:
				resultCode = PurchaseIAPResultCode.Cancelled;
				break;
			}

			_isPurchaseInProgress = false;

			PurchaseIAPResult result = new PurchaseIAPResult(inApp, resultCode, error);
			PsdkEventSystem.Instance.NotifyOnBillingPurchased(result);
			reportPurchaseCampaignResult (false);
		}

		public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
		{
			Debug.Log("Billing::Processing Purchase: " + e.purchasedProduct.definition.id);
			Debug.Log("Billing::Receipt: " + e.purchasedProduct.receipt);
			Debug.Log("Billing::Transaction Id: " + e.purchasedProduct.transactionID);

			_iapInValidation = CreatePurchasableInAppItem(e);
			_iapInValidationArgs = e;

			// If restore is in progress - Skip receipt validation ! We have to do this because the server will not validate a receipt that has already been validated in the past
			if(_isRestoreInProgress)
			{
				// Indicate we have handled this purchase, we will not be informed of it again.

				Debug.Log("Billing::ProcessPurchase : Skipping receipt validation since restore is in progress");

				if (IsNoAdsItem (_iapInValidation.id)) {
					Debug.Log ("Billing::ProcessPurchase : IsNoAdsItem=true calling PurchaseAd");
					PSDKMgr.Instance.PurchaseAd ();
				} 
				else {
					Debug.Log ("Billing::ProcessPurchase : IsNoAdsItem=false");
				}

				PurchaseIAPResult result = new PurchaseIAPResult(_iapInValidation, PurchaseIAPResultCode.Success, BillerErrors.NO_ERROR);

				SavePurchaseLocally(_iapInValidation.id);

				PsdkEventSystem.Instance.NotifyOnBillingPurchased(result);

				_isPurchaseInProgress = false;

				PSDKMgr.Instance.GetInAppPurchase().ItemPurchased(e.purchasedProduct.definition.storeSpecificId);

				return PurchaseProcessingResult.Complete;
			}

			Dictionary<string,object> recieptJson = Json.Deserialize(_iapInValidation.receipt) as Dictionary<string,object>;
			string payload = recieptJson.Get<string>("Payload");

			string purchaseToken = null;

			#if UNITY_ANDROID
			if (payload != null) {
				Dictionary<string,object> tmp = Json.Deserialize(payload) as Dictionary<string,object>;
				if (tmp != null) {
					string tmpStr = tmp.Get<string> ("json");
					if (tmpStr != null) {
						Dictionary<string,object> innerTmp = Json.Deserialize(tmpStr) as Dictionary<string,object>;
						if (innerTmp != null) {
							purchaseToken = innerTmp.Get<string> ("purchaseToken");
						}
						else {
							Debug.Log ("ProcessPurchase failed to Deserialize json");
						}
					}
					else {
						Debug.Log ("ProcessPurchase failed to find json attribut in payload");
					}
				}
				else {
					Debug.Log ("ProcessPurchase failed to Deserialize payload");
				}
			} 
			else {
				Debug.Log ("ProcessPurchase failed to find payload attribute");
			}
			#else
			purchaseToken = payload;
			#endif
			if (purchaseToken != null) {
				Debug.Log ("ProcessPurchase purchaseToken: " + purchaseToken);
				PSDKMgr.Instance.ValidateReceiptAndReport (purchaseToken, _iapInValidation.localizedPrice, _iapInValidation.currency, _iapInValidation.id);
			}
			else {
				Debug.Log ("ProcessPurchase purchaseToken was not found.");
			}

			reportPurchaseCampaignResult (true);

			OnValidPurchaseResponseEvent (_iapInValidation.localizedPrice, _iapInValidation.currency, _iapInValidation.id, true);
			if(!IsConsumable(e.purchasedProduct.definition.storeSpecificId)){
				PSDKMgr.Instance.GetInAppPurchase().ItemPurchased(e.purchasedProduct.definition.storeSpecificId);
			}

			return PurchaseProcessingResult.Complete;
		}

		#endregion


		// Initialisation validation
		private bool IsInitialisedWithLogging()
		{
			if( ! _isInitialised)
				Debug.Log("Attempted to use purchasing api when unity iap is not initialized. check Failed to initialize in the log for more details");
			return _isInitialised;
		}


		private void SavePurchaseLocally(string iapId)
		{
			string iaps = PlayerPrefs.GetString("PSDKBillingIAPs", "");
			if(iaps.Length > 0)
				iaps += ";";
			iaps += iapId;
			PlayerPrefs.SetString("PSDKBillingIAPs",iaps);
		}

		private bool IsSavedPurchased(string iapId)
		{
			string saveData = PlayerPrefs.GetString("PSDKBillingIAPs", "");
			string[] splitData = saveData.Split(new char[]{ ';' });
			bool isPurchased = false;
			foreach (string item in splitData)
			{
				if (item.Equals(iapId))
				{
					isPurchased = true;
					break;
				}
			}
			return isPurchased;
		}

		#region wrappers

		private InAppPurchasableItem CreatePurchasableInAppItem(PurchaseEventArgs purchaseEventArgs)
		{
			Product product = purchaseEventArgs.purchasedProduct;

			return CreatePurchasableInAppItem(product);
		}

		private InAppPurchasableItem CreatePurchasableInAppItem(Product product)
		{
			InAppPurchasableItem purchasableInAppItem;
			purchasableInAppItem.id = product.definition.id;
			purchasableInAppItem.name = product.metadata.localizedTitle;
			purchasableInAppItem.description = product.metadata.localizedDescription;
			purchasableInAppItem.localizedPrice = product.metadata.localizedPrice.ToString();
			purchasableInAppItem.receipt = product.receipt;
			purchasableInAppItem.transactionId = product.transactionID;
			purchasableInAppItem.currency = product.metadata.isoCurrencyCode;
			return purchasableInAppItem;
		}

		private void ResetPurchasableItem(InAppPurchasableItem purchasableInAppItem)
		{
			purchasableInAppItem.id = null;
			purchasableInAppItem.name = null;
			purchasableInAppItem.description = null;
			purchasableInAppItem.localizedPrice = null;
			purchasableInAppItem.receipt = null;
			purchasableInAppItem.transactionId = null;
			purchasableInAppItem.currency = null;
		}

		#endregion

		internal void OnValidPurchaseResponseEvent(string price, string currency, string itemId, bool isValid)
		{
			Debug.Log("Billing::OnValidPurchaseResponseEvent: Receipt validation result : " + isValid);

			PurchaseIAPResult result;
			if(isValid)
			{
				result = new PurchaseIAPResult(_iapInValidation, PurchaseIAPResultCode.Success, BillerErrors.NO_ERROR);
				SavePurchaseLocally(_iapInValidation.id);
				if (IsNoAdsItem (_iapInValidation.id)) {
					Debug.Log ("Billing::OnValidPurchaseResponseEvent IsNoAdsItem=true calling PurchaseAd");
					PSDKMgr.Instance.PurchaseAd ();
				} 
				else {
					Debug.Log ("Billing::OnValidPurchaseResponseEvent IsNoAdsItem=false");
				}
			}
			else
			{
				Debug.Log ("Billing::OnValidPurchaseResponseEvent item is not valid");
				result = new PurchaseIAPResult(_iapInValidation, PurchaseIAPResultCode.Failed, BillerErrors.REMOTE_VALIDATION_FAILED);	
			}
				

			PsdkEventSystem.Instance.NotifyOnBillingPurchased(result);

			_isPurchaseInProgress = false;

			_storeController.ConfirmPendingPurchase(_iapInValidationArgs.purchasedProduct);

			ResetPurchasableItem(_iapInValidation);
			_iapInValidationArgs = null;

		}

		private void ReadProductInfo(ConfigurationBuilder builder)
		{
			_iapMap = new Dictionary<string,string>();
			List<object> productArr = PSDKMgr.Instance.LocalConfig.GetArray(new string[] {"billing","iaps"});
			if(productArr != null){
				foreach(object productObj in productArr){
					if(productObj.GetType() == typeof(Dictionary<string,object>)){
						Dictionary<string,object> productDict = productObj as Dictionary<string,object>;
						string productTypeStr = productDict.GetString("type","");
						ProductType productType;
						if(TryParseProductType(productTypeStr,out productType)){
							string iapId = productDict.GetString("iapId","");
							string itemId = productDict.GetString("id","");
							bool isNoAds = productDict.GetBool("noAds");
							if(itemId.Length > 0 && iapId.Length > 0){
								Debug.Log("Billing::ReadProductInfo:Adding Product - iapId = " + iapId + ", id = " + itemId + ", type = " + productTypeStr + ", noAds = " + isNoAds.ToString());
								_iapMap.Add(itemId,iapId);
								if(isNoAds){
									_noAdsIapIds += ";" + iapId;
								}

								#if AMAZON
								const string androidStore = AmazonApps.Name;
								#else
								const string androidStore = GooglePlay.Name;
								#endif
								Debug.Log("Billing::ReadProductInfo:" + androidStore + " selected");
								string storeName = (Application.platform == RuntimePlatform.IPhonePlayer) ? AppleAppStore.Name : androidStore;
								var storeList = new List<string> { storeName };
								string[] stores = storeList.ToArray();

								builder.AddProduct(iapId,productType, new IDs {
									{iapId,stores}
								});
							}
						}
					}
				}
			}
		}

		private bool TryParseProductType(string productTypeStr, out ProductType productType)
		{
			productType = ProductType.Consumable;
			if (productTypeStr == "consumable") {
				productType = ProductType.Consumable;
				return true;
			}
			else if (productTypeStr == "non-consumable") {
				productType = ProductType.NonConsumable;
				return true;
			}
			else if (productTypeStr == "subscription") {
				productType = ProductType.Subscription;
				return true;
			}
			else {
				Debug.LogError("Billing::ParseProductType: failed to parse product type - " + productTypeStr);
				return false;
			}
		}

		public void psdkStartedEvent()
		{
			
		}

		private string GetFinalId(string itemId)
		{
			string finalId = null;

			if(_iapMap.Values.Contains(itemId)){
				finalId = itemId;
			}
			else {
				if(_iapMap.Keys.Contains(itemId)){
					_iapMap.TryGetValue(itemId, out finalId);
				}
			}
			return finalId;
		}

		public string GetMainId(string iapId)
		{
			if(_iapMap.ContainsValue(iapId)){
				foreach(string key in _iapMap.Keys){
					string val = null;
					_iapMap.TryGetValue(key, out val);
					if(val == iapId){
						return key;
					}
				}
			}
			return null;
		}
	}
}
