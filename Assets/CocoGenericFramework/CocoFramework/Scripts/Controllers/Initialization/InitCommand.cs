using System;
using UnityEngine;
using strange.extensions.command.impl;
using strange.framework.impl;
using strange.extensions.injector.api;

namespace TabTale 
{
	public class InitCommand : GameCommand
	{
		[Inject]
		public InitDoneSignal initDoneSignal { get; set; }

		[Inject]
		public IGameDB gameDB { get; set; }

		[Inject]
		public IBillingService billingService { get; set; }

		[Inject]
		public ModelSyncService modelSync { get; set; }

		[Inject]
		public SocialSync socialSync { get; set; }

		[Inject]
		public ISocialService socialService { get; set; }

		[Inject]
		public SocialServiceEvents socialServiceEvents {get;set;}

		[Inject]
		public SceneManagerEvents sceneManagerEvents {get;set;}

		[Inject]
		public ISocialNetworkService socialNetworkService { get; set; }

		[Inject]
		public INotificationService notificationService { get; set; }

		[Inject]
		public GeneralParameterConfigModel generalParamsConfigModel { get; set; }

		[Inject]
		public ConfigurationService configurationService { get; set; }

		public override void Execute()
		{
			logger.Log(Tag,"Execute");

			UpdateLaunchCount();

			InstantiateDebug();

			InitBackButtonService();

			InitServicesAsync();

			Retain ();
			modelSync.TryConnectToServer()
				.Then(socialNetworkService.Init)
				.Then(EndInitSequence);

		}

		private void InitServicesAsync()
		{
			notificationService.Initialize();

			bool shouldInitBilling = generalParamsConfigModel.GetBool("AutoInitBilling", true);
			if(shouldInitBilling)
			{
				billingService.InitBilling();
			}
		}

		private void OnPsdkReady()
		{
			Debug.Log ("PsdkCore: Ready");
		}

		private void EndInitSequence ()
		{
			Debug.Log ("InitCommand: GSDK Initialisation is done");

			initDoneSignal.Dispatch();
			Release ();
		}

		void InitBackButtonService()
		{
			//TODO: Move back button binding elsewhere (needs to be under a gameobject because of its update function)
			StrangeRoot strangeRoot = GameApplication.Instance.ModuleContainer.Get<StrangeRoot>();
			BackButtonService backButtonService = strangeRoot.GameRoot.gameObject.AddComponent<BackButtonService>();
			
			IInjector injector = strangeRoot.MainContext.injectionBinder.injector;
			injector.binder.Bind<IBackButtonService>().ToValue(backButtonService).CrossContext();
		}
		void InstantiateDebug ()
		{
			bool shouldShowDebug = false;

			if(configurationService.GetServerUrl() == configurationService.GetConfigParam("serverDev"))
				shouldShowDebug = true;
			if(Debug.isDebugBuild && GsdkSettingsData.Instance.IsDebugEnabled)
				shouldShowDebug = true;

			if(shouldShowDebug)
			{
				GameObject prefab = Resources.Load ("Debug/DebugCanvas") as GameObject;
				GameObject go = UnityEngine.Object.Instantiate (prefab) as GameObject;
				UnityEngine.Object.DontDestroyOnLoad (go);
			}
		}

		void UpdateLaunchCount ()
		{
			// Check For 'TimesLaunched', Set To 0 If Value Isnt Set (First Time Being Launched)
			int launchCount = PlayerPrefs.GetInt ("TimesLaunched", 0);

			// After Grabbing 'TimesLaunched' we increment the value by 1
			launchCount = launchCount + 1;

			// Set 'TimesLaunched' To The Incremented Value
			PlayerPrefs.SetInt("TimesLaunched", launchCount);
		}
	}
}

