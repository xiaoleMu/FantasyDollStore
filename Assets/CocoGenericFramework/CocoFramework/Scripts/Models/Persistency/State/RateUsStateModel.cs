using System;

namespace TabTale
{

	public class RateUsStateModel : StateModel<RateUsStateData>
	{
		public int SatisfactionPoints 
		{
			get { return _data.satisfactionPoints; }
			set 
			{
				_data.satisfactionPoints = value;
				Save ();
			}
		}

		public bool NeverShow 
		{
			get { return _data.neverShow; }
			set 
			{
				_data.neverShow = value;
				Save ();
			}
		}

		public DateTime LastDisplayed 
		{
			get { return _data.lastDisplayed; }
			set 
			{
				_data.lastDisplayed = value;
				Save ();
			}
		}

		public string LastUpdateVersion 
		{
			get { return _data.lastUpdateVersion; }
			set 
			{
				_data.lastUpdateVersion = value;
				Save ();
			}
		}

		public void ResetSatisfactionPoints()
		{
			SatisfactionPoints = 0;
		}

	}

}