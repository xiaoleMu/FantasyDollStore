using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using strange.extensions.signal.impl;

namespace TabTale
{
	public interface INotificationService
	{
		/// <summary>
		/// Gets a value indicating whether this instance is initialised.
		/// </summary>
		/// <value><c>true</c> if this instance is initialised; otherwise, <c>false</c>.</value>
		bool IsInitialised { get; }
		/// <summary>
		/// Initialize this instance.
		/// </summary>
		bool Initialize ();
		/// <summary>
		/// Posts a notification at once.
		/// </summary>
		/// <param name="title">The notification title.</param>
		/// <param name="text">The notification text.</param>
		/// <param name="id">The notification id.</param>
		/// <param name="notificationProfile">(Optional) The name of the notification profile (sound & icons settings).</param>
		void PostNotification (string title, string text, int id, string notificationProfile=null);
		/// <summary>
		/// Schedules a notification.
		/// </summary>
		/// <param name="triggerDateTime">DateTime value when the notification will be shown.</param>
		/// <param name="title">The notification title.</param>
		/// <param name="text">The notification text.</param>
		/// <param name="id">The notification id.</param>
		/// <param name="notificationProfile">(Optional) The name of the notification profile (sound & icons settings).</param>
		void ScheduleNotification (DateTime triggerDateTime, string title, string text, int id, string notificationProfile=null);
		/// <summary>
		/// Schedules a repeating notification.
		/// </summary>
		/// <param name="firstTriggerDateTime">DateTime value when the notification will be shown first time.</param>
		/// <param name="intervalSeconds">Seconds between the notification shows.</param>
		/// <param name="title">The notification title.</param>
		/// <param name="text">The notification text.</param>
		/// <param name="id">The notification id.</param>
		/// <param name="notificationProfile">(Optional) The name of the notification profile (sound & icons settings).</param>
		void ScheduleNotificationRepeating (DateTime firstTriggerDateTime, int intervalSeconds, string title, string text, int id, string notificationProfile=null);
		/// <summary>
		/// checks if notificationses are enabled.
		/// </summary>
		bool NotificationsEnabled ();
		/// <summary>
		/// enable or disable notifications.
		/// </summary>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		void SetNotificationsEnabled (bool enabled);
		/// <summary>
		/// Cancels the notification with the specified id (ignored if the specified notification is not found).
		/// </summary>
		/// <param name="id">notification id</param>
		void CancelNotification (int id);
		/// <summary>
		/// Hides the notification with the specified id  (ignored if the specified notification is not found).
		/// </summary>
		/// <param name="id">notification id</param>
		void HideNotification (int id);
		/// <summary>
		/// Cancels all notifications.
		/// </summary>
		void CancelAllNotifications ();
		/// <summary>
		/// Hides all notifications.
		/// </summary>
		void HideAllNotifications ();
		/// <summary>
		/// Returns the app icon badge value (iOS only).
		/// </summary>
		/// <remarks>
		/// Will always return <c>0</c> on non-iOS platforms.
		/// </remarks>
		int GetBadge();
		/// <summary>
		/// Sets the app icon badge value (iOS only).
		/// </summary>
		/// <remarks>
		/// Will be ignored on non-iOS platforms.
		/// </remarks>
		void SetBadge(int badgeNumber);
	}

	public class NotificationData
	{
		public string title;
		public string text;
		public int id;
		public IDictionary< string, string > userData;
	}

	public class NotificationClickedSignal : Signal<NotificationData> {}
	public class NotificationsReceivedSignal : Signal<IList<NotificationData>> {}
}
