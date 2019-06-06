using UnityEngine;
using System.Collections;

namespace TabTale {

	/// <summary>
	/// A Multi Publisher publishes any number of events to be handled by listeners. 
	/// TIdentifier is the type used to identify the different events handled by the 
	/// multi publisher, while THandler is the type of delegate expected when registering
	/// to events. For example, if we have an object that publishes events by name, it could
	/// implement IMultiPublisher<string, System.Action<string>>. This interface is built
	/// to accomodate a solution for "weak delegates" i.e., event handlers kept as 
	/// weak references.
	/// </summary>
	public interface IMultiPublisher<TIdentifier, THandler>
		where THandler : class
	{
		/// <summary>
		/// Subscribe to the event published by this publisher, of the type
		/// specified by THandler. Subscription of this type calls for a manual 
		/// removal (unsubscription).
		/// </summary>
		/// <param name="handler">The event handler delegate - can be a method, 
		/// or an anonymous function. If it is a lambda expression, removal must
		/// be done by calling UnSubscribe with the target reference - usually the
		/// target will be the object instace in whose method we created the lambda. </param>
		void Subscribe(TIdentifier id, THandler handler);
		
		/// <summary>
		/// Subscribe to the event published by this publisher, of the 
		/// type specified by THandler, while manually specifying the target object
		/// instance. Subscription of this type allows for automatic auto-removal
		/// of the handler.
		/// </summary>
		/// <param name="">.</param>
		/// <param name="handler">Handler.</param>
		void Subscribe(TIdentifier id, IRetainer<THandler> target, THandler handler);
		
		/// <summary>
		/// Manually unsubscribe the given event handler.
		/// </summary>
		/// <param name="handler">Handler.</param>
		void UnSubscribe(THandler handler);
		
		/// <summary>
		/// Manually unsubscribe every handler that has the given object as its target.
        /// </summary>
        /// <param name="target">Target.</param>
        void UnSubscribe(object target);

	}
}