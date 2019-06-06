using strange.extensions.command.impl;
using UnityEngine;

namespace TabTale
{
    public class PurchaseInAppItemCommand : Command
    {
        [Inject]
        public IBillingService billingService { get; set; }

		[Inject]
		public IAPConfigData iapConfigData {get;set;}

		[Inject]
		public IModalityManager modalityManager {get;set;}

		[Inject]
		public PurchasedIAPSignal purchasedIAPSignal {get;set;}

		[Inject]
		public GeneralParameterConfigModel generalParamtersConfigModel {get;set;}

		[Inject]
		public IAPConfigModel iapConfigModel { get; set; }

		[Inject]
		public StoreManager storeManager {get;set;}

		IModalHandle _waitingModal = null;

        public override void Execute()
        {
			Debug.Log("PurchaseInAppItemCommand.Execute of "+ iapConfigData.id);

			AddListeners();

			ShowWaitingModalIfNeeded();

			// After restoring items, we set a flag to know purchases were restored. While the flag is set we won't report any purchases to analytics.
			// Because of this, when attempting to purchase a new item we want to report to appsflyer analytics in case the purchase succeeded, so
			// here the flag has to be reset:
			PlayerPrefs.SetInt("restoredPurchases",0);

			string iapId = iapConfigModel.GetIAPId(iapConfigData);
			billingService.PurchaseItem(iapId);

			Retain ();

        }

		private void AddListeners()
		{
			purchasedIAPSignal.AddListener(HandleOnPurchaseEnded);
		}

		public void HandleOnPurchaseEnded(PurchaseIAPResult purchaseResult)
		{
			if(_waitingModal != null)
				_waitingModal.Close();
			
			Release ();
		}

		private void ShowWaitingModalIfNeeded()
		{
			// On ios - display a waiting modal to improve UI experience of first purchase (which is slow on ios)
			if(Application.platform == RuntimePlatform.IPhonePlayer || 
				Application.platform == RuntimePlatform.OSXEditor || 
				Application.platform == RuntimePlatform.WindowsEditor)
			{
				// Should be replaced by using a gsdk-only configuration table
				bool shouldShowModalDuringPurchase = generalParamtersConfigModel.GetBool("ShouldShowWaitingModal", true);

				if(shouldShowModalDuringPurchase)
				{
					_waitingModal = new AppModalHandle ("GamePopups/WaitForPurchaseModal", IModalMaskType.Masked);
					modalityManager.Add (_waitingModal, true, true);
				}
			}
		}

		private void Release()
		{
			RemoveListeners();

			base.Release();
		}

		private void RemoveListeners()
		{
			purchasedIAPSignal.RemoveListener(HandleOnPurchaseEnded);
		}
    }
}
