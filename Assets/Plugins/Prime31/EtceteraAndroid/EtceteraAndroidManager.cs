using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;



namespace Prime31
{
	public class EtceteraAndroidManager : AbstractManager
	{
#if UNITY_ANDROID
		/// <summary>
		/// Fired when an alert button is clicked and returns the text from the button
		/// </summary>
		public static event Action<string> alertButtonClickedEvent;

		/// <summary>
		/// Fired when the user presses the back button to avoid the alert
		/// </summary>
		public static event Action alertCancelledEvent;

		/// <summary>
		/// Fired when a prompt finishes with the text the user entered
		/// </summary>
		public static event Action<string> promptFinishedWithTextEvent;

		/// <summary>
		/// Fired when a prompt is cancelled either with back button or the negative button
		/// </summary>
		public static event Action promptCancelledEvent;

		/// <summary>
		/// Fired when a prompt finishes with the text the user entered
		/// </summary>
		public static event Action<string, string> twoFieldPromptFinishedWithTextEvent;

		/// <summary>
		/// Fired when a prompt is cancelled either with back button or the negative button
		/// </summary>
		public static event Action twoFieldPromptCancelledEvent;

		/// <summary>
		/// Fired when the user presses the back button while viewing a web page
		/// </summary>
		public static event Action webViewCancelledEvent;

		/// <summary>
		/// Fired when the user cancels selection of an image from the photo album
		/// </summary>
		public static event Action albumChooserCancelledEvent;

		/// <summary>
		/// Fired when a user chooses an image. Returns the full path to the image.
		/// </summary>
		public static event Action<string> albumChooserSucceededEvent;

		/// <summary>
		/// Fired when a user cancels the camera app without taking a picture
		/// </summary>
		public static event Action photoChooserCancelledEvent;

		/// <summary>
		/// Fired when a photo is taken. Returns the full path to the image.
		/// </summary>
		public static event Action<string> photoChooserSucceededEvent;

		/// <summary>
		/// Fired when a video is successfully taken and returns the full path to the video
		/// </summary>
		public static event Action<string> videoRecordingSucceededEvent;

		/// <summary>
		/// Fired when the user cancels the video recording operation
		/// </summary>
		public static event Action videoRecordingCancelledEvent;

		/// <summary>
		/// Fired when the text to speech system is ready for use
		/// </summary>
		public static event Action ttsInitializedEvent;

		/// <summary>
		/// Fired when the text to speech system fails to initialize
		/// </summary>
		public static event Action ttsFailedToInitializeEvent;

		/// <summary>
		/// Fired when the user chooses to review your app
		/// </summary>
		public static event Action askForReviewWillOpenMarketEvent;

		/// <summary>
		/// Fired when the remind me later button is pressed when asking for a review
		/// </summary>
		public static event Action askForReviewRemindMeLaterEvent;

		/// <summary>
		/// Fired when the dont ask me again button is pressed when asking for a review
		/// </summary>
		public static event Action askForReviewDontAskAgainEvent;

		/// <summary>
		/// Fired when the loaded JavaScript in an inline web view calls UnityBridge.sendMessage(). Note that the Android javascript interface
		/// has many open bugs and this will not work on all device/OS combinations!
		/// </summary>
		public static event Action<string> inlineWebViewJSCallbackEvent;

		/// <summary>
		/// Fired when a notification is received or after calling checkForNotifications and the app was launched from a notification
		/// </summary>
		public static event Action<string> notificationReceivedEvent;

		/// <summary>
		/// Fired when contacts have been loaded successfully
		/// </summary>
		public static event Action<List<EtceteraAndroid.Contact>> contactsLoadedEvent;

		/// <summary>
		/// Fired in response to calling requestPermissions. Includes the requestCode and whether the user granted/denied each permission.
		/// </summary>
		public static event Action<PermissionsResult> onRequestPermissionsResultEvent;

		/// <summary>
		/// Fired when the inline web view finishes loading a page
		/// </summary>
		public static event Action<string> webViewDidLoadURLEvent;


		static EtceteraAndroidManager()
		{
			AbstractManager.initialize( typeof( EtceteraAndroidManager ) );
		}


		public void alertButtonClicked( string positiveButton )
		{
			if( alertButtonClickedEvent != null )
				alertButtonClickedEvent( positiveButton );
		}


