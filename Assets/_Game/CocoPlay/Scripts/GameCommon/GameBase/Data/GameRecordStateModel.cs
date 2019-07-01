using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using TabTale;

namespace Game
{
	public class GameRecordStateModel : StateModel<GameRecordData>
	{

		public List<DollRecordData> RecordDolls {
			get {
				return _data.recordDolls;
			}
		}

		public void AddRecordDoll (DollRecordData recordDoll, int index = -1){
			if (index >= 0){
				_data.recordDolls[index] = recordDoll;
			}
			else {
				if (_data.recordDolls.Count >= 8) _data.recordDolls.RemoveAt (0);
				_data.recordDolls.Add (recordDoll);
			}
			Save ();
		}
	}

	public class DollRecordData {
		public int detailIndex;
		public List<string> dress;
	}
}
