using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale {

	#if UNITY_IPHONE
	public class UnityNotifications : MonoBehaviour, ILocalNotificationServices
	{
		#region ILocalNotificationServices implementation

		private class Notification : ILocalGameNotifiation
		{
			private UnityEngine.iOS.LocalNotification _localNotification;

			public Notification(UnityEngine.iOS.LocalNotification localNotification)
			{
				_localNotification = localNotification;
			}

			#region ILocalGameNotifiation implementation

			public string Title
			{
				get { return _localNotification.alertAction; }
			}

			public string Body
			{
				get { return _localNotification.alertBody; }
			}

			public string Image
			{
				get { return _localNotification.alertLaunchImage; }
			}

			public System.DateTime Time
			{
				get { return _localNotification.fireDate; }
			}

			#endregion
		}

		private IDictionary<UnityEngine.iOS.LocalNotification, Notification> _notifications = new Dictionary<UnityEngine.iOS.LocalNotification, Notification>();

		public ILocalGameNotifiation Schedule (string title, string body, string image, System.DateTime time)
		{
			UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification()
			{
				alertAction = title, 
				alertBody = body, 
				fireDate = time, 
				alertLaunchImage = image
			};
			Notification notification = new Notification(localNotification);
			_notifications[localNotification] = notification;
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);

			return notification;
		}

		private void Sync()
		{
			IDictionary<UnityEngine.iOS.LocalNotification, Notification> notifications = new Dictionary<UnityEngine.iOS.LocalNotification, Notification>();
			foreach(UnityEngine.iOS.LocalNotification ln in UnityEngine.iOS.NotificationServices.localNotifications)
			{
				notifications[ln] = _notifications[ln];
			}

			_notifications = notifications;
        }

		public System.Collections.Generic.IEnumerable<ILocalGameNotifiation> Pending 
		{
			get 
			{
				Sync ();
				return _notifications.Values.Cast<ILocalGameNotifiation>();
			}
		}

		#endregion

		#region IService implementation
		
		public ITask GetInitializer(IServiceResolver resolver)
		{
			return resolver.TaskFactory.FromEnumerableAction(Init);
		}
		
		IEnumerator Init()
		{
			yield break;
		}
		
		#endregion
	}
#else
	public class UnityNotifications : MonoBehaviour, ILocalNotificationServices
	{
		#region ILocalNotificationServices implementation
		public ILocalGameNotifiation Schedule (string title, string body, string image, System.DateTime time)
		{
			return _null.Schedule(title, body, image, time);
		}

		public IEnumerable<ILocalGameNotifiation> Pending 
		{
			get { return _null.Pending; }
		}

		#endregion
		#region IService implementation
		
		public ITask GetInitializer(IServiceResolver resolver)
		{
			return resolver.TaskFactory.FromEnumerableAction(Init);
		}
		
		IEnumerator Init()
		{
			yield break;
		}
		
		#endregion

		private NullNotifications _null = new NullNotifications();
	}

#endif
}
