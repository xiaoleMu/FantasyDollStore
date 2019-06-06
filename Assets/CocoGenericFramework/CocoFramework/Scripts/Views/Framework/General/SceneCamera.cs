using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class SceneCamera : GameView
	{
		/// <summary>
		/// The background object is a sprite which forms the entire 
		/// scene - we need it in order to understand what is the scene's
		/// "depth".
		/// </summary>
		public GameObject background;

		/// <summary>
		/// If this was an orthographic camera, what would have been its size?
		/// Reminder: size here means half the height of the screen.
		/// </summary>
		public float orthographicSize;

		public Vector3 normal = new Vector3(0, 0, 1);

		void Update ()
		{
			UpdateCamera();
		}

		private void UpdateCamera()
		{
			//what is the point to look at?
			Vector3 targetPoint = (background != null ? background.transform.position : Vector3.zero);

			//what is the normal of the surface?
			//Vector3 normal = new Vector3(0, 0, 1);
			if(background != null)
				normal = background.transform.rotation * normal;

			Vector2 orthoSize = new Vector2(GetComponent<Camera>().aspect * orthographicSize, orthographicSize);
			float d = Mathf.Max(orthoSize.x / 2.0f * Mathf.Tan(Mathf.Deg2Rad * GetComponent<Camera>().fieldOfView), 
			                    orthoSize.y / 2.0f * Mathf.Tan(Mathf.Deg2Rad * GetComponent<Camera>().fieldOfView));

			GetComponent<Camera>().transform.position = targetPoint - normal * d;
		}
	}
}
