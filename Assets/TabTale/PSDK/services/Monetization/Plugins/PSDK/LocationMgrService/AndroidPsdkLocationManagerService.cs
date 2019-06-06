#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	public class AndroidPsdkLocationManagerService  : IPsdkLocationManagerService, IPsdkAndroidService {
		
		AndroidPsdkServiceMgr _sm;
		AndroidJavaObject _androidLocationManagerService;
		
		public AndroidPsdkLocationManagerService(IPsdkServiceManager sm) {
			_sm=sm as AndroidPsdkServiceMgr;
		}

		public void psdkStartedEvent() {
			if (_androidLocationManagerService == null) {
				_androidLocationManagerService = GetUnityJavaObject();
			}
			
		}


		public bool Setup() {

			AndroidJavaObject javaLocationsDelegate = new AndroidJavaObject("com.tabtale.publishingsdk.unity.UnityLocationMgrDelegate");
			if (javaLocationsDelegate == null) {
				Debug.LogError("com.tabtale.publishingsdk.unity.UnityLocationMgrDelegate NULL");
				return false;
			}
			_sm.JavaClass.CallStatic("setLocationMgrDelegate", javaLocationsDelegate); 

			AndroidJavaObject webViewDelegate = new AndroidJavaObject("com.tabtale.publishingsdk.unity.UnityWebViewDelegate");
			if (webViewDelegate == null) {
				Debug.LogError("com.tabtale.publishingsdk.unity.UnityWebViewDelegate NULL");
				return false;
			}
			_sm.JavaClass.CallStatic("setWebViewDelegate", webViewDelegate); 
			
			return true;
		}


		/// <exception cref="System.Exception">location not available exception</exception>
		public long Show(string location) { // throw exception
			if (GetUnityJavaObject() == null) return LocationMgrAttributes.LOCATION_MGR_ATTR_NO_SOURCE;
                        try {
                        	return GetUnityJavaObject().Call<long>("show",location);    
                        }
                        catch (System.Exception e) {
                                Debug.LogException(e);
                                throw;
                        }
			return LocationMgrAttributes.LOCATION_MGR_ATTR_NO_SOURCE;
		}

		public void ReportLocation(string location){
			if (GetUnityJavaObject () != null) {
				try {
					GetUnityJavaObject().Call ("reportLocation",location);
				}
				catch (System.Exception e) {
					Debug.LogException(e);
					throw;
				}
			}
		}

		public void LevelOfFirstPopupStatus(bool enabled) {
			if (GetUnityJavaObject () != null) {
				try {
					GetUnityJavaObject().Call ("levelOfFirstPopupStatus",enabled);
				}
				catch (System.Exception e) {
					Debug.LogException(e);
					throw;
				}
			}
		}

		public bool IsViewVisible(){
			if (GetUnityJavaObject() == null) 
				return false;

			try {
				return GetUnityJavaObject().Call<bool>("isViewVisible");    
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				throw;
			}
			return false;
		}

		public long LocationStatus(string location){
			if (GetUnityJavaObject() == null) return LocationMgrAttributes.LOCATION_MGR_ATTR_NO_SOURCE;
			try {
				return GetUnityJavaObject().Call<long>("isLocationReady",location);    
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				throw;
			}
			return LocationMgrAttributes.LOCATION_MGR_ATTR_NO_SOURCE;
		}

		public bool IsLocationReady(string location){
			//STUB just for the interface, the real implementation is in PsdkLocationManager
			return false;
		}
		



		public AndroidJavaObject GetUnityJavaObject() {
			try {
				if (_androidLocationManagerService == null) 
					_androidLocationManagerService = _sm.GetUnityJavaObject().Call<AndroidJavaObject>("getLocationMgr");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}
			return _androidLocationManagerService;
		}

		public IPsdkLocationManagerService GetImplementation() {
			return this;
		}

		public string GetLocations()
		{
			if (GetUnityJavaObject () == null){
				return null;
			}
			try {
				return GetUnityJavaObject().Call<string>("getLocations");    
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				throw;
			}
		}
	}
}
#endif
