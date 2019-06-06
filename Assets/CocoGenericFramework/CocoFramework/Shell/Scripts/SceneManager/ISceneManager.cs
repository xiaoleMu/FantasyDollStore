using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale.SceneManager 
{
	public interface ISceneManager : IService
	{
		void SwitchScene(string newScene, IEnumerable<IEnumerableAction> preloadActions);
		void SwitchScene(string newScene);

		/// <summary>
		/// Called right after someone requests a scene transition (via a 
		/// call to Switch Scene)
		/// </summary>
		event System.Action<string> SceneRequested;

		/// <summary>
		/// Called right after the loading of a scene started (i.e.
		/// an actual call to Application.LoadLevel was performed).
		/// </summary>
		event System.Action<string> SceneLoadStarted;

		/// <summary>
		/// Called when a new scene was just loaded.
		/// </summary>
		event System.Action<string> SceneLoaded;

		event System.Action<string> TransitionOutStarted;
		event System.Action<string> TransitionOutEnded;
		event System.Action<string> TransitionInStarted;
		event System.Action<string> TransitionInEnded;

		ITask GetAdditiveLoader(string sceneName, SceneTransition sceneTransition = null, IEnumerable<GameObject> removals = null);

		ICollection<ITask> LoadActions { get; }

		string PendingLoad { get; }
	}

}
