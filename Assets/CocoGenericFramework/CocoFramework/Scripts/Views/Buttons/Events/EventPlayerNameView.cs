using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace TabTale
{
	public class EventPlayerNameView : MainView
	{
		[Inject]
		public EventSystemService eventSystemService { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[Inject]
		public SocialStateModel socialStateModel{ get; set; }

		public InputField nameTextInput;

		List<EventConfigData> availableEventData = new List<EventConfigData>();
		private EventConfigData _availableEventData = null;

		private void Awake()
		{
			base.Awake();
			availableEventData = eventSystemService.GetAvailableEvents ();
		}

		private void Start()
		{
			base.Start();
			_availableEventData = availableEventData [0];
		}

		public void OnClick()
		{
			string name = nameTextInput.text;

			if (networkCheck.HasInternetConnection ()) 
			{
				if (name != "" || name != " " && name.Length < 100) 
				{
					string eventID = _availableEventData.id;
					socialStateModel.SetPlayerName(name);
					eventSystemService.RegisterToEvent (eventID);
				}
			}
		}
	}
}
