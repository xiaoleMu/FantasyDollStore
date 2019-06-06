using System;

namespace TabTale
{
	public class PlayerInfoService
	{
		private const string _keyPlayerId = "playerId";

		public string PlayerId 
		{
			get
			{
				return TTPlayerPrefs.GetValue(_keyPlayerId, "0");
			}
		}

		public long NumericalPlayerId
		{
			get
			{
				return Convert.ToInt64(PlayerId);
			}
		}
	}
}

