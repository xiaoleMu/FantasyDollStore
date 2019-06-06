using UnityEngine;
using System.Collections;

namespace TabTale {

	public class GameStateModel
	{
		private bool _gameStarted = false;
		public bool GameStarted
		{
			get { return _gameStarted; }
		}

		public void StartGame()
		{
			_gameStarted = true;
		}

		private string _currentSceneName = "";
		public string CurrentSceneName
		{
			get { return _currentSceneName; }
		}

		public void StartScene(string sceneName)
		{
			_currentSceneName = sceneName;
		}
	}
}
