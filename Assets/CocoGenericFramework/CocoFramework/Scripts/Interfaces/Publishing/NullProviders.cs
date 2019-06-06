using UnityEngine;
using System;
using System.Collections;
using TabTale.Data;
using TabTale.Publishing;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using strange.extensions.signal.impl;

namespace TabTale
{
	public class NullService : IService
	{
		#region IService Implementation

		public ITask GetInitializer(IServiceResolver resolver)
		{
			return resolver.TaskFactory.FromEnumerableAction(Init);
		}

		IEnumerator Init()
		{
			yield break;
		}

		#endregion
	}


	public class NullPopupHandle : IPopupHandle
	{
		#region IPopupHandle implementation

		public Signal<LocationResult> Closed { get { return new Signal<LocationResult>(); } }

		public NullPopupHandle()
		{

		}

		public NullPopupHandle(ApplicationLocation location)
		{
			_location = location;
		}

		public void Open (float timeout = -1)
		{
			Debug.Log("NullPopupHandle.Open");
		}

		public void Open (Action<LocationResult> resultCallback, float timeout = -1)
		{
			Open();
		}

		public bool IsReady ()
		{
			return false;
		}

		private ApplicationLocation _location;
		public ApplicationLocation Location
		{
			get { return _location; }
			set { _location = value; }
		}

		private Signal<LocationResult> _result;
		public Signal<LocationResult> Result
		{
			get { return _result; }
		}

		public string Name { get { return "Null Popup"; }}

		#endregion


	}

	/*
	public class NullPopups : NullService, IPopupsProvider
	{
        public bool IsReady { get; private set; }

		public IEnumerator GetShowRoutine(ApplicationLocation location, PopupHandleReceiver handleReceiver, PopupResultHandler resultHandler)
		{
			Logger.LogDebug(string.Format("NullPopupsProvider, Showing popup for {0}", location));
			yield break;
		}


        public IModalHandle Show(ApplicationLocation location, bool isOverriding)
        {
            return Show(location, GameApplication.Instance.PopupResultHandler, isOverriding);
        }

		public IModalHandle Show(ApplicationLocation location, PopupResultHandler handler, bool isOverriding)
		{
			Logger.LogDebug(string.Format("NullPopupsProviderShowing, Show for {0}", location));
			return new NullPopupHandle();
		}


		public ContentState GetContentState(ApplicationLocation location)
		{
			Logger.LogInfo("NullPopupsProvider", string.Format("requesting state for location {0}", location));
			return ContentState.Unknown;
		}

		public void Subscribe(ApplicationLocation location, ContentStateChangeHandler handler)
		{
			Logger.LogInfo("NullPopupsProvider", string.Format("adding listener for state of location {0}", location));
		}

		public void Subscribe(ApplicationLocation location, IRetainer<ContentStateChangeHandler> target, ContentStateChangeHandler handler)
		{
			Logger.LogInfo("NullPopupsProvider", string.Format("subscribing to state of location {0}", location));
		}

		public void UnSubscribe(ContentStateChangeHandler handler)
		{
			Logger.LogInfo("NullPopupsProvider", "removing listener for state of some location");
		}

		public void UnSubscribe(object target)
		{
			Logger.LogInfo("NullPopupsProvider", "removing listener for state of some location...");
		}
	}
	*/

	public class NullLocations : ILocationManager
	{
		#region ILocationManager implementation

		public LocationLoadedSignal locationLoadedSignal { get; set; }

		public LocationClosedSignal locationClosedSignal { get; set; }

		IPromise<LocationResult> promise = new Promise<LocationResult>();

		public bool IsViewVisible()
		{
			return false;
		}

		public bool IsReady (ApplicationLocation location)
		{
			return false;
		}

		public IPopupHandle Get(ApplicationLocation location)
		{
			return new NullPopupHandle();
		}
		public IPromise<LocationResult> Show (ApplicationLocation location)
		{
			return promise;
		}

		#endregion

	}

	public class NullRewardedAds : IRewardedAds
	{
		private IPromise<bool> promise = new Promise<bool>();

		public IPromise<bool> ShowAd() { promise.Dispatch(false); return promise; }
		public bool IsAdReady() { return true; }
		public bool IsAdPlaying() { return false; }
	}

	public class NullBannerAds : IBannerAds
	{
		#region IBannerAds implementation

		public BannerAdsShownSignal 		bannerAdsShownSignal { get; private set; }

		public BannerAdsHiddenSignal 		bannerAdsHiddenSignal { get; private set; }

		public BannerAdsWillDisplaySignal 	bannerAdsWillDisplaySignal { get; private set; }

		public BannerAdsClosedSignal 		bannerAdsClosedSignal { get; private set; }

		public bool IsShowing { get { return true; } }

		public bool IsActive() { return false; }

		public float GetAdHeight() { return 0.0f; }

		public float GetAdHeightInPercentage() { return 0.0f; }

		public IPromise Show ()
		{
			return new Promise();
		}

		public IPromise Hide ()
		{
			 return new Promise();
		}

		public bool IsAlignedToTop ()
		{
			return false;
		}

		#endregion

	}

	public class NullAppInfoService : IAppInfo
	{
		#region IAppInfo implementation

		public string ApplicationId {
			get {
				return "NullAppInfoService";
			}
		}

		public string BundleIdentifier {
			get {
				return "NullBundleIdentifier";
			}
		}

		public string BundleVersion {
			get {
				return "0";
			}
		}

		#endregion

	}

	public class NullPsdkAppsFlyer : IAppsFlyer
	{
		#region IAppsFlyer implementation
		public void ReportPurchase(string price, string currency) { }

		#endregion
	}

	public class NullPsdkCrashTools : ICrashTools
	{
		#region ICrashTools implementation
		public void AddBreadCrumb (string crumb)
		{
			Debug.Log("NullPsdkCrashTools.AddBreadCrumb: " + crumb);
		}
		public void ClearAllBreadCrumbs ()
		{
			Debug.Log("NullPsdkCrashTools.ClearAllBreadCrumbs");
		}
		#endregion

	}
}