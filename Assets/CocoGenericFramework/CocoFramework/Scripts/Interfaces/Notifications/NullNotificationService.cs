using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class NullNotificationService : NullService, INotificationService
	{
		private const bool IS_INITIALIZED = true;

		private bool notificationsEnabled = true;

		public bool IsInitialised {
			get {
				return IS_INITIALIZED;
			}
		}

		#region INotificationService implementation

		public bool Initialize ()
		{
			Debug.Log ("Init Notification Service");
			return IS_INITIALIZED;
		}

		public void PostNotification (string title, string text, int id, string notificationProfile = null)
		{
			Debug.Log ("post Notification");
		}

		public void ScheduleNotification (System.DateTime triggerDateTime, string title, string text, int id, string notificationProfile = null)
		{
			Debug.Log ("schedule Notification");
		}

		public void ScheduleNotificationRepeating (System.DateTime firstTriggerDateTime, int intervalSeconds, string title, string text, int id, string notificationProfile = null)
		{
			Debug.Log ("schedule repeating Notification");
		}

		public bool NotificationsEnabled ()
		{
			return notificationsEnabled;
		}

		public void SetNotificationsEnabled (bool enabled)
		{
			notificationsEnabled = enabled;
		}

		public void CancelNotification (int id)
		{
			Debug.Log ("cancel Notification id: " + id);
		}

		public void HideNotification (int id)
		{
			Debug.Log ("hide Notification id: " + id);
		}

		public void CancelAllNotifications ()
		{
			Debug.Log ("cancel all Notifications");
		}

		public void HideAllNotifications ()
		{
			Debug.Log ("hide all Notifications");
		}

		public int GetBadge ()
		{
			return 0;
		}
		
		public void SetBadge (int badgeNumber)
		{

		}
		#endregion


	}
}
