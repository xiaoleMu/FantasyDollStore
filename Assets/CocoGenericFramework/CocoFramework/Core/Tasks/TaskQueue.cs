using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale 
{
	public class TaskQueue : TaskFactory, ITaskQueue
	{
		private Queue<QueueTask> _tasks = new Queue<QueueTask>();
		private Object _lock = new Object();

		private bool _running = false;
		private bool _active = true;
		public bool Active
		{
			get { return _active; }
			set { _active = value; }
		}

		private class QueueTask : CoroutineTask
		{
			public QueueTask(TaskFactory taskFactory, IEnumerableAction serialAction)
				: base(taskFactory, serialAction)
			{
			}

			public float RequestedTimeout = -1f;
		}

		private IEnumerator Run()
		{
			_running = true;

			while(true)
			{
				if(!_active)
					yield return null;

				_current = NextTask();

				if(_current == null)
				{
					CoreLogger.LogTrace(name, "no task in queue - calling QueueEmpty and yielding");
					QueueEmpty();
					yield return null;
				} else
				{
					CoreLogger.LogTrace(name, "starting next task...");

					CoreLogger.LogInfo(name, string.Format("starting task...{0}", _current.Name));

					bool notDone = true;
					_current.Start(result => {
						CoreLogger.LogInfo(name, string.Format("done with task {0}, result: {1}, time: {2}", _current.Name, result, _current.DoneTime));
						notDone = false;
					}, _current.RequestedTimeout);
					
					while(notDone)
					{
						yield return null;
					}
					
					_current = null;
				}
			}
		}

		public bool autoStart = true;

		public void Start()
		{
			if(autoStart)
				ManualStart();
		}

		IEnumerator _loop;

		public void HandleAll()
		{
			ITask task;
			lock(_lock)
			{
				task = NextTask();
			}

			while(task != null)
			{
				task.Handle();
				task = NextTask();
			}

			QueueEmpty();
		}

		private QueueTask NextTask()
		{
			QueueTask task = null;
			lock(_lock)
			{
				if(_tasks.Count > 0)
				{
					task = _tasks.Dequeue();
				}
			}

			return task;
		}

		public virtual void ManualStart()
		{
			if(!_running)
			{
				_loop = Run ();
				StartCoroutine(_loop);
			} else
			{
				CoreLogger.LogNotice(name, "TaskQueue.ManualStart called when queue already handling!");
			}
		}

		private QueueTask _current;

		public ITask Enqueue(IEnumerableAction enumerableAction)
		{
			QueueTask task = new QueueTask(this, enumerableAction);
			
			lock(_lock)
			{
				_tasks.Enqueue(task);
			}
			
			return task;
		}

		public ITask Enqueue(System.Func<IEnumerator> enumerableAction, string name = "")
		{
			return Enqueue(EnumerableAction.Create(enumerableAction, 1, name));
		}

		public int Count
		{
			get
			{
				int count;
				lock(_lock)
				{
					count = _tasks.Count + (_current != null ? 1 : 0);
				}

				return count;
			}
		}

		public ITask Enqueue(System.Action action, string name = "")
		{
			return Enqueue(() => EnumerableAction.WrapAction(action), name);
		}

		public event System.Action QueueEmpty = () => {};

		public ITask Parallelize(System.Func<bool> poller, float timeout = -1f, string name = "")
		{
			IEnumerableAction action = EnumerableAction.Create(() => WaitOnCondition(poller), 1, name);
			QueueTask task = new QueueTask(this, action) { RequestedTimeout = timeout };
			
			lock(_lock)
			{
				_tasks.Enqueue(task);
			}
			
			return task;
		}

		public override ITask FromPredicate (System.Func<bool> poller, string name)
		{
			return Enqueue(EnumerableAction.Create(poller));
		}
	}
}
