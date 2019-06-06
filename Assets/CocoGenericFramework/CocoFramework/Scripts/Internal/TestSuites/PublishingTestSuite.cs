using System;
using TabTale.Publishing;

namespace TabTale
{
	public class PublishingTestSuite : TestSuite
	{
		[Inject]
		public IRewardedAds rewardedAds { get; set; }

		[Inject]
		public ILocationManager locationManager { get; set; }

		[Inject]
		public IBannerAds bannerAds { get; set; }

		[Inject]
		public RequestAppShelfSignal requestAppShelfSignal {get; set;}

		[Inject]
		public RequestInterstitialSignal requestInterstitialSignal { get; set; }

		[Inject]
		public InterstitialDoneSignal interstitialDoneSignal { get; set; }

		[PostConstruct]
		public void Init()
		{
			AddTest(TestShowRewardedAds, "Show Session Start", "", locationManager.IsReady(ApplicationLocation.AppLoaded), "Session Start location is not ready");

			AddTest(TestShowRewardedAds, "Show Rewarded Ads", "", rewardedAds.IsAdReady(), "Rewarded ads are not ready");

			AddTest(TestShowInterstitial, "Show Interstitial", "", locationManager.IsReady(ApplicationLocation.SceneTransitions), "Scene Transitions location is not ready");

			AddTest(TestShowBanners, "Show Banners", "", bannerAds.IsActive(), "Banner ads are not active");

			AddTest(TestHideBanners, "Hide Banners", "", bannerAds.IsActive(), "Banner ads are not active");

			AddTest(TestMoreApps, "MoreApps", "", locationManager.IsReady(ApplicationLocation.MoreApps), "More apps location is not ready");
		}


		private void TestShowRewardedAds (Action<TestCaseResult> resultCallback)
		{
			rewardedAds.ShowAd().Then( success => {
				if(success)
				{
					resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, "Successfully shown"));
				}
				else
				{
					resultCallback(TestCaseResult.Create(TestCaseResultCode.Failure, "Failed to show"));
				}
			});
		}

		private void TestShowInterstitial (Action<TestCaseResult> resultCallback)
		{
			interstitialDoneSignal.AddOnce(result => {
				if(result.success)
				{
					resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, "Successfully shown"));
				}
				else
				{
					resultCallback(TestCaseResult.Create(TestCaseResultCode.Failure, "Failed to show"));
				}
			});

			requestInterstitialSignal.Dispatch(null);
		}

		private void TestShowBanners (Action<TestCaseResult> resultCallback)
		{
			bannerAds.Show().Then( () =>
				resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, "Successfully shown")));
		}

		private void TestHideBanners (Action<TestCaseResult> resultCallback)
		{
			bannerAds.Hide().Then( () =>
				resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, "Successfully shown")));
		}

		private void TestMoreApps (Action<TestCaseResult> resultCallback)
		{
			bool isMoreAppsAvailable = locationManager.IsReady(ApplicationLocation.MoreApps);

			if(isMoreAppsAvailable)
				resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, "More Apps Available"));
			else
				resultCallback(TestCaseResult.Create(TestCaseResultCode.Failure, "More Apps Unavailable"));

			requestAppShelfSignal.Dispatch();
		}
	}
}

