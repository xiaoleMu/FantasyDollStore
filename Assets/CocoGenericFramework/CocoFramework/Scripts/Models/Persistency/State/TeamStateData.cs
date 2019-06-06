using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class TeamStateData : IStateData
	{
		public TeamInfo teamInfo = new TeamInfo();
		public long id;
		public int score;
		public int fansCount;
		public string playerName;
		public string playerImage;

		#region IStateData implementation

		public string GetStateName ()
		{
			return "teamState";
		}

		public string ToLogString ()
		{
			return LitJson.JsonMapper.ToJson (this);
		}

		public IStateData Clone ()
		{
			TeamStateData c = new TeamStateData();
			c.teamInfo = teamInfo;
			c.id = id;
			c.score = score;
			c.fansCount = fansCount;
			c.playerName = playerName;
			c.playerImage = playerImage;

			return c;
		}

		#endregion
	}
}