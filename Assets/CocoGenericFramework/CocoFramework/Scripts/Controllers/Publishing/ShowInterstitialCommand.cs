using TabTale.Publishing;

namespace TabTale
{
	public class ShowInterstitialCommand : GameCommand
	{
		[Inject]
		public ApplicationLocation location { get; set; }

		[Inject]
		public SettingsStateModel settingsStateModel { get; set; }

		[Inject]
		public ILocationManager locationManager { get; set; }

		// [Inject]
		// public SoundManager soundManager { get; set; }

		[Inject]
		public InterstitialDoneSignal interstitialDoneSignal { get; set; }

		[Inject]
		public InterstitialShownSignal interstitialShownSignal { get; set; }

		private IPopupHandle popupHandle;

		public override void Execute ()
		{
			logger.Log (Tag, "Attempting to show Interstitial - showingAds: " + settingsStateModel.ShowingAds ());

			var targetLocation = location ?? ApplicationLocation.SceneTransitions;

			if (!settingsStateModel.ShowingAds ()) {
				logger.Log (Tag, "will not show ad since noAds was bought");
				var result = new LocationResult {
					location = targetLocation,
					success = false
				};
				interstitialDoneSignal.Dispatch (result);
			}

			logger.Log (Tag, "Attempting to show Interstitial");

			Retain ();
			popupHandle = locationManager.Get (targetLocation);
			popupHandle.Closed.AddListener (OnInterstitialCloseResult);
			popupHandle.Open (OnInterstitialShowResult);
		}

		private void OnInterstitialShowResult (LocationResult result)
		{
			logger.Log (Tag, "OnLocationResult:" + result.success);

			// if (result.success) {
			// 	//soundManager.PauseAllLayers();
			// 	popupHandle.Closed.AddListener (OnInterstitialCloseResult);
			// } else {
			// 	interstitialDoneSignal.Dispatch (result);
			// 	Release ();
			// }
			interstitialShownSignal.Dispatch (result);
		}

		private void OnInterstitialCloseResult (LocationResult result)
		{
			popupHandle.Closed.RemoveListener (OnInterstitialCloseResult);
			//soundManager.UnPauseAllLayers();
			interstitialDoneSignal.Dispatch (result);
			Release ();
		}
	}
}