using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TabTale.SceneManager;
using TabTale.Analytics;
using TabTale.Publishing;
//using TabTale.Publishing.PSdk;
using TabTale.AssetManagement;
using System.Linq;
using System.IO;
using TabTale.Data;

namespace TabTale {

	/// <summary>
	/// The GameApplication object is the "kernel" of the TabTale game - it is the entry point
	/// through which all interaction with publishing and game services provided by TabTale is
	/// performed. This also includes services provided by third parties, under the condition
	/// that they are abstracted by the publishing and/or game sdk.
	/// This object always is, and always must be, a singleton. However, it is not created
	/// by a "regular" singleton pattern, and its lifecycle is managed by the Unity engine.
	/// Instead, it is created as a regular Unity object, from a prototype (prefab) 
	/// to be provided in the resources folder of the game. In this manner, various
	/// aspects of the GameApplication object can be configured during design tiem 
	/// within the Unity editor. This process of creating the GameApplication instance 
	/// is called by the Instance static method.
	/// </summary>
	public class GameApplication : TaskFactory, IGameApplication
	{
		#region ISceneController implementation

		public bool AddExit (ISceneExit sceneExit)
		{
			return false;
		}

		public string SceneName 
		{
			get { return "<Unknown>"; }
		}

		public void PrepareToUnload()
		{
		}

		public Transform Transform
		{
			get { return this.transform; }
		}

		public bool ReadyToUnload
		{
			get { return true; }
		}

		public void OnSceneClosing()
		{
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

		#region IGameApplication implementation

		#region Service Providers

		IAssetManager _assetManager;
		public IAssetManager AssetManager
		{
			get { return _assetManager; }
		}

		ILocalNotificationServices _notificationServices = new NullNotifications();
		public ILocalNotificationServices NotificationServices
		{
			get { return _notificationServices; }
		}

		ISceneManager _sceneManager = new UnitySceneManager();
		public ISceneManager SceneManager
		{
			get { return _sceneManager; }
		}

		IGeneralDialogService _generalDialogService = new NullGeneralDialogService();
		public IGeneralDialogService GeneralDialog
		{
			get { return _generalDialogService; }
		}

		IBackButtonService _backButtonService = new NullBackButtonService();
		public IBackButtonService BackButtonService
		{
			get { return _backButtonService; }
		}

		private IModalityManager _modalityManager = new NullModalityManager();
		public IModalityManager ModalityManager
		{
			get { return _modalityManager; }
		}

		private IBillingService _billingService = new NullBillingService();
		public IBillingService BillingService
		{
			get { return _billingService; }
		}
		IApplicationEvents _applicationEvents;
		public IApplicationEvents ApplicationEvents
		{
			get { return _applicationEvents; }
		}

		#endregion

		class ApplicaitonEvents : IApplicationEvents
		{
			#region IApplicationEvents implementation
			
			public event System.Action GainedFocus = () => {};
			public event System.Action LostFocus = () => {};
			public event System.Action Paused = () => {};
			public event System.Action<float> Resumed = pauseTime => {};
			public event System.Action Quit = () => {};
			public event System.Action<int> LevelLoaded = level => {};
			
			#endregion

			public ApplicaitonEvents(GameApplication gameApplication)
			{
				gameApplication.GainedFocus += () => GainedFocus();
				gameApplication.LostFocus += () =>  LostFocus();
				gameApplication.Paused += () => Paused();
				gameApplication.Resumed += t => Resumed(t);
				gameApplication.Quit += () => Quit();
				gameApplication.LevelLoaded += level => LevelLoaded(level);
			}
		}

		class ConfigurationServiceAdaptor : IConfigurationService
		{
			#region IConfigurationService implementation

			public DataElement GetConfiguration (string id = "")
			{
				return GameApplication.GetConfiguration(id);
			}

			#endregion

			#region IService implementation

			public ITask GetInitializer (IServiceResolver moduleContainer)
			{
				return moduleContainer.TaskFactory.FromEnumerableAction(Init);
			}

			IEnumerator Init()
			{
				yield break;
			}

			#endregion
		}

		IConfigurationService _configurationService = new ConfigurationServiceAdaptor();
		public IConfigurationService Configuration
		{
			get { return _configurationService; }
		}
		
		public IDispatcher Dispatcher
		{
			get { return this; }
		}

		private ServiceResolver _serviceResolver;
		public IServiceResolver ServiceResolver
		{
			get { return _serviceResolver; }
		}

		#endregion

		#region Creation and Initialization

		static bool s_earlyAccess = true;

		override protected void Awake()
		{
			base.Awake();

			LoggerModules.Register();

			Debug.Log("Setting GSDK logging mode : Is logging enabled - " + Debug.isDebugBuild );
#if UNITY_2017_1_OR_NEWER
			Debug.unityLogger.filterLogType = Debug.isDebugBuild ? LogType.Log : LogType.Warning;
#else
			Debug.logger.filterLogType = Debug.isDebugBuild ? LogType.Log : LogType.Warning;
#endif

			CreateModuleContainer();
		}

		void OnLevelWasLoaded(int level)
		{
			LevelLoaded(level);
		}

		private void InitSceneManager()
		{
			_sceneManager = _serviceResolver.Get<ISceneManager>();

			//TODO: see if need to replace call to location manager
			_sceneManager.SceneLoadStarted += (sceneName) => {
				//_locationManager.ReportEvent(ApplicationLocation.SceneEnd);
			};

			_sceneManager.SceneLoaded += (sceneName) => {
				//_locationManager.ReportEvent(ApplicationLocation.SceneStart);
			};
		}

		private ITaskQueue _taskQueue;

		void EnqueueServiceResolution<TService>(System.Action<TService> setter, bool blocking)
			where TService : class, IService
		{
			if(blocking)
			{
				_taskQueue.Enqueue(() => {
					TService service = _serviceResolver.Get<TService>();
					setter(service);
				});
				return;
			}

			bool found = false;
			TService foundService = null;

			_taskQueue.Enqueue(() => {
				_serviceResolver.Get<TService>((result, service) => {
					foundService = service;
					setter(service);
                    found = true;
                });
			});

			_taskQueue.Parallelize(() => found, -1, string.Format("find service {0}", typeof(TService).Name)).Done += result => {
				CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("done preparing service {0}: {1}; provider is: {2}", typeof(TService).Name, result, 
				                                              foundService == null ? "null" : foundService.GetType().Name));
			};
		}

