using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class AsyncOperationTask : ControllerTask
	{
		System.Func<AsyncOperation> _operationFactory;
		AsyncOperation _operation;
#pragma warning disable 414
		ICoroutineFactory _coroutineFactory;
#pragma warning restore 414

		public AsyncOperationTask(ICoroutineFactory coroutineFactory, System.Func<AsyncOperation> operationFactory, string name = "")
		{
			_operationFactory = operationFactory;
			_coroutineFactory = coroutineFactory;
			_name = name;
		}

		public AsyncOperationTask(ICoroutineFactory coroutineFactory, AsyncOperation operation, string name = "")
		{
			_operation = operation;
			_name = name;
			_coroutineFactory = coroutineFactory;
		}

		override protected IEnumerator Controller()
		{
			_startTime = Time.realtimeSinceStartup;
			if(_operation == null)
				_operation = _operationFactory();

			bool done = false;

			while(!done)
			{
				if((Time.realtimeSinceStartup - _startTime >= _timeout) && (_timeout >= 0))
				{
					InvokeDone(TaskEnding.Timeout);
					yield break;
				}

				yield return null;

				done = _operation.isDone;
			}
		}
		
		#region ITask implementation	
		
		override public void Cancel ()
		{
		}
		
		override public TaskEnding Handle ()
		{
			_startTime = Time.realtimeSinceStartup;
			_operation = _operationFactory();

			while(!_operation.isDone)
			{
			}
			
			return TaskEnding.Done;
		}
		
		#endregion
	}


}
