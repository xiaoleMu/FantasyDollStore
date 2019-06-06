using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace TabTale
{
	public class CurrencyStateData : IStateData
	{
		public List<GameElementData> currencies = new List<GameElementData>();
		#region IStateData implementation

		public string GetStateName ()
		{
			return "currencyState";
		}

		public string ToLogString ()
		{
			return string.Format("CurrencyStateData: Currencies:{0}",currencies.ArrayString());
		}

		public IStateData Clone ()
		{
			CurrencyStateData c = new CurrencyStateData();
			c.currencies = new List<GameElementData>(currencies.Clone<GameElementData>());

			return c;
		}

		#endregion
	}
}