		void EnqueuePreStageServiceResolution(bool blocking)
		{
			EnqueueServiceResolution<IConfigurationService>(s => _configurationService = s, blocking);
			EnqueueServiceResolution<IModalityManager>(s => _modalityManager = s, blocking);
			EnqueueServiceResolution<IBackButtonService>(s => _backButtonService = s, blocking);
			EnqueueServiceResolution<IGeneralDialogService>(s => _generalDialogService = s, blocking);
		}

        void EnqueueServiceResolution(bool blocking)
		{
			EnqueueServiceResolution<IAssetManager>(s => _assetManager = s, blocking);

			EnqueueServiceResolution<ISceneManager>(s => _sceneManager = s, blocking);
		}

		void EnqueuePostStageServiceResolution(bool block)
		{
		}

		public float initTimeout = 20f;

		private void CreateModuleContainer()
		{
			if(_gameModules == null)
			{
				_gameModules = new ModuleContainer(this, _serviceResolver);
			}
		}

		private void CreateServiceResolver()
		{
			if(_serviceResolver == null)
			{
				ServiceResolver resolver = new ServiceResolver(this, LoggerModules.GameApplication);
				resolver.AddServices(new System.Type[] {
					typeof(ISceneManager), typeof(IAssetManager), typeof(ILocalNotificationServices), 
					typeof(IGeneralDialogService), typeof(IGameDB), typeof(IConfigurationService),
					typeof(IModalityManager), typeof(IRealtimeMultiplayerService), typeof(IBackButtonService)
				});

				resolver.AddProvider(() => new UnitySceneManager());
				resolver.AddProvider(() => new UnityAssetManager(this));
				resolver.AddProvider(() => new NullBackButtonService());
				resolver.AddProvider(() => new NullGeneralDialogService());
                resolver.AddProvider(() => new NullModalityManager());
				
				_serviceResolver = resolver;
			}
		}

