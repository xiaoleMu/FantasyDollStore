using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.SceneManager 
{
	public class UnitySceneManager : ISceneManager
	{
		public IEnumerable<GameObject> Transitions
		{
			get { yield break; }
		}

		public ITask GetAdditiveLoader(string newScene, SceneTransition sceneTransition = null, IEnumerable<GameObject> removals = null)
		{
			AsyncOperation operation = Application.LoadLevelAdditiveAsync(newScene);
			operation.allowSceneActivation = false;
			operation.priority = 0;
			ITask task = _taskFactory.FromAsyncOperation(operation);
			task.Name = string.Format("load scene {0}", newScene);
			return task;
		}

		public event System.Action<string> TransitionOutStarted = s => {};
		public event System.Action<string> TransitionOutEnded = s => {};
		public event System.Action<string> TransitionInStarted = s => {};
		public event System.Action<string> TransitionInEnded = s => {};

		public string PendingLoad
		{
			get { return null; }
		}

		public void SwitchScene(string newScene, IEnumerable<IEnumerableAction> preloadActions)
		{
			GameApplication.Instance.CoroutineFactory.StartCoroutine(() => _SwitchScene(newScene, preloadActions));
		}

		IEnumerator PreloadActions(IEnumerable<IEnumerableAction> preloadActions)
		{
			foreach(IEnumerableAction action in preloadActions)
			{
				yield return action.Run();
			}
		}

		void HandlePreloadActions(IEnumerable<IEnumerableAction> preloadActions)
		{
			foreach(IEnumerableAction action in preloadActions)
			{
				IEnumerator enumerator = action.Run();
				while(enumerator.MoveNext())
				{
				}
			}
		}

		public bool asyncLoading = true;
		IEnumerator _SwitchScene(string newScene, IEnumerable<IEnumerableAction> preloadActions)
		{
			SceneRequested(newScene);

			if(asyncLoading)
			{
				yield return PreloadActions(preloadActions);

				CoreLogger.LogNotice("UnitySceneManager", string.Format("switching to scene {0}", newScene));

				SceneLoadStarted(newScene);

				yield return Application.LoadLevelAsync(newScene);

			} else
			{
				HandlePreloadActions(preloadActions);

				CoreLogger.LogNotice("UnitySceneManager", string.Format("switching to scene {0}", newScene));
				Application.LoadLevel(newScene);
				
				SceneLoadStarted(newScene);

				yield break;
			}
		}

		static IEnumerable<IEnumerableAction> s_emptyTaskList = new List<IEnumerableAction>();
		public void SwitchScene(string newScene)
		{
			SwitchScene(newScene, s_emptyTaskList);
		}

		public event System.Action<string> SceneRequested = (s) => {};
		public event System.Action<string> SceneLoadStarted = (s) => {};
		public event System.Action<string> SceneLoaded = (s) => {};

		#region IService implementation

		ITaskFactory _taskFactory;
		public ITask GetInitializer(IServiceResolver resolver)
		{
			_taskFactory = resolver.TaskFactory;
			return _taskFactory.FromEnumerableAction(Init);
		}
		
		IEnumerator Init()
		{
			yield break;
		}
		
		#endregion

		IList<ITask> _loadActions = new List<ITask>();
		public ICollection<ITask> LoadActions
		{
			get { return _loadActions; }
		}

		public void QueueLoadAction(ITask action)
		{
		}
	}
}
