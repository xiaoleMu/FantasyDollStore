using UnityEngine;
using System.Collections;

namespace CocoPlay
{
	public class CocoMakeupSceneModule : CocoSceneModuleBase
	{
		protected override void InitSignals ()
		{
			base.InitSignals ();
			Bind <CocoMakeupPaintSignal> ();
			Bind <CocoMakeupPaintEndSignal> ();
			Bind<MakeupClearSignal>();
		}

		protected override void CleanSignals ()
		{
			Unbind <CocoMakeupPaintSignal> ();
			Unbind <CocoMakeupPaintEndSignal> ();
			Unbind<MakeupClearSignal>();
			base.CleanSignals ();
		}

		protected override void InitDatas ()
		{
			base.InitDatas ();
		}

		protected override void CleanDatas ()
		{
			base.CleanDatas ();
		}
	}
}