		void EnqueueInitModules(int fromStage, int toStage, bool block)
		{
			if(block)
			{
				_taskQueue.Enqueue(() => {
					_gameModules.Init(fromStage, toStage);
				});
				return;
			};

			bool initDone = false;
			_taskQueue.Enqueue(() => {
				_gameModules.Init(fromStage, toStage, () => {
					initDone = true;
				});
			});

			_taskQueue.Parallelize(() => initDone);
		}

		void EnqueueCreateResolvers()
		{
			_taskQueue.Enqueue(() => {
				CreateServiceResolver();
				CreateModuleContainer();
			}, "Create Resolvers").Done += result => {
				CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("done creating module containers: {0}", result));
			};
		}

		void EnqueueAddProviders()
		{
			_taskQueue.Enqueue(() => {
				_serviceResolver.AddProviders(GetComponentsInChildren(typeof(IService)).Cast<IService>());
			}, "Add Service Providers to Service Resolver").Done += result => {
				CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("done finding service providers: {0}", result));
			};
		}	

		// a flag to make sure we are not initializing two instances at once
		static bool s_initInProgress = false;

		public IEnumerator Init(bool block)
		{
			if(s_initInProgress)
			{
				CoreLogger.LogError(LoggerModules.GameApplication, "attempt to initialize kernel while already doing so!");
				yield break;
			}

			Application.targetFrameRate = this.targetFrameRate;

			s_instance = this;

			s_initInProgress = true;

			_taskQueue = gameObject.AddMissingComponent<TaskQueue>();
			_taskQueue.Active = false;

			EnqueueCreateResolvers();

			EnqueueAddProviders();

			EnqueuePreStageServiceResolution(block);
			_taskQueue.Enqueue(() => {
				LoadLoggerConfig();
			});
			EnqueueInitModules(int.MinValue, -1, block);

			EnqueueServiceResolution(block);
			EnqueueInitModules(0, 0, block);

			EnqueuePostStageServiceResolution(block);
			EnqueueInitModules(1, int.MaxValue, block);

			_taskQueue.Enqueue(() => {
				LoadLoggerConfig();
			});

			//register to the event that tells us when the queue is empty, 
			//which implies we have finished initializing
			bool initDone = false;

			//when the task queue empties, we will know we are done
			_taskQueue.Enqueue(() => {

				//at this point we can set the static instance safely
				s_earlyAccess = false;

				//init is done
				s_initInProgress = false;
                initDone = true;
			});

			//now start the actual initialization, by activating the queue
			float initStart = Time.time;

			//start running the init tasks
			_taskQueue.Active = true;

			if(block)
			{
				//the synchronous version:
				_taskQueue.HandleAll();

				//at this point in time, the singleton is ready for use
				_gameModules.Start();

				yield break;

			} else
			{
				//the async. version:

				//while(Time.time - initStart < initTimeout)
				while(true)
				{
					if(Time.time - initStart > initTimeout)
					{
						CoreLogger.LogWarning(LoggerModules.GameApplication, "timeout in creating GameApplication object");
					}

					if(initDone)
					{
						//at this point in time, the singleton is ready for use
						_gameModules.Start();

						yield return StartCoroutine(WaitForStrangeServiceProviderInit());

						InitDone();
						CoreLogger.LogDebug(LoggerModules.GameApplication, "GameApplication succesfully finished Init!");
                        yield break;
                    }
                    
                    yield return null;
				}
			}
		}

		private GameApplication()
		{
			//FIXME: unity doesn't like this line, can't figure out why...
			//all of a sudden, it started printing an error message when there's this early access to the
			//s_instance variable - very strange, possibly a Unity bug, may be related to some static
			//initialization
			//Debugger.Assert(s_instance == null, "cannot call GameApplication constructor twice!");

			_assetManager = new UnityAssetManager(this);

			CreateServiceResolver();

			_applicationEvents = new ApplicaitonEvents(this);
		}

		public static event System.Action InitDone = () => {};

		#endregion

		#region Framework Resources

		/// <summary>
		/// Loads the framework resource of the given name. The method will look for the resource in
		/// the cascading search path for framework resources - currently, that means first the
		/// LocalFramework folder, then the Framework folder (both must be under a "Resources"
		/// parent somewhere.
		/// </summary>
		/// <returns>The framework resource.</returns>
		/// <param name="name">Name.</param>
		public static TResource LoadFrameworkResource<TResource>(string name)
			where TResource : Object
		{
			//string pathSeaparator = Path.AltDirectorySeparatorChar;
			string pathSeparator = "/";

			string localResourcePath = "Local" + s_frameworkResourcePath;
			string platformLocalResourcePath = Application.platform.ToString() + pathSeparator + localResourcePath;
			string platformResourcePath = Application.platform.ToString() + pathSeparator + s_frameworkResourcePath;

			TResource resource = Resources.Load<TResource>(platformLocalResourcePath + pathSeparator + name);
			if(resource == null)
			{
				CoreLogger.LogInfo(LoggerModules.GameApplication, string.Format("resource {0} not found in {1}, looking in {2}...", name, platformLocalResourcePath, localResourcePath));
				resource = Resources.Load<TResource>(localResourcePath + pathSeparator + name);
				if(resource == null)
				{
					CoreLogger.LogInfo(LoggerModules.GameApplication, string.Format("resource {0} not found in {1}, looking in {2}...", name, localResourcePath, platformResourcePath));
					resource = Resources.Load<TResource>(platformResourcePath + pathSeparator + name);
					if(resource == null)
					{
						CoreLogger.LogInfo(LoggerModules.GameApplication, string.Format("resource {0} not found in {1}, looking in {2}...", name, platformResourcePath, s_frameworkResourcePath));
						resource = Resources.Load<TResource>(s_frameworkResourcePath + pathSeparator + name);
					}
				}
			}		

			if(resource == null)
			{
				CoreLogger.LogWarning(LoggerModules.GameApplication, string.Format("resource {0} not found in framework paths!", name));
			}

			return resource;
		}

		public static Object LoadFrameworkResource(string name)
		{
			return LoadFrameworkResource<Object>(name);
		}

		public static IList<TResource> LoadCascadingFrameworkResource<TResource>(string name)
			where TResource : Object
		{
			IList<TResource> resources = new List<TResource>();

			char pathSaparator = '/'; // Path.DirectorySeparatorChar

			string localResourcePath = "Local" + s_frameworkResourcePath;
			string platformLocalResourcePath = Application.platform.ToString() + pathSaparator + localResourcePath;
			string platformResourcePath = Application.platform.ToString() + pathSaparator + s_frameworkResourcePath;

			TResource resource = Resources.Load<TResource>(platformLocalResourcePath + pathSaparator + name);
			if(resource != null)
				resources.Add(resource);

			resource = Resources.Load<TResource>(localResourcePath + pathSaparator + name);
			if(resource != null)
				resources.Add(resource);

			resource = Resources.Load<TResource>(platformResourcePath + pathSaparator + name);
			if(resource != null)
				resources.Add(resource);

			resource = Resources.Load<TResource>(s_frameworkResourcePath + pathSaparator + name);
			if(resource != null)
				resources.Add(resource);
			
			return resources;
		}

		private static string s_frameworkResourcePath = "Framework";

		private static string s_applicationResourceName = "TabTale_Application";

		/// <summary>
		/// This is the part of the initialization sequence which is always synchronous - 
		/// it merely creates an instance of the GameApplication object, without doing any
		/// initialization - there's no need to make it async., since it's fast anyway.
		/// It's called by both the sync. and asnyc. versions of the CreateInstance method.
		/// </summary>
		/// <returns>The or get.</returns>
		public static GameApplication CreateOrGet()
		{
			//if an instance has already been created, don't create a new one
			s_instance = FindObjectOfType<GameApplication>();
			if(s_instance != null)
			{
				CoreLogger.LogDebug(LoggerModules.GameApplication, "an instance of GameApplication has miraculously appeared...");
				return s_instance;
			}
			
			//we want to create the instance as it is defined in the resources folder
			GameApplication instance;

			Object prefab = LoadFrameworkResource(s_applicationResourceName);
			if(prefab == null)
			{
				//no prefab found - we can still create a default
				Debug.LogError(string.Format("Unable to load {0}.prefab - creating object with defaults", s_applicationResourceName));
				GameObject appObj = new GameObject();
				instance = appObj.AddComponent<GameApplication>();
				
			} else
			{
				GameObject appObj = Instantiate(prefab) as GameObject;
				instance = appObj.GetComponent<GameApplication>();
			}

			GameObject.DontDestroyOnLoad(instance);

			return instance;
		}

		public bool AllowSynchronousCreation = true;
		public string synchronousCreationFallbackScene = "";

