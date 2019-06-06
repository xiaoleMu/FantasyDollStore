using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_ANDROID
namespace Prime31
{
	public class AndroidNotificationConfiguration
	{
		public long secondsFromNow;
		public string title = string.Empty;
		public string subtitle = string.Empty;
		public string tickerText = string.Empty;
		public string extraData = string.Empty;
		public string smallIcon = string.Empty;
		public string largeIcon = string.Empty;
		public int requestCode = -1;
		public string groupKey = string.Empty;
		public int color = -1;
		public bool isGroupSummary;
		public int cancelsNotificationId = -1;
		public bool sound = true;

		// audio files should be placed in the Plugins/Android/Etcetera_lib/res/raw folder and only the filename without extension should be passed as the soundUri
		public string soundUri = string.Empty;
		public bool vibrate = true;
		public bool useExactTiming = false;


		public AndroidNotificationConfiguration( long secondsFromNow, string title, string subtitle, string tickerText )
		{
			this.secondsFromNow = secondsFromNow;
			this.title = title;
			this.subtitle = subtitle;
			this.tickerText = tickerText;
		}


		public AndroidNotificationConfiguration build()
		{
			if( requestCode == -1 )
				requestCode = Random.Range( 0, int.MaxValue );

			if( !sound && !string.IsNullOrEmpty( soundUri ) )
				Debug.Log( "sound is set to false but a soundUri is present. Are you sure this is what you wanted to do?" );

			return this;
		}

	}


	public enum TTSQueueMode
	{
		Flush = 0,
		Add = 1
	}


	public class EtceteraAndroid
	{
		private static AndroidJavaObject _plugin;

		public enum ScalingMode
		{
			None,
			AspectFit,
			Fill
		}


