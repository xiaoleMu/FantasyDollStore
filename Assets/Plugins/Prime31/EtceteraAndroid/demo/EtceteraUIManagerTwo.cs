using UnityEngine;
using System.Collections.Generic;
using Prime31;



namespace Prime31
{
	public class EtceteraUIManagerTwo : MonoBehaviourGUI
	{
#if UNITY_ANDROID
		int _fiveSecondNotificationId;


		void OnGUI()
		{
			beginColumn();


			if( GUILayout.Button( "Show Inline Web View" ) )
			{
				EtceteraAndroid.inlineWebViewShow( "http://prime31.com/", 160, 430, Screen.width - 160, Screen.height - 100 );
			}


			if( GUILayout.Button( "Close Inline Web View" ) )
			{
				EtceteraAndroid.inlineWebViewClose();
			}


			if( GUILayout.Button( "Set Url of Inline Web View" ) )
			{
				EtceteraAndroid.inlineWebViewSetUrl( "http://google.com" );
			}


			if( GUILayout.Button( "Set Frame of Inline Web View" ) )
			{
				EtceteraAndroid.inlineWebViewSetFrame( 80, 50, 300, 400 );
			}


			if( GUILayout.Button( "Get First 25 Contacts" ) )
			{
				EtceteraAndroid.loadContacts( 0, 25 );
			}


			GUILayout.Label( "Request M Permissions" );

			if( GUILayout.Button( "Request Permission" ) )
			{
				EtceteraAndroid.requestPermissions( new string[] { "android.permission.READ_PHONE_STATE" } );
			}


			if( GUILayout.Button( "Should Show Permission Rationale" ) )
			{
				var shouldShow = EtceteraAndroid.shouldShowRequestPermissionRationale( "android.permission.READ_PHONE_STATE" );
				Debug.Log( "shouldShowRequestPermissionRationale: " + shouldShow );
			}


			if( GUILayout.Button( "Check Permission" ) )
			{
				var shouldShow = EtceteraAndroid.checkSelfPermission( "android.permission.READ_PHONE_STATE" );
				Debug.Log( "checkSelfPermission: " + shouldShow );
			}


			endColumn( true );


			if( toggleButtonState( "Camera Capture" ) )
				notificationsUI();
			else
				cameraCaptureUI();

			GUILayout.Space( 30 );
			toggleButton( "Camera Capture", "Notifications" );

			endColumn();


			if( bottomRightButton( "Previous Scene" ) )
			{
				loadLevel( "EtceteraTestScene" );
			}
		}


		void notificationsUI()
		{
			GUILayout.Label( "Notifications" );
			if( GUILayout.Button( "Schedule Notification in 5s" ) )
			{
				var noteConfig = new AndroidNotificationConfiguration( 5, "Notification Title - 5 Seconds", "The subtitle of the notification", "Ticker text gets ticked" )
				{
					extraData = "five-second-note",
					groupKey = "my-note-group"
				};

				// turn off sound and vibration for this notification
				noteConfig.sound = false;
				noteConfig.vibrate = false;

				_fiveSecondNotificationId = EtceteraAndroid.scheduleNotification( noteConfig );
				Debug.Log( "notificationId: " + _fiveSecondNotificationId );
			}


			if( GUILayout.Button( "Schedule Group Summary Notification in 5s" ) )
			{
				var noteConfig = new AndroidNotificationConfiguration( 5, "Group Summary Title", "Group Summary Subtitle - Stuff Happened", "Ticker text" )
				{
					extraData = "group-summary-note",
					groupKey = "my-note-group",
					isGroupSummary = true
				};
				EtceteraAndroid.scheduleNotification( noteConfig );
			}


			if( GUILayout.Button( "Cancel 5s Notification" ) )
			{
				EtceteraAndroid.cancelNotification( _fiveSecondNotificationId );
			}


			if( GUILayout.Button( "Check for Notifications" ) )
			{
				EtceteraAndroid.checkForNotifications();
			}


			if( GUILayout.Button( "Cancel All Notifications" ) )
			{
				EtceteraAndroid.cancelAllNotifications();
			}
		}


		void cameraCaptureUI()
		{
			GUILayout.Label( "Camera Capture" );
			if( GUILayout.Button( "Start Camera Capture" ) )
			{
				EtceteraAndroid.startCameraCapture( false, 160, 100, Screen.width - 180, Screen.height - 120 );
			}


			if( GUILayout.Button( "Stop Camera Capture" ) )
			{
				EtceteraAndroid.stopCameraCapture();
			}


			if( GUILayout.Button( "Set Frame of Camera Preview" ) )
			{
				EtceteraAndroid.cameraCaptureSetFrame( 40, 40, 300, 200 );
			}


			if( GUILayout.Button( "Set Frame of Camera Preview" ) )
			{
				EtceteraAndroid.cameraCaptureSetFrame( 300, 300, 800, 800 );
			}
		}

#endif
	}

}
