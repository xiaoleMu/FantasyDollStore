using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class EnergyStateModel : StateModel<EnergyStateData> 
	{

		public string LastChargeTime 
		{
			get { return _data.lastChargeTime; }
			set 
			{
				_data.lastChargeTime = value;
				Save ();
			}
		}
	}
}