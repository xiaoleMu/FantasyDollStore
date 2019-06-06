using UnityEngine;
using System;
using System.Collections;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using strange.extensions.signal.impl;
using TabTale.Publishing;

namespace TabTale
{
	/// <summary>
	/// The IPopupHandle interface is used for publishing popups.
	/// Currently, since psdk do not support Close option it does not implemenent the IModalHandle interface,
	/// and is not part of the modality system.
	/// </summary>
	public interface IPopupHandle
	{
		Signal<LocationResult> Closed { get; }


		ApplicationLocation Location { get; set;}
		string Name { get; }

		void Open (float timeout = -1);
		void Open (Action<LocationResult> resultCallback, float timeout = -1);

		bool IsReady();

	}
}
