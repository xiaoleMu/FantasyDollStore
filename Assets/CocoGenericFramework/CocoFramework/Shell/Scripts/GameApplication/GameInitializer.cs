using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TabTale.SceneManager;

namespace TabTale 
{
	public class GameInitializer : MonoBehaviour 
	{
		public string openingScene = "MainMenu";

		void Awake()
		{
			//At this point we want to initialize the SDK's
			//FIXME: we may want to reconsider this method - right now the concept
			//is to have the GameApplication object initialize what it can, and then
			//have the GameApplication object initialized through this object when running
			//in full "game" mode, and on its own automatically when running in "scene" mode
			// - that is, a single scene called from the unity editor
			//the downside to this is that we have to re-init all the services again and again
			//when debugging - it may be benefitial to create an "external" singleton to retain
			//this object - one that "lives" through unity editor run sessions (its lifecycle 
			//is an editor session, not a game session)
			//note that it's possible to do that later on, but the downside to that is that
			//we will need some method to "reset" it from the editor
			string nextScene = openingScene;
#if UNITY_EDITOR

			SceneController.s_jumpToInitOnTestScenes = false;

			if(GameApplication.RequestingScene!="")
			{
				nextScene = GameApplication.RequestingScene;
			} else
			{
				GameApplication.RequestingScene=nextScene;
			}
#endif

			//find the SplashScreen, so we can signal it off later
			SplashScreen splashScreen = gameObject.GetComponentInChildren<SplashScreen>();
			GameApplication gameApplication = GameApplication.CreateOrGet();
			ServiceInitializer serviceInitializer = new ServiceInitializer();
			if(splashScreen == null)			
			{
				GameApplication.InitDone += () => {
					GameApplication.Instance.SceneManager.SwitchScene(nextScene);
				};

				//iff we finish the pre-init phase, we can start the kernel
				serviceInitializer.Run (GetComponentsInChildren(typeof(IApplicationPreInit)).Cast<IApplicationPreInit>(), result => {
					if(result)
					{
						StartCoroutine(gameApplication.Init(false));
					} else
					{
						CoreLogger.LogError("Init", "failed to start the application");
                        Application.Quit();
                    }
                });

			} else
			{
				serviceInitializer.Run (GetComponentsInChildren(typeof(IApplicationPreInit)).Cast<IApplicationPreInit>(), result => {
					
					//iff we finish the pre-init phase, we can ask the splash screen to start the kernel
					if(result)
					{
						splashScreen.Enqueue(() => gameApplication.Init(false), "Build Game Application");
						
						splashScreen.Done += () => {
							
							CoreLogger.LogDebug("GameApplication.Instance == " + GameApplication.Instance);
							CoreLogger.LogDebug("GameApplication.Instance.SceneManager ==" + GameApplication.Instance.SceneManager);
							
							GameApplication.Instance.SceneManager.SwitchScene(nextScene);
                        };
                    } else
                    {
                        CoreLogger.LogError("Init", "failed to start the application");
                        Application.Quit();
                    }
                });

			}


#if UNITY_EDITOR
#else
			Debugger.Assert(GameApplication.Instance == null, "GameApplication initialization called twice!");
#endif
		}

		class ServiceInitializer
		{
			public event System.Action<bool> Done = b => {};

			private bool _currentResult = true;
			int _numServices = 0;
			int _servicesDone = 0;

			public ServiceInitializer Run(IEnumerable<IApplicationPreInit> services, System.Action<bool> doneHandler)
			{
				Done += doneHandler;

				foreach(IApplicationPreInit service in services)
				{
					_numServices++;
					service.Done += result => {
						if(result == false)
						{
							_currentResult = false;
						}
						_servicesDone++;
						if(_servicesDone == _numServices)
						{
							Done(_currentResult);
						}
					};
					service.Init();
				}

				if(_numServices == 0)
					Done(_currentResult);

				return this;
			}
		}
	
	}
}


