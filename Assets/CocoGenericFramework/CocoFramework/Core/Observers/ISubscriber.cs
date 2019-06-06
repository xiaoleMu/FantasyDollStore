using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale {

	/// <summary>
	/// This is the interface which allows for weak event delegates. A class which
	/// watns to subscribe to events through lambda expressions, and still enjoy 
	/// auto-release (i.e. no nedd to unsubscribe itself), should implement this 
	/// interface.
	/// </summary>
	public interface ISubscriber<THandler>
		where THandler : class
	{
		IRetainer<THandler> Retainer { get; }
	}
}
