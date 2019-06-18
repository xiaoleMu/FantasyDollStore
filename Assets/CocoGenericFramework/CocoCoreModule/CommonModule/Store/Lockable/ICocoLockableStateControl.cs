using UnityEngine;
using System.Collections;

namespace CocoPlay
{
	public interface ICocoLockableStateControl
	{
		string GetRVKey ();

		void OnRvReleased ();
	}
}
