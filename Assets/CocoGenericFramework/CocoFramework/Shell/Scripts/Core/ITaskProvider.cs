using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale {

	public interface ITaskProvider
	{
		IEnumerable<ITask> GetTasks(TaskQueue queue);
	}
}
