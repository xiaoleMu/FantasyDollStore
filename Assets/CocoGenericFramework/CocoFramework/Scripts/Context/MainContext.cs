using UnityEngine;
using System.Collections;
using TabTale.SceneManager;
using TabTale.AssetManagement;
using TabTale.Publishing;
using TabTale.Analytics;
using strange.extensions.injector.api;

namespace TabTale
{
	public class MainContext : BaseContext
	{
		IGameApplication _container;

		public MainContext (MonoBehaviour view) : base(view)
		{
			_container = GameApplication.Instance;

			StartStrange ();
		}

		public void StartStrange ()
		{
			BindLifeCycle ();

			BindSceneManager ();

			BindFramework ();

			BindMatch ();

			BindServices ();

			BindPublishingInterface ();

			BindSettings ();

			BindStore ();

			BindModels ();

			BindRestrictions ();

			BindSounds ();

			BindAnalytics();

			BindExamples ();

			BindEnergySystem ();

			BindTickerService ();

			BindEventSystem();

			BindTeams();

            injectionBinder.Bind<InnerPopUpHideHudElementsSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<ModalOpenedSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<ModalClosedSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<NetworkConnectionChangedSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<NetworkCheck> ().ToSingleton ().CrossContext ();


			injectionBinder.Bind<VibrationSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<ObjectPool> ().ToValue (GameApplication.Instance.GetComponent<ObjectPool>()).CrossContext();

			injectionBinder.Bind<IPopupHandle>().To<PopupHandle>();

#if UNITY_2017_1_OR_NEWER
			injectionBinder.Bind<ILogger>().ToValue(Debug.unityLogger).CrossContext();
#else
			injectionBinder.Bind<ILogger>().ToValue(Debug.logger).CrossContext();
#endif

			injectionBinder.Bind<TestSuite>().To<TestSuite>().CrossContext();
			injectionBinder.Bind<TestCase>().To<TestCase>().CrossContext();

			injectionBinder.Bind<CollectGameElementsSignal>().ToSingleton().CrossContext();
			commandBinder.Bind<CollectGameElementsSignal>().To<CollectGameElementsCommand>();


		}

		void BindAnalytics ()
		{
			injectionBinder.Bind<AnalyticsService>().ToSingleton().CrossContext();
			injectionBinder.Bind<IGsdkAnalyticsProvider>().To<GsdkAnalyticsProvider>().ToSingleton().CrossContext();
			commandBinder.Bind<CurrencyIncreasingSignal>().To<CurrencyIncreasingCommand>();
			commandBinder.Bind<CurrencyDecreasingSignal>().To<CurrencyDecreasingCommand>();
		}

		void BindServices ()
		{
			BindSocialService ();

			BindRewardService ();

			BindMysteryBox();

			// FIXME: Currently realtime multiplayer match manager is a service binded with Strange,
			// this should probably be changed to be a Module loaded through game application:
			BindRealtimeServices ();

			BindBillingService ();

			BindSocialNetworkService ();

			injectionBinder.Bind<IRoutineRunner> ().To<RoutineRunner>().ToSingleton().CrossContext ();

			injectionBinder.Bind<RateUsStateModel> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<NotificationClickedSignal>().ToSingleton().CrossContext();
			injectionBinder.Bind<NotificationsReceivedSignal>().ToSingleton().CrossContext();

			injectionBinder.Bind<RemoteImageService>().ToSingleton().CrossContext();

			injectionBinder.Bind<RegionalService>().ToSingleton().CrossContext();

			injectionBinder.Bind<PlayerInfoService>().ToSingleton().CrossContext();

			injectionBinder.Bind<ConfigurationService>().ToSingleton().CrossContext();

			injectionBinder.Bind<IAndroidPermissionWrapper>().To<PSdkAndroidPermissionsWrapper>().ToSingleton().CrossContext();
		}

		void BindMatch ()
		{
			injectionBinder.Bind<MatchEndSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<MatchRestoreSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<MatchStartSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<MatchStateModel>().ToSingleton().CrossContext();

		}

