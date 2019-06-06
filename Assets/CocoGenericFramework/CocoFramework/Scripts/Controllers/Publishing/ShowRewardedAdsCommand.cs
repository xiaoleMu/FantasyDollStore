using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using TabTale.Publishing;

namespace TabTale
{

    public class ShowRewardedAdsCommand : GameCommand
    {
        [Inject]
		public RewardedAdResultSignal rewardedAdResultSignal { get; set; }

		[Inject]
		public GameElementData reward { get; set; }

		[Inject]
		public IRewardedAds rewardedAds { get; set; }

        public override void Execute ()
        {
			Debug.Log(Tag + " Attempting to show Rewarded Ads");

			if( ! rewardedAds.IsAdReady())
			{
				rewardedAdResultSignal.Dispatch(false, reward);
				return;
			}

			Retain ();
			rewardedAds.ShowAd()
				.Then(HandleAdResult)
				.Fail(ex => {
					Debug.Log(Tag + " failed to show ad:" + ex.ToString());
					rewardedAdResultSignal.Dispatch(false, reward);
					Release();
				});
		}

		private void HandleAdResult(bool adWatched)
		{
			Debug.Log(string.Format("{0}.HandleAdResult - Ad Watched:{1}, Reward:{2}", Tag, adWatched, reward.ToString()));
			rewardedAdResultSignal.Dispatch(adWatched, reward);
			Release();
		}

    }

}