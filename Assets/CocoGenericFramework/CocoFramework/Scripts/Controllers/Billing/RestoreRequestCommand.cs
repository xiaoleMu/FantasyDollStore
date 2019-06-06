using UnityEngine;
using strange.extensions.command.impl;

namespace TabTale
{
	public class RequestRestoreCommand : Command
	{
		[Inject]
		public IBillingService billingService { get; set; }

		[Inject]
		public PurchasesRestoredSignal purchasedRestoredSignal { get; set; }

		public override void Execute()
		{
			purchasedRestoredSignal.AddListener(OnPurchasesRestored);
			Debug.Log ("RequestRestoreCommand.Execute - requesting restore");

			billingService.RestorePurchases();

			Retain();

		}

		private void OnPurchasesRestored(bool result)
		{
			Debug.Log ("RequestRestoreCommand.OnPurchasesRestored : result");
			Release();
		}
	}
}