using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TabTale.Plugins.PSDK;

public class RateUsDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PSDKMgr.Instance.Setup();
	}
	
	public void onAddSatisfactionPointClick()
	{
		Debug.Log("onAddSatisfactionPointClick:: result - " + PSDKMgr.Instance.GetRateUs().SmallSatisfactionPointReached());
	}

	public void onAddLargeSatClick()
	{
		Debug.Log("onAddLargeSatClick:: result - " + PSDKMgr.Instance.GetRateUs().LargeSatisfactionPointReached());
	}

	public void onShouldShowClick()
	{
		Debug.Log("onShouldShowClick:: result - " + PSDKMgr.Instance.GetRateUs().ShouldShowRateUs());
	}

	public void onShowClick()
	{
		PSDKMgr.Instance.GetRateUs().Show();
	}

	public void onNeverShowClick()
	{
		PSDKMgr.Instance.GetRateUs().NeverShow();
	}
}
