using UnityEngine;
using System.Collections;

namespace TabTale
{
	public abstract class BaseTask : ITask
	{
		protected virtual void InvokeDone(TaskEnding ending)
		{
			_taskState = TaskState.Done;

			Done(ending);
		}

		#region ITask implementation

		public event System.Action<TaskEnding> Done = result => {};

		public ITask Start (System.Action<TaskEnding> resultHandler, float timeout = -1f)
		{
			Done += resultHandler;
			return Start (timeout);
		}

		public void Cancel(System.Action<TaskEnding> resultHandler)
		{
			Done += resultHandler;
			Cancel ();
		}

		public abstract ITask Start (float timeout = -1f);
		public abstract void Cancel();
		public abstract TaskEnding Handle();

		protected TaskState _taskState = TaskState.Ready;
		public TaskState State 
		{
			get { return _taskState; }
		}

		protected float _startTime;
		public float StartTime 
		{
			get { return _startTime; }
		}

		protected string _name;
		public string Name 
		{
			get { return _name; }
			set { _name = value; }
		}

		#endregion

		#region IProgress implementation

		protected float _progress = 0;
		public float Progress 
		{
			get { return _progress; }
		}

		#endregion


	}
}
