using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace TabTale
{
	public class ModalsCanvas : MonoBehaviour,IModalsCanvas
	{

		public GameObject mask;

		public event Action MaskClickEvent;

		EventTrigger _maskEventTrigger;

		public void Awake ()
		{
			_maskEventTrigger = mask.GetComponent<EventTrigger> ();
			if (_maskEventTrigger != null) {
				_maskEventTrigger.AddTriggertListener (EventTriggerType.PointerClick, OnMaskClick);
			}
		}

		void OnMaskClick (BaseEventData arg0)
		{
			if (MaskClickEvent != null)
				MaskClickEvent ();
		}

		public void ShowMask ()
		{
			if (mask != null)
				mask.SetActive (true);
		}

		public void HideMask ()
		{
			if (mask != null)
				mask.SetActive (false);
		}


	}
}
