using strange.extensions.command.impl;
using UnityEngine;

namespace TabTale
{
	public class BillingReadyCommand : Command
	{
		[Inject]
		public IBillingService billingService { get; set; }
		
		public override void Execute()
		{
			if (Debug.isDebugBuild) Debug.Log("BillingReadyCommand.Execute");

			if(Application.platform == RuntimePlatform.Android)
			{
				if (Debug.isDebugBuild) Debug.Log("BillingReadyCommand - Android platform detected, automatically restoring purchases.");
				billingService.RestorePurchases();
			}
		}
	}
}

