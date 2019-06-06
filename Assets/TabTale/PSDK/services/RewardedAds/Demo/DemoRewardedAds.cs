using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

public class DemoRewardedAds : MonoBehaviour {
	
	IPsdkRewardedAds _rewardedAdsService;

	public void Awake () {

		PsdkEventSystem.Instance.onRewardedAdWillShowEvent += adWillShow;
		PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent += adDidClose;
		PsdkEventSystem.Instance.onRewardedAdIsNotReadyEvent += adIsNotReady;
		PsdkEventSystem.Instance.onRewardedAdIsReadyEvent += adIsReady;


		if (PSDKMgr.Instance.Setup()) {
			Debug.Log("DemoRewardedAds - PSDK Setup success!");
			PSDKMgr.Instance.AppIsReady();
		}
	}


	public void Show() {
		if (PSDKMgr.Instance.GetRewardedAdsService() != null && PSDKMgr.Instance.GetRewardedAdsService().IsAdReady()) {
			PSDKMgr.Instance.GetRewardedAdsService().ShowAd();
		}
	}


	public void IsAdReady() {
		if (PSDKMgr.Instance.GetRewardedAdsService() != null)
			Debug.Log("DemoRewardedAds - IsAdReady = " + PSDKMgr.Instance.GetRewardedAdsService().IsAdReady() ); 
		else
			Debug.Log("DemoRewardedAds - rewarded ads service null");
		
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
		if (rewarded)
			adShouldReward();
		else
			adShouldNotReward();

		Debug.Log("DemoRewardedAds::adDidClose !");
	}
	
	void adShouldReward() {
		Debug.Log("DemoRewardedAds::adShouldReward !");
	}
	
	void adShouldNotReward() {
		Debug.Log("DemoRewardedAds::adShouldNotReward !");
	}




}
