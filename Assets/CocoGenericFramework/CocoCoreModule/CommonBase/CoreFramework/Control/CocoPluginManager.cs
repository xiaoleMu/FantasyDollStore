using UnityEngine;
using System.Collections.Generic;
using TabTale;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using System.Collections;
using System;
using CocoPlay.Localization;
using Game;
using TabTale.Cocoplay;

#if COCO_FAKE
using CocoUIButtonID = CocoPlay.Fake.CocoUIButtonID;
using CocoAudioID = CocoPlay.Fake.CocoAudioID;
using CocoLanguage = CocoPlay.Fake.CocoLanguage;
#else
using CocoUIButtonID = Game.CocoUIButtonID;
using CocoAudioID = Game.CocoAudioID;
using CocoLanguage = Game.CocoLanguage;
#endif
#if UNITY_2017_2_OR_NEWER
using Prime31;
#endif

namespace CocoPlay{
	public class CocoPluginManager : GameView
	{

		#region show Privacy Policy

		private static string m_PrivacyPolicyURL = "https://www.tabtale.com/privacy_policy_none_multiview.html";
		private static string m_TermsOfUseURL = "https://www.tabtale.com/terms-of-use/";

		/// <summary>
		/// show url
		/// </summary>
		/// <param name="url"></param>
		public void ShowURL (string url)
		{
#if UNITY_IOS && !UNITY_EDITOR
			if(Application.platform == RuntimePlatform.IPhonePlayer) {
				EtceteraBinding.showWebPage (url, false);
				return;
			}
#elif UNITY_ANDROID
			if (Application.platform == RuntimePlatform.Android) {
				EtceteraAndroid.showWebView (url);
				return;
			}
#else
			Application.OpenURL (url);

#endif
		}

		/// <summary>
		/// show the privacy policy.
		/// </summary>
		public void showPrivacyPolicy (){
			ShowURL (m_PrivacyPolicyURL);
		}
		
		/// <summary>
		/// show the terms of use.
		/// </summary>
		public void showTermsOfUse (){
			ShowURL (m_TermsOfUseURL);
		}

		#endregion


		#region Language
		/// <summary>
		/// Gets the language.
		/// </summary>
		/// <value>The language.</value>
		[System.Obsolete ("Use 'CocoLanguageSetting.Language' instead")]
		public CocoLanguage Language
		{
			get
			{
				return CocoLanguageSetting.Language;
			}
		}
		#endregion

		#region WebCamTexture reverse device 

		private List<string> listFrontCamScaleXReverseDevices;
		private List<string> listFrontCamScaleYReverseDevices;
		private List<string> listBackCamScaleXReverseDevices;
		private List<string> listBackCamScaleYReverseDevices;

		private void InitScaleReverseDevices()
		{
			//front camera
			listFrontCamScaleXReverseDevices = new List<string>();
			listFrontCamScaleYReverseDevices = new List<string>();
			//back camera
			listBackCamScaleXReverseDevices = new List<string>();
			listBackCamScaleYReverseDevices = new List<string>();

			#if UNITY_ANDROID
			listFrontCamScaleXReverseDevices.Add("Amazon KF");
			listFrontCamScaleXReverseDevices.Add("KFJW");

			listFrontCamScaleYReverseDevices.Add("Amazon KF");
			listFrontCamScaleYReverseDevices.Add("KFJW");

			listBackCamScaleXReverseDevices.Add("Amazon KF");
			listBackCamScaleXReverseDevices.Add("Nexus 5X");

			listBackCamScaleYReverseDevices.Add("Amazon KF");
			listBackCamScaleYReverseDevices.Add("Nexus 5X");

			#endif
		}

		public float FrontCamScaleXReverse{
			get{
				float pReverseValue = (Application.platform == RuntimePlatform.Android) ? -1f : 1f;
				foreach (string pDevice in listFrontCamScaleXReverseDevices){
					if (SystemInfo.deviceModel.Contains(pDevice)){
						pReverseValue = -pReverseValue;
						break;
					}
				}

				return pReverseValue;
			}
		}

