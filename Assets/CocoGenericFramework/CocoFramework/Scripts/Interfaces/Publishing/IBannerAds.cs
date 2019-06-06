using System.Collections;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TabTale.Publishing 
{
	public interface IBannerAds
	{
		BannerAdsShownSignal 		bannerAdsShownSignal { get; }

		BannerAdsHiddenSignal 		bannerAdsHiddenSignal { get; }

		BannerAdsWillDisplaySignal 	bannerAdsWillDisplaySignal { get; }

		BannerAdsClosedSignal 		bannerAdsClosedSignal { get; }

		bool IsShowing { get; }

		IPromise Show();
		IPromise Hide();

		bool 	IsAlignedToTop();
		float 	GetAdHeight();
		float 	GetAdHeightInPercentage();
		bool 	IsActive();

	}
}

