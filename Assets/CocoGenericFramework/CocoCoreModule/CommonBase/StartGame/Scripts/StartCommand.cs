/// The only change in StartCommand is that we extend Command, not EventCommand

using System;
using UnityEngine;
using CocoPlay;


#if UNITY_EDITOR
using UnityEditor;
#endif
using strange.extensions.command.impl;
using TabTale;
using TabTale.Analytics;

namespace TabTale {
	public class StartCommand : Command
	{
		private GameStateModel _gameStateModel;

		[Inject]
		public SocialServiceEvents socialServiceEvents {get;set;}

		[Inject]
		public ISocialNetworkService socialNetworkService {get;set;}

		[Inject]
		public SocialSync socialSync {get;set;}

		[Inject]
		public IGeneralDialogService generalDialog{get;set;}

		public StartCommand(GameStateModel gameStateModel)
		{
			_gameStateModel = gameStateModel;
		}

		public override void Execute()
		{
//			Logger.LogDebug("StartCommand","Execute");

			IGameApplication container = GameApplication.Instance;

			_gameStateModel.StartGame();

			//game controlelrs
			GameObject pGameControllers = UnityEngine.GameObject.Instantiate (Resources.Load ("CocoMainController", typeof(GameObject))) as GameObject;
			pGameControllers.transform.parent = GameApplication.Instance.Transform;

			BackButtonService pBackButtonService = GameObject.FindObjectOfType <BackButtonService> ();
			injectionBinder.Bind <BackButtonService> ().ToValue (pBackButtonService).CrossContext ();
		}
	}
}

