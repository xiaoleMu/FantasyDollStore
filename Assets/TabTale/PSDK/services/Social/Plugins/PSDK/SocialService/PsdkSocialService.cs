using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
#endif

#if UNITY_IPHONE

#endif

namespace TabTale.Plugins.PSDK {
	
	public class PsdkSocialService : IPsdkSocial {


		[DllImport ("__Internal")]
		private static extern void showDialog(string title, string message);

		public const string GooglePlayAuthWasCancelled = "gPlayAuthCancelled";
		private bool shouldShowLeaderboards = false;
		private bool shouldShowAchivements = false;
		private bool _hasNetworkConnection = true;

		public PsdkSocialService(IPsdkServiceManager sm)
		{
			#if UNITY_IPHONE
			UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
			#endif
			#if UNITY_ANDROID
			PlayGamesClientConfiguration.Builder configBuilder = new PlayGamesClientConfiguration.Builder();

			if(PSDKMgr.Instance.GetCrossDevicePersistency() != null){
				configBuilder.AddOauthScope("https://www.googleapis.com/auth/drive.appdata");
				configBuilder.AddOauthScope("https://www.googleapis.com/auth/drive.file");
			}

			PlayGamesClientConfiguration config = configBuilder.Build();
			PlayGamesPlatform.InitializeInstance(config);
			PlayGamesPlatform.DebugLogEnabled = true;
			PlayGamesPlatform.Activate();
			#endif
		}

		public void Init ()
		{
			Debug.Log ("SocialService::Init");
			//		#if UNITY_IPHONE
			//		UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
			//		#endif
			#if UNITY_ANDROID && !UNITY_EDITOR
			// add a callback for monitoring sign out
			GooglePlayGames.Native.NativeClient.RegisterSignOutCallback(OnGoogleSignOut);
			// Activate the Google Play Games platform
			PlayGamesPlatform.Activate();
			PlayGamesPlatform.Instance.ShowUIDelegate = ShowUIDelegateHandle;
			#endif

			#if ! UNITY_EDITOR
			Authenticate(false);
			#endif

		}

		public void Authenticate ()
		{
			Authenticate(true);
		}

		public void Authenticate (bool isAfterLoadingSequence)
		{
			Debug.Log ("SocialService::Authenticate: " + isAfterLoadingSequence);
			#if UNITY_IPHONE
			Social.localUser.Authenticate (OnAuthenticate);
			#elif UNITY_ANDROID
			if( !GoogleLoginWasCancelled() || isAfterLoadingSequence)
			{
				PsdkEventSystem.Instance.ExecCoroutine(CheckInternetConnectionAndAuthenticateCoroutine());
			}
			else
			{
				Debug.Log("SocialService - Will not attempt to authenticate since the user previously cancelled the request to authenticate");
			}
			#endif

		}

		private IEnumerator CheckInternetConnectionAndAuthenticateCoroutine()
		{
			WWW www = new WWW("https://ping.ttpsdk.info/TabTale-Test?bundleId="+PsdkUtils.BundleIdentifier);
			yield return www;

			if (www.error==null || www.error=="") {
				_hasNetworkConnection = true;
				Social.localUser.Authenticate (OnAuthenticate);
			}
		}

		public void SignOut ()
		{
			Debug.Log ("SocialService::SignOut");

			#if UNITY_ANDROID
			PlayGamesPlatform.Instance.SignOut();
			PreventGoogleAutoSignIn();
			#endif
		}

		void OnAuthenticate (bool success)
		{

			#if UNITY_ANDROID
			if (success) 
			{
				PlayerPrefs.DeleteKey(GooglePlayAuthWasCancelled);
			}
			else if(_hasNetworkConnection)
			{
				Debug.Log("Google play games authentication cancelled");
				PreventGoogleAutoSignIn();
			}
			#endif
			Debug.Log ("SocialService::OnAuthenticate: " + success);
			PsdkEventSystem.Instance.NotifyOnSocialAuthenticate(success);

			#if UNITY_IPHONE
			if (success==true)
			UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
			#endif



			OnAuthenticateShowAchievements(success);
			OnAuthenticateShowLeaderboard(success);
			#if UNITY_IPHONE
			UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
			#endif
		}

		public bool ReportProgress (string achievementID, double progress)
		{
			Debug.Log ("SocialService::ReportProgress id:" + achievementID + " progress:" + progress + " IsAuthenticated:" + IsAuthenticated());

			if (!IsAuthenticated())
				return false;

			UnityEngine.SocialPlatforms.GKAchievementReporter.ReportAchievement(achievementID, (float)progress, true);
			return true;
		}

		public void ResetAllAchievements ()
		{
			Debug.Log ("SocialService.ResetAllAchievements request");
			#if UNITY_IPHONE
			UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform.ResetAllAchievements ((b) => {
				Debug.Log ("SocialService.ResetAllAchievements responce" + b);});
			#endif
		}

