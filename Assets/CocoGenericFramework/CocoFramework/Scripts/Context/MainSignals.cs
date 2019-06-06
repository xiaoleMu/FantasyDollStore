using strange.extensions.signal.impl;
using TabTale.Publishing;
using System.Collections.Generic;

namespace TabTale
{
	// Publishing:
	public class RequestAppShelfSignal 			: Signal { }
	public class RequestBannersSignal 			: Signal { }
	public class RequestInterstitialSignal 		: Signal<ApplicationLocation> { }
	public class RequestRewardedAdsSignal		: Signal<GameElementData> { }
	public class PauseGameMusicSignal			: Signal<bool> { }
	public class PsdkSessionStartSignal			: Signal { }
	public class PsdkRestartAppSignal			: Signal { }

	public class RewardedAdResultSignal 		: Signal<bool, GameElementData> { }
	public class RewardedAdReadySignal 			: Signal<bool> { }
	public class RewardedAdWillShowSignal		: Signal { }
	public class RewardedAdClosedSignal			: Signal<bool> { }
	public class BannerAdsShownSignal			: Signal { }
	public class BannerAdsHiddenSignal			: Signal { }
	public class BannerAdsWillDisplaySignal		: Signal { }
	public class BannerAdsClosedSignal 			: Signal { }

	public class LocationLoadedSignal			: Signal<LocationResult> { }
	public class LocationShownSignal			: Signal<LocationResult> { }
	public class LocationClosedSignal			: Signal<LocationResult> { }

	public class InterstitialShownSignal		: Signal<LocationResult> { }
	public class InterstitialDoneSignal			: Signal<LocationResult> { }

	// InApps / Store:
    public class RequestPurchaseInAppItemSignal : Signal<IAPConfigData> { }
	public class RequestRestoreSignal 			: Signal { }
	public class RewardItemBoughtSignal			: Signal<ItemConfigData> { }
	public class IAPPurchaseDoneSignal	: Signal<PurchaseIAPResult> {}

	// Models:
	public class RequestMatchStartSignal 		: Signal { }
	public class MatchRestoreSignal 			: Signal<MatchStateData> { }
	public class MatchStartSignal 				: Signal<MatchStateData> { }
	public class FirstWinInLevelSignal		 	: Signal<int> { }
	public class MatchScoreUpdatedSignal		: Signal<int> { }
	public class MatchEndSignal 				: Signal { }
	public class UpdateItemsSignal 				: Signal { }
	public class EnergyChargeCompleteSignal 	: Signal { }
	public class EnergyConsumedCompleteSignal 	: Signal { }
	public class TimerLoopCompleteSignal		: Signal<string> { }
    //Cinema
    public class DownloadFinishedSignal         : Signal<string> { }
    public class DownloadAbortedSignal          : Signal<string> { }
    public class DownloadStartedSignal          : Signal<string> { }
    public class ParentalGateClosedSignal       : Signal { }

    // Settings
    public class VibrationSignal				: Signal { }

	// Social
	public class ReportSocialProgressSignal 	: Signal { }
	public class SocialImageReadySignal			: Signal<string> { }

	// Lifecycle
	public class InitDoneSignal					: Signal { }

	// Network
	public class NetworkConnectionChangedSignal	: Signal<bool> { }

	public class RelationshipsUpdateSignal		: Signal { }

	public class RequestReceivedSignal          : Signal<List<RequestStateData>> { }

	public class ServerResponseSignal			: Signal<string> { }

	public class SocialProgressUpdatedSignal	: Signal { }

	public class CollectGameElementsSignal		: Signal <List<GameElementData>> { }

	// Mystery Box
	public class OpenMysteryBoxSignal			: Signal <ItemConfigData> { }
	public class MysteryBoxOpenedSignal			: Signal <MysteryBoxData> { }
	public class DoubleUpMysteryBoxSignal		: Signal <MysteryBoxData> { }

	// Modality
	public class ModalOpenedSignal 				: Signal<string> { }

	public class ModalClosedSignal 				: Signal<string> { }
	public class SoundManagerReadySignal		: Signal{ }

	// Teams
	public class TeamStateUpdatedSignal : Signal { }
	public class TeamScoreUpdatedSignal : Signal<int> { }
	public class RankChangedSignal : Signal<RankChange> { }

	// Events
	public class EventEndedSignal : Signal<string> { }
	public class EventConcludedSignal : Signal<string> { }
}
