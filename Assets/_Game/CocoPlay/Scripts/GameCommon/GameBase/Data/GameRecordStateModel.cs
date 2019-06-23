using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using TabTale;

namespace Game
{
	public class GameRecordStateModel : StateModel<GameRecordData>
	{

		public List<List<string>> RecordDolls {
			get {
				return _data.recordDolls;
			}
		}

		public void AddRecordDoll (List <string> recordDoll){
			if (_data.recordDolls.Count >= 8) _data.recordDolls.RemoveAt (0);
			_data.recordDolls.Add (recordDoll);
			Save ();
		}
	}
}
