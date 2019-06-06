using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using TabTale.Analytics;

namespace TabTale
{
	public class CurrencyIncreasingCommand : Command
	{
		[Inject]
		public GameElementData amount {get;set;}

		[Inject]
		public AnalyticsService analytics {get;set;}

		public override void Execute ()
		{
			//analytics.GainedResources(amount.key,(float)amount.value,"Currency",amount.key);
		}
	}
}
