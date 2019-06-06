using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class CoroutineTask : CoroutineTask<object>, ITask
	{
		public CoroutineTask(ICoroutineFactory coroutineFactory, IEnumerableAction serialAction)
			: base(coroutineFactory, serialAction)
		{
		}
	}

	public class CoroutineTask<TResult> : ITask<TResult>
		where TResult : class
	{
		IEnumerableAction<TResult> _serialAction;
		int _countIterations = 0;

		protected CoroutineTask ()
		{
		}

		public virtual float Progress
		{
			get { return _serialAction.EstimateProgress(_countIterations); }
		}

		public event System.Action<TResult> ResultSet = result => {};

		protected void InvokeResultSet()
		{
			ResultSet(_result);
		}

		protected TResult _result = default(TResult);
		public TResult Result
		{
			get { return _result; }
		}
		
		string _name = "[Unknown]";
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		
		protected ICoroutineFactory _coroutineFactory;
		public CoroutineTask(ICoroutineFactory coroutineFactory, IEnumerableAction<TResult> serialAction)
		{
			Init (coroutineFactory, serialAction);
		}

		protected void Init (ICoroutineFactory coroutineFactory, IEnumerableAction<TResult> serialAction)
		{
			_name = serialAction.Name;
			_coroutineFactory = coroutineFactory;
			_serialAction = serialAction;
		}

		public float StartTime
		{
			get { return _startTime; }
		}
		
		float _startTime, _doneTime;
		public float DoneTime
		{
			get { return _doneTime; }
		}

		public float WorkTime
		{
			get { return _doneTime - _startTime; }
		}
		
		float _timeout = float.MaxValue;
		
		public ITask Start(float timeout = -1f)
		{
			return Start (result => {}, timeout);
		}

		public ITask Start(System.Action<TaskEnding> resultHandler, float timeout = -1f)
		{
			_taskState = TaskState.Started;

			_startTime = Time.time;
			Done += resultHandler;
			
			if(timeout > 0f)
				_timeout = _startTime + timeout;
			
			_coroutineFactory.StartCoroutine(Controller);
			
			return this;
		}
		
		public event System.Action<TaskEnding> Done = result => {};
		
		IEnumerator Controller()
		{
			bool running = true;			
			
			IEnumerator serialAction = null;
			
			try
			{
				Debugger.Assert(_serialAction != null);
				
				CoreLogger.LogTrace(LoggerModules.Tasks, string.Format("calling start of serial action on {0}", _name));
				serialAction = _serialAction.Run();
				_countIterations = 1;
				
				CoreLogger.LogTrace(LoggerModules.Tasks, string.Format("calling first MoveNext on {0}", _name));
				running = serialAction.MoveNext();
				CoreLogger.LogTrace(LoggerModules.Tasks, string.Format("returned from first MoveNext on {0}", _name));
				
			} catch(System.Exception ex)
			{
				if(ExceptionHandling == TaskFactory.ExceptionHandling.Raise)
				{
					throw ex;
				} else if(ExceptionHandling == TaskFactory.ExceptionHandling.Ignore)
				{
					CoreLogger.LogNotice(LoggerModules.Tasks, string.Format("exception raised by task [{0}]: {1})\nStack:\n{2}", Name, ex.Message, ex.StackTrace));
				} else if(ExceptionHandling == TaskFactory.ExceptionHandling.ReportAsError)
				{
					CoreLogger.LogError(LoggerModules.Tasks, string.Format("exception raised by task [{0}]: {1})\nStack:\n{2}", Name, ex.Message, ex.StackTrace));
				}
				
				Done(TaskEnding.Exception);
			}			

			while(running)
			{
				_countIterations++;
				
				if(Time.time > _timeout)
				{
					_taskState = TaskState.Done;
					_doneTime = Time.time;
					Done(TaskEnding.Timeout);
					yield break;
				}
				
				if(_cancelRequested)
				{
					_taskState = TaskState.Done;
					Done(TaskEnding.Cancelled);
					_doneTime = Time.time;
					yield break;
				}
				
				switch(_taskState)
				{
				case TaskState.Started:
					yield return serialAction.Current;
					try
					{
						CoreLogger.LogTrace(LoggerModules.Tasks, string.Format("calling MoveNext #{0} on {1}", _countIterations, _name));
						Debugger.Assert(serialAction != null);
						running = serialAction.MoveNext();
						CoreLogger.LogTrace(LoggerModules.Tasks, string.Format("returned from MoveNext #{0} on {1}", _countIterations, _name));
					} catch(System.Exception ex)
					{
						if(ExceptionHandling == TaskFactory.ExceptionHandling.Raise)
						{
							throw ex;
						} else if(ExceptionHandling == TaskFactory.ExceptionHandling.Ignore)
						{
							CoreLogger.LogNotice(LoggerModules.Tasks, string.Format("exception raised by task [{0}] during iteration #{2}: {1}, {3})", Name, ex.Message, _countIterations, ex.StackTrace));
						} else if(ExceptionHandling == TaskFactory.ExceptionHandling.ReportAsError)
						{
							CoreLogger.LogError(LoggerModules.Tasks, string.Format("exception raised by task [{0}] during iteration #{2}: {1}, {3})", Name, ex.Message, _countIterations, ex.StackTrace));							break;
						}
						
						Done(TaskEnding.Exception);
					}
					break;
					
				case TaskState.Paused:
					yield return null;
					break;
					
				case TaskState.Done:
					Debugger.Assert(false, "task should have stopped already!");
					break;
				}
			}
			
			_taskState = TaskState.Done;
			_doneTime = Time.time;
			Done(TaskEnding.Done);
		}
		
		public TaskFactory.ExceptionHandling ExceptionHandling = TaskFactory.ExceptionHandling.ReportAsError;
		
		TaskState _taskState = TaskState.Ready;
		public TaskState State
		{
			get { return _taskState; }
		}
		
		bool _cancelRequested = false;
		public void Cancel()
		{
			_cancelRequested = true;
		}

		public void Cancel(System.Action<TaskEnding> resultHandler)
		{
			Done += resultHandler;
			Cancel ();
		}
		
		public override string ToString ()
		{
			return string.Format ("[Task: Routine={0}, State={1}]", _serialAction, State);
		}
		
		public TaskEnding Handle()
		{
			_startTime = Time.realtimeSinceStartup;
			
			if(_serialAction == null)
			{
				return TaskEnding.Done;
			}
			
			CoreLogger.LogTrace(LoggerModules.Tasks, string.Format("starting serial action on {0}", _name));
			IEnumerator serialAction = _serialAction.Run();
			
			CoreLogger.LogTrace(LoggerModules.Tasks, string.Format("calling MoveNext on {0}", _name));
			bool continueRunning = serialAction.MoveNext();
			CoreLogger.LogTrace(LoggerModules.Tasks, string.Format("returned from MoveNext on {0}", _name));
			while(continueRunning)
			{
				if(Time.realtimeSinceStartup > _timeout)
				{
					_doneTime = Time.realtimeSinceStartup;
					return TaskEnding.Timeout;
				}
				
				CoreLogger.LogTrace(LoggerModules.Tasks, string.Format("calling MoveNext on {0}", _name));
				continueRunning = serialAction.MoveNext();
				CoreLogger.LogTrace(LoggerModules.Tasks, string.Format("returned from MoveNext on {0}", _name));
			}
			
			_doneTime = Time.realtimeSinceStartup;
			
			return TaskEnding.Done;
		}
	}
}
