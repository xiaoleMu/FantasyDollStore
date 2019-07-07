using UnityEngine;
using System.Collections;
using CocoPlay;

namespace Game
{
	public class GameRecordSceneModule : CocoSceneModuleBase
	{

		protected override void InitDatas ()
		{
			base.InitDatas ();

			Bind <GameRecordVolueData> ();
		}

		protected override void CleanDatas ()
		{
			base.CleanDatas ();

			Unbind <GameRecordVolueData> ();
		}
	}
}
