using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TabTale
{
	public class ButtonClickSound : View
	{

		//[Inject]
		//public SfxChannel sfxChannel{ get; set; }

		//[Inject]
		//public GlobalSounds globalSounds { get; set; }

		protected override void Awake ()
		{
			base.Awake ();


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
		//	sfxChannel.Play(globalSounds.tapSnd);
		}
	}
}
