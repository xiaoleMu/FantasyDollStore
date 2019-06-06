using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {

	public class PsdkLocationManagerService  : IPsdkLocationManagerService {

		IPsdkLocationManagerService _impl;


		public PsdkLocationManagerService(IPsdkServiceManager sm) {
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkLocationManagerService(sm.GetImplementation()); break;
			#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkLocationManagerService(sm.GetImplementation()); break;
			#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor: 	_impl = new UnityEditorPsdkLocationManagerService(sm.GetImplementation()); break;
			default: throw new System.Exception("Platform not supported for PsdkLocationManagerService.");
			}
		}

		public bool Setup() {
			return _impl.Setup();
		}

		public void ReportLocation(string location){
			_impl.ReportLocation (location);
		}


		public long Show(string location) {
			PsdkEventSystem.Instance.PauseGameMusicEventNotification(LocationStatus(location) , true, PsdkEventSystem.MUSIC_PAUSE_CALLER_LOC_MGR);
			long rc = _impl.Show (location);
			if (rc == LocationMgrAttributes.LOCATION_MGR_ATTR_NO_SOURCE) {
				PsdkEventSystem.Instance.PauseGameMusicEventNotification(LocationStatus(location) , false, PsdkEventSystem.MUSIC_PAUSE_CALLER_LOC_MGR);
			}
			return rc;
		}

		public long LocationStatus(string location){
			return _impl.LocationStatus (location);
		}

		public bool IsViewVisible(){
			return _impl.IsViewVisible();
		}

		public bool IsLocationReady(string location){
			return (LocationStatus(location) != LocationMgrAttributes.LOCATION_MGR_ATTR_NO_SOURCE);
		}
		
		public void psdkStartedEvent() {
			_impl.psdkStartedEvent();
		}
	
		public IPsdkLocationManagerService GetImplementation() {
			return _impl;
		}

		public string GetLocations()
		{
			return _impl.GetLocations ();
		}

		public void LevelOfFirstPopupStatus(bool enabled) {
			_impl.LevelOfFirstPopupStatus (enabled);
		}
	}
}
