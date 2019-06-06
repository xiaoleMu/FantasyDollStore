using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

namespace TabTale
{
	public static class EventTriggerExtensions
	{
		public static void AddTriggertListener (this EventTrigger eventTrigger, EventTriggerType type, Action<BaseEventData> callback)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = type;
			entry.callback = new EventTrigger.TriggerEvent ();
			entry.callback.AddListener (new UnityAction<BaseEventData> (callback));
			eventTrigger.triggers.Add (entry);
		}
	}
}
