using UnityEngine;
using System.Collections;

namespace TabTale {

	public class CaptureCamera
	{
		private Camera _camera;
		private RenderTexture _texture;

		public Texture Texture
		{
			get { return _texture; }
		}

		public CaptureCamera(Camera original)
		{
			_camera = CameraUtils.CloneCamera(original);
			_camera.enabled = false;
			_texture = new RenderTexture(Screen.width, Screen.height, 24);
			_camera.targetTexture = _texture;
		}

		public CaptureCamera()
			: this(Camera.main)
		{
		}

		public void Capture()
		{
			_camera.Render();
		}
	}
}
