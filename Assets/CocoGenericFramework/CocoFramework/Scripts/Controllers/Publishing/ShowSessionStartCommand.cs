using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using TabTale.Publishing;

namespace TabTale
{
	public class ShowSessionStartCommand : GameCommand
	{
		[Inject]
		public LocationLoadedSignal locationLoadedSignal { get; set; }

		[Inject]
		public SettingsStateModel settingsStateModel { get; set; }

		[Inject]
		public ILocationManager locationManager { get; set; }

		[Inject]
		public IBannerAds bannerAds { get; set; }

		//		[Inject]
		//		public SoundManager soundManager { get; set; }
		[Inject]
		public GeneralParameterConfigModel generalParamtersConfigModel { get; set; }

		[Inject]
		public RequestBannersSignal requestBannersSignal { get; set; }

		private const string SessionStartTimeOut = "SessionStartTimeOut";

		private IPopupHandle popupHandle;

		public override void Execute ()
		{
			logger.Log ("ShowSessionStartCommand: Attempting to show start if available");

			// According to new guidelines - by default we won't show session start when no ads is bought
			bool disableSessionStartOnNoAds = generalParamtersConfigModel.GetBool ("DisableSessionStartOnNoAds", true);
			bool noAdsBought = !settingsStateModel.ShowingAds ();

			bool skipShowingSessionStart = disableSessionStartOnNoAds && noAdsBought;
			if (skipShowingSessionStart) {
				logger.Log ("ShowSessionStartCommand: No ads bought - skip showing session start");
				return;
			} else {
				logger.Log (Tag, "No ads bought - skip showing session start");
			}

			bool showSessionStartOnFirstSession = generalParamtersConfigModel.GetBool ("ShowSessionStartOnFirstSession", true);

			logger.Log (Tag, "ShowSessionStartOnFirstSession :" + showSessionStartOnFirstSession);

			int launchCount = TTPlayerPrefs.GetValue ("TimesLaunched", 1);
			logger.Log (Tag, "App Launch count : " + launchCount);

			if (!showSessionStartOnFirstSession && launchCount <= 1) {
				logger.Log ("ShowSessionStartCommand: skipping show in first game session");
				return;
			}

			if (Application.isEditor && !GsdkSettingsData.Instance.IsShowingSessionStartInEditor) {
				logger.Log ("ShowSessionStartCommand: skipping show in Unity Editor");
				return;
			}

			Retain ();

			popupHandle = locationManager.Get (ApplicationLocation.AppLoaded);

			popupHandle.Closed.AddListener (OnLocationClosed);
			float sessionStartTimeOut = generalParamtersConfigModel.GetFloat (SessionStartTimeOut, 10.0f);
			popupHandle.Open (OnLocationResult, sessionStartTimeOut);
		}

		void OnLocationResult (LocationResult result)
		{
			Debug.Log ("ShowSessionStartCommand - result: " + result.success);

			// if(! result.success)
			// {
			// 	ShowBanners();
			// }
			// else
			// {
			// 	//soundManager.MuteAllLayers();
			// 	popupHandle.Closed.AddListener(OnLocationClosed);
			// }
		}

		void OnLocationClosed (LocationResult result)
		{
			popupHandle.Closed.RemoveListener (OnLocationClosed);
			Debug.Log ("ShowSessionStartCommand - SessionStart Location closed");
			//soundManager.UnMuteAllLayers();
			ShowBanners ();
		}

		void ShowBanners ()
		{
			Debug.Log ("ShowSessionStartCommand - after attempt to show session start, requesting banners to show");
			requestBannersSignal.Dispatch ();

			Release ();
		}
	}
}