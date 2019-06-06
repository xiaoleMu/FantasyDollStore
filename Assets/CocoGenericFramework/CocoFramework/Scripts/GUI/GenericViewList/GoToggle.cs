using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace TabTale
{
	public class GoToggle : MonoBehaviour
	{

		public event Action<GoToggle> ToggleEvent = (b)=>{};

		public GameObject selected;
		public GameObject unSelected;
		public bool isOn=false;

		void Awake(){
			Button button = GetComponent<Button>();
			
			if(button){
				button.onClick.AddListener(OnClickButton);
			}
			else{
				EventTrigger eventTrigger = GetComponent<EventTrigger> ();
				if(eventTrigger)
					eventTrigger.AddTriggertListener(EventTriggerType.PointerClick,OnClickTrigger);
			}
		}

		void OnClickButton (){
			DoOnClick();
		}
		
		void OnClickTrigger (BaseEventData arg0)
		{
			DoOnClick();
		}

		void Start ()
		{
			IsOn=isOn;
		}

		public void DoOnClick ()
		{
			IsOn=!IsOn;
			ToggleEvent(this);
		}

		public bool IsOn {
			get {
				return isOn;
			}
			set {
				isOn = value;

				if(selected!=null)
					selected.SetActive(isOn);
				if(unSelected!=null)
					unSelected.SetActive(!isOn);
			}
		}
	}
}