using System;

	namespace TabTale
	{
	public class LeaderboardPlayerData : ICloneable
		{		
			public long playerId;
			public int score;
			public int rank;
			public string playerName;

			#region ICloneable implementation
			public object Clone ()
			{
				LeaderboardPlayerData c = new LeaderboardPlayerData();
				c.playerId = playerId;
				c.score = score;
				c.rank = rank;
				c.playerName = playerName;
				return c;
			}
			#endregion

			public override string ToString ()
			{
				return string.Format ("[LeaderboardPlayerData: playerId={0}, score={1}, rank={2}, playerName={3}]", playerId, score, rank, playerName);
			}
		}
	}