		void BindLifeCycle ()
		{
			commandBinder.Bind<StartSignal> ().To<LoadBindingsCommand>().To<SetupPsdkCommand>().To<InitCommand>().To<StartCommand> ().Once ().InSequence();

			injectionBinder.Bind<InitDoneSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<InitDoneSignal> ().To<InitDoneCommand> ();

			injectionBinder.Bind<OnApplicationPauseSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<OnApplicationPauseSignal> ().To<OnApplicationPauseCommand> ();

			injectionBinder.Bind<OnDestroySignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<StartSceneSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<StartSceneSignal> ().To<StartSceneCommand> ();
		}

		void BindSceneManager()
		{
			injectionBinder.Bind<SceneManagerEvents> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<SceneLoadedSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<FirstSceneLoadedSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<FirstSceneLoadedSignal> ().To<FirstSceneLoadedCommand> ().To<ShowSessionStartCommand>().Once();

			injectionBinder.Bind<GamePausedSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<GameResumedSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<GameQuitSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<FocusLostSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<FocusGainedSignal> ().ToSingleton ().CrossContext ();
		}

		void BindFramework ()
		{
			injectionBinder.Bind<ICoroutineFactory> ().ToValue (_container).CrossContext();//DOESNT WORK, EARLY ACCESS?
			injectionBinder.Bind<ITaskFactory> ().ToValue(_container.TaskFactory).CrossContext();
			injectionBinder.Bind<IModalityManager> ().ToValue (_container.ModalityManager).CrossContext ();
			injectionBinder.Bind<ISceneManager> ().ToValue (_container.SceneManager).CrossContext ();
			injectionBinder.Bind<IAssetManager> ().ToValue (_container.AssetManager).CrossContext ();
			injectionBinder.Bind<IGeneralDialogService> ().ToValue (_container.GeneralDialog).CrossContext ();

			injectionBinder.Bind<IInjector> ().ToValue (injectionBinder.injector).CrossContext();

			injectionBinder.Bind<ConnectionHandler>().To<ConnectionHandler>().CrossContext();
		}

		void BindRealtimeServices ()
		{
			/*
            injectionBinder.Bind<RealtimeMatchManager>().ToSingleton().CrossContext();

            injectionBinder.Bind<RealtimeMatchInitSignal>().ToSingleton().CrossContext();
            commandBinder.Bind<RealtimeMatchInitSignal>().To<RealtimeMatchInitCommand>();

            injectionBinder.Bind<RequestMultiplayerQuickMatchSignal>().ToSingleton().CrossContext();
            commandBinder.Bind<RequestMultiplayerQuickMatchSignal>().To<MultiplayerQuickMatchCommand>();

            injectionBinder.Bind<RequestMultiplayerInviteSignal>().ToSingleton().CrossContext();
            injectionBinder.Bind<RequestMultiplayerWaitingRoomSignal>().ToSingleton().CrossContext();

            injectionBinder.Bind<RequestMultiplayerMatchSignal>().ToSingleton().CrossContext();
            commandBinder.Bind<RequestMultiplayerMatchSignal>().To<MultiplayerMatchMakingCommand>();

            //injectionBinder.Bind<IRealtimeMatchManager> ().ToValue (_container.ModuleContainer.Get<RealtimeMatchManager>());
            */
		}

		void BindBillingService ()
		{
			injectionBinder.Bind<BillerInitDoneSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<PurchasedIAPSignal> ().ToSingleton ().CrossContext ();

			// if use receipt validation, the pruchase report will be done in psdk, otherwise we should report it in here.
//			#if ! NO_RECEIPT_VALIDATION && ! AMAZON
			commandBinder.Bind<PurchasedIAPSignal>().To<IAPPurchasedCommand>();
//			#else
//			commandBinder.Bind<PurchasedIAPSignal>().To<IAPPurchasedCommand>().To<IAPPurchasedAnalyticsCommand>().InSequence();
//			#endif

			injectionBinder.Bind<RequestRestoreSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<RequestRestoreSignal> ().To<RequestRestoreCommand> ();

			injectionBinder.Bind<PurchasesRestoredSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<PurchasesRestoredSignal> ().To<PurchasesRestoredCommand> ();

			// receipt validation be done in psdk now
//			#if ! NO_RECEIPT_VALIDATION && ! AMAZON
//			injectionBinder.ReBind<IReceiptValidator> ().Bind<ReceiptValidator>().To<ReceiptValidator>().ToSingleton().CrossContext();
//			#endif
		}

