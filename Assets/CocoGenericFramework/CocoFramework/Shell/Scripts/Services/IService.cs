using UnityEngine;
using System.Collections;

namespace TabTale 
{
	/// <summary>
	/// A basic interface for all service providers - used to force them to be interfaces.
	/// </summary>
	public interface IService
	{
		/// <summary>
		/// This will be called by the kernel right after construction, and before inserting
		/// the module into the module container. A module should not access any other module
		/// or service during this step - only later, in the StartModule method. If the method
		/// returns true, it is added to the container. If it returns false, it is discarded.
		/// </summary>
		/// <param name="moduleContainer">Module container.</param>
		ITask GetInitializer(IServiceResolver moduleContainer);
	}
}