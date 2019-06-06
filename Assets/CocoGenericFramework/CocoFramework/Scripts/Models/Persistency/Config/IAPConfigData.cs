using System.Collections.Generic;

namespace TabTale
{

	public class IAPConfigData : IConfigData
	{
		public string 			id;
		public string 			name;
		public string 			description ;
		public List<IAPData> 	iapData = new List<IAPData>();
		public bool 			consumable;
		public bool 			noAdsIap;
        public bool             isSubscription;

        public string 	store; // Deprecated
		public string 	iapId; // Deprecated

		#region IConfigData implementation

		public string GetTableName ()
		{
			return "iap_config";
		}

		public string GetServerTableName ()
		{
			return "IAP";
		}

		public string GetId ()
		{
			return id;
		}

		public string ToLogString ()
		{
			string res = "IAPConfigData: Id:{0}, Name:{1}, Description:{2}, IapData:{3}, Consumable:{4}, NoAdsIAP:{5}";
			return string.Format(res,id,name,description,iapData.ToArray().ToString(),consumable,noAdsIap);
		}

		public bool IsBlob()
		{
			return false;
		}

		public IConfigData Clone ()
		{
			IAPConfigData c = new IAPConfigData();
			c.id = id;
			c.name = name;
			c.description = description;
			c.iapData = iapData.Clone() as List<IAPData>;
			c.consumable = consumable;
			c.noAdsIap = noAdsIap;
            c.isSubscription = isSubscription;

			return c;
		}

		#endregion

	}
}