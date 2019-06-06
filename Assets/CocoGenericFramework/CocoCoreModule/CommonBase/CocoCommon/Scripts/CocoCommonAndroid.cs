#if UNITY_ANDROID

//#define ENABLE_SHARESDK

using UnityEngine;
using System;
using Prime31;
using System.Collections.Generic;

public class CocoCommonAndroid : AbstractManager
{

	public static event Action shareContentSucceeded;
	public static event Action shareContentCancelled;
	public static event Action<string> shareContentFailed;

	private static AndroidJavaObject _plugin;

	static CocoCommonAndroid ()
	{
		AbstractManager.initialize (typeof(CocoCommonAndroid));
		
		if (Application.platform != RuntimePlatform.Android)
			return;
		
		// find the plugin instance
		using (AndroidJavaClass pluginClass = new AndroidJavaClass ("com.cocoplay.cococommonplugin.CocoCommonPlugin"))
			_plugin = pluginClass.CallStatic<AndroidJavaObject> ("instance");
	}

	public static bool IsAppInstalled (string pAppPackageName)
	{
		if (Application.platform != RuntimePlatform.Android)
			return false;

		return _plugin.Call<bool> ("isAppInstalled", pAppPackageName);
	}

//	public static bool CanOpenUrl (string pUrl)
//	{
//		if (Application.platform != RuntimePlatform.Android)
//			return false;
//		
//		return _plugin.Call<bool> ("canOpenUrl", pUrl);
//	}

	// language
	
	public static string GetCurrentLanguage ()
	{
		if (Application.platform != RuntimePlatform.Android)
			return "en-US";
		
		return _plugin.Call<string> ("getCurrentLanguage");
	}
	
}
#endif