		void BindSocialService ()
		{
			injectionBinder.Bind<SocialServiceAuthenticateSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<SocialServiceAuthenticateSignal> ().To<SocialServiceAuthenticateCommand> ();
			injectionBinder.Bind<SocialServiceEvents> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<SocialSyncStartedSignal> ().ToSingleton ().CrossContext();
			injectionBinder.Bind<SocialSyncCompletedSignal> ().ToSingleton ().CrossContext();


			injectionBinder.Bind<RequestShowAchievementsSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<RequestShowAchievementsSignal> ().To<ShowAchievementsCommand> ();

			injectionBinder.Bind<RequestShowLeaderboardSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<RequestShowLeaderboardSignal> ().To<ShowLeaderboardButtonCommand> ();

			injectionBinder.Bind<ReportSocialProgressSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<RelationshipsScoresUpdateSignal>().ToSingleton().CrossContext();

			injectionBinder.Bind<RelationshipsUpdateSignal>().ToSingleton().CrossContext();
			commandBinder.Bind<RelationshipsUpdateSignal> ().To<HandleRelationshipsUpdateCommand> ();

			injectionBinder.Bind<RequestReceivedSignal>().ToSingleton().CrossContext();
			commandBinder.Bind<RequestReceivedSignal> ().To<OnRequestReceivedCommand> ();

			injectionBinder.Bind<RequestsManager>().ToSingleton().CrossContext();
		}

		void BindRewardService()
		{
			injectionBinder.Bind<RewardConfigModel>().ToSingleton().CrossContext();
			injectionBinder.Bind<RewardItemConfigModel>().ToSingleton().CrossContext();
			injectionBinder.Bind<RewardStateModel>().ToSingleton().CrossContext();
			injectionBinder.Bind<RewardService>().ToSingleton().CrossContext();

		}

		void BindMysteryBox()
		{
			injectionBinder.Bind<OpenMysteryBoxSignal>().ToSingleton().CrossContext();
			commandBinder.Bind<OpenMysteryBoxSignal> ().To<CollectMysteryBoxCommand> ();

			injectionBinder.Bind<MysteryBoxOpenedSignal>().ToSingleton().CrossContext();

			injectionBinder.Bind<DoubleUpMysteryBoxSignal>().ToSingleton().CrossContext();
			commandBinder.Bind<DoubleUpMysteryBoxSignal> ().To<DoubleUpMysteryBoxCommand> ();
		}

		void BindPublishingInterface()
		{
			injectionBinder.Bind<PsdkSessionStartSignal> ().ToSingleton ().CrossContext();
			commandBinder.Bind<PsdkSessionStartSignal> ().To<ShowSessionStartCommand> ();

			injectionBinder.Bind<PsdkRestartAppSignal> ().ToSingleton ().CrossContext();

			// App Shelf
			injectionBinder.Bind<RequestAppShelfSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<RequestAppShelfSignal> ().To<ShowAppShelfCommand> ();

			// Rewarded Ads
			injectionBinder.Bind<RewardedAdReadySignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<RewardedAdWillShowSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<RewardedAdClosedSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<RequestRewardedAdsSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<RequestRewardedAdsSignal> ().To<ShowRewardedAdsCommand> ();

			injectionBinder.Bind<RewardedAdResultSignal>().ToSingleton().CrossContext();
			commandBinder.Bind<RewardedAdResultSignal>().To<RewardedAdResultHandlerCommand>();

			// Banner Ads
			injectionBinder.Bind<BannerAdsShownSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<BannerAdsHiddenSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<BannerAdsClosedSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<BannerAdsWillDisplaySignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<RequestBannersSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<RequestBannersSignal> ().To<ShowBannersCommand> ();

            //pause game music signal

            injectionBinder.Bind<PauseGameMusicSignal>().ToSingleton().CrossContext();
            //commandBinder.Bind<PauseGameMusicSignal>().To<PauseGameMusicCommand>();


            // Interstitials
            injectionBinder.Bind<RequestInterstitialSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<RequestInterstitialSignal> ().To<ShowInterstitialCommand> ();

			injectionBinder.Bind<InterstitialShownSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<InterstitialDoneSignal> ().ToSingleton ().CrossContext ();

			// Location Manager

			injectionBinder.Bind<LocationLoadedSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<LocationShownSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<LocationClosedSignal> ().ToSingleton ().CrossContext ();

            //AppsFlyer App launcher
            injectionBinder.Bind<AppLauncherService>().ToSingleton().CrossContext();

			// Audience
			injectionBinder.Bind<IAudience>().To<PsdkAudienceProvider>();
        }

