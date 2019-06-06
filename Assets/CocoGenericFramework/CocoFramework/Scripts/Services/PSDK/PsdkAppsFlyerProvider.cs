using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;


namespace TabTale.Publishing
{
	public class PSdkAppsFlyerProvider : IAppsFlyer
	{
		private IPsdkAppsFlyer _psdkAppsFlyer;

		[PostConstruct]
		public void Init()
		{
			_psdkAppsFlyer = PSDKMgr.Instance.GetAppsFlyerService();
		}

		public void ReportPurchase(string price, string currency)
		{
			_psdkAppsFlyer.ReportPurchase(price, currency);
		}

	}
}
