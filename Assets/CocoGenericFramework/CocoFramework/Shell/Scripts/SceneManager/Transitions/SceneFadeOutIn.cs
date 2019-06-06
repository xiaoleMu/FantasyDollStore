using UnityEngine;
using System.Collections;

namespace TabTale.SceneManager
{
	public class SceneFadeOutIn : SceneTransition
	{
		public Color fadeColor = Color.black;

		public bool fadeIn = true;
		public bool fadeOut = true;

		Texture2D _texture;
		Rect _rect;

		void Start()
		{
			_texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
			_rect = new Rect(0, 0, Screen.width, Screen.height);
		}
		
		void OnGUI()
		{
			float progress = _progress;

			if(!fadeOut)
			{
				switch(_stage)
				{
				case Stage.Out:
					progress = 1f;
					break;
				}
			}

			if(!fadeIn)
			{
				switch(_stage)
				{
				case Stage.In:
					progress = 1f;
					break;
				}
			}

			GUI.depth = 0;
			Color old = GUI.color;
			GUI.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, progress);
			GUI.DrawTexture(_rect, _texture);
			GUI.color = old;
		}
	}
}