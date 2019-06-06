using UnityEngine;
using System.Collections;

namespace TabTale
{
	public abstract class ControllerTask : BaseTask
	{
		public ControllerTask()
			: this(GameApplication.Instance.TaskFactory)
		{
		}

		protected ControllerTask(ITaskFactory taskFactory)
		{
			_taskFactory = taskFactory;
		}

		override public TaskEnding Handle ()
		{
			IEnumerator controller = Controller();
			while(controller.MoveNext())
			{
			}
			
			InvokeDone(TaskEnding.Done);
			return TaskEnding.Done;
		}

		protected abstract IEnumerator Controller();

		public event System.Action Starting = () => {};

		protected void InvokeStarting()
		{
			_taskState = TaskState.Started;
			Starting();
		}

		ITaskFactory _taskFactory;

		protected float _timeout;
		override public ITask Start (float timeout = -1f)
		{
			_timeout = timeout;

			_startTime = Time.realtimeSinceStartup;
			InvokeStarting();
			
			_taskFactory.FromEnumerableAction(Controller).Start(result => InvokeDone(result), timeout);
			
			return this;
		}
	}
}
