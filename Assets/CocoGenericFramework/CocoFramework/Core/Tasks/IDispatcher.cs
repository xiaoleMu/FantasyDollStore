using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface IDispatcher
	{
		void Post(System.Action action, int minFrameDelay = 1);
		void Send(System.Action action);
	}
}
