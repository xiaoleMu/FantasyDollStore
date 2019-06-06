using System;
using System.Collections;
using UnityEngine;

#if !UNITY_EDITOR
using Prime31;
#endif

namespace CocoPlay.Native
{
	public class CocoNative
	{
		public static IEnumerator ShowAlertView (string title, string message, string button, Action buttonAction, string otherButton, Action otherButtonAction)
		{
			// events
			var clickOther = false;
			var clicked = false;
			Action<string> clickEvent = buttonText => {
				clicked = true;
				clickOther = buttonText == otherButton;
			};

#if !UNITY_EDITOR
	#if UNITY_IOS
			EtceteraManager.alertButtonClickedEvent += clickEvent;
			var buttons = string.IsNullOrEmpty (otherButton) ? new[] { button } : new[] { button, otherButton };
			EtceteraBinding.showAlertWithTitleMessageAndButtons (title, message, buttons);
			while (!clicked) {
				yield return new WaitForEndOfFrame ();
			}
			EtceteraManager.alertButtonClickedEvent -= clickEvent;

	#elif UNITY_ANDROID
			Action cancelEvent = () => {
				clicked = true;
				clickOther = !string.IsNullOrEmpty (otherButton);
			};

			EtceteraAndroidManager.alertButtonClickedEvent += clickEvent;
			EtceteraAndroidManager.alertCancelledEvent += cancelEvent;
			if (string.IsNullOrEmpty (otherButton))
				EtceteraAndroid.showAlert (title, message, button);
			else
				EtceteraAndroid.showAlert (title, message, button, otherButton);
			while (!clicked) {
				yield return new WaitForEndOfFrame ();
			}
			EtceteraAndroidManager.alertButtonClickedEvent -= clickEvent;
			EtceteraAndroidManager.alertCancelledEvent -= cancelEvent;

	#endif
#else
			SimpleAlertView.Show (title, message, button, otherButton, clickEvent);
			while (!clicked) {
				yield return new WaitForEndOfFrame ();
			}
#endif

			// process result
			if (clickOther) {
				if (otherButtonAction != null) {
					otherButtonAction ();
				}
			} else {
				if (buttonAction != null) {
					buttonAction ();
				}
			}
		}

		public static void ShowPrompt (string message)
		{
#if !UNITY_EDITOR
	#if UNITY_IOS
			CocoCommonBinding.ShowPromptMessage (message, 2f);
	#elif UNITY_ANDROID
			EtceteraAndroid.showToast (message, true);
	#endif
#else
			Debug.LogErrorFormat ("CocoNative->ShowPrompt: {0}", message);
#endif
		}

		public static void Log (string message)
		{
#if !UNITY_EDITOR
	#if UNITY_IOS
			CocoCommonBinding.NSLog (message);
	#endif
#endif

			Debug.LogError (message);
		}

		public static void Log (string format, params object[] objects)
		{
			var logMsg = string.Format (format, objects);
			Log (logMsg);
		}
	}
}