using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace TabTale 
{
	public interface IServiceResolver
	{
		/// <summary>
		/// This method will return the module requested if and only if
		/// it can be retrieved and initialized synchronously.
		/// </summary>
		/// <returns>The module.</returns>
		/// <typeparam name="TModule">The 1st type parameter.</typeparam>
		T Get<T>() where T : IService;
		
		void Get<T>(System.Action<TaskEnding, T> handler) where T : IService;

		IEnumerable<IService> ActiveProviders { get; }
		IEnumerable<IService> Providers { get; }
		ITaskFactory TaskFactory { get; }
	}
}
