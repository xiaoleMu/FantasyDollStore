# RewardedAds integration:

## Unity:

1) Integrate PublishingSDKCore.unitypackage. ( after importing the package look at Assets/PSDK/Documents/PublishingSDKCore/ReadME )
2) Import RewardedAds.unitypackage 

Change the API Compatibility leve to .NET 2 in the Unity OtherSetting at the Player settings.

You can look at example in  Assets/PSDK/Demo/RewardedAds.

Setup services before calling to PSDK start:

Register for events:

                PsdkRewardedAdsListnerService.Instance.adWillShowEvent += adWillShow;
                PsdkRewardedAdsListnerService.Instance.adDidCloseWithResultEvent += adDidClose;
                PsdkRewardedAdsListnerService.Instance.adIsNotReadyEvent += adIsNotReady;
                PsdkRewardedAdsListnerService.Instance.adIsReadyEvent += adIsReady;

Initialise PSDK and Rewarded Ads:

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

public class DemoRewardedAds : MonoBehaviour {
	
	IPsdkRewardedAds _rewardedAdsService;

	public void Awake () {

		PsdkRewardedAdsListnerService.Instance.adWillShowEvent += adWillShow;
		PsdkRewardedAdsListnerService.Instance.adDidCloseWithResultEvent += adDidClose;
		PsdkRewardedAdsListnerService.Instance.adIsNotReadyEvent += adIsNotReady;
		PsdkRewardedAdsListnerService.Instance.adIsReadyEvent += adIsReady;


		if (PSDKMgr.Instance.Setup()) {

//			Dictionary<string, string> keysDict = new Dictionary<string, string>() 
//			{
//				{ "ultraApplicationKey" , "2f558505"},
//				{ "ultraApplicationUserId" , "test id"},
//				{ "supersonicAdsApplicationKey" , "2aee6109"},
//				{ "supersonicAdsUserId" , "111"},
//				{ "adColonyApplicationId" , "appbdee68ae27024084bb334a"},
//				{ "adColonyZoneId" , "vzf8e4e97704c4445c87504e"},
//				{ "flurryVideoSpace" , "Cheating Tom IOS RV"},
//				{ "vungleApplicationId" , "53de45f423755cf021000027"},
//				{ "unityAdsGameId" , "14050"},
//				{ "unityAdsZoneId" , "14050-1399966087"}
//			};
//

			// Getting psdk AppShelf after psdk start
			_rewardedAdsService = PSDKMgr.Instance.GetRewardedAdsService();
		}
	}


	public void Show() {
		if (null == _rewardedAdsService)
			_rewardedAdsService = PSDKMgr.Instance.GetRewardedAdsService();

		if (null == _rewardedAdsService) {
			Debug.LogError("Null --- rewardedAds service");
			return;
		}

		Debug.Log ("1) Is Rewarded Ad playing before show : " + _rewardedAdsService.IsAdPlaying() );
		if (_rewardedAdsService.IsAdReady()) {
			Debug.Log ("Show Rewarded Ad");
			bool showedRewardedAd = _rewardedAdsService.ShowAd();
			Debug.Log ("2) Is Rewarded Ad playing after show : " + _rewardedAdsService.IsAdPlaying() );
		}
		else {
			Debug.Log ("RewardedAd  not ready !!");
		}
	}


	void OnMouseDown() {
		this.transform.localScale = this.transform.localScale * 0.9f;
		Show();
	}

	void OnMouseUp() {
		if (Input.touchCount > 1) {
			return;
		}
		this.transform.localScale = this.transform.localScale / 0.9f;
	}


	public bool IsAdReady() {
		if (null == _rewardedAdsService) {
			Debug.LogError("Null -- appShelf");
			return false;
		}
		return _rewardedAdsService.IsAdReady();
	}
	
	public bool IsAdPlaying() {
		if (null == _rewardedAdsService) {
			Debug.LogError("Null -- RewardedAds");
			return false;
		}
		return _rewardedAdsService.IsAdPlaying();
	}



	void adIsReady() {
		Debug.Log("DemoRewardedAds::adIsReady !");
	}
	
	void adIsNotReady() {
		Debug.Log("DemoRewardedAds::adIsNotReady !");
	}
	
	void adWillShow() {
		Debug.Log("DemoRewardedAds::adWillShow !");
	}
	
	void adDidClose(bool rewarded) {
		Debug.Log("DemoRewardedAds::adDidClose with result ! rewarded:" + rewarded);
	}
	



}



## iOS:

## Android:

if and AndroidManifest.xml does not exist you can copy one from:
/Applications/Unity/Unity.app/Contents/PlaybackEngines/AndroidPlayer/AndroidManifest.xml 

Add the following lines to the Assets/Plugins/Android/AndroidManifest.xml

in the Manifest section (before the application):

  <uses-permission android:name="android.permission.INTERNET" />
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
   
   <uses-sdk
       android:minSdkVersion="11"
       android:targetSdkVersion="19" />
       
       
    In the application tag add the following:
    
          <activity android:name="com.jirbo.adcolony.AdColonyOverlay"
            android:configChanges="keyboardHidden|orientation|screenSize"
            android:theme="@android:style/Theme.Translucent.NoTitleBar.Fullscreen" />
        <activity android:name="com.jirbo.adcolony.AdColonyFullscreen"
            android:configChanges="keyboardHidden|orientation|screenSize"
            android:theme="@android:style/Theme.Black.NoTitleBar.Fullscreen" />
        <activity android:name="com.jirbo.adcolony.AdColonyBrowser"
            android:configChanges="keyboardHidden|orientation|screenSize"
            android:theme="@android:style/Theme.Black.NoTitleBar.Fullscreen" />

        <activity android:name="com.supersonicads.sdk.android.WebViewActivity" 
            android:configChanges="orientation|screenSize" >
        </activity>
        
         <activity
                android:name="com.flurry.android.FlurryFullscreenTakeoverActivity"
                android:configChanges="keyboard|keyboardHidden|orientation|screenLayout|uiMode|screenSize|smallestScreenSize">
                 </activity>
  
         <activity
             android:name="com.vungle.publisher.FullScreenAdActivity"
             android:configChanges="keyboardHidden|orientation|screenSize"
             android:theme="@android:style/Theme.NoTitleBar.Fullscreen"/>
         <service android:name="com.vungle.publisher.VungleService"
             android:exported="false"/>
         
          <activity
                    android:name="com.unity3d.ads.android.view.UnityAdsFullscreenActivity" 
                android:configChanges="fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen"
                android:theme="@android:style/Theme.NoTitleBar.Fullscreen" />


In the activity android:name="com.unity3d.player.UnityPlayerNativeActivity" add/change the value to true:
  <meta-data android:name="unityplayer.ForwardNativeEventsToDalvik" android:value="true" />


