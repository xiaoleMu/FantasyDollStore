using UnityEngine;
using System.Collections;
using TabTale.Publishing;

namespace TabTale
{
	public class GsdkDemo : GameView
	{
		[Inject]
		public IBannerAds bannersAds { get; set; }

		[Inject]
		public RequestInterstitialSignal requestInterstitialSignal { get; set; }

		[Inject]
		public IStoreManager storeManager { get; set; }

		public void ShowBanners()
		{
			bannersAds.Show();
		}

		public void HideBanners()
		{
			bannersAds.Hide();
		}

		public void ShowInterstitial()
		{
			requestInterstitialSignal.Dispatch(null);
		}

		public void PurchaseRemoveAds()
		{
			storeManager.BuyItem("NoAds").Done(result => {
				logger.Log("PurchaseRemoveAds - Result:" + result);
			});
		}

	}
}