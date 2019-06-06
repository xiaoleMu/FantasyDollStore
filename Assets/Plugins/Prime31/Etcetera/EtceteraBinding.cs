using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using Prime31;


#if UNITY_IOS
namespace Prime31
{
	public enum P31RemoteNotificationType
	{
		None = 0,
		Badge = 1 << 0,
		Sound = 1 << 1,
		Alert = 1 << 2
	};


	public enum UIInterfaceOrientation
	{
	   Portrait = 1,
	   PortraitUpsideDown = 2,
	   LandscapeLeft = 4,
	   LandscapeRight = 3
	};


	public enum PhotoPromptType
	{
		Camera = 0,
		Album,
		CameraAndAlbum
	};


	public class EtceteraBinding
	{
		/// <summary>
		/// Takes a screenshot and puts it in the Application.persistentDataPath directory (which is Documents on iOS)
		/// Optional completion handler provides the path to the image.
		/// </summary>
	    public static IEnumerator takeScreenShot( string filename )
	    {
			return takeScreenShot( filename, null );
	    }


	    public static IEnumerator takeScreenShot( string filename, Action<string> completionHandler )
	    {
			yield return AbstractManager.coroutineSurrogate.StartCoroutine( getScreenShotTexture( tex =>
			{
				var path = Path.Combine( Application.persistentDataPath, filename );
				File.WriteAllBytes( path, tex.EncodeToPNG() );

				if( completionHandler != null )
					completionHandler( path );
			}) );
	    }


		public static IEnumerator getScreenShotTexture( Action<Texture2D> completionHandler )
		{
			yield return new WaitForEndOfFrame();

			var tex = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
			tex.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
			tex.Apply();

			completionHandler( tex );
		}


		[DllImport("__Internal")]
	    private static extern bool _etceteraApplicationCanOpenUrl( string url );

		/// <summary>
		/// Returns whether the application can open the url
		/// </summary>
	    public static bool applicationCanOpenUrl( string url )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _etceteraApplicationCanOpenUrl( url );
			return false;
	    }


		#region Pasteboard

		[DllImport("__Internal")]
		private static extern string _etceteraGetPasteboardString();

