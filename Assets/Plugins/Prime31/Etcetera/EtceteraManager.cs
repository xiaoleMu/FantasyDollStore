using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Prime31;



namespace Prime31
{
	public class EtceteraManager : AbstractManager
	{
#if UNITY_IOS
		/// <summary>
		/// Fired whenever any full screen view controller is dismissed
		/// </summary>
		public static event Action dismissingViewControllerEvent;

		/// <summary>
		/// Fired when the user cancels the image picker
		/// </summary>
		public static event Action imagePickerCancelledEvent;

		/// <summary>
		/// Fired when the user selects or takes a photo
		/// </summary>
		public static event Action<string> imagePickerChoseImageEvent;

		/// <summary>
		/// Fired when an image is saved to the album
		/// </summary>
		public static event Action saveImageToPhotoAlbumSucceededEvent;

		/// <summary>
		/// Fired when an image fails to be saved to the album
		/// </summary>
		public static event Action<string> saveImageToPhotoAlbumFailedEvent;

		/// <summary>
		/// Fired when a video is saved to the album
		/// </summary>
		public static event Action saveVideoToPhotoAlbumSucceededEvent;

		/// <summary>
		/// Fired when a video fails to be saved to the album
		/// </summary>
		public static event Action<string> saveVideoToPhotoAlbumFailedEvent;

		/// <summary>
		/// Fired when the user touches a button on the alert view
		/// </summary>
		public static event Action<string> alertButtonClickedEvent;

		/// <summary>
		/// Fired when the user touches the cancel button on a prompt
		/// </summary>
		public static event Action promptCancelledEvent;

		/// <summary>
		/// Fired when the user finishes entering text in the prompt
		/// </summary>
		public static event Action<string> singleFieldPromptTextEnteredEvent;

		/// <summary>
		/// Fired when the user finishes entering text in a two field prompt
		/// </summary>
		public static event Action<string, string> twoFieldPromptTextEnteredEvent;

		/// <summary>
		/// Fired when remote notifications are successfully registered for
		/// </summary>
		public static event Action<string> remoteRegistrationSucceededEvent;

		/// <summary>
		/// Fired when remote notification registration fails
		/// </summary>
		public static event Action<string> remoteRegistrationFailedEvent;

		/// <summary>
		/// Fired when Urban Airship registration succeeds
		/// </summary>
		public static event Action urbanAirshipRegistrationSucceededEvent;

		/// <summary>
		/// Fired when Urban Airship registration fails
		/// </summary>
		public static event Action<string> urbanAirshipRegistrationFailedEvent;

		/// <summary>
		/// Fired when Push.IO registration completes. If the parameter is null then there was no error. Non-null will contain an error message
		/// </summary>
		public static event Action<string> pushIORegistrationCompletedEvent;

		/// <summary>
		/// Fired when a remote notification is received
		/// </summary>
		public static event Action<IDictionary> remoteNotificationReceivedEvent;

		/// <summary>
		/// Fired when a remote notification launched your game
		/// </summary>
		public static event Action<IDictionary> remoteNotificationReceivedAtLaunchEvent;

		/// <summary>
		/// Fired when a local notification is received
		/// </summary>
		public static event Action<IDictionary> localNotificationWasReceivedEvent;

		/// <summary>
		/// Fired when a local notification is received at launch
		/// </summary>
		public static event Action<IDictionary> localNotificationWasReceivedAtLaunchEvent;

		/// <summary>
		/// Fired when the mail composer is dismissed
		/// </summary>
		public static event Action<string> mailComposerFinishedEvent;

		/// <summary>
		/// Fired when the SMS composer is dismissed
		/// </summary>
		public static event Action<string> smsComposerFinishedEvent;

		/// <summary>
		/// Fired when the web view finishes loading a page. Note that the SafariViewController offers no access to its state.
		/// </summary>
		public static event Action<string> webViewDidLoadURLEvent;

		/// <summary>
		/// Fired when a video is successfully taken and returns the full path to the video
		/// </summary>
		public static event Action<string> videoRecordingSucceededEvent;

		/// <summary>
		/// Fired when the user cancels the video recording operation or an error occurs
		/// </summary>
		public static event Action<string> videoRecordingFailedEvent;


		public static string deviceToken { get; private set; }
		public static string pushIOApiKey;
		public static string[] pushIOCategories;


	    static EtceteraManager()
	    {
			AbstractManager.initialize( typeof( EtceteraManager ) );
			#pragma warning disable
			var uselessVar = typeof( UnityEngine.iOS.RemoteNotification );
			#pragma warning restore
	    }


		void dismissingViewController()
		{
			if( dismissingViewControllerEvent != null )
				dismissingViewControllerEvent();
		}


		#region Image picker

		void imagePickerDidCancel( string empty )
		{
			if( imagePickerCancelledEvent != null )
				imagePickerCancelledEvent();
		}


		void imageSavedToDocuments( string filePath )
		{
			if( imagePickerChoseImageEvent != null )
				imagePickerChoseImageEvent( filePath );
		}


