using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TabTale
{
	public class ButtonClickBasicSound : MonoBehaviour {

		public AudioClip sound; 

		// Use this for initialization
		void Awake () {
			
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
		
		void DoOnClick(){
			//AudioSource.PlayClipAtPoint (sound, Vector3.zero);
		}
	}
}

