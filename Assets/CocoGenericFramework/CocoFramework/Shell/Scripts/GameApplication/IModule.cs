using UnityEngine;
using System.Collections;

namespace TabTale 
{
	/// <summary>
	/// A module is a component that can be loaded by the Application kernel (GameApplication), 
	/// initialized, and update during the run of the game. It can be anything from a background
	/// process monitoring the filesystem, to an entire game. Usually a module will inherit from
	/// the Module class, which is a MonoBehvaviour and implements this interface, but sometimes
	/// we may use other classes as the base (hence the interface). 
	/// </summary>
	public interface IModule
	{
		/// <summary>
		/// This will be called by the kernel right after construction, and before inserting
		/// the module into the module container. A module should not access any other module
		/// or service during this step - only later, in the StartModule method. If the method
		/// returns true, it is added to the container. If it returns false, it is discarded.
		/// </summary>
		/// <param name="moduleContainer">Module container.</param>
		ITask GetInitializer(IModuleContainer moduleContainer);

		/// <summary>
		/// Starts the module - it is equivalent to the Start method of MonoBehaviours.
		/// </summary>
		void StartModule();

		/// <summary>
		/// Stops the module - the mirror image of Start.
		/// </summary>
		void StopModule();

		/// <summary>
		/// Terminates the module - the mirror image of Init.
		/// </summary>
		void Terminate();

		/// <summary>
		/// Called at every "tick" of the game loop, the equivalent of the MonoBehaviour
		/// Update method.
		/// </summary>
		/// <param name="timeDelta">Time (in seconds) since the last frame.</param>
		void UpdateModule(float timeDelta);

		/// <summary>
		/// This is the init stage during which the module wishes to load - basically, 
		/// only -1, 0, and 1 have meaning - it means "before everything", "don't care" and
		/// "after everything"
		/// </summary>
		/// <value>The stage.</value>
		int Stage { get; }
	}
}