		public void alertCancelled( string empty )
		{
			if( alertCancelledEvent != null )
				alertCancelledEvent();
		}


		// handles single and two field prompts
		public void promptFinishedWithText( string text )
		{
			// Was this one prompt or 2?
			string[] promptText = text.Split( new string[] {"|||"}, StringSplitOptions.None );

			if( promptText.Length == 1 )
			{
				if( promptFinishedWithTextEvent != null )
					promptFinishedWithTextEvent( promptText[0] );
			}

			if( promptText.Length == 2 )
			{
				if( twoFieldPromptFinishedWithTextEvent != null )
					twoFieldPromptFinishedWithTextEvent( promptText[0], promptText[1] );
			}
		}


		public void promptCancelled( string empty )
		{
			if( promptCancelledEvent != null )
				promptCancelledEvent();
		}


		public void twoFieldPromptCancelled( string empty )
		{
			if( twoFieldPromptCancelledEvent != null )
				twoFieldPromptCancelledEvent();
		}


		public void webViewCancelled( string empty )
		{
			if( webViewCancelledEvent != null )
				webViewCancelledEvent();
		}


		public void albumChooserCancelled( string empty )
		{
			if( albumChooserCancelledEvent != null )
				albumChooserCancelledEvent();
		}


		public void albumChooserSucceeded( string path )
		{
			if( albumChooserSucceededEvent != null )
			{
				// make sure the file exists before proceeding to load it
				if( System.IO.File.Exists( path ) )
					albumChooserSucceededEvent( path );
				else if( albumChooserCancelledEvent != null )
					albumChooserCancelledEvent();
			}
		}


		public void photoChooserCancelled( string empty )
		{
			if( photoChooserCancelledEvent != null )
				photoChooserCancelledEvent();
		}


		public void photoChooserSucceeded( string path )
		{
			if( photoChooserSucceededEvent != null )
			{
				// make sur the file exists before proceeding to load it
				if( System.IO.File.Exists( path ) )
					photoChooserSucceededEvent( path );
				else if( photoChooserCancelledEvent != null )
					photoChooserCancelledEvent();
			}
		}


		public void videoRecordingSucceeded( string path )
		{
			if( videoRecordingSucceededEvent != null )
				videoRecordingSucceededEvent( path );
		}


		public void videoRecordingCancelled( string empty )
		{
			if( videoRecordingCancelledEvent != null )
				videoRecordingCancelledEvent();
		}


		public void ttsInitialized( string result )
		{
			var res = result == "1";

			if( res && ttsInitializedEvent != null )
				ttsInitializedEvent();

			if( !res && ttsFailedToInitializeEvent != null )
				ttsFailedToInitializeEvent();
		}


		public void ttsUtteranceCompleted( string utteranceId )
		{
			Debug.Log( "utterance completed: " + utteranceId );
		}


		// Ask for review
		public void askForReviewWillOpenMarket( string empty )
		{
			if( askForReviewWillOpenMarketEvent != null )
				askForReviewWillOpenMarketEvent();
		}


		public void askForReviewRemindMeLater( string empty )
		{
			if( askForReviewRemindMeLaterEvent != null )
				askForReviewRemindMeLaterEvent();
		}


		public void askForReviewDontAskAgain( string empty )
		{
			if( askForReviewDontAskAgainEvent != null )
				askForReviewDontAskAgainEvent();
		}


		// Inline web view
		public void inlineWebViewJSCallback( string message )
		{
			inlineWebViewJSCallbackEvent.fire( message );
		}


		// Notifications
		public void notificationReceived( string extraData )
		{
			notificationReceivedEvent.fire( extraData );
		}


		void contactsLoaded( string json )
		{
			if( contactsLoadedEvent != null )
			{
				var list = Json.decode<List<EtceteraAndroid.Contact>>( json );
				contactsLoadedEvent( list );
			}
		}


		void onRequestPermissionsResult( string json )
		{
			if( onRequestPermissionsResultEvent != null )
			{
				var result = Json.decode<EtceteraAndroidManager.PermissionsResult>( json );
				onRequestPermissionsResultEvent( result );
			}
		}


		void webViewDidLoadURL( string url )
		{
			webViewDidLoadURLEvent.fire( url );
		}


		public class PermissionsResult
		{
			public int requestCode;
			public string[] permissions;
			public bool[] grantResults;
		}

#endif
	}

}
