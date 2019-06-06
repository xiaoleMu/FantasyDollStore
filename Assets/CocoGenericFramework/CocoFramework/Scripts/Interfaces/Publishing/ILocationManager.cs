using UnityEngine;
using System.Collections;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TabTale.Publishing
{
	public struct LocationResult
	{
		public ApplicationLocation location;
		public bool sourceAssigned;
		public bool success;
		public bool playsMusic;
		public string message;
	}

	public interface ILocationManager
	{
		LocationLoadedSignal locationLoadedSignal { get; set; }
		LocationClosedSignal locationClosedSignal { get; set; }

		bool IsReady(ApplicationLocation location);
		bool IsViewVisible ();
		IPopupHandle Get(ApplicationLocation location);
		IPromise<LocationResult> Show(ApplicationLocation location);


	}
}