		public float FrontCamScaleYReverse{
			get{
				float pReverseValue = 1f;
				foreach (string pDevice in listFrontCamScaleYReverseDevices){
					if (SystemInfo.deviceModel.Contains(pDevice)){
						pReverseValue = -pReverseValue;
						break;
					}
				}

				return pReverseValue;
			}
		}

		public float BackCamScaleXReverse{
			get{
				float pReverseValue = 1f;
				foreach (string pDevice in listBackCamScaleXReverseDevices){
					if (SystemInfo.deviceModel.Contains(pDevice)){
						pReverseValue = -pReverseValue;
						break;
					}
				}

				return pReverseValue;
			}
		}

		public float BackCamScaleYReverse{
			get{
				float pReverseValue = (Application.platform == RuntimePlatform.Android) ? -1f : 1f;
				foreach (string pDevice in listBackCamScaleYReverseDevices){
					if (SystemInfo.deviceModel.Contains(pDevice)){
						pReverseValue = -pReverseValue;
						break;
					}
				}

				return pReverseValue;
			}
		}

		#endregion




	    [Inject]
	    public RequestGeneralLocalNotificationSignal requestGeneralLocalNotificationSignal { get; set; }

	    protected override void OnRegister()
	    {
	        base.OnRegister();

	        InitScaleReverseDevices();
	        CocoLanguageSetting.LoadLanguage();

	#if UNITY_IOS
	        ClearLocalNotifications();
	        Debug.LogWarning("SystemInfo.deviceModel is:" + SystemInfo.deviceModel);
	#endif

			#if UNITY_ANDROID
			InitPermissionFlagMapping ();
			#endif
	    }

		public void RegisterNotifications (){
			#if UNITY_IOS
			if (UnityEngine.iOS.NotificationServices.enabledNotificationTypes != (NotificationType.Alert | NotificationType.Badge | NotificationType.Sound))
				UnityEngine.iOS.NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
			#endif
		}

	    protected override void OnUnRegister()
	    {

	        base.OnUnRegister();
	    }


	    void OnApplicationPause(bool paused)
	    {
	        //程序进入后台时
	        if (paused)
	        {
	            requestGeneralLocalNotificationSignal.Dispatch();
	        }
	        else
	        {
	            ClearLocalNotifications();
	        }
	    }

	    void OnApplicationQuit()
	    {
	        requestGeneralLocalNotificationSignal.Dispatch();
	    }

	    void ClearLocalNotifications()
	    {
	#if UNITY_IOS
            UnityEngine.iOS.LocalNotification m_LocalNotification = new UnityEngine.iOS.LocalNotification();
            m_LocalNotification.applicationIconBadgeNumber = -1;
            UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow(m_LocalNotification);
            UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
            EtceteraBinding.setBadgeCount (0);
	#endif
	    }

		#region Permission Check (Android)

		#if UNITY_ANDROID
		[Inject]
		public IModalityManager modalityManager { get; set;}

		public static string PermissionRequestFlag = "PermissionRequestFlag";
//        GameRecordStateModel recordStateModel { get{ return CocoRoot.GetInstance<GameRecordStateModel>();} }
        CocoUIButtonClickSignal uiButtonClickSignal { get{ return CocoRoot.GetInstance<CocoUIButtonClickSignal>();} }
		// permission (android)
		public const string PERMISSION_READ_EXTERNAL_STORAGE = "android.permission.READ_EXTERNAL_STORAGE";
		public const string PERMISSION_WRITE_EXTERNAL_STORAGE = "android.permission.WRITE_EXTERNAL_STORAGE";
		public const string PERMISSION_CAMERA = "android.permission.CAMERA";
		Dictionary<string, int> mPermissionFlagsMapping;

