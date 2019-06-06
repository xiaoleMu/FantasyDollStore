using UnityEngine;
using System.Collections;
using TabTale.SceneManager;

namespace TabTale
{
	public interface ITransform
	{
		Transform Transform { get; }
	}

	public interface ISceneController : ITransform
	{
		string SceneName { get; }
		IDispatcher Dispatcher { get; }
		bool AddExit(ISceneExit sceneExit);

		/// <summary>
		/// Will be called by the Application when it intends to unload the scene,
		/// ideally waiting for ReadToUnload to become true.
		/// </summary>
		void PrepareToUnload();

		bool ReadyToUnload { get; }

		/// <summary>
		/// Will be called by the Application whenever the scene is actually unloaded.
		/// to switch. 
		/// </summary>
		void OnSceneClosing();

		ITaskFactory TaskFactory { get; }

		ICoroutineFactory CoroutineFactory { get; }
	}
}
