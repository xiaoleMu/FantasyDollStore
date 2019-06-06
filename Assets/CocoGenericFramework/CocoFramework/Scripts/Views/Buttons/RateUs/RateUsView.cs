using UnityEngine;
using System.Collections;
using System;

namespace TabTale
{
	public class RateUsView : MainView
	{
		[Inject]
		public IRateUsService rateUsService { get; set; }

		[Inject]
		public RateUsStateModel rateUsModel {get;set;}

		public UnityEngine.UI.Toggle NeverShowToggle;

		private void Start()
		{
			base.Start();
			rateUsModel.LastDisplayed = DateTime.Now;
		}

		public void OnClose ()
		{
			rateUsService.DialogResultEvent (RateUsDialogResult.Close, NeverShowToggle.isOn);
		}

		public void OnLaterClicked ()
		{
			rateUsService.DialogResultEvent (RateUsDialogResult.Later, NeverShowToggle.isOn);
		}

		public void OnRateUsClicked ()
		{
			rateUsService.DialogResultEvent (RateUsDialogResult.Rate, true); // On rate us click never show the dialog again
		}
	}
}
