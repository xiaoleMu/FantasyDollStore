using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class WaitTask : ITask
	{
		float _time;
		int _frames;
		ICoroutineFactory _coroutineFactory;
		public WaitTask(ICoroutineFactory coroutineFactory, float time, int frames)
		{
			_coroutineFactory = coroutineFactory;
			_time = time;
			_frames = frames;
		}

		IEnumerator Wait()
		{
			if(_time > 0)
				yield return new WaitForSeconds(_time);

			if(_frames > 0)
			{
				for(int i=0;i<_frames;i++)
					yield return null;
			}

			_state = TaskState.Done;
			Done(TaskEnding.Done);
		}

		#region ITask implementation

		public event System.Action<TaskEnding> Done = result => {};

		public ITask Start (System.Action<TaskEnding> resultHandler, float timeout = -1f)
		{
			Done += resultHandler;
			return Start (timeout);
		}

		public ITask Start (float timeout = -1f)
		{
			_state = TaskState.Started;
			_startTime = Time.realtimeSinceStartup;
			_coroutineFactory.StartCoroutine(Wait);
			return this;
		}

		public void Cancel(System.Action<TaskEnding> resultHandler)
		{
			Done += resultHandler;
			Cancel ();
		}

		public void Cancel ()
		{
		}

		public TaskEnding Handle ()
		{
			_state = TaskState.Started;
			IEnumerator enumerator = Wait ();
			while(enumerator.MoveNext())
			{
			}
			_state = TaskState.Done;
			Done(TaskEnding.Done);

			return TaskEnding.Done;
		}

		TaskState _state = TaskState.Ready;
		public TaskState State 
		{
			get { return _state; }
		}

		float _startTime;
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


		public float Progress 
		{
			get { return Mathf.Min((Time.realtimeSinceStartup - _startTime) / _time, 1f); }
		}

		#endregion
	}
}