#if UNITY_EDITOR

		static string s_requestingScene = "";
		public static string RequestingScene
		{
			get { return s_requestingScene; }
			set {s_requestingScene = value;}
		}

		public static void Init(string initSceneName)
		{
			if(s_instance != null)
				return;

			if(initSceneName == null || initSceneName == "")
			{
				CoreLogger.LogDebug(LoggerModules.GameApplication, "request to init GameApplication synchronously");
				IGameApplication app = GameApplication.Instance;
				if(app == null)
				{
					CoreLogger.LogCritical(LoggerModules.GameApplication, "failed to create GameApplication!");
					return;
				}
			}

			s_requestingScene = Application.loadedLevelName;
			Application.LoadLevel(initSceneName);
		}

#endif

		/// <summary>
		/// This initializes the GameApplication singleton as a blocking call - it means
		/// that access to Instance is valied immediately as this method returns.
		/// </summary>
		public static GameApplication CreateInstanceSync()
		{
#if UNITY_EDITOR
			s_requestingScene = "";
#endif

			if(s_instance != null)
			{
				CoreLogger.LogError(LoggerModules.GameApplication, "calling GameApplication sync init while in async init - this is a serious error, unless in a special debug mode!!");
				return s_instance;
			}

			CoreLogger.LogDebug(LoggerModules.GameApplication, "creating GameApplication instance synchronously");

			GameApplication instance = CreateOrGet();
			if(instance == null)
			{
				CoreLogger.LogCritical(LoggerModules.GameApplication, "unable to obtain kernel instance!");
				return null;
			}

			if(!instance.AllowSynchronousCreation)
			{
				if(instance.synchronousCreationFallbackScene != null && instance.synchronousCreationFallbackScene != "")
				{
					CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("synchronous creation not allowed in this game - " +
					                                              "reverting to scene {0}", instance.synchronousCreationFallbackScene));
					string scene = instance.synchronousCreationFallbackScene;
					DestroyObject(instance.gameObject);
#if UNITY_EDITOR
					GameApplication.Init(scene);
#endif
					return null;
				}

				CoreLogger.LogCritical(LoggerModules.GameApplication, "synchrnous creation not allowed in this Game!!!");
				return null;
			}

			IEnumerator steps = instance.Init(true);
			while(steps.MoveNext())
			{
			}

			return instance;
		}

        private static GameApplication s_instance;
        public static new IGameApplication Instance
        {
			get
			{
#if UNITY_EDITOR
				//Within the Unity Editor, when debugging, we allow developers to run a specific
				//scene, as opposed to going through the entire initialization sequence - in this
				//case, we want to allow creation of the application object upon demand, as in the
				//singleton pattern. In other cases, we want to it to already have been created by 
				//this time
//				if(s_instance == null)
//				{
//					CreateInstanceSync();
//				}

//				if(s_instance == null)
//				{
				//					Logger.LogError(LoggerModules.GameApplication, "Unable to find the mandatory GameApplication object!");
//				}
#endif
				if(s_earlyAccess)
				{
					CoreLogger.LogWarning(LoggerModules.GameApplication, "early access to application singleton - consider revising!");
				}

				//outside of the Unity Editor, this object can only be created in the regular manner
				return s_instance;

			}
		}

		#endregion

		public int targetFrameRate = 30;

		public PopupResultHandler PopupResultHandler
		{
			get
			{
				return (handle, result) => {
					CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("received popup result handler: {0}", result));
				};
			}
		}

		public PopupHandleReceiver PopupHandleReceiver
		{
			get
			{
				return (handle) => {
					CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("received popup handle: {0}", handle.ToString()));
				};
			}
		}

		void OnApplicationFocus(bool focusStatus)
		{
			if(focusStatus)
			{
				CoreLogger.LogInfo(LoggerModules.GameApplication, string.Format("Game gained focus at time: {0}", Time.realtimeSinceStartup));
				GainedFocus();
			} else
			{
				CoreLogger.LogInfo(LoggerModules.GameApplication, string.Format("Game lost focus at time: {0}", Time.realtimeSinceStartup));
				LostFocus();
			}
		}

		event System.Action GainedFocus = () => {};
		event System.Action LostFocus = () => {};
		event System.Action Paused = () => {};
		event System.Action<float> Resumed = pauseTime => {};
		event System.Action Quit = () => {};
		event System.Action<int> LevelLoaded = level => {};

		float _pauseStart;
		void OnApplicationPause(bool pauseStatus)
		{
			if(pauseStatus)
			{
				CoreLogger.LogInfo(LoggerModules.GameApplication, string.Format("Game Paused at time {0}", Time.realtimeSinceStartup));

				_pauseStart = Time.realtimeSinceStartup;
				Paused();
			} else
			{
				CoreLogger.LogInfo(LoggerModules.GameApplication, string.Format("Game Resumed at time {0}, after {1} seconds", Time.realtimeSinceStartup, Time.realtimeSinceStartup - _pauseStart));

				Resumed(Time.realtimeSinceStartup - _pauseStart);
				_pauseStart = 0;
			}
		}

		void OnApplicationQuit()
		{
			CoreLogger.LogInfo(LoggerModules.GameApplication, "application quit!");
			Quit();
		}

		ModuleContainer _gameModules;
		public IModuleContainer ModuleContainer
		{
			get { return _gameModules; }
		}

		#region Framework Configurations

		static IDictionary<string, DataElement> s_config = new Dictionary<string, DataElement>();
		static string s_defaultConfigName = "gameConfig";
		public static DataElement GetConfiguration(string id = "")
		{
			char pathSaparator = '/'; // Path.DirectorySeparatorChar
			string configName = "Config" + pathSaparator + (id == "" ? s_defaultConfigName : id);
			
			DataElement config;
			if(s_config.TryGetValue(configName, out config))
				return config;
			
			IList<TextAsset> cascadingConfig = GameApplication.LoadCascadingFrameworkResource<TextAsset>(configName);
			if(cascadingConfig.Count < 1)
				return DataElement.Null;
			
			config = DataElement.Parse(cascadingConfig[0].text);
			for(int i=1;i<cascadingConfig.Count;i++)
				config.Merge(DataElement.Parse(cascadingConfig[i].text));
			
			s_config[configName] = config;
			
			return config;
		}

		#endregion

		#region Logging

		void LoadLoggerConfig()
		{
			DataElement loggerConfig = GetConfiguration("logger");
			if(loggerConfig.IsNull)
				return;

			int defaultThreshold = 0;
			DataElement defaultModule = loggerConfig[CoreLogger.DefaultModuleName];
			if(!defaultModule.IsNull)
				defaultThreshold = CoreLogger.Severity.ParseSeverity(defaultModule);

			CoreLogger.LogDebug(LoggerModules.GameApplication, "/////////////////////////// setting logger thresholds ////////////////////////");

			foreach(string moduleName in CoreLogger.Modules)
			{
				CoreLogger.SetThreshold(moduleName, defaultThreshold);
			}

			CoreLogger.SetThreshold(defaultThreshold);

			foreach(KeyValuePair<string, DataElement> kvp in loggerConfig.GetDataPairs())
			{
				string moduleName = kvp.Key;
				int moduleThreshold = CoreLogger.Severity.ParseSeverity(kvp.Value);

				CoreLogger.SetThreshold(moduleName, moduleThreshold);
			}
		}

		#endregion

		private bool _strangeServicesInitDone;
		public bool StrangeServicesInitDone
		{
			get { return _strangeServicesInitDone; }
			set { _strangeServicesInitDone = value; }
		}
		IEnumerator WaitForStrangeServiceProviderInit()
		{
			while(!_strangeServicesInitDone)
			{
				yield return null;
			}

			yield break;
		}

		public new TComponent GetComponent<TComponent> () where TComponent : Component
		{
			return base.GetComponent<TComponent> ();
		}
	}
}
