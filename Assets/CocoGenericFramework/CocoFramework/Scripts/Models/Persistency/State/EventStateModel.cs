using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale
{
	public class EventStateModel : SharedStateModel<EventStateData>
	{
		[Inject]
		public EventConcludedSignal eventConcludedSignal { get; set; }

		// We have to cache the unconcluded events for performance reasons (Special case)
		// Caching such as this should only be done when needed
		// And requires overriding and updating data in ValidateAfterSync !
		private List<EventStateData> unConcludedEvents = new List<EventStateData>();

		private const string Tag = "EventStateModel";

		[PostConstruct]
		public void Init()
		{
			RefreshUnConcludedEvents();
		}

		#region isConcluded query, refresh and resolution
		private void RefreshUnConcludedEvents()
		{
			unConcludedEvents = _sharedStateItems.Where(ev => ev.isConcluded == false).ToList();
		}

		protected override bool ValidateAfterSync() 
		{ 
			base.ValidateAfterSync();

			RefreshUnConcludedEvents();

			return true;
		}

		public bool IsConcluded(string eventId)
		{
			EventStateData data = _sharedStateItems.FirstOrDefault(ev => ev.id == eventId);
			if(data == null)
			{
				return true;
			}

			return data.isConcluded;
		}

		#endregion

		#region Getters
		public EventStateData GetEvent(string eventId)
		{
			EventStateData data = _sharedStateItems.FirstOrDefault(ev => ev.id == eventId);
			return data;
		}

		public List<EventStateData> GetUnConcludedEvents()
		{
			return unConcludedEvents;
		}

		public int GetEventScore(string eventId)
		{
			EventStateData data = _sharedStateItems.FirstOrDefault(ev => ev.id == eventId);

			if(data == null)
			{
				return 0;
			}

			return data.score;
		}

		public int GetPlayerBucket(string eventId)
		{
			EventStateData data = _sharedStateItems.FirstOrDefault(ev => ev.id == eventId);

			if(data == null)
			{
				return 0;
			}

			return data.bucket;
		}

		#endregion

		#region Setters

		public void SetConcluded(string eventId)
		{
			EventStateData data = _sharedStateItems.FirstOrDefault(ev => ev.id == eventId);

			if(data == null)
			{
#if UNITY_2017_1_OR_NEWER
				Debug.unityLogger.LogError(Tag, "Cannot find an event with the specified id : " + eventId);
#else
				Debug.logger.LogError(Tag, "Cannot find an event with the specified id : " + eventId);
#endif
				return;
			}

			if(data.isConcluded)
			{
#if UNITY_2017_1_OR_NEWER
				Debug.unityLogger.LogError(Tag, "Event is already concluded : " + eventId);
#else
				Debug.logger.LogError(Tag, "Event is already concluded : " + eventId);
#endif
				return;
			}
				
			data.isConcluded = true;

			SaveEvent(data);

			eventConcludedSignal.Dispatch(eventId);
		}

		public void SaveEvent(EventStateData eventId)
		{
			Save(eventId);

			RefreshUnConcludedEvents();
		}

		#endregion
	}
}

