using UnityEngine;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using strange.extensions.injector.api;

namespace TabTale.Publishing
{
	public class PSdkLocationManagerProvider : ILocationManager
	{
		[Inject]
		public ICrashTools crashTools { get; set; }

		[Inject]
		public PauseGameMusicSignal pauseGameMusicSignal { get; set; }

		[Inject]
		public LocationLoadedSignal locationLoadedSignal { get; set; }

		[Inject]
		public LocationShownSignal locationShownSignal { get; set; }

		[Inject]
		public LocationClosedSignal locationClosedSignal { get; set; }

		private readonly Dictionary<ApplicationLocation, IPromise<LocationResult>> locationResultPromises =
			new Dictionary<ApplicationLocation, IPromise<LocationResult>> ();

		private IPsdkServiceManager psdkMgr;

		[Inject]
		public IInjector injector { get; set; }

		[PostConstruct]
		public void Init ()
		{
			psdkMgr = PSDKMgr.Instance;

			AddListeners ();
		}

		private void AddListeners ()
		{
			PsdkEventSystem psdkEventSystem = PsdkEventSystem.Instance;
			psdkEventSystem.pauseGameMusicEvent += (isPaused) => { pauseGameMusicSignal.Dispatch (isPaused); };

			psdkEventSystem.onLocationLoadedEvent += (location, attr) => { locationLoadedSignal.Dispatch (ParsePsdkAttributes (location, true, attr)); };
			psdkEventSystem.onClosedEvent += (location, attr) => { locationClosedSignal.Dispatch (ParsePsdkAttributes (location, true, attr)); };
			psdkEventSystem.onLocationFailedEvent += (location, error) => { };
			psdkEventSystem.onConfigurationLoadedEvent += () => { };

			psdkEventSystem.onShownEvent += (location, attr) => OnShown (location, true, attr);
			psdkEventSystem.onShownFailedEvent += (location, attr) => OnShown (location, false, attr);
		}

		private void OnShown (string location, bool success, long attributes = 0)
		{
			var result = ParsePsdkAttributes (location, success, attributes);
			if (!locationResultPromises.ContainsKey (result.location)) {
				Debug.LogErrorFormat ("PSdkLocationManagerProvider.OnShown : location {0} already ended with other reason.", location);
				return;
			}

			var promise = locationResultPromises [result.location];
			locationResultPromises.Remove (result.location);

			promise.Dispatch (result);
		}

		private void OnShowFail (IPromise<LocationResult> promise, ApplicationLocation location, string failMessage)
		{
			var result = new LocationResult {
				location = location,
				success = false,
				message = failMessage,
			};

			promise.Dispatch (result);
		}


		public bool IsReady (ApplicationLocation location)
		{
			// psdkMgr.GetLocationManagerService().ReportLocation(location.ToString());
			return psdkMgr.GetLocationManagerService ().IsLocationReady (location.Name);
		}

		public bool IsViewVisible ()
		{
			return psdkMgr.GetLocationManagerService ().IsViewVisible ();
		}

		public IPopupHandle Get (ApplicationLocation location)
		{
			var popupHandle = injector.binder.GetInstance<IPopupHandle> ();
			popupHandle.Location = location;
			return popupHandle;
		}

		/// <summary>
		/// Promise to show the specified location if available immediately, otherwise reports failure
		/// </summary>
		/// <param name="location">Location.</param>
		public IPromise<LocationResult> Show (ApplicationLocation location)
		{
			Debug.Log ("PsdkLocationManagerProvider.Show for location " + location);

			var locationShownPromise = new Promise<LocationResult> ();

			// there is exists location, terminate both of them
			if (locationResultPromises.ContainsKey (location)) {
				// terminate last
				var message = string.Format ("PSdkLocationManagerProvider.Show : Terminate last location [{0}] that was already requested without result",
					location);
				var lastPromise = locationResultPromises [location];
				locationResultPromises.Remove (location);
				OnShowFail (lastPromise, location, message);

				// also terminate this
				message = string.Format ("PSdkLocationManagerProvider.Show : Terminate location [{0}] because last one was already requested without result",
					location);
				OnShowFail (locationShownPromise, location, message);
				return locationShownPromise;
			}

			// not ready
			if (!IsReady (location)) {
				var message = string.Format ("PSdkLocationManagerProvider.Show : location [{0}] not ready", location);
				OnShowFail (locationShownPromise, location, message);
				return locationShownPromise;
			}

			// show
			Debug.Log ("PsdkLocationManagerProvider : attempting to show " + location);
			psdkMgr.GetLocationManagerService ().ReportLocation (location.Name);
			crashTools.AddBreadCrumb ("PSDK.LocationManager.ShowLocation: " + location);

			locationResultPromises.Add (location, locationShownPromise);
			var showAttributes = psdkMgr.GetLocationManagerService ().Show (location.Name);

			// PSDK will not dispatch shown failed event in case there is no source
			// This should be fixed in a future psdk version and then this code can be removed:
			if (showAttributes == LocationMgrAttributes.LOCATION_MGR_ATTR_NO_SOURCE) {
				Debug.Log ("PsdkLocationManagerProvider - no source for location:" + location);
				OnShown (location.Name, false, showAttributes);
			}

			return locationShownPromise;
		}


		public LocationResult ParsePsdkAttributes (string location, bool success, long attributes)
		{
			var result = new LocationResult {
				location = ApplicationLocation.NameToEvent (location),
				success = success
			};

			if ((attributes & LocationMgrAttributes.LOCATION_MGR_ATTR_NO_SOURCE) == 0)
				result.sourceAssigned = false;
			if ((attributes & LocationMgrAttributes.LOCATION_MGR_ATTR_SOURCE_EXIST) > 0)
				result.sourceAssigned = true;
			if ((attributes & LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC) > 0)
				result.playsMusic = true;

			return result;
		}
	}
}