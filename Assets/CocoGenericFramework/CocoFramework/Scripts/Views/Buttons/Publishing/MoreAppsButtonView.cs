using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;
using TabTale.Publishing;
using System;

namespace TabTale {
	
	public class MoreAppsButtonView : MainView
	{
		// Setting to false will instead set it to non interactable when not ready
		public bool hideIfNotReady = true; 

		public float checkShowAvailabilityLoopLength = 2.0f;

		[Inject]
		public RequestAppShelfSignal requestAppShelfSignal {get; set;}

	    [Inject]
	    public SoundManager soundManager { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[Inject]
		public ILocationManager locationManager { get; set; }

		[Inject]
		public LocationClosedSignal locationClosedSignal { get; set; }

		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		private bool keepChecking = true;

		private CanvasGroup _canvasGroup;

		private int _maxRefreshAttempts = 3;

		/// <summary>
		/// The more apps button view contains the following refresh logic that determines its visiblity:
		/// 1. The moreapps location won't be available when a location is open, so when a location is closed (like interstitial) it will check if the moreapps location is ready
		/// 2. Since it takes time to download the app shelf it will attempt to refresh multiple times
		/// </summary>
		protected override void OnRegister ()
		{
			base.OnRegister ();

			RefreshButtonVisibility();

			if(checkShowAvailabilityLoopLength > 0)
			{
				StartCoroutine(RefreshButtonVisibilityCoro());
			}

			locationClosedSignal.AddListener(OnLocationClosed);

		}

		private void OnLocationClosed(LocationResult result)
		{
			Debug.Log (String.Format("MoreAppsButtonView.OnLocationClosed {0} - Refreshing more apps button visibility", result.location.ToString()));

			// Refresh the button visibility when any location that is not more apps is closed
			if(result.location != ApplicationLocation.MoreApps)
				RefreshButtonVisibility();
		}

		private IEnumerator RefreshButtonVisibilityCoro()
		{
			int refreshAttempts = 0;

			while(keepChecking)
			{
				yield return new WaitForSeconds(checkShowAvailabilityLoopLength);

				if (gameObject != null) // Safety check
					RefreshButtonVisibility();

				refreshAttempts++;
				if(refreshAttempts > _maxRefreshAttempts)
					keepChecking = false;
			}
		}
		private void RefreshButtonVisibility()
		{
			if(locationManager.IsReady(ApplicationLocation.MoreApps))
			{
				SetVisible(true);
				gameObject.GetComponent<Button>().interactable = true;
			}
			else
			{
				if(hideIfNotReady)
				{
					SetVisible(false);
					Debug.Log ("MoreAppsButtonView - Hiding more apps since it is not ready");
				}
				else
				{
					gameObject.GetComponent<Button>().interactable = false;
					Debug.Log ("MoreAppsButtonView - More apps set to non-interactable since it is not ready");
				}
			}
		}

		public void OnClick()
		{
        	soundManager.PlaySound(SoundMapping.Map.GeneralButtonClick, SoundLayer.Main);
			requestAppShelfSignal.Dispatch();
		}

		public void OnDestroy()
		{
			keepChecking = false;
			StopCoroutine(RefreshButtonVisibilityCoro());

			locationClosedSignal.RemoveListener(OnLocationClosed);

			base.OnDestroy();
		}

		private void SetVisible(bool visible)
		{
			CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
			if(canvasGroup == null)
				canvasGroup = gameObject.AddComponent<CanvasGroup>();

			canvasGroup.alpha = (visible ? 1 : 0);
			canvasGroup.blocksRaycasts = visible;
		}


        void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus && networkCheck.HasInternetConnection())
            {
                SetVisible(true);
                gameObject.GetComponent<Button>().interactable = true;
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus && networkCheck.HasInternetConnection())
            {
                SetVisible(true);
                gameObject.GetComponent<Button>().interactable = true;
            }
        }
    }
	
}