using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class ProgressStateData : IStateData
	{
		public int maxLevel;
		public int xp;
		public int topScore;
		public int rankIndex;
		public List<GameElementData> progress = new List<GameElementData>();

		#region IStateData implementation
		
		public string GetStateName ()
		{
			return "progressState";
		}
		//TODO: Replace all arraystring in logstring
		public string ToLogString ()
		{
			return string.Format("ProgressStateData: Progress:{0}, maxLevel:{1}, rank:{2}, xp:{3}, topScore:{4}",progress.ArrayString(), maxLevel, rankIndex, xp, topScore);
		}
		
		public IStateData Clone ()
		{
			ProgressStateData c = new ProgressStateData();
			c.maxLevel = maxLevel;
			c.rankIndex = rankIndex;
			c.topScore = topScore;
			c.xp = xp;
			c.progress = new List<GameElementData>(progress.Clone());
			return c;
		}
		
		#endregion
	}
}