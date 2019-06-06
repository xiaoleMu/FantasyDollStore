using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface IRateUsService
	{
		bool SmallSatisfactionPointReached ();
		
		bool LargeSatisfactionPointReached ();
		
		bool ShouldShowRateUs ();

		void Show();

		#region Obsolete

		[System.Obsolete("For new projects use Show instead. This function is supported only for backwards compatability purposes")]
		void DialogResultEvent (RateUsDialogResult result, bool never);


		[System.Obsolete("This function is supported for backwards compatability, should not be used for new projects")]
		void SetActive (bool active);

		#endregion
		
	}

	public enum RateUsDialogResult
	{
		Rate,
		Later,
		Close
	}

}