using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;
using TabTale.SceneManager;

namespace TabTale
{
	[RequireComponent(typeof(SceneController))]
	public class SceneStrangeBridge : View
	{
		[Inject]
		public StartSceneSignal startSceneSignal { get; set; }

		protected override void Start()
		{
			base.Start();

			SceneController sceneController = GetComponent<SceneController>();

			sceneController.SceneStarted += () => {
				startSceneSignal.Dispatch(sceneController.SceneName);
			};
		}
	}
}
