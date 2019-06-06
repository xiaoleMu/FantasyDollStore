using UnityEngine;
using System.Collections;



namespace Prime31
{
	public class EtceteraEventListener : MonoBehaviour
	{
#if UNITY_IOS
		void OnEnable()
		{
			// Listen to all events for illustration purposes
			EtceteraManager.dismissingViewControllerEvent += dismissingViewControllerEvent;
			EtceteraManager.imagePickerCancelledEvent += imagePickerCancelled;
			EtceteraManager.imagePickerChoseImageEvent += imagePickerChoseImage;
			EtceteraManager.saveImageToPhotoAlbumSucceededEvent += saveImageToPhotoAlbumSucceededEvent;
			EtceteraManager.saveImageToPhotoAlbumFailedEvent += saveImageToPhotoAlbumFailedEvent;
			EtceteraManager.saveVideoToPhotoAlbumSucceededEvent += saveVideoToPhotoAlbumSucceededEvent;
			EtceteraManager.saveVideoToPhotoAlbumFailedEvent += saveVideoToPhotoAlbumFailedEvent;
			EtceteraManager.alertButtonClickedEvent += alertButtonClicked;
			
			EtceteraManager.promptCancelledEvent += promptCancelled;
			EtceteraManager.singleFieldPromptTextEnteredEvent += singleFieldPromptTextEntered;
			EtceteraManager.twoFieldPromptTextEnteredEvent += twoFieldPromptTextEntered;
			
			EtceteraManager.remoteRegistrationSucceededEvent += remoteRegistrationSucceeded;
			EtceteraManager.remoteRegistrationFailedEvent += remoteRegistrationFailed;
			EtceteraManager.pushIORegistrationCompletedEvent += pushIORegistrationCompletedEvent;
			EtceteraManager.urbanAirshipRegistrationSucceededEvent += urbanAirshipRegistrationSucceeded;
			EtceteraManager.urbanAirshipRegistrationFailedEvent += urbanAirshipRegistrationFailed;
			EtceteraManager.remoteNotificationReceivedEvent += remoteNotificationReceived;
			EtceteraManager.remoteNotificationReceivedAtLaunchEvent += remoteNotificationReceivedAtLaunch;
			
			EtceteraManager.localNotificationWasReceivedAtLaunchEvent += localNotificationWasReceivedAtLaunchEvent;
			EtceteraManager.localNotificationWasReceivedEvent += localNotificationWasReceivedEvent;
			
			EtceteraManager.mailComposerFinishedEvent += mailComposerFinished;
			EtceteraManager.smsComposerFinishedEvent += smsComposerFinished;
			EtceteraManager.webViewDidLoadURLEvent += webViewDidLoadURLEvent;

			EtceteraManager.videoRecordingSucceededEvent += videoRecordingSucceededEvent;
			EtceteraManager.videoRecordingFailedEvent += videoRecordingFailedEvent;
		}
		
		
		void OnDisable()
		{
			// Remove all event handlers
			EtceteraManager.dismissingViewControllerEvent -= dismissingViewControllerEvent;
			EtceteraManager.imagePickerCancelledEvent -= imagePickerCancelled;
			EtceteraManager.imagePickerChoseImageEvent -= imagePickerChoseImage;
			EtceteraManager.saveImageToPhotoAlbumSucceededEvent -= saveImageToPhotoAlbumSucceededEvent;
			EtceteraManager.saveImageToPhotoAlbumFailedEvent -= saveImageToPhotoAlbumFailedEvent;
			EtceteraManager.saveVideoToPhotoAlbumSucceededEvent -= saveVideoToPhotoAlbumSucceededEvent;
			EtceteraManager.saveVideoToPhotoAlbumFailedEvent -= saveVideoToPhotoAlbumFailedEvent;
			EtceteraManager.alertButtonClickedEvent -= alertButtonClicked;
			
			EtceteraManager.promptCancelledEvent -= promptCancelled;
			EtceteraManager.singleFieldPromptTextEnteredEvent -= singleFieldPromptTextEntered;
			EtceteraManager.twoFieldPromptTextEnteredEvent -= twoFieldPromptTextEntered;
			
			EtceteraManager.remoteRegistrationSucceededEvent -= remoteRegistrationSucceeded;
			EtceteraManager.remoteRegistrationFailedEvent -= remoteRegistrationFailed;
			EtceteraManager.pushIORegistrationCompletedEvent -= pushIORegistrationCompletedEvent;
			EtceteraManager.urbanAirshipRegistrationSucceededEvent -= urbanAirshipRegistrationSucceeded;
			EtceteraManager.urbanAirshipRegistrationFailedEvent -= urbanAirshipRegistrationFailed;
			EtceteraManager.remoteNotificationReceivedAtLaunchEvent -= remoteNotificationReceivedAtLaunch;
			
			EtceteraManager.localNotificationWasReceivedAtLaunchEvent -= localNotificationWasReceivedAtLaunchEvent;
			EtceteraManager.localNotificationWasReceivedEvent -= localNotificationWasReceivedEvent;
			
			EtceteraManager.mailComposerFinishedEvent -= mailComposerFinished;
			EtceteraManager.smsComposerFinishedEvent -= smsComposerFinished;
			EtceteraManager.webViewDidLoadURLEvent -= webViewDidLoadURLEvent;

			EtceteraManager.videoRecordingSucceededEvent -= videoRecordingSucceededEvent;
			EtceteraManager.videoRecordingFailedEvent -= videoRecordingFailedEvent;
		}
		
		
		void dismissingViewControllerEvent()
		{
			Debug.Log( "dismissingViewControllerEvent" );
		}
		
		
		void imagePickerCancelled()
		{
			Debug.Log( "imagePickerCancelled" );
		}
		
	
		void imagePickerChoseImage( string imagePath )
		{
			Debug.Log( "image picker chose image: " + imagePath );
		}
		
		
		void saveImageToPhotoAlbumSucceededEvent()
		{
			Debug.Log( "saveImageToPhotoAlbumSucceededEvent" );
		}
	
	
		void saveImageToPhotoAlbumFailedEvent( string error )
		{
			Debug.Log( "saveImageToPhotoAlbumFailedEvent: " + error );
		}


