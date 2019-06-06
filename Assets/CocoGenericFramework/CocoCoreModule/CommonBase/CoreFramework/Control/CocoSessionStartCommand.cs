using UnityEngine;
using System.Collections;
using TabTale;
using CocoPlay;
using TabTale.Publishing;

public class CocoSessionStartCommand : ShowSessionStartCommand {

	public override void Execute ()
	{
		bool shouldShowSessionStart = false;

		shouldShowSessionStart = CocoGlobalData.m_GamePlayIsStarted;

		if (shouldShowSessionStart) {
			base.Execute ();
		}
	}
}
