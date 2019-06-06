using System.Collections.Generic;

namespace TabTale
{
	public class MatchStateData : IStateData
	{
		public List<PlayerMatchData> playersMatchData = new List<PlayerMatchData>();
		public LevelMatchData levelMatchData;
		#region IStateData implementation
		
		public string GetStateName ()
		{
			return "matchState";
		}
		
		public string ToLogString ()
		{
			return string.Format("MatchStateData: LevelData:{0}, Players:{1}",levelMatchData,playersMatchData.ArrayString());
		}
		
		public IStateData Clone ()
		{
			MatchStateData c = new MatchStateData();
			c.playersMatchData = new List<PlayerMatchData>(playersMatchData.Clone());
			c.levelMatchData = (LevelMatchData)levelMatchData.Clone();
			return c;
		}
		
		#endregion
	}
}