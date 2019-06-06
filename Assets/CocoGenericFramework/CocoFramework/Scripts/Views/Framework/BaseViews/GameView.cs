using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;

namespace TabTale {
	/// <summary>
	/// A game view is a behaviour which is connected to a strange context as a view. 
	/// It receives the StartSignal and StartSceneSignal signals.
	/// Its usage is as follows: Instead of the usual Start and Update
	/// methods used by Unity, you should use OnStart, OnUpdate, since they
	/// are follow the game lifecycle - OnStart will be sent only when the
	/// StartSignal is sent, and OnUpdate will only be called if the
	/// game/scene are already running. In addition, OnRegister will be called
	/// right after registering with the context.
	/// </summary>
	public class GameView : View
	{
        protected enum ViewType { GameView, MainView };

		[Inject]
		public StartSignal startSignal { get; set; }
		
		[Inject]
		public StartSceneSignal sceneStartSignal { get; set; }

		[Inject]
		public GameStateModel gameStateModel { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		bool _isRegisterFired=false;

		protected string _tag = "";
		protected string Tag 
		{
			get { return _tag; }
			set { _tag = value; }
		}

		protected override void Awake ()
		{
            RegisterView();
			base.Awake ();

			if(registeredWithContext && !_isRegisterFired){
				_isRegisterFired=true;
				OnRegister();
			}

		}

        protected virtual void RegisterView()
        {
            RegisterWithContextType(ViewType.GameView);
        }

		protected override void Start ()
		{
			base.Start ();

			if(registeredWithContext && !_isRegisterFired)
			{
				_isRegisterFired=true;
				OnRegister();
			}

		}

		protected override void OnDestroy ()
		{
			if(registeredWithContext)
				OnUnRegister();

			base.OnDestroy ();
		}
		
		virtual protected void OnStartGame()
		{
		}		

		virtual protected void OnStartScene(string sceneName)
		{			
		}
		
		virtual protected void OnRegister()
		{
			AddListeners();
			SetTag();
		}

		virtual protected void SetTag()
		{
			Tag = GetType().Name;
		}

		virtual protected void OnUnRegister()
		{
			StrangeRoot strangeRoot = GameApplication.Instance.ModuleContainer.Get<StrangeRoot>();
			GameRoot gameRoot = strangeRoot.GameRoot;
			gameRoot.context.RemoveView (this);

			RemoveListeners();
		}

		virtual protected void AddListeners ()
		{
		}
		
		virtual protected void RemoveListeners ()
		{
		}

        virtual protected void RegisterWithContextType(ViewType viewType)
        {
            if (!registeredWithContext && GameApplication.Instance != null)
            {
                StrangeRoot strangeRoot = GameApplication.Instance.ModuleContainer.Get<StrangeRoot>();
                MonoBehaviour view = this;
                
                if(viewType == ViewType.GameView)
                {
                    GameRoot gameRoot = strangeRoot.GameRoot;
                    if(gameRoot == null)
                    {
                        Debug.LogError("Error loading game context, Game Root (the bootstrap) is null");
                        return;
                    }
                    
                    gameRoot.context.AddView(view);
                }
                else if(viewType == ViewType.MainView)
                {
                    strangeRoot.context.AddView(view);
                }
            }
        }


	}
}
