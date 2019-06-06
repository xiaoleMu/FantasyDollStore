using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class RelationshipScoreStateData : ISharedData
	{
		public string id;
		public List<ScoreData> scores = new List<ScoreData>();
		public const string TABLE_NAME = "relationship_score_state";

		#region ISharedData implementation

		public string GetTableName ()
		{
			return TABLE_NAME;
		}

		public string GetId ()
		{
			return id;
		}

		public string ToLogString ()
		{
			return string.Format ("[RelationshipScoreStateData: id={0}, scores={1}]", id, scores.ArrayString());
		}

		public ISharedData Clone ()
		{
			RelationshipScoreStateData clone = new RelationshipScoreStateData();
			clone.id = this.id;
			clone.scores = new List<ScoreData>(scores.Clone());
			return clone;
		}

		#endregion
	}
}
