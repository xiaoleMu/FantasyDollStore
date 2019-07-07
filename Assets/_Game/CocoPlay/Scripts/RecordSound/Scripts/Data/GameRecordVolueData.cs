using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;
using System.Collections.Generic;
using CocoPlay;

namespace Game
{
	public class GameRecordVolueData
	{

		public CCAnimationData CA_RecordVolue_Standby = new CCAnimationData("standby01");
		public CCAnimationData CA_RecordVolue_Pose1 = new CCAnimationData("pose01");
		public CCAnimationData CA_RecordVolue_Pose2 = new CCAnimationData("pose02");
		public CCAnimationData CA_RecordVolue_Pose3 = new CCAnimationData("pose03");



		public GameRecordVolueData (){
			InitRecordVolueData ();
		}

		private void InitRecordVolueData (){
			

		}
			
	}

}
