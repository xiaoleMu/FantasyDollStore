using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TabTale.Plugins.PSDK;
using TabTale;

public class BillingDemo : MonoBehaviour {

	//IMPORTANT: The PSDK Billing Service uses the Unity Billing Service. Before using, you will need to enable it in the Services Window (Window->Services->In-App Purchasing). Please only enable the service, do not click "Import".


	public void onClick(int mNum)
	{
		string iapId = GetComponentInChildren<InputField>().text;
		switch (mNum) {
		case 0:
			PSDKMgr.Instance.GetBilling().InitBilling();
			break;
		case 1:
			Debug.Log("IsInitialised - " + PSDKMgr.Instance.GetBilling().IsInitialised()); 
			break;
		case 2:
			Debug.Log("IsPurchaseInProgress - " + PSDKMgr.Instance.GetBilling().IsPurchaseInProgress()); 
			break;
		case 3:
			Debug.Log("IsRestoreInProgress - " + PSDKMgr.Instance.GetBilling().IsRestoreInProgress()); 
			break;
		case 4:
			PSDKMgr.Instance.GetBilling().RestorePurchases();
			break;
		case 5:
			PSDKMgr.Instance.GetBilling().PurchaseItem(iapId);
			break;
		case 6:
			Debug.Log("IsPurchased - " + PSDKMgr.Instance.GetBilling().IsPurchased(iapId)); 
			break;
		case 7:
			Debug.Log("IsConsumable - " + PSDKMgr.Instance.GetBilling().IsConsumable(iapId)); 
			break;
		case 8:
			Debug.Log("GetLocalizedPriceString - " + PSDKMgr.Instance.GetBilling().GetLocalizedPriceString(iapId)); 
			break;
		case 9:
			Debug.Log("GetISOCurrencySymbol - " + PSDKMgr.Instance.GetBilling().GetISOCurrencySymbol(iapId)); 
			break;
		case 10:
			Debug.Log("GetPriceInLocalCurrency - " + PSDKMgr.Instance.GetBilling().GetPriceInLocalCurrency(iapId).ToString()); 
			break;
		case 11:
			PSDKMgr.Instance.GetBilling().ClearTransactions();
			break;
		case 12:
			Debug.Log("IsNoAdsItem - " + PSDKMgr.Instance.GetBilling().IsNoAdsItem(iapId).ToString()); 
			break;
		default:
			break;
		}
	}



	private void onBillingPurchased(PurchaseIAPResult result)
	{
		Debug.Log("PurchaseResult - " + result.purchasedItem.id + " - " + result.result.ToString());
	}
	

}
