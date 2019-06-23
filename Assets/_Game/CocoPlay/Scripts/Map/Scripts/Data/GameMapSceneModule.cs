using UnityEngine;
using System.Collections;
using CocoPlay;

namespace Game
{
	public class GameMapSceneModule : CocoSceneModuleBase
	{
		protected override void InitDatas ()
		{
			base.InitDatas ();

			Bind <GameDollData> ();
		}

		protected override void CleanDatas ()
		{
			base.CleanDatas ();

			Unbind <GameDollData> ();
		}
	}
}
