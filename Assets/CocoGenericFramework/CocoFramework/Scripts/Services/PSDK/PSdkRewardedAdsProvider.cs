using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TabTale.Publishing
{
	public class PSdkRewardedAdsProvider : IRewardedAds
	{
		[Inject]
		public RewardedAdReadySignal rewardedAdReadySignal { get; set; }

		[Inject]
		public RewardedAdWillShowSignal rewardedAdWillShowSignal { get; set; }

		[Inject]
		public RewardedAdClosedSignal rewardedAdClosedSignal { get; set; }

		public IPsdkServiceManager psdkMgr;

		private IPromise<bool> adClosedPromise = new Promise<bool>();

#if UNITY_2017_1_OR_NEWER
		private ILogger logger = Debug.unityLogger;
#else
		private ILogger logger = Debug.logger;
#endif

		private const string Tag = "PSdkRewardedAdsProvider";

		[PostConstruct]
		public void Init()
		{
			AddListeners();
			psdkMgr = PSDKMgr.Instance;
		}
		
		private void AddListeners()
		{
			PsdkEventSystem.Instance.onRewardedAdIsReadyEvent 				+= ()					=> { rewardedAdReadySignal.Dispatch(true); };
			PsdkEventSystem.Instance.onRewardedAdIsNotReadyEvent 			+= ()					=> { rewardedAdReadySignal.Dispatch(false); };
			PsdkEventSystem.Instance.onRewardedAdWillShowEvent 				+= () 					=> { rewardedAdWillShowSignal.Dispatch(); };

			PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent 	+= (watchedAd) 			=> 
			{ 
				Debug.Log(Tag + ".onRewardedAdDidClosedWithResultEvent - Ad watched:" + watchedAd);

				rewardedAdClosedSignal.Dispatch(watchedAd);

				// if made a promise to any clients that the ad will close - fulfill the promise:
				adClosedPromise.Dispatch(watchedAd);
				adClosedPromise = new Promise<bool>();
			};
		}

		#region IRewardedAds implementation

		public bool IsAdPlaying ()
		{
			bool isAdPlaying = psdkMgr.GetRewardedAdsService().IsAdPlaying();

			Debug.Log(Tag + ".IsAdPlaying:" + isAdPlaying);

			return psdkMgr.GetRewardedAdsService().IsAdPlaying();
		}

		public IPromise<bool> ShowAd ()
		{
			Debug.Log(Tag + ".ShowAd");
			if(! IsAdPlaying())
			{
				psdkMgr.GetRewardedAdsService().ShowAd();
			}
			else
			{
				adClosedPromise.ReportFail(new System.Exception("Attempted to show a rewarded video while a video is playing"));

			}
			return adClosedPromise;
		}

		public bool IsAdReady ()
		{
			return psdkMgr.GetRewardedAdsService().IsAdReady();
		}

		#endregion
	}
}