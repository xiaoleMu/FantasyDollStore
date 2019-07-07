using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CocoPlay;

namespace Game{
	public class GameRecordAnimationData : ISceneAnimationData {

		List<CCAnimationData> m_basicData;
		List<CCAnimationData> m_PoseData;
		int random_count;
		int oldIndex = -1;

		public GameRecordAnimationData()
		{
			m_basicData = new List<CCAnimationData>() 
			{
				CocoRoot.GetInstance <GameRecordVolueData>().CA_RecordVolue_Standby,
			};

			m_PoseData = new List<CCAnimationData>() {
				CocoRoot.GetInstance <GameRecordVolueData>().CA_RecordVolue_Pose1,
				CocoRoot.GetInstance <GameRecordVolueData>().CA_RecordVolue_Pose2,
				CocoRoot.GetInstance <GameRecordVolueData>().CA_RecordVolue_Pose3,
			};
		}

		public CCAnimationData GetNextAnimData()
		{
			int index;
			if(random_count <= 0)
			{
				random_count = 2;
				index = getRandomIndex ();
				return m_PoseData[index];
			}
			else
			{
				random_count--;
				index = Random.Range(0, m_basicData.Count);
				return m_basicData[index];
			}
		}

		int getRandomIndex()
		{
			int index = Random.Range(0, m_PoseData.Count);
			if (index != oldIndex) {
				oldIndex = index;
				return index;
			} else
				return getRandomIndex ();
		}
	}
}
