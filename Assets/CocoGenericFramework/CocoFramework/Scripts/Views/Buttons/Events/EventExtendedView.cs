using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
using TabTale;

namespace TabTale
{
	public class EventExtendedView : GameView
	{
		[Inject]
		public EventSystemService eventSystemService { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		public void CheckEvents()
		{
			switch (eventSystemService.CheckEventState()) 
			{
			case(EventDispatch.noEvents):
				Debug.LogWarning ("EventExtendedView" + " No Events Available");
				break;
			case (EventDispatch.noInternet):
				Debug.LogWarning ("EventExtendedView" + " No Internet Available");
				break;
			case (EventDispatch.showAvailable):
				Debug.LogWarning ("EventExtendedView" + " Show Available events");
				break;
			case (EventDispatch.showCurrent):
				Debug.LogWarning ("EventExtendedView" + " Show Current Available");
				break;
			case (EventDispatch.requestFailed):
				Debug.LogWarning ("EventExtendedView" + " Request to server failed");
				break;
			}
		}
	}
}
