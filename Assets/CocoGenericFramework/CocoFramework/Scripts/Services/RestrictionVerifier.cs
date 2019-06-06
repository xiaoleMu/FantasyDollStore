using UnityEngine;
using System.Collections.Generic;
using strange.extensions.signal.impl;

namespace TabTale
{
	public class RestrictionVerifier
	{
		[Inject]
		public CurrencyStateModel currencyStateModel { get; set; }

		[Inject]
		public InventoryStateModel inventoryStateModel { get; set; }

		[Inject]
		public ProgressStateModel progressStateModel { get; set; }

		[Inject]
		public ItemConfigModel itemConfigModel { get; set; }

		[Inject]
		public RestrictionsNotMetSignal restrictionsNotMetSignal { get; set; }

		public bool VerifyRestrictions(IEnumerable<GameElementData> restrictions)
		{
			List<GameElementData> failedRestrictions = new List<GameElementData>();

			foreach(GameElementData restriction in restrictions)
			{
				switch(restriction.type)
				{
				case GameElementType.Currency:
					if (! currencyStateModel.VerifyRestriction(restriction) )
					{
						failedRestrictions.Add(restriction);
					}
					break;
				case GameElementType.Item:
					if (! inventoryStateModel.VerifyRestriction(restriction) )
					{
						failedRestrictions.Add(restriction);
					}
					break;
				case GameElementType.State:
					if (! progressStateModel.VerifyRestriction(restriction) )
					{
						failedRestrictions.Add(restriction);
					}
					break;

				}
			}

			if(failedRestrictions.Count > 0)
			{
				restrictionsNotMetSignal.Dispatch(failedRestrictions);
				return false;
			}
			else
			{
				return true;
			}
		}

	}

	public class RestrictionsNotMetSignal : Signal<List<GameElementData>> { }
}
