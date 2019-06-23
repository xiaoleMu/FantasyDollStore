using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CocoPlay;

namespace Game{
	public class GameDollAnimationData : ISceneAnimationData {

		List<CCAnimationData> m_basicData;
		List<CCAnimationData> m_PoseData;
		int random_count;
		int oldIndex = -1;

		public GameDollAnimationData()
		{
			m_basicData = new List<CCAnimationData>() 
			{
				CocoRoot.GetInstance <GameDollData>().CA_Dressup_Standby,
			};

			m_PoseData = new List<CCAnimationData>() {
				CocoRoot.GetInstance <GameDollData>().CA_Dressup_Pose1,
				CocoRoot.GetInstance <GameDollData>().CA_Dressup_Pose2,
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
