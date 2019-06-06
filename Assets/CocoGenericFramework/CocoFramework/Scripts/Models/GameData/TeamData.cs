using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public enum TeamMedalType
	{
		gold,
		silver,
		bronze
	}

	public class TeamData : ICloneable
	{
		public string id;
		public TeamInfo teamInfo = new TeamInfo();
		public int fansCount;
		public int score;
		public int rankChange;
		public List<GenericPropertyData> properties = new List<GenericPropertyData>();

		public TeamData() {}

		public TeamData (string teamId, TeamInfo teamInfo, int fansCount = 0, int score = 0, int rankChange = 0, List<GenericPropertyData> properties = null)
		{
			this.id = teamId;
			this.teamInfo = teamInfo;
			this.fansCount = fansCount;
			this.score = score;
			this.rankChange = rankChange;

			if(properties != null)
				this.properties = properties;
		}


		public GenericPropertyData GetProperty(string id)
		{
			GenericPropertyData g = properties.FirstOrDefault(p => p.id == id);
			return g ?? new GenericPropertyData();
		}

		public int GetMedals(TeamMedalType medalType)
		{
			string medalProperty = GetProperty(medalType.ToString()).value;
			int numOfMedals = 0;
			int.TryParse(medalProperty, out numOfMedals);

			return numOfMedals;
		}

		#region ICloneable implementation

		public object Clone ()
		{
			return new TeamData(id,teamInfo,fansCount,score, rankChange, properties.Clone().ToList());
		}

		#endregion

		public override string ToString ()
		{
			return LitJson.JsonMapper.ToJson (this);
		}
	}
}
