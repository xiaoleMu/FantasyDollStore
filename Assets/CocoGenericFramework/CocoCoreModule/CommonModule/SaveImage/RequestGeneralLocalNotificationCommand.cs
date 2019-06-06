using strange.extensions.signal.impl;
using strange.extensions.command.impl;
using System;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
#if UNITY_2017_2_OR_NEWER
using Prime31;
#endif

namespace CocoPlay
{
	/// <summary>
	/// Request general local notification command.  需要添加其他推送 继承 重写AddExtraNotification方法添加其他推送
	/// </summary>
	public class RequestGeneralLocalNotificationCommand : Command
	{
		protected bool IsTestMode { get; set; }

		public override void Execute ()
		{
			#if UNITY_IOS

			IsTestMode = CocoDebugSettingsData.Instance.IsNotificationTestModeEnabled;

			#if !UNITY_EDITOR
			if (NotificationServices.enabledNotificationTypes == NotificationType.None) {
				return;
			}
			#endif

			Retain ();
			ClearLocalNotifications ();
			InitLocalNotifications ();
			AddExtraNotification ();
			Release ();
			#endif
		}

		#if UNITY_IOS

		//LocalNotification m_LocalNotification;

		private void InitLocalNotifications ()
		{
			InitGeneralNotificationDatas ();
			if (m_GeneralNotificationTexts == null || m_GeneralNotificationTexts.Length <= 0) {
				return;
			}

			// regester new notification
			// fire day - next (Friday, Saturday or Sunday), 10:00 - 22:00
			DateTime fireDate = DateTime.Now.Date;
			fireDate = fireDate.AddDays ((float)(DayOfWeek.Saturday - fireDate.DayOfWeek)
			+ UnityEngine.Random.Range (-1, 2) + UnityEngine.Random.Range (10.0f, 22.0f) / 24.0f);


			if (IsTestMode) {
				fireDate = DateTime.Now.Date;
				fireDate = fireDate.AddDays (DayOfWeek.Saturday - fireDate.DayOfWeek + 0.5f);
			}

			if (fireDate <= DateTime.Now) {
				fireDate = fireDate.AddDays (7);
			}

			if (IsTestMode) {
				UnityEngine.Debug.Log (string.Format ("下次推送时间:{0}年{1}月{2}日{3}时",
					fireDate.Year, fireDate.Month, fireDate.Day, fireDate.Hour));
			}

			int textIndex = UnityEngine.Random.Range (0, m_GeneralNotificationTexts.Length);
			string alertBodyKey = m_GeneralNotificationTexts [textIndex];
			if (IsTestMode) {
				UnityEngine.Debug.LogErrorFormat ("{0}->InitLocalNotifications: index: {1}], text: {2}", GetType ().Name, textIndex, alertBodyKey);
			}

			ScheduleLocalNotification (Localization.CocoLocalization.Get (alertBodyKey), fireDate, CalendarUnit.Week);
		}

		protected void ScheduleLocalNotification (string pAlertBody, DateTime pFireDate, CalendarUnit pUnit)
		{
			LocalNotification localNotification = new LocalNotification ();
			localNotification.fireDate = pFireDate;
			localNotification.alertBody = pAlertBody;
			localNotification.applicationIconBadgeNumber = 1;
			localNotification.hasAction = true;
			localNotification.repeatCalendar = CalendarIdentifier.GregorianCalendar;
			localNotification.repeatInterval = pUnit;
			NotificationServices.ScheduleLocalNotification (localNotification);

			if (IsTestMode) {
				UnityEngine.Debug.LogErrorFormat ("{0}->ScheduleLocalNotification: pAlertBody: {1}], pFireDate: {2}, pUnit: {3}", GetType ().Name, pAlertBody, pFireDate, pUnit);
			}
		}

		protected void ClearLocalNotifications ()
		{
			LocalNotification localNotification = new LocalNotification ();
			localNotification.applicationIconBadgeNumber = -1;
			NotificationServices.PresentLocalNotificationNow (localNotification);
			NotificationServices.CancelAllLocalNotifications ();
			NotificationServices.ClearLocalNotifications ();
			EtceteraBinding.setBadgeCount (0);
		}

		protected virtual void AddExtraNotification ()
		{
		}

		protected string[] m_GeneralNotificationTexts;

		protected virtual void InitGeneralNotificationDatas ()
		{
			m_GeneralNotificationTexts = new[] {
				"txt_notification_hello_weekly_1",
				"txt_notification_hello_weekly_2",
				"txt_notification_hello_weekly_3",
				"txt_notification_hello_weekly_4",
				"txt_notification_hello_weekly_5",
				"txt_notification_hello_weekly_6",
			};
		}

		#endif
	}

	public class RequestGeneralLocalNotificationSignal : Signal
	{

	}
}