		private void InitPermissionFlagMapping ()
		{
			mPermissionFlagsMapping = new Dictionary<string, int> ();
			mPermissionFlagsMapping.Add (PERMISSION_CAMERA, 0x01);
			mPermissionFlagsMapping.Add (PERMISSION_READ_EXTERNAL_STORAGE, 0x02);
			mPermissionFlagsMapping.Add (PERMISSION_WRITE_EXTERNAL_STORAGE, 0x02);
		}

		public IEnumerator RequestPermission (string pPermission, string pRequestRationale, Action<bool> pEndAction)
		{
			bool granted = false;

			Debug.Log ("RequestPermission: [" + pPermission + "] " + pRequestRationale);

			if (mPermissionFlagsMapping.ContainsKey (pPermission)) {
				AndroidPermissionsWrapper wrapper = AndroidPermissionsWrapper.Instance;
				bool shouldShowRationale = false;
				bool shouldRequested = true;

				// check if permission is granted
				if (wrapper.CheckSelfPermission (pPermission)) {
					granted = true;
					shouldRequested = false;
				} else {
					if (wrapper.ShouldShowRequestPermissionRationale (pPermission)) {
						Debug.Log ("RequestPermission: ShouldShowRequestPermissionRationale -> true");
						shouldShowRationale = true;
					} else {
						// check if already request before this
						if ((PlayerPrefs.GetInt (PermissionRequestFlag) & mPermissionFlagsMapping [pPermission]) != 0) {	// already request
							// maybe user checked "Don't show again"
							shouldShowRationale = true;
							shouldRequested = false;
							Debug.Log ("RequestPermission: already request");
						}
					}
				}

				// show request rationale
				if (shouldShowRationale) {
					Debug.Log ("RequestPermission: show request rationale");
					string title = GameLocalization.Get ("txt_prompt_title_info");
					string okButton = GameLocalization.Get ("txt_prompt_button_ok");
					string cancelButton = string.Empty;
					if (!shouldRequested) {
						okButton = GameLocalization.Get ("txt_permission_open_settings");
						cancelButton = GameLocalization.Get ("txt_prompt_button_cancel");
					}
					bool clickOK = false;
					yield return StartCoroutine (ShowAlert (title, pRequestRationale, okButton, cancelButton, (ok) => {
						clickOK = ok;
					}));

					// open app settings
					if (!shouldRequested && clickOK) {
						Debug.Log ("RequestPermission: OpenAppSettings start");
						OpenAppSettings ();
						yield return new WaitForEndOfFrame ();
						yield return new WaitForEndOfFrame ();
						// assumed user allow request
						shouldRequested = true;
						Debug.Log ("RequestPermission: OpenAppSettings end");
					}
				}

				// request permission
				if (shouldRequested) {
					Debug.Log ("RequestPermission: request start");
					bool processing = true;
					Action<string[],bool[]> requestResultEvent = (permissions, grants) => {
						for (int i = 0; i < permissions.Length; i++) {
							Debug.Log ("RequestPermission: [" + permissions [i] + "]<->[" + pPermission + "]: " + grants [i]);
							if (permissions [i] == pPermission) {
								granted = grants [i];
								break;
							}
						}
						processing = false;
					};

					wrapper.OnRequestPermissionsResultEvent += requestResultEvent;
					wrapper.RequestPermissions (new string[] {pPermission});
					if (Application.platform == RuntimePlatform.Android) {
						while (processing)
							yield return new WaitForEndOfFrame ();
					} else {
						yield return new WaitForSeconds (1.0f);
						granted = UnityEngine.Random.value < 0.5f;
						processing = false;
					}
					wrapper.OnRequestPermissionsResultEvent -= requestResultEvent;
					Debug.Log ("RequestPermission: request end -> granted: " + granted);

					// record request flag
					if (!granted) {
						int recordPermissionRequestFlag = PlayerPrefs.GetInt (PermissionRequestFlag);
						recordPermissionRequestFlag |= mPermissionFlagsMapping [pPermission];
						PlayerPrefs.SetInt (PermissionRequestFlag, recordPermissionRequestFlag);
						PlayerPrefs.Save ();
					}
				}
			}

			if (pEndAction != null)
				pEndAction (granted);
		}

