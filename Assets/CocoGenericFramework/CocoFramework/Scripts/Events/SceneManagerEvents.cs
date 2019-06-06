using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;
using TabTale.SceneManager;
using UnityEngine.SceneManagement;
using System;

namespace TabTale
{
	public class SceneManagerEvents
	{
		[Inject]
		public SceneLoadedSignal sceneLoadedSignal {get;set;}

		[Inject]
		public FirstSceneLoadedSignal firstSceneLoadedSignal {get;set;}

		[Inject]
		public GamePausedSignal gamePausedSignal {get;set;}

		[Inject]
		public GameQuitSignal gameQuitSignal {get;set;}

		[Inject]
		public ICrashTools crashTools {get;set;}
   
		[Inject]
		public ILogger logger {get;set;}

		private const string Tag = "SceneManagerEvents";

        [PostConstruct]
		public void Init()
		{
			logger.Log(Tag,"Init");

            GameObject go = Resources.Load("Debug/OnSceneUnloadCrashToolsImpl") as GameObject;
            ISceneManager sceneManager = GameApplication.Instance.ModuleContainer.ServiceResolver.Get<ISceneManager>();
       
            GameApplication.Instance.ApplicationEvents.LevelLoaded += s => {
                crashTools.AddBreadCrumb("On scene loaded: " + Application.loadedLevelName);
				sceneLoadedSignal.Dispatch(Application.loadedLevelName);
                GameObject.Instantiate(go);
            };
            
            // Bind first scene loaded signal, dispatch the signal only once
			Action<string> OnFirstSceneLoaded = null;
			OnFirstSceneLoaded = s => {
                crashTools.AddBreadCrumb("On FIRST scene loaded: " + s);
                firstSceneLoadedSignal.Dispatch(s);
				sceneManager.SceneLoaded -= OnFirstSceneLoaded;
			};
			sceneManager.SceneLoaded += OnFirstSceneLoaded;
            
			GameApplication.Instance.ApplicationEvents.Paused += gamePausedSignal.Dispatch;
			GameApplication.Instance.ApplicationEvents.Quit += gameQuitSignal.Dispatch;

            /*
			sceneManager.TransitionOutStarted += s
				=> binder.GetInstance<SceneTransitionOutStartedSignal>().Dispatch(s);
			sceneManager.TransitionOutEnded += s =>
				binder.GetInstance<SceneTransitionOutEndedSignal>().Dispatch(s);
			sceneManager.TransitionInStarted += s =>
				binder.GetInstance<SceneTransitionInStartedSignal>().Dispatch(s);
			sceneManager.TransitionInEnded += s
				=> binder.GetInstance<SceneTransitionInEndedSignal>().Dispatch(s);
			sceneManager.SceneLoadStarted += s =>
				binder.GetInstance<SceneLoadStartedSignal>().Dispatch(s);
			*/

        }

	}


}


