using UnityEngine;
using System.Collections;
using Prime31;

public class CCLog : MonoBehaviour 
{
	static public void ShowPopMsg(string pMsg, float pDuration = 3f)
	{
		#if UNITY_IPHONE
		CocoCommonBinding.ShowPromptMessage (pMsg, pDuration);
		#elif UNITY_ANDROID
		EtceteraAndroid.showToast (pMsg, true);
		#else
		Debug.Log ("GamePluginManager->ShowPromptMessage: " + pMsg + ", " + pDuration);
		#endif
	}
}
