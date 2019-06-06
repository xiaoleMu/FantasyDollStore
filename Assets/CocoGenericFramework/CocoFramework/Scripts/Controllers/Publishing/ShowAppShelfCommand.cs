using UnityEngine;
using strange.extensions.command.impl;
using TabTale.Publishing;

namespace TabTale
{
	public class ShowAppShelfCommand : Command
	{
		[Inject]
		public ILocationManager locationManager { get; set; }

		[Inject]
		public IBannerAds bannerAds { get; set; }

		// [Inject]
		// public SettingsStateModel settingsStateModel { get; set; }

		[Inject]
		public RequestBannersSignal requestBannersSignal { get; set; }

		private IPopupHandle popupHandle;

		private bool bannersWereShown;

		public override void Execute ()
		{
			CoreLogger.LogDebug ("ShowAppShelfCommand : Attempting to show app shelf");

			if (locationManager.IsReady (ApplicationLocation.MoreApps)) {
				Retain ();
				popupHandle = locationManager.Get (ApplicationLocation.MoreApps);
				popupHandle.Closed.AddListener (HandleCloseResult);
				popupHandle.Open (HandleShowResult);
			} else {
				Debug.LogWarning ("ShowAppShelfCommand - Will not show app shelf, location not ready");
			}
		}

		private void HandleShowResult (LocationResult result)
		{
			// if(result.success)
			// {
			// 	if(bannerAds.IsShowing)
			// 	{
			// 		bannerAds.Hide();
			// 		bannersWereShown = true;
			// 	}
			//
			// 	popupHandle.Closed.AddListener(HandleCloseResult);
			// }
			// else
			// {
			// 	Release();
			// }

			if (!result.success) {
				return;
			}

			if (bannerAds.IsShowing) {
				bannerAds.Hide ();
				bannersWereShown = true;
			}
		}

		private void HandleCloseResult (LocationResult result)
		{
			popupHandle.Closed.RemoveListener (HandleCloseResult);

			if (bannersWereShown) {
				requestBannersSignal.Dispatch ();
			}

			Release ();
		}
	}
}