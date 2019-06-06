using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.SceneManager 
{
	public class SceneManager : MonoService, ISceneManager
	{	
		public SceneTransition defaultSceneTransition;

		public StringPair[] sceneMapping;
		DictionaryMapper<string, string> _sceneMapping;
		
		/// <summary>
		/// This is the time a scene is allowed to "prepare" once it is given the
		/// unload notification.
		/// </summary>
		public float sceneUnloadTimeout = 5.0f;

		public bool asyncLoading = false;
		public bool asyncPreloading = true;

		public IEnumerator PreloadActions(IEnumerable<IEnumerableAction> preloadActions)
		{
			foreach(IEnumerableAction action in preloadActions)
			{
				yield return action.Run();
			}
		}

		public string PendingLoad
		{
			get { return (_pendingLoad != null ? _pendingLoad.SceneName : null); }
		}
		
		public void HandlePreloadActions(IEnumerable<IEnumerableAction> preloadActions)
		{
			foreach(IEnumerableAction action in preloadActions)
			{
				IEnumerator enumerator = action.Run();
				while(enumerator.MoveNext())
				{
				}
			}
		}

		public float additiveLoaderWatermark = 0.9f;

		ISceneLoaderTask _pendingLoad;
		public ISceneLoaderTask PendingLoadTask
		{
			get { return _pendingLoad; }
			set { _pendingLoad = value; }
		}

		public float additiveLoaderDelay = 1;

		public ITask GetAdditiveLoader(string sceneName, SceneTransition sceneTransition = null, IEnumerable<GameObject> removals = null)
		{
			if(sceneTransition == null)
			   sceneTransition = defaultSceneTransition;

//FIXME: Implement Additive for non unity pro
#if UNITY_PRO_LICENSED
			ITask task = new AdditiveSceneLoaderTask(this, sceneName, additiveLoaderWatermark, sceneTransition, removals, additiveLoaderDelay);

			task.Done += result => {
				if(result.IsOk())
				{
					Logger.LogDebug(LoggerModules.SceneManager, string.Format("loaded scene {0}", sceneName));
					SceneLoaded(sceneName);
				}
			};

			return task;
#else
			CoreLogger.LogWarning("Warning - attempting to additively load a scene without Unity Pro License - currently unsupported action");
			return new SceneLoaderTask(this, sceneName, sceneTransition, null);
#endif
		}

		public void HookTransition(SceneTransition transition)
		{
			if(transition == null)
				return;
			transition.StartTransitionOut += OnTransitionOutStarted;
			transition.StartTransitionIn += OnTransitionInStarted;
			transition.EndTransitionOut += OnTransitionOutEnded;
			transition.EndTransitionIn += OnTransitionInEnded;            
        }

		public void UnhookTransition(SceneTransition transition)
		{
			if(transition == null)
				return;
			transition.StartTransitionOut -= OnTransitionOutStarted;
			transition.StartTransitionIn -= OnTransitionInStarted;
			transition.EndTransitionOut -= OnTransitionOutEnded;
			transition.EndTransitionIn -= OnTransitionInEnded;            
        }
        
        public ISceneLoaderTask GetLoader(string sceneName, SceneTransition sceneTransition = null, IEnumerable<IEnumerableAction> preloadActions = null)
		{
			if(sceneTransition == null)
				sceneTransition = defaultSceneTransition;

			ISceneLoaderTask task = new SceneLoaderTask(this, sceneName, sceneTransition, preloadActions);
			task.Done += result => {
				if(result.IsOk())
				{
					CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("loaded scene {0}", sceneName));
					SceneLoaded(sceneName);
				}
			};

			return task;
		}

		static IEnumerable<IEnumerableAction> s_emptyTaskList = new List<IEnumerableAction>();
		public void SwitchScene(string newScene)
		{
			SwitchScene(newScene, s_emptyTaskList);
		}

		ITask _sceneSwitch;

		//FIXME queue all scene switches, whether additive or not, as tasks

		/// <summary>
		/// Switches to the scene specified by newScene. The sequence of operation is:
		/// first the current scene receives a notification that it is about to be unloaded.
		/// It then has time to prepare and do whatever it needs, until the timeout passes,
		/// or it signals that it is ready by setting its ReadyToUnload flag to true.
		/// Once either happens, the old scene receives a SceneClosing notification, 
		/// and the loading of the new scene begins.
		/// Note that Unity does not guaranteee anything concerning the actual time it
        /// takes to load/unload a scene - once the call to LoadLeve is made, the scenes
        /// should consider themselves to be in Limbo and presume nothing.
        /// </summary>
        /// <param name="newScene">Name of the new scene to switch to.</param>
        /// <param name="handovers">If passed, contains a collection of objects that will
        /// be preserved for passing on to the next scene </param>
		public void SwitchScene(string newScene, IEnumerable<IEnumerableAction> preloadActions)
        {
			string realScene = _sceneMapping[newScene];
			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("requested scene {0} (mapped to {1})", newScene, realScene));

			SceneRequested(realScene);

			GetLoader(realScene, defaultSceneTransition, preloadActions).Start();
        }

		void Awake()
		{
			_sceneMapping = new DictionaryMapper<string, string>(k => k);
			foreach(StringPair stringPair in sceneMapping)
			{
				_sceneMapping[stringPair.First] = stringPair.Second;
			}
		}

		void OnTransitionOutStarted(SceneTransition sceneTransition)
		{
			CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("scene manager received start transition out event"));
			TransitionOutStarted(SceneController.Current == null ? "<unknown>" : SceneController.Current.SceneName);
		}

		void OnTransitionInStarted(SceneTransition sceneTransition)
		{
			CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("scene manager received start transition in event"));
			TransitionInStarted(SceneController.Current == null ? "<unknown>" : SceneController.Current.SceneName);
		}

		void OnTransitionOutEnded(SceneTransition sceneTransition)
		{
			CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("scene manager received end transition out event"));
			TransitionOutEnded(SceneController.Current == null ? "<unknown>" : SceneController.Current.SceneName);
		}

		void OnTransitionInEnded(SceneTransition sceneTransition)
		{
			CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("scene manager received end transition in event"));
			TransitionInEnded(SceneController.Current == null ? "<unknown>" : SceneController.Current.SceneName);
			UnhookTransition(sceneTransition);
		}

		public event System.Action<string> SceneRequested = s => {};
		public event System.Action<string> SceneLoadStarted = s => {};
		public event System.Action<string> SceneLoaded = s => {};
		public event System.Action<string> TransitionOutStarted = s => {};
		public event System.Action<string> TransitionOutEnded = s => {};
		public event System.Action<string> TransitionInStarted = s => {};
		public event System.Action<string> TransitionInEnded = s => {};

		protected override IEnumerator InitService (IServiceResolver resolver)
		{
			_taskFactory = resolver.TaskFactory;
			yield break;
		}	
        
		IList<ITask> _loadActions = new List<ITask>();
		public ICollection<ITask> LoadActions
		{
			get { return _loadActions; }
		}

		public void QueueLoadAction(ITask action)
		{
			_loadActions.Add(action);
		}
	}
}