		void saveVideoToPhotoAlbumSucceededEvent()
		{
			Debug.Log( "saveVideoToPhotoAlbumSucceededEvent" );
		}
	
	
		void saveVideoToPhotoAlbumFailedEvent( string error )
		{
			Debug.Log( "saveVideoToPhotoAlbumFailedEvent: " + error );
		}
		
		
		void alertButtonClicked( string text )
		{
			Debug.Log( "alert button clicked: " + text );
		}
		
		
		void promptCancelled()
		{
			Debug.Log( "promptCancelled" );
		}
		
		
		void singleFieldPromptTextEntered( string text )
		{
			Debug.Log( "field : " + text );
		}
		
		
		void twoFieldPromptTextEntered( string textOne, string textTwo )
		{
			Debug.Log( "field one: " + textOne + ", field two: " + textTwo );
		}
		
		
		void remoteRegistrationSucceeded( string deviceToken )
		{
			Debug.Log( "remoteRegistrationSucceeded with deviceToken: " + deviceToken );
		}
		
		
		void remoteRegistrationFailed( string error )
		{
			Debug.Log( "remoteRegistrationFailed : " + error );
		}
		
		
		void pushIORegistrationCompletedEvent( string error )
		{
			if( error != null )
				Debug.Log( "pushIORegistrationCompletedEvent failed with error: " + error );
			else
				Debug.Log( "pushIORegistrationCompletedEvent successful" );
		}
		
		
		void urbanAirshipRegistrationSucceeded()
		{
			Debug.Log( "urbanAirshipRegistrationSucceeded" );
		}
		
		
		void urbanAirshipRegistrationFailed( string error )
		{
			Debug.Log( "urbanAirshipRegistrationFailed : " + error );
		}
		
		
		void remoteNotificationReceived( IDictionary notification )
		{
			Debug.Log( "remoteNotificationReceived" );
			Prime31.Utils.logObject( notification );
		}
		
		
		void remoteNotificationReceivedAtLaunch( IDictionary notification )
		{
			Debug.Log( "remoteNotificationReceivedAtLaunch" );
			Prime31.Utils.logObject( notification );
		}
		
		
		void localNotificationWasReceivedEvent( IDictionary notification )
		{
			Debug.Log( "localNotificationWasReceivedEvent" );
			Prime31.Utils.logObject( notification );
		}
		
		
		void localNotificationWasReceivedAtLaunchEvent( IDictionary notification )
		{
			Debug.Log( "localNotificationWasReceivedAtLaunchEvent" );
			Prime31.Utils.logObject( notification );
		}
	
		
		void mailComposerFinished( string result )
		{
			Debug.Log( "mailComposerFinished: " + result );
		}
		
		
		void smsComposerFinished( string result )
		{
			Debug.Log( "smsComposerFinished: " + result );
		}


		void webViewDidLoadURLEvent( string url )
		{
			Debug.Log( "webViewDidLoadURLEvent: " + url );
		}


		void videoRecordingSucceededEvent( string path )
		{
			Debug.Log( "videoRecordingSucceededEvent: " + path );
		}


		void videoRecordingFailedEvent( string error )
		{
			Debug.Log( "videoRecordingFailedEvent: " + error );
		}
#endif
	}

}
