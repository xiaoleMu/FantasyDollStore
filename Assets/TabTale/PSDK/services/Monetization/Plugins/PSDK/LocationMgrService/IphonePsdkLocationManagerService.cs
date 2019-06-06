using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	public class IphonePsdkLocationManagerService  : IPsdkLocationManagerService{

		[DllImport("__Internal")]
		private static extern void psdkLocationMgrReportLocation(string location);

		[DllImport ("__Internal")]
		private static extern bool psdkSetupLocationsManager();

		[DllImport ("__Internal")]
		private static extern bool psdkLocationMgrIsViewVisible();

		[DllImport ("__Internal")]
		private static extern long psdkLocationMgrShow(string location);
		
		[DllImport ("__Internal")]
		private static extern long psdkLocationMgrIsLocationReady(string location);

		[DllImport ("__Internal")]
		private static extern string psdkLocationMgrGetLocations ();

		[DllImport ("__Internal")]
		private static extern void psdkLocationMgrLevelOfFirstPopupStatus (bool enabled);

		public IphonePsdkLocationManagerService(IPsdkServiceManager sm) {
		}

		public void ReportLocation(string location){
			psdkLocationMgrReportLocation (location);
		}

		public bool Setup() {
			return psdkSetupLocationsManager();
		}

		public long Show(string location){
			return psdkLocationMgrShow (location);
		}

		public long LocationStatus(string location){
			return psdkLocationMgrIsLocationReady (location);
		}

		public bool IsViewVisible(){
			return psdkLocationMgrIsViewVisible ();
		}
		
		public bool IsLocationReady(string location){
			//STUB just for the interface, the real implementation is in PsdkLocationManager
			return false;
		}
		
		public void psdkStartedEvent() {
		}

		public IPsdkLocationManagerService GetImplementation() {
			return this;
		}

		public string GetLocations()
		{
			return psdkLocationMgrGetLocations ();
		}

		public void LevelOfFirstPopupStatus(bool enabled) {
			psdkLocationMgrLevelOfFirstPopupStatus (enabled);
		}
	}
}
