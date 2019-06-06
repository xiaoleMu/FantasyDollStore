using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using TabTale;

namespace CocoPlay
{
	public class CocoGlobalRecordData : IStateData
	{
		LitJson.JsonData pdata;

		#region Game Common

		public int mGameDuration = 0;
		public int mSessionNumber = 0;
		public bool mFirstTimeFlowOver = false;
		public int mFunnelSceneCount = 0;

		#endregion

		#region AB Test

		public GPType curLocalGpType = GPType.None;
		public GPType curGpType = GPType.None;

		#endregion

		public string SaveStr = "";

		public string GetStateName ()
		{
			return "CocoRecordState";
		}

		public override string ToString ()
		{
			return "CocoRecordState";
		}

		public string ToLogString ()
		{
			return "CocoRecordState";
		}

		public IStateData Clone ()
		{
			CocoGlobalRecordData data = new CocoGlobalRecordData ();

			data.SaveStr = SaveStr;

			#region Game Common
			data.mGameDuration = mGameDuration;
			data.mSessionNumber = mSessionNumber;
			data.mFirstTimeFlowOver = mFirstTimeFlowOver;
			data.mFunnelSceneCount = mFunnelSceneCount;
			#endregion

			#region AB Test
			data.curLocalGpType = curLocalGpType;
			data.curGpType = curGpType;
			#endregion

			return data;
		}
	}
}
