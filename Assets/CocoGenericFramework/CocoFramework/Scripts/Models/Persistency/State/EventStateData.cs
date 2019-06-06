using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class EventStateData : ISharedData
	{
		public string id;
		public int score;
		public int progressionIndex;
		public string playerName;
		public int bucket;
		public bool isConcluded;

		#region IStateData implementation

		public string GetTableName ()
		{
			return "event_state";
		}

		public string GetId()
		{
			return id;
		}

		public string ToLogString ()
		{
			return string.Format ("[EventSharedData: Id={0}, Score={1}, ProgressionIndex={2}, PlayerName={3}, Bucket={4}, IsConcluded={5}", 
				id, score, progressionIndex, playerName, bucket, isConcluded);
		}

		public ISharedData Clone ()
		{
			var c = new EventStateData();
			c.id = id;
			c.score = score;
			c.progressionIndex = progressionIndex;
			c.playerName = playerName;
			c.bucket = bucket;
			c.isConcluded = isConcluded;

			return c;
		}

		#endregion
	}
}