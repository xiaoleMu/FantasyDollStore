using UnityEngine;
using System.Collections;

namespace TabTale.SceneManager
{
	public interface ISceneLoaderTask : ITask
	{
		string SceneName { get; }
	}
}