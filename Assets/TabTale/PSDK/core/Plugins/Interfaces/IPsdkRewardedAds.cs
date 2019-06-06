using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.Plugins.PSDK {
	public interface IPsdkRewardedAds  : IPsdkService {
		bool Setup();
		bool ShowAd();
		bool IsAdReady();
		bool IsAdPlaying();
		IPsdkRewardedAds GetImplementation();
	}
}
