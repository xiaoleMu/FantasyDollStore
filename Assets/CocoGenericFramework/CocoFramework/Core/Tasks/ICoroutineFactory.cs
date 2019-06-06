using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface ICoroutineFactory
	{
		/// <summary>
		/// Starts a Coroutine from the given factory method - the method returns
		/// an IEnumerator, which is then used as the Coroutine.
		/// </summary>
		/// <returns>The coroutine.</returns>
		/// <param name="enumeratorFactory">Enumerator factory.</param>
		Coroutine StartCoroutine(System.Func<IEnumerator> enumeratorFactory);

		/// <summary>
		/// Runs the given action as a Coroutine.
		/// </summary>
		/// <returns>The coroutine.</returns>
		/// <param name="enumerableAction">Enumerable action.</param>
		Coroutine StartCoroutine(IEnumerableAction enumerableAction);

		Coroutine Concat(Coroutine routine1, Coroutine courinte2);

		/// <summary>
		/// Creates a Coroutine that starts a task and waits for it to end.
		/// </summary>
		/// <returns>The wait.</returns>
		/// <param name="task">Task.</param>
		Coroutine StartWait(ITask task);

		/// <summary>
		/// Creates a Coroutine that starts a task with timeout and waits for it to end.
		/// </summary>
		/// <returns>The wait.</returns>
		/// <param name="task">Task.</param>
		/// <param name="timeout">Timeout.</param>
		Coroutine StartWait(ITask task, float timeout);

		/// <summary>
		/// Creates a Coroutine that waits for a task to end - task should be started
		/// someplace else, as this Coroutine will not do so.
		/// </summary>
		/// <param name="task">Task.</param>
		Coroutine Wait(ITask task);

		/// <summary>
		/// Creates a Coroutine that waits for a task to end, with a timeout
		/// - task should be started someplace else, as this Coroutine will not do so.
		/// </summary>
		/// <param name="task">Task.</param>
		Coroutine Wait(ITask task, float timeout);
	}
}
