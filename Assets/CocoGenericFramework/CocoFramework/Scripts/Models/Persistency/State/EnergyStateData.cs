using UnityEngine;
using System.Collections;
using System;

namespace TabTale
{
	public class EnergyStateData : IStateData 
	{
		public string lastChargeTime = new DateTime().ToString();

		#region IStateData implementation

		public string GetStateName ()
		{
			return "energyState";
		}

		public string ToLogString ()
		{
			return string.Format("lastChargeTime:{0}",lastChargeTime);
		}

		public IStateData Clone ()
		{
			EnergyStateData c = new EnergyStateData();
			c.lastChargeTime = lastChargeTime;
			return c;
		}

		#endregion


	}
}