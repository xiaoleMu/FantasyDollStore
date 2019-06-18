using UnityEngine;
using System.Collections;

public class CCSceneAnimationData : ISceneAnimationData
{
	public CCAnimationData Basic_StandBy;
	
	public CCAnimationData GetNextAnimData ()
	{
		return Basic_StandBy;
	}

	public void SetStanbyData(CCAnimationData _StandBy)
	{
		Basic_StandBy = _StandBy;
	}
}