		bool clickOne = false;
		bool processing = true;
		public IEnumerator ShowAlert (string pTitle, string pMessage, string pButtonOne, string pButtonTwo, Action<bool> pEndAction)
		{
			clickOne = false;
			bool onlyOneButton = string.IsNullOrEmpty (pButtonTwo);
			processing = true;

		#if USE_SYSTEM_ALERT_VIEW
		Action<string> clickEvent = (button) => {
		clickOne = button == pButtonOne;
		processing = false;
		};
		Action cancelEvent = () => {
		clickOne = false;
		processing = false;
		};

		EtceteraAndroidManager.alertButtonClickedEvent += clickEvent;
		EtceteraAndroidManager.alertCancelledEvent += cancelEvent;
		if (onlyOneButton)
		EtceteraAndroid.showAlert (pTitle, pMessage, pButtonOne);
		else
		EtceteraAndroid.showAlert (pTitle, pMessage, pButtonOne, pButtonTwo);
		if (Application.platform == RuntimePlatform.Android) {
		while (processing)
		yield return null;
		} else {
		yield return new WaitForSeconds (1.0f);
		clickOne = UnityEngine.Random.value < 0.5f;
		processing = false;
		}
		EtceteraAndroidManager.alertButtonClickedEvent -= clickEvent;
		EtceteraAndroidManager.alertCancelledEvent -= cancelEvent;
		#else
	//		Action<string> clickEvent = (button) => {
	//			clickOne = (button == "ShowAlertViewOne");
	//			processing = false;
	//		};
            uiButtonClickSignal.AddListener (clickEvent);

			IModalHandle handle = modalityManager.Add (new AppModalHandle ("ShowAlertViewUIPrefab", IModalMaskType.NonMasked), true);
			GameUIAlertViewContentAdjuster adjuster = handle.Parent.GetComponentInChildren<GameUIAlertViewContentAdjuster> ();
			adjuster.UpdateContent (pTitle, pMessage, pButtonOne, pButtonTwo);

			while (processing)
				yield return null;
            uiButtonClickSignal.RemoveListener (clickEvent);
		#endif

			if (pEndAction != null)
				pEndAction (clickOne);
		}

        private void clickEvent (CocoUINormalButton pButtonName){
            if (pButtonName.UseButtonID)
                return;
			StartCoroutine (waitForTimeClickEvent (pButtonName));
		}

        private IEnumerator waitForTimeClickEvent(CocoUINormalButton pButtonName){
			yield return new WaitForSeconds (0.3f);

            clickOne = (pButtonName.ButtonKey == "ShowAlertViewOne");
			processing = false;
		}

		void OpenAppSettings ()
		{
		#if !UNITY_EDITOR
			AndroidJavaClass jcPlayer =  new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject joActivity = jcPlayer.GetStatic<AndroidJavaObject> ("currentActivity");

			// String action = Settings.ACTION_APPLICATION_DETAILS_SETTINGS;
			AndroidJavaClass jcSettings = new AndroidJavaClass("android.provider.Settings");
			string action = jcSettings.GetStatic<string>("ACTION_APPLICATION_DETAILS_SETTINGS");

			// Uri joUri = Uri.parse ("package:" + getPackageName ());
			string packageName = joActivity.Call<string> ("getPackageName");
			AndroidJavaClass jcUri = new AndroidJavaClass ("android.net.Uri");
			AndroidJavaObject joUri = jcUri.CallStatic<AndroidJavaObject> ("parse", "package:" + packageName);

			// Intent joIntent = new Intent (action, joUri);
			AndroidJavaObject joIntent = new AndroidJavaObject ("android.content.Intent", action, joUri);

			// startActivity (joIntent);
			joActivity.Call ("startActivity", joIntent);
		#endif
		}
		
		#endif

	#endregion
	}
}

