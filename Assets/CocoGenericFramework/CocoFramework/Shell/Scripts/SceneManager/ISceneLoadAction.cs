using UnityEngine;
using System.Collections;

namespace TabTale.SceneManager
{
	public interface ISceneLoadAction
	{
		System.Func<IEnumerator> Action { get; }
		float Progress { get; }
		string Name { get; }
	}
}
