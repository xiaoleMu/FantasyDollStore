using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using TabTale.Analytics;

namespace TabTale
{
	public class CurrencyDecreasingCommand : Command
	{
		[Inject]
		public GameElementData amount {get;set;}

		[Inject]
		public AnalyticsService analytics {get;set;}

		public override void Execute ()
		{
			//analytics.LostResources(amount.key,(float)amount.value ,"Currency",amount.key);
		}
	}
}
