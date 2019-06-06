using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale.SceneManager
{
	public class SceneLoaderTask : ControllerTask, ISceneLoaderTask
	{
		#region ISceneLoaderTask implementation
		
		public string SceneName 
		{
			get { return _sceneName; }
		}
		
		#endregion

		string _sceneName;
		SceneManager _sceneManager;
		SceneTransition _sceneTransition;
		IEnumerable<IEnumerableAction> _preloadActions;
		public SceneLoaderTask(SceneManager sceneManger, string sceneName, SceneTransition sceneTransition, IEnumerable<IEnumerableAction> preloadActions)
		{
			_sceneName = sceneName;
			_sceneManager = sceneManger;
			_sceneTransition = sceneTransition;
			_preloadActions = preloadActions;

			GameApplication.Instance.ApplicationEvents.LevelLoaded += OnLevelLoaded;

			_name = string.Format("loading scene {0}", sceneName);
		}

		void OnLevelLoaded(int level)
		{
			float loadTime = Time.realtimeSinceStartup - _levelLoadStart;
			int loadFrames = Time.frameCount - _levelLoadStartFrame;
			_levelLoadStart = -1f;
			_levelLoadStartFrame = -1;
			
			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("requested scene {0} was loaded; loading took {1} seconds and {2} frames", _sceneName, loadTime, loadFrames));				
		}

		#region ITask implementation

		override public void Cancel ()
		{
			CoreLogger.LogError(LoggerModules.SceneManager, string.Format("cancel requested on loading of scene {0}, but is not supported!", _sceneName));
		}

		#endregion

		public void InjectTransition(SceneTransition transition, ISceneController sceneController)
		{
			GameObject newInstance = GameObject.Instantiate(transition.gameObject) as GameObject;
			IEnumerable<SceneTransition> sceneTransitions = newInstance.GetComponents<SceneTransition>();
			_sceneManager.HookTransition(sceneTransitions.FirstOrDefault());

			GameObject.DontDestroyOnLoad(newInstance);
			newInstance.transform.parent = sceneController.Transform;
			
			foreach(SceneTransition sceneTransition in sceneTransitions)
			{
				CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("\tinjecting effect {0}", sceneTransition.GetType().Name));
			}
		}

		public event System.Action SceneLoadStarted = () => {};

		float _levelLoadStart;
		int _levelLoadStartFrame;
		AsyncOperation _asyncLoadOperation;
		override protected IEnumerator Controller()
		{
			ISceneLoaderTask pending = _sceneManager.PendingLoadTask;
			if(pending != null)
			{
				if(pending.SceneName == _sceneName)
				{
					CoreLogger.LogNotice(LoggerModules.SceneManager, string.Format("{0}: pending request for the same scene ({1}); ending operation", _name, _sceneName));
					InvokeDone(TaskEnding.Done);
					yield break;
				}
			}

			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("setting pending load to {0}", _name));
			_sceneManager.PendingLoadTask = this;

			if(pending != null)
			{
				CoreLogger.LogNotice(LoggerModules.SceneManager, string.Format("{0}: waiting on loader {1}...", _name, pending.Name));
				yield return GameApplication.Instance.CoroutineFactory.Wait(pending);
				CoreLogger.LogNotice(LoggerModules.SceneManager, string.Format("{0}: waiting on loader {1} done!", _name, pending.Name));
			}

			float startTime = Time.realtimeSinceStartup;
			
			//check to see if the current scene has a TabTale controller (all scenes are supposed to have one)
			ISceneController currentSceneController = SceneController.Current;
			
			if(currentSceneController != null)
			{
				if(_sceneTransition != null)
				{
					if(currentSceneController == null)
					{
						CoreLogger.LogNotice(LoggerModules.SceneManager, "no controller found in scene - injection of transitions is not possible!");
					} else
					{
						CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("injecting effect {0} into the scene", _sceneTransition.name));
						InjectTransition(_sceneTransition, currentSceneController);
					}
				}
				
				//tell the current scene it is about to be unloaded
				currentSceneController.PrepareToUnload();
				
				CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format ("{0}: waiting for current scene {1} to be ready to unload", _name, currentSceneController.SceneName));
				
				//async. waiting for either the old scene's signal that it's ready, or the timeout
				
				while(!currentSceneController.ReadyToUnload && (Time.realtimeSinceStartup - startTime <= _sceneManager.sceneUnloadTimeout))
				{
					yield return null;
				}
				
				CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("scene {0} done - wait took {1} seconds", currentSceneController.SceneName, Time.realtimeSinceStartup - startTime));
				
				//tell the current scene it is being unloaded
				currentSceneController.OnSceneClosing();
			} else
			{
				CoreLogger.LogWarning(LoggerModules.SceneManager, "Unable to find TabTale Scene Controller!");
			}			
			
			_levelLoadStart = Time.realtimeSinceStartup;
			_levelLoadStartFrame = Time.frameCount;
			
			if(_sceneManager.asyncPreloading)
			{
				yield return _sceneManager.PreloadActions(_preloadActions);
			} else
			{
				_sceneManager.HandlePreloadActions(_preloadActions);
			}
			
			if(_sceneManager.asyncLoading && !Application.platform.IsEditor())
			{
				CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("Loading new scene {0} asynchronously...", _sceneName));
				
				SceneLoadStarted();

				_asyncLoadOperation = Application.LoadLevelAsync(_sceneName);
				yield return _asyncLoadOperation;
				_asyncLoadOperation = null;
				
			} else
			{
				//start loading the new scene - note that even though this is a blocking call,
				//it actually starts a background thing, and from this moment on everything
				//is in Limbo
				Application.LoadLevel(_sceneName);
				_progress = 1;
				
				CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("Loading new scene {0} synchronously...", _sceneName));

				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				
				SceneLoadStarted();
			}

			GameApplication.Instance.ApplicationEvents.LevelLoaded -= OnLevelLoaded;

			if(_sceneManager.PendingLoadTask == this)
			{
				CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("resetting pending load from {0} to null", _name));
				_sceneManager.PendingLoadTask = null;
			}
		}
	}
}
