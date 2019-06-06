using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale 
{
	public interface ITaskQueue : ITaskFactory
	{
		bool Active { get; set; }
		void Start();

		void HandleAll();

		ITask Enqueue(System.Action action, string name = "");
		ITask Enqueue(System.Func<IEnumerator> enumerableAction, string name = "");
		ITask Enqueue(IEnumerableAction enumerableAction);
		ITask Parallelize(System.Func<bool> poller, float timeout = -1f, string name = "");

		int Count { get; }

		event System.Action QueueEmpty;
	}
}
