using UnityEngine;
using System.Linq;

namespace TabTale
{
	public class IAPConfigModel : ConfigModel<IAPConfigData>
	{
		#region Getters

		public IAPConfigData GetIAP (string id)
		{
			var originalData = GetOriginalIAP (id);
			return originalData != null ? originalData.Clone () as IAPConfigData : null;
		}

		public string GetName (string id)
		{
			var originalData = GetOriginalIAP (id);
			return originalData != null ? originalData.name : string.Empty;
		}

		public string GetDescription (string id)
		{
			var originalData = GetOriginalIAP (id);
			return originalData != null ? originalData.description : string.Empty;
		}

        public bool IsSubscription(string id)
        {
	        var originalData = GetOriginalIAP (id);
	        return originalData != null && originalData.isSubscription;
        }

		private IAPConfigData GetOriginalIAP (string id)
		{
			return _configs.FirstOrDefault (iap => iap.GetId () == id);
		}

		public string GetIAPId (string id)
		{
			IAPConfigData iapConfigData = GetIAP(id);

			if(iapConfigData == null)
			{
				Debug.LogError("IAPConfigModel - Cannot find IAP with id: " + id);
				return "";
			}

			return GetIAPId(iapConfigData);
		}

		public IAPConfigData GetIAPByPurchaseId(string purchasedId)
		{
			foreach(var iapConfigData in _configs)
			{
				var iapData = GetStoreSpecificIapId(iapConfigData);

				if(iapData == null)
					continue;

				if(iapData.iapId == purchasedId)
				{
					return iapConfigData;
				}
			}

			return null;
		}

		public string GetIAPId (IAPConfigData iapConfigData)
		{
			var iapData = GetStoreSpecificIapId(iapConfigData);

			if(iapData == null)
			{
				Debug.LogError("IAPConfigModel - Cannot find current store iap data for iap id: " + iapConfigData.id);
				return "";
			}

			return iapData.iapId;
		}

		private IAPData GetStoreSpecificIapId(IAPConfigData iapConfigData)
		{
			// Try the current store first
			var iapData = iapConfigData.iapData.FirstOrDefault(iap => iap.store == StoreInfo.CurrentStoreType);

			// Cascade to the general store type (all stores)
			// This is the default use case unless there are differnent iap ids defined in the different stores
			if(iapData == null)
			{
				iapData = iapConfigData.iapData.FirstOrDefault(iap => iap.store == StoreType.All);
			}

			return iapData;
		}

		public bool GetConsumable (string id)
		{
			var originalData = GetOriginalIAP (id);
			return originalData != null && originalData.consumable;
		}

		public bool GetNoAdsIAP (string id)
		{
			var originalData = GetOriginalIAP (id);
			return originalData != null && originalData.noAdsIap;
		}

		#endregion
	}
}
