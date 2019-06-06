using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace TabTale 
{
	public abstract class TaskFactory : CoroutineFactory, ITaskFactory
	{
		protected IEnumerator WaitOnCondition(System.Func<bool> poller)
		{
			while(!poller())
			{
				yield return null;
			}
		}

		public enum ExceptionHandling
		{
			ReportAsError, 
			Ignore, 
			Raise
		}
		
		public ExceptionHandling exceptionHandling = ExceptionHandling.ReportAsError;

		public ITask NullTask
		{
			get { return FromPredicate(() => true, "NullTask"); }
		}

		IEnumerator StartParallelTasks(ICollection<ITask> tasks)
		{
			int numTasks = tasks.Count;
			int numDone = 0;
			foreach(ITask task in tasks)
			{
				task.Start(result => {
					numDone++;
				});
			}
			
			while(numDone < numTasks)
			{
				yield return null;
			}
		}

		public ITask Parallelize(ICollection<ITask> tasks)
		{
			string name = string.Format("parallelization of {0} tasks", tasks.Count);
			IEnumerableAction serialAction = EnumerableAction.Create(() => StartParallelTasks(tasks), tasks.Count, name);
			return this.FromEnumerableAction(serialAction);
		}

		public ITask<T> FromEnumerableAction<T>(IEnumerableAction<T> action)
			where T : class
		{
			CoroutineTask<T> task = new CoroutineTask<T>(this, action);
			task.ExceptionHandling = this.exceptionHandling;
			
			return task;
		}

		public ITask FromEnumerableAction(System.Func<IEnumerator> routine)
		{
			return FromEnumerableAction(EnumerableAction.Create(routine));
		}
		
		public virtual ITask FromPredicate(System.Func<bool> poller, string name = "")
		{
			return FromEnumerableAction(() => WaitOnCondition(poller));
		}

		#region ITaskFactory Implementation

		public ITask Serialize(IEnumerable<ITask> tasks)
		{
			return new SerializedTask(this, tasks);
		}

		public ITask FromAsyncOperation(System.Func<AsyncOperation> operationFactory)
		{
			return new AsyncOperationTask(this, operationFactory);
		}

		public ITask FromAsyncOperation(AsyncOperation operation)
		{
			return new AsyncOperationTask(this, operation);
		}

		public ITask GetWaiter(float time, int frames)
		{
			return new WaitTask(this, time, frames);
		}

		#endregion
	}
}
