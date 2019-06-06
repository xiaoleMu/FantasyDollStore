using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace TabTale {
    
	[RequireComponent(typeof(Button))]
	public class BuyItemButtonView : GameView 
    {
		[Inject]
		public IBillingService billingService { get; set; }
        [Inject]
        public ItemConfigModel itemConfigModel { get; set; }
		[Inject]
		public IAPConfigModel iapConfigModel { get; set; }
        [Inject]
        public SoundManager soundManager { get; set; }
        [Inject]
        public NetworkCheck networkCheck { get; set; }
		[Inject]
		public StoreManager storeManager { get; set; }
		[Inject]
		public SettingsStateModel settingsStateModel { get; set; }
		[Inject]
		public PurchasesRestoredSignal purchasesRestoredSignal { get; set;}

        public string itemID;
	    public ItemConfigData itemConfig;
		public Text price;
		public bool hideOnPurchase = false;

		private string _iapConfigId = "";
		private string _storeSpecificId = "";

	    protected override void OnRegister()
	    {
	        base.OnRegister();

            itemConfig = itemConfigModel.GetItem(itemID);

			_iapConfigId = itemConfigModel.GetIAPConfigID(itemConfig.id);

			if(! _iapConfigId.IsNullOrEmpty())
			{
				_storeSpecificId = iapConfigModel.GetIAPId(iapConfigModel.GetIAP(_iapConfigId));
			}
				
			if ( !IsItemPurchased() )
                UpdatePrice();


			RefreshButtonUI();

	    }

		protected void OnEnable()
		{
			RefreshButtonUI();
		}

		protected override void AddListeners()
		{
			base.AddListeners();
			purchasesRestoredSignal.AddListener(OnPurchasesRestored);
		}

		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			purchasesRestoredSignal.RemoveListener(OnPurchasesRestored);
		}

	    protected override void OnUnRegister()
	    {
	        base.OnUnRegister();
	    }

		public bool IsItemPurchased()
		{
			bool isPurchased = billingService.IsPurchased(_storeSpecificId);
			return !IsConsumableItem() && isPurchased;
		}

        public virtual void OnClick()
        {
            Debug.Log("BuyItemButton itemId:"+itemID+" click");
            soundManager.PlaySound(SoundMapping.Map.GeneralButtonClick, SoundLayer.Main);

			if(networkCheck.HasInternetConnection())
			{
				storeManager.BuyItem(itemConfig.id).Done(result => 
					{
						if(result.resultCode == BuyItemResultCode.Success)
						{
							RefreshButtonUI();
						}
					});
			}
			else
				networkCheck.ShowNoConnectionPopup();
        }

		private bool IsConsumableItem()
		{
			if (! _iapConfigId.IsNullOrEmpty())
			{
				return billingService.IsConsumable(_storeSpecificId);
			}
		    else
			{
		        return false;
			}
		}

		private bool IsNoAdsPurchased ()
		{
			return (itemConfig.id.Equals (ItemConfigModel.NO_ADS_ID) && 
				!settingsStateModel.ShowingAds ());
		}

		private void UpdatePrice()
		{
			string itemPrice = billingService.GetLocalizedPriceString(_storeSpecificId);

			if(price != null && !string.IsNullOrEmpty(itemPrice))
				price.text = itemPrice;
		}

		private void RefreshButtonUI()
		{
			bool isPurchased = billingService.IsPurchased(_storeSpecificId);

			if( ! IsConsumableItem() && (isPurchased || IsNoAdsPurchased()))
			{
				gameObject.GetComponent<Button>().interactable = false;

				if(hideOnPurchase)
					gameObject.SetActive(false);

			}
		}

		private void OnPurchasesRestored(bool success)
		{
			RefreshButtonUI();
		}
    }

}
