using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace TabTale.SceneManager
{
	public class SceneLoader : ObjectHider
	{
		public string realScene;

		/// <summary>
		/// These objects will be removed (destroyed) when the loading finished.
		/// </summary>
		public GameObject[] removals;

		float _progress = 0;
		public float Progress
		{
			get { return _progress; }
		}

		public string forceInitScene = "Init";

		/// <summary>
		/// Minimum time for the transition.
		/// </summary>
		public float minimumTime = 0;

		void Awake()
		{
#if UNITY_EDITOR
		
			if(SceneController.s_jumpToInitOnTestScenes)
			{
				SceneController.s_jumpToInitOnTestScenes = false;
				string temp = forceInitScene;
				forceInitScene = "";
				GameApplication.RequestingScene = Application.loadedLevelName;
                Application.LoadLevel(temp);
                return;
			}
#endif
		}

		public SceneTransition sceneTransition;
		public float loadTimeout = 10f;
	
		IEnumerator Load()
		{
			reactivate = true;

			Deactivate();

			float startRealtime = Time.realtimeSinceStartup;
			float startGametime = Time.time;
			int startFrame = Time.frameCount;

			ISceneLoadAction[] actions = GetComponentsInChildren(typeof(ISceneLoadAction)).Cast<ISceneLoadAction>().ToArray();
			float total = actions.Length + 1;
			float done = 0;

			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("SceneLoader beginning {0} actions...", actions.Length));

			foreach(ISceneLoadAction action in GetComponentsInChildren(typeof(ISceneLoadAction)))
			{
				float actionStart = Time.realtimeSinceStartup;
				int actionFrameStart = Time.frameCount;
				yield return GameApplication.Instance.CoroutineFactory.StartCoroutine(action.Action);

				CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("performed load action {0} in {1} seconds and {2} frames", 
				                                                         action.Name, Time.realtimeSinceStartup - actionStart, Time.frameCount - actionFrameStart));

				done += 1;
				_progress = done / total;

				CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("progress is {0} ({1}/{2})", _progress, done, total));
			}

			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("beginning to proxy-load scene {0}, actions took {1} seconds real time, {2} seconds game time, {3} frames", 
			                                                          realScene, Time.realtimeSinceStartup - startRealtime, Time.time - startGametime, Time.frameCount - startFrame));
			startRealtime = Time.realtimeSinceStartup;
			startGametime = Time.time;
			startFrame = Time.frameCount;

			while(Time.realtimeSinceStartup - startRealtime < minimumTime)
				yield return null;

			bool loaded = false;
			ITask loader = GameApplication.Instance.SceneManager.GetAdditiveLoader(realScene, sceneTransition, cleanup ? removals : null);
			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("starting additive loader {0}", loader.Name));
			loader.Start(result => {
				if(!result.IsOk())
				{
					CoreLogger.LogError(LoggerModules.SceneManager, string.Format("failed to load scene {0}!", realScene));
				} else
				{
					CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("done proxy-loading of scene {0}: took {1} seconds real-time, {2} game time, {3} frames", 
					                                                          realScene, Time.realtimeSinceStartup - startRealtime, Time.time - startGametime, Time.frameCount - startFrame));
				}
				loaded = true;
				_progress = 1f;

				Reactivate();

			}, loadTimeout);

			while(!loaded)
			{
				float loaderProgress = loader.Progress;
				_progress = (done + loaderProgress) / total;

				CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("progress is {0} ({1}/{2}); loading progress is {3}; yielding...", _progress, done, total, loaderProgress));

				yield return null;
			}
        }
        
        public bool cleanup = true;
        
		override public void Start()
		{
			StartCoroutine(Load ());
		}
	}
}
