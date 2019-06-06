using System;
using RSG;
using UnityEngine;

namespace TabTale
{
	public class NullReceiptValidator : IReceiptValidator
	{
		[Inject]
		public ILogger logger { get; set; }

		#region IReceiptValidator implementation

		public IPromise<bool> Validate (InAppPurchasableItem inAppItem)
		{
			var promise = new Promise<bool>();

			logger.Log("NullReceiptValidator.Validate - Always returning true");

			promise.Resolve(true);

			return promise;
		}

		#endregion


	}
}

