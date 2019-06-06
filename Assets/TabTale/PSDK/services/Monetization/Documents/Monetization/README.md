Monrtization Core integration:

Unity:

1) Integrate PublishingSDKCore.unitypackage. ( after importing the package look at Assets/PSDK/Documents/PublishingSDKCore/ReadME )
2) import the following unity packages:
	Monetization.unitypackage

3) put a proper bundle indentifier and product name
4) Register for Analytics events and implement analytics forward to your analytics agent:

iOS:

Android:

Add the following lines to the Assets/Plugins/Android/AndroidManifest.xml

  <uses-permission android:name="android.permission.INTERNET" />
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
   
   <uses-sdk
       android:minSdkVersion="15"
       android:targetSdkVersion="22" />

  <meta-data android:name="unityplayer.ForwardNativeEventsToDalvik" android:value="true" />

PSDK AppShelf initialisation example:

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

public class DemoAppShelf : MonoBehaviour {

	IPsdkAppShelf _appShelf;
	bool _appShelfVisible = false;

	public void Awake () {

		#region location listner  delagates 
		PsdkEventSystem.Instance.onClosedEvent 				+= OnClosed;
		PsdkEventSystem.Instance.onConfigurationLoadedEvent 	+= OnConfigurationLoaded;
		PsdkEventSystem.Instance.onLocationFailedEvent 		+= OnLocationFailed;
		PsdkEventSystem.Instance.onLocationLoadedEvent 		+= OnLocationLoaded;
		PsdkEventSystem.Instance.onShownEvent 				+= OnShown;
		#endregion


		if (PSDKMgr.Instance.Setup()) {
			
			// Getting psdk AppShelf after psdk setup
			_appShelf = PSDKMgr.Instance.GetAppShelf();
		}
	}


	public void Close() {
		Debug.Log ("Close app shelf");
		if (null != _appShelf ) 
			_appShelf.Close();
		else 
			Debug.LogError("Null -+- appShelf");
	}

	public void Show(string location) {
		if (null == _appShelf) {
			_appShelf = PSDKMgr.Instance.GetAppShelf();
			Debug.LogError("Null --- appShelf");
			return;
		}
		if (_appShelf.IsLocationReady(location)) {
			Debug.Log ("Show app shelf");
			bool showedAppShelf = _appShelf.Show(location);
		}
		else {
			Debug.Log ("AppShelf location " + location + " not ready !!");
		}
	}

	public void Update() {
		// Check android back button
		#if UNITY_ANDROID
		if (RuntimePlatform.Android == Application.platform) {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				if (_appShelfVisible) {
					Debug.Log ("AndroidBackButtonPressed:: AppShelf visible closing it");
					Close ();
					return;
				}
				Debug.Log ("AndroidBackButtonPressed:: Closing the App");
				Application.Quit();
			}
		}
		#endif
	} 
	

	void OnMouseDown() {
		this.transform.localScale = this.transform.localScale * 0.9f;
		Show("MoreApps");
	}
	
	void OnMouseUp() {
		if (Input.touchCount > 1) {
			return;
		}
		this.transform.localScale = this.transform.localScale / 0.9f;
	}
	

	void OnApplicationPause(bool paused) {
		if (! paused) {
				PSDKMgr.Instance.AppIsReady();
		}
	}


	public bool IsLocationReady(string location) {
		if (null == _appShelf) {
			Debug.LogError("Null -- appShelf");
			return false;
		}
		Debug.Log ("AppShelf::IsLocationReady:" + location);
		return _appShelf.IsLocationReady(location);
	}

	public void OnLocationLoaded(string location) {
		Debug.Log("AppShelf : UnityLocationMgrDelegate::OnLocationLoaded " + location);
	}
	
	public void OnLocationFailed(string location, string message) {
		Debug.Log("AppShelf : UnityLocationMgrDelegate::OnLocationFailed " + location + ", msg:" + message);
	}
	
	
	public void OnShown(string location) {
		Debug.Log("AppShelf : UnityLocationMgrDelegate::OnShown " + location);
		_appShelfVisible = true;
	}
	
	
	public void OnClosed(string location) {
		Debug.Log("AppShelf : UnityLocationMgrDelegate::OnClosed " + location);
		_appShelfVisible = false;
	}
	
	
	public void OnConfigurationLoaded() {
		Debug.Log("AppShelf : UnityLocationMgrDelegate::OnConfigurationLoaded ");
	}


}
