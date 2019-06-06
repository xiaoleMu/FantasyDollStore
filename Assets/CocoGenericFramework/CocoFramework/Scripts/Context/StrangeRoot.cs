using UnityEngine;
using System.Collections;
using strange.extensions.context.impl;
using strange.extensions.context.api;
using strange.extensions.signal.impl;
using TabTale.SceneManager;
using strange.extensions.injector.api;
using System;
using TabTale.Plugins.PSDK;


namespace TabTale
{
	public class StrangeRoot : TaskFactory, IModule, IContextView
	{
        private GameRoot _gameRoot;
        public GameRoot GameRoot { get { return _gameRoot; } }

		private MainContext _mainContext;
		public MainContext MainContext { get { return _mainContext; } }

		#region IModule implementation

		public void StartModule ()
		{
			CoreLogger.LogDebug("StrangeRoot", string.Format("StartModule"));

			_mainContext = new MainContext(this);
			context = _mainContext;

            GameObject go = new GameObject();
			go.name = "GameRoot";
            _gameRoot = go.AddComponent<GameRoot>();
            _gameRoot.Init(this.gameObject);

			//HookupApplicationEvents(); //TODO: Uncomment

			StartCoroutine(StartCoro());
		}

		void HookupApplicationEvents()
		{

			IInjectionBinder binder = ((MainContext)context).injectionBinder;
			IApplicationEvents appEvents = GameApplication.Instance.ApplicationEvents;


			appEvents.Paused += () => binder.GetInstance<GamePausedSignal>().Dispatch();
			appEvents.Resumed += t => binder.GetInstance<GameResumedSignal>().Dispatch(t);
			appEvents.LostFocus += () => binder.GetInstance<FocusLostSignal>().Dispatch();
			appEvents.GainedFocus += () => binder.GetInstance<FocusGainedSignal>().Dispatch();
			
			ISceneManager sceneManager = _moduleContainer.ServiceResolver.Get<ISceneManager>();
			
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
			sceneManager.SceneLoaded += s =>
				binder.GetInstance<SceneLoadedSignal>().Dispatch(s);
				
		}
		IEnumerator StartCoro()
		{
			yield return new WaitForEndOfFrame();
            ((MainContext)context).SendStartSignal();
		}

		public void StopModule ()
		{
		}

		public void Terminate ()
		{
		}

		public void UpdateModule (float timeDelta)
		{
		}

		public int Stage
		{
			get { return 1; }
		}

		#endregion

		IModuleContainer _moduleContainer;
		public ITask GetInitializer(IModuleContainer moduleContainer)
		{
			_moduleContainer = moduleContainer;
			
			return FromEnumerableAction(ModuleInitialize);
		}
		
		IEnumerator ModuleInitialize()
		{
			yield break;
		}


		/// <summary>
		/// When a ContextView is Destroyed, automatically removes the associated Context.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if(context != null)
            ((MainContext)context).SendOnDestroySignal();

			if (context != null)
				Context.firstContext.RemoveContext(context);
		}
		
		#region IView implementation
		
		public bool requiresContext {get;set;}
		
		public bool registeredWithContext {get;set;}
        
        public bool autoRegisterWithContext{ get; set; }
        
        #endregion

		#region IContextView implementation

		public IContext context{get;set;}

		#endregion

	}

	public class GameQuitSignal : Signal { }

	public class GamePausedSignal : Signal { }

	public class GameResumedSignal : Signal<float> { }

	public class FocusGainedSignal : Signal { }

	public class FocusLostSignal : Signal { }

	public class SceneTransitionOutStartedSignal : Signal<string> { }

	public class SceneTransitionOutEndedSignal : Signal<string> { }

	public class SceneTransitionInStartedSignal : Signal<string> { }

	public class SceneTransitionInEndedSignal : Signal<string> { }

	public class SceneRequestedSignal : Signal<string> { }

	public class SceneLoadStartedSignal : Signal<string> { }

	public class SceneLoadedSignal : Signal<string> { }

	public class FirstSceneLoadedSignal : Signal<string> { }
}
