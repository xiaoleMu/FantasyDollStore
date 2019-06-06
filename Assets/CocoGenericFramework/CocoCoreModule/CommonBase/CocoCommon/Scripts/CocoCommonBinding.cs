#region Feature Definitions

//#define ENABLE_FEATURE_SHARE
//#define ENABLE_FEATURE_FOR_PARENTS
//#define ENABLE_FEATURE_MORE_APPS
//#define ENABLE_FEATURE_PROMOTION
//#define ENABLE_FEATURE_PARENTAL_GATE

#endregion

using UnityEngine;
using System;
using System.Runtime.InteropServices;
#if UNITY_IPHONE
using Prime31;
#endif

#if UNITY_IPHONE

public class CocoCommonBinding : AbstractManager
{

	static CocoCommonBinding ()
	{
		AbstractManager.initialize (typeof(CocoCommonBinding));
	}

	#region Common APIs

	[DllImport("__Internal")]
	private static extern float _cocoCommonScreenScaleFactor ();

	public static float ScreenScaleFactor {
		get {
			if (Application.platform == RuntimePlatform.IPhonePlayer)
				return _cocoCommonScreenScaleFactor ();

			return 1.0f;
		}
	}

	[DllImport("__Internal")]
	private static extern bool _cocoCommonShowAppStoreInternally (long appId);

	public static bool ShowAppStoreInternally (long pAppId) {
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			return _cocoCommonShowAppStoreInternally (pAppId);

		return false;
	}

	[DllImport("__Internal")]
	private static extern bool _cocoCommonCameraDeviceIsAvailable ();

	public static bool CameraDeviceIsAvailable {
		get {
			if (Application.platform == RuntimePlatform.IPhonePlayer)
				return _cocoCommonCameraDeviceIsAvailable ();

			return false;
		}
	}

	[DllImport("__Internal")]
	private static extern string _getCountryCode();
	public static string getCountryCode()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			string tempStr = _getCountryCode ();
			string[] sArra = tempStr.Split ('_');
			string resultStr1 = sArra [sArra.Length - 1];
			string resultStr2 = resultStr1.Substring(0, 2).ToLower();
			return resultStr2;
		}
		return "us";
	}

	[DllImport("__Internal")]
	private static extern void _cocoCommonShowPromptMessage (string msg, float duration);

	public static void ShowPromptMessage (string pMsg, float pDuration)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_cocoCommonShowPromptMessage (pMsg, pDuration);
			return;
		}

		Debug.Log ("CocoCommonBinding->ShowPromptMessage: " + pMsg + ", " + pDuration);
	}

	[DllImport("__Internal")]
	private static extern void _cocoCommonNSLog(string msg);

	public static void NSLog (string msg){
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_cocoCommonNSLog (msg);
			return;
		}

		Debug.Log ("CocoCommonBinding->NSLog: " + msg);
	}

	#endregion


	#region Share Features

	// share
	public enum ShareType
	{
		ShareTypeSinaWeibo = 1,		/**< SinaWeibo */
		ShareTypeTencentWeibo,		/**< TencentWeibo */
		ShareTypeSohuWeibo,		/**< SohuWeibo */
		ShareType163Weibo,		/**< 163Weibo */
		ShareTypeDouBan,		/**< DouBan */
		ShareTypeQQSpace,		/**< QQSpace */
		ShareTypeRenren,		/**< Renren */
		ShareTypeKaixin,		/**< Kaixin */
		ShareTypePengyou,		/**< Pengyou */
		ShareTypeFacebook,		/**< Facebook */
		ShareTypeTwitter,		/**< Twitter */
		ShareTypeEvernote,		/**< Evernote */
		ShareTypeFoursquare,		/**< Foursquare */
		ShareTypeGooglePlus,		/**< GooglePlus */
		ShareTypeInstagram,		/**< Instagram */
		ShareTypeLinkedIn,		/**< LinkedIn */
		ShareTypeTumbir,		/**< Tumbir */
		ShareTypeMail,		/**< Mail */
		ShareTypeSMS,		/**< SMS */
		ShareTypeAirPrint,		/**< AirPrint */
		ShareTypeCopy,		/**< Copy */
		ShareTypeWeixiSession,		/**< WeixiSession */
		ShareTypeWeixiTimeline,		/**< WeixiTimeline */
		ShareTypeQQ,		/**< QQ */
		ShareTypeInstapaper,		/**< Instapaper */
		ShareTypePocket,		/**< Pocket */
		ShareTypeYouDaoNote,		/**< YouDaoNote */
		ShareTypeAny = 99		/**< Any */
	}

	public static event Action shareContentSucceeded;
	public static event Action shareContentCancelled;
	public static event Action<string> shareContentFailed;

	// event callbacks

	public void ShareContentSucceeded ()
	{
		if (shareContentSucceeded != null)
			shareContentSucceeded ();
	}

	public void ShareContentCancelled ()
	{
		if (shareContentCancelled != null)
			shareContentCancelled ();
	}

	public void ShareContentFailed (string pError)
	{
		if (shareContentFailed != null)
			shareContentFailed (pError);
	}


