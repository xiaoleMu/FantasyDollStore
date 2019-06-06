using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace TabTale.Plugins.PSDK {

	public class PsdkLocationComponent : MonoBehaviour {

		public bool handleSessionStartAutomatically = true;

		public InterstitialLocation locationName;

		public UnityEvent OnLoaded;
		public UnityEvent OnFailed;
		public UnityEvent OnShown;
		public UnityEvent OnClosed;

		private string _location;

		// Use this for initialization
		void Start () {
			_location = Psdk.InterstitialLocationToString(locationName);
			PsdkEventSystem.Instance.onLocationLoadedEvent += OnLocationLoaded;
			PsdkEventSystem.Instance.onLocationFailedEvent += OnLocationFailed;
			PsdkEventSystem.Instance.onShownEvent += OnLocationShown;
			PsdkEventSystem.Instance.onClosedEvent += OnLocationClosed;
			PsdkEventSystem.Instance.onShownFailedEvent += OnLocationClosed;
			PsdkEventSystem.Instance.onPsdkReady += OnPsdkReady;
		}

		void OnDestroy()
		{
			PsdkEventSystem.Instance.onLocationLoadedEvent -= OnLocationLoaded;
			PsdkEventSystem.Instance.onLocationFailedEvent -= OnLocationFailed;
			PsdkEventSystem.Instance.onShownEvent -= OnLocationShown;
			PsdkEventSystem.Instance.onClosedEvent -= OnLocationClosed;
			PsdkEventSystem.Instance.onShownFailedEvent -= OnLocationClosed;
			PsdkEventSystem.Instance.onPsdkReady -= OnPsdkReady;
		}

		public void Show()
		{
			if(PSDKMgr.Instance != null && PSDKMgr.Instance.GetLocationManagerService() != null)
				PSDKMgr.Instance.GetLocationManagerService().Show(_location);
		}

		public bool IsReady()
		{
			if(PSDKMgr.Instance != null && PSDKMgr.Instance.GetLocationManagerService() != null)
				return PSDKMgr.Instance.GetLocationManagerService().IsLocationReady(_location);
			return false;
		}

		private void OnLocationLoaded(string location, long attr)
		{
			if(handleSessionStartAutomatically && _location == PsdkLocation.sessionStart){
				Show();
			}
			if(location == _location && OnLoaded != null){
				OnLoaded.Invoke();
			}
		}

		private void OnLocationFailed(string location, string reason)
		{
			if(location == _location && OnFailed != null){
				OnFailed.Invoke();
			}
		}

		private void OnLocationShown(string location, long attr)
		{
			if(location == _location && OnShown != null){
				OnShown.Invoke();
			}
		}

		private void OnLocationClosed(string location, long attr)
		{
			if(location == _location && OnClosed != null){
				OnClosed.Invoke();
			}
		}

		private void OnPsdkReady()
		{
			if(handleSessionStartAutomatically && _location == PsdkLocation.sessionStart){
				Show();
			}
		}
	}
}