		/// <summary>
		/// Gets the current pasteboard string data. Returns NULL if there is none.
		/// </summary>
		public static string getPasteboardString()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _etceteraGetPasteboardString();
			return string.Empty;
		}


		[DllImport("__Internal")]
		private static extern void _etceteraSetPasteboardString( string str );

		/// <summary>
		/// Sets the pasteboard string data
		/// </summary>
		public static void setPasteboardString( string str )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraSetPasteboardString( str );
		}


		[DllImport("__Internal")]
		private static extern void _etceteraSetPasteboardImage( byte[] bytes, int length );

		/// <summary>
		/// Sets the pasteboard image data. The imageBytes data should be the raw data from a valid image file.
		/// </summary>
		public static void setPasteboardImage( byte[] imageBytes )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraSetPasteboardImage( imageBytes, imageBytes.Length );
		}

		#endregion


		#region Language

	    [DllImport("__Internal")]
	    private static extern string _etceteraGetCurrentLanguage();

		/// <summary>
		/// Returns the locale currently set on the device
		/// </summary>
	    public static string getCurrentLanguage()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _etceteraGetCurrentLanguage();
			return "en";
	    }


	    [DllImport("__Internal")]
	    private static extern string _etceteraLocaleObjectForKey( bool useAutoupdatingLocale, string key );

		/// <summary>
		/// Wraps the NSLocale objectForKey method. Passing true for useAutoUpdatingLocale will use the autoupdatingCurrentLocale, false will use the currentLocale
		/// Some useful keys to request are kCFLocaleCurrencySymbolKey and kCFLocaleCountryCodeKey
		/// </summary>
	    public static string localeObjectForKey( bool useAutoUpdatingLocale, string key )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _etceteraLocaleObjectForKey( useAutoUpdatingLocale, key );
			return string.Empty;
	    }


	    [DllImport("__Internal")]
	    private static extern string _etceteraGetLocalizedString( string key, string defaultValue );

		/// <summary>
		/// Uses the Xcode Localizable.strings system to get a localized version of the given string
		/// </summary>
	    public static string getLocalizedString( string key, string defaultValue )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _etceteraGetLocalizedString( key, defaultValue );
			return string.Empty;
	    }

		#endregion;


		#region UIAlertView and P31AlertView

		// Shows a standard Apple alert with the given title, message and buttonTitle
		[System.Obsolete( "Use the showAlertWithTitleMessageAndButtons. This method will be removed." )]
	    public static void showAlertWithTitleMessageAndButton( string title, string message, string buttonTitle )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				showAlertWithTitleMessageAndButtons( title, message, new string[] { buttonTitle } );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraShowAlertWithTitleMessageAndButtons( string title, string message, string buttons );

		/// <summary>
		/// Shows a standard Apple alert with the given title, message and an array of buttons. At least one button must be included.
		/// </summary>
	    public static void showAlertWithTitleMessageAndButtons( string title, string message, string[] buttons )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraShowAlertWithTitleMessageAndButtons( title, message, Prime31.Json.encode( buttons ) );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraShowPromptWithOneField( string title, string message, string placeHolder, bool autocomplete );

		/// <summary>
		/// Shows a prompt with one text field
		/// </summary>
	    public static void showPromptWithOneField( string title, string message, string placeHolder, bool autocomplete )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraShowPromptWithOneField( title, message, placeHolder, autocomplete );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraShowPromptWithTwoFields( string title, string message, string placeHolder1, string placeHolder2, bool autocomplete );

		/// <summary>
		/// Shows a prompt with two text fields
		/// </summary>
	    public static void showPromptWithTwoFields( string title, string message, string placeHolder1, string placeHolder2, bool autocomplete )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraShowPromptWithTwoFields( title, message, placeHolder1, placeHolder2, autocomplete );
	    }


		[DllImport("__Internal")]
		private static extern string _etceteraDismissAlertView();

		/// <summary>
		/// Dismisses an alert view shown by the showAlert* or showPrompt* methods. Note that this method is only callable under certain, specific situations due to the way UnityPause works.
		/// </summary>
		public static void dismissAlertView()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraDismissAlertView();
		}

		#endregion;


		#region Web, SMS and Mail

		[DllImport("__Internal")]
		private static extern void _etceteraShowWebPage( string url, bool showControls );

		/// <summary>
		/// Opens a web view with the url (Url can either be a resource on the web or a local file) and optional controls (back, forward, copy, open in Safari)
		/// </summary>
		public static void showWebPage( string url, bool showControls )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraShowWebPage( url, showControls );
		}


		[DllImport("__Internal")]
		private static extern void _etceteraShowWebPageInSafariViewController( string url );

		/// <summary>
		/// iOS 9+. Opens a the SafariViewController with the given URL. If the SafariViewController is not available this will default to calling
		/// showWebPage( url, true )
		/// </summary>
		public static void showWebPageInSafariViewController( string url )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraShowWebPageInSafariViewController( url );
		}


	    [DllImport("__Internal")]
	    private static extern bool _etceteraIsEmailAvailable();

		/// <summary>
		/// Checks to see if an email account is setup on the device
		/// </summary>
	    public static bool isEmailAvailable()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _etceteraIsEmailAvailable();
			return false;
	    }


	    [DllImport("__Internal")]
	    private static extern bool _etceteraIsSMSAvailable();

		/// <summary>
		/// Checks to see if SMS is available on the current device
		/// </summary>
	    public static bool isSMSAvailable()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _etceteraIsSMSAvailable();
			return false;
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraShowMailComposer( string toAddress, string subject, string body, bool isHTML );

		/// <summary>
		/// Opens the mail composer with the given information
		/// </summary>
	    public static void showMailComposer( string toAddress, string subject, string body, bool isHTML )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraShowMailComposer( toAddress, subject, body, isHTML );
	    }


		/// <summary>
		/// Opens the mail composer with a screenshot of the current state of the game attached
		/// </summary>
	    public static IEnumerator showMailComposerWithScreenshot( string toAddress, string subject, string body, bool isHTML )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				yield return AbstractManager.coroutineSurrogate.StartCoroutine( getScreenShotTexture( tex =>
				{
					var bytes = tex.EncodeToPNG();
					showMailComposerWithAttachment( bytes, "image/png", "screenshot.png", toAddress, subject, body, isHTML );
				}) );
			}
	    }


		/// <summary>
		/// Opens the mail composer with the given attachment file. The attachment data must be stored in a file on disk.
		/// </summary>
	    public static void showMailComposerWithAttachment( string filePathToAttachment, string attachmentMimeType, string attachmentFilename, string toAddress, string subject, string body, bool isHTML )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				if( !filePathToAttachment.StartsWith( "/" ) )
				{
					Debug.Log( "file path passed to showMailComposerWithAttachment is not a legit path: " + filePathToAttachment + ". Be sure to test your paths with File.Exists before using them!" );
					return;
				}

				if( !File.Exists( filePathToAttachment ) )
				{
					Debug.Log( "file path passed to showMailComposerWithAttachment does not exist: " + filePathToAttachment + ". Be sure to test your paths with File.Exists before using them!" );
					return;
				}

				var bytes = File.ReadAllBytes( filePathToAttachment );
				showMailComposerWithAttachment( bytes, attachmentMimeType, attachmentFilename, toAddress, subject, body, isHTML );
			}
	    }


		[DllImport("__Internal")]
		private static extern void _etceteraShowMailComposerWithRawAttachment( byte[] bytes, int length, string attachmentMimeType, string attachmentFilename, string toAddress, string subject, string body, bool isHTML );

		/// <summary>
		/// Opens the mail composer with the given attachment
		/// </summary>
	    public static void showMailComposerWithAttachment( byte[] attachmentData, string attachmentMimeType, string attachmentFilename, string toAddress, string subject, string body, bool isHTML )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraShowMailComposerWithRawAttachment( attachmentData, attachmentData.Length, attachmentMimeType, attachmentFilename, toAddress, subject, body, isHTML );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraShowSMSComposer( string recipients, string body );

		/// <summary>
		/// Opens the sms composer with the given body and optional recipients
		/// </summary>
	    public static void showSMSComposer( string body )
	    {
	        showSMSComposer( new string[]{}, body );
	    }


	    public static void showSMSComposer( string[] recipients, string body )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraShowSMSComposer( Prime31.Json.encode( recipients ), body );
	    }

		#endregion;


		#region Activity View

	    [DllImport("__Internal")]
	    private static extern void _etceteraShowActivityView();

		/// <summary>
		/// Shows a simple native spinner.  You must call hideActivityView to hide it
		/// </summary>
	    public static void showActivityView()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraShowActivityView();
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraHideActivityView();

		/// <summary>
		/// Hides any activity views that are showing
		/// </summary>
	    public static void hideActivityView()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraHideActivityView();
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraShowBezelActivityViewWithLabel( string label );

		/// <summary>
		/// Shows a bezel activity view with a label
		/// </summary>
	    public static void showBezelActivityViewWithLabel( string label )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraShowBezelActivityViewWithLabel( label );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraShowBezelActivityViewWithImage( string label, string imagePath );

		/// <summary>
		/// Shows a bezel activity view with a label and image
		/// </summary>
	    public static void showBezelActivityViewWithImage( string label, string imagePath )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraShowBezelActivityViewWithImage( label, imagePath );
	    }

		#endregion;


		#region Ask For Review, Photo and Push Notifications
		
	    [DllImport("__Internal")]
	    private static extern void _etceteraAskForReviewNatively();

		/// <summary>
		/// iOS 10.3+ only! Displays the App Store review page or does nothing, depending on Apple's mood. There are no callbacks and no
		/// indications if this method does anything at all because that is how Apple operates.
		/// </summary>
	    public static void askForReviewNatively()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraAskForReviewNatively();
	    }


	    [DllImport("__Internal")]
	    private static extern bool _etceteraAskForReview( int launchesUntilPrompt, int hoursUntilFirstPrompt, float hoursBetweenPrompts, string title, string message, string iTunesAppId );

		/// <summary>
		/// Shows the ask for review prompt with constraints.
		/// launchesUntilPrompt will not allow the prompt to be shown until it is launched that many times.
		/// hoursUntilFirstPrompt is the time since the first launch that needs to expire before the prompt is shown
		/// hoursBetweenPrompts is the time that needs to expire since the last time the prompt was shown
		/// Returns true if the prompt was shown.
		/// </summary>
	    public static bool askForReview( int launchesUntilPrompt, int hoursUntilFirstPrompt, float hoursBetweenPrompts, string title, string message, string iTunesAppId )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _etceteraAskForReview( launchesUntilPrompt, hoursUntilFirstPrompt, hoursBetweenPrompts, title, message, iTunesAppId );
			return false;
	    }


		/// <summary>
	    /// Legacy parameters defaulting to 48 hours until the first prompt occurs
		/// </summary>
	    public static void askForReview( int launchesUntilPrompt, float hoursBetweenPrompts, string title, string message, string iTunesAppId )
	    {
			askForReview( launchesUntilPrompt, 48, hoursBetweenPrompts, title, message, iTunesAppId );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraAskForReviewImmediately( string title, string message, string iTunesAppId );

		/// <summary>
		/// Opens the ask for review dialogue immediately
		/// </summary>
	    public static void askForReview( string title, string message, string iTunesAppId )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraAskForReviewImmediately( title, message, iTunesAppId );
	    }


		[DllImport("__Internal")]
	    private static extern void _etceteraOpenAppStoreReviewPage( string iTunesAppId );

		/// <summary>
		/// Opens App Store to the specified appId
		/// </summary>
	    public static void openAppStoreReviewPage( string iTunesAppId )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraOpenAppStoreReviewPage( iTunesAppId );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraSetPopoverPoint( float xPos, float yPos );

		/// <summary>
		/// Sets the position from which the popover for prompting for a photo will show when on an iPad. Note that this is in iOS screen space!
		/// </summary>
	    public static void setPopoverPoint( float xPos, float yPos )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraSetPopoverPoint( xPos, yPos );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraPromptForPhoto( float scaledToSize, int promptType, float jpegCompression, bool allowsEditing );

		/// <summary>
		/// for backwards compatibility
		/// </summary>
		public static void promptForPhoto( float scaledToSize )
		{
			promptForPhoto( scaledToSize, PhotoPromptType.CameraAndAlbum );
		}

	    public static void promptForPhoto( float scaledToSize, PhotoPromptType promptType )
	    {
	    	promptForPhoto( scaledToSize, promptType, 0.8f, false );
	    }


		/// <summary>
		/// Prompts the user to either take a photo or choose from the photo library. scaledToSize should be set
		/// less than 1.0f in most cases to avoid getting a huge image from the camera or photo library unless you plan to resize
		/// the image later. jpegCompression should be between 0.1 - 1. Photos are automatically rotated to match the EXIF data. Note that
		/// if you pass in a value of 0 or less for jpegCompression you will get back a PNG file instead.
		/// When complete either the imagePickerCancelledEvent or imagePickerChoseImageEvent event will fire.
		/// </summary>
		public static void promptForPhoto( float scaledToSize, PhotoPromptType promptType, float jpegCompression, bool allowsEditing )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraPromptForPhoto( scaledToSize, (int)promptType, jpegCompression, allowsEditing );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraPromptForVideo( int promptType );

		/// <summary>
		/// Prompts the user to either take a video or choose from the media library. Results in the imagePickerCancelled/imagePickerChoseImageEvent
		/// firing.
		/// </summary>
	    public static void promptForVideo( PhotoPromptType promptType )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer )
	    		_etceteraPromptForVideo( (int)promptType );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraPromptForMultiplePhotos( int maxNumberOfPhotos, float scaledToSize, float jpegCompression );

		/// <summary>
		/// Prompts the user to choose one or more photos from the photo library. scaledToSize should be set
		/// less than 1.0f in most cases to avoid getting a huge image from the camera or photo library unless you plan to resize
		/// the image later. jpegCompression should be between 0 - 1. Photos are automatically rotated to match the EXIF data.
		/// Fires the same events as promptForPhoto.
		/// </summary>
		public static void promptForMultiplePhotos( int maxNumberOfPhotos, float scaledToSize, float jpegCompression = 0.8f )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraPromptForMultiplePhotos( maxNumberOfPhotos, scaledToSize, jpegCompression );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraResizeImageAtPath( string filePath, float width, float height );

		/// <summary>
		/// Resizes and optionally crops the image at the given path. Note that the image will be saved as a JPEG to keep EXIF data intact if possible.
		/// </summary>
	    public static void resizeImageAtPath( string filePath, float width, float height )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraResizeImageAtPath( filePath, width, height );
	    }


	    [DllImport("__Internal")]
	    private static extern string _etceteraGetImageSize( string filePath );

		/// <summary>
		/// Gets the size of the image at the given path.  Returns 0,0 for invalid paths
		/// </summary>
	    public static Vector2 getImageSize( string filePath )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				var res = _etceteraGetImageSize( filePath );
				var parts = res.Split( new char[] { ',' } );
				if( parts.Length == 2 )
					return new Vector2( float.Parse( parts[0] ), float.Parse( parts[1] ) );
			}

			return Vector2.zero;
	    }


		[DllImport("__Internal")]
	    private static extern void _etceteraSaveImageToPhotoAlbum( string filePath );

		/// <summary>
		/// Writes the given image to the users photo album
		/// </summary>
	    public static void saveImageToPhotoAlbum( string filePath )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraSaveImageToPhotoAlbum( filePath );
	    }


		[DllImport("__Internal")]
	    private static extern void _etceteraSaveVideoToSavedPhotosAlbum( string filePath );

		/// <summary>
		/// Writes the given video to the users photo album. Results in the saveVideoToPhotoAlbumSucceeded/FailedEvent firing.
		/// </summary>
	    public static void saveVideoToSavedPhotosAlbum( string filePath )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraSaveVideoToSavedPhotosAlbum( filePath );
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraSetUrbanAirshipCredentials( string appKey, string appSecret, string alias );

		/// <summary>
		/// Sets the Urban Airship credentials and optionally the alias. Set these before calling registerForRemoteNotifications
		/// </summary>
		public static void setUrbanAirshipCredentials( string appKey, string appSecret )
		{
			setUrbanAirshipCredentials( appKey, appSecret, null );
		}

	    public static void setUrbanAirshipCredentials( string appKey, string appSecret, string alias )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraSetUrbanAirshipCredentials( appKey, appSecret, alias );
	    }


		/// <summary>
		/// Sets the Push.io credentials and optionally the PushIO categories. Set these before calling registerForRemoteNotifications
		/// </summary>
		public static void setPushIOCredentials( string apiKey )
		{
			setPushIOCredentials( apiKey, null );
		}

	    public static void setPushIOCredentials( string apiKey, string[] categories )
	    {
			EtceteraManager.pushIOApiKey = apiKey;
			EtceteraManager.pushIOCategories = categories;
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraRegisterForRemoteNotifications( int types );

		/// <summary>
		/// Registers the game for remote (push) notifications.  types is a bitmask
		/// </summary>
	    public static void registerForRemoteNotifications( P31RemoteNotificationType types )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraRegisterForRemoteNotifications( (int)types );
	    }


		/// <summary>
		/// Registers the deviceToken with GameThrive. Note that a GameThrive app ID is required to use GameThrive.
		/// </summary>
		public static IEnumerator registerDeviceWithGameThrive( string gameThriveAppId, string deviceToken, Dictionary<string,string> additionalParameters = null, Action<WWW> completionHandler = null )
		{
			var url = string.Format( "https://gamethrive.com/api/v1/players" );

			var parameters = new Dictionary<string,string>();
			parameters.Add( "device_type", "0" );
			parameters.Add( "app_id", gameThriveAppId );
			parameters.Add( "identifier", deviceToken );

			if( additionalParameters != null )
			{
				foreach( var key in additionalParameters.Keys )
					parameters.Add( key, additionalParameters[key] );
			}

			var json = Json.encode( parameters );
			var bytes = System.Text.Encoding.UTF8.GetBytes( json );


			var headers = new Dictionary<string,string>();
			headers.Add( "Content-Type", "application/json" );

			using( var www = new WWW( url, bytes, headers ) )
			{
				yield return www;

				if( completionHandler != null )
					completionHandler( www );
			}
		}


	    [DllImport("__Internal")]
	    private static extern int _etceteraGetEnabledRemoteNotificationTypes();

		/// <summary>
		/// Gets the bitmasked notification types the user has registered for
		/// </summary>
	    public static P31RemoteNotificationType getEnabledRemoteNotificationTypes()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return ( P31RemoteNotificationType )_etceteraGetEnabledRemoteNotificationTypes();
			return P31RemoteNotificationType.None;
	    }
		
		
		[DllImport("__Internal")]
		private static extern void _etceteraScheduleLocalNotification( int secondsFromNow, string text, string action, int badgeCount, string sound, string launchImage );

		public static void scheduleLocalNotification( int secondsFromNow, string text, string action )
		{
			scheduleLocalNotification( secondsFromNow, text, action, 0, string.Empty, string.Empty );
		}

		// Schedules a local notification.  Text is the text in the alert prompt, action is the button text, sound is an audio file in your app bundle and launchImage is a different default.png to use when launching.
		public static void scheduleLocalNotification( int secondsFromNow, string text, string action, int badgeCount, string sound, string launchImage )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraScheduleLocalNotification( secondsFromNow, text, action, badgeCount, sound, launchImage );
		}


		[DllImport("__Internal")]
		private static extern void _etceteraCancelAllLocalNotifications();

		// Cancels all scheduled notifications
		public static void cancelAllLocalNotifications()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraCancelAllLocalNotifications();
		}


	    [DllImport("__Internal")]
	    private static extern int _etceteraGetBadgeCount();

		/// <summary>
		/// Gets the current application badge count
		/// </summary>
	    public static int getBadgeCount()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _etceteraGetBadgeCount();
			return 0;
	    }


	    [DllImport("__Internal")]
	    private static extern void _etceteraSetBadgeCount( int badgeCount );

		/// <summary>
		/// Sets the current application badge count
		/// </summary>
	    public static void setBadgeCount( int badgeCount )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraSetBadgeCount( badgeCount );
	    }


	    [DllImport("__Internal")]
	    private static extern int _etceteraGetStatusBarOrientation();

		/// <summary>
		/// Gets the current UIApplication's status bar orientation
		/// </summary>
	    public static UIInterfaceOrientation getStatusBarOrientation()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return (UIInterfaceOrientation)_etceteraGetStatusBarOrientation();
			return UIInterfaceOrientation.Portrait;
	    }

		#endregion;


		#region Inline web view

		[DllImport("__Internal")]
		private static extern void _etceteraInlineWebViewShow( int x, int y, int width, int height );

		/// <summary>
		/// Shows the inline web view. Remember, iOS uses points not pixels for positioning and layout!
		/// </summary>
		public static void inlineWebViewShow( int x, int y, int width, int height )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraInlineWebViewShow( x, y, width, height );
		}


		[DllImport("__Internal")]
		private static extern void _etceteraInlineWebViewClose();

		/// <summary>
		/// Closes the inline web view
		/// </summary>
		public static void inlineWebViewClose()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraInlineWebViewClose();
		}


		[DllImport("__Internal")]
		private static extern void _etceteraInlineWebViewSetUrl( string url );

		/// <summary>
		/// Sets the current url for the inline web view
		/// </summary>
		public static void inlineWebViewSetUrl( string url )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraInlineWebViewSetUrl( url );
		}


		[DllImport("__Internal")]
		private static extern void _etceteraInlineWebViewSetFrame( int x, int y, int width, int height );

		/// <summary>
		/// Sets the current frame for the inline web view
		/// </summary>
		public static void inlineWebViewSetFrame( int x, int y, int width, int height )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraInlineWebViewSetFrame( x, y, width, height );
		}

		#endregion


		#region Camera access

		[DllImport("__Internal")]
		private static extern void _etceteraStartCameraCapture( bool useFrontFacingCamera, int x, int y, int width, int height );

		/// <summary>
		/// Starts up a video recording and displays a preview on top of the Unity game view. Remember, iOS uses points not pixels for positioning and layout!
		/// </summary>
		public static void startCameraCapture( bool useFrontFacingCamera, int x, int y, int width, int height )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraStartCameraCapture( useFrontFacingCamera, x, y, width, height );
		}


		[DllImport("__Internal")]
		private static extern void _etceteraStopCameraCapture();

		/// <summary>
		/// Stops the video recording and fires the videoRecordingSucceeded/FailedEvent
		/// </summary>
		public static void stopCameraCapture()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraStopCameraCapture();
		}


		[DllImport("__Internal")]
		private static extern void _etceteraCameraCaptureSetFrame( int x, int y, int width, int height );

		/// <summary>
		/// Sets the current frame for the camera preview. The values sent are multiplied by the screens dpi on the native side.
		/// </summary>
		public static void cameraCaptureSetFrame( int x, int y, int width, int height )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_etceteraCameraCaptureSetFrame( x, y, width, height );
		}

		#endregion



	    [DllImport("__Internal")]
	    private static extern string _etceteraGetContacts( int startIndex, long count );

	    public static List<Contact> getContacts( int startIndex, long count )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				var json = _etceteraGetContacts( startIndex, count );
				return Json.decode<List<Contact>>( json );
			}
			return new List<Contact>();
	    }

	}



	public class Contact
	{
		public string name;
		public List<string> emails;
		public List<string> phoneNumbers;
	}
}
#endif
