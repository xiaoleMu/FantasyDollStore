using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace TabTale.Plugins.PSDK
{
	public class PsdkRateUsService : IPsdkRateUs 
	{

		[DllImport ("__Internal")]
		private static extern void _DisplayRateUsModal(string applicationId, bool userAction);

		private int _satisfactionPointsThreshold;
		private int _coolDown;

		private int _satisfactionPoints;
		private bool _neverShow;
		private DateTime _lastDisplayed;


		public PsdkRateUsService(IPsdkServiceManager sm)
		{
			_coolDown = (int)PSDKMgr.Instance.LocalConfig.GetLong(new String[] {"rateUs","coolDown"},0);
			_satisfactionPointsThreshold = (int)PSDKMgr.Instance.LocalConfig.GetLong(new String[] {"rateUs","satisfactionPointsThreshold"},0);
			_neverShow = PlayerPrefs.GetInt("neverShow",0) == 1 ? true : false;
			_satisfactionPoints = PlayerPrefs.GetInt("satisfactionPoints",0);
			string dateString = PlayerPrefs.GetString("lastDisplayed","");
			if(dateString.Length > 0){
				DateTime.TryParse(dateString,out _lastDisplayed);
			}
		}



		public IPsdkRateUs GetImplementation()
		{
			return this;
		}

		public bool SmallSatisfactionPointReached ()
		{
			UpdateInfo();
			_satisfactionPoints++;
			return ShouldShow(false);
		}

		public bool LargeSatisfactionPointReached ()
		{
			UpdateInfo();
			_satisfactionPoints = _satisfactionPointsThreshold+1;
			return ShouldShow(false);
		}

		public bool ShouldShowRateUs ()
		{
			return ShouldShow(true);
		}
    
		public void Show (bool userAction = false) //equivalent to DialogResultEvent with Rate (other states were never used?)
		{
			_lastDisplayed = DateTime.Now;
			PlayerPrefs.SetString("lastDisplayed",_lastDisplayed.ToString());
            #if UNITY_IOS && !UNITY_EDITOR
			    _DisplayRateUsModal(PSDKMgr.Instance.BundleIdentifier, userAction);
#elif AMAZON
			Application.OpenURL ("amzn://apps/android?p=" + PSDKMgr.Instance.BundleIdentifier);
#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject appLauncher = new AndroidJavaObject("com.tabtale.publishingsdk.core.AppLauncher");
			if(appLauncher != null){
				appLauncher.Call("OpenAppImpl", "google", null, null, PsdkUtils.CurrentActivity);
			}
			else {
				Debug.Log("PsdkRateUsService:: Could not initiate appLauncher - class not found");
			}
#elif UNITY_EDITOR
            Debug.Log("called RateUs Show in editor mode with userAction:" + userAction);
			#endif
		}

		public void NeverShow () //equivalent to DialogResultEvent with neverShow true
		{
			_neverShow = true;
			PlayerPrefs.SetInt("neverShow",1);
		}

		private void UpdateInfo()
		{
			string bundleVersion = 		PsdkUtils.bundleVersion;
			int appVersion = 			int.Parse(bundleVersion.Replace(".",""));

			int lastUpdatedVersion = PlayerPrefs.GetInt("lastUpdatedVersion",-1);

			if (lastUpdatedVersion == -1) {
				Debug.LogWarning ("[RateUsService] Warning - Could not parse last version updated by rate us service");
			}

			if(appVersion > lastUpdatedVersion)
			{
				PlayerPrefs.SetInt("lastUpdatedVersion", appVersion);
				_satisfactionPoints = 0;
				PlayerPrefs.SetInt("satisfactionPoints",0);
			}

		}

		private bool ShouldShow(bool resetPoints = true)
		{
			if (_neverShow) {
				Debug.Log ("ShouldShow return false since _neverShow = true");
				return false;
			}

			if (!CoolDownOver()) {
				Debug.Log ("ShouldShow return false since CoolDown is not over");
				return false;
			}

			if (_satisfactionPoints < _satisfactionPointsThreshold) {
				Debug.Log ("ShouldShow return false since _satisfactionPoints '" + _satisfactionPoints + "' < _satisfactionPointsThreshold '" + _satisfactionPointsThreshold + "'");
				return false;
			}

			if (resetPoints) 
			{
				_satisfactionPoints = 0;
				PlayerPrefs.SetInt("satisfactionPoints",0);
			}
				
			return true;
		}

		private bool CoolDownOver()
		{
			if(DateTime.Now.Subtract(_lastDisplayed).TotalMinutes >= _coolDown)
				return true;
			
			return false;
		}

		public void psdkStartedEvent()
		{
			
		}
			
	}
		
}
