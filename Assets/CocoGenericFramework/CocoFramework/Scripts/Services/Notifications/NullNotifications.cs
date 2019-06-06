using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale {

	public class NullNotifications : ILocalNotificationServices
	{
		#region ILocalNotificationServices implementation

		public ILocalGameNotifiation Schedule (string title, string body, string image, System.DateTime time)
		{
			LocalGameNotification notification = new LocalGameNotification(title, body, image, time);
			_pending.Add(notification);

			return notification;
		}

		public IEnumerable<ILocalGameNotifiation> Pending 
		{
			get { return _pending.Cast<ILocalGameNotifiation>(); }
		}

		#endregion

		private IList<LocalGameNotification> _pending = new List<LocalGameNotification>();

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
}