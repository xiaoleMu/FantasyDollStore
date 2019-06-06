using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class SerializedTask : ITask
	{
		IEnumerable<ITask> _tasks;
		ICoroutineFactory _coroutineFactory;
		
		public SerializedTask(ICoroutineFactory coroutineFactory, IEnumerable<ITask> tasks)
		{
			_tasks = tasks;
			_coroutineFactory = coroutineFactory;

			_name = "serialized task";
		}
		
		#region ITask implementation
		
		public event System.Action<TaskEnding> Done = result => {};
		
		public ITask Start (System.Action<TaskEnding> resultHandler, float timeout = -1f)
		{
			Done += resultHandler;
			return Start (timeout);
		}

		bool _cancel;
		float _startTime;
		IEnumerator Controller()
		{
			_state = TaskState.Started;

			foreach(ITask task in _tasks)
			{
				bool done = false;
				task.Start(result => done = true);
				while(!done)
				{
					if(_cancel)
					{
						task.Cancel();
						Done(TaskEnding.Cancelled);
						_state = TaskState.Done;
						yield break;
					}

					if(Time.realtimeSinceStartup > _deadLine)
					{
						task.Cancel();
						Done(TaskEnding.Timeout);
						_state = TaskState.Done;
						yield break;
					}

					yield return null;
				}	

				yield return null;
			}

			Done(TaskEnding.Done);
			_state = TaskState.Done;
		}

		float _timeout = float.MaxValue;
		float _deadLine = float.MaxValue;
		public ITask Start (float timeout = -1f)
		{
			_timeout = timeout;
			_startTime = Time.realtimeSinceStartup;
			if(_timeout > 0)
				_deadLine = _startTime + _timeout;

			_coroutineFactory.StartCoroutine(() => Controller());
			return this;
		}

		public void Cancel(System.Action<TaskEnding> resultHandler)
		{
			Done += resultHandler;
			Cancel();
		}
		
		public void Cancel ()
		{
			_cancel = true;
		}
		
		public TaskEnding Handle ()
		{
			IEnumerator enumerator = Controller();
			while(enumerator.MoveNext())
			{
			}

			Done(TaskEnding.Done);
			return TaskEnding.Done;
		}

		TaskState _state = TaskState.Ready;
		public TaskState State 
		{
			get { return _state; }
		}
		
		public float StartTime 
		{
			get { return _startTime; }
		}

		string _name;
		public string Name 
		{
			get { return _name; }
			set { _name = value; }
        }

        #endregion

        #region IProgress implementation

		float _progress = 0;
        public float Progress 
		{
			get { return _progress; }
        }

        #endregion
    }
    
}
