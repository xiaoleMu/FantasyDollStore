using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

public class LocationMgrDemo : MonoBehaviour {

	const string TEST_LOCATION = "moreApps";

	//sessionStart is a special location that should be implmented to show automatically once a psdk session begins. please notice how it is implemented below.
	const string SESSION_START_LOCATION_NAME = "sessionStart";

	void Start () {

		PsdkEventSystem.Instance.onClosedEvent 					+= OnClosed;
		PsdkEventSystem.Instance.onConfigurationLoadedEvent 	+= OnConfigurationLoaded;
		PsdkEventSystem.Instance.onLocationFailedEvent 			+= OnLocationFailed;
		PsdkEventSystem.Instance.onLocationLoadedEvent 			+= OnLocationLoaded;
		PsdkEventSystem.Instance.onShownEvent 					+= OnShown;
		PsdkEventSystem.Instance.onShownFailedEvent 			+= OnShownFailed;
		PsdkEventSystem.Instance.pauseGameMusicEvent 			+= OnPauseGameMusic;

		if (PSDKMgr.Instance.Setup()) {
			Debug.Log("LocationMgrDemo - PSDK Setup success!");
			PSDKMgr.Instance.AppIsReady();
		}
	}

	public void ReportLocation()
	{
		if(PSDKMgr.Instance.GetLocationManagerService() != null){
			Debug.Log("LocationMgrDemo : ReportLocation:"+TEST_LOCATION);
			PSDKMgr.Instance.GetLocationManagerService().ReportLocation(TEST_LOCATION);
		}
			
	}

	public void IsLocationReady()
	{
		if(PSDKMgr.Instance.GetLocationManagerService() != null)
			Debug.Log("LocationMgrDemo : IsLocationReady:"+TEST_LOCATION+": " + PSDKMgr.Instance.GetLocationManagerService().IsLocationReady(TEST_LOCATION));
	}

	public void Show()
	{
		if(PSDKMgr.Instance.GetLocationManagerService() != null)
			Debug.Log("LocationMgrDemo : Show:"+TEST_LOCATION+": " + PSDKMgr.Instance.GetLocationManagerService().Show(TEST_LOCATION));
	}

	public void OnLocationLoaded(string location, long attributes) {
		Debug.Log("LocationMgrDemo : UnityLocationMgrDelegate::OnLocationLoaded " + location);
		//sessionStart is called from 2 events, but will only show once when ready - handled by psdk
		if(location == SESSION_START_LOCATION_NAME)
			PSDKMgr.Instance.GetLocationManagerService().Show(SESSION_START_LOCATION_NAME); 
	}

	public void OnLocationFailed(string location, string message) {
		Debug.Log("LocationMgrDemo : UnityLocationMgrDelegate::OnLocationFailed " + location + ", msg:" + message);
	}


	public void OnShown(string location, long attributes) {
		Debug.Log("LocationMgrDemo : UnityLocationMgrDelegate::OnShown " + location);
	}

	public void OnShownFailed(string location, long attributes) {
		Debug.Log("LocationMgrDemo : UnityLocationMgrDelegate::OnShownFailed " + location);
	}


	public void OnClosed(string location, long attributes) {
		Debug.Log("LocationMgrDemo : UnityLocationMgrDelegate::OnClosed " + location);
	}


	public void OnConfigurationLoaded() {
		Debug.Log("LocationMgrDemo : UnityLocationMgrDelegate::OnConfigurationLoaded ");
	}

	public void OnPauseGameMusic(bool shouldPause)
	{
		if(shouldPause){
			//pause game music
		}
		else {
			//restart game music
		}
	}

	public void OnPsdkReady()
	{
		Debug.Log("LocationMgrDemo : UnityLocationMgrDelegate::OnPsdkReady ");
		//sessionStart is called from 2 events, but will only show once when ready - handled by psdk
		PSDKMgr.Instance.GetLocationManagerService().Show(SESSION_START_LOCATION_NAME);
	}
}
