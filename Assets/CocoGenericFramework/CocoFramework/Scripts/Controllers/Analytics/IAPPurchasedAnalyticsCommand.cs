using UnityEngine;
using strange.extensions.command.impl;

namespace TabTale.Analytics
{
	public class IAPPurchasedAnalyticsCommand : GameCommand
	{
		[Inject]
		public IAPConfigModel iapConfigModel {get;set;}

		[Inject]
		public PurchaseIAPResult purchaseResult { get; set; }

		[Inject]
		public IAnalyticsService analyticsService {get;set;}

		[Inject]
		public IBillingService billingService { get; set; }

		[Inject]
		public IModalityManager modalityManager {get;set;}

		public override void Execute()
		{
			string iapId = purchaseResult.purchasedItem.id;

			// Report purchases to analytics only if restore purchases is not in progress
			if( ! billingService.IsRestoreInProgress)
			{
				analyticsService.ReportPurchase(billingService.GetPriceInLocalCurrency(iapId).ToString(), 
					billingService.GetISOCurrencySymbol(iapId),
					iapId);
			}
			else
			{
				logger.Log (Tag,"Execute - Skipping report to analytics since restore flag is set");
			}
		}
	}
}