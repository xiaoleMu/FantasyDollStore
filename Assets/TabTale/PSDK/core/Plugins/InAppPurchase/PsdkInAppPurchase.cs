using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {
	public class PsdkInAppPurchase : IPsdkInAppPurchase {

		IPsdkInAppPurchase _impl;

		public PsdkInAppPurchase(IPsdkServiceManager sm) {
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphoneInAppPurchase (); break;
				#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidInAppPurchase (sm.GetImplementation ()); break;
				#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:		_impl = new UnityInAppPurchase (); break;
			default:
				throw new System.Exception ("Platform not supported for External Configuration.");
			}
		}

		public bool Setup() {
			bool rc = _impl.Setup();
			return rc;
		}

		public IPsdkInAppPurchase GetImplementation()
		{
			return _impl;
		}

		public void PurchaseCampaignCompleteTransaction(bool result)
		{
			_impl.PurchaseCampaignCompleteTransaction(result);
		}

		public void psdkStartedEvent() {
			_impl.psdkStartedEvent();
		}

		public void AddProduct(string id, string price, bool isPurchased)
		{
			_impl.AddProduct(id,price,isPurchased);
		}

		public void ItemPurchased(string id)
		{
			_impl.ItemPurchased(id);
		}
	}
}