		public void SetPlayerScore (string leaderboardId, long score)
		{
			Debug.logger.Log("SocialService", string.Format("SetPlayerScore:{0},Leaderboard id:{1} ",score, leaderboardId));

			if (!IsAuthenticated())
				return;

			Social.ReportScore (score, leaderboardId, OnReportScore);
		}

		void OnReportScore (bool success)
		{
			Debug.Log ("SocialService::OnReportScore success:" + success);
		}

		public void ShowLeaderboard ()
		{
			if (!IsAuthenticated()) {	

				Debug.Log ("TTUSocial::ShowLeaderboard not Authenticated");
				ShowUnAuthenticatedError(); //TODO - PUT IN A NATIVE NOTIFICATION

				#if UNITY_ANDROID
				shouldShowLeaderboards = true;
				Authenticate();
				#endif
				return;
			}

			Social.ShowLeaderboardUI ();
		}

		void OnAuthenticateShowLeaderboard (bool success)
		{
			if(shouldShowLeaderboards){
				if (success)
					Social.ShowLeaderboardUI ();
				shouldShowLeaderboards = false;
			}
		}

		#if UNITY_ANDROID
		private void ShowUIDelegateHandle(UIStatus status)
		{
			Debug.Log("ShowUIDelegateHandle - " + status.ToString());
			if(status == UIStatus.NotAuthorized){
				PreventGoogleAutoSignIn();
			}
		}
		#endif

		public void ShowAchievements ()
		{
			if (!IsAuthenticated()) {
				Debug.Log ("TTUSocial::Not Authenticated");
				ShowUnAuthenticatedError(); //TODO - PUT IN A NATIVE NOTIFICATION
				#if UNITY_ANDROID
				shouldShowAchivements = true;
				Authenticate();
				#endif
				return;
			}
			Social.ShowAchievementsUI ();
		}

		void OnAuthenticateShowAchievements (bool success)
		{
			if(shouldShowAchivements){
				if (success)
					Social.ShowAchievementsUI ();
				shouldShowAchivements = false;
			}
		}

		void ShowUnAuthenticatedError(string title = "")
		{
			#if UNITY_IPHONE
			showDialog("","Not Connected to game center");
			#endif
			#if UNITY_ANDROID
			AndroidJavaObject dialogObject = new AndroidJavaObject("com.tabtale.unitydialogandroid.UnityDialogFragment");

			if(dialogObject != null){
				dialogObject.Call("setTitle",new object[] {""});
				dialogObject.Call("setMessage",new object[] {"Not connected to Google Play"});
				AndroidJavaObject activity = PsdkUtils.CurrentActivity;
				if(activity != null){
					AndroidJavaObject fragmentManager = activity.Call<AndroidJavaObject>("getFragmentManager");
					if(fragmentManager != null){
						dialogObject.Call("show",new object[] {fragmentManager,"psdkUnityDialog"});
					}
					else {
						Debug.Log("Social::ShowUnAuthenticatedError:could not initiate fragmentManager.");
					}
				}
				else {
					Debug.Log("Social::ShowUnAuthenticatedError:could not initiate activity.");
				}
			}
			else {
				Debug.Log("Social::ShowUnAuthenticatedError:could not initiate dialogObject.");
			}
			#endif


//			if(GameApplication.Instance.GeneralDialog != null)
//			{
//				GeneralDialogData data = new GeneralDialogData ();
//				string dismissButton = "Ok";
//				data.title = title;
//				data.hasCloseButton = false;
//
//				if(Application.platform == RuntimePlatform.Android)
//					data.message = "Not connected to Google Play";
//				else
//					data.message = "Not Connected to game center";
//
//				data.buttons.Add(new BasicDialogButtonData("OK"));
//
//				GameApplication.Instance.GeneralDialog.Show(data);
//			}
		}

		public bool IsAuthenticated()
		{
			return Social.localUser.authenticated;

		}

		public string SocialId() 
		{
			return  Social.localUser.id;
		}

		#if UNITY_ANDROID

			private bool GoogleLoginWasCancelled() 
		{
			return PlayerPrefs.HasKey(GooglePlayAuthWasCancelled);
		}

		private void OnGoogleSignOut() 
		{
			Debug.Log ("Social Service - OnGoogleSignOut");
			PreventGoogleAutoSignIn();
		}

		private static void PreventGoogleAutoSignIn ()
		{
			if(Debug.isDebugBuild) Debug.Log("disabling auto sign in to google play games");
			PlayerPrefs.SetInt (GooglePlayAuthWasCancelled, 1);
			PlayerPrefs.Save ();
		}

		#endif

		public void psdkStartedEvent()
		{
			
		}

	}
}
