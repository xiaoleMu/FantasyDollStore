using UnityEngine;
using System;
using System.Collections;
using strange.extensions.signal.impl;
using TabTale.Publishing;

namespace TabTale
{
	public class PopupHandle : IPopupHandle
	{
		public Signal<LocationResult> _popupResult = new Signal<LocationResult> ();

		public Signal<LocationResult> Result {
			get { return _popupResult; }
		}

		public Signal<LocationResult> _popupClosed = new Signal<LocationResult> ();

		public Signal<LocationResult> Closed {
			get { return _popupClosed; }
		}

		[Inject]
		public ILocationManager locationManager { get; set; }

		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		private ApplicationLocation _location;

		public ApplicationLocation Location {
			get { return _location; }
			set { _location = value; }
		}

		public PopupHandle ()
		{
		}

		public PopupHandle (ApplicationLocation location)
		{
			Location = location;
		}


		private bool locationLoaded;

		private bool locationClosed = true;

		public string Name {
			get { return "PSDKPopup"; }
		}

		public bool IsReady ()
		{
			return locationManager.IsReady (_location);
		}

		public void Open (Action<LocationResult> resultCallback, float timeout = -1)
		{
			if (resultCallback != null)
				Result.AddOnce (resultCallback);

			Open (timeout);
		}

		public void Open (float timeout = -1)
		{
			// ready, show it
			if (IsReady ()) {
				locationClosed = false;
				locationManager.locationClosedSignal.AddListener (OnLocationClosed);

				locationManager.Show (Location)
					.Then (OnLocationShown)
					.Fail (ex => {
						Debug.LogFormat ("PopupHandle for location: {0} - open failed because exception: {1}", _location, ex);
						PopupFailed ();
					});
				return;
			}

			var immediateRequest = timeout <= 0;
			Debug.LogFormat ("PopupHandle for location: {0} - not ready for immediate request: {1}", _location, immediateRequest);

			// force fail for immediate request
			if (immediateRequest) {
				Debug.LogFormat ("PopupHandle for location: {0} - open failed because location not ready", _location);
				PopupFailed ();
				return;
			}

			// wait load for no immediate request
			routineRunner.StartCoroutine (WaitForLocation (timeout));
		}

		private void OnLocationShown (LocationResult result)
		{
			if (_popupResult != null)
				_popupResult.Dispatch (result);

			// force close if not success
			if (!result.success) {
				OnLocationClosed (result);
			}
		}

		private void OnLocationLoaded (LocationResult result)
		{
			if (result.location != _location) {
				return;
			}

			Debug.LogFormat ("PopupHandle for location: {0} - location was loaded", _location);
			locationLoaded = true;
		}

		private void OnLocationClosed (LocationResult result)
		{
			if (result.location != _location) {
				return;
			}

			if (!locationClosed) {
				locationManager.locationClosedSignal.RemoveListener (OnLocationClosed);
				locationClosed = true;
			}

			if (_popupClosed != null) {
				_popupClosed.Dispatch (result);
			}
		}

		private IEnumerator WaitForLocation (float timeout)
		{
			locationManager.locationLoadedSignal.AddListener (OnLocationLoaded);
			locationLoaded = false;

			var startWaitTime = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup < startWaitTime + timeout) {
				if (locationLoaded) {
					break;
				}

				yield return null;
			}

			locationManager.locationLoadedSignal.RemoveListener (OnLocationLoaded);

			if (!locationLoaded) {
				PopupFailed ();
				yield break;
			}

			Debug.Log ("PopupHandle for location: " + _location + " - Attempting to show loaded location");
			Open ();
		}

		private void PopupFailed ()
		{
			var result = new LocationResult {
				location = Location,
				success = false
			};

			OnLocationShown (result);
		}
	}
}