using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;

namespace TabTale
{
	public class SignalDialogButtonData : BasicDialogButtonData
	{

		public BaseSignal signal;
		public object[] args;


		public SignalDialogButtonData ()
		{
		}
		
		public SignalDialogButtonData (string caption, BaseSignal signal):base(caption)
		{
			this.signal=signal;
		}

		public SignalDialogButtonData (string caption, BaseSignal signal,object[] args):base(caption)
		{
			this.signal=signal;
			this.args=args;
		}

		public override void Dispatch ()
		{
			signal.Dispatch(args);
		}

	}
}