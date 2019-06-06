using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface IEnumerableAction<TResult>
	{
		event System.Action<TResult> ResultSet;

		IEnumerator Run(System.Action<TResult> handler);
		IEnumerator Run();

		float EstimateProgress(int iteration);

		string Name { get; }
	}

	public interface IEnumerableAction : IEnumerableAction<object>
	{
	}
}
