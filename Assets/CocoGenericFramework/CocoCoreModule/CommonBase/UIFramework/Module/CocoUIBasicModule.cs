using UnityEngine;
using System.Collections;

namespace CocoPlay
{
	public class CocoUIBasicModule : CocoModuleBase
	{
		#region Signals

		protected override void InitSignals ()
		{
			Bind<CocoUIButtonClickSignal> ();
			Bind<CocoUIButtonTriggerClickSignal> ();
		}

		protected override void CleanSignals ()
		{
			Unbind<CocoUIButtonClickSignal> ();
			Unbind<CocoUIButtonTriggerClickSignal> ();
		}

		#endregion
	}
}
