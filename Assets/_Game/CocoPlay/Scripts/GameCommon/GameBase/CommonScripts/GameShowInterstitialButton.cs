using UnityEngine;
using System.Collections;
using CocoPlay;
using UnityEngine.UI;

public class GameShowInterstitialButton : CocoUINormalButton {

	private Text nameText;

	protected override void OnRegister ()
	{
		base.OnRegister ();

		nameText = GetComponentInChildren <Text> ();

		if (!CocoDebugSettingsData.Instance.IsGodModeEnabled) {
			gameObject.SetActive (false);
		}

		if (CocoDebugSettingsData.Instance.IsSkipInterstitialEnabled) {
			if (nameText != null)
				nameText.text = "Show Interstitial:false";
		} else {
			if (nameText != null)
				nameText.text ="Show Interstitial:true";
		}
	}

	protected override void OnClick ()
	{
		base.OnClick ();

		if (!CocoDebugSettingsData.Instance.IsGodModeEnabled) {
			return;
		}

		if (CocoDebugSettingsData.Instance.IsSkipInterstitialEnabled) {
			CocoDebugSettingsData.Instance.IsSkipInterstitialEnabled = false;
			if (nameText != null)
				nameText.text = "Show Interstitial:true";
		} else {
			CocoDebugSettingsData.Instance.IsSkipInterstitialEnabled = true;
			if (nameText != null)
				nameText.text ="Show Interstitial:false";
		}
	}
}
