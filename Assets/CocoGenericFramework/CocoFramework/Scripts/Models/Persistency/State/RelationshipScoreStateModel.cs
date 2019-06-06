using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public class RelationshipScoreStateModel : SharedStateModel<RelationshipScoreStateData>
	{
		public ICollection<ScoreData> GetScores(string levelConfigId)
		{
			RelationshipScoreStateData scoreStateData = _sharedStateItems.FirstOrDefault(data => data.id == levelConfigId);
			if (scoreStateData != null)
			{
				return scoreStateData.scores.Clone();
			}
			else
			{
				return null;
			}
		}

		public string GetTableName()
		{
			return RelationshipScoreStateData.TABLE_NAME;
		}
	}
}
