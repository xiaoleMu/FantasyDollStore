using UnityEngine;
using System.Collections;

namespace TabTale {

	/// <summary>
	/// A TabTale application emits various events at various points - session start, 
	/// scene load, purchase etc. - a provider of this service listens on these
	/// events and does things with them...analytics, commercials, etc.
	/// </summary>
	public interface IApplicationEventsListener : IService
	{
		void ReportEvent(ApplicationLocation applicationEvent, object payload = null);

		event System.Action<ApplicationLocation, object> EventReported;
	}
}
