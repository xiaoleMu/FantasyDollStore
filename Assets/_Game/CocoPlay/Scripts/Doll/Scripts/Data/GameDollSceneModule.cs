﻿using UnityEngine;
using System.Collections;
using CocoPlay;

namespace Game
{
	public class GameDollSceneModule : CocoSceneModuleBase
	{
		protected override void InitSignals ()
		{
			base.InitSignals ();
			Bind <GameDollCategoryBtnClickSignal> ();
			Bind <GameDollItemBtnClickSignal> ();
		}

		protected override void CleanSignals ()
		{
			Unbind <GameDollCategoryBtnClickSignal> ();
			Unbind <GameDollItemBtnClickSignal> ();
			base.CleanSignals ();
		}

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