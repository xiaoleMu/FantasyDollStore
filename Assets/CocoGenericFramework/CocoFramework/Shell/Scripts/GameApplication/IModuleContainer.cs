using UnityEngine;
using System.Collections;

namespace TabTale
{
	/*public interface IResolver<TBase>
	{
		/// <summary>
		/// This method will return the module requested if and only if
		/// it can be retrieved and initialized synchronously.
		/// </summary>
		/// <returns>The module.</returns>
		/// <typeparam name="TModule">The 1st type parameter.</typeparam>
		T Get<T>() where T : TBase;

		void Get<T>(System.Action<TaskResult, T> handler) where T : TBase;
	}*/

	public interface IModuleContainer
	{
		/// <summary>
		/// This method will return the module requested if and only if
		/// it can be retrieved and initialized synchronously.
		/// </summary>
		/// <returns>The module.</returns>
		/// <typeparam name="TModule">The 1st type parameter.</typeparam>
		T Get<T>() where T : IModule;
		
		void Get<T>(System.Action<TaskEnding, T> handler) where T : IModule;

		IServiceResolver ServiceResolver { get; }
		ITaskFactory TaskFactory { get; }
	}
}
