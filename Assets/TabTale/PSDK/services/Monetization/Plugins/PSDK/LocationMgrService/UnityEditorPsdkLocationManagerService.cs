using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {

	public class UnityEditorPsdkLocationManagerService  : IPsdkLocationManagerService{

		public UnityEditorPsdkLocationManagerService(IPsdkServiceManager sm) {
		}

		public bool Setup() {
			return true;
		}

		public void ReportLocation(string location){
		
		}

		public long Show(string location) {
			return PsdkLocationDebugButtons.Instance.Show (location);
		}

		public long LocationStatus(string location){
			return PsdkLocationDebugButtons.Instance.LocationStatus (location);
		}

		public bool IsViewVisible(){
			return PsdkLocationDebugButtons.Instance.IsViewVisible();
		}

		public bool IsLocationReady(string location){
			//STUB just for the interface, the real implementation is in PsdkLocationManager
			return PsdkLocationDebugButtons.Instance.IsLocationReady(location);
		}
		
		//
//		public string IsLocationReady(string location) {
//			return PsdkLocationDebugButtons.Instance.IsLocationReady (location);
//		}

		public void psdkStartedEvent() {
		}
		
		public IPsdkLocationManagerService GetImplementation() {
			return this;
		}

		public string GetLocations()
		{
			return "";
		}

		public void LevelOfFirstPopupStatus(bool enabled) {
		}
	}
}