		void saveImageToPhotoAlbumFailed( string error )
		{
			saveImageToPhotoAlbumFailedEvent.fire( error );
		}


		void saveImageToPhotoAlbumSucceeded( string empty )
		{
			saveImageToPhotoAlbumSucceededEvent.fire();
		}


		void saveVideoToPhotoAlbum( string errorOrEmpty )
		{
			if( string.IsNullOrEmpty( errorOrEmpty ) )
				saveVideoToPhotoAlbumSucceededEvent.fire();
			else
				saveVideoToPhotoAlbumFailedEvent.fire( errorOrEmpty );
		}


		/// <summary>
		/// Loads up a Texture2D with the image at the given path
		/// </summary>
		public static IEnumerator textureFromFileAtPath( string filePath, Action<Texture2D> del, Action<string> errorDel )
		{
			using( WWW www = new WWW( filePath ) )
			{
				yield return www;

				if( !string.IsNullOrEmpty( www.error ) )
				{
					if( errorDel != null )
						errorDel( www.error );
				}

				// Assign the texture to a local variable to avoid leaking it (Unity bug)
				Texture2D tex = www.texture;

				if( tex != null )
					del( tex );
				else
					errorDel( "www.texture was null. Texture not loaded" );
			}
		}

		#endregion;


		#region Alert and Prompt

		void alertViewClickedButton( string buttonTitle )
		{
			if( alertButtonClickedEvent != null )
				alertButtonClickedEvent( buttonTitle );
		}


		void alertPromptCancelled( string empty )
		{
			if( promptCancelledEvent != null )
				promptCancelledEvent();
		}


		void alertPromptEnteredText( string text )
		{
			// Was this one prompt or 2?
			string[] promptText = text.Split( new string[] {"|||"}, StringSplitOptions.None );
			if( promptText.Length == 1 )
			{
				if( singleFieldPromptTextEnteredEvent != null )
					singleFieldPromptTextEnteredEvent( promptText[0] );
			}

			if( promptText.Length == 2 )
			{
				if( twoFieldPromptTextEnteredEvent != null )
					twoFieldPromptTextEnteredEvent( promptText[0], promptText[1] );
			}
		}

		#endregion;


		#region Remote Notifications

		void remoteRegistrationDidSucceed( string deviceToken )
		{
			EtceteraManager.deviceToken = deviceToken;
			if( remoteRegistrationSucceededEvent != null )
				remoteRegistrationSucceededEvent( deviceToken );

			// if we have Push.IO data perform registration
			if( pushIOApiKey != null )
				StartCoroutine( registerDeviceWithPushIO() );
		}


		private IEnumerator registerDeviceWithPushIO()
		{
			var url = string.Format( "https://api.push.io/r/{0}?di={1}&dt={2}", pushIOApiKey, SystemInfo.deviceUniqueIdentifier, deviceToken );

			// add categories if we have them
			if( pushIOCategories != null && pushIOCategories.Length > 0 )
				url += "&c=" + string.Join( ",", pushIOCategories );

			using( var www = new WWW( url ) )
			{
				yield return www;

				if( pushIORegistrationCompletedEvent != null )
					pushIORegistrationCompletedEvent( www.error );
			}
		}


		void remoteRegistrationDidFail( string error )
		{
			if( remoteRegistrationFailedEvent != null )
				remoteRegistrationFailedEvent( error );
		}


		void urbanAirshipRegistrationDidSucceed( string empty )
		{
			urbanAirshipRegistrationSucceededEvent.fire();
		}


		void urbanAirshipRegistrationDidFail( string error )
		{
			urbanAirshipRegistrationFailedEvent.fire( error );
		}


		void remoteNotificationWasReceived( string json )
		{
			remoteNotificationReceivedEvent.fire( json.dictionaryFromJson() );
		}


		void remoteNotificationWasReceivedAtLaunch( string json )
		{
			remoteNotificationReceivedAtLaunchEvent.fire( json.dictionaryFromJson() );
		}


		void localNotificationWasReceived( string json )
		{
			localNotificationWasReceivedEvent.fire( json.dictionaryFromJson() );
		}


		void localNotificationWasReceivedAtLaunch( string json )
		{
			localNotificationWasReceivedAtLaunchEvent.fire( json.dictionaryFromJson() );
		}

		#endregion;


		void mailComposerFinishedWithResult( string result )
		{
			mailComposerFinishedEvent.fire( result );
		}


		void smsComposerFinishedWithResult( string result )
		{
			smsComposerFinishedEvent.fire( result );
		}


		void webViewDidLoadURL( string url )
		{
			webViewDidLoadURLEvent.fire( url );
		}


		void videoRecordingSucceeded( string path )
		{
			videoRecordingSucceededEvent.fire( path );
		}


		void videoRecordingFailed( string error )
		{
			videoRecordingFailedEvent.fire( error );
		}

#endif
	}

}
