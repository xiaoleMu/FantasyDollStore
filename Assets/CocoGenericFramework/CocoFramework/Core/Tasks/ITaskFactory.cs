using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale 
{
	public interface ITaskFactory
	{
		/// <summary>
		/// Create a special task which waits for something to happen.
		/// This task will continue to run until the poller callback returns true.
		/// </summary>
		/// <returns>The waiter task.</returns>
		/// <param name="poller">A callback that will be called every cycle
		/// by the task handler - if it is true, the task will complete.</param>
		/// <param name="name">Name for the task to appear in debug logs</param>
		ITask FromPredicate(System.Func<bool> poller, string name = "");

		ITask FromAsyncOperation(System.Func<AsyncOperation> operation);
		ITask FromAsyncOperation(AsyncOperation operation);

		ITask NullTask { get; }

		ITask GetWaiter(float time, int frames);

		ITask Serialize(IEnumerable<ITask> tasks);
		ITask Parallelize(ICollection<ITask> tasks);

		ITask FromEnumerableAction(System.Func<IEnumerator> serialAction);
		ITask<T> FromEnumerableAction<T>(IEnumerableAction<T> serialAction) where T : class;
	}
}
