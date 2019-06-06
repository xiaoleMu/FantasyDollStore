using System;
using UnityEngine;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using TabTale;
using System.Collections.Generic;
using TabTale.Publishing;

namespace TabTale 
{
	public class IAPPurchasedCommand : GameCommand
	{   
		[Inject]
		public PurchaseIAPResult purchaseResult { get; set; }

		[Inject]
		public ItemConfigModel itemConfigModel { get; set; }

		[Inject]
		public SettingsStateModel settingsStateModel { get; set; }

		[Inject]
		public IAPConfigModel iapConfigModel { get; set; }

		[Inject]
		public IBannerAds bannerAds { get; set; }

		[Inject]
		public CollectGameElementsSignal collectGameElementsSignal { get; set; }

		[Inject]
		public IAPPurchaseDoneSignal iapPurchaseDoneSignal { get; set; }

		private string _iapId;

		private ItemConfigData _purchasedItem;

		public override void Execute()
		{
			_iapId = purchaseResult.purchasedItem.id;

			logger.Log(Tag, "IAP id: " + _iapId);

			if(purchaseResult.result == PurchaseIAPResultCode.Success)
			{
				IAPConfigData iapConfig = iapConfigModel.GetIAPByPurchaseId(_iapId); 
				_purchasedItem = itemConfigModel.GetItemByIapConfigId(iapConfig.id);

				if(_purchasedItem == null)
				{
					logger.LogError(Tag, "Could not find item config id for iap + " + _iapId);
					Fail();
					return;
				}

				CollectPurchasedItem();

				if(DidPurchasedNoAds())
				{
					logger.Log(Tag, "PurchaseNoAdsCommand of " + _purchasedItem);

					settingsStateModel.SetNoAds();
					bannerAds.Hide();
				}

				EnablePromotionEligibility();

			}
			else
			{
				logger.Log(Tag, "Purchase failed");
				Fail();
			}

			iapPurchaseDoneSignal.Dispatch(purchaseResult);
		}

		private void CollectPurchasedItem()
		{
			GameElementData ge = itemConfigModel.ToGameElement(_purchasedItem);
			collectGameElementsSignal.Dispatch(new List<GameElementData> { ge });
		}

		private bool DidPurchasedNoAds()
		{
			bool itemHasIAP = !string.IsNullOrEmpty(_purchasedItem.iapConfigId);
			bool isNoAdsIAP = itemConfigModel.GetIAPConfigData(_purchasedItem.id).noAdsIap;

			logger.Log(Tag, string.Format("DidPurchaseNoAds - itemHasIAP:{0}, isNoAdsIAP:{1}", itemHasIAP, isNoAdsIAP));

			return itemHasIAP && isNoAdsIAP;
		}

		/// <summary>
		/// Once the user purchases any in-app he is eligible for promotion
		/// </summary>
		private void EnablePromotionEligibility()
		{
			settingsStateModel.SetPromotionEligibility(true);
		}
	}
}
