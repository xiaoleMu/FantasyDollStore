using System;
using RSG;

namespace TabTale
{
	public interface IReceiptValidator
	{
		IPromise<bool> Validate(InAppPurchasableItem inAppItem);
	}
}

