using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface IObservableAction<TStepResult, TActionResult>
	{
		event System.Func<TStepResult, bool> Step;
		event System.Action<TActionResult> Done;
		event System.Func<System.Exception, bool> Exception;

		IObservableAction<TStepResult, TActionResult> Start();
		void Cancel();
	}

	public interface IObservableAction<TStepResult> : IObservableAction<TStepResult, object>
	{
	}

	public interface IObservableAction : IObservableAction<object, object>
	{
	}
}
