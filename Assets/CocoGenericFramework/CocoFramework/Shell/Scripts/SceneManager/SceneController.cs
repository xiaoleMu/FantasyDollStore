using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale.SceneManager 
{
	public enum SceneType
	{
		Technical, 
		Menu, 
		Gameplay, 
		Advertising, 
		Testing
	}

	public class SceneController : TaskFactory, IResourceLibrary, ISceneController, IBackButtonListener
	{
		public bool handleBackButton = true;

		static IList<SceneController> s_activeControllers = new List<SceneController>();

		#region ISceneController implementation

		public string sceneName;
		public string SceneName
		{
			get
			{
				if(sceneName == null)
					return Application.loadedLevelName;
				
				if(sceneName == "")
					return Application.loadedLevelName;
				
				return sceneName;
			}
		}

		public IDispatcher Dispatcher
		{
			get { return this; }
		}

		ICollection<ISceneExit> _pendingExits = new List<ISceneExit>();
		public bool AddExit(ISceneExit exit)
		{
			if(_inExit)
				return false;
			
			_pendingExits.Add(exit);
			return true;
		}

		public void PrepareToUnload()
		{
			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("scene {0} received PrepareToUnload - calling Terminate", SceneName));
			
			StartCoroutine(Terminate ());
		}

		public Transform Transform
		{
			get { return this.transform; }
		}

		bool _readyToUnload = false;
		public bool ReadyToUnload
		{
			get { return _readyToUnload; }
		}

		public void OnSceneClosing()
		{
			if(_readyToUnload)
			{
				CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("scene {0} received SceneClosing", SceneName));
			} else
			{
				CoreLogger.LogWarning(LoggerModules.SceneManager, string.Format("scene {0} received SceneClosing without being ready!", SceneName));
				
				//FIXME: better terminate all exits and stuff?
			}
		}

		public ITaskFactory TaskFactory
		{
			get { return this; }
        }

		public ICoroutineFactory CoroutineFactory
		{
			get { return this; }
		}

		#endregion

		#region IResourceLibrary implementation

		public T GetResource<T> (string name) where T : Object
		{
			return GetResource<T>(name, false);
		}

		public T GetResource<T>(string name, bool isCascading) where T : Object
		{
			foreach(IResourceLibrary lib in _resourceLibraries)
			{
				T resource = lib.GetResource<T>(name);
				if(resource != null)
					return resource;
			}
			
			return null;
		}

		#endregion

		ICollection<ISceneEntry> _pendingEntries = new List<ISceneEntry>();
		public bool AddEntry(ISceneEntry entry)
		{
			if(_inEntry)
				return false;

			_pendingEntries.Add(entry);
			return true;
		}

		public SceneType sceneType = SceneType.Gameplay;

		IList<IResourceLibrary> _resourceLibraries;

		static WeakReference<SceneController> s_current = new WeakReference<SceneController>(null);
		public static ISceneController Current
		{
			get 
			{ 
				if(s_current.Target == null)
					return GameApplication.Instance;

				return s_current.Target; 
			}
		}

		public string forceInitScene = "";
		public static bool s_jumpToInitOnTestScenes = true;

		override protected void Awake()
		{
			base.Awake();

			if(s_activeControllers.Count > 0)
			{
				if(s_activeControllers.FirstOrDefault(c => c.SceneName == SceneName) != null)
				{
					CoreLogger.LogWarning(LoggerModules.SceneManager, string.Format("scene controller with name {0} already active!", SceneName));
                } else
				{
					CoreLogger.LogWarning(LoggerModules.SceneManager, string.Format("scene controller {0} awakening while there are other active controllers!", SceneName));
				}
			}

			s_activeControllers.Add(this);

			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("scene controller for scene {0} awakening and setting itself current instead of {1}", 
			                                                          SceneName, s_current.Target == null ? "null" : s_current.Target.SceneName));
			s_current.Target = this;

#if UNITY_EDITOR
			//a testing scene would not be a part of the build
			if(sceneType == SceneType.Testing)
			{
				if(s_jumpToInitOnTestScenes)
				{
					s_jumpToInitOnTestScenes = false;
					string temp = forceInitScene;
					forceInitScene = "";
					GameApplication.RequestingScene = Application.loadedLevelName;
					Application.LoadLevel(temp);
					return;
				}
			}

			if(Application.loadedLevel != 0 && GameApplication.RequestingScene=="")
			{
				GameApplication.RequestingScene = Application.loadedLevelName;

				if(forceInitScene != "")
				{
					Application.LoadLevel(forceInitScene);
				} else
				{
					Application.LoadLevel(0);
				}
			}
#endif
		}

		void Start()
		{
			StartCoroutine(Init());
		}

		void OnDestroy()
		{
			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("scene controller {0} going down!", SceneName));
			s_activeControllers.Remove(this);
		}

		bool _readyToRun = false;
		public bool ReadyToRun
		{
			get { return _readyToRun; }
		}

		bool _inEntry = false;
		IEnumerator Init()
		{
			float startTime = Time.realtimeSinceStartup;
			int startFrame = Time.frameCount;

			IEnumerable<ISceneEntry> entries = GetComponentsInChildren(typeof(ISceneEntry)).Cast<ISceneEntry>()
				.Concat(_pendingEntries);

			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("scene {0} Starting Init with {1} scene entries", SceneName, entries.Count()));

			//make sure all scene initializations are done
			_inEntry = true;
			while(_inEntry)
			{
				bool entering = false;
				foreach(ISceneEntry entry in entries)
				{
					if(!entry.Done)
					{
						entering = true;
						break;
					}
				}
				
				_inEntry = entering;
				
				yield return null;
			}

			_readyToRun = true;

			_resourceLibraries = GetComponentsInChildren(typeof(IResourceLibrary)).Cast<IResourceLibrary>().ToList();

			_inEntry = false;
			_pendingEntries.Clear();

			if(handleBackButton && GameApplication.Instance != null)
				SubscribeToBackButtonEvent();

			SceneStarted();

			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("Init Done - took {0} seconds, {1} frames", Time.realtimeSinceStartup - startTime, Time.frameCount - startFrame));
		}

		bool _inExit = false;
		IEnumerator Terminate()
		{
			float startTime = Time.realtimeSinceStartup;
			int startFrame = Time.frameCount;

			IEnumerable<ISceneExit> exits = GetComponentsInChildren(typeof(ISceneExit)).Cast<ISceneExit>()
				.Concat(_pendingExits);

			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("scene {0} Starting Terminate with {1} exits", SceneName, exits.Count()));

			UnSubscribeFromBackButtonEvent();

			yield return GameApplication.Instance.CoroutineFactory.StartCoroutine(EnumerableAction.Parallelize(exits.Select(e => e.ExitSequence)));

			_readyToUnload = true;
			_inExit = false;
			_pendingExits.Clear();

			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("Terminate Done in {0} seconds, {1} frames", Time.realtimeSinceStartup - startTime, Time.frameCount - startFrame));
		}

		public event System.Action SceneStarted = () => {};

		#region IBackButtonListener implementation

		public void SubscribeToBackButtonEvent ()
		{
			GameApplication.Instance.BackButtonService.AddListener(this);
		}
		
		public void UnSubscribeFromBackButtonEvent ()
		{
			// Verify the scene is already running and we already subscribed:
			if(_readyToRun)
				GameApplication.Instance.BackButtonService.RemoveListener(this);
		}
		
		
		public bool HandleBackButtonPress ()
		{
			GameApplication.Instance.BackButtonService.DefaultBackButtonAction();
			return true;
		}
		
		#endregion
	}
}