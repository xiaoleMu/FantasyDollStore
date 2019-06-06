using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

namespace TabTale
{
	public class RateUsService : IRateUsService
	{
		private IPsdkRateUs _psdkRateUs;

		private bool serviceActive = true;

		[PostConstruct]
		public void Init()
		{
			_psdkRateUs = PSDKMgr.Instance.GetRateUs();
		}

		public bool SmallSatisfactionPointReached ()
		{
			return _psdkRateUs.SmallSatisfactionPointReached();
		}

		public bool LargeSatisfactionPointReached ()
		{
			return _psdkRateUs.LargeSatisfactionPointReached();
		}

		public bool ShouldShowRateUs ()
		{
			if( !serviceActive ) return false; // serviceActive should always be true, this condition is kept for backwards compatability

			return _psdkRateUs.ShouldShowRateUs();
		}

		public void Show()
		{
			_psdkRateUs.Show();
		}

		[Obsolete("This function is supported for backwards compatability, but for new projects use Show instead")]
		public void DialogResultEvent (RateUsDialogResult result, bool neverShow)
		{
			if(result == RateUsDialogResult.Rate)
			{
				_psdkRateUs.Show();
			}

			if (neverShow) 
			{
				_psdkRateUs.NeverShow();
			}
		}


		[Obsolete("This function is supported for backwards compatability, should not be used for new projects")]
		public void SetActive (bool active) 
		{ 
			serviceActive = active; 
		}

		[Obsolete("Use ShouldShowRateUs instead")]
		public bool NeverShow 
		{
			get { return !ShouldShowRateUs(); }
		}
	}

}
