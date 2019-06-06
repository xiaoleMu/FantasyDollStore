using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public enum MatchStatus { Lost,Pause,Playing,Ready,Won }

	public enum PlayerType { Local = 0, Remote = 1 }

	public class PlayerMatchData : ICloneable
	{
		public SocialStateData playerProfile;
		public PlayerType playerType;
		public List<GameElementData> currenciesEarned = new List<GameElementData>();
		public int xpGained;
		public int score;
		public List<GameElementData> progress = new List<GameElementData>();
		public List<ItemData> itemsInUse = new List<ItemData>();
		public MatchStatus matchStatus;
		public List<GameElementData> properties = new List<GameElementData>();
		public int index;

		//TODO: replace object[] once realtime match is implemented.
		public List<object> actions = new List<object>();

		public PlayerMatchData() {}

		#region ICloneable implementation

		public object Clone ()
		{
			PlayerMatchData c = new PlayerMatchData();
			c.playerProfile = playerProfile.Clone() as SocialStateData;
			c.playerType = playerType;
			c.currenciesEarned = currenciesEarned.Select(currency => currency.Clone() as GameElementData).ToList();
			c.xpGained = xpGained;
			c.score = score;
			c.progress = progress.Select(p => p.Clone() as GameElementData).ToList();
			c.itemsInUse = itemsInUse.Select(item => item.Clone() as ItemData).ToList();
			c.properties = properties.Select(p => p.Clone() as GameElementData).ToList();
			c.matchStatus = matchStatus;
			c.index = index;

			return c;
		}

		#endregion
	}
}