		void BindSettings ()
		{
			injectionBinder.Bind<RequestOpenSettingsSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<RequestOpenSettingsSignal> ().To<OpenSettingCommand> ();
		}

		void BindStore ()
		{
			injectionBinder.Bind<RequestPurchaseInAppItemSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<RequestPurchaseInAppItemSignal> ().To<PurchaseInAppItemCommand> ();

			injectionBinder.Bind<RewardItemBoughtSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<IAPPurchaseDoneSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<IStoreManager> ().Bind<StoreManager>().To<StoreManager>().ToSingleton().CrossContext();
		}

		void BindSocialNetworkService ()
		{
			injectionBinder.Bind<SocialNetworkInitSignal> ().ToSingleton ().CrossContext();
			injectionBinder.Bind<SocialNetworkLoginCompletedSignal> ().ToSingleton ().CrossContext();
			injectionBinder.Bind<SocialNetworkPhotoSignal> ().ToSingleton ().CrossContext();
			injectionBinder.Bind<SocialNetworkAppRequestSignal> ().ToSingleton ().CrossContext();
			injectionBinder.Bind<SocialNetworkUserNameReadySignal> ().ToSingleton ().CrossContext();
			injectionBinder.Bind<SocialNetworkServerVerificationCompleteSignal> ().ToSingleton ().CrossContext();
			injectionBinder.Bind<SocialImageReadySignal> ().ToSingleton ().CrossContext();
			injectionBinder.Bind<SocialProgressUpdatedSignal> ().ToSingleton ().CrossContext();


			commandBinder.Bind<SocialNetworkInitSignal> ().To<OnSocialNetworkInitCommand> ();
			commandBinder.Bind<SocialNetworkLoginCompletedSignal> ().To<SocialNetworkAuthenticateCommand> ();
			commandBinder.Bind<SocialNetworkAppRequestSignal> ().To<SocialNetworkAppRequestResultCommand> ();
			commandBinder.Bind<SocialNetworkServerVerificationCompleteSignal> ().To<HandleSocialServerVerificationCompleteCommand> ();
			commandBinder.Bind<SocialNetworkUserEmailReadySignal>().To<SocialNetworkUserEmailReadyCommand>();

		}

		void BindRestrictions()
		{
			injectionBinder.Bind<RestrictionVerifier>().ToSingleton().CrossContext();
		}

		void BindSounds ()
		{
			injectionBinder.Bind<SoundManager> ().ToSingleton ().CrossContext ();

			/*
			injectionBinder.Bind<SfxChannel> ().ToSingleton();

			GlobalSounds globalSounds = _container.AssetManager.GetResource<GlobalSounds>("Sounds/GlobalSounds");
			injectionBinder.Bind<GlobalSounds>().ToValue(globalSounds);

			BgMusicChannel bgMusicChannel = ComponentUtils.CreateComponentObject<BgMusicChannel>();
			injectionBinder.Bind<BgMusicChannel>().ToValue(bgMusicChannel);
			*/
		}


		void BindModels ()
		{
			BindModelEvents ();
			BindModelSync ();
			BindConfigsModels ();
			BindStateModels ();
		}

		private void BindStateModels ()
		{
			injectionBinder.Bind<GameStateModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<CurrencyStateModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<SettingsStateModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<InventoryStateModel>().ToSingleton().CrossContext();
			injectionBinder.Bind<SocialStateModel>().ToSingleton().CrossContext();
			injectionBinder.Bind<ProgressStateModel>().ToSingleton().CrossContext();
			injectionBinder.Bind<LevelStateModel>().ToSingleton().CrossContext();
			injectionBinder.Bind<RelationshipStateModel>().ToSingleton().CrossContext();
			injectionBinder.Bind<RelationshipScoreStateModel>().ToSingleton().CrossContext();
			injectionBinder.Bind<RequestStateModel>().ToSingleton().CrossContext();
			injectionBinder.Bind<EventStateModel>().ToSingleton().CrossContext();
		}

