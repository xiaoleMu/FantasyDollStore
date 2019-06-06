using UnityEngine;
using System.Collections;

namespace TabTale.SceneManager
{
	public interface ISceneExit
	{
		System.Func<IEnumerator> ExitSequence { get; }
	}
}