		static EtceteraAndroid()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			// find the plugin instance
			using( var pluginClass = new AndroidJavaClass( "com.prime31.EtceteraPlugin" ) )
				_plugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );
		}


		/// <summary>
		/// Loads up a Texture2D with the image at the given path
		/// </summary>
		public static Texture2D textureFromFileAtPath( string filePath )
		{
			var bytes = System.IO.File.ReadAllBytes( filePath );
			var tex = new Texture2D( 1, 1 );
			tex.LoadImage( bytes );
			tex.Apply();

			Debug.Log( "texture size: " + tex.width + "x" + tex.height );

			return tex;
		}


		/// <summary>
		/// Toggles low profile mode for the standard decor view on Honeycomb+
		/// </summary>
		public static void setSystemUiVisibilityToLowProfile( bool useLowProfile )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "setSystemUiVisibilityToLowProfile", useLowProfile );
		}


		/// <summary>
		/// Plays a video either locally (must be in the StreamingAssets folder or accessible via full path) or remotely.  The video format must be compatible with the current
		/// device.  Many devices have different supported video formats so choose the most common (probably 3gp).
		/// When playing a video from the StreamingAssets folder you only need to provide the filename via the pathOrUrl parameter.
		/// </summary>
		public static void playMovie( string pathOrUrl, uint bgColor, bool showControls, ScalingMode scalingMode, bool closeOnTouch )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "playMovie", pathOrUrl, (int)bgColor, showControls, (int)scalingMode, closeOnTouch );
		}


		/// <summary>
		/// Sets the theme for any alerts displayed. See the Android documentation for available themes: http:///developer.android.com/reference/android/app/AlertDialog.html#constants
		/// Make sure you know which Android OS version your app is currently installed on and ensure that you only use themes supported by that version!
		/// </summary>
		public static void setAlertDialogTheme( int theme )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "setAlertDialogTheme", theme );
		}


		/// <summary>
		/// Shows a Toast notification.  You can choose either short or long duration
		/// </summary>
		public static void showToast( string text, bool useShortDuration )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showToast", text, useShortDuration );
		}


		/// <summary>
		/// Shows a native alert with a single button
		/// </summary>
		public static void showAlert( string title, string message, string positiveButton )
		{
			showAlert( title, message, positiveButton, string.Empty );
		}


		/// <summary>
		/// Shows a native alert with two buttons
		/// </summary>
		public static void showAlert( string title, string message, string positiveButton, string negativeButton )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showAlert", title, message, positiveButton, negativeButton );
		}


		/// <summary>
		/// Shows an alert with a text prompt embedded in it
		/// </summary>
		public static void showAlertPrompt( string title, string message, string promptHint, string promptText, string positiveButton, string negativeButton )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showAlertPrompt", title, message, promptHint, promptText, positiveButton, negativeButton );
		}


		/// <summary>
		/// Shows an alert with two text prompts embedded in it
		/// </summary>
		public static void showAlertPromptWithTwoFields( string title, string message, string promptHintOne, string promptTextOne, string promptHintTwo, string promptTextTwo, string positiveButton, string negativeButton )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showAlertPromptWithTwoFields", title, message, promptHintOne, promptTextOne, promptHintTwo, promptTextTwo, positiveButton, negativeButton );
		}


		/// <summary>
		/// Shows a native progress indicator.  It will not be dismissed until you call hideProgressDialog
		/// </summary>
		public static void showProgressDialog( string title, string message )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showProgressDialog", title, message );
		}


		/// <summary>
		/// Hides the progress dialog
		/// </summary>
		public static void hideProgressDialog()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "hideProgressDialog" );
		}


		/// <summary>
		/// Shows a web view with the given url. To display local files they must be in the StreamingAssets folder and can be referenced with a url like this: "file:////android_asset/some_file.html"
		/// </summary>
		public static void showWebView( string url )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showWebView", url );
		}


		/// <summary>
		/// Shows a web view without a title bar optionally disabling the back button. If you disable the back button the only way
		/// a user can close the web view is if the web page they are on has a link with the protocol close:///  It is highly recommended
		/// to not disable the back button! Users are accustomed to it working as it is a default Android feature.
		/// </summary>
		public static void showCustomWebView( string url, bool disableTitle, bool disableBackButton )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showCustomWebView", url, disableTitle, disableBackButton );
		}


		/// <summary>
		/// Lets the user choose an email program (or uses the default one) to send an email prefilled with the arguments
		/// </summary>
		public static void showEmailComposer( string toAddress, string subject, string text, bool isHTML )
		{
			showEmailComposer( toAddress, subject, text, isHTML, string.Empty );
		}

		public static void showEmailComposer( string toAddress, string subject, string text, bool isHTML, string attachmentFilePath )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showEmailComposer", toAddress, subject, text, isHTML, attachmentFilePath );
		}


		/// <summary>
		/// Checks to see if the SMS composer is available on this device
		/// </summary>
		public static bool isSMSComposerAvailable()
		{
			if( Application.platform != RuntimePlatform.Android )
				return false;

			return _plugin.Call<bool>( "isSMSComposerAvailable" );
		}


		/// <summary>
		/// Shows the SMS composer with the body string and optional recipients
		/// </summary>
		public static void showSMSComposer( string body )
		{
			showSMSComposer( body, null );
		}

		public static void showSMSComposer( string body, string[] recipients )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			// prep the recipients if we have any
			var recipientString = string.Empty;
			if( recipients != null && recipients.Length > 0 )
			{
				recipientString = "smsto:";
				foreach( var r in recipients )
					recipientString += r + ";";
			}

			_plugin.Call( "showSMSComposer", recipientString, body );
		}


		/// <summary>
		/// resizes the image at path to the specified width/height
		/// </summary>
		public static bool resizeImageAtPath( string path, int width, int height )
		{
			if( Application.platform != RuntimePlatform.Android )
				return false;

			return _plugin.Call<bool>( "resizeImageAtPath", path, width, height );
		}


		/// <summary>
		/// Displays the Android native share intent for sharing just an image. Any apps installed that implement image sharing will show up
		/// in the chooser.
		/// </summary>
		public static void shareImageWithNativeShareIntent( string pathToImage, string chooserText )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "shareImageWithNativeShareIntent", pathToImage, chooserText );
		}


		/// <summary>
		/// Displays the Android native share intent for sharing text with an optional image. Any apps installed that implement image sharing will show up
		/// in the chooser.
		/// </summary>
		public static void shareWithNativeShareIntent( string text, string subject, string chooserText, string pathToImage = null )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "shareWithNativeShareIntent", text, subject, chooserText, pathToImage );
		}
		
		
		/// <summary>
		/// Displays the Android native share intent for sharing a file. Any apps installed that implement file sharing will show up
		/// in the chooser.
		/// </summary>
		public static void shareFileWithNativeShareIntent( string pathToFile, string chooserText, string mimeType = "video/*" )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "shareFileWithNativeShareIntent", pathToFile, chooserText, mimeType );
		}


		/// <summary>
		/// Prompts the user to take a photo. The photoChooserSucceededEvent/photoChooserCancelledEvent will fire with the result.
		/// </summary>
		public static void promptToTakePhoto( string name, bool insertIntoMediaStore = true, bool applyExifOrientation = false )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "promptToTakePhoto", name, insertIntoMediaStore, applyExifOrientation );
		}


		/// <summary>
		/// Prompts the user to choose an image from the photo album
		/// </summary>
		public static void promptForPictureFromAlbum( string name )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "promptForPictureFromAlbum", name );
		}


		/// <summary>
		/// Prompts the user to take a video and records it saving with the given name (no file extension is needed for the name)
		/// </summary>
		public static void promptToTakeVideo( string name )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "promptToTakeVideo", name );
		}


		/// <summary>
		/// Saves an image to the photo gallery
		/// </summary>
		public static bool saveImageToGallery( string pathToPhoto, string title )
		{
			if( Application.platform != RuntimePlatform.Android )
				return false;

			return _plugin.Call<bool>( "saveImageToGallery", pathToPhoto, title );
		}


		/// <summary>
		/// Scales the image. Scale should be 1 to not change the size and less than 1 for smaller images.
		/// </summary>
		public static void scaleImageAtPath( string pathToImage, float scale )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "scaleImageAtPath", pathToImage, scale );
		}


		/// <summary>
		/// Gets the image size for the image at the given path
		/// </summary>
		public static Vector2 getImageSizeAtPath( string pathToImage )
		{
			if( Application.platform != RuntimePlatform.Android )
				return Vector2.zero;

			var sizeString = _plugin.Call<string>( "getImageSizeAtPath", pathToImage );
			var parts = sizeString.Split( new char[] { ',' } );
			return new Vector2( int.Parse( parts[0] ), int.Parse( parts[1] ) );
		}


		public static void enableImmersiveMode( bool shouldEnable )
		{
			if( Application.platform == RuntimePlatform.Android )
				_plugin.Call( "enableImmersiveMode", shouldEnable ? 1 : 0 );
		}


		public static void loadContacts( int startingIndex, int totalToRetrieve )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "loadContacts", startingIndex, totalToRetrieve );
		}


		#region TTS

		/// <summary>
		/// Starts up the TTS system
		/// </summary>
		public static void initTTS()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "initTTS" );
		}


		/// <summary>
		/// Tears down and destroys the TTS system
		/// </summary>
		public static void teardownTTS()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "teardownTTS" );
		}


		/// <summary>
		/// Speaks the text passed in
		/// </summary>
		public static void speak( string text )
		{
			speak( text, TTSQueueMode.Add );
		}


		/// <summary>
		/// Speaks the text passed in optionally queuing it or flushing the current queue
		/// </summary>
		public static void speak( string text, TTSQueueMode queueMode )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "speak", text, (int)queueMode );
		}


		/// <summary>
		/// Stops the TTS system from speaking the current text
		/// </summary>
		public static void stop()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "stop" );
		}


		/// <summary>
		/// Plays silence for the specified duration in milliseconds
		/// </summary>
		public static void playSilence( long durationInMs, TTSQueueMode queueMode )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "playSilence", durationInMs, (int)queueMode );
		}


		/// <summary>
		/// Speech pitch. 1.0 is the normal pitch, lower values lower the tone of the synthesized voice, greater values increase it.
		/// </summary>
		public static void setPitch( float pitch )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "setPitch", pitch );
		}


		/// <summary>
		/// Speech rate. 1.0 is the normal speech rate, lower values slow down the speech (0.5 is half the
		/// normal speech rate), greater values accelerate it (2.0 is twice the normal speech rate).
		/// </summary>
		public static void setSpeechRate( float rate )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "setSpeechRate", rate );
		}

	#endregion


		#region Ask For Review

		/// <summary>
		/// Allows you to set the button titles when using any of the askForReview methods below
		/// </summary>
		public static void askForReviewSetButtonTitles( string remindMeLaterTitle, string dontAskAgainTitle, string rateItTitle )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "askForReviewSetButtonTitles", remindMeLaterTitle, dontAskAgainTitle, rateItTitle );
		}


		/// <summary>
		/// Shows the ask for review prompt with constraints. launchesUntilPrompt will not allow the prompt to be shown until it is launched that many times.
		/// hoursUntilFirstPrompt is the time since the first launch that needs to expire before the prompt is shown
		/// hoursBetweenPrompts is the time that needs to expire since the last time the prompt was shown
		/// Returns true if the prompt was shown.
		/// NOTE: once a user reviews your app the prompt will never show again until you call resetAskForReview
		/// </summary>
		public static bool askForReview( int launchesUntilPrompt, int hoursUntilFirstPrompt, int hoursBetweenPrompts, string title, string message, bool isAmazonAppStore = false )
		{
			if( Application.platform != RuntimePlatform.Android )
				return false;

			if( isAmazonAppStore )
				_plugin.Set<bool>( "isAmazonAppStore", true );

			return _plugin.Call<bool>( "askForReview", launchesUntilPrompt, hoursUntilFirstPrompt, hoursBetweenPrompts, title, message );
		}


		/// <summary>
		/// Shows the ask for review prompt immediately unless the user pressed the dont ask again button
		/// </summary>
		public static void askForReviewNow( string title, string message, bool isAmazonAppStore = false )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			if( isAmazonAppStore )
				_plugin.Set<bool>( "isAmazonAppStore", true );

			_plugin.Call( "askForReviewNow", title, message );
		}


		/// <summary>
		/// Resets all stored values such as launch count, first launch date, etc. Use this if you release a new version and want that version to be reviewed
		/// </summary>
		public static void resetAskForReview()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "resetAskForReview" );
		}


		/// <summary>
		/// Opens the review page in the Play Store directly. This will do not checks. It is here to allow you to make your own in-game UI for your ask for review prompt.
		/// </summary>
		public static void openReviewPageInPlayStore( bool isAmazonAppStore = false )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			if( isAmazonAppStore )
				_plugin.Set<bool>( "isAmazonAppStore", true );

			_plugin.Call( "openReviewPageInPlayStore" );
		}

		#endregion


		#region Camera access

		/// <summary>
		/// Starts up a video recording and displays a preview on top of the Unity game view. The preview dimensions are provided via the int parameters.
		/// camcorderProfile matches the CamcorderProfile class on the Android side. 0 is low, 1 is high, 2 - 6 are QCIF, CIF, 480p, 720p, 1080p. 7 is QVGA and
		/// 8 is 2160p. It is highly recommended to test thoroughly on your target hardware if you are not using 0 or 1 which are widely supported.
		/// </summary>
		public static void startCameraCapture( bool useFrontFacingCamera, int x, int y, int width, int height, int camcorderProfile = 1 )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "startCameraCapture", useFrontFacingCamera, x, y, width, height, camcorderProfile );
		}


		/// <summary>
		/// Stops the video recording and removes the camera preview
		/// </summary>
		public static void stopCameraCapture()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "stopCameraCapture");
		}


		/// <summary>
		/// Sets the current frame for the camera preview
		/// </summary>
		public static void cameraCaptureSetFrame( int x, int y, int width, int height )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "cameraCaptureSetFrame", x, y, width, height );
		}

		#endregion


		#region Inline web view

		/// <summary>
		/// Shows the inline web view. The values sent are multiplied by the screens dpi on the native side. Note that Unity's input handling will still occur so make sure
		/// nothing is touchable that is behind the web view while it is displayed.
		/// </summary>
		public static void inlineWebViewShow( string url, int x, int y, int width, int height )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "inlineWebViewShow", url, x, y, width, height );
		}


		/// <summary>
		/// Closes the inline web view
		/// </summary>
		public static void inlineWebViewClose()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "inlineWebViewClose");
		}


		/// <summary>
		/// Sets the current url for the inline web view
		/// </summary>
		public static void inlineWebViewSetUrl( string url )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "inlineWebViewSetUrl", url );
		}


		/// <summary>
		/// Sets the current frame for the inline web view. The values sent are multiplied by the screens dpi on the native side.
		/// </summary>
		public static void inlineWebViewSetFrame( int x, int y, int width, int height )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "inlineWebViewSetFrame", x, y, width, height );
		}

		#endregion


		#region Notifications

		// Schedules a notification to fire in secondsFromNow. The extraData will be returned with the notificationReceivedEvent and the int returned (requestCode) can be used to cancel the notification.
		// Note that you can specify your own requestCode by just passing in an int of your choosing for the requestCode parameter.
		[System.Obsolete( "Use the scheduleNotification variant that accepts a AndroidNotificationConfiguration parameter" )]
		public static int scheduleNotification( long secondsFromNow, string title, string subtitle, string tickerText, string extraData, int requestCode = -1 )
		{
			return scheduleNotification( new AndroidNotificationConfiguration( secondsFromNow, title, subtitle, tickerText )
			{
				extraData = extraData,
				requestCode = requestCode
			});
		}


		// Schedules a notification to fire in secondsFromNow. The extraData will be returned with the notificationReceivedEvent and the int returned (requestCode) can be used to cancel the notification.
		// Note that you can specify your own requestCode by just passing in an int of your choosing for the requestCode parameter.
		[System.Obsolete( "Use the scheduleNotification variant that accepts a AndroidNotificationConfiguration parameter" )]
		public static int scheduleNotification( long secondsFromNow, string title, string subtitle, string tickerText, string extraData, string smallIcon, string largeIcon, int requestCode = -1 )
		{
			return scheduleNotification( new AndroidNotificationConfiguration( secondsFromNow, title, subtitle, tickerText )
			{
				extraData = extraData,
				smallIcon = smallIcon,
				largeIcon = largeIcon,
				requestCode = requestCode
			});
		}


		/// <summary>
		/// Schedules a notification to fire. Use the fields in the AndroidNotificationConfiguration object to configure the notification. When dealing with grouped notifications
		/// the groupKey must match for all notifications and you must set one notification to be the summary notification (AndroidNotificationConfiguration.isGroupSummary
		/// must be true). The extraData will be returned with the notificationReceivedEvent and the int returned (requestCode) can be used to cancel the notification.
		/// If AndroidNotificationConfiguration.cancelsNotificationId is set when the notification fires it will call cancelNotification with that notification Id.
		/// </summary>
		public static int scheduleNotification( AndroidNotificationConfiguration config )
		{
			if( Application.platform != RuntimePlatform.Android )
				return -1;

			config.build();
			return _plugin.Call<int>( "scheduleNotification", Json.encode( config ) );
		}


		/// <summary>
		/// Cancels the notification with the given notificationId
		/// </summary>
		public static void cancelNotification( int notificationId )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "cancelNotification", notificationId );
		}


		/// <summary>
		/// Cancels all pending notifications
		/// </summary>
		public static void cancelAllNotifications()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "cancelAllNotifications" );
		}


		/// <summary>
		/// Checks to see if the app was launched from a notification. If it was the normal notificationReceivedEvent will fire.
		/// Calling this at every launch is a good idea if you are using notifications and want the extraData. This methiod
		/// will return the extraData from the last used intent everytime you call it.
		/// </summary>
		public static void checkForNotifications()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "checkForNotifications" );
		}

		#endregion


		#region Android M Permissions

		/// <summary>
		/// Requests permissions on Android M. Results in the onRequestPermissionsResultEvent firing.
		/// </summary>
		public static void requestPermissions( string[] permissions, int requestCode = 575757 )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "requestPermissions", new object[] { permissions, requestCode } );
		}


		/// <summary>
		/// Checks to see if you should provide permission rationale before requesting a permission
		/// </summary>
		public static bool shouldShowRequestPermissionRationale( string permission )
		{
			if( Application.platform != RuntimePlatform.Android )
				return false;

			return _plugin.Call<bool>( "shouldShowRequestPermissionRationale", permission );
		}


		/// <summary>
		/// Checks to see if the user granted a permission
		/// </summary>
		public static bool checkSelfPermission( string permission )
		{
			if( Application.platform != RuntimePlatform.Android )
				return false;

			return _plugin.Call<bool>( "checkSelfPermission", permission );
		}

		#endregion


		public class Contact
		{
			public string name;
			public List<string> emails;
			public List<string> phoneNumbers;
		}

	}
}
#endif