		void BindModelEvents ()
		{
			injectionBinder.Bind<ModelSyncStartSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<ConnectionDisconnectedSignal>().ToSingleton().CrossContext ();
			injectionBinder.Bind<ModelSyncCompletedSignal>().ToSingleton().CrossContext ();
			injectionBinder.Bind<UpdateItemsSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<SyncStatesSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<SyncConfigsSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<SyncSharedStateSignal>().ToSingleton().CrossContext ();;
			injectionBinder.Bind<ConnectionAllowUserToSwitchUserSignal> ().ToSingleton ().CrossContext();
			injectionBinder.Bind<ConnectionAllowUserToStartNewGameSignal> ().ToSingleton ();
			injectionBinder.Bind<UpdateCurrencySignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<InsufficientCurrencySignal>().ToSingleton().CrossContext();
			injectionBinder.Bind<CurrencyIncreasingSignal>().ToSingleton().CrossContext();
			injectionBinder.Bind<CurrencyDecreasingSignal>().ToSingleton().CrossContext();
			injectionBinder.Bind<RankUpSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<TopScoreSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<RestrictionsNotMetSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<FirstWinInLevelSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<MatchScoreUpdatedSignal> ().ToSingleton ().CrossContext ();
		}

		void BindConfigsModels ()
		{
			injectionBinder.Bind<GeneralParameterConfigModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<GSDKConfigModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<ItemConfigModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<AchievementsConfigModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<LeaderboardConfigModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<IAPConfigModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<CategoryConfigModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<ItemGroupConfigModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<TextConfigModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<RankConfigModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<CollectibleConfigModel> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<EventConfigModel> ().ToSingleton ().CrossContext ();

		}

		void BindModelSync ()
		{
			GameDbInitializer gameDbInitializer = _container.ModuleContainer.Get<GameDbInitializer> ();
			injectionBinder.Bind<IGameDB> ().ToValue (gameDbInitializer.GameDB).CrossContext ();
			injectionBinder.Bind<ServerTime> ().ToSingleton().CrossContext();
			injectionBinder.Bind<ISyncService>().Bind<ModelSyncService>().To<ModelSyncService>().ToSingleton().CrossContext();
			injectionBinder.Bind<ISocialSync> ().Bind<SocialSync>().To<SocialSync>().ToSingleton().CrossContext();

			injectionBinder.Bind<ServerResponseSignal>().ToSingleton().CrossContext();
		}

		void BindExamples ()
		{
			injectionBinder.Bind<ExampleRequestOpenShopSignal> ().To<ExampleShopButtonCommand> ();
		}

		void BindEnergySystem ()
		{
			injectionBinder.Bind<EnergyChargeCompleteSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<EnergyConsumedCompleteSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<EnergySystemService> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<EnergyStateModel> ().ToSingleton(). CrossContext();
		}

		void BindEventSystem ()
		{
			injectionBinder.Bind<EventEndedSignal> ().ToSingleton(). CrossContext();
                        injectionBinder.Bind<EventConcludedSignal> ().ToSingleton(). CrossContext();

			injectionBinder.Bind<EventSystemService> ().ToSingleton(). CrossContext();

		}

		void BindTickerService ()
		{
			injectionBinder.Bind<TimerLoopCompleteSignal> ().ToSingleton ().CrossContext ();
			//commandBinder.Bind<TimerLoopCompleteSignal> ().To<OnTimerLoopCompleteCommand> ();
			injectionBinder.Bind<TickerService> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<SoundManagerReadySignal> ().ToSingleton ().CrossContext ();
		}

		void BindTeams ()
		{
			injectionBinder.Bind<TeamScoreUpdatedSignal> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<TeamStateUpdatedSignal> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<ITeamSyncService> ().To<TeamSyncService> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<TeamSyncConnectionHandler> ().To<TeamSyncConnectionHandler> ().CrossContext ();

			injectionBinder.Bind<TeamStateModel> ().ToSingleton ().CrossContext ();

			injectionBinder.Bind<RankChangedSignal> ().ToSingleton ().CrossContext ();


		}


	}
}
