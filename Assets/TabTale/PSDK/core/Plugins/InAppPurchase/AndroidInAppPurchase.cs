#if UNITY_ANDROID
using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {
	public class AndroidInAppPurchase : IPsdkInAppPurchase {

		AndroidPsdkServiceMgr _sm;
		AndroidJavaObject _inAppPurchaseObject;

		public AndroidInAppPurchase(IPsdkServiceManager sm){
			_sm = sm as AndroidPsdkServiceMgr;
		}

		public bool  Setup() {
			if (null == _sm) {
				Debug.LogError("AndroidPsdkServiceManager::Setup InAppPurchase NULL PSDK Service Manager ");
				return false;
			}

			AndroidJavaObject javaInAppDelegate = new AndroidJavaObject("com.tabtale.publishingsdk.unity.UnityInAppDelegate");
			if (javaInAppDelegate == null) {
				Debug.LogError("com.tabtale.publishingsdk.unity.UnityInAppDelegate NULL");
				return false;
			}
			_sm.JavaClass.CallStatic("setInAppDelegate", javaInAppDelegate);

			return true;
		}

		public AndroidJavaObject GetUnityJavaObject()
		{
			try {
				if (null == _inAppPurchaseObject)
					_inAppPurchaseObject = _sm.GetUnityJavaObject().Call<AndroidJavaObject>("getInAppPurchase");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}

			return _inAppPurchaseObject;
		}

		public void psdkStartedEvent() {
			if (_inAppPurchaseObject == null)
				_inAppPurchaseObject = GetUnityJavaObject();

		}

		public IPsdkInAppPurchase GetImplementation()
		{
			return this;
		}

		public void PurchaseCampaignCompleteTransaction(bool result)
		{
			if(GetUnityJavaObject() != null){
				GetUnityJavaObject().Call("purchaseCampaignCompleteTransaction",new object[] {result});
			}
		}

		public void AddProduct(string id, string price, bool isPurchased)
		{
			if(GetUnityJavaObject() != null)
				GetUnityJavaObject().Call("addProduct", new object[] {id,price,isPurchased});
		}

		public void ItemPurchased(string id)
		{
			if(GetUnityJavaObject() != null)
				GetUnityJavaObject().Call("itemPurchased", new object[] {id});
		}
	}
}
#endif