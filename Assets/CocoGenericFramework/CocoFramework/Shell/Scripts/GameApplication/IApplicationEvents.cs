using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface IApplicationEvents
	{
		event System.Action GainedFocus;
		event System.Action LostFocus;
		event System.Action Paused;
		event System.Action<float> Resumed;
		event System.Action Quit;
		event System.Action<int> LevelLoaded;
	}
}
