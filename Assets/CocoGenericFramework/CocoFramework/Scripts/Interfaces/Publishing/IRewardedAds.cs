using UnityEngine;
using System.Collections;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TabTale.Publishing 
{
	public interface IRewardedAds
	{
		IPromise<bool> ShowAd();
		bool IsAdReady();
		bool IsAdPlaying();
	}
}
