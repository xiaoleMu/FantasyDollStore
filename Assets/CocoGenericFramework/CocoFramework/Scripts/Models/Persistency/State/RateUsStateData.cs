using UnityEngine;
using System.Collections;
using System;


namespace TabTale
{
	public class RateUsStateData : IStateData
	{

		public int satisfactionPoints;
		public bool neverShow;
		public DateTime lastDisplayed;
		public string lastUpdateVersion;

	#region IStateData implementation

		public string GetStateName ()
		{
			return "rateUsState";
		}

		public string ToLogString ()
		{
			return "RateUsState: " + LitJson.JsonMapper.ToJson (this);
		}

		public IStateData Clone ()
		{
			string json = LitJson.JsonMapper.ToJson (this);
			RateUsStateData c = LitJson.JsonMapper.ToObject<RateUsStateData> (json);
			return c;
		}

	#endregion
	}

}