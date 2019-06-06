using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;

namespace TabTale
{
	public class StartSceneCommand : Command
	{
		private string _sceneName;
		private GameStateModel _gameStateModel;

		public StartSceneCommand(string sceneName, GameStateModel gameStateModel)
		{
			_sceneName = sceneName;
			_gameStateModel = gameStateModel;
		}

		public override void Execute ()
		{
			_gameStateModel.StartScene(_sceneName);
		}


	}
}