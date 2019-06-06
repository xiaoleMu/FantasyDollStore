using UnityEngine;
using System.Collections;

namespace TabTale.SceneManager 
{
	public class SceneCrossfade : SceneTransition
	{
		private RenderTexture _targetTexture;

		protected override void OnActivate ()
		{
			_targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
			base.OnActivate ();
		}

		protected override void OnStartTransitionOut ()
		{
			Camera.main.targetTexture = _targetTexture;
		}

		void OnGUI()
		{
			float progress = _progress;

			switch(_stage)
			{
			case Stage.Done:
			case Stage.Inactive:
				return;

			case Stage.Limbo:
			case Stage.Out:
				progress = 1f;
				break;
			}

			GUI.depth = 0;
			Color old = GUI.color;
			GUI.color = new Color(old.r, old.g, old.b, progress);
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _targetTexture);
			GUI.color = old;
		}

	}
}
