using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class DebugButton : GameView 
	{
		public GameObject debugPanel;
		public GameObject debugDetailsView;

		private GameObject _gameDebugView;

		private enum DebugState
		{
			DebugPanelClosed,
			ShowingGameSpecificDebug,
			ShowingGsdkDebug
		}

		protected override void OnRegister()
		{
			base.OnRegister();

			var prefab = Resources.Load<GameObject>("Debug/GameDebugCanvas");
			if (prefab != null)
			{
				_gameDebugView = Instantiate(prefab) as GameObject;
				DontDestroyOnLoad(_gameDebugView);
				logger.Log(Tag,"Loaded game debug canvas...");
			}

		}

		public void ToggleOnClick()
		{
			if(_gameDebugView != null)
			{
				bool shouldShowGameDebugView = !_gameDebugView.activeSelf && !debugPanel.activeSelf;
				debugPanel.SetActive(_gameDebugView.activeSelf);
				_gameDebugView.SetActive(shouldShowGameDebugView);

			}
			else
			{
				debugPanel.SetActive(!debugPanel.activeSelf);	
			}
        }
	}
}