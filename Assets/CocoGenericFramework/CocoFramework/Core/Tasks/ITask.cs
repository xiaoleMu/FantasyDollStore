using UnityEngine;
using System.Collections;

namespace TabTale 
{
	public enum TaskState
	{
		/// <summary>
		/// Task has been created, but not yet running
		/// </summary>
		Ready, 

		/// <summary>
		/// Task has started, and is actively running
		/// </summary>
		Started, 
		
		/// <summary>
		/// Task has started, but has been paused
		/// </summary>
		Paused, 
		
		/// <summary>
		/// Task is stopped - either not started, or finished
		/// </summary>
		Done
	}

	public static class TaskStateExtensions
	{
		public static bool IsRunning(this TaskState _this)
		{
			return (_this == TaskState.Started) || (_this == TaskState.Paused);
		}

		public static bool IsDone(this TaskState _this)
		{
			return _this == TaskState.Done;
		}
	}
	
	public enum TaskEnding
	{
		/// <summary>
		/// Task was cancelled by a call to Cancel
		/// </summary>
		Cancelled, 
		
		/// <summary>
		/// Task has been stopped due to a timeout
		/// </summary>
		Timeout,
		
		/// <summary>
		/// Task has finished
		/// </summary>
		Done, 
		
		/// <summary>
		/// Task has raised an exception and was stopped
		/// </summary>
		Exception
	}

	public static class TaskEndingExtensions
	{
		public static bool IsOk(this TaskEnding _this)
		{
			return _this == TaskEnding.Done;
		}
	}
	
	public interface ITask : IProgress
	{
		/// <summary>
		/// Current state of the task
		/// </summary>
		/// <value>The state.</value>
		TaskState State { get; }

		ITask Start(System.Action<TaskEnding> resultHandler, float timeout = -1f);
		ITask Start(float timeout = -1f);

		event System.Action<TaskEnding> Done;
		
		/// <summary>
		/// Cancel the task and stop it from running at the next available
		/// opportunity
		/// </summary>
		/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
		void Cancel();

		void Cancel(System.Action<TaskEnding> resultHandler);
		
		/// <summary>
		/// Handle the task immediately - this is a blocking call that will return when
		/// the task is done
		/// </summary>
		TaskEnding Handle();
		
		/// <summary>
		/// Start Time of the task, in Unity frame time.
		/// </summary>
		/// <value>The start time.</value>
		float StartTime { get; }

		string Name { get; set; }
	}

	public interface ITask<TResult> : ITask
		where TResult : class
	{
		/// <summary>
		/// The resource being requested - starts as null and stays that way until
		/// a resource has actually been loaded.
		/// </summary>
		/// <value>The resource.</value>
		TResult Result { get; }
		
		/// <summary>
		/// This event will be called if and when a result is set, right after the call
		/// to the Done event. This is sugar for reading the result of Done and then
		/// accessing Result.
		/// </summary>
		event System.Action<TResult> ResultSet;
	}
}
