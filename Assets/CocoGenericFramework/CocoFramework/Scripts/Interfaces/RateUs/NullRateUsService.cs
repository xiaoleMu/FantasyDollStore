using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class NullRateUsService : IRateUsService
	{
		#region IRateUsService implementation

		public bool SmallSatisfactionPointReached ()
		{
			Debug.Log ("NullRateUsService : SmallSatisfactionPointReached");
			return false;
		}

		public bool LargeSatisfactionPointReached ()
		{
			Debug.Log ("NullRateUsService : LargeSatisfactionPointReached");
			return false;
		}

		public bool ShouldShowRateUs ()
		{
			Debug.Log ("NullRateUsService : ShouldShowRateUs");
			return false;
		}

		public void SetActive (bool a)
		{
			Debug.Log ("NullRateUsService : SetActive");
		}

		public void Show()
		{
			Debug.Log ("NullRateUsService : Show");	
		}

		public void DialogResultEvent (RateUsDialogResult a, bool b)
		{
			Debug.Log("Obsolete function call");
		}

		#endregion


	}
}