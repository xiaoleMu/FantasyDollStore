PublishingSDK Core integration:

Unity:

1) import the following unity packages:

	PublishingSDKCore.unitypackage

2) Put psdk_ios.json/psdk_google.json/psdk_amazon.json configuration file in Assets/StreamingAssets
3) put a proper bundle indentifier and product name

iOS:
/* empty */

Android:
	call "Update Manifest" from PSDK Menu, and after that update the targetSdkVersion and minSdkVersion to the correct values.
	

   <uses-sdk
       android:minSdkVersion="15"
       android:targetSdkVersion="22" />

  <meta-data android:name="unityplayer.ForwardNativeEventsToDalvik" android:value="true" />

PSDK initialization example:


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

public class DemoAll : MonoBehaviour {


	public void Awake() {


		#region PSDK service setup and initialization 

		if (PSDKMgr.Instance.Setup()) {

			// Getting AppLifeCycleManager after psdk start
			_appLifeCycleMgr = PSDKMgr.Instance.GetAppLifecycleManager();
		}
	}

	void OnApplicationPause(bool paused) {
		if (! paused) {
			PSDKMgr.Instance.AppIsReady();
		}
	}

	#endregion App life cycle manager functinality


	#endregion analytics events		


}
