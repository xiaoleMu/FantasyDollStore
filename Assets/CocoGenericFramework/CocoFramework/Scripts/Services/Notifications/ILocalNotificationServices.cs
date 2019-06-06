using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale {

	public interface ILocalNotificationServices : IService
	{
		ILocalGameNotifiation Schedule(string title, string body, string image, System.DateTime time);
		IEnumerable<ILocalGameNotifiation> Pending { get; }
	}
}
