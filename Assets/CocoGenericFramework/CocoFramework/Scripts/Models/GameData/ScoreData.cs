using UnityEngine;
using System.Collections;
using System;

namespace TabTale
{
	public class ScoreData : ICloneable
	{
		public string playerId;
		public int score;

		public ScoreData() {}

		public ScoreData (string playerId, int score)
		{
			this.playerId = playerId;
			this.score = score;
		}

		#region ICloneable implementation

		public object Clone ()
		{
			return new ScoreData(playerId, score);
		}

		#endregion

		public override string ToString ()
		{
			return string.Format ("[ScoreData: playerId={0}, score={1}]", playerId, score);
		}
	}
}