#if ENABLE_FEATURE_SHARE

	[DllImport("__Internal")]
	private static extern void _cocoCommonEnableSSO (bool isEnabled);
	[DllImport("__Internal")]
	private static extern void _cocoCommonShowContentForShareType (ShareType shareType, string imagePath, string title, string content);
	[DllImport("__Internal")]
	private static extern void _cocoCommonShowContentForShareMail (string imagePath, string title, string content, bool isHTML, string toAddress);

#endif

	public static void EnableSSO (bool pIsEnabled)
	{
#if ENABLE_FEATURE_SHARE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_cocoCommonEnableSSO (pIsEnabled);
			return;
		}

		Debug.Log ("CocoCommonBinding->EnableSSO: " + pIsEnabled);
#endif
	}

	public static void ShowContentForShareType (ShareType pShareType, string pImagePath, string pTitle, string pContent)
	{
#if ENABLE_FEATURE_SHARE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_cocoCommonShowContentForShareType (pShareType, pImagePath, pTitle, pContent);
			return;
		}

		Debug.Log ("CocoCommonBinding->ShowContentForShareType: " + pShareType + ", " + pImagePath + ", " + pTitle + ", " + pContent);
#endif
	}

	public static void ShowContentForShareMail (string pImagePath, string pTitle, string pContent, bool pIsHTML, string pToAddress)
	{
#if ENABLE_FEATURE_SHARE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_cocoCommonShowContentForShareMail (pImagePath, pTitle, pContent, pIsHTML, pToAddress);
			return;
		}

		Debug.Log ("CocoCommonBinding->ShowContentForShareMail: " + pImagePath + ", " + pTitle + ", " + pContent + ", " + pIsHTML + ", to: " + pToAddress);
#endif
	}

	#endregion


	#region ForParents Features

#if ENABLE_FEATURE_FOR_PARENTS
	[DllImport("__Internal")]
	private static extern void _cocoCommonShowForParents (long appId);
#endif

	public static void ShowForParents (long pAppId)
	{
#if ENABLE_FEATURE_FOR_PARENTS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_cocoCommonShowForParents (pAppId);
			return;
		}

		Debug.Log ("CocoCommonBinding->ShowForParents");
#endif
	}

	#endregion


	#region MoreApps Features

#if ENABLE_FEATURE_MORE_APPS
	[DllImport("__Internal")]
	private static extern void _cocoCommonShowMoreApps (bool useParentalGate);
	[DllImport("__Internal")]
	private static extern void _cocoCommonHideMoreApps ();
	[DllImport("__Internal")]
	private static extern void _cocoCommonShowMoreAppsView ();
#endif

	public static void ShowMoreApps ()
	{
#if ENABLE_FEATURE_MORE_APPS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_cocoCommonShowMoreApps (IsParentalGateEnabled);
			return;
		}

		Debug.Log ("CocoCommonBinding->ShowMoreApps");
#endif
	}

	public static void HideMoreApps ()
	{
#if ENABLE_FEATURE_MORE_APPS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_cocoCommonHideMoreApps ();
			return;
		}

		Debug.Log ("CocoCommonBinding->HideMoreApps");
#endif
	}

	public static void ShowMoreAppsView ()
	{
#if ENABLE_FEATURE_MORE_APPS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_cocoCommonShowMoreAppsView ();
			return;
		}

		Debug.Log ("CocoCommonBinding->ShowMoreAppsView");
#endif
	}
	#endregion


	#region Promotion Features

	// promotion
	public static event Action promotionClosed;

	// event callbacks

	public void PromotionClosed ()
	{
		if (promotionClosed != null)
			promotionClosed ();
	}

#if ENABLE_FEATURE_PROMOTION
	[DllImport("__Internal")]
	private static extern void _cocoCommonShowPromotion (string url, bool needCallback);
#endif

	public static void ShowPromotion (string pUrl) {
#if ENABLE_FEATURE_PROMOTION
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_cocoCommonShowPromotion (pUrl, (promotionClosed != null));
			return;
		}

		Debug.Log ("CocoCommonBinding->ShowPromotion: " + pUrl);
#endif
	}

	#endregion


	#region Parental Gate

	// parental gate
	public static event Action<bool> parentalGateClosed;

	// event callbacks

	public void ParentalGateVerifyFinished (string pResult)
	{
		if (parentalGateClosed != null)
			parentalGateClosed (pResult == "1");
	}

	public static bool IsParentalGateEnabled {
		get {
#if ENABLE_FEATURE_PARENTAL_GATE
			return true;
#else
			return false;
#endif
		}
	}

#if ENABLE_FEATURE_PARENTAL_GATE
	[DllImport("__Internal")]
	private static extern void _cocoCommonShowParentalGate ();
#endif

	public static void ShowParentalGate ()
	{
#if ENABLE_FEATURE_PARENTAL_GATE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_cocoCommonShowParentalGate ();
			return;
		}

		Debug.Log ("CocoCommonBinding->ShowParentalGate");
#endif
	}

	#endregion

	[DllImport("__Internal")]
private static extern bool _cocoPhotoLibraryIsAvailable();

public static bool PhotoLibraryIsAvailable {
	get {
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			return _cocoPhotoLibraryIsAvailable ();

		return false;
	}
}
		
}
#endif
